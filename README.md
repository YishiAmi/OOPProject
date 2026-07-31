# RPG Class Library

A small .NET class library containing reusable RPG domain models, accompanied by a minimal Windows Forms application that demonstrates how to consume the library. The repository is a shared object-oriented programming project, not a complete game.

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
└── RpgDemo/         Windows Forms demo consumer
```

`RpgLibrary` builds `RpgLibrary.dll`. Before resolving its references,
`RpgDemo` builds the library and then loads that compiled DLL from the
matching configuration folder (`bin/Debug` or `bin/Release`). The demo
therefore exercises the same distributable library artifact that another
application would consume.

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
