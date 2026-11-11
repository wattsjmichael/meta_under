# UNDER — SPRINT LOG
Meta Quest 3 | Unity 6 | Gaming / New Experience

This is the execution log for UNDER.

The Project Bible defines what the game is.
This file defines what we are building next, what “done” means, and what has actually been proven in-headset.

## RULES OF THE SPRINT

- One Foundation at a time.
- Every Foundation has a headset-testable acceptance gate.
- Codex stops when the current gate is complete.
- Test on Quest 3 before marking a VR interaction Foundation DONE.
- Fix blockers before moving forward.
- Commit after every completed Foundation.
- New ideas go to BACKLOG, not directly into the build.
- Final art does not rescue an unproven interaction.
- If a Foundation fails to feel good, iterate there instead of stacking more systems on top.
- “Works in Editor” is not the same as “works on Quest.”

## STATUS KEY

- `TODO` — not started
- `ACTIVE` — current work
- `BLOCKED` — cannot proceed; blocker must be recorded
- `TEST` — implementation complete, awaiting Quest test
- `DONE` — acceptance criteria passed in headset where applicable
- `CUT` — deliberately removed from scope

---

# PRE-FLIGHT — PROJECT BASELINE

Status: `DONE`

Purpose:
Establish a recoverable project before AI-assisted implementation begins.

Completed:
- Unity 6 project created from Mixed Reality template.
- Meta Quest/OpenXR configuration established.
- Native Android Quest build successfully launched.
- Hand/XR packages present.
- Codex connected to Unity through Unity MCP.
- Live editor mutation proven with `CODEX_TEST`.
- Project direction changed from mixed reality to full VR.
- `UNDER_CODEX_PROJECT_BIBLE.md` created.
- Unity `.gitignore` created.
- Baseline pushed to Git.

Baseline rule:
Do not casually upgrade Unity, OpenXR, XR Interaction Toolkit, XR Hands, Meta OpenXR, or Unity MCP during the sprint. Package changes require a concrete reason.

---

# FOUNDATION 001 — HANDS + TABLE

Status: `DONE`

## Goal

Create the smallest reliable full-VR UNDER scene on standalone Quest 3.

This Foundation proves that the inherited Mixed Reality project can become a clean controller-free VR project without breaking the working Quest XR stack.

## Build

Create `UNDER_Prototype`.

Preserve the working:
- OpenXR / Meta Quest configuration
- XR Origin infrastructure
- XR Hands
- XR Interaction Toolkit
- Android / Quest build configuration

Remove or disable MR-template systems that are no longer needed:
- Passthrough
- AR plane detection
- Scene understanding
- Spatial/scene permissions
- MR sample UI
- MR sample spawning/demo content

Scene contains only what is needed to prove:
- Opaque virtual environment/background
- Floor
- Seated/stationary player
- Tracked hands
- Comfortable table
- One grabbable cube

## Acceptance Gate

On a native Quest 3 build:

- [x] Game launches successfully.
- [x] Passthrough is not visible.
- [x] Player is stationary/seated.
- [x] Both tracked hands appear and follow reliably.
- [x] Table is comfortably positioned from seated posture.
- [x] Cube is resting on table.
- [x] Player can grab cube with tracked hand.
- [x] Player can move cube naturally.
- [x] Player can release cube.
- [x] No blocking compile/runtime errors.
- [x] Performance is acceptable for this empty prototype.

## Explicitly Not Included

No:
- Basement art
- Artifact
- Hole
- Portal
- Creatures
- Audio
- Menus
- Game loop
- Final lighting

## Completion

Quest Test Date: 2026-09-30
Result: PASS on native Quest 3. Direct tracked-hand grabbing works with either hand, throwing works, Recall works from outside seated reach, and a recalled object can immediately be grabbed and thrown again. Hand tracking initializes promptly when UNDER is launched hands-first without active controllers.
Problems Found: Quest controller-to-hand modality switching can delay hand availability when controllers are active or selected at startup.
Fixes: Configured the Android OpenXR path as hands-first, retained direct near-field XRI interaction, and verified safe Recall-to-grab Rigidbody state restoration.
Commit: Pending.
Notes: Known issue/constraint for later hardening — controller-present and controller-to-hand startup transitions are not yet an accepted launch path. Until hardened, launch UNDER hands-first without active controllers.

---

# FOUNDATION 002 — THE HOLE

Status: `DONE`

