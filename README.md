# Between Sus

A social-deduction mod for RimWorld, inspired by the party game of the same genre. You start on a spaceship map. Each colonist gets a secret team (crewmate or impostor) and a secret role. You know only your own role, you command everyone else as usual, and they may refuse an order.

Work in progress. First milestone: two teams and two roles (innocent crewmate, impostor). See `Docs/DESIGN.md` for the full plan.

## Requirements

- RimWorld, latest version
- Harmony

## Build

```bash
cd Source
dotnet build -c Release
```

The assembly lands in `Mod/Assemblies/`.

## Credits

See `ATTRIBUTION.md`.
