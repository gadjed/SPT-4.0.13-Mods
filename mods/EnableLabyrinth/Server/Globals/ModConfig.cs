using System.Reflection;
using _enableLabyrinth.Models;
using _enableLabyrinth.Models.Enums;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Utils;

namespace _enableLabyrinth.Globals;

[Injectable(TypePriority = OnLoadOrder.PreSptModLoader)]
public class ModConfig : IOnLoad
{
    public ModConfig(
        ModHelper modHelper,
        JsonUtil jsonUtil,
        FileUtil fileUtil,
        ISptLogger<ModConfig> logger,
        EnableLabyrinth enableLabyrinth)
    {
        _modHelper = modHelper;
        _jsonUtil = jsonUtil;
        _fileUtil = fileUtil;
        _logger = logger;
        _enableLabyrinth = enableLabyrinth;
        _modPath = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
    }
    
    private static ModHelper? _modHelper;
    private static JsonUtil? _jsonUtil;
    private static FileUtil? _fileUtil;
    private static ISptLogger<ModConfig>? _logger;
    private static EnableLabyrinth? _enableLabyrinth;
    
    public static ServerConfig Config {get; private set;} = null!;
    public static ServerConfig OriginalConfig {get; private set;} = null!;
    private static int _isActivelyProcessingFlag = 0;
    public static string _modPath = string.Empty;
    
    public async Task OnLoad()
    {
        Config = await _jsonUtil.DeserializeFromFileAsync<ServerConfig>(_modPath + "/config.json") ?? throw new ArgumentNullException();
        OriginalConfig = DeepClone(Config);
    }
    
    public static async Task<ConfigOperationResult> ReloadConfig()
    {
        if (Interlocked.CompareExchange(ref _isActivelyProcessingFlag, 1, 0) != 0)
            return ConfigOperationResult.ActiveProcess;

        try
        {
            var configPath = Path.Combine(_modPath, "config.json");

            var configTask = _jsonUtil.DeserializeFromFileAsync<ServerConfig>(configPath);
            await Task.WhenAll(configTask);

            Config = configTask.Result ?? throw new ArgumentNullException(nameof(Config));
            OriginalConfig = DeepClone(Config);
            
            await _enableLabyrinth.OnLoad();
            return ConfigOperationResult.Success;
        }
        catch (Exception ex)
        {
            return ConfigOperationResult.Failure;
        }
        finally
        {
            Interlocked.Exchange(ref _isActivelyProcessingFlag, 0);
        }
    }
    
    public static async Task<ConfigOperationResult> SaveConfig()
    {
        if (Interlocked.CompareExchange(ref _isActivelyProcessingFlag, 1, 0) != 0)
            return ConfigOperationResult.ActiveProcess;

        try
        {
            var pathToMod = _modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());
            var configPath = Path.Combine(pathToMod, "config.json");

            var serializedConfigTask = Task.Run(() => _jsonUtil.Serialize(Config, true));
            await Task.WhenAll(serializedConfigTask);

            var writeConfigTask = _fileUtil.WriteFileAsync(configPath, serializedConfigTask.Result!);
            await Task.WhenAll(writeConfigTask);
            
            OriginalConfig = DeepClone(Config);

            await _enableLabyrinth.OnLoad();
            return ConfigOperationResult.Success;
        }
        catch (Exception ex)
        {
            return ConfigOperationResult.Failure;
        }
        finally
        {
            Interlocked.Exchange(ref _isActivelyProcessingFlag, 0);
        }
    }
    
    private static T DeepClone<T>(T source)
    {
        var json = _jsonUtil.Serialize(source);
        return _jsonUtil.Deserialize<T>(json)!;
    }
}