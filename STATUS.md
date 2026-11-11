---
mod: Between Sus
packageId: nelim.betweensus
repo: Rimworld-Between-Sus
remote: https://github.com/vbardales/Rimworld-Between-Sus.git
visibility: public
detached: yes
workflow_stage: settings[1.0.0]
licence: original
licence_at: "2026-11-11: original work by Nelim (requested by Alexandre), MIT; no third-party code or asset reused, Harmony and Krafs.Rimworld.Ref only as dependencies"
upstream_mod_remotes: N/A
settings_audit: unchecked
localization: unchecked
translation_en: unchecked
translation_fr: unchecked
dependencies: declared
workshop: none
workshop_required_items: n/a, not released yet
published: no
tested_on: never
remaining:
  - "2026-11-11: ticket 20261112-002350-786-1e9d (Pickle, commit 5618076, 01-reference-capture, evidence Tests/Pickle/Inbox/reference-capture) is queued: the first one (90b2) failed in staging, incompatible standalone clearscreen and screenshotmode with the bundle. The TM does not know this session (REGISTER never delivered): after the clear, check Submit-PickleRun.ps1 -List and .pickle-state/rimworld-ticket-dispatcher/results/<id>.json by hand. If it fails on Undefined step for ScreenshotMode, ask PiT (Check-Steps wants @requires:nelim.pickletools.screenshotmode, the bundle already holds it)"
  - "2026-11-12: art plan: the ghost layers (Art/Work/ghost-layers.webp) are not wired and must be redone from the real colonist capture of that ticket; crew suit and ejection animation (engine fuel) wait for the same capture; role icons, stations, button and consoles are wired"
  - "2026-11-12: prepublication 0.1.0 item exists (PublishedFileId 3838119558, committed at the owner order)"
  - "2026-11-11: unverified: at localize, translation_fr is not done yet; /review-translation is mandatory before the owner's French review (AUDIT.md 6.g, TM rule of 2026-11-11), translation_fr goes through pending_review"
  - "2026-11-11: dependencies (revision 728bef7): Harmony (brrainz.harmony, Workshop 2009463077) is the only dependency, hard, because the code patches with HarmonyLib; no other mod referenced in code or XML; no DLC required; no optional compat (OptionalCompat.xml not needed, Check-OptionalCompat 0 target); 4.f n/a (no animal); multiplayer mods are for 1.1.0"
  - "2026-11-11: unverified: French translation is for the localize stage, art and later work are in Docs/BACKLOG.md"
  - "2026-11-11: unverified: Alexandre's rules (evidence, door sabotage, ejection, ghosts, vote argument) are built but never run in RimWorld; the ghost has no distinct look yet"
  - "2026-11-11: unverified: tasks, sabotage, kill, meeting, vote, win conditions and the 11 roles are built but never run in RimWorld; Harmony targets Pawn.GetGizmos, Pawn_JobTracker.TryTakeOrderedJob and MapGenerator.GenerateMap unchecked in game"
  - "2026-11-11: owner decision: develop every feature first, then Pickle tests produce the captures; rest of the pitch waits for Alexandre, design choices made meanwhile are in Docs/DESIGN.md and can change"
updated: 2026-11-12
protocols_read_sha: 0ec00941d9922269b308d0234a9062b70e31a34e
---

# Between Sus: status

Current state only: the front matter above is the source of truth. Design decisions live in `Docs/DESIGN.md`.
