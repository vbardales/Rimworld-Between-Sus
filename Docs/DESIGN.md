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
- Madness crises (Alexandre, 2026-11-13): they hit every pawn, main pawn included, and are driven by mood (low mood triggers them). A crisis makes the pawn hit the walls. Open, still unanswered: threshold and duration, real damage or visual only, whether a crisis exposes an impostor or counts as evidence, whether a crisis interrupts a meeting, a vote or a task.

## Gameplay (Alexandre's answers, 2026-11-11; the rest are my defaults and can change)

- **Tasks:** about 20 task stations. A crewmate completes one for good, an impostor only fakes it. Tasks no longer win the game: they give the crew a way to spot impostors (see Evidence) and give ghosts something to do.
- **Evidence:** a fake task seen by a crewmate gives 1 evidence against the impostor, a kill seen by a crewmate gives 3 per witness. At 3 evidence, the crew can eliminate that impostor without a vote (a gizmo for the main pawn, automatic for the others after a short delay).
- **Meeting and vote:** called by a body report or by the emergency button, which the player and the colonists can both press. Everyone is gathered in the cafeteria. The player votes and can write an argument, which makes the others more suspicious of the accused. The colonists vote too, by suspicion. The plurality is ejected, a tie or skip ejects nobody.
- **Ejection:** the pawn is thrown into the fuel of an engine (flash, smoke, sound), a text says whether it was a crewmate or an impostor and how many impostors are left. A good sound for an impostor, a bad one for a crewmate. If the ejected pawn is the main pawn, it becomes a ghost instead of leaving: either a task ghost (only does tasks) or, at random, a winged guardian ghost that also shields one colonist from the next kill.
- **Win, impostors:** only they and at most one crewmate remain, or a reactor or O2 sabotage runs out of time.
- **Win, crew:** every impostor is voted out or eliminated on evidence. The crew cannot win during a sabotage, except a door sabotage.
- **Kill:** an impostor kills a crewmate within 2 cells when nobody else sees it. Cooldown 15000 ticks (ninja half, pro a quarter). Other impostors hunt on their own, the main pawn uses the Kill gizmo.
- **Sabotages:** one at a time, cooldown 30000 ticks. Reactor (5400 ticks, repair the two reactor stations), O2 (7200 ticks, repair the O2 and admin stations), doors (all doors locked for 1800 ticks, no repair). During any sabotage the emergency button is off and bodies cannot be reported.
- **Consoles:** security console (cameras, names per room), admin console (head count per room).

## Ship map (17 rooms and corridors)

Cafeteria (emergency button), weapons, navigation, O2, shields, communications, storage, admin, electrical, security, medbay, upper engine, lower engine, rear reactor, front reactor, two corridors. Placeholder art everywhere (vanilla textures, tinted).

## Not done yet

Dedicated art, space outside the hull, French translation, role icons, cover score for impostors beyond the simple rules above, anything the pitch changes.