## Goal

Prove UNDER's signature spatial illusion.

An ordinary table must appear to open into a space dramatically larger and deeper than should physically exist beneath it.

This is the first “holy shit” test.

## Build

Use prototype geometry only.

Create:
- Opening/breach through the table
- View into a controlled environment beneath it
- Strong depth cues
- At least one visible reference object/movement far below
- Comfortable viewing angle from seated position

The implementation may cheat aggressively. The player experience matters more than physically correct geometry.

Potential techniques may include:
- Portal rendering
- Render textures
- Masked geometry
- Secondary camera/environment
- Hidden geometry
- Forced perspective
- Combination of techniques

Do not commit to a complicated portal architecture until the simplest convincing technique is tested in-headset.

## Acceptance Gate

On Quest 3:

- [x] Opening reads as a hole rather than a flat image.
- [x] Player naturally leans over to inspect it.
- [x] Looking into it produces convincing depth.
- [x] Space below feels larger than the physical table/basement allows.
- [x] Head movement preserves the illusion.
- [x] No obvious rendering failure destroys the effect.
- [x] Comfortable from seated position.
- [x] Performance remains acceptable.

### Human Kill Test

After first viewing, answer:

**Did I instinctively want to lean over and look farther into it?**

If NO, Foundation 002 is not done.

## Completion

Quest Test Date: 2026-10-01
Result: PASS on native Quest 3. The tabletop aperture isolates UNDER from the ordinary room, preserves the normal floor outside and beneath the table, and presents an impossible deep space with moving depth references.
Problems Found: The room floor had been physically opened and the large UNDER environment could be seen beside and underneath the table, destroying the spatial-impossibility illusion.
Fixes: Restored one continuous solid room floor, removed the table-apron concealment workaround, added a top-facing stencil aperture, restricted every UNDER material to the aperture stencil, and made the ordinary floor reject only aperture pixels. UNDER now uses normal geometry depth inside the opening while remaining invisible from beside or beneath the table.
Commit: Pending.
Notes: Uses one lightweight stencil mask, real prototype geometry, the existing stereo XR camera, and one moving depth cue. No render texture, secondary camera, portal reprojection, extra lights, or Foundation 003 interaction is present. Android is explicitly Vulkan, matching the tested Quest render path.

---

# FOUNDATION 003 — REACH INTO UNDER

Status: `DONE`

## Goal

Prove that the player can physically interact across the breach boundary.

This turns the portal from scenery into a gameplay space.

## Build

Create a simple object below the opening.

Player must:
1. See it below.
2. Reach a tracked hand through the opening.
3. Grab it.
4. Pull it upward.
5. Bring it completely into the basement.
6. Continue manipulating it normally.

Use a primitive object. Do not use creature art yet.

## Acceptance Gate

On Quest 3:

- [x] Hand can cross the visual breach without the illusion breaking badly.
- [x] Grab detection works below the table.
- [x] Player can retrieve object.
- [x] Object transitions into basement correctly.
- [x] Physics remains stable.
- [x] No confusing collision wall blocks the hand.
- [x] Interaction is understandable without text instructions.
- [x] Comfortable and repeatable for vertical-slice production; remaining regression and interaction-edge-case work is deferred to final polish.

### Technical Kill Test

If reaching through the opening cannot be made reliable and convincing, stop and redesign the breach implementation before continuing.

## Completion

Quest Test Date: 2026-10-01 partial native Quest 3 pass; accepted for vertical-slice production on 2026-11-11.
Result: ACCEPTED for vertical-slice production. Direct tracked-hand reach, grab, room extraction, and throwing passed on Quest 3. The existing Foundation 001 Recall implementation is reliably gated off in UNDER and registered after room extraction. Remaining Recall regression, hand-boundary rendering, and interaction edge cases are deferred to final polish.
Problems Found: Foundation 002's impossible space is stencil-isolated but shares normal Unity world coordinates, requiring an explicit visual-state transition. Runtime enabling also left the extracted object's `RecallableObject` outside the shared Recall registry in the Editor integration test.
Fixes: Added a reusable finite `PortalBoundary` and generic `PortalCrossingParticipant`. The participant switches portal/room material bindings only after a real aperture crossing. `RecallableObject` now exposes a reusable availability method that synchronizes component state and shared-registry membership; the crossing participant uses it to disable Recall in UNDER and enable the unchanged Recall motion/gesture system in the room.
Commit: Included in the vertical-slice production checkpoint.
Notes: The remaining Foundation 003 acceptance gate is explicitly waived for the competition sprint. The existing hands remain one normally rendered mesh per hand. Table/rim depth naturally occludes much of the hand at the boundary from the intended lean-over view, but there is no exact per-vertex portal clipping; extreme oblique views may show a hand segment outside the aperture. Gauntlet Pull is retained as a provisional gameplay mechanic and will be reevaluated during final polish.

