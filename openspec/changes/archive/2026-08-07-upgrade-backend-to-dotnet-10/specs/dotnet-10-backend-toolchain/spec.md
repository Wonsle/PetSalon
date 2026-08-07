## ADDED Requirements

### Requirement: Unified .NET 10 target framework

Every first-party project in the PetSalon backend solution SHALL target net10.0, and project references SHALL restore without target-framework compatibility errors.

#### Scenario: Restore all backend projects

- **WHEN** an operator runs `dotnet restore PetSalon.sln` from the backend directory with the configured NuGet feeds available
- **THEN** every first-party backend and test project in the solution restores successfully for net10.0

### Requirement: Deterministic .NET 10 SDK selection

The backend directory SHALL contain an SDK selection policy that requires the .NET 10.0.3xx feature band and permits the latest installed patch in that feature band.

#### Scenario: Resolve the repository SDK

- **WHEN** an operator runs `dotnet --version` from the backend directory with a compatible 10.0.3xx SDK installed
- **THEN** the command reports a 10.0.3xx SDK version

### Requirement: Compatible Microsoft platform dependencies

Direct Microsoft.EntityFrameworkCore, Microsoft.AspNetCore, and Microsoft.Extensions package references SHALL use stable 10.0.x versions compatible with net10.0, and all direct Entity Framework Core package references SHALL use the same version.

#### Scenario: Build with upgraded platform packages

- **WHEN** an operator builds `PetSalon.sln` after package restore
- **THEN** the solution builds with zero errors and without package downgrade or target-framework compatibility warnings

### Requirement: Preserve backend contracts during toolchain migration

The migration MUST NOT introduce a database schema migration or intentionally change existing HTTP routes, DTO shapes, authentication behavior, or business rules.

#### Scenario: Upgrade produces no schema migration

- **WHEN** the .NET 10 migration changes are reviewed
- **THEN** no new Entity Framework Core migration or model snapshot change is present

### Requirement: Verify available automated tests

All test projects discoverable from the backend solution SHALL target a framework compatible with .NET 10 and MUST pass after the upgrade.

#### Scenario: Execute solution tests

- **WHEN** an operator runs `dotnet test PetSalon.sln --no-build` after a successful build
- **THEN** every discoverable automated test completes successfully, or the command explicitly reports that the solution contains no test projects
