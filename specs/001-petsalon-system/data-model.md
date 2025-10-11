# Data Model: PetSalon Management System

**Feature**: 001-petsalon-system
**Date**: 2025-10-11
**Status**: Phase 1 Design Complete

## Entity Relationship Diagram

```mermaid
erDiagram
    Pet ||--o{ PetRelation : "has contacts"
    ContactPerson ||--o{ PetRelation : "manages pets"
    Pet ||--o{ ReserveRecord : "books appointments"
    Pet ||--o{ Subscription : "subscribes to"
    Pet ||--o{ PetServicePrice : "custom pricing"

    Subscription ||--o{ ReserveRecord : "used by"
    SubscriptionType ||--o{ Subscription : "defines"

    Service ||--o{ PetServicePrice : "pricing per pet"
    Service ||--o{ ReservationService : "included in"
    ServiceAddon ||--o{ ReservationAddon : "added to"

    ReserveRecord ||--o{ ReservationService : "contains"
    ReserveRecord ||--o{ ReservationAddon : "includes"
    ReserveRecord ||--o{ PaymentRecord : "generates payment"

    SystemCode ||--|| CodeType : "categorized by"

    Pet {
        bigint PetID PK "Auto-increment"
        nvarchar_100 PetName "Required"
        varchar_20 Gender FK "SystemCode"
        varchar_50 Breed FK "SystemCode"
        date BirthDay "Optional"
        money NormalPrice "Default null"
        money SubscriptionPrice "Default null"
        nvarchar_500 PhotoPath "Optional"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    ContactPerson {
        bigint ContactPersonID PK "Auto-increment"
        nvarchar_100 Name "Required"
        nvarchar_50 NickName "Optional"
        varchar_20 ContactNumber "Required, indexed"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    PetRelation {
        bigint PetRelationID PK "Auto-increment"
        bigint PetID FK "Required"
        bigint ContactPersonID FK "Required"
        varchar_20 RelationshipType FK "SystemCode"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    ReserveRecord {
        bigint ReserveRecordID PK "Auto-increment"
        bigint PetID FK "Required"
        bigint SubscriptionID FK "Optional"
        datetime2 ReserverDate "Required, indexed"
        time ReserverTime "Required"
        varchar_20 Status FK "SystemCode, indexed"
        varchar_20 ServiceType FK "SystemCode"
        decimal_10_2 TotalAmount "Calculated"
        bit UseSubscription "Default false"
        int SubscriptionDeductionCount "Default 0"
        nvarchar_500 Memo "Optional"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    Subscription {
        bigint SubscriptionID PK "Auto-increment"
        bigint PetID FK "Required"
        bigint SubscriptionTypeID FK "Required"
        datetime2 StartDate "Required"
        datetime2 EndDate "Required, CHECK > StartDate"
        int TotalUsageLimit "Required, > 0"
        int UsedCount "Default 0"
        int ReservedCount "Default 0"
        decimal_10_2 TotalAmount "Required"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    SubscriptionType {
        bigint SubscriptionTypeID PK "Auto-increment"
        nvarchar_100 TypeName "Required"
        nvarchar_500 Description "Optional"
        int DefaultUsageLimit "Required"
        decimal_10_2 DefaultPrice "Required"
        bit IsActive "Default true"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    Service {
        bigint ServiceID PK "Auto-increment"
        nvarchar_100 ServiceName "Required"
        varchar_20 ServiceType FK "SystemCode"
        decimal_10_2 BasePrice "Required"
        int Duration "Minutes, default 60"
        nvarchar_500 Description "Optional"
        bit IsActive "Default true"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    ServiceAddon {
        bigint ServiceAddonID PK "Auto-increment"
        nvarchar_100 AddonName "Required"
        decimal_10_2 Price "Required"
        nvarchar_500 Description "Optional"
        bit IsActive "Default true"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    PetServicePrice {
        bigint PetServicePriceID PK "Auto-increment"
        bigint PetID FK "Required"
        bigint ServiceID FK "Required"
        decimal_10_2 CustomPrice "Required"
        datetime2 EffectiveDate "Required"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
        UNIQUE_PetID_ServiceID "Constraint"
    }

    ReservationService {
        bigint ReservationServiceID PK "Auto-increment"
        bigint ReserveRecordID FK "Required"
        bigint ServiceID FK "Required"
        decimal_10_2 Price "Snapshot"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
    }

    ReservationAddon {
        bigint ReservationAddonID PK "Auto-increment"
        bigint ReserveRecordID FK "Required"
        bigint ServiceAddonID FK "Required"
        decimal_10_2 Price "Snapshot"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
    }

    PaymentRecord {
        bigint PaymentRecordID PK "Auto-increment"
        decimal_10_2 Amount "Required"
        varchar_20 PaymentType "Income/Expense"
        varchar_20 PaymentCategory FK "SystemCode"
        datetime2 PaymentDate "Required, indexed"
        bigint ReserveRecordID FK "Optional"
        nvarchar_500 Memo "Optional"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 ModifyUser "Audit"
        datetime2 ModifyTime "Audit"
    }

    SystemCode {
        int CodeID PK "Auto-increment"
        varchar_50 CodeType "Required, indexed"
        varchar_50 Code "Required"
        nvarchar_100 Name "Required"
        nvarchar_100 CodeName "Display name"
        int Sort "Display order"
        bit IsActive "Default true"
        datetime2 StartDate "Optional"
        datetime2 EndDate "Optional"
        nvarchar_50 CreateUser "Audit"
        datetime2 CreateTime "Audit"
        nvarchar_50 UpdateUser "Audit"
        datetime2 UpdateTime "Audit"
        UNIQUE_CodeType_Code "Constraint"
    }
```