---

# FOUNDATION 004 — THE LOCK

Status: `ACTIVE`

## Goal

Prove the artifact's tactile interaction language before making final art.

The artifact is a LOCK, not a key.

## Build

Create a placeholder artifact with a small sequence of hand-driven mechanical interactions.

Candidate interaction vocabulary:
- Grab
- Rotate ring
- Slide component
- Press opposing surfaces
- Pull/separate component
- Two-handed alignment
- Snap into final state

Do not use every interaction simply because it exists.

Prefer 3–4 interactions that are reliable and satisfying.

## Acceptance Gate

On Quest 3:

- [ ] Player can pick up artifact naturally.
- [ ] Interaction affordances can be discovered without a conventional tutorial.
- [ ] Mechanisms respond reliably to hands.
- [ ] Interaction has clear visual/audio/haptic-equivalent feedback where possible.
- [ ] Two-handed interaction works if required.
- [ ] Artifact has an ordered state progression.
- [ ] Player can reach a final solved state.
- [ ] Final state fires a clean gameplay event.
- [ ] Puzzle is interesting without requiring tiny precision.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 005 — THE MISTAKE

Status: `TODO`

## Goal

Connect curiosity to consequence.

Solving the artifact must cause the player to realize they have opened something they should not have.

## Build

Sequence:
1. Player completes artifact.
2. Mechanical final CLACK.
3. Comfortable room ambience changes/stops.
4. Delay.
5. Knock from below.
6. Additional directional cue.
7. Breach begins.
8. Impossible depth is revealed.

No creature yet.

## Acceptance Gate

- [ ] Artifact solve cleanly triggers sequence.
- [ ] Player understands sound is coming from below.
- [ ] Timing creates anticipation rather than confusion.
- [ ] Breach opening feels caused by artifact.
- [ ] Player's attention is naturally directed without HUD arrows.
- [ ] Foundation 002 depth illusion survives the animated opening.
- [ ] Sequence can reset reliably for testing.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 006 — FIRST CREATURE

Status: `TODO`

## Goal

Create the first creature reveal and establish UNDER's creepy/funny tone.

## Build

One creature only.

Sequence:
1. Distant movement/scratching.
2. Creature climbs toward opening.
3. Fingers/hands appear first if asset/animation supports it.
4. Creature pulls itself into basement.
5. Brief recognition beat.
6. Creature notices an important placeholder object.
7. Creature grabs/interferes with it and moves away.

Use an existing properly licensed creature asset if suitable.

Record it in `THIRD_PARTY_ASSETS.md`.

## Acceptance Gate

- [ ] Creature entrance is readable.
- [ ] Player notices it without forced camera control.
- [ ] Scale feels appropriately small and physical.
- [ ] Creature belongs visually in the experience.
- [ ] Reveal produces tension followed by personality.
- [ ] Creature can transition from breach to basement reliably.
- [ ] Behavior is deterministic enough to test repeatedly.
- [ ] Quest performance remains acceptable.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 007 — CREATURE PHYSICALITY

Status: `TODO`

## Goal

Make the creature feel like a thing in the room rather than an animated prop.

## Build

Player can:
- Grab creature
- Hold creature
- Move creature
- Release creature
- Throw creature

Creature can:
- React to grab
- Suspend normal authored behavior while held
- Resume appropriate behavior after release
- Use release velocity when thrown
- Recover without exploding into physics nonsense

Optional only if reliable:
- Creature grabs/clings to player's hand or forearm

## Acceptance Gate

- [ ] Grabbing feels forgiving.
- [ ] Creature does not jitter uncontrollably.
- [ ] Animation/physics transition is believable enough.
- [ ] Throw direction/velocity feels understandable.
- [ ] Creature recovers after being dropped.
- [ ] Player immediately understands “I can physically deal with these little bastards.”
- [ ] Repeated grabbing does not break creature state.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 008 — CONTROLLED CHAOS

Status: `TODO`

## Goal

