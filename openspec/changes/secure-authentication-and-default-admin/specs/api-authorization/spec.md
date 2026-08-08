## ADDED Requirements

### Requirement: Authenticated API by default
The system SHALL require a valid general-purpose JWT for every controller API unless the endpoint is explicitly anonymous or password-change-only.

#### Scenario: Anonymous business request
- **WHEN** a caller without a JWT requests a business API
- **THEN** the system returns HTTP 401 without executing the action

### Requirement: Explicit anonymous endpoint allowlist
The system SHALL allow anonymous access only to the login endpoint and SHALL require an explicit anonymous designation for every future exception.

#### Scenario: Anonymous login
- **WHEN** a caller posts credentials to POST /api/Account/login without a JWT
- **THEN** the system processes the login request

##### Example: Default administrator login request
- **GIVEN** a fresh installation with admin awaiting password change
- **WHEN** the caller posts UserName=admin and Password=password without an Authorization header
- **THEN** the response is HTTP 200 with RequiresPasswordChange=true

#### Scenario: Unlisted anonymous request
- **WHEN** a caller requests any other controller API without a JWT
- **THEN** the system returns HTTP 401

### Requirement: Database-backed permission authorization
The system SHALL derive endpoint authorization from permission claims resolved through UserRole, SCRole, RolePermission, and SCPermission. Controllers SHALL declare stable permission codes and MUST NOT declare Admin, Manager, Designer, or another role name as an authorization condition.

#### Scenario: Caller lacks endpoint permission
- **WHEN** an authenticated caller requests an endpoint without its declared permission
- **THEN** the system returns HTTP 403 without executing the action

#### Scenario: Caller has endpoint permission
- **WHEN** an authenticated caller requests an endpoint with its declared permission
- **THEN** the system permits the request independent of the caller's role name

##### Example: Financial report permission
- **GIVEN** a valid general JWT containing permission=finance.read
- **WHEN** the caller requests GET /api/Financial/monthly-revenue
- **THEN** authorization succeeds and the controller action executes

#### Scenario: Role mapping changes without controller deployment
- **WHEN** an operator grants finance.read to a different role and the affected user signs in again
- **THEN** the user can access financial endpoints without changing controller code

### Requirement: Transactional role permission management
The system SHALL allow a caller with permissions.manage to list permissions and replace a role's complete permission set in one transaction. Unknown, inactive, or duplicate permission codes MUST return HTTP 400 without changing the existing mapping.

#### Scenario: Replace role permissions
- **WHEN** an authorized caller replaces a role's permissions with auth.login and finance.read
- **THEN** the database contains exactly those two mappings for that role

#### Scenario: Invalid permission replacement
- **WHEN** a replacement request contains an unknown, inactive, or duplicate PermissionCode
- **THEN** the system returns HTTP 400 and preserves the original role permissions

### Requirement: Password-change token isolation
The system SHALL accept token_use=password_change only at the password change endpoint and SHALL reject it at business APIs.

#### Scenario: Restricted token calls business API
- **WHEN** a password-change token calls a business API
- **THEN** the system returns HTTP 403

#### Scenario: Restricted token changes password
- **WHEN** a valid password-change token calls POST /api/Password/change
- **THEN** the system evaluates the request

##### Example: Initial admin changes password
- **GIVEN** a valid JWT containing token_use=password_change and no business roles
- **WHEN** admin submits CurrentPassword=password, NewPassword=Petsalon-2026!, and matching ConfirmPassword
- **THEN** the system evaluates and commits the password change

### Requirement: Authentication and authorization error semantics
The system SHALL return HTTP 401 for missing or invalid authentication and HTTP 403 for authenticated callers lacking the required permission or token purpose. Responses MUST NOT expose Token contents, secrets, passwords, hashes, or internal exceptions.

#### Scenario: Expired token
- **WHEN** a caller presents an expired JWT
- **THEN** the system returns HTTP 401 with a sanitized response

#### Scenario: Insufficient permission
- **WHEN** an authenticated caller lacks the required permission
- **THEN** the system returns HTTP 403 with a sanitized response
