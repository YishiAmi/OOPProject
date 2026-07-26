# Repository Guidelines

## Project Structure & Module Organization

`OOPProject.sln` contains two .NET 10 projects:

- `RpgLibrary/` builds the reusable domain-model library. Shared interfaces live in `Contracts/`; classes are grouped by `Combat/`, `Items/`, and `World/`.
- `RpgDemo/` is a small console consumer that references the library. Keep `Program.cs` demonstrative; application or gameplay logic does not belong here.
- `README.md` documents the solution and module ownership.

Generated `bin/` and `obj/` directories are ignored and must not be committed. Add future automated tests in a separate `RpgLibrary.Tests/` project.

## Build, Test, and Development Commands

Run commands from the repository root:

```bash
dotnet restore OOPProject.sln
dotnet build OOPProject.sln
dotnet run --project RpgDemo/RpgDemo.csproj
dotnet test OOPProject.sln
dotnet format OOPProject.sln --verify-no-changes
```

`restore` resolves dependencies, `build` compiles both projects, and `run` executes the minimal demo. `test` will discover test projects once added. The final command checks standard .NET formatting without changing files.

## Coding Style & Naming Conventions

Use four-space indentation and conventional C# brace placement. Use `PascalCase` for types, methods, and public properties; use `camelCase` for parameters and local variables. Keep one public type per matching file, such as `Character.cs`. Use module namespaces (`RpgLibrary.Combat`, `RpgLibrary.Items`, or `RpgLibrary.World`) and place cross-module interfaces in `RpgLibrary.Contracts`. Preserve nullable correctness and avoid suppressing warnings without an explanation.

## Testing Guidelines

There is no automated suite yet. Add behavioral tests in `RpgLibrary.Tests`, naming files after the class under test. Use descriptive names such as `Heal_WhenAmountExceedsMaximum_CapsHealth`. Cover interface contracts and boundary behavior; do not test simple auto-properties solely for coverage.

## Commit & Pull Request Guidelines

Recent commits use short, imperative summaries such as `Update README.md` and `Revise team members section in README`. Keep subjects concise but more specific when possible, for example `Add health bounds tests`.

Pull requests should explain the change and motivation, list validation commands run, and link related issues. Include console output or screenshots when behavior visible to users changes. Keep each PR focused and avoid committing `bin/`, `obj/`, IDE settings, or unrelated cleanup.
