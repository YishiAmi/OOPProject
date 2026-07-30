# World Layer — Design Notes and Personal Explanation

## What I am trying to achieve

The `RpgLibrary.World` namespace is the part of the library that connects the
game's locations, NPCs, quests, shops, and overall player-facing game state.
The project will eventually be consumed by a WinForms demo and exported as
`RpgLibrary.dll`, so I designed this layer as reusable game logic rather than
as a console program.

The most important decision is that the library does not decide how anything
looks. It changes game state and returns data. The WinForms project decides
whether that data appears in a label, dialogue box, list, progress bar, or
message box.

For example, the library should return `PurchaseStatus.InsufficientGold`.
It should not call:

```csharp
Console.WriteLine("Not enough gold.");
```

The form can translate that status into any presentation it wants. This also
makes the DLL usable by another UI in the future without rewriting the World
classes.

When I use the word "API" in these notes, I mean the public C# methods,
properties, result records, and enums exposed by the DLL. This project does
not need an HTTP or web API for WinForms.

## Architectural boundaries I am preserving

`GameManager` is the facade for player-driven, top-level game operations. A
form should ask `GameManager` to accept a quest, complete a quest, travel, or
buy an item instead of changing several domain objects itself.

Methods such as `Map.AddNpc`, `NPC.AssignQuest`, and `Shop.AddStock` remain
public because a DLL consumer needs a way to construct and configure a world.
I treat those as setup or authoring APIs, not player actions. After a play
session starts, the form should route player actions through `GameManager`.
This is a usage boundary rather than an authorization/security boundary.

The World namespace uses the shared contracts:

- `ICombatant` for the player and map enemies.
- `IShopItem` for anything a shop can stock.

World does not depend on concrete classes from the `Combat` or `Items`
folders. This means World can work with any future combatant or shop item as
long as that object correctly implements the shared interface.

I have not changed any shared contract. Contract changes affect multiple team
members and must be agreed upon explicitly.

Every public type remains in its own file. That makes the project structure
predictable and follows the repository's one-class-per-file rule.

## Why state is encapsulated

Originally, several collections and properties were publicly mutable. For
example, callers could directly set a shop quantity to `-50`, replace an
NPC's quest, or clear a map's enemy list without going through a meaningful
operation.

The new design uses:

- `private set` when an object should expose state but control its mutation.
- `IReadOnlyList<T>` when consumers should inspect a collection but not edit
  the underlying list directly.
- Explicit methods such as `AddNpc`, `AddStock`, and `AssignQuest` when a
  state change requires validation.
- `internal` methods for operations that should only be coordinated inside
  the library, normally by `GameManager` or `Shop`.

Read-only does not mean the game can never change. It means changes happen
through methods that preserve the object's rules.

## Quest lifecycle

A quest now moves through a simple lifecycle:

```text
Available -> Active -> Completed
```

- `Available`: an NPC may offer the quest, but the player has not accepted it.
- `Active`: the quest is in the player's active quest log and can gain
  progress.
- `Completed`: its requirements were met and its reward was issued.

This lifecycle fixes two important problems. A quest cannot be completed
before it is accepted, and its gold reward cannot be awarded repeatedly.

### `Quest`

`Quest` remains abstract because the game should create a `MainQuest` or
`SideQuest`, not an undefined generic quest.

Its constructor is `protected`. Derived quest classes can call it, but
application code cannot instantiate `Quest` directly.

The class owns the common quest data:

- `Title`: the name shown in the quest log.
- `Objective`: a human-readable description of the goal.
- `GoldReward`: the amount awarded once on successful completion.
- `RequiredProgress`: the total work required.
- `CurrentProgress`: the work reported so far.
- `Status`: the current lifecycle state.
- `Type`: implemented by each derived quest type.

`Completed` remains as a convenient calculated property for UI binding and
simple checks. It is derived from `Status`, so there are not two independent
values that could disagree.

`CanComplete` is also calculated. It is true only when the quest is active and
its current progress has reached the requirement.

Progress is intentionally generic. World does not need to know whether one
point means defeating an enemy, collecting an item, visiting a map, or
talking to an NPC. The appropriate game code reports progress to
`GameManager`.

The mutation methods are `internal`:

- `Accept()` changes `Available` to `Active`.
- `AddProgress(int)` increases and clamps progress at the requirement.
- `Complete()` changes a ready active quest to `Completed`.