## Entity Specifications

### 1. Pet Entity

**Purpose**: Core entity representing pets being groomed.

**Validation Rules**:
- `PetName`: Required, max 100 characters, trim whitespace
- `Gender`: Required, must exist in SystemCode where CodeType='Gender'
- `Breed`: Required, must exist in SystemCode where CodeType='Breed'
- `BirthDay`: Optional, cannot be future date
- `NormalPrice`: Optional, must be ≥ 0 if provided
- `SubscriptionPrice`: Optional, must be ≥ 0 if provided
- `PhotoPath`: Optional, validated via file upload API (max 10MB, JPG/PNG/GIF only)

**Business Rules**:
- Soft delete: Do not allow hard delete if pet has reservations or subscriptions
- Photo naming: `{PetID}_{GUID}.{extension}`
- Custom pricing via `PetServicePrice` overrides `Service.BasePrice`

**Indexes**:
- Primary key: `PetID` (clustered)
- Non-clustered: `Gender`, `Breed` (for filtering)
- Full-text: `PetName` (for search)

### 2. ContactPerson Entity

**Purpose**: Pet owners and related contacts.

**Validation Rules**:
- `Name`: Required, max 100 characters
- `NickName`: Optional, max 50 characters
- `ContactNumber`: Required, max 20 characters, format validation (e.g., phone regex)

**Business Rules**:
- Cannot delete if linked to pets via `PetRelation`
- Must have at least one pet relationship to be meaningful

**Indexes**:
- Primary key: `ContactPersonID` (clustered)
- Non-clustered: `ContactNumber` (for lookup by phone)
- Full-text: `Name` (for search)

### 3. PetRelation Entity

**Purpose**: Many-to-many relationship between pets and contacts.

**Validation Rules**:
- `PetID`: Required, must exist in Pet table
- `ContactPersonID`: Required, must exist in ContactPerson table
- `RelationshipType`: Required, must exist in SystemCode where CodeType='Relationship'
- Unique constraint: Cannot have duplicate (PetID, ContactPersonID, RelationshipType)

**Business Rules**:
- One pet can have multiple contacts (owner, family, caregiver)
- One contact can manage multiple pets
- At least one contact should have relationship type 'Owner'

**Indexes**:
- Primary key: `PetRelationID` (clustered)
- Non-clustered: `PetID` (for "get contacts by pet")
- Non-clustered: `ContactPersonID` (for "get pets by contact")

### 4. ReserveRecord Entity

**Purpose**: Grooming appointment bookings.

**Validation Rules**:
- `PetID`: Required, must exist
- `SubscriptionID`: Optional, must exist if provided and be valid (not expired, has remaining count)
- `ReserverDate`: Required, cannot be more than 6 months in past or future
- `ReserverTime`: Required, format HH:mm
- `Status`: Required, must be valid SystemCode (PENDING, CONFIRMED, IN_PROGRESS, COMPLETED, CANCELLED, NO_SHOW)
- `ServiceType`: Required, must exist in SystemCode
- `TotalAmount`: Calculated field (sum of services + addons), cannot be negative
- `UseSubscription`: Boolean, true if using subscription
- `SubscriptionDeductionCount`: Default 0, must be ≤ subscription remaining count

**Business Rules**:
- Cannot double-book same time slot (check for conflicts)
- When booking with subscription: increment `ReservedCount`, decrement `RemainingCount`
- When completing appointment: increment `UsedCount`, decrement `ReservedCount`
- When cancelling: decrement `ReservedCount`, increment `RemainingCount`
- Auto-create PaymentRecord when status changes to COMPLETED

**State Transitions**:
```
PENDING → CONFIRMED → IN_PROGRESS → COMPLETED
    ↓          ↓           ↓
CANCELLED  CANCELLED  CANCELLED
    ↓          ↓           ↓
              NO_SHOW  NO_SHOW
```

