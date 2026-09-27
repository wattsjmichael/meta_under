# UNDER — CODEX PROJECT BIBLE
Version 0.1 — September 27, 2026

## 1. PURPOSE

This file is the persistent source of truth for AI-assisted development of UNDER.

Read this document before making substantial design or implementation changes. If a task prompt conflicts with this document, stop and report the conflict rather than silently changing the game's direction.

UNDER is intentionally small. Finishing a short, exceptional experience is more important than adding systems.

## 2. HIGH CONCEPT

UNDER is a short, hand-tracked Meta Quest 3 VR creature-horror experience set in a stylized late-1980s suburban basement.

The player is seated at a table with a strange mechanical artifact. There is no conventional tutorial. Curiosity teaches the controls.

The player manipulates and eventually solves the artifact.

That is a mistake.

The artifact is not a key. It is a lock.

Solving it opens a physically impossible breach through the table into a vast place below. Small creatures begin climbing into the basement. The player must physically manage the escalating chaos, understand how the artifact works, and ultimately force the breach closed.

Target experience length: approximately 8–10 minutes.

Core emotional arc:

CURIOSITY → DISCOVERY → UNEASE → DELIGHT → CHAOS → PANIC → MASTERY → RELIEF → ONE LAST KNOCK

## 3. THE PLAYER PROMISE

The player should feel:

“I touched something I should not have touched, opened something impossible with my own hands, and now I have to physically put it right.”

The defining interaction is not shooting.

The defining interaction is HANDS.

The player should manipulate, grab, rotate, push, pull, pry, hold, throw, resist, and seal.

If a mechanic would work essentially unchanged with a gamepad on a television, reconsider it.

## 4. PLATFORM AND TECHNICAL DIRECTION

Primary target:
- Meta Quest 3
- Standalone Android build
- Unity 6
- OpenXR / Meta Quest support
- XR Interaction Toolkit
- Unity XR Hands
- Controller-free hand tracking from beginning to end

Development:
- Codex is connected to the live Unity Editor through Unity MCP.
- Codex may inspect scenes, create/edit objects and scripts, inspect console output, run tests, and perform other supported editor operations.
- Human headset testing is authoritative for comfort and interaction feel.

UNDER IS NOW FULL VR.

Do NOT build:
- Passthrough
- Scene understanding
- Room scanning
- Real-world plane detection
- Semantic furniture detection
- Spatial-data permissions
- A dependency on the player's physical table

The original Unity project came from the Mixed Reality template. Preserve only the infrastructure useful to the full-VR Quest experience.

## 5. DESIGN PILLARS

### Hands Are the Game
Hand tracking is not a controller substitute. Important interactions should exploit natural hand motion.

### Impossible Depth
The breach must create a strong sense that an enormous space exists beneath an ordinary table. Physically leaning over and looking down should be a signature moment.

### One Great Room
UNDER does not need a world. It needs one memorable basement.

### Practical Creature Feature
The visual tone should evoke late-1980s practical creature movies: tactile, chunky, slightly grotesque, occasionally funny, and charmingly physical.

Do not imitate protected creature designs, props, characters, environments, or sequences from existing films.

### Curiosity Is the Tutorial
Avoid floating instructions whenever the environment and object behavior can teach the player instead.

### Escalation, Not Feature Count
The same small set of interactions should become increasingly difficult and meaningful as the situation deteriorates.

## 6. SETTING

A fictional late-1980s suburban basement/rec room.

Desired environmental language:
- Wood paneling
- Period carpet
- Card/game/work table
- CRT television
- VHS tapes
- Cassette deck or stereo
- Warm practical lamps
- Small basement window
- Period clutter
- Original posters/graphics
- Original synth-inspired ambience/music
- Lived-in rather than haunted-house cliché

The basement should initially feel safe and nostalgic.

The horror comes from something impossible entering that safe space.

Avoid trademarked brands and copyrighted poster/art reproductions unless separately licensed.

## 7. THE ARTIFACT

The artifact is the central physical object and narrative device.

It must NOT resemble the Hellraiser/Lament Configuration puzzle box in visual design.

Current direction:
- Palm-sized
- Mechanical / ancient / inexplicable
- Potentially spherical, irregular, or ring-based rather than cubic
- Interlocking rings, plates, catches, sliders, markings, or nested mechanisms
- Satisfying mechanical feedback

Potential interaction vocabulary:
1. Grab with one or two hands.
2. Rotate an outer mechanism.
3. Snap a component into alignment.
4. Press opposing surfaces.
5. Pull or separate a mechanism.
6. Align markings.
7. Final mechanical CLACK.

The opening interaction quietly teaches the hand mechanics needed later.

Narrative truth:

THE ARTIFACT IS NOT A KEY.
THE ARTIFACT IS A LOCK.

The player believes they solved it.
They actually unlocked it.

The finale should require knowledge learned while opening it, ideally reversing or reconfiguring the artifact while under pressure.

## 8. THE BREACH