This is deliberate. A WinForms consumer should call the facade rather than
calling these lifecycle operations directly.

Constructor validation rejects:

- Missing titles.
- Missing objectives.
- Negative gold rewards.
- Progress requirements below one.

These are programmer/configuration errors, not normal gameplay outcomes, so
exceptions are appropriate.

### `MainQuest`

`MainQuest` is sealed because it is already the concrete main-story quest
type. It calls the base constructor and identifies itself as
`QuestType.Main`.

It adds `Chapter`, which allows the UI to group and order the main story.
Chapter numbers must start at one.

The constructor requires `chapter` before its optional `requiredProgress`:

```csharp
var quest = new MainQuest(
    "The First Crystal",
    "Recover the crystal from the ruins.",
    goldReward: 100,
    chapter: 1,
    requiredProgress: 3);
```

Named arguments make content setup clear and avoid confusing two integer
parameters.

Using `QuestType.Main` is cleaner than making every consumer write:

```csharp
if (quest is MainQuest)
```

The form can switch on the enum to select an icon, color, or quest-log tab.

### `SideQuest`

`SideQuest` also calls the base constructor and identifies itself as
`QuestType.Side`.

It adds `Region`, which gives the quest a useful world-specific grouping.
WinForms can filter optional quests by the region the player is exploring.

I intentionally did not make side quests repeatable yet. Repeatability affects
status resets and reward rules, so it should be introduced only when those
rules are clearly defined.

### `QuestStatus` and `QuestType`

These enums prevent the rest of the program from relying on magic strings
such as `"done"` or `"main"`. Enum values are compiler-checked and work well
in `switch` expressions in WinForms.

## NPC design

An `NPC` owns a name, dialogue, and an optional offered quest.

`Talk()` already had the correct basic shape because it returned a string
instead of printing it. It remains a small read API.

`OfferedQuest` now has a private setter. Quest assignment happens through:

- The optional constructor argument.
- `AssignQuest(Quest)`.
- `RemoveQuest()`.

This prevents unrestricted property replacement while still allowing the
world to change which quest an NPC offers.

The class validates that the name and dialogue are not blank. An NPC without
a quest remains valid because not every NPC needs to be a quest giver.

For now, an NPC offers one quest. If the final game requires several quests
from one NPC, this can later become a controlled read-only collection.

## Map design

A `Map` is a container for:

- Its name and difficulty.
- NPCs.
- Enemies represented by `ICombatant`.
- An optional local shop.

The NPC and enemy lists are private. Their public properties are
`IReadOnlyList<T>` views. Setup and runtime changes use:

- `AddNpc` and `RemoveNpc`.
- `AddEnemy` and `RemoveEnemy`.
- `SetLocalShop`.

`ContainsNpc` and `ContainsEnemy` support facade validation. For example,
`GameManager` can reject a request to talk to an NPC who is not present on
the current map.

Add methods ignore duplicate object references. This keeps the same NPC or
enemy instance from being inserted twice accidentally.

Map difficulty currently accepts any integer of one or higher. If the team
later decides on a fixed scale such as 1–10, the upper-bound validation can
be added without changing the overall API.

## Shop and stock design

The shop system has two responsibilities:

- `Shop` owns the collection of stock slots.
- `ShopSlot` owns the quantity for one `IShopItem`.

Player gold is not owned by either class. It belongs to `GameManager`.

### `ShopSlot`

`ShopSlot.Item` is an `IShopItem`, preserving the contract boundary.

`Quantity` has a private setter and `IsInStock` is calculated from it.
Only controlled internal methods can change quantity:

- `Restock(int)`.
- `TryTakeOne()`.
- `TryRemove(int)`.

This prevents negative stock and gives the WinForms UI a direct value for
enabling or disabling the Buy button.

The slot accepts a starting quantity of zero so a shop may retain and display
a sold-out item. Restocking operations require at least one unit.

### `Shop`

`Shop.Inventory` is a read-only view of its slots. Consumers can bind the
slots to a list or grid, but stock changes go through the shop.

`AddStock` either restocks the slot holding the same item instance or creates
a new slot. Reference identity is used because `IShopItem` does not define a
stable item ID or equality contract. Two different item instances are
therefore treated as separate stock entries even if their names match.

