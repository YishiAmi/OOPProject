<p align="center">
  <a href="https://yishiami.github.io/OOPProject/">
    <img src="docs/assets/coconut.jpg" width="112" height="112" alt="RpgLibrary coconut project icon">
  </a>
</p>

# RpgLibrary

A reusable .NET 10 class library for building turn-based RPG systems. The
project models combat, characters, items, inventories, quests, shops, maps,
and game state without coupling the core library to a particular user
interface.

[**Read the documentation**](https://yishiami.github.io/OOPProject/) ·
[Browse the source](RpgLibrary/) ·
[View the demo](RpgDemo/)

> This is a shared object-oriented programming project and API demonstration,
> not a complete game.

## Features

- Turn-based combat with skills, ultimate abilities, targeting, and enemy AI.
- Reusable equipment, consumable, inventory, and shop models.
- Maps, NPC interactions, quests, rewards, and player progression.
- Shared contracts for communication between library subsystems.
- A facade-based world API coordinated through `GameManager`.
- A Windows Forms example that consumes the compiled library DLL.
- Interactive API documentation and standard UML class diagrams.

## Documentation

The complete API reference is published with GitHub Pages:

### [yishiami.github.io/OOPProject](https://yishiami.github.io/OOPProject/)

It includes architecture guidance, public type references, integration
examples, searchable documentation, and detailed UML diagrams for the
Contracts, Combat, Items, and World namespaces.

## Repository structure

```text
OOPProject.sln
├── RpgLibrary/
│   ├── Contracts/   Shared subsystem interfaces
│   ├── Combat/      Combatants, battles, skills, and enemy behavior
│   ├── Items/       Equipment, consumables, and inventory
│   ├── World/       Maps, NPCs, quests, shops, and GameManager
│   └── Exceptions/  Domain-specific failure types
├── RpgDemo/         Windows Forms DLL consumer
└── docs/            GitHub Pages site and PlantUML sources
```

`RpgLibrary` produces `RpgLibrary.dll`. Before resolving its references,
`RpgDemo` builds the library and loads that compiled DLL from the matching
Debug or Release output directory. The project is currently distributed as
source and a buildable DLL rather than as a NuGet package.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Windows to run the `RpgDemo` Windows Forms application

## Build and run

Restore and build the solution:

```bash
dotnet restore OOPProject.sln
dotnet build OOPProject.sln
```

On Windows, run the demonstration application:

```bash
dotnet run --project RpgDemo/RpgDemo.csproj
```

## Architecture

The library is divided into focused subsystems. Top-level game operations are
coordinated through `RpgLibrary.World.GameManager`, and public extension points
are provided through interfaces in `RpgLibrary.Contracts`.

When contributing:

- Keep one public type per file.
- Place types in their appropriate module folder and namespace.
- Coordinate changes to shared contracts with the team.
- Keep Windows Forms and other framework-specific UI code outside the core
  library.

## Team

- Ammar Mustaqim Bin Mohamad Shamsul Akhmar — 22005646
- Mohamedelhassan Mohamed Elhag Momen — 25014926
- Mohd Harith Bin Abd Manan — 26000341
