# Changelog

## 0.1.0 — 2026-08-03

- Initial release for **SPT 4.0.13**
- `GET /modinventory/api/manifest` — host mod inventory with per-file `sha256` / size
- `GET /modinventory/api/file?path=` — download allowlisted files from the host game root
- Configurable scan roots; profiles / logs / `.pdb` never served