`RemoveStock` decreases an existing slot only when enough units are
available.

`TryTake` is internal because the public purchase operation belongs to the
facade. It validates:

1. The selected slot belongs to this shop.
2. The item is in stock.
3. The player has enough gold.
4. Exactly one unit can be removed.

It captures the item's price once for the transaction and returns that
captured amount to `GameManager`. This matters because `IShopItem` only
guarantees a getter; a concrete item class could still expose a mutable price.
Affordability, stock removal, and wallet debit therefore use the same value.

The previous `Buy(IShopItem, ref int)` method was removed because it accepted
items that were not stocked, did not decrement quantity, and mixed shop state
with player wallet mutation.

Selecting a `ShopSlot` rather than searching by item name also avoids
ambiguity when two items share a display name.

## Result records and status enums

Expected gameplay failures are returned as statuses. Invalid arguments,
invalid object configuration, and impossible numeric overflow may still throw
exceptions.

This distinction is important:

- A negative starting-gold configuration is a programmer error, so the
  constructor throws.
- A player not having enough gold is a normal gameplay result, so the method
  returns `PurchaseStatus.InsufficientGold`.

### `NpcInteractionResult`

`TalkToNpc` returns:

- A status.
- The selected NPC.
- The dialogue string.
- The offered quest, if any.
- Whether that quest can currently be accepted.

`NpcInteractionStatus` contains:

- `Success`.
- `NpcNotInCurrentMap`.

This gives the form everything required to display dialogue and control its
Accept Quest button without reading or parsing console text.

### `QuestActionResult`

Quest facade methods return:

- `Status`.
- The relevant quest, when one exists.
- Gold awarded by that operation.
- The player's current gold balance.

`QuestActionStatus` contains:

- `Success`.
- `NpcNotInCurrentMap`.
- `NoQuestOffered`.
- `AlreadyAccepted`.
- `QuestNotActive`.
- `InvalidProgress`.
- `RequirementsNotMet`.
- `AlreadyCompleted`.

The `Succeeded` property is a convenience for callers that only need a
yes-or-no branch. The complete status remains available when the UI needs a
specific message.

### `PurchaseResult`

A purchase returns:

- `PurchaseStatus`.
- The selected `IShopItem`.
- The exact `PricePaid`, captured during the transaction.
- The remaining gold.

`PurchaseStatus` contains:

- `Success`.
- `ItemNotSold`.
- `OutOfStock`.
- `InsufficientGold`.
- `ShopUnavailable`.

As with quest results, `Succeeded` provides a convenient success check.

## `GameManager` as the facade

`GameManager` owns the current top-level World state:

- The player as `ICombatant`.
- The current map.
- Player gold.
- Active quests.
- Completed quests.

The quest collections use read-only views. The form can render the quest log
but cannot bypass the quest lifecycle by adding arbitrary entries.

### Construction

The constructor requires a player and starting map and accepts optional
starting gold:

```csharp
var gameManager = new GameManager(player, startingMap, startingGold: 100);
```

The previous hard-coded testing balance was removed. Starting gold is now
chosen by the consuming game and cannot be negative.

### Travel

`ChangeMap(Map)` validates and stores the destination, then returns the new
current map. It does not print a travel message.

The form can use the return value:

```csharp
Map destination = gameManager.ChangeMap(selectedMap);
mapNameLabel.Text = destination.MapName;
```

The current version allows travel to any supplied map. A world graph or map
registry can be introduced later if the game needs locked routes.

### Talking and accepting quests

`TalkToNpc(NPC)` first verifies that the NPC belongs to the current map.
Successful interaction returns dialogue and quest availability.

Talking does not automatically accept a quest. Dialogue and acceptance are
separate user decisions in a graphical UI.

`AcceptQuest(NPC)` verifies:

1. The NPC is on the current map.
2. The NPC offers a quest.
3. The quest is neither active nor completed.

On success, it changes the quest to `Active` and adds it to the active quest
log.

### Reporting progress

`AddQuestProgress(Quest, int)` requires a positive amount and an active quest
owned by this manager.

The progress is clamped at the requirement. Reporting 10 points to a quest
that needs 5 produces `5/5`, not `10/5`.

The method does not automatically complete the quest. Reaching the objective
and turning it in remain separate actions, which works naturally for JRPG
quest-giver interactions.

