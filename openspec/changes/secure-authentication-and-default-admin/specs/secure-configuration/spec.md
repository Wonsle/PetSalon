## ADDED Requirements

### Requirement: Secrets outside version control
The application SHALL obtain the database connection string and JWT signing key from environment-specific secret providers. Tracked files MUST NOT contain a usable database password, signing key, or production credential.

#### Scenario: Production environment injection
- **WHEN** ConnectionStrings__DefaultConnection and JwtSettings__SignKey are supplied by the environment
- **THEN** the application uses them without persisting them to a tracked file

##### Example: Container secret names
- **GIVEN** a container with both required environment variables set to non-placeholder values
- **WHEN** ASP.NET Core builds its configuration
- **THEN** the connection string and signing key resolve from environment variables

#### Scenario: Repository inspection
- **WHEN** tracked settings and examples are inspected
- **THEN** they contain only non-sensitive placeholders and instructions

##### Example: Credential pattern scan
- **GIVEN** the tracked project files
- **WHEN** a credential scan checks appsettings and environment examples
- **THEN** no usable SQL password or JWT signing key is found

### Requirement: Startup secret validation
The application SHALL validate secrets before accepting requests and SHALL reject empty values, documented placeholders, and JWT signing keys shorter than 32 bytes.

#### Scenario: Missing signing key
- **WHEN** the application starts without JwtSettings__SignKey
- **THEN** startup fails and identifies only the missing key name

##### Example: Empty environment
- **GIVEN** JwtSettings__SignKey is absent and the connection string is valid
- **WHEN** the application starts
- **THEN** startup fails before binding a listening port and mentions JwtSettings__SignKey without printing a value

#### Scenario: Valid secrets
- **WHEN** required secrets are non-placeholder values and the signing key is at least 32 bytes
- **THEN** startup succeeds without logging their values

### Requirement: Development and deployment setup documentation
The project SHALL document .NET User Secrets for local development and environment or platform secrets for containers and production, including both required configuration keys.

#### Scenario: Local setup
- **WHEN** a developer follows the local instructions
- **THEN** the backend receives required secrets without editing tracked settings

##### Example: User Secrets configuration
- **GIVEN** a clean clone with no local appsettings changes
- **WHEN** the developer sets both documented keys with dotnet user-secrets
- **THEN** the backend passes startup secret validation

#### Scenario: Container setup
- **WHEN** an operator follows deployment instructions
- **THEN** the backend receives required secrets through environment injection

##### Example: Compose deployment
- **GIVEN** the operator supplies rotated values through the deployment environment
- **WHEN** the backend container starts
- **THEN** both required keys resolve without being baked into the image

### Requirement: Credential rotation boundary
The deployment procedure SHALL require rotation of every database password and JWT signing key previously committed and MUST NOT expose the replacements.

#### Scenario: Existing installation upgrade
- **WHEN** an existing installation adopts this change
- **THEN** the operator rotates both credentials before enabling the deployment

##### Example: Rotation checklist
- **GIVEN** the old SQL password and signing key existed in Git history
- **WHEN** the secured version is deployed
- **THEN** neither old credential can authenticate or validate newly issued tokens
