# Bloom & Grow - Game Design Document

## Working titles
"Bloom & Grow" or "Petal Perfect"

## Core concept

You run a small flower shop. Customers walk up to the counter with requests -
some vague, some hyper-specific, some emotional - you assemble a bouquet from
available stock, they react based on hidden preferences, you haggle over
price, and you use earnings to grow the shop while unlocking new flowers and
a light story about the shop's history.

Inspiration: *Good Pizza, Great Pizza*, reskinned around the far richer
design space flowers offer (meaning, seasonality, decay, color/composition)
instead of topping placement.

## Core gameplay loop

1. **Customer arrives** with a request, ranging from simple ("something
   red") to specific ("a dozen roses, no baby's breath, budget $30") to
   emotional ("flowers for my mother's funeral").
2. **Build the arrangement** - drag flowers, greenery, wrapping, and a vase
   into the workspace.
3. **Customer reacts** - checked against hidden preferences: budget, color
   mood, flower meaning, allergy, occasion fit.
4. **Negotiate price** - push back if lowballed, same haggle-loop feel as
   the pizza game.
5. **Get paid + reputation change** - money funds upgrades; reputation
   unlocks new customer types and story beats.
6. **Between customers** - restock inventory, watch stock wilt, upgrade the
   shop, respond to story events.

## Systems that differentiate this from the pizza game

- **Wilting/freshness** - stock loses freshness every in-game day. Over-order
  and it wilts on the shelf (wasted cost); under-order and you can't fill
  requests. Implemented as FIFO batches per species
  (`Assets/Scripts/Core/Economy/InventoryManager.cs`).
- **Language of flowers** - every species has a primary (and optional
  secondary) meaning. Occasions declare expected meanings and taboo
  meanings; getting it wrong (yellow roses for romance, lilies for a
  birthday) tanks the "meaning fit" score and is a great source of both
  comedy and difficulty
  (`Assets/Scripts/Core/Scoring/ArrangementScorer.cs`).
- **Seasonal availability** - species declare which seasons they're in
  season for, forcing substitution and pricing strategy across the
  Spring/Summer/Fall/Winter cycle (`Assets/Scripts/Core/GameState/DayCycleController.cs`).
- **Color theory / composition** - customers can ask for a color palette
  and an arrangement style (Round, Cascade, Minimalist, Wild) instead of
  pizza's topping placement.
- **Occasions system** - Everyday, Romance, Apology, Sympathy, Wedding,
  Birthday, Anniversary, Graduation, Get Well, New Baby, Prom - each with
  its own typical budget, forgiveness level, and meaning rules.

## Difficulty curve

Reputation gates a `DifficultyTier` (`Assets/Scripts/Core/Generation/DifficultyTier.cs`):

- **Simple** (rep < 30): forgiving, vague requests, high patience, small
  orders.
- **Intermediate** (rep 30-60): allergy-sensitive customers start
  appearing, moderate haggling, exclusion requests ("no baby's breath").
- **Advanced** (rep 60-85): specific style/required-species requests,
  lower patience, bigger minimum orders.
- **VIP** (rep >= 85): large multi-bouquet-scale orders, least patience,
  least vagueness - the wedding-planner / mystery-shopper tier.

## Progression & monetization

- **Shop upgrades** (`Assets/Scripts/Core/Economy/ShopUpgrade.cs`): bigger
  cooler (more inventory slots), better register (faster haggling), design
  table upgrades (more simultaneous flower types), delivery van (off-site
  orders like weddings).
- **Unlockables**: new flower species, vase types, wrapping styles,
  seasonal decor - gated by reputation thresholds via
  `ReputationTracker.OnUnlockGranted`.
- **Monetization (if F2P)**: cosmetic shop skins, optional rush-delivery
  boosts, ad-watch for bonus stock. Explicitly **not** pay-to-win on the
  core arrangement/scoring mechanics - keep the skill-based loop fair.
- **Story mode**: episodic chapters (inheriting the shop, saving it from
  closure, a citywide flower festival) layered on top of the core loop.

## Art & tone

Warm, cozy pixel-art or soft painterly style. Charming customer portraits
with exaggerated mood reactions (delighted/offended/confused/disappointed -
see `FloristGame.Data.CustomerMood`). Ambient shop sounds: bell chime,
scissors snipping, paper wrapping.

## Architecture

Gameplay rules live in `FloristGame.Core` - plain C#, zero UnityEngine
dependency (`"noEngineReferences": true` in its asmdef) so they're fully
unit-testable without the Editor:

- `Flowers/` - species, color, meaning, season vocabulary.
- `Arrangements/` - the bouquet the player builds (stems, wrapping, vase, style).
- `Customers/` - occasion profiles and generated customer requests.
- `Scoring/` - `ArrangementScorer`, the heart of the game: turns a request +
  arrangement + price into a 0-100 satisfaction score and human-readable notes.
- `Economy/` - inventory/wilting, haggle negotiation, reputation, cash ledger,
  shop upgrades.
- `Generation/` - procedural `RequestGenerator`, scaling with `DifficultyTier`.
- `GameState/` - day/season cycle.

`FloristGame.Gameplay` is the thin Unity layer on top: ScriptableObject data
definitions (`Data/`) that convert into Core types, the `GameManager`
orchestrator, drag-and-drop workspace interaction, and UI bindings.

See `README.md` for how to open the project and run the test suite.
