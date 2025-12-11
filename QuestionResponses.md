# Technical Question Responses

## 1. Differences Between IaaS, PaaS, and SaaS

> **Key Terms:**
> - **Provider** - The cloud service vendor (e.g., Microsoft Azure, AWS, Google Cloud) who owns and operates the physical data centers, servers, networking equipment, and infrastructure.
> - **Customer** - Your organization or team that purchases cloud services and builds/deploys applications on the provider's platform.

### Infrastructure as a Service (IaaS)
IaaS provides a cloud platform that hosts a variety of virtualized infrastructure concerns. Customers are responsible for choosing the correct versions of concerns and manage the interoperability between them. Maintenance of these resources are the responsibility of the customer including software versions, operating system versions, security updates and configuration.

The provider supplies the physical infrastructure as a platform upon which the customer can build any combination of virtualized infrastructure.

**Examples:**
- Azure Virtual Machines
- Azure Virtual Network
- Azure Storage

### Platform as a Service (PaaS)
PaaS provides a higher level of abstraction that allows customers to focus on application development requirements while the provider maintains a stable, secure updated virtual environment for the customer.

**Examples:**
- Azure SQL Server
- Azure Cosmos DB
- Azure Container Apps
- Azure Kubernetes Service
- Azure Functions

### Software as a Service (SaaS)
SaaS delivers fully managed applications over the internet. The provider manages everything from infrastructure to application code and data.  The customer uses these services as the provider delivers them with little or no custom modifications outside of standard configuration.

**Examples:**
- Dropbox
- Azure DevOps
- ServiceNow

## 2. Build vs. Buy Decision Considerations

**Consider building if:**
- It will be less expensive than buying software and end user licenses
- The software must be highly customizable
- The functionality is tightly coupled to specific business needs
- Custom integrations with existing systems must be tightly controlled
- The software will be reused within the organization and needs to be updated in a single location
- The enterprise has technical staff capable of building and maintaining the required quality and operational efficiency


## 3. Serverless Architecture: Foundational Elements and Considerations


### Foundational Elements

- Pay-per-Execution
- Memory & CPU
- Cost Optimization
- Automatic Scaling
- Cold Start Latency
- Duration Limits
- Event-Driven
- Stateless Design
- Azure Functions
- HTTP triggers, queue/event triggers
- Orchestration for workflows
- Centralized API gateway
- Centralized Throttling
- Centralized Authentication


---

## 4. Composition Over Inheritance

### Core Concept
"Composition over inheritance" is a design principle that favors building objects by combining smaller, focused components rather than creating deep inheritance hierarchies. It promotes flexibility, reusability, and maintainability.

### Problems with Inheritance
1. **Tight Coupling** - Child classes are tightly bound to parent implementation
2. **Fragile Base Class Problem** - Changes to base class can break derived classes
3. **Inflexibility** - Difficult to change behavior at runtime
4. **Deep Hierarchies** - Complex inheritance trees become hard to understand
5. **Multiple Inheritance Challenges** - Diamond problem, method resolution complexity

### Advantages of Composition
1. **Loose Coupling** - Components are independent and interchangeable
2. **Runtime Flexibility** - Behavior can be changed by swapping components
3. **Better Testability** - Components can be tested in isolation
4. **Easier Maintenance** - Changes are localized to specific components
5. **Reusability** - Components can be reused across different contexts

Consider Composition When:
- Behavior varies independently across objects
- You need runtime flexibility
- Multiple behaviors need to be mixed and matched
- Building complex objects from simple parts

Consider Inheritance When:
- There's a clear "is-a" relationship (e.g., Cat is an Animal)
- Behavior is fundamental to the type and unlikely to change
- Creating a stable, well-understood domain model

---

## 5. Backend for Frontend (BFF) Pattern - Applied Example

### The Pattern
The **Backend for Frontend (BFF)** pattern involves creating separate backend services tailored to the specific needs of different frontend clients (web, mobile, desktop). Each BFF aggregates, transforms, and optimizes data from multiple microservices or APIs to match its client's requirements.
- Over-fetching or under-fetching of data

### Enterprise BFF Architecture with Shared Services
```
┌──────────────┐      ┌──────────────────┐
│   Admin      │      │   Admin BFF      │
│   Portal     │─────►│   (Internal      │──┐
└──────────────┘      │    Tools)        │  │
                      └──────────────────┘  │
                                            │
┌──────────────┐      ┌──────────────────┐ │
│  Consumer    │      │  Consumer BFF    │ │
│     App      │─────►│  (Public Web/    │─┤
└──────────────┘      │   Mobile)        │ │
                      └──────────────────┘ │
                                           │
┌──────────────┐      ┌──────────────────┐ │
│ Commercial   │      │ Commercial BFF   │ │
│   Portal     │─────►│  (B2B Partner    │─┤
└──────────────┘      │   API)           │ │
                      └──────────────────┘ │
                                           │
                      ┌────────────────────┼────────────────────┐
                      │                    │                    │
                      ▼                    ▼                    ▼
              ┌───────────────┐   ┌───────────────┐   ┌───────────────┐
              │   Identity    │   │   Profile     │   │   Payment     │
              │   Service     │   │   Service     │   │   Service     │
              └───────┬───────┘   └───────┬───────┘   └───────┬───────┘
                      │                   │                   │
                      ▼                   ▼                   ▼
              ┌───────────────┐   ┌───────────────┐   ┌───────────────┐
              │     OKTA      │   │      DB       │   │  B2B Payment  │
              │  Auth Provider│   │  Repository   │   │    Service    │
              └───────────────┘   └───────────────┘   └───────────────┘
```

**Shared Services Layer:**
- **Identity Service** - Handles authentication/authorization via OKTA integration
- **Profile Service** - Manages user/customer data from database repository
- **Payment Service** - Processes payments through B2B payment gateway

**Benefits of Shared Services:**
- Single source of truth for identity across all BFFs
- Consistent user profiles regardless of client type
- Centralized payment processing with unified reconciliation
- Reduced code duplication
- Easier compliance and security auditing

**Changes I would Make Base on Experience:**
- Include all BFF services in a monorepo so each could share changes, pipelines, logic and refactoring in one codebase.
- Include templates in the monorepo for anything that repeats.
- Use feature flags for managing service aggregation updates and rollouts.
- Refactor aggressively to reuse shared logic in microservices or library packages (Nuget, etc).
