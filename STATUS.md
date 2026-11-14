---
mod: Between Sus
packageId: nelim.betweensus
repo: Rimworld-Between-Sus
remote: https://github.com/vbardales/Rimworld-Between-Sus.git
visibility: public
detached: yes
workflow_stage: playTests[1.0.0]
licence: original
licence_at: "2026-11-11: original work by Nelim (requested by Alexandre), MIT; no third-party code or asset reused, Harmony and Krafs.Rimworld.Ref only as dependencies"
upstream_mod_remotes: N/A
settings_audit: not_applicable
localization: complete
translation_en: complete
translation_fr: complete
dependencies: declared
workshop: none
workshop_required_items: n/a, not released yet
published: no
tested_on: never
remaining:
  - "2026-11-12: ticket 20261112-002350-786-1e9d (01-reference-capture, commit 5618076) came back exit 0 but its only scenario was SKIPPED (0 steps, 0 ms, no capture); run read and deleted from Inbox (refused). 514d (bundle line = StandaloneBase shell, no steps) failed: Undefined step "the screen is clear". f1fb cancelled; NPT: StandaloneBase is a DLL-less stub, the real bundle is PickleTools/Mod. Refiled as ticket 20261112-113834-896-d948 with `nelim.pickletools path:PickleTools/Mod` in wsl-deps.reference.map: the steps loaded, the run CRASHED (launcher exit 134, no report); evidence read and deleted from Inbox (refused). DEFECT, cause in Player.log: Mod/Defs/Things/CrewSuit.xml:39 `<li Class=\"CompProperties_Colorable\" />` is not a type in 1.6 (ArgumentException: Could not find type named CompProperties_Colorable), so BS_CrewSuit never loads and ScenPart_AssignRoles.DressAll (Source) calls ThingMaker.MakeThing with a null def: NullReferenceException in PostGameStart, map generation fails. FIXED in commit (compClass CompColorable), b954 loaded the game but failed: Log.Error "Tried to change priority on disabled worktype BS_Tasks for pawn Prissy" (ScenPart_AssignRoles.Start); fixed with a WorkTypeIsDisabled guard, refiled as ticket 20261112-123810-337-bfa5; TM read the crash as a SectionLayer_Sand fault, but the sand NREs follow the failed map generation"
  - "2026-11-12: ticket 00b0 (reference-capture) GREEN 1/1 (summary.json passed, 21475 ms): game loads, crew in suits visible at 90 percent, two captures (crew-close, ship-cafeteria) kept in Tests/Pickle/Parked/reference-capture-00b0 (git-ignored, on disk only; exit: delete once the ghost layers are redone from them); Player.log has only the 4 harmless 'did not load any content' notices of the test mods; run deleted from Inbox"
  - "2026-11-12: (history) ticket bfa5 came back red: the BS_Tasks error is gone, the camera step failed (\"to fill 90 percent the camera root size should be 8.333334; it reads 11\"), the game bounds the camera root size at 11; NPT widened the CameraZoom range (PickleTools/CameraZoom/Mod), the feature stays at 90 percent; refiled as ticket 20261112-140657-875-00b0 (commit 342a81a), waiting for RUN_DONE; if it still reads 11, tell NPT the read-back; evidence read and deleted from Inbox (refused)"
  - "2026-11-12: art plan: the ghost layers (Art/Work/ghost-layers.webp) are not wired and must be redone from the real colonist capture of that ticket; crew suit and ejection animation (engine fuel) wait for the same capture; role icons, stations, button and consoles are wired"
  - "2026-11-12: prepublication 0.1.0 item exists (PublishedFileId 3838119558, committed at the owner order)"
  - "2026-11-11: unverified: at localize, translation_fr is not done yet; /review-translation is mandatory before the owner's French review (AUDIT.md 6.g, TM rule of 2026-11-11), translation_fr goes through pending_review"
  - "2026-11-11: dependencies (revision 728bef7): Harmony (brrainz.harmony, Workshop 2009463077) is the only dependency, hard, because the code patches with HarmonyLib; no other mod referenced in code or XML; no DLC required; no optional compat (OptionalCompat.xml not needed, Check-OptionalCompat 0 target); 4.f n/a (no animal); multiplayer mods are for 1.1.0"
  - "2026-11-11: unverified: French translation is for the localize stage, art and later work are in Docs/BACKLOG.md"
  - "2026-11-11: unverified: Alexandre's rules (evidence, door sabotage, ejection, ghosts, vote argument) are built but never run in RimWorld; the ghost has no distinct look yet"
  - "2026-11-11: unverified: tasks, sabotage, kill, meeting, vote, win conditions and the 11 roles are built but never run in RimWorld; Harmony targets Pawn.GetGizmos, Pawn_JobTracker.TryTakeOrderedJob and MapGenerator.GenerateMap unchecked in game"
  - "2026-11-11: owner decision: develop every feature first, then Pickle tests produce the captures; rest of the pitch waits for Alexandre, design choices made meanwhile are in Docs/DESIGN.md and can change"
  - "2026-11-12: /review-translation page 1 of 1 done (ChatGPT answer read): 8 English text fixes applied (scenario, task station, consoles, cameras, ejection, track and necro report); 6 postponed or refused in Docs/review-refusals.md; waiting for Virginie on proper nouns (title French form, Harmony); translation_fr stays pending until the French table exists"
  - "2026-11-12: second /review-translation (after the fixes): one text fix applied (BS_SabotageFixed), nothing else; title French form still waits for Virginie"
  - "2026-11-13: third /review-translation: BS_EvidenceReady fixed (They to {0}); title French form still waits for Virginie, TITLE? sent to Audit"
  - "2026-11-13: fourth /review-translation: BS_EmergencyButton description fixed (of to in the cafeteria); title decided (Entre Sus(pects)), recorded in Docs/review-names.md"
  - "2026-11-13: English text review validated by Virginie (five ChatGPT passes, last one with no correction); translation_en and translation_fr stay unchecked until the localize audit and the French table"
  - "2026-11-13: unverified: French and room names in game (raw keys, clipping, RoleDef abilities.N.label injection through [MustTranslate]) wait for playTests; Virginie French review pending after /review-translation"
  - "2026-11-13: French /review-translation page 1 of 1 read: 4 French fixes applied, 0 refused, no proper noun to decide; waiting for Virginie French review (translation_fr stays partial until she validates)"
  - "2026-11-13: second French /review-translation read: 0 applied, 1 refused (BS_Crewmate_Innocent.tip, masculine-only agreement), no proper noun to decide; waiting for Virginie French review"
  - "2026-11-13: writeTests: TESTING.md has 27 functional scenarios (S01 to S27), none played; Pickle features 02-boot and 03-emergency-button written, not run (first tickets at playTests, one each, -pickle-scenario-timeout=400); Inbox/ created; 5 code risks listed in TESTING.md (evidence gizmo window, shapeshift colour after a save, kill range 2.5 against 2, About text, placeholder art)"
  - "2026-11-13: playTests: tickets ac05 (02-boot) and cb77 (03-emergency-button) came back: 03-emergency-button GREEN twice (6570 ms, then ticket cb77 10955 ms, minified in Tests/Pickle/Evidence/button); 02-boot RED (twice, boot and boot-1): mod load not clean, `[warning] Between Sus - Pickle tests dependency (nelim.betweensus) needs to have <downloadUrl> and/or <steamWorkshopUrl>`; cause: companion Tests/Pickle/Mod/About/About.xml lists nelim.betweensus without a URL; FIXED (steamWorkshopUrl 3838119558, the prepublication item); refiled 02-boot alone as ticket fdda: GREEN (8169 ms, load clean), minified in Tests/Pickle/Evidence/boot-2; both written Pickle scenarios (02-boot, 03-emergency-button) are green, Inbox empty; open: owner owes the crew suit reference clothes (not found in the repo) and Alexandre owes answers on the madness crises"
  - "2026-11-13: crew suit candidates (owner, from the NPT apparel catalogue): armor image 1 = one of Apparel_ArmorLocust (Royalty, no stuff, no CompColorable, not dyeable as is), ArmorRecon, ArmorReconPrestige, ArmorMarineGrenadier, ArmorMarinePrestige, PowerArmor, PlateArmor, ArmorCataphract; helmet image 2 = LisaWork_ApparelWieldingMask (Lisa Work Apparel, Workshop 3709891968, third-party mod, would be a new dependency); ticket edfd (04-suit-colours, pass map wsl-deps.suits.map, evidence Tests/Pickle/Inbox/suits) dyes each one and saves portraits; 2026-11-14: owner picked ArmorRecon (core, takes a dye); helmet open: LisaWork_ApparelWieldingMask is Metallic-stuff, CutoutComplex, no CompColorable: dye step passed but the portrait stays silver, so the dye shows nothing; owner asks whether a colour mod would colour it"
  - "2026-11-13: ticket edfd was red (colonist Aldora does not exist with seed betweensus-suits; colonists are Cahuella, Dweeb, Dane, Hannibal, Misaki, Leon, Nails, Chaz); feature fixed to Cahuella, refiled as ticket 0b8c: GREEN 1/1 (11667 ms, summary.json passed), 9 portraits (8 suits + LisaWork helmet) sent to Virginie, evidence parked in Tests/Pickle/Parked/suits-0b8c (git-ignored; exit: delete once the armor is chosen); open for Virginie: pick the armor (Locust needs the Royalty DLC and has no CompColorable, Recon is core) and say whether Lisa Work Apparel (Workshop 3709891968) is accepted as a dependency for the helmet or a drawn original is wanted; open for Alexandre via Virginie: madness crises (every pawn, driven by mood, in Docs/DESIGN.md) still lack duration, real damage or visual, whether a crisis exposes an impostor or counts as evidence, whether it interrupts a meeting, a vote or a task"
