## ADDED Requirements

### Requirement: Database-backed credential verification
The system SHALL authenticate from SCUser using a normalized unique UserName and adaptive salted hash. It MUST NOT store, log, compare, or return plaintext passwords.

#### Scenario: Valid active user
- **WHEN** an active user submits the correct password
- **THEN** the system verifies PasswordHash and evaluates MustChangePassword

##### Example: Normal administrator login
- **GIVEN** admin is active, has MustChangePassword=false, and resolves auth.login through its role permissions
- **WHEN** admin submits the current password
- **THEN** the system issues a general JWT containing permission=auth.login

#### Scenario: Invalid login variants
- **WHEN** a user is missing, inactive, or supplies the wrong password
- **THEN** every variant returns the same HTTP 401 response

### Requirement: Database-backed permission claims and login eligibility
The system SHALL union all active permissions associated through the user's roles into a general JWT with user ID, normalized UserName, and deduplicated permission claims. A user without auth.login MUST NOT receive a general JWT. Role claims can be included for display but MUST NOT determine login eligibility or endpoint authorization.

#### Scenario: Permissions from multiple roles
- **WHEN** a user receives auth.login and finance.read through different roles and completes normal login
- **THEN** the JWT contains both permission claims and the database user ID

##### Example: Deduplicated permission claims
- **GIVEN** SCUserID=7 resolves auth.login through two roles and finance.read through one role
- **WHEN** user 7 completes login
- **THEN** the JWT has subject 7 and exactly one claim for each of auth.login and finance.read

#### Scenario: Missing login permission
- **WHEN** a user without auth.login supplies valid credentials
- **THEN** the system returns HTTP 403 without issuing a general JWT

#### Scenario: Permission change takes effect on next login
- **WHEN** a role permission mapping changes after a general JWT was issued
- **THEN** newly issued tokens reflect the new mapping while the existing token remains unchanged until expiration

### Requirement: Idempotent default administrator provisioning
After migration, installation SHALL ensure one active admin associated with Admin and all baseline permissions when admin does not exist. Baseline permissions SHALL include auth.login, system.settings.manage, accounts.manage, permissions.manage, files.delete.permanent, and finance.read. The initial credential SHALL be admin/password, PasswordHash SHALL contain only a salted hash, and MustChangePassword SHALL be true.

#### Scenario: Fresh installation
- **WHEN** the application starts against a database without admin
- **THEN** it creates Admin, admin, their association, baseline permissions, Admin permission mappings, a hash of password, and MustChangePassword=true

##### Example: Empty account tables
- **GIVEN** SCUser, SCRole, and UserRole contain no rows
- **WHEN** the initializer runs once
- **THEN** one admin, one Admin role, one user-role association, six baseline permissions, six Admin permission mappings exist, and PasswordHash is not password

#### Scenario: Repeated startup
- **WHEN** admin already exists
- **THEN** startup does not alter its hash, MustChangePassword, IsActive, user-role associations, or an already initialized role permission matrix

##### Example: Customized existing admin
- **GIVEN** admin has a changed hash, MustChangePassword=false, IsActive=true, and two role associations
- **WHEN** the initializer runs again
- **THEN** all persisted account values and existing role permission mappings remain unchanged

#### Scenario: Concurrent provisioning
- **WHEN** two instances provision concurrently
- **THEN** the unique UserName constraint prevents duplicate admin users

##### Example: Two startup transactions
- **GIVEN** no admin exists and two initializers begin concurrently
- **WHEN** both attempt to commit
- **THEN** one admin row remains and both initializers resolve that same row

### Requirement: Mandatory first-login password change
A user with MustChangePassword=true SHALL receive only a short-lived password-change token and RequiresPasswordChange=true. The token MUST NOT contain business roles or access business APIs.

#### Scenario: Initial admin login
- **WHEN** admin signs in with password while MustChangePassword is true
- **THEN** the response has RequiresPasswordChange=true, empty roles, and a restricted token

##### Example: Default credential response
- **GIVEN** freshly provisioned admin/password
- **WHEN** admin logs in for the first time
- **THEN** the JWT has token_use=password_change, no Admin role, and a lifetime no longer than 15 minutes

#### Scenario: Login after change
- **WHEN** admin signs in with the new password after MustChangePassword=false
- **THEN** the response has RequiresPasswordChange=false and an Admin JWT

##### Example: New credential login
- **GIVEN** admin changed the password to Petsalon-2026!
- **WHEN** admin logs in with Petsalon-2026!
- **THEN** the response contains permission=auth.login, the Admin baseline permission claims, and no token_use=password_change claim

### Requirement: Password change validation and transaction
Password change SHALL require the current password, matching confirmation, and a new password of at least 12 characters that differs from password and the case-insensitive UserName. Hash update and MustChangePassword clearing SHALL be one transaction.

#### Scenario: Successful change
- **WHEN** a restricted-token holder supplies valid current and new passwords
- **THEN** the system commits both state changes and requires a new login

##### Example: Valid replacement
- **GIVEN** admin has a valid password-change token and CurrentPassword=password
- **WHEN** NewPassword and ConfirmPassword are both Petsalon-2026!
- **THEN** PasswordHash verifies Petsalon-2026!, MustChangePassword=false, and the client must log in again

#### Scenario: Weak password
- **WHEN** the new password is shorter than 12 characters or equals password or UserName
- **THEN** the system returns HTTP 400 without changing state

#### Scenario: Transaction failure
- **WHEN** persistence fails during password change
- **THEN** both state changes roll back

##### Example: Failed database commit
- **GIVEN** the original hash verifies password and MustChangePassword=true
- **WHEN** SaveChanges fails during the password change transaction
- **THEN** the original hash still verifies password and MustChangePassword remains true

### Requirement: Login audit state
The system SHALL update LastLogin only after successful credential verification. Failed authentication MUST NOT change LastLogin.

#### Scenario: Successful login
- **WHEN** valid credentials are verified
- **THEN** LastLogin is updated

##### Example: Successful timestamp update
- **GIVEN** LastLogin is 2026-08-01T08:00:00Z
- **WHEN** authentication succeeds at 2026-08-07T12:00:00Z
- **THEN** LastLogin becomes 2026-08-07T12:00:00Z

#### Scenario: Failed login
- **WHEN** invalid credentials are submitted
- **THEN** LastLogin remains unchanged

##### Example: Failed timestamp update
- **GIVEN** LastLogin is 2026-08-01T08:00:00Z
- **WHEN** authentication fails at 2026-08-07T12:00:00Z
- **THEN** LastLogin remains 2026-08-01T08:00:00Z
