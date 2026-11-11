# Between Sus: design

Requested by Alexandre. Social-deduction mod for RimWorld 1.6 (C#, Harmony), inspired by Among Us. Never use the name "Among Us" or its assets in the mod title or art.

## Technical base

- Folder `BetweenSus/`: `Source/` (net48, Krafs.Rimworld.Ref 1.6.4871, Lib.Harmony 2.4.3), `Mod/` (About, Assemblies, Defs).
- packageId `nelim.betweensus`. Build: `dotnet build -c Release` in `Source/`, output in `Mod/Assemblies/`, intermediates in `.build/`.
- Done (2026-11-11): Team, RoleDef with abilities, GameComponent_Roles (main pawn, assignment, refusal chance), two roles, two reveal windows, ScenPart_AssignRoles, BS_CrewSuit with Palette, order refusal patch, ship layout (17 rooms) with GenStep_ShipLayout and Patch_ForceShipMap, scenario BS_BetweenSus. Built, not yet loaded in the game.

## Roles

Two teams (big roles): Crewmate, Impostor. A small role is the post AND the function of a pawn and belongs to one team.

- Crewmate: innocent (done), engineer, scientist, tracker (follows a pawn on the map), necromancer (hears the bodies of the dead).
- Impostor: impostor (done), shapeshifter, phantom, acid, ninja, pro.
- Role card (screen 2): icon, title, subtitle, 2 to 3 abilities with icon and text, one tip. `RoleDef` needs an `abilities` list (title, text, icon). Border cyan for crewmates, red for impostors.

## Start of the scenario

- Map: custom scenario, the ship replaces the normal start map.
- Two screens in a row, for the main pawn only: screen 1 = team (crewmate: crowd of 8 to 10 pawns and "there are N impostors among us"; impostor: only the fellow impostors, rest as silhouettes), screen 2 = small role card.
- All pawns wear the same suit `BS_CrewSuit` (forced apparel, one Among Us style colour per pawn, no duplicate). The suit never reveals a role.

## Control model (decided 2026-11-11)

- The player has a main pawn (`mainPawn`) and knows only its role.
- The player controls every other pawn as usual, but cannot see their roles, and they may refuse an order (floating text, no reason shown).
- Crewmate: obeys almost always, small refusal chance tied to mood.
- Impostor: has a cover. Obeys normal orders, including repairing a sabotage (even its own) to look innocent. Refuses only orders that would expose it (attacking or accusing another impostor, staying under a camera while preparing a kill). Chance to repair its own sabotage rises when observed (a crewmate in the room, a camera on the room). Kills only when unobserved.
- Needs a per-impostor cover score. Refusals are not a reliable clue, the player must deduce from actions (movements, absences, deaths).
- Planned implementation: Harmony patches on player-forced jobs and drafted orders.

## Ship map (14 rooms)

Cafeteria (emergency button, starts a vote), weapons, navigation, O2, shields, communications, storage, admin (map table, control post), electrical, security (cameras), medbay, upper engine, lower engine, rear reactor, front reactor. Name the two reactors "rear reactor" and "front reactor" in the defs. Mockups generated 2026-11-10 (map, crewmate screen, impostor screen, engineer card, two pawn sprites).

## Next milestone

1. `mainPawn` in `GameComponent_Roles`, role screens only for it.
2. Order refusal patch with the cover rule above.
3. `BS_CrewSuit` and colour palette.
4. Scenario def, ship map generator, the two start screens.
5. Remaining small roles, one at a time.