updated: 2026-11-13
protocols_read_sha: 2662ac460346298294f4be0ddf4954cbd2b8677e
---

# Between Sus: status

Current state only: the front matter above is the source of truth. Design decisions live in `Docs/DESIGN.md`.

## Settings audit

Date 2026-11-13, revision 784c819+. Result: `not_applicable`, no settings for 1.0.0.

- **Inventory (5.a):** tunable values in `Source/GameComponent_Ship.cs`: `KillCooldown` 15000, `SabotageCooldown` 30000, `MeetingCooldown` 8000, `EvidenceNeeded` 3; `ScenPart_AssignRoles.impostorCount` (default 2, no editor UI, capped at a quarter of the colonists). No inherited setting, no optional integration.
- **Decision (5.b):** none. These are balance constants of a design that still waits for Alexandre's pitch (`Docs/DESIGN.md`); exposing them now would freeze guesses. Lobby options such as the number of impostors are in `Docs/BACKLOG.md`. Revisit when the pitch lands.
- **Verified (5.c, 5.d):** no `Mod` subclass, no `ModSettings`, no `DoSettingsWindowContents` in `Source/`; no `MainButtonDef` in `Mod/Defs`: no empty settings page and no shortcut.
- **5.e to 5.f:** not applicable, no control exists.
- **Check-SettingsContract.ps1:** reports "no MainButtonDef" until `not_applicable` is recorded, as it says.

