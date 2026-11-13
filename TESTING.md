# Testing

Stage `writeTests[1.0.0]`. Nothing here was played in RimWorld yet except the reference capture of ticket 00b0 (the game loads, the crew suits show). Every scenario below waits for `playTests`. Numbers were read from the code on 2026-11-13 (`Source/GameComponent_Ship.cs`, `Source/ShipFurnisher.cs`, `Source/ShipLayout.cs`, `Mod/Defs`); `Tests/Rule-OwnValues.ps1` fails when one of them changes. `PUBLICATION.md` does not exist yet, so the promised behaviors come from `Docs/DESIGN.md` and the in-game texts.

Ticks: 60 ticks = 1 second at normal speed; a day = 60000 ticks. The game logic runs once every 250 ticks (`GameComponent_Ship.GameComponentTick`).

## Automated offline checks

Run from the protocols root (`Rimworld/`), no game started.

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Run-OfflineChecks.ps1 -ModRoot BetweenSus -SourceDirs BetweenSus/Source -ExtraAssemblies BetweenSus/Mod/Assemblies/BetweenSus.dll -Brief
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Check-HarmonyContract.ps1 -ModRoot BetweenSus -MinPatches 3 -Hooks 'Pawn.GetGizmos:','Pawn_JobTracker.TryTakeOrderedJob:Job','MapGenerator.GenerateMap'
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Check-Steps.ps1 -ModRoot BetweenSus
powershell -NoProfile -ExecutionPolicy Bypass -File scripts/Check-PickleCompanion.ps1 -ModRoot BetweenSus
powershell -NoProfile -ExecutionPolicy Bypass -File BetweenSus/Tests/Rule-OwnValues.ps1 -ModRoot BetweenSus
```

The generated `Tests/Test-Mod.ps1` and `.github` do not exist yet: CI/CD generates them after the prepublication `0.1.0` (item exists, `About/PublishedFileId.txt` committed) and once the junction `Mod/` to `RimWorld/Mods/` is mounted. Until then the commands above are run by hand and the result is written in `STATUS.md`.

| Check | What it proves | Does not prove |
|---|---|---|
| `Run-OfflineChecks` (ModBasics, Translations, XmlFields, DefRefs, TypeRefs, ConfigErrors, DefInjected, Textures, PatchTargets, SettingsContract) | XML parses, every def reference and C# type in the XML exists, French and English keys match, every Keyed call in `Source/` has a key, settings contract is `not_applicable` as recorded | that RimWorld accepts the defs, that a list index such as `abilities.0.label` reaches the field (read as proof by the game rules, checked in game) |
| `Check-HarmonyContract` | the three Harmony targets (`Pawn.GetGizmos`, `Pawn_JobTracker.TryTakeOrderedJob`, `MapGenerator.GenerateMap`) exist in the game DLL with one overload; injected parameters are valid; `PatchAll` is called once under one id | that the patches behave in play |
| `Check-Steps`, `Check-PickleCompanion` | the Pickle feature lines each match exactly one step; the companion mod and its pass maps are well formed | that a scenario passes |
| `Tests/Rule-OwnValues.ps1` | about and Harmony dependency; 11 roles (5 crewmates, 6 impostors, the two plain roles weigh 3); the cooldown, evidence and sabotage numbers of this file; scenario of 8 colonists and 2 impostors; the props the furnisher places; 17 rooms apart, 21 doors each between two rooms, ship connected from the Cafeteria, a `BS_Room_*` text in English and French | that doors open, that pawns path, that the furnisher found free cells |

Known, accepted: `Check-ModBasics` fails on `Mod/About/Preview.png is missing`. The preview is made at `mountPreview`, after this stage. `Check-XmlClasses` is skipped (no game type list).

C# unit tests (`Run-UnitTests.ps1`) are not applicable: every rule runs on live `Pawn`, `Map` and `Find` objects of the game; a console test would build fakes and prove the fakes. What a running game shows is in the Pickle suite and in the scenarios below.

## Pickle suite (written, not run)

`Tests/Pickle/` holds the companion mod and three features, see `Tests/Pickle/README.md`. Evidence lands in `Tests/Pickle/Inbox/`.

| Feature | Scenarios it plays |
|---|---|
| `01-reference-capture` | S01 (captures, owner looks) |
| `02-boot` | S01, S02, S27 |
| `03-emergency-button` | S13 (the refused press at the start only) |

Everything that needs a chosen role (kill, sabotage, abilities) stays manual: the role of the main pawn is drawn at random and no Pickle step forces it.

## Functional scenarios

All scenarios start a new colony on the scenario **Between Sus** (8 colonists, map size at least 97 x 49; 120 recommended). `S` = scenario. "Main pawn" = the colonist whose role the player is shown. Developer mode may be used to force a role or advance time, and is then noted in the run.

### Start and roles

**S01. The scenario starts on the ship.**
Pre: RimWorld 1.6 with Harmony and Between Sus.
Actions: Scenario list, pick Between Sus, keep 8 colonists, start.
Expected: the map is the ship (95 x 47 cells), 17 rooms and corridors with walls, 21 doors and ship lights (no power needed); the colonists stand in the ship, all in the same crew suit with a different colour each; the cafeteria holds an emergency button; the suits cannot be taken off. The scenario text reads "Between Sus" and its summary "A crew on a ship, and impostors hiding among them."

**S02. Team screen, then role screen, for the main pawn only.**
Pre: S01 just started.
Actions: Read the first screen, click Continue; read the second screen, click Understood.
Expected: screen 1 names the team. Crewmate: border cyan, the whole crew shown, text "There are 2 impostors among us". Impostor: border red, only the fellow impostors shown (the main pawn and the other one), text "Eliminate the crew without being caught". Screen 2 shows the role icon, title, description, each ability and the tip; same border colours. The game is paused while a screen is open. Repeat on at least three new colonies to see both teams.

**S03. Teams and roles are drawn.**
Pre: S01.
Actions: Start ten new colonies, read the end-of-game letter (S22 or S23) or use developer mode to inspect roles.
Expected: each colony has exactly 2 impostors among 8 pawns (impostor count is capped at a quarter of the colonists, here 2); every pawn has one of the 11 roles of its team; the plain roles (Innocent crewmate, Impostor) come up more often than the others (weight 3 against 1). The role of any pawn other than the main pawn is never shown during play.

**S04. Colonists may refuse an order; the main pawn never does.**
Pre: S01, game running, a crewmate colonist (not the main pawn) with an average mood.
Actions: Give the same order (move to a cell) 50 times.
Expected: roughly 4 % of the orders are refused, with the floating text "Refuses" and no reason; the colonist keeps its previous job. At mood 0 the chance rises to about 24 %. The main pawn obeys every order. An impostor refuses about 2 % of the orders, and about 90 % of the orders to attack a fellow impostor.

### Tasks and evidence

**S05. A crewmate completes a task once.**
Pre: S01, a crewmate colonist, work type "Tasks" active (it is set to priority 1 at the start).
Actions: Let the colonist work, or order it to the task station.
Expected: the colonist walks to a task station and works 300 ticks (5 s; an engineer 150 ticks); a message "Task done (N of M)." shows the count; the station is then unavailable for good. There are at most 19 task stations (18 placements in the code, one of them in the shared case of both reactor rooms; a placement that collides is skipped), "about 20" in the design. Fix stations take no part in this count.

**S06. An impostor only fakes a task, and a crewmate who sees it gains evidence.**
Pre: S01, an impostor colonist (not the main pawn) and a crewmate within 14 cells with line of sight.
Actions: Let the impostor work at a task station.
Expected: after 300 ticks the station stays not done and is unavailable to that impostor for 2500 ticks, then available again to everybody. If at least one crewmate saw it, the impostor gets 1 evidence; none seen, none gained. The progress message does not move.

**S07. Evidence of 3 lets the crew eliminate an impostor without a vote.**
Pre: an impostor with 3 evidence (three seen fake tasks, or one witnessed kill: each witness gives 3).
Actions: Wait; if the main pawn is a crewmate, watch for the message and the gizmo.
Expected: if the main pawn is a crewmate, the message "The crew has enough evidence against X. X can be eliminated." appears and the main pawn gets an "Eliminate" gizmo listing the accusable impostors. At every logic tick (250 ticks) the crew also eliminates each accusable impostor on its own with a chance of 0.5. Elimination follows S16 with the cause "X was eliminated based on the evidence."

### Kills

**S08. The impostor's Kill gizmo.**
Pre: the main pawn is an impostor; a crewmate stands next to it.
Actions: Select the main pawn, click Kill; repeat with no crewmate near; repeat before the cooldown ends.
Expected: the first kill is possible 12000 ticks after the start; before that "Your blade is not ready yet." Only a crewmate within 2.5 cells is hit (the nearest); none in range gives "Nobody is within reach." A kill gives the message "You killed X." and starts a cooldown of 15000 ticks (ninja 7500, pro 3750); the body lies on the map. The gizmo does not check witnesses: the player decides.

**S09. A witnessed kill is evidence.**
Pre: S08 with k other crewmates within 14 cells and in line of sight of the impostor (not the victim).
Actions: Kill.
Expected: the impostor gets 3 evidence per witness. The Necromancer main pawn, if any, gets "You hear the body of X in room." with the room name in the game language.

**S10. AI impostors hunt on their own.**
Pre: S01 running, the main pawn is a crewmate, impostors not the main pawn.
Actions: Let time run for two days.
Expected: an impostor walks to the nearest crewmate and kills it only when nobody sees it and it is within 2.5 cells; the first kill comes after 12000 ticks. Kill cooldowns follow S08. The game never kills a pawn the impostor is not close to.

**S11. Role abilities.**
Pre: force a role with developer mode (the role of a pawn is not otherwise visible), main pawn.
Actions and expected, one per role:
- Innocent crewmate: no gizmo besides Eliminate (S07).
- Engineer: tasks 150 ticks, repairs 225 ticks (half of 300 and 450).
- Scientist: gizmo "Vitals" opens a dialog "Still alive: names." listing the living colonists.
- Tracker: gizmo "Track" lists the other living colonists; after choosing one, the message "You are tracking X." shows and about every 2500 ticks for one day (60000 ticks) the message "X is in room." is shown.
- Necromancer: see S09.
- Impostor: Kill and Sabotage gizmos.
- Shapeshifter: gizmo "Shapeshift"; the suit takes the colour of one random other colonist for one day (60000 ticks) and the message "You look like X now." is shown; a second use during the day does nothing; the colour returns after 60000 ticks.
- Phantom: gizmo "Vanish": for 900 ticks (15 s) the kills of this impostor are not seen by AI rules and give no witness evidence of the "observed" kind (the manual Kill gizmo ignores witnesses anyway).
- Acid: the body of a victim disappears at once and cannot be reported.
- Ninja: kill cooldown 7500. Pro: kill cooldown 3750; own sabotages leave 1800 ticks more.

**S12. A guardian ghost shields one colonist from the next kill.**
Pre: the main pawn was ejected and became a guardian ghost (S16, 50 % chance).
Actions: Click Protect, choose a living colonist; an impostor then kills that colonist.
Expected: the message "You shield X from the next kill." shows; the next kill of X fails with "A guardian shielded the target: the kill failed.", the killer's cooldown still starts, X lives, and the shield is gone.

### Meetings and votes

**S13. The emergency button.**
Pre: S01, the cafeteria button.
Actions: Select the button, click "Call a meeting" (a) in the first 2500 ticks, (b) after that, (c) during a sabotage, (d) within 8000 ticks of the end of a meeting.
Expected: (a) and (d) refuse with "The button is not ready yet."; (c) refuses likewise (button off during any sabotage); (b) opens the meeting window titled "X pressed the emergency button" or "Emergency meeting" and teleports every living colonist within 10 cells of the cafeteria centre. Colonists may press the button on their own with a chance of 0.02 at each logic tick when it is ready.

**S14. A body is reported.**
Pre: a body of a colonist lies on the map, no sabotage running.
Actions: A living crewmate comes within 10 cells with line of sight.
Expected: a meeting opens titled "X found the body of Y"; each body is reported once. The same body does not open a second meeting. No body is reported during a sabotage; the report follows the end of the sabotage.

**S15. Vote, argument, skip.**
Pre: a meeting is open, the main pawn alive.
Actions: (a) Vote for a colonist, write an argument, Confirm. (b) Vote then Back. (c) Skip. (d) Main pawn dead or a ghost.
Expected: (a) the message `You accuse X: "..."` shows and X becomes more suspicious for the AI (weight +2); the AI votes (a crewmate skips 20 % of the time, an impostor about 44 % and never votes for an impostor; the target is drawn with a weight of 1 plus suspicion); the plurality is ejected only when it beats the skips and the second place, a tie or a skip majority ejects nobody ("Nobody was ejected."). (b) returns to the list without voting. (c) closes and resolves with no player vote. (d) the window is a spectator view ("You can only watch the vote.") with a Continue button. The window cannot be closed otherwise, and the game is paused. After the vote all colonists' bodies are removed, and the next meeting is blocked for 8000 ticks.

**S16. Ejection.**
Pre: a vote or an evidence elimination picks X.
Actions: Watch the result.
Expected: X is moved to the centre of the upper or lower engine, with a flash, smoke and a sound (a good one for an impostor, a bad one for a crewmate); a floating text says "X was an impostor." (green) or "X was not an impostor." (red); a letter "X was ejected by vote" or "X was eliminated based on the evidence" gives the same text and "Impostors left: N". X leaves the map. If X is the main pawn it stays as a ghost instead, with the message "You are a guardian ghost now..." or "You are a ghost now. You can only do tasks." (50 % each).

### Sabotage

**S17. Reactor sabotage.**
Pre: the main pawn is an impostor, or wait for an AI impostor; at least 15000 ticks since the start.
Actions: Sabotage, Reactor meltdown.
Expected: a letter "Reactor meltdown" says "Sabotage! Repair the stations in: rooms. About 90 seconds left." naming the two repair stations (rear and front reactor) (5400 ticks; pro: 7200). Both repair stations become available and take 450 ticks (engineer 225) each. Both repaired in time: "The sabotage has been repaired.", sabotage cooldown 30000 ticks. Not repaired: the impostors win (S23) with "The sabotage was not repaired in time." The button and body reports are off meanwhile; the crew cannot win meanwhile.

**S18. O2 sabotage.**
Pre: as S17.
Actions: Sabotage, O2 depletion.
Expected: as S17 with 7200 ticks (letter "About 120 seconds left."; pro: 9000) and the repair stations in the O2 room and in Admin.

**S19. Door sabotage.**
Pre: as S17.
Actions: Sabotage, Door lockdown.
Expected: every door is forbidden for 1800 ticks (30 s) and a letter says so; no repair station; no meeting meanwhile; then the doors open and the message "The sabotage has been repaired." shows, cooldown 30000 ticks. The crew may still win during a door sabotage.

**S20. A second sabotage too early.**
Pre: a sabotage is running or its cooldown not over.
Actions: Sabotage again.
Expected: "No sabotage possible right now." The first sabotage is possible 15000 ticks after the start.

### Consoles

**S21. Security and admin consoles.**
Pre: S01.
Actions: Select the security console (security room), click Cameras; select the admin console (admin room), click Ship map.
Expected: Cameras opens a dialog listing, for each room that holds a colonist, "Room: names"; Ship map lists "Room: count". Room names are in the game language. An empty ship gives "Nobody is in any room."

### Ghosts, win, save, languages

**S22. The crew wins.**
Pre: all impostors ejected or eliminated; no reactor or O2 sabotage running.
Actions: Finish the last ejection.
Expected: the game pauses with a dialog and letter "The crew wins", "Every impostor is gone.", then each pawn with its role. During a reactor or O2 sabotage the same state does not end the game until it is fixed.

**S23. The impostors win.**
Pre: living impostors and at most one living crewmate, or a reactor or O2 sabotage that ran out.
Actions: Let the last kill or the timer pass.
Expected: the game pauses with "The impostors win" and the reason "The impostors are as many as the crew." or "The sabotage was not repaired in time.", then each pawn with its role.

**S24. A ghost only does tasks.**
Pre: the main pawn is a ghost (S16).
Actions: Order it to attack, haul or any job; order it to walk; let it do a task.
Expected: attack or other jobs are refused with the text "A ghost can only do tasks."; moving and tasks work; it takes no part in votes or win counts (spectator view, S15 d).

**S25. Save and load.**
Pre: a game with a running sabotage, a ghost, a shapeshifted pawn and some kill cooldowns.
Actions: Save, reload.
Expected: roles, main pawn, end state, sabotage kind and end tick, cooldowns, ghosts, locked doors and fixed stations persist. Evidence, suspicion, vanish and the tracked pawn are not saved (they reset), see "Known risks".

**S26. French.**
Pre: language French.
Actions: Play S02, S05, S13, S15, S16, S21.
Expected: no raw key (`BS_...`), every text in French, rooms in French, role cards in French including abilities (`abilities.N.label`), the scenario summary, the work type and job reports of the task stations. The scenario label stays "Between Sus". The title in the mod list stays English for now (`Entre Sus(pects)` is decided but not applied to About).

**S27. No error is logged.**
Pre: S01 to S26 played in one session.
Actions: Read `Player.log` after each scenario.
Expected: no red error and no exception mentioning Between Sus or Harmony. The lines "did not load any content" from test mods are harmless.

## Known risks found while reading the code

To check at `playTests`; none is fixed yet (this stage writes tests, design choices are Virginie's and Alexandre's).

1. **Evidence gizmo window is short.** S07: the crew eliminates an accusable impostor by itself with a chance of 0.5 every 250 ticks, even when the main pawn is a crewmate. The main pawn's "Eliminate" gizmo exists only between the third evidence and that automatic elimination, about 500 ticks on average. The design says "a gizmo for the main pawn, automatic for the others".
2. **Shapeshift colour and a save.** S11, S25: the borrowed colour is restored from `shifted`, which is not saved. After a reload the suit may keep the borrowed colour for good.
3. **Kill range.** `Docs/DESIGN.md` says 2 cells; the code accepts 2.5 (gizmo and AI).
4. **About text.** `Mod/About/About.xml` still says "First milestone: two teams and two roles" (11 roles exist). Belongs to `writeDocs`.
5. **Placeholder art.** Props use vanilla textures; ghost layers are not drawn (`STATUS.md`, `remaining`).
