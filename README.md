# Bloom & Grow

A flower-shop customer-order sim in the spirit of *Good Pizza, Great Pizza*.
Run the counter, build bouquets to match each customer's (sometimes vague,
sometimes very specific) request, haggle over price, and grow the shop.
See `docs/GDD.md` for the full design.

## Project status

This is an early scaffold, not a finished game:

- The gameplay rules (scoring, haggling, wilting/inventory, reputation,
  procedural customer generation, day/season cycle) are fully implemented
  in `Assets/Scripts/Core/` as plain, engine-independent C#, and covered by
  the EditMode test suite in `Assets/Tests/EditMode/`.
- A thin Unity integration layer (`Assets/Scripts/Gameplay/`) wires that
  logic up to ScriptableObject content, a `GameManager`, and a
  drag-and-drop workspace + UI bindings.
- There's no scene, art, or audio yet - those need to be authored in the
  Unity Editor (this scaffold was built without one available). See
  "Next steps" below.

## Requirements

- Unity **2022.3 LTS** (the project was bootstrapped against 2022.3.50f1;
  any 2022.3.x patch should open it fine).

## Opening the project

1. Open Unity Hub, "Add project", point it at this folder.
2. Let Unity import and generate the missing `Library/` cache and default
   `ProjectSettings/*.asset` files (only `ProjectVersion.txt` is committed;
   everything else Unity fills in with defaults on first open).
3. Create a new empty scene and add a `GameManager` (from
   `FloristGame.Gameplay`) to a GameObject.
4. Run **Florist Game > Generate Starter Content** from the Editor menu
   (`Assets/Editor/StarterContentGenerator.cs`). This creates a starter set
   of 14 flowers, 11 occasion profiles, 5 wrapping types, and 5 vase types
   as ScriptableObject assets under `Assets/Data/Generated/`.
5. Drag the generated `FlowerDefinition` and `OccasionDefinition` assets
   into the `GameManager`'s `flowerCatalog` / `occasionCatalog` lists.
6. Build out a UI scene (canvas, drop zone, draggable flower chips, haggle
   panel, HUD) wiring up to `WorkspaceController`, `HaggleController`, and
   the `*UI` scripts in `Assets/Scripts/Gameplay/UI/` - none of that exists
   as a scene yet.

## Running the tests

Window > General > Test Runner > EditMode > Run All. These tests exercise
`FloristGame.Core` directly (no scene or play mode required):

- `ArrangementScorerTests` - budget/color/meaning/freshness/compliance scoring.
- `HaggleNegotiatorTests` - accept/counter/walk-away behavior.
- `InventoryManagerTests` - FIFO consumption and wilting.
- `ReputationTrackerTests` - reputation deltas and unlock gating.
- `RequestGeneratorTests` - difficulty-tier boundaries and generated-request invariants.

## Project layout

```
Assets/
  Scripts/
    Core/       - engine-independent gameplay rules (FloristGame.Core.asmdef, noEngineReferences)
    Gameplay/   - Unity layer: ScriptableObject data, GameManager, drag-and-drop, UI
  Editor/       - StarterContentGenerator (Florist Game > Generate Starter Content)
  Tests/EditMode/ - NUnit tests for Core
docs/
  GDD.md        - full game design document
```

## Next steps

- Build the actual gameplay scene (canvas layout, drop zone, chip prefabs).
- Source or commission art: flower icons, customer portraits per mood,
  vase/wrapping sprites, shop background.
- Add ambient audio (bell chime, scissor snip, paper wrapping) and hook it
  into the relevant events on `GameManager`.
- Shop upgrade UI spending `EconomyLedger.Cash` against `ShopUpgrade` entries.
- Story/episodic layer on top of the core loop.