**Indexes**:
- Primary key: `ReserveRecordID` (clustered)
- Non-clustered: `PetID` (for pet history)
- Non-clustered: `SubscriptionID` (for subscription usage)
- Composite: `(ReserverDate, Status)` (for calendar queries)

### 5. Subscription Entity

**Purpose**: Monthly grooming packages for pets.

**Validation Rules**:
- `PetID`: Required, must exist
- `SubscriptionTypeID`: Required, must exist
- `StartDate`: Required, cannot be more than 1 year in past
- `EndDate`: Required, must be > StartDate, typically StartDate + 1 month
- `TotalUsageLimit`: Required, must be > 0
- `UsedCount`: Default 0, must be ≤ TotalUsageLimit
- `ReservedCount`: Default 0, must be ≥ 0
- `TotalAmount`: Required, must be > 0

**Calculated Fields**:
- `RemainingCount = TotalUsageLimit - UsedCount - ReservedCount`

**Business Rules**:
- Cannot have overlapping subscriptions for same pet (validate date ranges)
- Cannot delete subscription if it has reserved or used counts > 0
- Warn when RemainingCount = 0 (subscription exhausted)
- Warn when EndDate is within 7 days (renewal reminder)
- Subscription is valid if: `CurrentDate BETWEEN StartDate AND EndDate AND RemainingCount > 0`

**Indexes**:
- Primary key: `SubscriptionID` (clustered)
- Non-clustered: `PetID` (for pet subscriptions)
- Composite: `(PetID, StartDate, EndDate)` (for overlap checks)

### 6. Service Entity

**Purpose**: Grooming service definitions (base services).

**Validation Rules**:
- `ServiceName`: Required, unique, max 100 characters
- `ServiceType`: Required, must exist in SystemCode
- `BasePrice`: Required, must be ≥ 0
- `Duration`: Required, must be > 0, in minutes
- `IsActive`: Boolean, default true

**Business Rules**:
- Cannot delete if referenced by PetServicePrice or ReservationService
- Soft delete by setting IsActive = false
- BasePrice can be overridden per pet via PetServicePrice

**Indexes**:
- Primary key: `ServiceID` (clustered)
- Non-clustered: `ServiceType` (for filtering by type)
- Non-clustered: `IsActive` (for active services list)

### 7. ServiceAddon Entity

**Purpose**: Additional services/upgrades (styling, treatments, etc.).

**Validation Rules**:
- `AddonName`: Required, unique, max 100 characters
- `Price`: Required, must be ≥ 0
- `IsActive`: Boolean, default true

**Business Rules**:
- Cannot delete if referenced by ReservationAddon
- Soft delete by setting IsActive = false

**Indexes**:
- Primary key: `ServiceAddonID` (clustered)
- Non-clustered: `IsActive` (for active addons list)

### 8. PetServicePrice Entity

**Purpose**: Custom pricing per pet for specific services.

**Validation Rules**:
- `PetID`: Required, must exist
- `ServiceID`: Required, must exist
- `CustomPrice`: Required, must be ≥ 0
- `EffectiveDate`: Required, when this price becomes active
- Unique constraint: (PetID, ServiceID)

**Business Rules**:
- Custom price overrides Service.BasePrice for this pet
- When calculating reservation cost, check PetServicePrice first, then Service.BasePrice
- Multiple price records allowed per pet/service with different effective dates (price history)

**Indexes**:
- Primary key: `PetServicePriceID` (clustered)
- Unique: `(PetID, ServiceID)` (enforce one active price per pet/service)

### 9. PaymentRecord Entity

**Purpose**: Financial transaction tracking.

**Validation Rules**:
- `Amount`: Required, must be != 0
- `PaymentType`: Required, enum ('Income', 'Expense')
- `PaymentCategory`: Required, must exist in SystemCode (IncomeType or ExpenseType)
- `PaymentDate`: Required, cannot be future date
- `ReserveRecordID`: Optional, links payment to reservation

**Business Rules**:
- Income: Amount > 0, PaymentCategory from IncomeType SystemCodes
- Expense: Amount < 0, PaymentCategory from ExpenseType SystemCodes
- Auto-created when ReserveRecord status changes to COMPLETED
- Manual entry allowed for non-reservation income/expenses (retail, rent, utilities)

**Indexes**:
- Primary key: `PaymentRecordID` (clustered)
- Non-clustered: `PaymentDate` (for date range queries)
- Non-clustered: `PaymentType, PaymentCategory` (for financial reports)

### 10. SystemCode Entity

**Purpose**: Configurable lookup data.

