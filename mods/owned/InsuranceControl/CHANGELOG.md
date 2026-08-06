# Changelog

## 1.1.0

- Merged **Insure All (Prapor)** into Insurance Control as a client+server package
- Client plugin `InsuranceControl.Client.dll` with F12 settings:
  - Enable / label / insurer (Prapor or Therapist)
  - Button offset, gap, size, font size
  - Verbose logging
- Shared GUID `gadjed.insurancerefund` for client + server; marks `gadjed.insureallprapor` incompatible
- Server `config.json`: default `DebugReturnSeconds` set to `0`

## 1.0.2

- Insurance Refund server: content enrichment + pre-raid snapshot

## 1.0.1 / 1.0.0

- Initial Insurance Refund releases
