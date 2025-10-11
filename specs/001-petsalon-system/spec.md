# Feature Specification: PetSalon Management System

**Feature Branch**: `001-petsalon-system`
**Created**: 2025-10-11
**Status**: Draft
**Input**: User description: ".NET 8 + Vue 3 + TypeScript + SQL Server，支援桌面和行動裝置"

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Pet Registration & Management (Priority: P1) 🎯 MVP

As a pet grooming staff member, I need to register new pets and maintain their information so that I can provide personalized grooming services and track their history.

**Why this priority**: This is the foundation of the entire system. Without pet data, no other features (reservations, subscriptions, payments) can function. This delivers immediate value by digitizing pet records.

**Independent Test**: Can be fully tested by creating a new pet record with name, breed, gender, birthday, pricing, and photo. Staff can view, edit, and manage pet information without requiring any other features.

**Acceptance Scenarios**:

1. **Given** no pets exist in the system, **When** I click "Add New Pet" and enter pet details (name, breed, gender, birthday, pricing), **Then** the system creates a pet record with a unique ID
2. **Given** a pet exists, **When** I upload a pet photo (JPG/PNG/GIF), **Then** the system stores the photo securely and displays it in the pet profile
3. **Given** a pet record exists, **When** I update pricing information (normal price, subscription price), **Then** the system saves the new pricing and uses it for future calculations
4. **Given** multiple pets exist, **When** I search by pet name or filter by breed, **Then** the system displays matching pets instantly
5. **Given** a pet record exists, **When** I view pet details on mobile device, **Then** the interface adapts to mobile screen with touch-friendly controls

---

### User Story 2 - Contact Person Management (Priority: P1) 🎯 MVP

As a staff member, I need to manage pet owners and their contact information so that I can communicate with them about appointments and send reminders.

**Why this priority**: Essential for business operations. Staff need to know who to contact for appointments, payment follow-ups, and emergencies. Works independently but naturally complements pet management.

**Independent Test**: Can be fully tested by creating contact persons with phone numbers, linking them to pets with relationship types (owner, family, caregiver), and managing contact lists.

**Acceptance Scenarios**:

1. **Given** no contacts exist, **When** I add a new contact person with name, nickname, and phone number, **Then** the system creates a contact record
2. **Given** a contact person and a pet exist, **When** I link them with relationship type "Owner", **Then** the system establishes the relationship and shows it in both profiles
3. **Given** a pet has multiple contacts, **When** I view the pet profile, **Then** the system displays all linked contacts with their relationship types
4. **Given** a contact person exists, **When** I search for them by name or phone number, **Then** the system quickly locates and displays their profile
5. **Given** viewing contacts on mobile, **When** I tap a phone number, **Then** the device prompts to make a call

---

### User Story 3 - Appointment Booking & Calendar (Priority: P2)

As a staff member, I need to book grooming appointments and view them in a calendar format so that I can manage daily schedules and avoid overbooking.

**Why this priority**: Core business function that depends on Pet and Contact data. Enables revenue generation through service bookings.

**Independent Test**: Can be fully tested by creating appointments with date/time, selecting services, viewing calendar, and managing appointment statuses (pending, confirmed, in-progress, completed, cancelled).

**Acceptance Scenarios**:

1. **Given** a pet exists, **When** I create a new appointment with date, time, and service type, **Then** the system books the appointment and shows it in the calendar
2. **Given** multiple appointments exist, **When** I view the calendar, **Then** the system displays appointments color-coded by status with time slots
3. **Given** an appointment exists, **When** I change its status to "Confirmed", **Then** the system updates the status and sends notification (if configured)
4. **Given** viewing calendar on mobile, **When** I tap an appointment, **Then** a bottom sheet shows appointment details with action buttons
5. **Given** an appointment has a pet with subscription, **When** I view appointment details, **Then** the system shows available subscription and suggests using it

---

### User Story 4 - Subscription Package Management (Priority: P2)

As a staff member, I need to create and manage monthly subscription packages for pets so that I can offer discounted grooming services and ensure recurring revenue.

**Why this priority**: Important for customer retention and predictable revenue. Builds on Pet management. Integrates with appointments.

**Independent Test**: Can be fully tested by creating subscriptions with start/end dates, usage limits, tracking used/reserved counts, and checking availability.

**Acceptance Scenarios**:

1. **Given** a pet exists, **When** I create a monthly subscription with 4 grooming sessions, **Then** the system records the subscription with start/end dates and usage limits
2. **Given** a subscription exists, **When** I book an appointment using the subscription, **Then** the system reserves one count and updates remaining usage
3. **Given** a reservation is confirmed, **When** the grooming is completed, **Then** the system deducts the reserved count from the subscription
4. **Given** a subscription is about to expire, **When** 7 days remain, **Then** the system flags it for renewal reminder
5. **Given** viewing subscriptions on mobile, **When** I swipe a subscription card, **Then** quick actions appear (renew, view usage, cancel)

---

### User Story 5 - Payment & Financial Tracking (Priority: P3)

As a business owner, I need to track all income and expenses so that I can monitor financial health and generate reports for decision-making.

**Why this priority**: Important for business insights but not blocking daily operations. Can be added after core workflow is established.

**Independent Test**: Can be fully tested by recording payment transactions (income from grooming, retail, subscriptions; expenses for utilities, rent, supplies), categorizing them, and generating financial reports.

**Acceptance Scenarios**:

1. **Given** an appointment is completed, **When** I mark it as paid, **Then** the system automatically creates a payment record with income type "Grooming"
2. **Given** I purchase supplies, **When** I record an expense with type "Supplies" and amount, **Then** the system logs it in the expense ledger
3. **Given** multiple transactions exist, **When** I view monthly financial report, **Then** the system shows total income, total expenses, and net profit with category breakdowns
4. **Given** viewing financial dashboard on tablet, **When** I pinch to zoom charts, **Then** the charts scale responsively with touch gestures
5. **Given** I need to export data, **When** I select date range and export format, **Then** the system generates downloadable report

---

### User Story 6 - System Administration & Reporting (Priority: P3)

As a system administrator, I need to manage system codes, user accounts, and generate analytics reports so that I can maintain the system and gain business insights.

**Why this priority**: Necessary for long-term maintenance but not required for MVP launch. Can be added iteratively.

**Independent Test**: Can be fully tested by managing system codes (breeds, genders, service types, statuses), configuring user roles, and generating reports on reservations, subscriptions, and revenue trends.

**Acceptance Scenarios**:

1. **Given** I'm an admin user, **When** I add a new breed to SystemCode, **Then** the breed appears in pet registration dropdowns across the system
2. **Given** multiple users exist, **When** I assign "Admin" role to a user, **Then** they gain access to system administration features
3. **Given** 3 months of data exist, **When** I generate quarterly report, **Then** the system shows trends in appointments, popular services, and revenue growth
4. **Given** viewing admin panel on desktop, **When** I access audit logs, **Then** the system displays all data modifications with user and timestamp
5. **Given** system codes need updates, **When** I edit a code type on mobile, **Then** the form adapts with proper input controls

---

### Edge Cases

- What happens when a pet photo upload exceeds 10MB size limit?
  → System rejects upload and displays error message with size limit

- How does system handle double-booking (same time slot)?
  → System prevents overlapping appointments by validating time conflicts before saving

- What happens when a subscription expires but appointments are still using it?
  → System flags expired subscriptions and prevents new reservations, but honors existing confirmed appointments

- How does system handle network loss during mobile appointment booking?
  → Frontend caches form data in local storage and prompts to retry when connection returns

- What happens when a contact person is deleted but pets are still linked?
  → System prevents deletion if pets are linked, requires unlinking first or reassigning pets

