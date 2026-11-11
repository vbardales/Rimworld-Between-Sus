# Changelog

## [Unreleased]

- Teams and roles: `Team`, `RoleDef` (with abilities), `GameComponent_Roles` (assignment, main pawn, save/load), roles `BS_Crewmate_Innocent` and `BS_Impostor_Impostor`.
- Scenario "Between Sus": 8 starting pawns standing on the ship, roles drawn at start, two reveal screens (team, then small role) for the main pawn.
- Crew suit `BS_CrewSuit`: same suit for everyone, one colour per pawn, locked on the pawn (placeholder texture).
- Order refusal: a pawn may refuse a player order (Harmony patch on `TryTakeOrderedJob`); impostors keep their cover.
- Ship map: 17 rooms and corridors built by a map generator step, player start in the cafeteria.
- Ship furniture: tables and chairs, shelves, beds, consoles, plants and machines in each room, glowing ship lights that need no power, and the emergency button in the cafeteria (placeholder art).
- Game loop: about 20 task stations (crewmates complete them, impostors fake them), reactor and O2 sabotages with a timer and repair stations, kills when unobserved, body reports, emergency button, meeting with a vote and ejection, win conditions.
- Eleven small roles: innocent, engineer, scientist, tracker, necromancer (crew); impostor, shapeshifter, phantom, acid, ninja, pro (impostors).
- Security console (cameras) and admin console (head count per room).
- Rules from the pitch: the crew wins only by eliminating every impostor (vote or evidence), impostors win when at most one crewmate is left; no crew win during a sabotage except doors.
- Door sabotage, evidence (fake tasks and kills seen by crewmates), ejection into an engine with a flash and a sound, ghosts (task ghost or winged guardian ghost), vote with an argument, colonists pressing the emergency button.
- Art from the owner: icons for the eleven roles (shown on the role card), textures for the task, repair and emergency stations and the two consoles.
