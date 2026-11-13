---
mod: Between Sus
packageId: nelim.betweensus
repo: Rimworld-Between-Sus
remote: https://github.com/vbardales/Rimworld-Between-Sus.git
visibility: public
detached: yes
workflow_stage: localize[1.0.0]
licence: original
licence_at: "2026-11-11: original work by Nelim (requested by Alexandre), MIT; no third-party code or asset reused, Harmony and Krafs.Rimworld.Ref only as dependencies"
upstream_mod_remotes: N/A
settings_audit: not_applicable
localization: unchecked
translation_en: unchecked
translation_fr: unchecked
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
  - "2026-11-13: Ecosystem (PiI, docs/inventory/ecosystem-steam/BetweenSus.md, a5c2b74): no duplicate, no replacement; corpus scan of 25 defNames and 15 labels gave 4 weak hits, 0 shared defName; Steam has only Among Us jokes (Metalhorror skin, potion, two sounds), none with impostor, tasks, meetings or votes; no original mod, so no patch or retexture to play"
  - "2026-11-12: /review-translation page 1 of 1 done (ChatGPT answer read): 8 English text fixes applied (scenario, task station, consoles, cameras, ejection, track and necro report); 6 postponed or refused in Docs/review-refusals.md; waiting for Virginie on proper nouns (title French form, Harmony); translation_fr stays pending until the French table exists"
  - "2026-11-12: second /review-translation (after the fixes): one text fix applied (BS_SabotageFixed), nothing else; title French form still waits for Virginie"
  - "2026-11-13: third /review-translation: BS_EvidenceReady fixed (They to {0}); title French form still waits for Virginie, TITLE? sent to Audit"
  - "2026-11-13: fourth /review-translation: BS_EmergencyButton description fixed (of to in the cafeteria); title decided (Entre Sus(pects)), recorded in Docs/review-names.md"
  - "2026-11-13: English text review validated by Virginie (five ChatGPT passes, last one with no correction); translation_en and translation_fr stay unchecked until the localize audit and the French table"
  - "2026-11-13: unverified: Check-SettingsContract.ps1 FAILs (code, no MainButtonDef) by design; way out is a rule in the mod local test script (Tests/Rule-OwnValues.ps1 + row in LOCAL_SCRIPTS.md), done with the test shape conversion (needs the 0.1.0 junction and generated skeleton); Audit asks Virginie whether the script should read settings_audit not_applicable, no script change meanwhile"
updated: 2026-11-13
protocols_read_sha: a5c2b748de9f1e3275c45ba2877929b21ca94269
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