Turn the established mechanics into a game without turning UNDER into a wave shooter.

## Build

Use approximately three reusable creature behaviors.

Candidate behaviors:
1. Emerge/climb.
2. Steal/interfere/flee.
3. Cling/grab/block.

Create a short escalation sequence where the player's real objective is restoring/reversing the artifact while creatures interfere.

The creatures are pressure, not the objective.

## Acceptance Gate

- [ ] Player understands what they are trying to accomplish.
- [ ] Creature interference creates urgency.
- [ ] Throwing/grabbing is useful but not the entire game.
- [ ] Player must divide attention between artifact and creatures.
- [ ] Escalation feels authored rather than random.
- [ ] Difficulty can be tuned.
- [ ] Sequence remains readable.
- [ ] No requirement for inventory, weapons, crafting, or progression systems emerges.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 009 — SOMETHING BIG

Status: `TODO`

## Goal

Deliver a finale with scale without building a giant AI boss.

## Build

Represent a much larger presence below the breach using the cheapest convincing combination of:
- Partial anatomy
- Hands/arms
- Head
- Silhouette
- Eye(s)
- Sound
- Environmental movement
- Scripted animation
- Lighting
- Table/breach reaction

Current finale direction:
1. Large presence approaches/emerges.
2. Player uses an established fictional charge/flare/alchemical device.
3. Device disrupts/knocks the presence back.
4. Player uses both hands to physically force the breach closed.
5. Player reverses/reseals the artifact.
6. Breach closes.

Do not create a fully simulated boss unless testing proves it is necessary.

## Acceptance Gate

- [ ] Large presence feels dramatically larger than previous creatures.
- [ ] Scale is communicated before full reveal.
- [ ] Player understands the immediate danger.
- [ ] Finale uses hands meaningfully.
- [ ] Closing requires physical two-handed action.
- [ ] Artifact knowledge from opening matters.
- [ ] Sequence has a clear climax.
- [ ] No major comfort violation.
- [ ] Performance survives finale.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 010 — THE EXPERIENCE

Status: `TODO`

## Goal

Turn the proven prototype chain into a polished, authored 8–10 minute release/submission candidate.

This is where prototype geometry becomes UNDER.

## Build / Polish

Environment:
- Late-1980s basement art pass
- Period-authentic fictional props
- Table final
- Lighting final
- Environmental storytelling

Artifact:
- Final visual design
- Materials
- Mechanical animation
- Feedback

Breach:
- Final visuals
- Depth environment
- Atmospherics
- Lighting response
- Performance optimization

Creatures:
- Final selected assets
- Material consistency
- Animation polish
- Reactions
- Audio

Audio:
- Basement room tone
- CRT/stereo/cassette ambience
- Artifact mechanical language
- Knocks/scratches
- Creature audio
- UNDER ambience
- Finale
- Original/licensed music

Experience:
- Opening pacing
- Natural onboarding
- Reset/restart behavior
- Failure handling if needed
- Ending
- Final tiny knock
- Dadbod Studios presentation

Technical:
- Quest performance pass
- Build reliability
- Hand-tracking reliability
- Comfort pass
- Error cleanup
- Android build verification
- Submission capture

## Acceptance Gate

- [ ] Complete experience can be played start-to-finish.
- [ ] Target runtime approximately 8–10 minutes.
- [ ] No required controller interaction.
- [ ] No obvious placeholder content remains.
- [ ] No blocking bugs.
- [ ] Hand interactions survive repeated playthroughs.
- [ ] First five minutes are compelling.
- [ ] Breach remains the signature visual moment.
- [ ] Art direction is coherent.
- [ ] Audio direction is coherent.
- [ ] Frame rate/performance acceptable on Quest 3.
- [ ] New player can understand the experience without developer coaching.
- [ ] Final build installs and launches cleanly.
- [ ] Submission footage/screenshots captured.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Build Version:
Notes:

---

# RELEASE / SUBMISSION GATE

Status: `TODO`

Target competition deadline:
November 18, 2026 — verify official deadline/time before final submission.

Final tasks:
- [ ] Verify current competition rules.
- [ ] Verify track/division.
- [ ] Verify required store/build state.
- [ ] Verify hand-tracking requirements.
- [ ] Verify submission materials.
- [ ] Verify third-party licenses.
- [ ] Verify music/audio rights.
- [ ] Create final build.
- [ ] Fresh-install final build on Quest 3.
- [ ] Complete uninterrupted playthrough.
- [ ] Capture gameplay video.
- [ ] Capture screenshots/key art.
- [ ] Write submission description.
- [ ] Submit before deadline, not at deadline.
- [ ] Tag final Git commit/release.

