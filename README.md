
# SCRYBE

SCRYBE is a standalone, embeddable narrative authoring and execution system.

It is designed to let authors construct interactive dialogue and narrative logic visually, compile that content into portable executable data, and integrate the result into games and other host applications.

SCRYBE is not a game engine. It does not own rendering, audio, input, gameplay, or the host application's lifecycle.

## Project status

SCRYBE is undergoing a ground-up rebuild.

The previous implementation is retained separately as historical reference material. Its architecture, behavior, APIs, formats, and tests are not automatically authoritative for this implementation.

Features described below represent intended capabilities unless explicitly marked as implemented.

## Goals

SCRYBE aims to provide:

- A visual, node-based narrative authoring environment.
- A validated, versioned project representation.
- Deterministic compilation of authored content.
- A portable, versioned compiled format (`.scrybin`).
- A host-independent interpreter.
- A stable managed runtime SDK.
- A C-compatible native integration boundary.
- Explicit host events for dialogue, choices, and gameplay requests.
- Predictable execution, diagnostics, and failure behavior.
- Reliable project persistence and runtime state restoration.
- A practical development and testing workflow.

The editor and runtime are separate products built on shared contracts. Applications must be able to execute compiled content without installing or starting the editor.

## Architectural principles

### Clear ownership

Each component owns a defined responsibility.

Components must not acquire responsibilities simply because their existing dependencies make an implementation convenient.

### One authoritative implementation

Project validation, compilation, execution, and serialization must not develop competing implementations across the editor, tools, and runtime.

Editor preview and exported content must follow the same semantic execution contracts.

### Explicit boundaries

Dependencies flow from foundational libraries toward their consumers.

Foundational libraries must not depend on the desktop editor, application-specific hosts, or presentation frameworks.

### Determinism

Equivalent inputs and compilation settings must produce equivalent compiled outputs.

Stable identifiers, canonical serialization, predictable ordering, and controlled versioning are deliberate design concerns.

### Versioned compatibility

Project documents, compiled binaries, runtime state, and public integration interfaces have distinct compatibility requirements.

Format changes require explicit decisions, compatibility tests, and documented behavior.

Unsupported versions and malformed inputs must fail predictably rather than silently changing meaning.

### Separation from the host

SCRYBE describes and executes narrative logic.

The consuming application decides how to present dialogue, receive input, perform gameplay actions, and manage its own world.

Host integration must not require a particular renderer, game engine, or UI framework.

### Correctness before optimization

Public contracts and observable behavior take priority over premature optimization.

Performance-sensitive systems should be measured with representative benchmarks and improved without weakening correctness.

### Testable by design

Core behavior must be testable without launching the desktop editor.

Tests should include invalid inputs, boundary conditions, deterministic output, lifecycle behavior, and complete compilation-to-execution workflows.

Passing tests are evidence, not proof that the application is release-ready.

## Rebuild approach

Development proceeds through small, independently reviewable commits.

Each change should have:

- A clearly defined purpose.
- An intentionally limited scope.
- Explicit ownership and dependency decisions.
- Appropriate verification.
- No unrelated refactoring or speculative features.

The initial implementation will prioritize the project model, persistence, compiler, binary format, interpreter, and managed runtime.

The graphical editor, native integration, packaging, and release workflows will follow as their underlying contracts mature.

## Licensing

SCRYBE is intended to be distributed under the GNU Lesser General Public License version 3.0 (LGPL-3.0-only).

The complete applicable license texts will be added before implementation code is introduced.

Third-party dependencies and assets retain their respective licenses.
