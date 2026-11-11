# Backlog

Out of scope for 1.0.0, to do later or when the pitch from Alexandre asks for it.

- Cover score for impostors beyond the simple rules (obey, refuse only an exposing order).
- Icons for the eleven roles on the role card.
- Space outside the hull instead of ground, dedicated models for engines, reactors, consoles and tanks, dedicated crew suit texture.
- Power network so the vanilla machines of the ship work.
- Anything the pitch from Alexandre changes: tasks, sabotages, kills, vote, win conditions, role powers.

## 1.1.0: game modes (Alexandre, 2026-11-11)

Choose the mode at start: solo (what 1.0.0 does), or multiplayer, either random (matchmaking) or private (invitation only).

- Research first: how existing mods do multiplayer and matchmaking (the community Multiplayer mod, any lobby or server browser). Corpus search only through `scripts/Search-Workshop.sh`, asked to the owner first (about 5 minutes).
- Make roles, role screens, votes, kills and tasks work with one human per pawn: synchronised state, simultaneous votes, shared random numbers.
- Private mode: invited players only. Random mode: needs a matchmaking route that the research must find.

### Multiplayer flow (Alexandre, 2026-11-11)

- Two choices: create a game or join one. Joining matters once there are several maps.
- Lobby ("salon"): players waiting before the start, host settings (private or random, number of impostors).
- Skin: each player picks a suit colour. 1.0.0 already gives one colour per pawn with no duplicate.
- Open points to design with Alexandre: lobby rules, skins, map choice, what happens when a player leaves.

## Art to produce (placeholders today)

Winged guardian ghost and plain ghost look, ejection animation frames (engine fuel), role icons, crew suit, task and repair stations, emergency button, security and admin consoles. Prompts are given to the owner in RimWorld style when needed.