---

# BACKLOG — NOT APPROVED FOR IMPLEMENTATION

Put cool ideas here so they stop hijacking the sprint.

- Additional creature variants
- Additional artifact mechanisms
- Alternate endings
- More basement rooms
- Easter eggs
- Advanced creature/player attachment
- Optional controller support
- Replay variations
- Additional environmental interactions
- Anything beginning with “what if we also…”

Backlog items are ideas, not commitments.

---

# DECISION LOG

Use this section for decisions that change implementation direction.

### 2026-09-27
Decision:
UNDER is full VR rather than mixed reality.

Reason:
Full VR gives complete control over environment, table placement, lighting, occlusion, creature behavior, impossible portal scale, reliability, and Quest performance while avoiding Scene API/real-world surface setup friction.

Impact:
Passthrough and room scanning are no longer part of the core architecture.

### 2026-09-27
Decision:
Controller-free tracked hands are the primary interaction model.

Reason:
Physical hand interaction is central to the identity of UNDER and to the artifact/breach/creature experience.

### 2026-09-27
Decision:
Development uses gated Foundations rather than feature-parallel implementation.

Reason:
Every major technical/design risk should be proven in-headset before content is stacked on top of it.

### 2026-09-30
Decision:
Foundation 001 is accepted with a hands-first startup constraint.

Reason:
Native Quest 3 testing passed direct two-hand grabbing, throwing, seated-reach Recall, immediate post-Recall grabbing/throwing, and prompt hand initialization when launched without active controllers. Controller-to-hand modality switching can still delay hand availability when controllers are active or selected at startup.

Impact:
Foundation 002 may proceed, but controller-present startup and controller-to-hand switching remain a known hardening task. The validated launch path is hands-first without active controllers.

---

# SESSION LOG

Add one entry per meaningful development session.

## 2026-09-27 — Foundation 001 Implementation

Date: 2026-09-27
Foundation: FOUNDATION 001 — HANDS + TABLE
Goal: Create the minimal full-VR Quest 3 hands-and-table prototype.

Changes:
- Created `UNDER_Prototype` with the existing tracked-hand/XRI infrastructure, stripped of MR scene systems.
- Added an opaque virtual background, prototype floor, seated-height table, and one grabbable cube.
- Disabled Android Meta passthrough and spatial/scene OpenXR features while preserving Quest and hand tracking.
- Made `UNDER_Prototype` the sole enabled build scene.

Quest Test:
- Pending native Quest 3 validation.

Result:
- Implementation complete in Editor; status moved to `TEST` pending headset acceptance.

Next:
- Verify launch, opaque VR, both hands, seated reach, cube grab/move/release, and performance on Quest 3.

Commit:
- Pending Quest acceptance.

## 2026-09-30 — Foundation 001 Quest Acceptance

Date: 2026-09-30
Foundation: FOUNDATION 001 — HANDS + TABLE
Goal: Complete the native Quest 3 acceptance gate.

Changes:
- No new implementation in this session; recorded the successful headset result.
- Marked Foundation 001 `DONE`.
- Marked Foundation 002 `ACTIVE` without beginning implementation.

Quest Test:
- PASS: hands initialize promptly when UNDER is launched hands-first without active controllers.
- PASS: either tracked hand can directly grab and naturally manipulate the cube.
- PASS: direct throwing preserves release motion.
- PASS: Recall retrieves the cube from outside seated reach.
- PASS: a recalled cube can immediately be directly grabbed and thrown again.

Result:
- Foundation 001 acceptance gate passed on native Quest 3.

Known Issue / Constraint:
- Active controllers or a controller-selected startup path can delay controller-to-hand switching and hand availability. Preserve the hands-first launch path for now and harden mixed controller/hand startup later.

Next:
- Foundation 002 — THE HOLE is active, but implementation has not started.

Commit:
- Pending.

## 2026-10-01 — Foundation 002 Implementation

Date: 2026-10-01
Foundation: FOUNDATION 002 — THE HOLE
Goal: Build the simplest Quest-friendly prototype that can prove impossible depth through the table.

