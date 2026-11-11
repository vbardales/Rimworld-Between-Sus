# Changelog

## [Unreleased]

- Teams and roles: `Team`, `RoleDef` (with abilities), `GameComponent_Roles` (assignment, main pawn, save/load), roles `BS_Crewmate_Innocent` and `BS_Impostor_Impostor`.
- Scenario "Between Sus": 8 starting pawns standing on the ship, roles drawn at start, two reveal screens (team, then small role) for the main pawn.
- Crew suit `BS_CrewSuit`: same suit for everyone, one colour per pawn, locked on the pawn (placeholder texture).
- Order refusal: a pawn may refuse a player order (Harmony patch on `TryTakeOrderedJob`); impostors keep their cover.
- Ship map: 17 rooms and corridors built by a map generator step, player start in the cafeteria.