**Validation Rules**:
- `CodeType`: Required, max 50 characters
- `Code`: Required, max 50 characters
- `Name` or `CodeName`: Required, max 100 characters
- `Sort`: Optional, for display ordering
- `IsActive`: Boolean, default true
- Unique constraint: (CodeType, Code)

**Code Types**:
- `Breed`: Dog breeds (Poodle, Golden Retriever, Shiba Inu, etc.)
- `Gender`: Male, Female
- `ServiceType`: Bath, Grooming, Nail Trimming, Special Styling, Monthly Package
- `ReservationStatus`: PENDING, CONFIRMED, IN_PROGRESS, COMPLETED, CANCELLED, NO_SHOW
- `Relationship`: Owner, Father, Mother, Family, Friend, Caregiver
- `IncomeType`: Grooming, Retail, Addon, Subscription
- `ExpenseType`: Utilities, Phone, Rent, Supplies, Equipment, Marketing

**Business Rules**:
- Cannot delete if referenced by other tables
- Soft delete by setting IsActive = false
- StartDate/EndDate allow for seasonal or promotional codes

**Indexes**:
- Primary key: `CodeID` (clustered)
- Unique: `(CodeType, Code)` (business key)
- Non-clustered: `CodeType` (for fetching codes by type)

## Relationships & Cardinality

| Parent | Child | Type | Cascade |
|--------|-------|------|---------|
| Pet | PetRelation | One-to-Many | Restrict (prevent delete if relations exist) |
| ContactPerson | PetRelation | One-to-Many | Restrict |
| Pet | ReserveRecord | One-to-Many | Restrict |
| Pet | Subscription | One-to-Many | Restrict |
| Pet | PetServicePrice | One-to-Many | Cascade (delete custom pricing if pet deleted) |
| Subscription | ReserveRecord | One-to-Many | Restrict |
| SubscriptionType | Subscription | One-to-Many | Restrict |
| Service | PetServicePrice | One-to-Many | Cascade |
| Service | ReservationService | One-to-Many | Restrict |
| ServiceAddon | ReservationAddon | One-to-Many | Restrict |
| ReserveRecord | ReservationService | One-to-Many | Cascade |
| ReserveRecord | ReservationAddon | One-to-Many | Cascade |
| ReserveRecord | PaymentRecord | One-to-Many | Set Null (keep payment even if reservation deleted) |

## Data Integrity Constraints

### Check Constraints

1. **Subscription Dates**: `EndDate > StartDate`
2. **Subscription Counts**: `UsedCount >= 0 AND ReservedCount >= 0 AND TotalUsageLimit > 0`
3. **Pricing**: `BasePrice >= 0 AND CustomPrice >= 0 AND TotalAmount >= 0`
4. **Payment Types**: `PaymentType IN ('Income', 'Expense')`

### Unique Constraints

1. `SystemCode`: `(CodeType, Code)` - no duplicate codes within same type
2. `PetServicePrice`: `(PetID, ServiceID)` - one active custom price per pet/service
3. `Service.ServiceName` - service names must be unique

### Foreign Key Constraints

All foreign keys enforce referential integrity. Delete behaviors:
- **Restrict**: Prevent parent delete if children exist (most relationships)
- **Cascade**: Delete children when parent deleted (detail records like ReservationService)
- **Set Null**: Nullify foreign key when parent deleted (optional relationships like PaymentRecord.ReserveRecordID)

## Audit Fields Pattern

All tables include:
- `CreateUser` (nvarchar(50)): Username or UserId who created the record
- `CreateTime` (datetime2): UTC timestamp when record was created
- `ModifyUser` (nvarchar(50)): Username or UserId who last modified the record
- `ModifyTime` (datetime2): UTC timestamp when record was last modified

These fields are automatically populated by `EntitySaveChangesInterceptor` and should never be manually set in business logic.

## Indexes Summary

### Critical Performance Indexes

1. **Calendar Queries**: `ReserveRecord(ReserverDate, Status)` - composite index for fast calendar loading
2. **Phone Lookup**: `ContactPerson(ContactNumber)` - for quick contact search
3. **Subscription Validation**: `Subscription(PetID, StartDate, EndDate)` - for overlap checks
4. **Financial Reports**: `PaymentRecord(PaymentDate, PaymentType, PaymentCategory)` - for report generation

### Foreign Key Indexes

All foreign key columns should have non-clustered indexes for join performance:
- `PetRelation(PetID)`, `PetRelation(ContactPersonID)`
- `ReserveRecord(PetID)`, `ReserveRecord(SubscriptionID)`
- `Subscription(PetID)`
- etc.

## Next Steps

1. Generate OpenAPI contracts for API endpoints based on this data model
2. Create quickstart.md with database setup instructions
3. Update agent context files with entity information