## Ecosystem

Date 2026-11-13. Source: PiI, `docs/inventory/ecosystem-steam/BetweenSus.md` (commit a5c2b74 of the protocols checkout).

- **Result:** no duplicate, no replacement.
- **Corpus scan:** 25 defNames and 15 labels gave 4 weak hits and 0 shared defName.
- **Steam:** only Among Us jokes (Metalhorror skin, potion, two sounds); none has impostor, tasks, meetings or votes.
- **Consequence:** no original mod, so no patch and no retexture to play. Dependency stays Harmony only.

## Translation audit

Date 2026-11-13, revision of this commit. Inventory: Keyed (English and French, 85 keys incl. 17 room names), Defs (RoleDef 11 roles with tip and abilities, ThingDef 8, WorkTypeDef, WorkGiverDef, JobDef, ScenarioDef, ScenPartDef) from `Mod/Defs` and `Source`.

- **Code fixes:** room names were English identifiers split by `SplitCamelCase` (hardcoded); now `BS_Room_<Name>` keys (`ShipLayout.Room.Label`) and `BS_RoomLine` for the console lines. `RoleDef.tip` and `RoleAbility.label/description` carry `[MustTranslate]` so DefInjected reaches them. Two unused keys removed (`BS_WinTasks`, `BS_Ejected`).
- **French:** `Mod/Languages/French/Keyed` and `DefInjected` (folders `BetweenSus.RoleDef`, `ThingDef`, `WorkTypeDef`, `WorkGiverDef`, `JobDef`, `ScenarioDef`, `ScenPartDef`). No tu, no vous, no nous: infinitives, nominal and third-person forms. Pawn names arrive as plain strings (no gender available): epicene wording (`Éjection de {0}`, `Élimination de {0} d'après les preuves`). Role labels use `membre d'équipage` plus an apposed noun. Scenario label keeps `Between Sus` (a label).
- **Checks:** `Check-Translations.ps1 -ModRoot .` PASS (48 texts, 26 nested, 44 Keyed calls); 12 nested injection paths (`abilities.N.*`, `scenario.summary`) are proved by the game rules (numeric segment = list index), not by a script: in-game check at playTests.
- **Known English defect:** `BS_TeamSubtitleCrewmate` ("There are {0} impostors among us") has no singular form; French avoids it ("Imposteurs à bord : {0}").
- **Review:** ChatGPT answers read 2026-11-13 (4 French fixes applied, 1 refusal logged). **French reviewed and validated by Virginie, 2026-11-13, revision 95ed2ac, no correction.**