In this version, "turning it in" means making a separate
`CompleteQuest(Quest)` call. It does not require the player to be on the
offering NPC's map. If location-bound turn-ins are required later, the facade
should accept the NPC as part of the completion command and validate it.

### Completing quests

`CompleteQuest(Quest)` verifies that the quest is active and ready. On
success it:

1. Marks the quest completed.
2. Removes it from active quests.
3. Adds it to completed quests.
4. Awards its gold exactly once.
5. Returns the awarded amount and new balance.

Repeated completion attempts return `AlreadyCompleted` and cannot duplicate
the reward.

### Shopping

`GetCurrentShop()` returns the current map's optional shop.

`BuyItem(ShopSlot)` asks that shop to validate the selected slot and stock.
Only after a successful stock removal does `GameManager` deduct the price.

The old `EnterShop` method was removed because entering a shop and purchasing
an item are different actions. In WinForms, opening the shop panel is a UI
operation; `BuyItem` is the domain transaction.

### Gold

Quest completion and purchases update `PlayerGold` internally.

`AwardGold(int)` is also available for other World-level rewards or demo
setup. It accepts zero or a positive amount and returns the new balance.

Gold uses `int`. Exceeding `int.MaxValue` throws `OverflowException` instead
of silently wrapping into a negative balance. This is treated as invalid game
configuration rather than an ordinary gameplay status.

### Why the old `Attack` method was removed

The old method always dealt 15 damage. It ignored attack stats, skills, turn
order, targeting rules, and the Combat subsystem.

World should not invent a second combat system. The current shared
`ICombatant` contract exposes health, defense, damage, and healing, but it
does not expose a complete battle operation. A proper `StartBattle` facade
method should be added only after the Combat team's public API and contracts
are consistent.

Removing the hard-coded method is safer than publishing incorrect combat
behavior in the DLL.

## Expected WinForms usage

The form calls the API and converts statuses into presentation text.

### Dialogue

```csharp
NpcInteractionResult interaction = gameManager.TalkToNpc(selectedNpc);

if (interaction.Succeeded)
{
    dialogueLabel.Text =
        $"{interaction.Npc.Name}: {interaction.Dialogue}";

    acceptQuestButton.Enabled = interaction.CanAcceptQuest;
}
```

### Quest acceptance

```csharp
QuestActionResult result = gameManager.AcceptQuest(selectedNpc);

questStatusLabel.Text = result.Status switch
{
    QuestActionStatus.Success => "Quest accepted.",
    QuestActionStatus.NoQuestOffered => "This NPC has no quest.",
    QuestActionStatus.AlreadyAccepted => "That quest is already active.",
    QuestActionStatus.AlreadyCompleted => "That quest is already complete.",
    _ => "The quest cannot be accepted here."
};
```

### Quest progress and completion

```csharp
gameManager.AddQuestProgress(activeQuest, 1);
QuestActionResult completion = gameManager.CompleteQuest(activeQuest);

if (completion.Succeeded)
{
    goldLabel.Text = completion.GoldRemaining.ToString();
    RefreshQuestLists();
}
```

### Shopping

```csharp
PurchaseResult result = gameManager.BuyItem(selectedSlot);

shopStatusLabel.Text = result.Status switch
{
    PurchaseStatus.Success => $"Bought {result.Item.Name}.",
    PurchaseStatus.OutOfStock => "That item is out of stock.",
    PurchaseStatus.InsufficientGold => "Not enough gold.",
    PurchaseStatus.ItemNotSold => "That item is not sold here.",
    PurchaseStatus.ShopUnavailable => "There is no shop on this map.",
    _ => "Purchase failed."
};

goldLabel.Text = result.GoldRemaining.ToString();
```

The form can refresh its controls immediately after each result. No console
capture, output parsing, or UI dependency is required.

## What is intentionally not solved inside World

### Final item inventory transfer

`BuyItem` returns the purchased `IShopItem`, but the current Contracts folder
does not contain an inventory abstraction. World therefore cannot insert the
item into the Items team's concrete `Inventory` without violating the
dependency rule.

The team should eventually agree on an inventory contract if `GameManager`
must perform the final transfer. Until then, returning the purchased item
keeps the World transaction contract-safe.

### Battle startup

