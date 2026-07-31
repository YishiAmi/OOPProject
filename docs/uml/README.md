# PlantUML diagram sources

The SVG files in this folder are generated from the matching `.puml` files.
They document the current code relationships, including dependencies that the
team may later refactor.

There are two deliberate levels:

- `*-overview.puml` and `*-overview.svg` are concise presentation diagrams
  for quick architectural orientation.
- The files without `-overview` are the canonical standard UML diagrams.
  The documentation displays these for Contracts, World, Combat, and Items.
  They include typed members, visibility, stereotypes, multiplicity,
  inheritance, interface realization, composition, and aggregation.

The shared colors and layout rules live in `theme.puml`. PlantUML resolves that
file through the local `!include theme.puml` directive.

To regenerate the diagrams, download an official PlantUML JAR and run this
command from the repository root:

```bash
java -Djava.awt.headless=true -jar /path/to/plantuml.jar \
  -tsvg \
  -charset UTF-8 \
  -failfast2 \
  docs/uml/architecture-overview.puml \
  docs/uml/contracts-overview.puml \
  docs/uml/world-overview.puml \
  docs/uml/combat-overview.puml \
  docs/uml/items-overview.puml \
  docs/uml/architecture.puml \
  docs/uml/contracts.puml \
  docs/uml/world.puml \
  docs/uml/combat.puml \
  docs/uml/items.puml
```

Do not edit the generated SVG files manually. Update the `.puml` source and
regenerate its output instead.
