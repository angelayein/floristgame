# Bloom & Grow - Art Brief

Style target (from the GDD): warm, cozy pixel-art or soft painterly - think
inviting neighborhood flower shop, not photoreal. Whichever direction is
picked, keep it **consistent across every category below** - portraits,
flowers, and props all need to read as the same world.

All assets: PNG with alpha (transparent background), no baked-in drop
shadows/backgrounds unless noted.

---

## 1. Customer portraits (highest priority)

Six customer archetypes are scripted in, each needs a set of mood
portraits. The game swaps portraits live based on how the player's
arrangement scores, so **all six moods can appear for any archetype** -
it's not the player's/designer's choice which mood shows up.

Moods: `Neutral`, `Happy`, `Delighted`, `Confused`, `Disappointed`, `Offended`

**Technical spec:** square canvas, bust/headshot framing, same crop and
camera angle across all 6 moods of one archetype so swapping doesn't jump
around. Recommend 512x512.

**Naming convention:** `{ArchetypeName}_{Mood}.png`, e.g. `RomanticPartner_Happy.png`.

**Fallback behavior:** if a mood isn't provided, the game shows whichever
portrait was authored first for that archetype - so if art has to be
prioritized, make sure `Neutral` is always included and imported first.

Below is each archetype with the moods already described and the gaps to
fill. Priority order = the ones already described (game needs those
regardless) > gaps.

### 1. Romantic Partner (`RomanticPartner`)
Someone buying for a significant other. Values beauty, symbolism, emotional meaning.

| Mood | Direction |
|---|---|
| Neutral | soft smile, casual outfit |
| Happy | holding bouquet, excited expression |
| Delighted | teary-eyed, impressed |
| Disappointed | worried expression |
| Confused | *(needed - not yet described)* |
| Offended | *(needed - not yet described)* |

Barks: "I want something that says 'I love you' without being too obvious." / "She loves elegant flowers, but I don't know where to start." / "Could you make it feel special?" / "I hope they like it..."

### 2. Busy Professional (`BusyProfessional`)
In a rush, wants something reliable and efficient.

| Mood | Direction |
|---|---|
| Neutral | business clothes, checking watch |
| Happy | relieved smile |
| Confused | overwhelmed expression |
| Delighted | *(needed)* |
| Disappointed | *(needed)* |
| Offended | *(needed)* |

Barks: "I have a meeting soon, so I need something quick." / "Can you make it look impressive?" / "I trust your judgment." / "This took longer than I expected..."

### 3. Cheerful Grandparent (`CheerfulGrandparent`)
Warm, nostalgic. Loves classic flowers and thoughtful arrangements.

| Mood | Direction |
|---|---|
| Neutral | kind elderly person |
| Happy | warm smile |
| Delighted | laughing |
| Confused | *(needed)* |
| Disappointed | *(needed)* |
| Offended | *(needed)* |

Barks: "Flowers always remind me of my garden." / "Surprise me with something lovely." / "This reminds me of springtime!" / "It's beautiful, just like the old days."

### 4. Trendy Influencer (`TrendyInfluencer`)
Cares about aesthetics, uniqueness, social media appeal.

| Mood | Direction |
|---|---|
| Neutral | stylish outfit, phone in hand |
| Happy | taking photos |
| Delighted | posting the bouquet |
| Confused | *(needed)* |
| Disappointed | *(needed)* |
| Offended | *(needed)* |

Barks: "I need something that will look amazing in photos." / "What's the most unique flower you have?" / "Make it match my aesthetic." / "This is totally going on my feed!"

### 5. Nervous First-Time Buyer (`FirstTimeBuyer`)
Doesn't know much about flowers, needs guidance.

| Mood | Direction |
|---|---|
| Neutral | unsure expression |
| Happy | confident after advice |
| Confused | overwhelmed by choices |
| Delighted | *(needed)* |
| Disappointed | *(needed)* |
| Offended | *(needed)* |

Barks: "I've never bought flowers before..." / "Are these flowers okay together?" / "I just want them to like it." / "Thank you for helping me!"

