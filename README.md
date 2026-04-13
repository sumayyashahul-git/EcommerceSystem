# E-Commerce Microservices System

A production-ready E-Commerce platform built with .NET 8 microservices architecture, featuring Clean Architecture, CQRS, event-driven messaging, and both Angular and React frontends.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Services](#services)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [API Gateway](#api-gateway)
- [Infrastructure](#infrastructure)
- [Project Structure](#project-structure)
- [Design Patterns](#design-patterns)
- [Order Flow](#order-flow)
- [Testing](#testing)
- [Future Enhancements](#future-enhancements)

---

## Architecture Overview

```
+-----------------------------------------------------+
|           Angular (4200) / React (3000)             |
+-------------------------+---------------------------+
                          |
+-------------------------+---------------------------+
|           API Gateway - Ocelot (Port 5000)          |
+----+------+------+------+------+------+-------------+
     |      |      |      |      |      |
   5001   5002   5003   5004   5005   5006   5007
Identity Product Basket Order Inventory Payment Notification
  SQL    SQL   Redis  SQL    SQL    SQL    (events only)
                      +------------------------+
                               RabbitMQ
                          (async messaging)
```

---

## Services

| Service | Port | Database | Description |
|---|---|---|---|
| API Gateway | 5000 | - | Ocelot reverse proxy, single entry point |
| Identity Service | 5001 | SQL Server | User registration, login, JWT tokens |
| Product Service | 5002 | SQL Server | Product catalog management |
| Basket Service | 5003 | Redis | Shopping cart with TTL expiry |
| Order Service | 5004 | SQL Server | Order placement and management |
| Inventory Service | 5005 | SQL Server | Stock levels and reservations |
| Payment Service | 5006 | SQL Server | Payment processing |
| Notification Service | 5007 | - | Event-driven notifications |

---

## Tech Stack

### Backend
- **.NET 8** — All microservices
- **ASP.NET Core Web API** — REST endpoints
- **Entity Framework Core 8** — ORM and database migrations
- **MediatR 14** — CQRS implementation
- **MassTransit 8** — Message bus abstraction
- **RabbitMQ 3.13** — Message broker
- **Ocelot 23** — API Gateway and reverse proxy
- **BCrypt.Net** — Password hashing
- **JWT Bearer Authentication** — Stateless authentication

### Storage
- **SQL Server 2022** — Persistent relational data
- **Redis 7** — Ephemeral cart storage with TTL

### Infrastructure
- **Docker and Docker Compose** — Containerized infrastructure
- **RabbitMQ** — Asynchronous inter-service communication
- **Jaeger** — Distributed request tracing
- **Prometheus** — Metrics collection
- **Grafana** — Metrics visualization and dashboards

### Frontend
- **Angular 17** — NgRx state management, Angular Material UI
- **React 18** — Redux Toolkit, Tailwind CSS, React Query

---

## Prerequisites

Ensure the following are installed before running the project:

| Tool | Purpose |
|---|---|
| .NET 8 SDK | Backend service compilation and runtime |
| Docker Desktop (WSL2) | Running infrastructure containers |
| Node.js LTS | Frontend build tools |
| Visual Studio 2022 | IDE with ASP.NET Core workload |
| Angular CLI | Angular frontend tooling |
| Postman | API testing |
| DBeaver (optional) | Database inspection |

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/EcommerceSystem.git
cd EcommerceSystem
```

### 2. Start Infrastructure

```bash
docker compose up -d
```

This command starts the following containers:

| Container | Port | Purpose |
|---|---|---|
| SQL Server | 1433 | All relational databases |
| Redis | 6379 | Basket and cache storage |
| RabbitMQ | 5672 / 15672 | Message broker and management UI |
| Jaeger | 16686 | Distributed tracing UI |
| Prometheus | 9090 | Metrics collection |
| Grafana | 3100 | Metrics dashboard |

### 3. Verify Containers

```bash
docker compose ps
```

All containers should show status **Up**.

### 4. Open Solution

Open `EcommerceSystem.sln` in Visual Studio 2022.

### 5. Configure Startup Projects

```
Right-click Solution -> Properties -> Multiple startup projects
Set all API projects to Start
```

### 6. Run the Solution

Press **F5**. All services will start and databases will be created automatically via EF Core migrations on startup.

---

## API Gateway

All client requests are routed through the API Gateway on port **5000**. Individual service ports are never exposed to clients.

| Upstream Route (Gateway) | Downstream Service |
|---|---|
| /api/identity/{everything} | Identity Service :5001 |
| /api/products/{everything} | Product Service :5002 |
| /api/basket/{everything} | Basket Service :5003 |
| /api/orders/{everything} | Order Service :5004 |
| /api/inventory/{everything} | Inventory Service :5005 |
| /api/payments/{everything} | Payment Service :5006 |
| /api/notification/{everything} | Notification Service :5007 |

### Sample Requests via Gateway

```
GET  http://localhost:5000/api/products
POST http://localhost:5000/api/identity/register
POST http://localhost:5000/api/orders
GET  http://localhost:5000/api/basket/{userId}
```

---

## Infrastructure

### Management Dashboards

| Service | URL | Default Credentials |
|---|---|---|
| RabbitMQ Management | http://localhost:15672 | guest / guest |
| Jaeger Tracing UI | http://localhost:16686 | - |
| Grafana Dashboard | http://localhost:3100 | admin / admin |
| Prometheus | http://localhost:9090 | - |

### Stop Infrastructure

```bash
docker compose down
```

---

## Project Structure

```
EcommerceSystem/
├── src/
│   ├── Gateway/
│   │   └── ApiGateway/
│   │       ├── ocelot.json              Route configuration
│   │       └── Program.cs
│   ├── Services/
│   │   ├── Identity/
│   │   │   ├── Identity.API             Controllers, middleware, DI
│   │   │   ├── Identity.Application     Commands, queries, DTOs
│   │   │   ├── Identity.Domain          Entities, domain events
│   │   │   └── Identity.Infrastructure  EF Core, repositories, JWT
│   │   ├── Product/                     (same structure)
│   │   ├── Basket/                      (same structure)
│   │   ├── Order/                       (same structure)
│   │   ├── Inventory/                   (same structure)
│   │   ├── Payment/                     (same structure)
│   │   └── Notification/                (consumers only, no database)
│   ├── SharedKernel/
│   │   ├── Domain/                      BaseEntity, AggregateRoot
│   │   ├── Events/                      Integration events
│   │   ├── Exceptions/                  Custom exception hierarchy
│   │   ├── Common/                      ApiResponse, PagedResult
│   │   └── Interfaces/                  IRepository generic interface
│   └── Frontends/
│       ├── ecommerce-angular/           Angular 17 frontend
│       └── ecommerce-react/             React 18 frontend
├── docker-compose.yml
├── prometheus.yml
└── EcommerceSystem.sln
```

---

## Design Patterns

### Clean Architecture

Each service follows a strict 4-layer architecture with inward-only dependencies:

```
API -> Application -> Domain
Infrastructure -> Application
```

- **Domain** — Entities, value objects, domain events. No external dependencies.
- **Application** — CQRS commands and queries, business logic, interface contracts.
- **Infrastructure** — EF Core, repositories, external service implementations.
- **API** — Controllers, middleware, dependency injection registration.

### CQRS with MediatR

```csharp
// Command - changes system state
public record PlaceOrderCommand : IRequest<OrderDto>
{
    public Guid UserId { get; init; }
    public List<OrderItemRequest> Items { get; init; } = new();
}

// Query - reads system state without modification
public record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto>;

// Handler - contains business logic
public class PlaceOrderCommandHandler
    : IRequestHandler<PlaceOrderCommand, OrderDto> { }
```

### Event-Driven Architecture

```
Order Service
    publishes OrderPlacedIntegrationEvent
            |
      RabbitMQ Exchange (fanout)
            |
    +-------+-------+
    |               |
Inventory       Payment
Consumer        Consumer
(reserve stock) (process payment)
    |               |
    +-------+-------+
            |
      Notification
        Consumer
    (send confirmation)
```

### Domain Events vs Integration Events

| Type | Scope | Transport | Purpose |
|---|---|---|---|
| Domain Event | Within service | In-memory | Tracks what happened in the domain |
| Integration Event | Cross-service | RabbitMQ | Triggers actions in other services |

### Generic Repository Pattern

```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

---

## Order Flow

```
1. Client sends POST /api/orders via API Gateway

2. Order Service:
   - Validates request
   - Creates order (status: Pending)
   - Persists to SQL Server
   - Publishes OrderPlacedIntegrationEvent to RabbitMQ
   - Returns 202 Accepted immediately

3. Async background processing via RabbitMQ:
   - Inventory Service receives event and reserves stock
   - Payment Service receives event and processes payment
   - Notification Service receives event and sends confirmation

4. Each consumer publishes its own result event:
   - StockReservedIntegrationEvent
   - PaymentProcessedIntegrationEvent
```

---

## Authentication Flow

```
1. Register: POST /api/identity/register
   Returns: JWT access token and refresh token

2. Login: POST /api/identity/login
   Returns: JWT access token and refresh token

3. Subsequent requests:
   Authorization: Bearer {jwt_token}

4. Token expiry: 60 minutes
   Refresh token expiry: 7 days
```

---

## Basket Storage

Shopping cart data is stored in Redis as JSON with automatic expiry:

```
Key:   basket:{userId}
Value: { "userId": "...", "items": [...], "totalAmount": 999.99 }
TTL:   24 hours (auto-deleted, no manual cleanup required)
```

---

## Testing

### API Testing via Postman

```bash
# Step 1 - Register a user
POST http://localhost:5000/api/identity/register
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "Test@123"
}

# Step 2 - Add inventory for a product
POST http://localhost:5000/api/inventory
{
  "productId": "{productId}",
  "productName": "Product Name",
  "quantity": 100
}

# Step 3 - Add item to basket
POST http://localhost:5000/api/basket
{
  "userId": "{userId}",
  "productId": "{productId}",
  "productName": "Product Name",
  "price": 999.99,
  "quantity": 2
}

# Step 4 - Place an order
POST http://localhost:5000/api/orders
{
  "userId": "{userId}",
  "shippingAddress": "Dubai, UAE",
  "items": [
    {
      "productId": "{productId}",
      "productName": "Product Name",
      "unitPrice": 999.99,
      "quantity": 2
    }
  ]
}

# Step 5 - Verify inventory was reduced
GET http://localhost:5000/api/inventory/product/{productId}

# Step 6 - Verify payment was processed
GET http://localhost:5000/api/payments/order/{orderId}
```

### Individual Service Swagger

Each service exposes its own Swagger UI for direct testing:

```
http://localhost:5001/swagger  Identity Service
http://localhost:5002/swagger  Product Service
http://localhost:5003/swagger  Basket Service
http://localhost:5004/swagger  Order Service
http://localhost:5005/swagger  Inventory Service
http://localhost:5006/swagger  Payment Service
```

---

## Future Enhancements

- Angular 17 frontend with NgRx state management
- React 18 frontend with Redux Toolkit and Tailwind CSS
- OpenTelemetry distributed tracing across all services
- Kubernetes deployment manifests
- Integration test suite using MassTransit test harness
- Outbox pattern for guaranteed message delivery
- Redis caching layer for Product Service
- JWT validation at API Gateway level
- Per-endpoint rate limiting
- Health check endpoints with readiness and liveness probes

---

## License

MIT License
