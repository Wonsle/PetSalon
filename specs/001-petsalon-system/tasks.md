# Tasks: PetSalon Management System

**Input**: Design documents from `/specs/001-petsalon-system/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/openapi.yaml ✅

**Tests**: Tests are NOT included in this task list (not explicitly requested in specification)

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`
- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions
- Backend: `PetSalon.Backend/PetSalon.[Layer]/`
- Frontend: `PetSalon.Frontend/src/`
- SQL: `SQL/`

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Verify .NET 8 SDK and Node.js 20+ installed on development machine
- [ ] T002 Verify SQL Server 2019+ connection and create PetSalon database
- [ ] T003 [P] Initialize SQL Server with SystemCode data: Run scripts in `SQL/70-InintialData/` (Breed, Gender, ServiceType, ReservationStatus, Relationship, PaymentType)
- [ ] T004 [P] Configure backend connection string in `PetSalon.Backend/PetSalon.Web/appsettings.json` and `appsettings.Development.json`
- [ ] T005 [P] Configure JWT settings in `PetSalon.Backend/PetSalon.Web/appsettings.json` (Issuer, Audience, SecretKey)
- [ ] T006 [P] Install frontend dependencies: Run `npm install` in `PetSalon.Frontend/`
- [ ] T007 [P] Configure Vite proxy to backend API in `PetSalon.Frontend/vite.config.ts` (proxy http://localhost:5150)
- [ ] T008 Verify backend builds successfully: `dotnet build PetSalon.Backend/PetSalon.sln`
- [ ] T009 Verify frontend builds successfully: `npm run build` in `PetSalon.Frontend/`
- [ ] T010 [P] Create pet photo upload directory: `PetSalon.Backend/PetSalon.Web/wwwroot/uploads/pets/`

**Checkpoint**: Basic project structure verified and dependencies installed

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T011 Create database tables: Run SQL scripts in `SQL/10-Table/` for Pet, ContactPerson, PetRelation, ReserveRecord, Subscription, SubscriptionType, Service, ServiceAddon, PetServicePrice, ReservationService, ReservationAddon, PaymentRecord, SystemCode
- [ ] T012 Verify all foreign key constraints and indexes are created correctly
- [ ] T013 Regenerate EF Core entity models from database using EF Core Power Tools with `PetSalon.Backend/PetSalon.Models/efpt.config.json`
- [ ] T014 [P] Implement `EntitySaveChangesInterceptor` in `PetSalon.Backend/PetSalon.Tools/Interceptors/EntitySaveChangesInterceptor.cs` for automatic audit field population
- [ ] T015 [P] Implement `JwtHelpers` in `PetSalon.Backend/PetSalon.Tools/Helpers/JwtHelpers.cs` for token generation and validation
- [ ] T016 [P] Implement `IUserContext` and `JwtUserContext` in `PetSalon.Backend/PetSalon.Tools/Context/` to extract current user from JWT claims
- [ ] T017 Configure DbContext with SaveChangesInterceptor in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T018 Configure JWT authentication middleware in `PetSalon.Backend/PetSalon.Web/Program.cs` with bearer token validation
- [ ] T019 Configure CORS policy in `PetSalon.Backend/PetSalon.Web/Program.cs` to allow frontend origin (http://localhost:3000, http://localhost:3001)
- [ ] T020 [P] Create base API service class in `PetSalon.Frontend/src/api/baseApi.ts` with Axios instance, interceptors for JWT token attachment, and error handling
- [ ] T021 [P] Create Pinia auth store in `PetSalon.Frontend/src/stores/auth.ts` for authentication state management (user, token, isAuthenticated)
- [ ] T022 [P] Configure Vue Router navigation guards in `PetSalon.Frontend/src/router/index.ts` to protect routes requiring authentication
- [ ] T023 [P] Create TypeScript interfaces in `PetSalon.Frontend/src/types/common.ts` for SystemCode, ApiResponse, PaginationRequest, PaginationResponse
- [ ] T024 [P] Create CommonService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/ICommonService.cs` with methods: GetSystemCodesByType, GetSystemCodeTypes, CreateSystemCode, UpdateSystemCode, DeleteSystemCode
- [ ] T025 Implement CommonService in `PetSalon.Backend/PetSalon.Services/Implementations/CommonService.cs` with SystemCode CRUD operations
- [ ] T026 [P] Create CommonController in `PetSalon.Backend/PetSalon.Web/Controllers/CommonController.cs` with endpoints: GET /api/common/systemcodes/{codeType}, POST /api/common/systemcodes, PUT /api/common/systemcodes/{id}, DELETE /api/common/systemcodes/{id}
- [ ] T027 Register CommonService in dependency injection container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T028 [P] Create common API service in `PetSalon.Frontend/src/api/commonApi.ts` with methods to fetch SystemCodes by type
- [ ] T029 [P] Create reusable form components in `PetSalon.Frontend/src/components/common/`: FormInput.vue, FormSelect.vue, FormDatePicker.vue, FormTextarea.vue with PrimeVue and validation
- [ ] T030 [P] Setup responsive layout structure in `PetSalon.Frontend/src/components/layout/`: AppLayout.vue, AppHeader.vue, AppSidebar.vue, AppFooter.vue with mobile-first design and 44px touch targets
- [ ] T031 Test foundation: Verify JWT token generation, SystemCode API endpoints respond correctly, frontend can authenticate and fetch SystemCodes

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - Pet Registration & Management (Priority: P1) 🎯 MVP

**Goal**: Enable staff to register new pets with complete information, upload photos, search/filter pets, and manage pet data on desktop and mobile devices

**Independent Test**: Create a new pet record with name "Max", breed "Poodle", gender "Male", birthday, pricing. Upload a photo. Search for "Max" and verify it appears. Edit pricing. View pet details on mobile browser and verify responsive layout with touch-friendly controls.

### Implementation for User Story 1

- [ ] T032 [P] [US1] Create Pet DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/PetDto.cs`: CreatePetRequest, UpdatePetRequest, PetResponse, PetListItemResponse
- [ ] T033 [P] [US1] Create TypeScript interfaces in `PetSalon.Frontend/src/types/pet.ts`: Pet, CreatePetRequest, UpdatePetRequest, PetListItem
- [ ] T034 [P] [US1] Create IPetService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IPetService.cs` with methods: GetAllPets, GetPetById, CreatePet, UpdatePet, DeletePet, UploadPetPhoto, SearchPets
- [ ] T035 [US1] Implement PetService in `PetSalon.Backend/PetSalon.Services/Implementations/PetService.cs` with full CRUD operations, photo upload validation (10MB max, JPG/PNG/GIF), filename generation ({petId}_{guid}.{ext}), and search logic
- [ ] T036 [US1] Create PetController in `PetSalon.Backend/PetSalon.Web/Controllers/PetController.cs` with endpoints: GET /api/pet, GET /api/pet/{id}, POST /api/pet, PUT /api/pet/{id}, DELETE /api/pet/{id}, POST /api/pet/{id}/photo
- [ ] T037 Register PetService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T038 [P] [US1] Create pet API service in `PetSalon.Frontend/src/api/petApi.ts` with typed methods for all pet endpoints including photo upload with FormData
- [ ] T039 [P] [US1] Create Pinia pet store in `PetSalon.Frontend/src/stores/pet.ts` for pet list state, selected pet, loading states, and caching
- [ ] T040 [US1] Create PetList view in `PetSalon.Frontend/src/views/pet/PetList.vue` with PrimeVue DataTable, search input, breed/gender filters, add pet button, responsive grid on mobile
- [ ] T041 [US1] Create PetDetail view in `PetSalon.Frontend/src/views/pet/PetDetail.vue` displaying pet information, photo, pricing, with edit button and mobile-responsive layout
- [ ] T042 [US1] Create PetForm component in `PetSalon.Frontend/src/components/pet/PetForm.vue` with fields: name (required), breed (SystemCode dropdown), gender (SystemCode dropdown), birthday (DatePicker), normal price, subscription price, photo upload with 10MB validation and preview
- [ ] T043 [US1] Add pet routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /pets (list), /pets/:id (detail), /pets/new (create), /pets/:id/edit (edit)
- [ ] T044 [US1] Implement mobile-responsive CSS for pet views with touch targets ≥44px, proper spacing, and mobile-first breakpoints (320px, 768px, 1024px)
- [ ] T045 [US1] Add client-side validation for pet form: required fields, birthday not in future, prices ≥0, photo size ≤10MB, photo type in [JPG, PNG, GIF]
- [ ] T046 [US1] Add server-side validation in PetService: trim whitespace from PetName, validate breed/gender exist in SystemCode, validate photo extension and size
- [ ] T047 [US1] Implement error handling for pet photo upload failures (disk full, invalid format, size exceeded) with user-friendly messages

**Checkpoint**: User Story 1 complete - Staff can register, search, edit pets with photos on desktop and mobile independently

---

## Phase 4: User Story 2 - Contact Person Management (Priority: P1) 🎯 MVP

**Goal**: Enable staff to manage pet owners and contacts with phone numbers, link them to pets with relationship types, search contacts, and access contact information on mobile devices

**Independent Test**: Create a new contact person "John Smith" with phone "0912345678". Link John to pet "Max" with relationship "Owner". Create another contact "Jane Doe" with relationship "Family". Search for "John" and verify contact appears. View contact profile with linked pets. Tap phone number on mobile and verify call prompt.

### Implementation for User Story 2

- [ ] T048 [P] [US2] Create ContactPerson DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/ContactPersonDto.cs`: CreateContactPersonRequest, UpdateContactPersonRequest, ContactPersonResponse, ContactPersonListItemResponse, LinkPetRequest
- [ ] T049 [P] [US2] Create TypeScript interfaces in `PetSalon.Frontend/src/types/contact.ts`: ContactPerson, CreateContactRequest, UpdateContactRequest, ContactListItem, PetRelation
- [ ] T050 [P] [US2] Create IContactPersonService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IContactPersonService.cs` with methods: GetAllContacts, GetContactById, CreateContact, UpdateContact, DeleteContact, LinkPetToContact, UnlinkPetFromContact, GetPetsByContactId, GetContactsByPetId
- [ ] T051 [US2] Implement ContactPersonService in `PetSalon.Backend/PetSalon.Services/Implementations/ContactPersonService.cs` with full CRUD, validation (phone format, cannot delete if linked to pets), relationship management, and search logic
- [ ] T052 [US2] Create ContactPersonController in `PetSalon.Backend/PetSalon.Web/Controllers/ContactPersonController.cs` with endpoints: GET /api/contactperson, GET /api/contactperson/{id}, POST /api/contactperson, PUT /api/contactperson/{id}, DELETE /api/contactperson/{id}, POST /api/contactperson/{contactId}/pets/{petId}, DELETE /api/contactperson/{contactId}/pets/{petId}
- [ ] T053 Register ContactPersonService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T054 [P] [US2] Create contact API service in `PetSalon.Frontend/src/api/contactApi.ts` with typed methods for all contact and relationship endpoints
- [ ] T055 [P] [US2] Create Pinia contact store in `PetSalon.Frontend/src/stores/contact.ts` for contact list state, selected contact, and caching
- [ ] T056 [US2] Create ContactList view in `PetSalon.Frontend/src/views/contact/ContactList.vue` with PrimeVue DataTable, search by name/phone, add contact button, responsive grid on mobile
- [ ] T057 [US2] Create ContactDetail view in `PetSalon.Frontend/src/views/contact/ContactDetail.vue` displaying contact info, linked pets with relationship types, clickable phone number (tel: link), mobile-responsive layout
- [ ] T058 [US2] Create ContactForm component in `PetSalon.Frontend/src/components/contact/ContactForm.vue` with fields: name (required), nickname, contact number (required, phone validation), relationship type dropdown when linking to pet
- [ ] T059 [US2] Create PetRelationManager component in `PetSalon.Frontend/src/components/contact/PetRelationManager.vue` to add/remove pet relationships with dropdown to select pet and relationship type (Owner, Family, Caregiver, etc.)
- [ ] T060 [US2] Add contact routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /contacts (list), /contacts/:id (detail), /contacts/new (create), /contacts/:id/edit (edit)
- [ ] T061 [US2] Integrate contact linking in PetDetail view: Show linked contacts with relationship types, add "Link Contact" button to open dialog
- [ ] T062 [US2] Implement mobile-responsive CSS for contact views with clickable phone numbers, touch-friendly buttons, and proper spacing
- [ ] T063 [US2] Add client-side validation: required name, required contact number, phone format validation (Taiwan mobile: 09XX-XXXXXX)
- [ ] T064 [US2] Add server-side validation in ContactPersonService: prevent deletion if PetRelation exists, validate relationship type exists in SystemCode, prevent duplicate (PetID, ContactPersonID, RelationshipType)
- [ ] T065 [US2] Implement error handling for contact deletion blocked by pet relationships with clear user message suggesting to unlink pets first

**Checkpoint**: User Story 2 complete - Staff can manage contacts, link them to pets, search contacts, and access contact info on mobile independently

---

## Phase 5: User Story 3 - Appointment Booking & Calendar (Priority: P2)

**Goal**: Enable staff to book grooming appointments with date/time/service selection, view appointments in calendar format (daily/weekly/monthly), manage appointment statuses through workflow, and access calendar on mobile with bottom sheet modals

**Independent Test**: Create an appointment for pet "Max" on tomorrow at 2:00 PM with service type "Grooming". View the calendar and verify appointment appears. Change status to "Confirmed". View calendar on mobile and tap appointment to see bottom sheet with details. If pet has subscription, verify system suggests using it.

### Implementation for User Story 3

- [ ] T066 [P] [US3] Create ReserveRecord DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/ReservationDto.cs`: CreateReservationRequest, UpdateReservationRequest, ReservationResponse, ReservationListItemResponse, ReservationCalendarEventResponse
- [ ] T067 [P] [US3] Create TypeScript interfaces in `PetSalon.Frontend/src/types/reservation.ts`: Reservation, CreateReservationRequest, UpdateReservationRequest, ReservationListItem, CalendarEvent, ReservationStatus
- [ ] T068 [P] [US3] Create IReservationService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IReservationService.cs` with methods: GetAllReservations, GetReservationById, GetReservationsByDateRange, CreateReservation, UpdateReservation, DeleteReservation, UpdateStatus, CheckTimeConflict, GetAvailableSubscription
- [ ] T069 [US3] Implement ReservationService in `PetSalon.Backend/PetSalon.Services/Implementations/ReservationService.cs` with full CRUD, status workflow validation (PENDING→CONFIRMED→IN_PROGRESS→COMPLETED/CANCELLED/NO_SHOW), time conflict detection, subscription integration (reserve count when booking, deduct when completing), and calendar query optimization
- [ ] T070 [US3] Create ReservationController in `PetSalon.Backend/PetSalon.Web/Controllers/ReservationController.cs` with endpoints: GET /api/reservation, GET /api/reservation/{id}, GET /api/reservation/calendar?start={date}&end={date}, POST /api/reservation, PUT /api/reservation/{id}, DELETE /api/reservation/{id}, PATCH /api/reservation/{id}/status
- [ ] T071 Register ReservationService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T072 [P] [US3] Create reservation API service in `PetSalon.Frontend/src/api/reservationApi.ts` with typed methods for all reservation endpoints including calendar data fetching
- [ ] T073 [P] [US3] Create Pinia reservation store in `PetSalon.Frontend/src/stores/reservation.ts` for reservation list state, calendar events, selected reservation, and caching
- [ ] T074 [US3] Install and configure PrimeVue FullCalendar component in `PetSalon.Frontend/` with npm package
- [ ] T075 [US3] Create Calendar view in `PetSalon.Frontend/src/views/reservation/Calendar.vue` with PrimeVue FullCalendar, day/week/month views, color-coded events by status (Pending=yellow, Confirmed=blue, In Progress=green, Completed=gray, Cancelled=red), click event to open detail modal
- [ ] T076 [US3] Create ReservationList view in `PetSalon.Frontend/src/views/reservation/ReservationList.vue` with PrimeVue DataTable, filter by status/date/pet, add reservation button
- [ ] T077 [US3] Create ReservationDetail component in `PetSalon.Frontend/src/components/reservation/ReservationDetail.vue` displaying pet info, date/time, service type, status, memo, with status update buttons (Confirm, Start, Complete, Cancel), show available subscription suggestion
- [ ] T078 [US3] Create ReservationForm component in `PetSalon.Frontend/src/components/reservation/ReservationForm.vue` with fields: pet selector (searchable dropdown), date (DatePicker, cannot be >6 months past/future), time (TimePicker), service type (SystemCode dropdown), subscription toggle (if pet has valid subscription), memo, with time conflict validation
- [ ] T079 [US3] Implement mobile bottom sheet for appointment detail on mobile devices using PrimeVue Dialog with position bottom and swipe-to-close gesture
- [ ] T080 [US3] Add reservation routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /reservations (list), /reservations/calendar (calendar), /reservations/new (create), /reservations/:id (detail)
- [ ] T081 [US3] Implement mobile-responsive CSS for calendar view with touch-friendly event cards, simplified day view on mobile, and easy date navigation
- [ ] T082 [US3] Add client-side validation: required pet, required date/time, date within 6 months range, service type required, time conflict check before submission
- [ ] T083 [US3] Add server-side validation in ReservationService: validate status transitions, prevent double-booking with CheckTimeConflict, validate subscription is valid (not expired, has remaining count), validate pet exists
- [ ] T084 [US3] Implement subscription integration logic: When UseSubscription=true, increment ReservedCount, decrement RemainingCount; When status changes to COMPLETED, increment UsedCount, decrement ReservedCount; When CANCELLED, decrement ReservedCount, increment RemainingCount
- [ ] T085 [US3] Implement error handling for time conflicts with clear message showing conflicting appointment details and suggesting alternative times

**Checkpoint**: User Story 3 complete - Staff can book appointments, view calendar, manage statuses, and access on mobile independently

---

## Phase 6: User Story 4 - Subscription Package Management (Priority: P2)

**Goal**: Enable staff to create and manage monthly subscription packages for pets with usage limits, track used/reserved/remaining counts, flag expiring subscriptions for renewal reminders, and manage subscriptions on mobile with swipe actions

**Independent Test**: Create a monthly subscription for pet "Max" with 4 grooming sessions starting today, ending in 30 days. Book an appointment using the subscription and verify ReservedCount=1, RemainingCount=3. Complete the appointment and verify UsedCount=1, ReservedCount=0, RemainingCount=3. Create subscription expiring in 5 days and verify system flags it for renewal. View subscriptions on mobile and swipe card to reveal quick actions (renew, view usage, cancel).

### Implementation for User Story 4

- [ ] T086 [P] [US4] Create Subscription DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/SubscriptionDto.cs`: CreateSubscriptionRequest, UpdateSubscriptionRequest, SubscriptionResponse, SubscriptionListItemResponse, SubscriptionUsageResponse with calculated RemainingCount
- [ ] T087 [P] [US4] Create SubscriptionType DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/SubscriptionTypeDto.cs`: SubscriptionTypeResponse
- [ ] T088 [P] [US4] Create TypeScript interfaces in `PetSalon.Frontend/src/types/subscription.ts`: Subscription, CreateSubscriptionRequest, UpdateSubscriptionRequest, SubscriptionListItem, SubscriptionType, SubscriptionUsage
- [ ] T089 [P] [US4] Create ISubscriptionService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/ISubscriptionService.cs` with methods: GetAllSubscriptions, GetSubscriptionById, GetSubscriptionsByPetId, CreateSubscription, UpdateSubscription, DeleteSubscription, CheckSubscriptionValidity, GetExpiringSubscriptions, ReserveCount, DeductCount, ReleaseReservedCount
- [ ] T090 [US4] Implement SubscriptionService in `PetSalon.Backend/PetSalon.Services/Implementations/SubscriptionService.cs` with full CRUD, validation (StartDate < EndDate, no overlapping subscriptions for same pet, cannot delete if UsedCount or ReservedCount > 0), count management logic, validity check (CurrentDate BETWEEN StartDate AND EndDate AND RemainingCount > 0), and expiring subscription detection (EndDate within 7 days)
- [ ] T091 [US4] Create SubscriptionController in `PetSalon.Backend/PetSalon.Web/Controllers/SubscriptionController.cs` with endpoints: GET /api/subscription, GET /api/subscription/{id}, GET /api/subscription/pet/{petId}, GET /api/subscription/expiring, POST /api/subscription, PUT /api/subscription/{id}, DELETE /api/subscription/{id}
- [ ] T092 Register SubscriptionService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T093 [P] [US4] Create subscription API service in `PetSalon.Frontend/src/api/subscriptionApi.ts` with typed methods for all subscription endpoints
- [ ] T094 [P] [US4] Create Pinia subscription store in `PetSalon.Frontend/src/stores/subscription.ts` for subscription list state, expiring subscriptions, and caching
- [ ] T095 [US4] Create SubscriptionList view in `PetSalon.Frontend/src/views/subscription/SubscriptionList.vue` with PrimeVue DataTable, filter by pet/status, show usage progress bars (UsedCount/TotalUsageLimit), highlight expiring subscriptions, add subscription button
- [ ] T096 [US4] Create SubscriptionDetail view in `PetSalon.Frontend/src/views/subscription/SubscriptionDetail.vue` displaying pet info, subscription type, date range, usage counts with visual progress, total amount, renewal button
- [ ] T097 [US4] Create SubscriptionForm component in `PetSalon.Frontend/src/components/subscription/SubscriptionForm.vue` with fields: pet selector, subscription type dropdown (populate from SubscriptionType table), start date (DatePicker), end date (auto-calculated as start+1 month, editable), total usage limit, total amount
- [ ] T098 [US4] Create SubscriptionUsageCard component in `PetSalon.Frontend/src/components/subscription/SubscriptionUsageCard.vue` showing UsedCount, ReservedCount, RemainingCount with color-coded progress bars and icons
- [ ] T099 [US4] Implement mobile swipe actions for subscription cards using PrimeVue gesture directives: swipe left to reveal quick actions (Renew, View Usage, Cancel), swipe right to close
- [ ] T100 [US4] Add subscription routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /subscriptions (list), /subscriptions/:id (detail), /subscriptions/new (create), /subscriptions/:id/edit (edit)
- [ ] T101 [US4] Integrate subscription info in PetDetail view: Show active subscriptions with usage, add "Add Subscription" button
- [ ] T102 [US4] Implement mobile-responsive CSS for subscription views with swipe-friendly cards, clear usage visualization, and touch-friendly renewal buttons
- [ ] T103 [US4] Add client-side validation: required pet, start date cannot be >1 year in past, end date > start date, total usage limit > 0, total amount > 0, check for overlapping subscriptions
- [ ] T104 [US4] Add server-side validation in SubscriptionService: validate no overlapping subscriptions for same pet (check date range overlap), validate counts (UsedCount ≥0, ReservedCount ≥0, UsedCount+ReservedCount ≤ TotalUsageLimit), prevent deletion if counts > 0
- [ ] T105 [US4] Implement scheduled task or API endpoint to flag expiring subscriptions (EndDate within 7 days) for renewal reminders
- [ ] T106 [US4] Implement error handling for subscription creation conflicts (overlapping dates) with clear message showing existing subscription details

**Checkpoint**: User Story 4 complete - Staff can manage subscriptions, track usage, receive renewal reminders, and manage on mobile independently

---

## Phase 7: User Story 5 - Payment & Financial Tracking (Priority: P3)

**Goal**: Enable business owners to track all income and expenses, auto-create payment records when appointments are completed, categorize transactions, generate monthly financial reports with category breakdowns, and access financial dashboard on tablet with responsive charts

**Independent Test**: Mark an appointment as "Completed" and verify system auto-creates a payment record with income type "Grooming". Manually record an expense for "Supplies" with amount 5000. View monthly financial report and verify it shows total income, total expenses, net profit with category breakdowns. View dashboard on tablet and verify charts scale with pinch-to-zoom gestures. Export financial data as CSV.

### Implementation for User Story 5

- [ ] T107 [P] [US5] Create PaymentRecord DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/PaymentDto.cs`: CreatePaymentRequest, UpdatePaymentRequest, PaymentResponse, PaymentListItemResponse, FinancialReportResponse, CategorySummaryResponse
- [ ] T108 [P] [US5] Create TypeScript interfaces in `PetSalon.Frontend/src/types/payment.ts`: Payment, CreatePaymentRequest, UpdatePaymentRequest, PaymentListItem, FinancialReport, CategorySummary, PaymentType
- [ ] T109 [P] [US5] Create IPaymentService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IPaymentService.cs` with methods: GetAllPayments, GetPaymentById, GetPaymentsByDateRange, CreatePayment, UpdatePayment, DeletePayment, AutoCreatePaymentFromReservation, GenerateFinancialReport, ExportFinancialData
- [ ] T110 [US5] Implement PaymentService in `PetSalon.Backend/PetSalon.Services/Implementations/PaymentService.cs` with full CRUD, validation (Amount != 0, Income Amount > 0, Expense Amount < 0, PaymentCategory exists in SystemCode), auto-creation logic (triggered when ReserveRecord status changes to COMPLETED, create PaymentRecord with PaymentType=Income, PaymentCategory=Grooming, Amount=TotalAmount), financial report aggregation by date range and category
- [ ] T111 [US5] Create PaymentController in `PetSalon.Backend/PetSalon.Web/Controllers/PaymentController.cs` with endpoints: GET /api/payment, GET /api/payment/{id}, GET /api/payment/report?start={date}&end={date}, GET /api/payment/export?start={date}&end={date}&format={csv|excel}, POST /api/payment, PUT /api/payment/{id}, DELETE /api/payment/{id}
- [ ] T112 Register PaymentService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T113 [P] [US5] Implement CSV/Excel export functionality in PaymentService using EPPlus or ClosedXML library for Excel, CsvHelper for CSV
- [ ] T114 [P] [US5] Create payment API service in `PetSalon.Frontend/src/api/paymentApi.ts` with typed methods for all payment endpoints including report generation and data export
- [ ] T115 [P] [US5] Create Pinia payment store in `PetSalon.Frontend/src/stores/payment.ts` for payment list state, financial report data, and caching
- [ ] T116 [US5] Install chart library in `PetSalon.Frontend/`: Install Chart.js or use PrimeVue Chart component for financial visualizations
- [ ] T117 [US5] Create PaymentList view in `PetSalon.Frontend/src/views/payment/PaymentList.vue` with PrimeVue DataTable, filter by type/category/date, color-code income (green) vs expense (red), add payment button
- [ ] T118 [US5] Create FinancialDashboard view in `PetSalon.Frontend/src/views/payment/FinancialDashboard.vue` with date range selector, summary cards (total income, total expenses, net profit), category breakdown pie charts, trend line charts, and export button
- [ ] T119 [US5] Create PaymentForm component in `PetSalon.Frontend/src/components/payment/PaymentForm.vue` with fields: amount (required, number), payment type (Income/Expense radio), payment category (SystemCode dropdown filtered by type: IncomeType or ExpenseType), payment date (DatePicker, cannot be future), reservation link (optional, searchable dropdown), memo
- [ ] T120 [US5] Create FinancialReportCard component in `PetSalon.Frontend/src/components/payment/FinancialReportCard.vue` displaying summary metrics with visual indicators (icons, colors) and responsive layout
- [ ] T121 [US5] Implement responsive chart scaling with touch gestures (pinch-to-zoom) using Chart.js zoom plugin or PrimeVue Chart touch events
- [ ] T122 [US5] Add payment routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /payments (list), /payments/dashboard (dashboard), /payments/new (create), /payments/:id/edit (edit)
- [ ] T123 [US5] Integrate payment auto-creation in ReservationService: When UpdateStatus changes to COMPLETED, call PaymentService.AutoCreatePaymentFromReservation with reservation details
- [ ] T124 [US5] Implement mobile-responsive CSS for financial dashboard with stacked layout on mobile, responsive charts, and clear metric cards
- [ ] T125 [US5] Implement tablet-responsive CSS for financial dashboard with optimized grid layout, larger touch targets for date range selector, and gesture-friendly charts
- [ ] T126 [US5] Add client-side validation: required amount, amount != 0, required payment type, required category, payment date cannot be future, validate category matches type (IncomeType for Income, ExpenseType for Expense)
- [ ] T127 [US5] Add server-side validation in PaymentService: validate Amount sign matches PaymentType, validate PaymentCategory exists in SystemCode with correct CodeType, validate PaymentDate ≤ CurrentDate
- [ ] T128 [US5] Implement data export with proper filename (FinancialReport_YYYYMMDD_YYYYMMDD.csv), include all columns (Date, Type, Category, Amount, Memo), handle large datasets (streaming for >10,000 records)
- [ ] T129 [US5] Implement error handling for export failures (disk space, file permissions) with user-friendly messages and retry option

**Checkpoint**: User Story 5 complete - Business owners can track finances, view reports, export data, and access dashboard on tablet independently

---

## Phase 8: User Story 6 - System Administration & Reporting (Priority: P3)

**Goal**: Enable system administrators to manage system codes (breeds, genders, service types, statuses), configure user accounts and roles, generate analytics reports on reservations/subscriptions/revenue trends, access audit logs, and perform admin tasks on mobile

**Independent Test**: As admin user, add a new breed "Corgi" to SystemCode and verify it appears in pet registration dropdowns. Assign "Admin" role to a user and verify they gain access to system administration features. Generate quarterly report and verify it shows trends in appointments, popular services, and revenue growth. View admin panel on desktop and access audit logs showing all data modifications with user and timestamp. Edit a SystemCode on mobile and verify form adapts with proper input controls.

### Implementation for User Story 6

- [ ] T130 [P] [US6] Create User and Role DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/UserDto.cs`: CreateUserRequest, UpdateUserRequest, UserResponse, AssignRoleRequest
- [ ] T131 [P] [US6] Create analytics DTOs in `PetSalon.Backend/PetSalon.Models/DTOs/AnalyticsDto.cs`: ReservationTrendResponse, PopularServiceResponse, RevenueTrendResponse, QuarterlyReportResponse
- [ ] T132 [P] [US6] Create TypeScript interfaces in `PetSalon.Frontend/src/types/admin.ts`: User, Role, AnalyticsReport, ReservationTrend, ServicePopularity, RevenueTrend, AuditLog
- [ ] T133 [P] [US6] Create IUserService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IUserService.cs` with methods: GetAllUsers, GetUserById, CreateUser, UpdateUser, DeleteUser, AssignRole, RevokeRole
- [ ] T134 [P] [US6] Create IAnalyticsService interface in `PetSalon.Backend/PetSalon.Services/Interfaces/IAnalyticsService.cs` with methods: GetReservationTrends, GetPopularServices, GetRevenueTrends, GenerateQuarterlyReport, GetAuditLogs
- [ ] T135 [US6] Implement UserService in `PetSalon.Backend/PetSalon.Services/Implementations/UserService.cs` with user management, role assignment (Admin, User roles), password hashing (bcrypt with cost factor 12), and validation
- [ ] T136 [US6] Implement AnalyticsService in `PetSalon.Backend/PetSalon.Services/Implementations/AnalyticsService.cs` with aggregation queries for reservation trends (count by month), popular services (count by service type), revenue trends (sum by month), quarterly report compilation, and audit log retrieval
- [ ] T137 [US6] Extend CommonService in `PetSalon.Backend/PetSalon.Services/Implementations/CommonService.cs` to support full CRUD on SystemCode (create, update, delete with validation)
- [ ] T138 [US6] Create UserController in `PetSalon.Backend/PetSalon.Web/Controllers/UserController.cs` with endpoints: GET /api/user, GET /api/user/{id}, POST /api/user, PUT /api/user/{id}, DELETE /api/user/{id}, POST /api/user/{id}/role
- [ ] T139 [US6] Create AnalyticsController in `PetSalon.Backend/PetSalon.Web/Controllers/AnalyticsController.cs` with endpoints: GET /api/analytics/reservations, GET /api/analytics/services, GET /api/analytics/revenue, GET /api/analytics/quarterly, GET /api/analytics/auditlogs
- [ ] T140 [US6] Extend CommonController in `PetSalon.Backend/PetSalon.Web/Controllers/CommonController.cs` with role-based authorization [Authorize(Roles = "Admin")] for create/update/delete SystemCode endpoints
- [ ] T141 Register UserService and AnalyticsService in DI container in `PetSalon.Backend/PetSalon.Web/Program.cs`
- [ ] T142 [P] [US6] Create user API service in `PetSalon.Frontend/src/api/userApi.ts` with typed methods for user management and role assignment
- [ ] T143 [P] [US6] Create analytics API service in `PetSalon.Frontend/src/api/analyticsApi.ts` with typed methods for all analytics endpoints
- [ ] T144 [P] [US6] Create Pinia admin store in `PetSalon.Frontend/src/stores/admin.ts` for users, roles, analytics data, and caching
- [ ] T145 [US6] Create AdminPanel view in `PetSalon.Frontend/src/views/admin/AdminPanel.vue` with tabs: System Codes, Users, Analytics, Audit Logs, protected by [Admin role check in router]
- [ ] T146 [US6] Create SystemCodeManagement component in `PetSalon.Frontend/src/components/admin/SystemCodeManagement.vue` with DataTable for each CodeType (Breed, Gender, ServiceType, etc.), CRUD operations, mobile-responsive forms
- [ ] T147 [US6] Create UserManagement component in `PetSalon.Frontend/src/components/admin/UserManagement.vue` with DataTable, create user form, role assignment dropdown (Admin, User), password reset functionality
- [ ] T148 [US6] Create AnalyticsDashboard component in `PetSalon.Frontend/src/components/admin/AnalyticsDashboard.vue` with date range selector, reservation trend chart, popular services bar chart, revenue trend line chart, quarterly summary cards
- [ ] T149 [US6] Create AuditLogViewer component in `PetSalon.Frontend/src/components/admin/AuditLogViewer.vue` with DataTable showing entity type, action (Create/Update/Delete), user, timestamp, before/after values, filterable by entity/user/date
- [ ] T150 [US6] Add admin routes to Vue Router in `PetSalon.Frontend/src/router/index.ts`: /admin (panel), /admin/systemcodes (codes), /admin/users (users), /admin/analytics (analytics), /admin/auditlogs (logs), with navigation guard checking for Admin role
- [ ] T151 [US6] Implement role-based UI rendering: Hide admin menu items for non-admin users, show "Access Denied" for unauthorized access attempts
- [ ] T152 [US6] Implement mobile-responsive CSS for admin panel with tabbed interface, collapsible sections, touch-friendly forms, and proper input controls for SystemCode editing on mobile
- [ ] T153 [US6] Implement quarterly report generation logic: Aggregate data for 3-month periods, calculate appointment growth percentage, identify top 5 popular services, calculate total revenue and growth percentage, format as downloadable PDF or detailed HTML report
- [ ] T154 [US6] Add client-side validation for SystemCode form: required CodeType, required Code, required Name, validate Code uniqueness within CodeType, validate Sort is number
- [ ] T155 [US6] Add server-side validation in CommonService: prevent deleting SystemCode if referenced by other tables (check Pet.Breed, Pet.Gender, ReserveRecord.Status, etc.), enforce unique (CodeType, Code), validate StartDate < EndDate if both provided
- [ ] T156 [US6] Implement error handling for SystemCode deletion blocked by references with clear message showing which entities are referencing it (e.g., "Cannot delete breed 'Poodle' because 15 pets are using it")
- [ ] T157 [US6] Implement audit log pagination and filtering: Support pagination for large audit log datasets (>10,000 records), filter by entity type, user, date range, action type

**Checkpoint**: User Story 6 complete - Administrators can manage system configuration, users, view analytics, and access audit logs independently

---

## Phase 9: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T158 [P] Create comprehensive API documentation in Swagger: Add XML comments to all controllers, document request/response models, add example values, document error codes
- [ ] T159 [P] Implement global error handling in frontend: Create error boundary components, user-friendly error messages, retry mechanisms, offline detection
- [ ] T160 [P] Implement logging throughout backend: Use ILogger for all service methods, log CRUD operations with entity IDs, log authentication attempts, log exceptions with stack traces
- [ ] T161 [P] Performance optimization: Enable response compression in `Program.cs`, implement output caching for GET endpoints (SystemCodes, Services), optimize EF queries with AsNoTracking for read-only operations
- [ ] T162 [P] Security hardening: Add rate limiting to authentication endpoints (10 requests/minute), implement CSRF protection for state-changing operations, add input sanitization for memo/text fields, enforce HTTPS in production
- [ ] T163 [P] Accessibility improvements: Add ARIA labels to all interactive elements, ensure keyboard navigation works for all forms, test with screen reader, verify color contrast ratios meet WCAG 2.0 AA standards
- [ ] T164 [P] Create user guide documentation in `docs/user-guide.md`: Document how to register pets, book appointments, manage subscriptions, view financial reports, with screenshots
- [ ] T165 [P] Create developer documentation in `docs/developer-guide.md`: Document project structure, how to add new entities, how to regenerate EF models, API conventions, testing procedures
- [ ] T166 Code cleanup: Remove unused imports, add missing TypeScript types, ensure consistent code style (run Prettier on frontend, dotnet format on backend), remove debug console.log statements
- [ ] T167 Implement offline data caching in frontend: Use IndexedDB via localForage to cache pet list, contact list, SystemCodes for offline viewing, sync when connection restored
- [ ] T168 Implement push notification setup (optional): Add service worker for web push notifications, implement server-side notification service for appointment reminders and subscription renewals
- [ ] T169 Run quickstart.md validation: Follow setup instructions from scratch on clean machine, verify all steps work, update any outdated commands or configurations
- [ ] T170 [P] Performance testing: Load test API with 50 concurrent users using k6 or JMeter, verify response times <2s, test calendar rendering with 100+ appointments, verify mobile page load <3s on throttled 3G connection
- [ ] T171 [P] Security audit: Run OWASP ZAP scan on API endpoints, test SQL injection resistance, verify JWT token expiration enforcement, test CORS configuration with unauthorized origins
- [ ] T172 [P] Mobile device testing: Test on real iOS Safari and Android Chrome devices, verify touch targets ≥44px, test responsive breakpoints, verify phone number clickable, test bottom sheets and swipe gestures
- [ ] T173 Create deployment scripts: Docker compose for SQL Server and backend, deployment checklist for production, environment variable configuration guide, backup/restore procedures
- [ ] T174 Final integration testing: Complete end-to-end user journeys for all 6 user stories, verify all checkpoints pass, test cross-story integrations (pet→contact→reservation→subscription→payment flow)

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Story 1 (Phase 3)**: Depends on Foundational (Phase 2) completion
- **User Story 2 (Phase 4)**: Depends on Foundational (Phase 2) completion, integrates with US1 (pet relationships)
- **User Story 3 (Phase 5)**: Depends on Foundational (Phase 2) and US1 (Pet) and US2 (ContactPerson) completion
- **User Story 4 (Phase 6)**: Depends on Foundational (Phase 2) and US1 (Pet) completion, integrates with US3 (Reservation)
- **User Story 5 (Phase 7)**: Depends on Foundational (Phase 2) and US3 (Reservation) completion
- **User Story 6 (Phase 8)**: Depends on Foundational (Phase 2) completion, independent from other stories
- **Polish (Phase 9)**: Depends on desired user stories being complete

### User Story Dependencies

- **User Story 1 (Pet Management)**: No dependencies on other stories - Can start immediately after Foundational
- **User Story 2 (Contact Management)**: No strict dependencies, but integrates with US1 (linking contacts to pets)
- **User Story 3 (Appointments)**: Requires US1 (Pet) and US2 (ContactPerson) for booking appointments
- **User Story 4 (Subscriptions)**: Requires US1 (Pet) and integrates with US3 (Reservation) for usage tracking
- **User Story 5 (Payments)**: Requires US3 (Reservation) for auto-payment creation
- **User Story 6 (Admin)**: Independent - only needs Foundational layer

### Recommended Execution Order

**MVP First (User Stories 1 & 2 Only)**:
1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL)
3. Complete Phase 3: User Story 1 (Pet Management)
4. Complete Phase 4: User Story 2 (Contact Management)
5. **STOP and VALIDATE**: Test US1 and US2 independently
6. Deploy MVP with basic pet and contact management

**Incremental Delivery**:
1. MVP (US1 + US2) → Validate → Deploy
2. Add Phase 5: User Story 3 (Appointments) → Validate → Deploy
3. Add Phase 6: User Story 4 (Subscriptions) → Validate → Deploy
4. Add Phase 7: User Story 5 (Payments) → Validate → Deploy
5. Add Phase 8: User Story 6 (Admin) → Validate → Deploy
6. Complete Phase 9: Polish → Final Deploy

**Parallel Team Strategy**:

After Foundational (Phase 2) completion:
- **Team A**: User Story 1 (Pet) + User Story 2 (Contact) [These are tightly integrated]
- **Team B**: User Story 6 (Admin) [Independent, can start immediately]
- After Team A completes US1+US2:
  - **Team A**: User Story 3 (Appointments)
  - **Team C**: User Story 4 (Subscriptions)
- After Team A completes US3:
  - **Team A**: User Story 5 (Payments)

### Parallel Opportunities

Within each phase, tasks marked [P] can run in parallel:

**Phase 1 (Setup)**:
- T003, T004, T005, T006, T007, T010 can all run in parallel

**Phase 2 (Foundational)**:
- After T013 (EF Core regeneration), T014-T031 can run in parallel groups:
  - Backend: T014, T015, T016, T024, T026
  - Frontend: T020, T021, T022, T023, T028, T029, T030

**User Story Phases**:
- DTOs and TypeScript interfaces can be created in parallel
- Service interfaces and API services can be created in parallel
- View components can be developed in parallel after services are complete

---

## Implementation Strategy

### MVP First (User Stories 1 & 2)

**Rationale**: Pet and Contact management form the foundation. Without pet data, no other features can function. Contacts enable communication and are essential for appointment management.

**Deliverables**:
1. Staff can register and manage pets with photos
2. Staff can manage contacts and link them to pets with relationships
3. Search and filter capabilities for both pets and contacts
4. Mobile-responsive interface for on-floor operations

**Time Estimate**: 2-3 weeks for MVP (US1 + US2)

### Incremental Feature Delivery

After MVP validation, add features in priority order:

**Phase 5 (US3 - Appointments)**: Enables core revenue-generating activity
- Time Estimate: 2 weeks
- Value: Staff can book and manage grooming appointments

**Phase 6 (US4 - Subscriptions)**: Enables recurring revenue model
- Time Estimate: 1.5 weeks
- Value: Customer retention through monthly packages

**Phase 7 (US5 - Payments)**: Enables financial tracking
- Time Estimate: 1.5 weeks
- Value: Business insights and decision-making

**Phase 8 (US6 - Admin)**: Enables system configuration
- Time Estimate: 1 week
- Value: Long-term maintainability and analytics

**Phase 9 (Polish)**: Production-ready refinements
- Time Estimate: 1 week
- Value: Security, performance, documentation

**Total Estimate**: 10-12 weeks for full implementation

---

## Parallel Example: User Story 1

```bash
# Launch all DTOs and interfaces in parallel:
Task T032: "Create Pet DTOs in PetSalon.Backend/PetSalon.Models/DTOs/PetDto.cs"
Task T033: "Create TypeScript interfaces in PetSalon.Frontend/src/types/pet.ts"
Task T034: "Create IPetService interface in PetSalon.Backend/PetSalon.Services/Interfaces/IPetService.cs"

# After T035 (PetService implementation), launch frontend components in parallel:
Task T038: "Create pet API service in PetSalon.Frontend/src/api/petApi.ts"
Task T039: "Create Pinia pet store in PetSalon.Frontend/src/stores/pet.ts"

# After API integration, launch views in parallel:
Task T040: "Create PetList view"
Task T041: "Create PetDetail view"
Task T042: "Create PetForm component"
```

---

## Notes

- **[P] tasks** = different files, no dependencies, can be parallelized
- **[Story] label** maps task to specific user story for traceability
- Each user story should be independently completable and testable
- Commit after each task or logical group of tasks
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
- All file paths are exact and implementation-ready
- Task numbering (T001-T174) provides clear execution order
- Backend uses 4-layer architecture: Web (Controllers) → Services → Models (Entities/DTOs) → Tools (Utilities)
- Frontend follows Vue 3 Composition API with TypeScript strict mode
- Mobile-first responsive design with 44px minimum touch targets
- Database-first approach: SQL scripts → EF Core regeneration → never manually edit entities

---

## Success Criteria Validation

After completing all tasks, verify against spec.md success criteria:

- **SC-001**: Staff can register a new pet in under 3 minutes ✓ (US1)
- **SC-002**: Staff can book an appointment in under 2 minutes ✓ (US3)
- **SC-003**: 90% of appointments use subscriptions when available ✓ (US4 integration with US3)
- **SC-004**: System handles 100 concurrent users ✓ (T170 performance testing)
- **SC-005**: Mobile interface usability score 4.0/5.0+ ✓ (T172 mobile testing)
- **SC-006**: Reduce phone call workload by 30% ✓ (US2 + US3 digitized appointments)
- **SC-007**: Financial reports generated within 5 seconds ✓ (US5 with query optimization)
- **SC-008**: 95% of user actions succeed on first attempt ✓ (comprehensive validation + error handling throughout)
- **SC-009**: Zero data loss incidents ✓ (T173 backup/restore procedures)
- **SC-010**: Lighthouse score 80+ on mobile 3G ✓ (T170 performance testing)