Changes:
- Rebuilt the tabletop as four physical rails surrounding a large rectangular opening.
- Opened the floor beneath the table while preserving the visible room floor and table setup.
- Added a short dark throat and table aprons that conceal the geometric cheat outside the opening.
- Added a roughly 20-meter-deep physical underworld vista using primitive shelves, ledges, columns, distant walls, and repeated emissive scale markers.
- Added one subtle distant moving light using `UnderworldDrifter`.
- Preserved the Foundation 001 cube, direct hand interaction, throwing, Recall, and XR configuration.

Technique:
- Single-camera, real-geometry aperture and vista. Stereo disparity and head-motion parallax come directly from the existing XR camera; there is no render texture, secondary portal camera, or stencil pass.

Editor Test:
- Play Mode completed with no errors.
- The distant motion cue moved correctly.
- The regression cube remained dynamic with Recall enabled.
- `UNDER_Prototype` remains the sole enabled build scene.

Quest Test:
- Pending native Quest 3 stereo, comfort, performance, and human lean-over validation.

Result:
- Implementation moved to `TEST`; Foundation 002 is not yet `DONE`.

Next:
- Test the breach on Quest 3 and answer the Human Kill Test: “Did I instinctively want to lean over and look farther into it?”

Commit:
- Pending Quest acceptance.

## 2026-10-01 — Foundation 002 View-Isolation Fix

Date: 2026-10-01
Foundation: FOUNDATION 002 — THE HOLE
Goal: Correct the failed literal-pit implementation so UNDER is visible only through the tabletop aperture.

Changes:
- Restored the basement/room floor as one continuous solid object and removed the physical floor opening.
- Removed the table aprons that attempted to conceal the old world-space pit.
- Added a top-facing, colorless stencil aperture inside the tabletop opening.
- Converted UNDER prototype materials to a stencil-tested single-camera shader with normal geometry depth.
- Added a stencil-aware ordinary floor material so the intact floor is hidden only in aperture pixels viewed from above.
- Removed the two underworld point lights; the portal materials use a cheap prototype lighting term.
- Preserved all Foundation 001 XR, tracked-hand, direct-grab, throwing, and Recall systems.

Editor Test:
- Lean-over and left/right-offset views show the deep geometry only inside the aperture with real head-motion parallax.
- Beside-table and under-table views show the continuous room floor and no UNDER geometry.
- Play Mode completed with no errors; only the existing Windows OpenXR loader-path warning was logged.
- The distant motion cue remained active, and the regression cube remained dynamic with XR Grab enabled.
- `UNDER_Prototype` remains in the Quest build.

Quest Test:
- Pending native Quest 3 stereo/view-isolation retest.

Result:
- Corrected implementation remains `TEST`; Foundation 002 is not `DONE`.

Next:
- Repeat the five-view isolation/parallax acceptance test on Quest 3. Do not begin Foundation 003.

Commit:
- Pending Quest acceptance.

## 2026-10-01 — Foundation 002 Quest Acceptance

Date: 2026-10-01
Foundation: FOUNDATION 002 — THE HOLE
Goal: Validate the corrected stencil-isolated impossible-space prototype on Quest 3.

Quest Test:
- PASS: UNDER is visible through the tabletop aperture without leaking beside or beneath the table.
- PASS: The normal room floor remains visually intact outside the aperture.
- PASS: The deep space and moving references communicate impossible scale.

Result:
- Foundation 002 acceptance gate passed and status changed to `DONE`.

Next:
- Begin Foundation 003 only: direct tracked-hand interaction across the breach boundary.

Commit:
- Pending.

## 2026-10-01 — Foundation 003 Implementation

Date: 2026-10-01
Foundation: FOUNDATION 003 — REACH INTO UNDER
Goal: Prove that a tracked hand can directly retrieve a physical object through the impossible aperture.

Changes:
- Added a reusable finite `PortalBoundary` to the existing aperture.
- Added a generic `PortalCrossingParticipant` with per-renderer UNDER/room material bindings and room-only behaviour activation.
- Added one bright primitive test object 14 cm below the aperture on a small portal-only reach platform.
- Reused the Foundation 001 velocity-tracked `XRGrabInteractable`, dynamic Rigidbody, throw settings, and `RecallableObject` configuration.
- Kept Recall disabled while the object belongs to UNDER; it becomes available only after a valid extraction through the aperture.
- Preserved the Foundation 001 cube and all XR/hand systems, plus the Foundation 002 stencil/parallax implementation and intact room floor.

