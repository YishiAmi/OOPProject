# RPG Class Library

A small .NET class library containing reusable RPG domain models, accompanied by a minimal console application that demonstrates how to consume the library. The repository is a shared object-oriented programming project, not a complete game.

## Team Members

- Ammar Mustaqim Bin Mohamad Shamsul Akhmar — 22005646
- Mohamedelhassan Mohamed Elhag Momen — 25014926
- Mohd Harith Bin Abd Manan — 26000341

## Solution Structure

```text
OOPProject.sln
├── RpgLibrary/
│   ├── Contracts/   Shared interfaces
│   ├── Combat/      Characters, enemies, and skills
│   ├── Items/       Equipment, consumables, and inventory
│   └── World/       NPCs, shops, maps, quests, and game state
└── RpgDemo/         Minimal console consumer
```

`RpgLibrary` builds `RpgLibrary.dll`. `RpgDemo` references that project and shows basic object creation without adding gameplay logic.

## Build and Run

The solution targets .NET 10.

```bash
dotnet restore OOPProject.sln
dotnet build OOPProject.sln
dotnet run --project RpgDemo/RpgDemo.csproj
```

## Module Ownership

- **Combat:** Person A
- **Items:** Person B
- **World:** Person C
- **Contracts:** shared; coordinate changes because multiple modules depend on them

Keep public types in individual files and use their module namespace, such as `RpgLibrary.Combat` or `RpgLibrary.Items`.