### 6. Luxury Client (`LuxuryClient`)
Expensive tastes, expects premium quality.

| Mood | Direction |
|---|---|
| Neutral | elegant clothing |
| Happy | impressed |
| Delighted | approving nod |
| Offended | unimpressed |
| Confused | *(needed)* |
| Disappointed | *(needed)* |

Barks: "I'm looking for something truly exceptional." / "Quality matters more than price." / "Surprise me with your finest work." / "I expected something more refined."

**Total: 6 archetypes x 6 moods = 36 portraits** for full coverage. If that's too much for a first pass, ship each archetype's already-described moods first (18 images) and backfill the gaps - the fallback behavior above means nothing breaks, it'll just show the wrong expression until filled in.

---

## 2. Flower stem icons (14 species)

These are the drag-and-drop chips in the shop's stock tray. Small,
consistent icon style (same lighting/angle/scale across all 14 so the
tray reads as one set).

**Technical spec:** square canvas, recommend 128x128 or 256x256, single
stem or small cluster, transparent background.

**Naming convention:** `{species_id}.png` (matches the ids below exactly, so import is a straight drag-in).

| id | Name | Color |
|---|---|---|
| `rose_red` | Red Rose | Red |
| `rose_yellow` | Yellow Rose | Yellow |
| `rose_white` | White Rose | White |
| `lily_stargazer` | Stargazer Lily | Pink |
| `daisy` | Daisy | White |
| `sunflower` | Sunflower | Yellow |
| `tulip` | Tulip | Red |
| `carnation` | Carnation | Pink |
| `orchid` | Orchid | Purple |
| `babys_breath` | Baby's Breath | White |
| `chrysanthemum` | Chrysanthemum | Orange |
| `marigold` | Marigold | Orange |
| `peony` | Peony | Pink |
| `iris` | Iris | Blue |

(Nice-to-have, not required for launch: a visibly "wilted" variant per
flower - the game has a freshness/wilting mechanic and currently only
shows a fill-bar for it, but a droopy/browning version of each icon at
low freshness would sell the mechanic much better if there's time.)

---

## 3. Wrapping paper swatches (5)

Small icon per option, shown in a selector row.

| id | Name |
|---|---|
| `Kraft` | Kraft Paper |
| `Tissue` | Tissue Paper |
| `Cellophane` | Cellophane |
| `Ribbon` | Ribbon Wrap |
| `LuxuryBox` | Luxury Box |

Same icon spec as flowers (square, transparent, same scale as each other).

## 4. Vase icons (5)

| id | Name |
|---|---|
| `Basic` | Basic Vase |
| `Mason` | Mason Jar |
| `Glass` | Glass Vase |
| `Ceramic` | Ceramic Vase |
| `Crystal` | Crystal Vase |

Same icon spec as flowers.

---

## 5. Shop chrome (lower priority, no code depends on these yet)

- Shop interior background (counter, cooler, window - the panel behind all the UI)
- Bell/register icon for the cash display
- A simple heart/star icon for the reputation bar fill

---

## What's *not* needed yet (so effort isn't wasted)

- **No rendered "finished bouquet" art.** The game currently tracks the
  built arrangement as data (which flowers, wrapping, vase, style) but
  doesn't compose them into a single bouquet image - that's a future
  system, not built yet. Don't spend time on a bouquet-compositing asset
  set until that renderer exists.
- **Arrangement styles** (Round / Cascade / Minimalist / Wild) are
  currently just labels the player picks, with no unique art per style.

---

## Naming recap (so nothing needs renaming on import)

- Portraits: `{ArchetypeName}_{Mood}.png` (e.g. `LuxuryClient_Offended.png`)
- Flowers: `{species_id}.png` (e.g. `rose_red.png`)
- Wrappings: `Wrapping_{id}.png` (e.g. `Wrapping_Kraft.png`)
- Vases: `Vase_{id}.png` (e.g. `Vase_Crystal.png`)
