# AI Assistant Context & Repository Rules

**ATTENTION AI CODING ASSISTANTS:** Read these rules before generating, modifying, or refactoring any code in this repository. 

## 1. Project Architecture & Dependency Strict Rules
*   **Structure:** This project consists of a C# Class Library (`RpgLibrary`) and a Windows Forms Demo (`RpgDemo`)[cite: 1].
*   **The Facade Pattern:** The project relies heavily on the Facade pattern[cite: 1]. `RpgLibrary.World.GameManager` is the Facade[cite: 1]. Do NOT create direct dependencies between the `Combat` and `Items` folders. All top-level coordination happens exclusively through `GameManager`[cite: 1].
*   **Interface Contracts:** Subsystems MUST communicate ONLY through the shared interfaces located in `RpgLibrary/Contracts/` (`ICombatant`, `IShopItem`, `IEquippable`)[cite: 1].
*   **IMMUTABILITY:** Never modify the interfaces in `RpgLibrary/Contracts/` unless explicitly instructed by the user.

## 2. Namespace & File Structure Conventions
*   **File Isolation:** Strictly enforce one class per file[cite: 1]. Do not combine multiple classes into a single `.cs` file.
*   **Namespaces:** You must strictly adhere to the following exact namespaces based on the folder location[cite: 1]. Do NOT hallucinate namespaces like `namespace rpg` or `RPGGameLibrary`[cite: 1]:
    *   `RpgLibrary.Contracts` (Shared interfaces)
    *   `RpgLibrary.Combat` (Enemies, Bosses, Skills)
    *   `RpgLibrary.Items` (Weapons, Armor, Inventory)
    *   `RpgLibrary.World` (Map, NPC, Quest, GameManager)

## 3. Anti-Hallucination ("Vibe Coding" Prevention)
*   **Implementation Checks:** When generating a new class, immediately check its required interface. 
    *   If generating a weapon/armor/potion for the `Items` layer, it MUST implement `IShopItem`[cite: 1]. 
    *   If generating an enemy/boss/character for the `Combat` layer, it MUST implement `ICombatant`[cite: 1].
*   **Properties:** Ensure interface implementation is exact. For example, `IShopItem` requires a `Price` property, do not arbitrarily substitute this with `Value`[cite: 1].
*   **Dead Code:** Do not create or reference classes in the root `RpgLibrary/` directory (e.g., a stray `Character.cs`). All classes must live in their respective subfolders[cite: 1, 2].