The breach appears in/through the table after the artifact is unlocked.

It must feel spatially impossible rather than like a flat black texture.

Desired experience:
- Table begins normal.
- Subtle physical/audio disturbances precede the opening.
- Crack or deformation appears.
- Opening reveals genuine perceived depth.
- Player can lean over and look down.
- Space below appears dramatically larger/deeper than the basement could physically contain.
- Movement far below hints at scale before creatures arrive.

Technical implementation may cheat aggressively.

Possible cheats:
- Portal camera/render texture
- Masked geometry
- Secondary hidden environment
- Depth illusion
- Controlled scripted vista

The illusion matters more than physically simulating an infinite environment.

## 9. CREATURES

Creature direction:
- Small practical-effects-style monsters
- Strong silhouettes
- Chunky proportions
- Expressive movement
- Creepy/funny balance
- Readable on Quest hardware
- Limited behavior set with strong animation and reactions

We may use properly licensed third-party creature assets as a foundation.

Keep a THIRD_PARTY_ASSETS.md file documenting:
- Asset/package name
- Creator/publisher
- Source/store
- License
- Acquisition date
- How UNDER uses/modifies it

AI-generated content should not become visual sludge. Prefer coherent art direction and deliberately selected/licensed assets over generating everything merely because it is possible.

Initial creature beat:
- Something climbs toward the breach.
- Small fingers appear.
- Creature pulls itself into the basement.
- Brief curious/comedic recognition between player and creature.
- Creature notices something valuable, potentially an artifact component.
- Creature steals it or interferes and runs.

Creature behaviors should be few and reusable. Target roughly three core behaviors rather than many enemy types.

Examples:
- Climb/emerge
- Steal/interfere/flee
- Grab/cling/block

When grabbed by the player, a creature can transition from authored behavior/animation to a controlled physical/grab state, then use release velocity when thrown.

## 10. EXPERIENCE ARC

Target: 8–10 minutes.

### Beat 1 — Arrival
Player is seated in the basement.
Hands are available.
Artifact is on the table.
No conventional tutorial.

### Beat 2 — Curiosity
Player discovers the artifact's mechanisms.
Interaction teaches grab, rotate, press, pull, and two-handed manipulation.

### Beat 3 — Unlock
Artifact reaches its solved configuration.
CLACK.
Silence.

### Beat 4 — Something Under the Table
A knock.
Then another.
Sound establishes that it is below the player/table.

### Beat 5 — The Impossible Breach
The table changes.
The breach opens.
Player physically leans over and sees impossible depth.

### Beat 6 — First Creature
A small creature climbs out.
Moment begins strange/cute/funny before becoming a problem.

### Beat 7 — Escalation
More interference.
Creatures steal, cling, grab, block, or interfere with the artifact.
Player grabs and throws creatures and tries to understand how to reseal the lock.

### Beat 8 — The Large Presence
Do not build a fully simulated giant monster unless absolutely necessary.
Use partial anatomy, silhouette, sound, scale, scripted hands/arms/head, or movement below the breach.
Something much larger attempts to emerge.

### Beat 9 — Desperate Seal
Current possible direction:
- A fictional dangerous charge / pyrotechnic / alchemical device is established earlier.
- Player sends it into the breach.
- Light/sound/force disrupts the large creature.
- Player physically forces the breach together.
- Player restores/reverses the artifact lock using skills learned in the opening.

Opening symmetry:
Two hands help open the problem.
Two hands desperately close it.

### Beat 10 — Silence
Everything returns to normal enough to breathe.

Then:

A tiny knock from beneath the table.

End.

## 11. SCOPE LOCK

Unless explicitly approved, DO NOT ADD:
- Open world
- Multiple levels
- Inventory system
- Crafting
- Skill trees
- Character classes
- Weapon arsenal
- Procedural progression
- Roguelike meta-progression
- Large narrative dialogue system
- Multiplayer
- Online backend
- Accounts
- Economy
- Collectible grind
- Multiple major bosses
- Large enemy roster
- Unnecessary menus
- Generic wave-shooter combat

Current content budget:
- One basement
- One table
- One artifact
- One breach
- Approximately three reusable small-creature behaviors
- One scripted large-creature finale
- One complete 8–10 minute experience

New ideas are not automatically features.
Record good ideas in a backlog rather than implementing them.

## 12. INTERACTION AND COMFORT

Primary mode is seated/stationary.

Important interactions should be reachable comfortably around the table. Do not require the player to walk around a physical room.

Avoid interactions that require:
- Reaching dangerously far
- Repeated floor-level bending
- Turning completely around without reason
- Precise tiny finger actions unsupported by reliable tracking
- Long arm holds that create fatigue

Hand interactions should have forgiving targets, sensible snapping, and strong audiovisual feedback.

Reliability beats simulation purity.

## 13. ART DIRECTION

Target feeling:
“An original late-80s creature feature that somehow became an interactive VR toy.”