## Test audit

Date 2026-11-13, tested revision 6ad143b (Source unchanged since 737fbb2, Mod since f10c3fe, DLL as committed). All run from the protocols root, by hand: the generated `Tests/Test-Mod.ps1` and `.github` wait for the prepublication junction (CI/CD).

- **7.b, rejoués:** `Run-OfflineChecks.ps1 -ModRoot BetweenSus -SourceDirs BetweenSus/Source -ExtraAssemblies BetweenSus/Mod/Assemblies/BetweenSus.dll`: 9 of 10 pass; the one failure is `ModBasics`: `Mod/About/Preview.png is missing` (made at `mountPreview`, accepted); `XmlClasses` skipped (no game type list). `Check-HarmonyContract.ps1 -Hooks Pawn.GetGizmos:,Pawn_JobTracker.TryTakeOrderedJob:Job,MapGenerator.GenerateMap`: exit 0 (8 passed). `Check-Translations.ps1`, `Check-SettingsContract.ps1`: exit 0. `Tests/Rule-OwnValues.ps1`: exit 0 (about, 11 roles, constants, scenario, props, 17 rooms and 21 doors connected, room keys EN and FR); negative copy fails on six injected defects. Declared in `scripts/LOCAL_SCRIPTS.md` (d14760b), not yet validated by Audit.
- **7.c, XML:** covered by `XmlFields`, `DefRefs`, `TypeRefs`, `ConfigErrors`, `DefInjected`, `Textures` above. No XML patch (0 of 0 operations).
- **7.d, Pickle:** `Tests/Pickle/Mod/Pickle/Features/` 01-reference-capture (ran green, ticket 00b0), 02-boot and 03-emergency-button (written, not run). `Check-Steps.ps1` and `Check-PickleCompanion.ps1` exit 0; the companion no longer lists `nelim.pickletools` as a dependency (the pass map stages it). Pass map `wsl-deps.suite.map`. No local step: nothing to declare to NPT.
- **7.e, not applicable:** C# unit tests (`Run-UnitTests.ps1`): every rule runs on live `Pawn`, `Map` and `Find` objects, a console test would prove its own fakes. Kills, sabotages, votes and role abilities stay manual: no Pickle step forces the random role of the main pawn.
- **7.a:** `TESTING.md`, 27 scenarios with preconditions, actions and expected result; numbers and texts checked against `GameComponent_Ship.cs`, `ShipFurnisher.cs`, `ShipLayout.cs`, `RoleDefs.xml` and `Languages/English/Keyed`. `PUBLICATION.md` does not exist (behaviors taken from `Docs/DESIGN.md`).
