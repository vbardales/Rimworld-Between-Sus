# Between Sus: design

Requested by Alexandre. Social-deduction mod for RimWorld 1.6 (C#, Harmony), inspired by Among Us. Never use the name "Among Us" or its assets in the mod title or art.

## Technical base

- Folder `BetweenSus/`: `Source/` (net48, Krafs.Rimworld.Ref 1.6.4871, Lib.Harmony 2.4.3), `Mod/` (About, Assemblies, Defs).
- packageId `nelim.betweensus`. Build: `dotnet build -c Release` in `Source/`, output in `Mod/Assemblies/`, intermediates in `.build/`.
- Workflow decision (2026-11-11): develop every feature first, then Pickle tests produce the captures.
- Game modes (Alexandre, 2026-11-11): solo in 1.0.0; multiplayer (random or private by invitation) is planned for 1.1.0, see `Docs/BACKLOG.md`.

## Roles

Two teams (big roles): Crewmate, Impostor. A small role is the post AND the function of a pawn and belongs to one team. Each pawn gets one small role at random, weighted by `RoleDef.weight`.

- Crewmate: innocent, engineer (tasks and repairs twice as fast), scientist (vitals gizmo: who is alive), tracker (follows one pawn, reports its room), necromancer (hears each body, with the room).
- Impostor: impostor (plain), shapeshifter (takes another pawn's suit colour for one day), phantom (invisible to witnesses for a short time), acid (the body dissolves, nothing to report), ninja (half kill cooldown), pro (quarter kill cooldown, faster sabotage).
- Role card (screen 2): title, description, abilities, tip. Border cyan for crewmates, red for impostors.

## Start of the scenario

- Map: custom scenario, the ship replaces the normal start map. 8 starting pawns, 2 impostors.
- Two screens in a row, for the main pawn only: team, then small role.
- All pawns wear the same suit `BS_CrewSuit`, one colour per pawn. The suit never reveals a role.

## Control model

- The player has a main pawn (`mainPawn`) and knows only its role. The player commands every other pawn as usual, cannot see their roles, and they may refuse an order (floating text, no reason shown).
- Crewmate: refuses rarely, more when the mood is low. Impostor: keeps its cover, obeys, refuses only an order that would expose it (attacking a fellow impostor).
- The main pawn never refuses. Its role gizmos appear on it only.

## Gameplay choices (2026-11-11, made without the pitch, easy to change)

- **Tasks:** about 20 task stations around the ship. A crewmate working at one completes it for good (progress bar = done / total). An impostor can work at one as a fake task: nothing counts, it only looks busy. Work type `BS_Tasks`, priority 1 for everyone at start.
- **Win, crew:** every task done, or every impostor ejected or dead.
- **Win, impostors:** living impostors are at least as many as living crewmates, or a sabotage timer runs out.
- **Kill:** an impostor kills a crewmate within 2 cells when nobody else sees it (line of sight, 14 cells). Cooldown 15000 ticks (ninja half, pro a quarter). Other impostors hunt on their own, the main pawn uses the Kill gizmo. The body stays unless the killer is acid.
- **Report:** a crewmate who sees a body within 10 cells calls a meeting.
- **Meeting:** also called by the emergency button in the cafeteria (cooldown). Everyone is teleported to the cafeteria, the player votes (a pawn or skip), the others vote by suspicion (pawns near a kill count). Plurality ejects the pawn (it leaves the map, its team is revealed with the number of impostors left). Tie or skip: nobody leaves.
- **Sabotage:** an impostor (gizmo for the main pawn, automatic for the others) starts one at a time, cooldown 30000 ticks.
  - Reactor: meltdown in 5400 ticks, both reactor stations (rear and front) must be repaired.
  - O2: depletion in 7200 ticks, the O2 and admin stations must be repaired.
  - Timer reaches zero: impostors win.
- **Consoles:** security console lists each room with the pawns in it (cameras), admin console lists each room with a head count.

## Ship map (17 rooms and corridors)

Cafeteria (emergency button), weapons, navigation, O2, shields, communications, storage, admin, electrical, security, medbay, upper engine, lower engine, rear reactor, front reactor, two corridors. Placeholder art everywhere (vanilla textures, tinted).

## Not done yet

Dedicated art, space outside the hull, French translation, role icons, cover score for impostors beyond the simple rules above, anything the pitch changes.