Priorities:
1. Strong silhouette
2. Readability in headset
3. Coherent period aesthetic
4. Tactile materials
5. Deliberate lighting
6. Performance
7. Detail

Avoid:
- Generic photoreal horror
- Random asset-store collage
- Excessive darkness hiding weak art
- Modern objects accidentally breaking the period
- Inconsistent AI-generated visual styles

Stylization is welcome.

## 14. AUDIO DIRECTION

Audio is critical.

Important motifs:
- Mechanical artifact clicks
- Silence after solving
- Knock from below
- Scratching beneath table
- Distant impossible ambience
- Creature scurrying
- CRT hum
- Cassette/stereo ambience
- Sudden interruption of comfortable room tone
- Large low-frequency movement from far below

Spatial audio should help the player understand where to look without HUD arrows.

## 15. CODEX WORKING RULES

When implementing a task:

1. Read this file.
2. Inspect existing project state before modifying it.
3. Make the smallest coherent change that satisfies the requested acceptance test.
4. Reuse existing working XR infrastructure where sensible.
5. Do not install packages without a clear technical need.
6. Do not silently replace working systems because another architecture seems cleaner.
7. Do not expand scope.
8. Do not create art/content that was not requested merely to make a prototype look complete.
9. Prefer reversible changes.
10. Keep scripts focused and named clearly.
11. Check the Unity Console after changes.
12. Do not leave compile errors.
13. Report warnings that may matter on Quest.
14. Save modified scenes/assets.
15. Report files/scenes/settings changed.
16. Stop when the requested acceptance criteria are met.

If blocked:
- Diagnose first.
- Report the exact blocker.
- Propose the smallest next test.
- Do not respond by building a different large system.

## 16. PROTOTYPE GATES

Development proceeds through gates.

Do not jump ahead simply because later content is exciting.

### FOUNDATION 001 — Hands + Table
Goal:
Standalone Quest 3 full-VR scene.

Acceptance:
- Virtual environment visible; no passthrough.
- Seated/stationary XR origin.
- Tracked hands function.
- Table positioned comfortably.
- One cube rests on table.
- Player can grab, move, and release cube using hands.
- Runs natively on Quest 3 at acceptable performance.
- No compile errors.

### FOUNDATION 002 — The Hole
Goal:
Prove UNDER's signature spatial illusion.

Acceptance:
- Breach exists in/through table.
- Player can comfortably lean over it.
- It appears substantially deeper/larger than physical table geometry.
- Something can exist below the table and be perceived through breach.
- Experience remains comfortable and performant.

Human test:
The player should instinctively want to lean over and look into it.

### FOUNDATION 003 — Reach Into UNDER
Goal:
Prove interaction across breach boundary.

Acceptance:
- Object exists below opening.
- Player reaches through opening.
- Hand interaction remains stable.
- Player grabs object below and pulls it into basement.
- Object behaves correctly after extraction.

This is the first major technical kill test.

### FOUNDATION 004 — Artifact
Goal:
Prove tactile puzzle language.

Acceptance:
- Placeholder artifact supports several reliable hand interactions.
- Clear mechanical feedback.
- Ordered state progression.
- Final solved event.
- No narrative polish required.

### FOUNDATION 005 — Unlock
Goal:
Connect artifact solve to breach opening.

### FOUNDATION 006 — First Creature
Goal:
One creature climbs out and can be physically handled.

### FOUNDATION 007 — Creature Physicality
Goal:
Grab, hold, release/throw, and reliable state transitions.

### FOUNDATION 008 — Escalation
Goal:
Small set of creature behaviors create controlled chaos.

### FOUNDATION 009 — Finale
Goal:
Large-presence sequence and physical resealing.

### FOUNDATION 010 — Polish
Goal:
Replace prototype feel with authored experience:
- Art
- Sound
- Lighting
- Animation
- Feedback
- Performance
- Onboarding
- Build reliability
- Submission capture

## 17. CURRENT NEXT TASK

FOUNDATION 001 ONLY.

Create a minimal full-VR Quest 3 prototype from the existing Unity project.

Preserve:
- Working Meta Quest/OpenXR configuration
- XR Origin as appropriate
- XR Hands
- XR Interaction Toolkit infrastructure
- Anything required for standalone Quest deployment

Remove or disable where safe:
- Passthrough
- AR plane detection
- Scene understanding
- Spatial permissions
- MR sample UI
- MR sample spawning/demo content

Create:
- Scene: UNDER_Prototype
- Seated/stationary XR setup
- Neutral opaque environment
- Simple floor
- Simple table
- One grabbable cube

Do NOT create:
- Basement art
- Artifact
- Breach
- Monsters
- Game loop
- Menus
- Audio
- Additional gameplay

After implementation:
- Save scene.
- Add it to Android/Quest build scenes.
- Check console.
- Report changes.
- STOP.

## 18. NORTH STAR

UNDER succeeds when a player physically leans over an ordinary table, looks into an impossible world beneath it, and immediately wants someone else to put on the headset.

Build toward that moment.

Finish the game.