The Combat code currently expects combat members such as speed and basic
attacks that are not present in the checked-in `ICombatant` contract.
`GameManager` cannot expose a clean battle-start method until that mismatch is
resolved by the relevant team members.

### Persistence

These classes currently manage in-memory state. Saving and loading can be
added later using separate persistence models or services. I did not mix file
I/O into the domain classes because a WinForms game may choose JSON, a
database, or another format.

## Cross-team problems and recommended order

These problems are outside the World namespace but affect the full solution.
Not all of them are compiler errors, so I would handle them in this order:

1. Agree on the canonical `ICombatant`, item, equipment, and inventory
   boundaries.
2. Fix the current compile blockers and namespace mismatches.
3. Remove presentation output from domain classes and restore one class per
   file.
4. Convert and test the WinForms demo.

I would divide the work between the team members as follows.

### Combat owner

1. Choose one combat interface name. The contract that exists is
   `ICombatant`, but `Hero`, `Enemy`, `Skill`, `Boss`, and ultimate classes
   refer to an undefined `ICombat`. Those signatures and implementations
   currently cannot compile together.
2. Reconcile the battle engine with `ICombatant`. `BattleSystem`,
   `ConsoleBattleUI`, and the enemy strategies call `Speed` and
   `BasicAttack`, but those members are not declared by the current
   interface. The contract must not be edited without explicit team
   agreement. The alternative is to redesign the Combat code so it only uses
   the existing members.
3. Add or redesign skill targeting. `BattleSystem` and `ConsoleBattleUI`
   read `Skill.Target`, but `Skill` does not currently declare that property.
4. Decide how an uncharged ultimate fails. `BattleSystem` catches
   `UltimateNotChargedException`, but `UltimateSkill.Use` currently prints a
   message and returns instead of throwing that exception. Both sides must
   use the same behavior.
5. Remove domain-level console output from heroes, enemies, bosses, skills,
   and ultimates. Combat already has `IBattleUI`; battle messages should flow
   through that abstraction or through structured battle events/results.
   `ConsoleBattleUI` may remain as an optional console adapter, but it should
   not be the only or unavoidable presentation.
6. Supply a WinForms implementation of `IBattleUI`, and ensure a battle does
   not block the WinForms UI thread while waiting inside `Console.ReadLine`.
7. Split files that contain several public classes. Examples include
   `Enemy.cs`, `Skill.cs`, `UltimateSkill.cs`, `BattleActions.cs`,
   `BattleSystem.cs`, and `EnemyStrategies.cs`. The repository requires one
   class per file.
8. Rename `Combat/Character.cs` or its class so the file and public type agree.
   The file currently contains `Hero`, which makes discovery confusing even
   after the interface errors are fixed.

### Items owner

1. Change every item namespace from `RPGGameLibrary.Items` to the required
   `RpgLibrary.Items`.
2. Make shop-compatible items implement `IShopItem` exactly. The interface
   requires `Name` and `Price`; the current base item exposes `Value` instead.
   World shopping cannot be exercised with the current item classes.
3. Decide which classes implement the planned `IEquippable` contract. The
   repository instructions mention it, but no `IEquippable.cs` currently
   exists. Adding or changing a shared contract requires explicit team
   agreement.
4. Remove `Console.WriteLine` from `Item`, `Weapon`, `Armor`, `Potion`,
   `Inventory`, `Equipment`, and `ItemManager`. Methods should return a
   result, change state, or expose data for WinForms to render.
5. Encapsulate `Inventory.Items` and `Capacity`. Public setters currently let
   a consumer replace the list or shrink capacity below the current item
   count. Expose a read-only item list and mutate it through validated
   methods.
6. Make equipment properties nullable or initialize them. A new character
   can legitimately have no weapon or armor, but the current non-nullable
   properties are not initialized.
7. Align item stat names with the code that consumes them. The root character
   expects `AttackBonus` and `DefenseBonus`, while the current item classes
   expose `Damage` and `Defense`.
8. Connect `InventoryFullException` and `NotEnoughGoldException` only if the
   chosen API truly uses exceptions. Expected WinForms outcomes are usually
   easier to handle as result statuses; unused exception classes should not
   exist only for decoration.

### Character and shared-contract owner

1. Remove or relocate the stray `RpgLibrary/Character.cs`. Its `namespace
   rpg` and root-folder location violate the repository rules, and it
   duplicates the character concept in Combat.
