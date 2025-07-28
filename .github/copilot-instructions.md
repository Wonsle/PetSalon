# AI Rules for {{project-name}}

{{project-description}}

## FRONTEND

### Guidelines for VUE

#### VUE_CODING_STANDARDS

- Use the Composition API instead of the Options API for better type inference and code reuse
- Implement <script setup> for more concise component definitions
- Use Suspense and async components for handling loading states during code-splitting
- Leverage the defineProps and defineEmits macros for type-safe props and events
- Use the new defineOptions for additional component options
- Implement provide/inject for dependency injection instead of prop drilling in deeply nested components
- Use the Teleport component for portal-like functionality to render UI elsewhere in the DOM
- Leverage ref over reactive for primitive values to avoid unintended unwrapping
- Use v-memo for performance optimization in render-heavy list rendering scenarios
- Implement shallow refs for large objects that don't need deep reactivity

#### NUXT

- Use Nuxt 3 with the Composition API and <script setup> for modern applications
- Leverage auto-imports for Vue and Nuxt composables to reduce boilerplate
- Implement server routes with the server directory for API functionality
- Use Nuxt modules for extending functionality instead of custom plugins when possible
- Leverage the useAsyncData and useFetch composables for data fetching with SSR support
- Implement middleware (defineNuxtRouteMiddleware) for navigation guards
- Use Nuxt layouts for consistent page layouts across routes
- Leverage Nitro for server-side rendering and API routes
- Implement Nuxt plugins for global functionality registration
- Use state management with useState for simple state or Pinia for complex applications

#### VUEX

- Migrate to Pinia instead of Vuex for Vue 3 projects as it provides better TypeScript support
- If using Vuex, implement modules pattern to organize related state, getters, mutations, and actions
- Use namespaced modules to avoid naming conflicts in larger applications
- Leverage plugins for cross-cutting concerns like persistence or analytics
- Avoid direct state mutations outside of mutations to maintain predictable state changes
- Use mapState, mapGetters, and mapActions helpers to simplify component code
- Implement Vuex with the composition API using useStore for better TypeScript support
- Use actions for async operations and mutations for synchronous state changes
- Leverage getters for computed state to avoid redundant calculations
- Implement proper error handling in actions with try/catch blocks

#### VUE_ROUTER

- Use route guards (beforeEach, beforeEnter) for authentication and authorization checks
- Implement lazy loading with dynamic imports for route components to improve performance
- Use named routes instead of hardcoded paths for better maintainability
- Leverage route meta fields to store additional route information like permissions or layout data
- Implement scroll behavior options to control scrolling between route navigations
- Use navigation duplicates handling to prevent redundant navigation to the current route
- Implement the composition API useRouter and useRoute hooks instead of this.$router
- Use nested routes for complex UIs with parent-child relationships
- Leverage route params validation with sensitive: true for parameters that shouldn't be logged
- Implement dynamic route matching with path parameters and regex patterns for flexible routing

#### PINIA

- Create multiple stores based on logical domains instead of a single large store
- Use the setup syntax (defineStore with setup function) for defining stores for better TypeScript inference
- Implement getters for derived state to avoid redundant computations
- Leverage the storeToRefs helper to extract reactive properties while maintaining reactivity
- Use plugins for cross-cutting concerns like persistence, state resets, or dev tools
- Implement actions for asynchronous operations and complex state mutations
- Use composable stores by importing and using stores within other stores
- Leverage the $reset() method to restore initial state when needed
- Implement $subscribe for reactive store subscriptions
- Use TypeScript with proper return type annotations for maximum type safety


## BACKEND

### Guidelines for DOTNET

#### ENTITY_FRAMEWORK

- Use the repository and unit of work patterns to abstract data access logic and simplify testing
- Implement eager loading with Include() to avoid N+1 query problems for {{entity_relationships}}
- Use migrations for database schema changes and version control with proper naming conventions
- Apply appropriate tracking behavior (AsNoTracking() for read-only queries) to optimize performance
- Implement query optimization techniques like compiled queries for frequently executed database operations
- Use value conversions for complex property transformations and proper handling of {{custom_data_types}}

#### ASP_NET

- Use minimal APIs for simple endpoints in .NET 6+ applications to reduce boilerplate code
- Implement the mediator pattern with MediatR for decoupling request handling and simplifying cross-cutting concerns
- Use API controllers with model binding and validation attributes for {{complex_data_models}}
- Apply proper response caching with cache profiles and ETags for improved performance on {{high_traffic_endpoints}}
- Implement proper exception handling with ExceptionFilter or middleware to provide consistent error responses
- Use dependency injection with scoped lifetime for request-specific services and singleton for stateless services


## DATABASE

### Guidelines for SQL

#### SQLSERVER

- Use parameterized queries to prevent SQL injection
- Implement proper indexing strategies based on query patterns
- Use stored procedures for complex business logic that requires database access to {{business_entities}}


## CODING_PRACTICES

### Guidelines for VERSION_CONTROL

#### GIT

- Use conventional commits to create meaningful commit messages
- Use feature branches with descriptive names following {{branch_naming_convention}}
- Write meaningful commit messages that explain why changes were made, not just what
- Keep commits focused on single logical changes to facilitate code review and bisection
- Use interactive rebase to clean up history before merging feature branches
- Leverage git hooks to enforce code quality checks before commits and pushes


