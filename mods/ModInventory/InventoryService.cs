using System.Security.Cryptography;
using System.Text.Json.Serialization;
using SPTarkov.DI.Annotations;
using Path = System.IO.Path;

namespace ModInventory;

[Injectable(InjectionType.Singleton)]
public class InventoryService
{
    private ModConfig _config = new();

    public string GameRoot { get; private set; } = "";

    public void Initialize(string modFolder, ModConfig config)
    {
        _config = config;
        GameRoot = ResolveGameRoot(modFolder);
    }

    public ModsManifestDto BuildManifest()
    {
        EnsureReady();
        var mods = new Dictionary<string, ModEntryDto>(StringComparer.OrdinalIgnoreCase);

        foreach (var rootRel in _config.ScanRoots)
        {
            var absoluteRoot = Path.Combine(GameRoot, rootRel.Replace('/', Path.DirectorySeparatorChar));
            if (!Directory.Exists(absoluteRoot))
            {
                continue;
            }

            foreach (var file in Directory.EnumerateFiles(absoluteRoot, "*", SearchOption.AllDirectories))
            {
                if (ShouldIgnore(file))
                {
                    continue;
                }

                var rel = NormalizeRel(Path.GetRelativePath(GameRoot, file));
                var modId = InferModId(rootRel, rel);
                if (IsExcludedMod(modId))
                {
                    continue;
                }

                if (!mods.TryGetValue(modId, out var entry))
                {
                    entry = new ModEntryDto { Id = modId };
                    mods[modId] = entry;
                }

                var info = new FileInfo(file);
                entry.Files.Add(new ManifestFileDto
                {
                    Path = rel,
                    Sha256 = Sha256File(file),
                    Size = info.Length,
                });
            }
        }

        foreach (var entry in mods.Values)
        {
            entry.Files.Sort((a, b) => string.Compare(a.Path, b.Path, StringComparison.OrdinalIgnoreCase));
        }

        return new ModsManifestDto
        {
            Spt = "4.0.13",
            Revision = DateTime.UtcNow.ToString("o"),
            GameRootHint = GameRoot,
            Mods = mods.Values.OrderBy(m => m.Id, StringComparer.OrdinalIgnoreCase).ToList(),
        };
    }

    public (bool Ok, string? AbsolutePath, string? Error) ResolveAllowedFile(string relativePath)
    {
        EnsureReady();
        var rel = NormalizeRel(relativePath);
        if (rel.Contains("..", StringComparison.Ordinal) || Path.IsPathRooted(relativePath))
        {
            return (false, null, "Invalid path.");
        }

        if (!_config.ScanRoots.Any(root =>
                rel.StartsWith(root.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(rel, root.TrimEnd('/'), StringComparison.OrdinalIgnoreCase)))
        {
            return (false, null, "Path outside allowlist.");
        }

        if (ShouldIgnore(rel))
        {
            return (false, null, "Path ignored.");
        }

        var absolute = Path.GetFullPath(Path.Combine(GameRoot, rel.Replace('/', Path.DirectorySeparatorChar)));
        var rootFull = Path.GetFullPath(GameRoot);
        if (!absolute.StartsWith(rootFull, StringComparison.OrdinalIgnoreCase))
        {
            return (false, null, "Path escaped game root.");
        }

        if (!File.Exists(absolute))
        {
            return (false, null, "File not found.");
        }

        return (true, absolute, null);
    }

    private void EnsureReady()
    {
        if (string.IsNullOrEmpty(GameRoot))
        {
            throw new InvalidOperationException("ModInventory is not initialized.");
        }
    }

    private bool IsExcludedMod(string modId) =>
        _config.ExcludeModFolders.Any(x => string.Equals(x, modId, StringComparison.OrdinalIgnoreCase));

    private static string InferModId(string scanRoot, string relativePath)
    {
        var prefix = scanRoot.TrimEnd('/') + "/";
        var under = relativePath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? relativePath[prefix.Length..]
            : relativePath;

        var first = under.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
        if (string.IsNullOrEmpty(first))
        {
            return Path.GetFileNameWithoutExtension(relativePath);
        }

        // Loose DLL directly in plugins/
        if (first.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileNameWithoutExtension(first);
        }

        return first;
    }

    private static string ResolveGameRoot(string modFolder)
    {
        var full = Path.GetFullPath(modFolder);
        // .../SPT/user/mods/ModInventory
        var sptUserMods = Path.GetDirectoryName(full); // mods
        var sptUser = Path.GetDirectoryName(sptUserMods); // user
        var spt = Path.GetDirectoryName(sptUser); // SPT
        if (!string.IsNullOrEmpty(spt) &&
            string.Equals(Path.GetFileName(spt), "SPT", StringComparison.OrdinalIgnoreCase))
        {
            var parent = Path.GetDirectoryName(spt);
            if (!string.IsNullOrEmpty(parent) && LooksLikeGameRoot(parent))
            {
                return parent;
            }
        }

        // .../user/mods/ModInventory (flat)
        if (!string.IsNullOrEmpty(spt) && LooksLikeGameRoot(spt))
        {
            return spt;
        }

        var cwd = Directory.GetCurrentDirectory();
        if (LooksLikeGameRoot(cwd))
        {
            return cwd;
        }

        var cwdParent = Path.GetDirectoryName(cwd);
        if (!string.IsNullOrEmpty(cwdParent) && LooksLikeGameRoot(cwdParent))
        {
            return cwdParent;
        }

        return cwd;
    }

    private static bool LooksLikeGameRoot(string path) =>
        Directory.Exists(Path.Combine(path, "BepInEx"))
        || File.Exists(Path.Combine(path, "EscapeFromTarkov.exe"))
        || File.Exists(Path.Combine(path, "SPT.Server.exe"));

    private static bool ShouldIgnore(string path)
    {
        var name = Path.GetFileName(path);
        if (name.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("thumbs.db", StringComparison.OrdinalIgnoreCase) ||
            name.Equals(".ds_store", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var n = path.Replace('\\', '/');
        return n.Contains("/profiles/", StringComparison.OrdinalIgnoreCase)
               || n.Contains("/logs/", StringComparison.OrdinalIgnoreCase)
               || n.EndsWith(".log", StringComparison.OrdinalIgnoreCase)
               || n.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRel(string path) =>
        path.Replace('\\', '/').TrimStart('/');

    private static string Sha256File(string absolutePath)
    {
        using var stream = File.OpenRead(absolutePath);
        var hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

public sealed class ModsManifestDto
{
    [JsonPropertyName("spt")]
    public string Spt { get; set; } = "4.0.13";

    [JsonPropertyName("revision")]
    public string Revision { get; set; } = "";

    [JsonPropertyName("gameRootHint")]
    public string? GameRootHint { get; set; }

    [JsonPropertyName("mods")]
    public List<ModEntryDto> Mods { get; set; } = [];
}

public sealed class ModEntryDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("files")]
    public List<ManifestFileDto> Files { get; set; } = [];
}

public sealed class ManifestFileDto
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = "";

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; } = "";

    [JsonPropertyName("size")]
    public long Size { get; set; }
}