Architecture:
- UNDER and the room share Unity world coordinates; stencil controls presentation, not physics position.
- A participant changes side only when its physical path intersects the finite aperture plane. Crossing the same plane outside the opening is ignored.
- The transition is bidirectional and generic enough for later props or creature roots with multiple renderer bindings.

Editor Test:
- Portal-side object and platform are visible through the aperture and hidden from the ordinary under-table view.
- Play Mode verified UNDER → room and room → UNDER transitions through the aperture.
- Crossing the boundary plane outside the aperture did not change state.
- Room extraction enabled the unchanged Recall component and normal URP rendering; the Rigidbody remained under XRI control.
- No blocking Console errors occurred.

Hand Rendering Limitation:
- The existing single hand mesh is not split into separate stencil and room passes. Table/rim depth supplies natural boundary occlusion from the intended view, but exact partial-hand clipping is not implemented in this first interaction test.

Quest Test:
- Pending native Quest 3 direct-reach, extraction, throwing, Recall, stereo, and comfort validation.

Result:
- Foundation 003 moved to `TEST`; it is not `DONE`.

Next:
- Run the complete Foundation 003 headset acceptance sequence. Do not begin Foundation 004.

Commit:
- Pending Quest acceptance.

## 2026-10-01 — Foundation 003 Extracted-Object Recall

Date: 2026-10-01
Foundation: FOUNDATION 003 — REACH INTO UNDER
Goal: Reuse Foundation 001 Recall after successful portal extraction.

Changes:
- Preserved the existing Recall gesture, movement, arrival catch, direct grab, and XRI throw implementation.
- Kept the portal object excluded from Recall while it belongs to UNDER.
- Added explicit Recall availability/registry synchronization when the crossing participant changes sides.
- Kept the portal stencil, finite-boundary crossing, direct-only interaction, and seated placement unchanged.

Test:
- Verified the object is absent from the Recall registry before extraction.
- Verified room extraction enables and registers the same `RecallableObject` used by the Foundation 001 cube.
- Verified Recall starts for the extracted object and restores a dynamic Rigidbody with XRI grab/throw enabled.
- Repeated Recall initiation succeeded after re-extraction; no kinematic angular-velocity warning or other error occurred.

Result:
- Foundation 003 remains `TEST` pending the complete native Quest Recall sequence.

Next:
- Quest test: extract, throw out of reach, Recall, direct grab, throw again. Do not begin Foundation 004.

Commit:
- Pending Quest acceptance.

## 2026-11-11 — Vertical-Slice Production Checkpoint

Date: 2026-11-11
Foundation: FOUNDATION 002 / FOUNDATION 003 / FOUNDATION 004
Goal: Protect the accepted portal foundation and move into competition vertical-slice production.

Decisions:
- Foundation 002 is accepted and remains `DONE`.
- Foundation 003 is accepted for vertical-slice production and is now `DONE`.
- Remaining portal/hand-boundary regression, Recall regression, and interaction edge cases are deferred to final polish.
- Gauntlet Pull is retained as a provisional gameplay mechanic for the vertical slice.
- Foundation 004 — THE LOCK is now `ACTIVE`.
- The project has entered vertical-slice production with 8 days remaining in the competition sprint.

Recovered Work:
- The ordered Artifact Lock controller, rotary mechanism, slider mechanism, two-hand seal, and deterministic Editor builder scripts were recovered from the interrupted session.
- The Artifact Lock builder has not been executed. No Artifact Lock GameObjects or generated artifact materials have been added to the scene yet.

Next:
- Continue Foundation 004 deliberately from the protected checkpoint.
- Do not execute the Artifact Lock builder until the scene mutation and generated assets are explicitly reviewed.

Commit:
- Checkpoint UNDER portal interactions and begin artifact lock.

## Session Template

Date:
Foundation:
Goal:

Changes:
-

Quest Test:
-

Result:
-

Next:
-

Commit:

---

# CURRENT ACTION

FOUNDATION 004 — THE LOCK

Status is `ACTIVE`.

UNDER is in vertical-slice production with 8 days remaining. Foundation 002 and Foundation 003 are accepted for production; remaining regression and interaction-edge-case work is deferred to final polish. Gauntlet Pull remains provisional. Continue THE LOCK from the recovered scripts without executing the Artifact Lock builder until its scene mutation and generated assets are explicitly reviewed.