- How does system handle concurrent edits by multiple staff members?
  → Last-write-wins with optimistic concurrency using ModifyTime field; shows warning if data changed

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: System MUST allow staff to register new pets with name, breed, gender, birthday, pricing (normal and subscription), and optional photo upload
- **FR-002**: System MUST support photo upload for pets in JPG, PNG, GIF formats with maximum 10MB file size
- **FR-003**: System MUST allow staff to create and manage contact persons with name, nickname, and phone number
- **FR-004**: System MUST support linking pets to contact persons with relationship types (Owner, Father, Mother, Family, Friend, Caregiver)
- **FR-005**: System MUST allow staff to book appointments with pet, date, time, service type, and optional notes
- **FR-006**: System MUST display appointments in calendar view with daily, weekly, and monthly formats
- **FR-007**: System MUST support appointment status workflow (Pending → Confirmed → In Progress → Completed/Cancelled/No Show)
- **FR-008**: System MUST allow creating subscription packages for pets with start date, end date, total usage limit, and pricing
- **FR-009**: System MUST track subscription usage (total limit, used count, reserved count, remaining count)
- **FR-010**: System MUST automatically reserve subscription counts when appointment is booked with subscription
- **FR-011**: System MUST deduct subscription counts when appointment is marked as completed
- **FR-012**: System MUST record payment transactions with type (income/expense), category, amount, and date
- **FR-013**: System MUST auto-create payment record when appointment is marked as paid
- **FR-014**: System MUST provide financial reports showing income, expenses, and net profit by time period
- **FR-015**: System MUST support system code management for configurable lookup data (breeds, genders, service types, statuses, relationships, payment types)
- **FR-016**: System MUST maintain audit trail for all data modifications (CreateUser, CreateTime, ModifyUser, ModifyTime)
- **FR-017**: System MUST authenticate users via JWT tokens with role-based access control (Admin, User roles)
- **FR-018**: System MUST be responsive and functional on desktop (1024px+), tablet (768px), and mobile (320px+) screens
- **FR-019**: System MUST provide touch-friendly UI elements with minimum 44px touch targets on mobile devices
- **FR-020**: System MUST validate all inputs on both client-side (TypeScript) and server-side (.NET) with clear error messages

### Non-Functional Requirements

- **NFR-001**: System response time MUST be under 2 seconds for all CRUD operations under normal load
- **NFR-002**: System MUST handle at least 50 concurrent users without performance degradation
- **NFR-003**: Mobile pages MUST load within 3 seconds on 3G network connection
- **NFR-004**: System MUST have 99.5% uptime during business hours (9 AM - 9 PM local time)
- **NFR-005**: Database backups MUST be performed daily with 30-day retention
- **NFR-006**: User passwords MUST be hashed using industry-standard algorithms (bcrypt/PBKDF2)
- **NFR-007**: All API endpoints MUST be documented in Swagger/OpenAPI format
- **NFR-008**: System MUST support cross-browser compatibility (Chrome, Firefox, Safari, Edge latest versions)
- **NFR-009**: System MUST use HTTPS for all communications in production
- **NFR-010**: System MUST support data export in CSV/Excel format for reports

### Key Entities

- **Pet**: Represents pets being groomed. Attributes: PetID, PetName, Breed (SystemCode), Gender (SystemCode), BirthDay, NormalPrice, SubscriptionPrice, Photo path, audit fields
- **ContactPerson**: Pet owners and related contacts. Attributes: ContactPersonID, Name, NickName, ContactNumber, audit fields
- **PetRelation**: Links pets to contacts. Attributes: PetRelationID, PetID, ContactPersonID, RelationshipType (SystemCode), audit fields
- **ReserveRecord**: Grooming appointments. Attributes: ReserveRecordID, PetID, SubscriptionID (optional), ReserverDate, ReserverTime, Status (SystemCode), ServiceType (SystemCode), TotalAmount, UseSubscription flag, Memo, audit fields
- **Subscription**: Monthly packages. Attributes: SubscriptionID, PetID, SubscriptionTypeID, StartDate, EndDate, TotalUsageLimit, UsedCount, ReservedCount, TotalAmount, audit fields
- **PaymentRecord**: Financial transactions. Attributes: PaymentRecordID, Amount, PaymentType (income/expense), PaymentCategory (SystemCode), PaymentDate, ReserveRecordID (optional), Memo, audit fields
- **Service**: Grooming service definitions. Attributes: ServiceID, ServiceName, ServiceType (SystemCode), BasePrice, Duration, Description, IsActive, audit fields
- **SystemCode**: Configurable lookup data. Attributes: CodeID, CodeType, Code, CodeName, Name, Sort, IsActive, StartDate, EndDate, audit fields

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Staff can register a new pet with complete information (including photo) in under 3 minutes
- **SC-002**: Staff can book an appointment from pet selection to confirmation in under 2 minutes
- **SC-003**: 90% of appointment bookings successfully use subscription packages when available
- **SC-004**: System handles 100 concurrent users during peak hours (Saturday mornings) without errors
- **SC-005**: Mobile interface usability score of 4.0/5.0 or higher from staff feedback surveys
- **SC-006**: System reduces phone call workload by 30% through digitized appointment management
- **SC-007**: Financial reports generated within 5 seconds for any time period up to 1 year
- **SC-008**: 95% of all user actions complete successfully without errors on first attempt
- **SC-009**: Zero data loss incidents during first 6 months of operation
- **SC-010**: Mobile pages achieve Lighthouse performance score of 80+ on 3G connection