2. Select one canonical player character implementation. At the moment the
   root `Character` and Combat `Hero` have incompatible APIs.
3. If the root character logic is retained, fix its missing item type
   references, the invalid parameterless `new Inventory()`, and the missing
   `AttackBonus`/`DefenseBonus` item members.
4. Agree as a team on what `ICombatant` must contain before anyone edits it.
   The final interface must be implemented exactly by every playable
   character, enemy, and boss.
5. Agree on whether World needs an inventory abstraction for completed shop
   purchases. Until one exists, `GameManager` deliberately returns the
   purchased `IShopItem` instead of depending on the concrete Items
   inventory.

### WinForms demo owner

1. Replace the current object-initializer demo. It tries to create a
   nonexistent `RpgLibrary.Combat.Character`, constructs `Weapon` with an API
   that does not exist, and directly instantiates abstract `Quest`.
2. Construct `MainQuest` or `SideQuest` through their constructors and
   demonstrate the quest lifecycle through `GameManager`.
3. Update the repository architecture instructions to reflect the planned
   WinForms demo; they currently describe `RpgDemo` as a console application.
   Then convert the project using a Windows target framework, enable
   `UseWindowsForms`, set the correct output type, and start a form with
   `Application.Run`.
4. Keep all display text, control updates, and message boxes in the demo.
   Consume the World result records instead of reintroducing console output
   into the DLL.
5. Update the README after the conversion. It currently describes
   `RpgDemo` as a console application.

World should not work around these problems by depending on concrete,
incorrect types. The correct solution is for each subsystem owner to align
their implementation with the shared contracts.

## Practical validation checklist

Before connecting WinForms, I would test the World layer with these cases:

1. Constructing any World object with invalid required data throws a clear
   argument exception.
2. A map cannot contain the same NPC or enemy instance twice.
3. Talking to an NPC on another map returns `NpcNotInCurrentMap`.
4. An NPC without a quest returns `NoQuestOffered` when acceptance is tried.
5. A quest cannot gain progress before acceptance.
6. Progress never exceeds `RequiredProgress`.
7. A quest cannot complete before its requirement is met.
8. Completing a quest awards gold once and moves it between quest lists.
9. A shop rejects a slot belonging to another shop.
10. A sold-out slot cannot go below zero.
11. An unsuccessful purchase does not change gold or stock.
12. A successful purchase decrements stock and gold exactly once.
13. A map without a shop returns `ShopUnavailable`.
14. No class in `RpgLibrary.World` calls `Console.WriteLine`,
    `Console.Write`, or `Console.ReadLine`.

## Runtime and language assumptions

The repository currently targets .NET 10 with nullable reference types and
implicit usings enabled. The World code uses records, file-scoped namespaces,
`ArgumentNullException.ThrowIfNull`, and implicit imports such as
`System.Collections.Generic`.

World objects are designed for one `GameManager` per play session. A quest,
map, NPC, shop, or slot instance should not be shared between several active
game sessions because those objects contain mutable session state.

The layer also assumes the normal WinForms single-UI-thread model. Its
collections and purchase operations are not synchronized for concurrent
writes. If game state is later changed from worker threads or a server, access
must be serialized or protected before the same instances are shared.

## Hard-coded gameplay audit

The executable World code contains no fixed map names, dialogue, enemy damage,
quest rewards, shop contents, item prices, or nonzero testing balance. Those
values all come from constructors or method arguments.

A few numeric literals remain as domain invariants and safe defaults:

- Zero is the default starting-gold balance.
- One is the default quest progress requirement.
- Chapter, map difficulty, and positive quantities must start at one.
- Stock, rewards, prices, and gold cannot be negative.

These are validation rules rather than hard-coded game content. If the team
wants different rules, they should be changed deliberately or moved into a
future configuration object.

## Summary of the design

The World layer is configured around one rule: domain objects own and protect
their state, while `GameManager` coordinates the operations a game UI wants
to perform.

Quests own lifecycle and progress. NPCs offer quests. Maps own NPCs, enemies,
and an optional shop. Shops own stock. `GameManager` owns current-map,
quest-log, and wallet state. Result objects carry outcomes back to WinForms.

This structure gives the project a usable facade, demonstrates encapsulation,
inheritance, abstraction, and composition, and keeps the final DLL independent
from any specific presentation technology.
