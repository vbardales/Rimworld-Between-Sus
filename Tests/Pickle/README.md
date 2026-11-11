# In-game scenarios, run by Pickle

Played inside a running RimWorld by Pickle (`rimworks.pickle`). `Mod/` is a companion mod, **Between Sus - Pickle tests**, never published: it holds the feature files.

| Feature | Checks |
| --- | --- |
| 01-reference-capture | a new colony on the scenario `BS_BetweenSus` reaches a paused map with no logged error, and two captures (the cafeteria, then the crew close up) |

`wsl-deps.reference.map` stages Harmony and the PickleTools steps the feature needs (new colony, camera zoom, clear screen, screenshot mode). The new colony is not reproducible and takes minutes: one ticket, with `-Extra '-pickle-scenario-timeout=400'`.

Evidence lands in `Tests/Pickle/Inbox/<run>`.
