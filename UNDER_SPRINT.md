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

Status: `ACTIVE`

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

- [ ] Game launches successfully.
- [ ] Passthrough is not visible.
- [ ] Player is stationary/seated.
- [ ] Both tracked hands appear and follow reliably.
- [ ] Table is comfortably positioned from seated posture.
- [ ] Cube is resting on table.
- [ ] Player can grab cube with tracked hand.
- [ ] Player can move cube naturally.
- [ ] Player can release cube.
- [ ] No blocking compile/runtime errors.
- [ ] Performance is acceptable for this empty prototype.

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

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 002 — THE HOLE

Status: `TODO`

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

- [ ] Opening reads as a hole rather than a flat image.
- [ ] Player naturally leans over to inspect it.
- [ ] Looking into it produces convincing depth.
- [ ] Space below feels larger than the physical table/basement allows.
- [ ] Head movement preserves the illusion.
- [ ] No obvious rendering failure destroys the effect.
- [ ] Comfortable from seated position.
- [ ] Performance remains acceptable.

### Human Kill Test

After first viewing, answer:

**Did I instinctively want to lean over and look farther into it?**

If NO, Foundation 002 is not done.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 003 — REACH INTO UNDER

Status: `TODO`

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

- [ ] Hand can cross the visual breach without the illusion breaking badly.
- [ ] Grab detection works below the table.
- [ ] Player can retrieve object.
- [ ] Object transitions into basement correctly.
- [ ] Physics remains stable.
- [ ] No confusing collision wall blocks the hand.
- [ ] Interaction is understandable without text instructions.
- [ ] Comfortable and repeatable.

### Technical Kill Test

If reaching through the opening cannot be made reliable and convincing, stop and redesign the breach implementation before continuing.

## Completion

Quest Test Date:
Result:
Problems Found:
Fixes:
Commit:
Notes:

---

# FOUNDATION 004 — THE LOCK

Status: `TODO`

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

---

# SESSION LOG

Add one entry per meaningful development session.

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

FOUNDATION 001 — HANDS + TABLE

Do not work ahead.

When it passes on Quest 3:
1. Record results above.
2. Commit.
3. Push.
4. Change Foundation 001 to `DONE`.
5. Change Foundation 002 to `ACTIVE`.
6. Begin the hole.
