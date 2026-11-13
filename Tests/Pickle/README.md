# In-game scenarios, run by Pickle

Played inside a running RimWorld by Pickle (`rimworks.pickle`). `Mod/` is a companion mod, **Between Sus - Pickle tests**, never published: it holds the feature files. Written at `writeTests`, none run yet (first runs at `playTests`). The scenarios they play are named in `../../TESTING.md`.

| Feature | Checks |
| --- | --- |
| 01-reference-capture | a new colony on the scenario `BS_BetweenSus` reaches a paused map with no logged error, and two captures (the cafeteria, then the crew close up) |
| 02-boot | a new colony starts, the team screen leads to the role screen (button `BS_Continue`, window `Window_RoleReveal`), the load of the mod is clean and no error is logged (TESTING.md S01, S02, S27) |
| 03-emergency-button | the cafeteria button, selected and clicked at the start of the game, is refused with `BS_MeetingCooldown` (S13, first 2500 ticks) |

Kills, sabotages, votes and role abilities stay manual (TESTING.md): the main pawn's role is drawn at random and no Pickle step forces it.

`wsl-deps.reference.map` stages Harmony and the PickleTools steps the reference capture needs (new colony, camera zoom, clear screen, screenshot mode). `wsl-deps.suite.map` is the same plus LoadAudit, for 02 and 03. A new colony is not reproducible and takes minutes: one ticket each, with `-Extra '-pickle-scenario-timeout=400'`.

Evidence lands in `Tests/Pickle/Inbox/<run>` (not committed).
