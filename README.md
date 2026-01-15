# Toyana - Enterprise Wedding SaaS Platform

Toyana is a scalable, enterprise-grade Wedding SaaS platform built with modern .NET technologies and a microservices architecture. It aims to streamline the wedding planning process for vendors, clients, and administrators.

## 🚀 Purpose & Capabilities

Toyana provides a comprehensive suite of services to manage the wedding lifecycle:

- **Identity Service**: Secure authentication and Authorization (RBAC), managing Users, Vendors, and Administrators.
- **Vendor Center**: The command side for vendor management, allowing vendors to manage their profiles, services, and availability.
- **Catalog Service**: Optimized for search and discovery, using real-time event synchronization to provide a fast catalog of wedding vendors.
- **Ordering Service**: Manages the booking process, from initial inquiry to confirmed reservations and scheduling.
- **Payment Service**: Handles secure financial transactions and payment processing.
- **Chat Service**: Real-time communication gateway between vendors and clients.
- **Alerts Service**: Automated notifications and integrations (e.g., Telegram bots) to keep stakeholders informed.
- **Admin Portal**: Centralized management for platform administrators to oversee the entire ecosystem.

## 🛠 Tech Stack

### Backend
- **Framework**: .NET 10
- **Architecture**: Microservices, Domain-Driven Design (DDD), CQRS, Clean Architecture.
- **Messaging & Sagas**: [Wolverine](https://wolverine.netlify.app/) for out-of-process messaging and robust saga management.
- **Data Storage**: 
  - **PostgreSQL**: Primary relational storage.
  - **Marten**: Used as an Event Store and Document Database for high-performance reading and event sourcing.
  - **Redis/Valkey**: Distributed caching and session management.
- **API Gateway**: [YARP (Yet Another Reverse Proxy)](https://microsoft.github.io/reverse-proxy/) for centralized routing, authentication, and observability.

### Frontend
- **Framework**: [SvelteKit](https://kit.svelte.dev/) with TypeScript.
- **Styling**: Tailwind CSS for a premium, responsive UI.
- **Tooling**: Vite for fast development and building.

### Infrastructure & Observability
- **Containerization**: Docker & Docker Compose.
- **Message Broker**: RabbitMQ.
- **Observability Stack**: Full OpenTelemetry integration:
  - **Grafana**: For dashboards and visualization.
  - **Tempo**: For distributed tracing.
  - **Loki**: For log aggregation.
  - **Prometheus**: For metrics collection.
  - **OTel Collector**: Centralized telemetry processing.

## 🏗 Project Structure

```text
├── apps/               # Frontend applications (Admin, Client, Vendor)
├── src/                # Backend Microservices
├── shared/             # Shared libraries (Contracts, Shared Logic)
├── infrastructure/     # Docker, Observability, and DB configurations
├── tests/              # End-to-End and Unit tests
└── Toyana.sln          # Main Solution file
```

## 🔮 Future Goals

- **Mobile Applications**: Native iOS and Android apps for on-the-go wedding management.
- **AI Planning Assistant**: Leveraging LLMs to help clients plan their wedding itinerary and budget.
- **Guest Management**: RSVP tracking, seating arrangements, and dietary requirement management.
- **Vendor Integrations**: Connecting with external calendars (Google/Outlook) and payment providers.
- **Multi-tenant Scaling**: Advanced sharding and scaling strategies for global expansion.

## 🛠 Getting Started

### Prerequisites
- Docker & Docker Compose
- .NET 10 SDK
- Node.js & npm (for frontends)

### Running the Project
1. Clone the repository.
2. Run the infrastructure:
   ```bash
   docker compose up -d
   ```
3. (Optional) Run specific microservices locally or via Docker.
4. Run frontend apps:
   ```bash
   cd apps/client-web && npm install && npm run dev
   ```

### Running Tests
Execute the E2E test suite:
```bash
dotnet test tests/Toyana.E2E.Tests/Toyana.E2E.Tests.csproj
```

