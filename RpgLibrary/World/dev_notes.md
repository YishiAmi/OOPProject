### 📝 Project Checkpoint: OOP Project (World Layer)

**Current Status:** 500 Internal Server Error — brain didn't handle this exception

---

### 1. Architectural Rules 
To prevent messy merge conflicts and AI hallucination, the following hard rules are in effect across the repository:
*   **One Class Per File:** No exceptions. `NPC.cs` only contains `public class NPC`.
*   **Namespace Strictness:** Files must use the exact namespace of their folder (e.g., `RpgLibrary.Combat`, `RpgLibrary.Items`, `RpgLibrary.World`).
*   **Contract-Driven Design:** Subsystems do not talk directly to each other's concrete classes. They communicate exclusively through interfaces (`ICombatant`, `IShopItem`) located in `RpgLibrary.Contracts`.


### 2. Classes 
This is the current state of your specific domain. 

| Class | Type | Status |
| :--- | :--- | :--- |
| **`Quest`** | Abstract Class | Done | 
| **`MainQuest`** | Concrete Class | Done | 
| **`SideQuest`** | Concrete Class | Done | 
| **`NPC`** | Concrete Class | Done |
| **`Map`** | Concrete Class | Done | 
| **`ShopSlot`** | Data Container | Done | 
| **`Shop`** | Concrete Class | Planned | 
| **`GameManager`** | Facade / Entry | Planned | 

### 3. To Do
-   [ ] you'll figure it out.
-   [ ] Finish Gamemanager
-   [ ] Finish Shop methods for populating inventory and buying
-   [ ] convert all basic outputs into api to be used with winform