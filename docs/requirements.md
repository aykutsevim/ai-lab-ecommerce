# GameVault E-Commerce Platform — System Requirements Document

**Version:** 1.0  
**Date:** 2026-02-10  
**Status:** Draft  
**Project Codename:** GameVault

---

## 1. Executive Summary

GameVault is a full-stack e-commerce platform for gaming consoles and accessories. It features a React-based storefront for customers, a React-based admin panel for product management, and a .NET 10 backend API. The platform's distinctive capability is an **AI-powered comment summarizer** that leverages a self-hosted Ollama LLM instance to generate concise summaries from user reviews, displayed alongside product listings. Payment processing is out of scope for v1 and will be represented by a mockup checkout flow. The project is structured as a monorepo with Docker Compose orchestration for deployment.

---

## 2. Project Architecture Overview

```
gamevault/
├── docker-compose.yml
├── docker-compose.override.yml
├── .env.example
├── src/
│   ├── backend/                  # .NET 10 Web API
│   │   ├── GameVault.Api/        # API host / controllers
│   │   ├── GameVault.Core/       # Domain models, interfaces
│   │   ├── GameVault.Infrastructure/ # EF Core, Ollama client, services
│   │   └── GameVault.Tests/      # Unit & integration tests
│   ├── frontend/
│   │   ├── storefront/           # React – customer-facing SPA
│   │   └── admin/                # React – admin panel SPA
│   └── shared/                   # Shared TypeScript types / API contracts
├── db/
│   ├── migrations/               # EF Core migrations (auto-generated)
│   └── seed/                     # Seed data (mock-products.json)
└── docs/
    └── requirements.md           # This file
```

### 2.1 Technology Stack

| Layer | Technology | Version |
|-------|-----------|---------|
| Backend Runtime | .NET | 10 (latest) |
| Backend Framework | ASP.NET Core Web API | 10 |
| ORM | Entity Framework Core | 10 |
| Database | PostgreSQL | 16+ |
| Cache | Redis | 7+ |
| Frontend (Storefront) | React | 19+ |
| Frontend (Admin) | React | 19+ |
| UI Library | Tailwind CSS 4 + shadcn/ui | Latest |
| State Management | TanStack Query (React Query) | v5 |
| Routing | React Router | v7 |
| Build Tool | Vite | 6+ |
| AI/LLM | Ollama (external, self-hosted) | Latest |
| Containerization | Docker + Docker Compose | Latest |
| Reverse Proxy | Nginx | Latest (Alpine) |

---

## 3. Data Model

The data model is derived from the provided `mock-products.json` file. All entities use UUID primary keys generated server-side.

### 3.1 Entity Relationship Diagram (Textual)

```
User (1) ──── (*) Comment
Category (1) ──── (*) Product
Product (1) ──── (*) ProductVariant
Product (1) ──── (*) ProductImage
Product (1) ──── (*) ProductSpec
Product (1) ──── (*) ProductTag
Product (1) ──── (*) Comment
Product (1) ──── (0..1) CommentSummary
Product (1) ──── (*) OrderItem
Order (1) ──── (*) OrderItem
User (1) ──── (*) Order
```

### 3.2 Entity Definitions

#### 3.2.1 Category

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| Name | varchar(100) | NOT NULL, UNIQUE | e.g. "Game Consoles" |
| Slug | varchar(120) | NOT NULL, UNIQUE | e.g. "game-consoles" |
| Description | text | NULLABLE | |
| ImageUrl | varchar(500) | NULLABLE | |
| SortOrder | int | NOT NULL, DEFAULT 0 | |
| IsActive | bool | NOT NULL, DEFAULT true | |
| CreatedAt | timestamptz | NOT NULL | |
| UpdatedAt | timestamptz | NOT NULL | |

#### 3.2.2 Product

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| CategoryId | UUID | FK → Category.Id, NOT NULL | |
| Name | varchar(200) | NOT NULL | |
| Slug | varchar(220) | NOT NULL, UNIQUE | |
| Brand | varchar(100) | NOT NULL | |
| ShortDescription | varchar(500) | NOT NULL | |
| Description | text | NOT NULL | |
| Price | decimal(10,2) | NOT NULL | |
| CompareAtPrice | decimal(10,2) | NULLABLE | Strikethrough / original price |
| Currency | varchar(3) | NOT NULL, DEFAULT 'USD' | |
| InStock | bool | NOT NULL, DEFAULT true | |
| StockCount | int | NOT NULL, DEFAULT 0 | |
| Rating | decimal(2,1) | NOT NULL, DEFAULT 0 | Computed average |
| ReviewCount | int | NOT NULL, DEFAULT 0 | Computed count |
| Subcategory | varchar(100) | NULLABLE | e.g. "controllers", "audio" |
| IsActive | bool | NOT NULL, DEFAULT true | |
| CreatedAt | timestamptz | NOT NULL | |
| UpdatedAt | timestamptz | NOT NULL | |

#### 3.2.3 ProductVariant

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| Name | varchar(100) | NOT NULL | e.g. "Midnight Black" |
| Color | varchar(7) | NULLABLE | Hex code e.g. "#1a1a2e" |
| ImageUrl | varchar(500) | NULLABLE | |
| AdditionalPrice | decimal(10,2) | NOT NULL, DEFAULT 0 | Price delta from base |
| SortOrder | int | NOT NULL, DEFAULT 0 | |
| CreatedAt | timestamptz | NOT NULL | |

#### 3.2.4 ProductImage

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| Url | varchar(500) | NOT NULL | |
| Alt | varchar(300) | NOT NULL | |
| VariantName | varchar(100) | NULLABLE | Links image to variant visually |
| SortOrder | int | NOT NULL, DEFAULT 0 | |
| CreatedAt | timestamptz | NOT NULL | |

#### 3.2.5 ProductSpec

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| Key | varchar(100) | NOT NULL | e.g. "processor", "ram" |
| Value | varchar(500) | NOT NULL | e.g. "Custom 12-core Zen 5" |
| SortOrder | int | NOT NULL, DEFAULT 0 | |

#### 3.2.6 ProductTag

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| Tag | varchar(50) | NOT NULL | e.g. "next-gen", "4k" |

**Unique constraint:** (ProductId, Tag)

#### 3.2.7 User

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| Email | varchar(254) | NOT NULL, UNIQUE | |
| PasswordHash | varchar(500) | NOT NULL | BCrypt hash |
| FirstName | varchar(100) | NOT NULL | |
| LastName | varchar(100) | NOT NULL | |
| Role | varchar(20) | NOT NULL, DEFAULT 'Customer' | "Admin" or "Customer" |
| IsActive | bool | NOT NULL, DEFAULT true | |
| CreatedAt | timestamptz | NOT NULL | |
| UpdatedAt | timestamptz | NOT NULL | |

#### 3.2.8 Comment

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| UserId | UUID | FK → User.Id, NOT NULL | |
| Title | varchar(200) | NULLABLE | |
| Body | text | NOT NULL | |
| Rating | int | NOT NULL, CHECK 1–5 | Star rating |
| IsApproved | bool | NOT NULL, DEFAULT true | Moderation flag |
| CreatedAt | timestamptz | NOT NULL | |
| UpdatedAt | timestamptz | NOT NULL | |

#### 3.2.9 CommentSummary

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| ProductId | UUID | FK → Product.Id, UNIQUE | One summary per product |
| Summary | text | NOT NULL | AI-generated summary |
| CommentCount | int | NOT NULL | Comment count at generation time |
| ModelName | varchar(100) | NOT NULL | Ollama model used |
| GeneratedAt | timestamptz | NOT NULL | |
| IsStale | bool | NOT NULL, DEFAULT false | Flagged when new comments arrive |

#### 3.2.10 Order

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| UserId | UUID | FK → User.Id, NOT NULL | |
| Status | varchar(30) | NOT NULL, DEFAULT 'Pending' | See enum below |
| TotalAmount | decimal(12,2) | NOT NULL | |
| Currency | varchar(3) | NOT NULL, DEFAULT 'USD' | |
| ShippingAddress | text | NULLABLE | JSON blob (mockup) |
| Notes | text | NULLABLE | |
| CreatedAt | timestamptz | NOT NULL | |
| UpdatedAt | timestamptz | NOT NULL | |

**Order Statuses (v1 mockup):** Pending → Confirmed → Shipped → Delivered → Cancelled

#### 3.2.11 OrderItem

| Column | Type | Constraints | Notes |
|--------|------|------------|-------|
| Id | UUID | PK | |
| OrderId | UUID | FK → Order.Id, NOT NULL | |
| ProductId | UUID | FK → Product.Id, NOT NULL | |
| VariantId | UUID | FK → ProductVariant.Id, NULLABLE | |
| Quantity | int | NOT NULL, CHECK > 0 | |
| UnitPrice | decimal(10,2) | NOT NULL | Price snapshot at order time |
| CreatedAt | timestamptz | NOT NULL | |

---

## 4. Functional Requirements

### 4.1 Admin Panel (React SPA)

#### FR-A01: Authentication & Authorization
- Admin users log in with email/password via JWT-based authentication.
- Only users with Role = "Admin" can access the admin panel.
- JWT tokens are stored in httpOnly cookies; refresh token rotation is implemented.

#### FR-A02: Dashboard
- Display summary cards: total products, total orders, total users, total comments.
- Show recent orders list (last 10).
- Show products with stale comment summaries requiring regeneration.

#### FR-A03: Category Management
- List all categories with product count, sort order, and active status.
- Create, update, and delete categories (soft-delete via IsActive flag).
- Reorder categories via drag-and-drop or sort order field.
- Slug is auto-generated from the name but can be overridden.

#### FR-A04: Product Management
- List products with filtering by category, stock status, price range, and search by name/brand.
- Paginated list with configurable page size (10/25/50).
- Create a new product with all fields: name, slug, category, brand, price, compareAtPrice, descriptions, stock info, subcategory.
- Manage product variants inline (name, color hex, image upload).
- Manage product images with drag-and-drop upload and sort ordering.
- Manage product specs as key-value pairs with add/remove rows.
- Manage product tags as a tag input component.
- Toggle product active status.
- Bulk import from JSON (compatible with mock-products.json format).

#### FR-A05: Comment Moderation
- List all comments with filtering by product, rating, approval status, and date range.
- Approve or reject individual comments.
- Bulk approve/reject selected comments.
- View comment summary for a product and trigger manual re-summarization.

#### FR-A06: Order Management (Mockup)
- List all orders with filtering by status, date range, and user.
- View order details with line items.
- Update order status (Pending → Confirmed → Shipped → Delivered / Cancelled).

#### FR-A07: User Management
- List users with filtering by role and active status.
- View user details with their order and comment history.
- Toggle user active status.
- Promote/demote user role (Customer ↔ Admin).

#### FR-A08: Comment Summary Management
- View all product summaries with staleness indicators.
- Trigger re-summarization for individual products.
- Trigger bulk re-summarization for all stale summaries.
- View Ollama connection health status.

### 4.2 Storefront (React SPA)

#### FR-S01: Authentication
- Register with email, password, first name, last name.
- Login with email/password via JWT.
- Password reset flow (email-based, mockup in v1 — logs reset link to console).
- Persistent login via refresh token.

#### FR-S02: Home Page
- Display featured categories with images.
- Display featured/promoted products (top-rated or manually selected).
- Display recently viewed products (client-side localStorage).

#### FR-S03: Category Browsing
- List all active categories.
- Click a category to see its products.
- Breadcrumb navigation (Home → Category → Product).

#### FR-S04: Product Listing
- Display products in a responsive grid (cards with image, name, brand, price, rating, stock badge).
- If a comment summary exists for a product, display a truncated AI summary snippet on the card.
- Filtering: price range, brand, in-stock only, tags, subcategory.
- Sorting: price (asc/desc), rating, newest, name.
- Pagination with configurable page size.
- Search bar with debounced full-text search across name, brand, description, and tags.

#### FR-S05: Product Detail Page
- Full product information: images (gallery with variant switching), name, brand, price/compareAtPrice, descriptions, specs table, tags, stock status.
- Variant selector (visual color swatches) that updates displayed image.
- "Add to Cart" button with quantity selector.
- Product comments section (paginated, sorted by newest first).
- AI-generated comment summary displayed prominently above individual comments if available, with a badge indicating it was AI-generated and the number of comments it was based on.
- Comment submission form (authenticated users only): title, body, 1–5 star rating.

#### FR-S06: Shopping Cart
- Client-side cart (localStorage) synced to server on login.
- Add/remove items, update quantities.
- Display line items with variant info, unit price, subtotal.
- Display cart total.
- "Proceed to Checkout" button.

#### FR-S07: Checkout (Mockup)
- Shipping address form (name, address lines, city, state, zip, country).
- Order summary review.
- "Place Order" button that creates an Order with status "Pending" — no real payment is processed.
- Confirmation page with order number.
- A clearly visible banner stating: "Payment integration is a mockup. No real charges are made."

#### FR-S08: User Profile
- View and edit profile information (name, email).
- Change password.
- View order history with status.
- View submitted comments.

### 4.3 Backend API (.NET 10)

#### FR-B01: RESTful API
- All endpoints follow REST conventions with proper HTTP methods and status codes.
- API versioning via URL path prefix: `/api/v1/`.
- All list endpoints support pagination (offset-based), filtering, and sorting.
- Consistent JSON response envelope: `{ data, meta: { page, pageSize, total }, errors }`.

#### FR-B02: Authentication Endpoints
- `POST /api/v1/auth/register` — Create user account.
- `POST /api/v1/auth/login` — Issue JWT access + refresh tokens.
- `POST /api/v1/auth/refresh` — Rotate refresh token.
- `POST /api/v1/auth/logout` — Invalidate refresh token.
- `POST /api/v1/auth/forgot-password` — Initiate reset (mockup).
- `POST /api/v1/auth/reset-password` — Complete reset (mockup).

#### FR-B03: Category Endpoints
- `GET /api/v1/categories` — Public list (active only for storefront, all for admin).
- `GET /api/v1/categories/{slug}` — Public detail.
- `POST /api/v1/admin/categories` — Admin create.
- `PUT /api/v1/admin/categories/{id}` — Admin update.
- `DELETE /api/v1/admin/categories/{id}` — Admin soft-delete.

#### FR-B04: Product Endpoints
- `GET /api/v1/products` — Public list with filters, sorting, pagination; includes comment summary snippet if available.
- `GET /api/v1/products/{slug}` — Public detail with variants, images, specs, tags, summary.
- `POST /api/v1/admin/products` — Admin create (with nested variants, images, specs, tags).
- `PUT /api/v1/admin/products/{id}` — Admin update.
- `DELETE /api/v1/admin/products/{id}` — Admin soft-delete.
- `POST /api/v1/admin/products/import` — Admin bulk import from JSON.

#### FR-B05: Comment Endpoints
- `GET /api/v1/products/{productId}/comments` — Public paginated list.
- `POST /api/v1/products/{productId}/comments` — Authenticated user creates comment; triggers summary staleness flag.
- `PUT /api/v1/admin/comments/{id}/approve` — Admin approve.
- `PUT /api/v1/admin/comments/{id}/reject` — Admin reject.

#### FR-B06: Comment Summary Endpoints
- `GET /api/v1/products/{productId}/summary` — Public; returns current AI summary.
- `POST /api/v1/admin/summaries/{productId}/generate` — Admin trigger re-summarization.
- `POST /api/v1/admin/summaries/generate-stale` — Admin trigger bulk re-summarization for all stale.
- `GET /api/v1/admin/summaries/health` — Admin check Ollama connection status.

#### FR-B07: Order Endpoints (Mockup)
- `POST /api/v1/orders` — Authenticated user creates order from cart.
- `GET /api/v1/orders` — Authenticated user's order history.
- `GET /api/v1/orders/{id}` — Authenticated user's order detail.
- `GET /api/v1/admin/orders` — Admin list all orders.
- `PUT /api/v1/admin/orders/{id}/status` — Admin update order status.

#### FR-B08: User Endpoints
- `GET /api/v1/users/me` — Get current user profile.
- `PUT /api/v1/users/me` — Update profile.
- `PUT /api/v1/users/me/password` — Change password.
- `GET /api/v1/admin/users` — Admin list users.
- `PUT /api/v1/admin/users/{id}/role` — Admin change role.
- `PUT /api/v1/admin/users/{id}/status` — Admin toggle active.

#### FR-B09: Dashboard Endpoints
- `GET /api/v1/admin/dashboard/stats` — Aggregated counts (products, orders, users, comments).
- `GET /api/v1/admin/dashboard/recent-orders` — Last 10 orders.
- `GET /api/v1/admin/dashboard/stale-summaries` — Products needing re-summarization.

### 4.4 Comment Summarizer (Distinctive Feature)

#### FR-CS01: Summary Generation Trigger
- When a new comment is submitted for a product, the existing CommentSummary for that product is marked as `IsStale = true`.
- A background job (Hangfire or .NET BackgroundService) periodically checks for stale summaries and regenerates them.
- Configurable minimum comment threshold before first summary generation (default: 3 comments).
- Admin can also manually trigger regeneration via the API.

#### FR-CS02: Ollama Integration
- The backend communicates with an external Ollama instance via HTTP REST API.
- Ollama connection parameters are configured entirely via environment variables (no Ollama in Docker Compose).
- All approved comments for a product are collected, formatted into a prompt, and sent to Ollama for summarization.
- The prompt instructs the model to produce a concise, neutral summary highlighting common themes, praised features, and reported issues.

#### FR-CS03: Summary Storage & Display
- The generated summary is stored in the CommentSummary table with the model name, generation timestamp, and the comment count at the time of generation.
- On product listing pages (cards), the summary is shown as a truncated snippet (max 150 characters) with an "AI Summary" badge.
- On product detail pages, the full summary is displayed in a highlighted section above the individual comments, with metadata: "Generated from N reviews using AI" and the generation date.

#### FR-CS04: Resilience & Fallback
- If Ollama is unreachable, the summary generation job logs a warning and retries with exponential backoff (3 retries, 30s/60s/120s).
- If generation fails after retries, the product retains its previous summary (if any) and the IsStale flag remains true for the next cycle.
- The admin panel shows Ollama health status and surfaces products stuck in stale state.

---

## 5. Non-Functional Requirements

### 5.1 Performance

| Requirement | Target |
|-------------|--------|
| NFR-P01: API response time (p95) | < 200ms for list endpoints, < 100ms for detail endpoints |
| NFR-P02: Product listing page load (FCP) | < 1.5s on 4G connection |
| NFR-P03: Database query time (p95) | < 50ms |
| NFR-P04: Comment summary generation | < 30s per product (depends on Ollama model/hardware) |
| NFR-P05: Concurrent users supported | Minimum 500 simultaneous users |
| NFR-P06: Image delivery | Served via static file server / CDN-ready paths |

### 5.2 Scalability

- **NFR-S01:** The backend API is stateless and horizontally scalable behind a load balancer.
- **NFR-S02:** Database connection pooling via Npgsql with configurable pool size.
- **NFR-S03:** Redis is used for response caching (product lists, summaries) and distributed locking for summary generation jobs (preventing duplicate work).
- **NFR-S04:** Background summary jobs are idempotent and safe for concurrent execution.

### 5.3 Security

- **NFR-SEC01:** All passwords are hashed with BCrypt (minimum work factor 12).
- **NFR-SEC02:** JWT access tokens have a 15-minute expiry; refresh tokens have a 7-day expiry with rotation.
- **NFR-SEC03:** All admin endpoints require the "Admin" role claim in the JWT.
- **NFR-SEC04:** Input validation is enforced at both the API (FluentValidation) and database (constraints) levels.
- **NFR-SEC05:** CORS is configured to allow only the storefront and admin panel origins.
- **NFR-SEC06:** Rate limiting is applied to authentication endpoints (10 requests/minute per IP).
- **NFR-SEC07:** SQL injection is prevented by using parameterized queries via EF Core.
- **NFR-SEC08:** XSS protection via React's default escaping and Content-Security-Policy headers.
- **NFR-SEC09:** HTTPS is enforced in production (TLS termination at Nginx).

### 5.4 Reliability & Availability

- **NFR-R01:** Health check endpoints (`/health`, `/health/ready`) for liveness and readiness probes.
- **NFR-R02:** Structured logging via Serilog with configurable sinks (console, file, seq).
- **NFR-R03:** Database migrations are applied automatically on startup in development; via CLI in production.
- **NFR-R04:** Graceful degradation: if Ollama is unavailable, all features except summary generation continue to function normally.
- **NFR-R05:** Automated database backups are outside the scope of Docker Compose but documented as a deployment prerequisite.

### 5.5 Maintainability

- **NFR-M01:** Clean Architecture / Vertical Slice pattern in the .NET backend with clear separation of concerns (API → Core → Infrastructure).
- **NFR-M02:** All API endpoints are documented via Swagger/OpenAPI (auto-generated from controllers).
- **NFR-M03:** Frontend code follows a feature-based folder structure.
- **NFR-M04:** Shared TypeScript types are generated from the OpenAPI spec (using openapi-typescript or similar).
- **NFR-M05:** Code quality enforced via ESLint + Prettier (frontend) and .editorconfig + dotnet format (backend).
- **NFR-M06:** Git conventional commits are recommended.

### 5.6 Testability

- **NFR-T01:** Backend unit tests targeting domain logic and services (xUnit + Moq/NSubstitute).
- **NFR-T02:** Backend integration tests using WebApplicationFactory with Testcontainers (PostgreSQL).
- **NFR-T03:** Frontend component tests using Vitest + React Testing Library.
- **NFR-T04:** Minimum 70% code coverage target for backend business logic.

### 5.7 Observability

- **NFR-O01:** Structured JSON logging with correlation IDs across requests.
- **NFR-O02:** Application metrics exposed via a `/metrics` endpoint (Prometheus format, optional).
- **NFR-O03:** Ollama integration logs include request/response times and token counts.

---

## 6. System Requirements — Infrastructure & Deployment

### 6.1 Docker Compose Services

The `docker-compose.yml` defines the following services. **Ollama is NOT included** — it is integrated via environment variables pointing to an external instance.

| Service | Image | Exposed Port | Notes |
|---------|-------|-------------|-------|
| `api` | Custom (Dockerfile in src/backend) | 5000 (internal) | .NET 10 Web API |
| `storefront` | Custom (Dockerfile in src/frontend/storefront) | 3000 (internal) | Nginx serving React build |
| `admin` | Custom (Dockerfile in src/frontend/admin) | 3001 (internal) | Nginx serving React build |
| `postgres` | postgres:16-alpine | 5432 | Persistent volume |
| `redis` | redis:7-alpine | 6379 | Persistent volume |
| `nginx` | nginx:alpine | 80, 443 | Reverse proxy / TLS termination |

### 6.2 Environment Variables (.env)

```env
# ── Database ──
POSTGRES_HOST=postgres
POSTGRES_PORT=5432
POSTGRES_DB=gamevault
POSTGRES_USER=gamevault
POSTGRES_PASSWORD=<secret>

# ── Redis ──
REDIS_HOST=redis
REDIS_PORT=6379

# ── JWT ──
JWT_SECRET=<secret-min-64-chars>
JWT_ISSUER=gamevault-api
JWT_AUDIENCE=gamevault-clients
JWT_ACCESS_TOKEN_EXPIRY_MINUTES=15
JWT_REFRESH_TOKEN_EXPIRY_DAYS=7

# ── Ollama (External) ──
OLLAMA_BASE_URL=http://host.docker.internal:11434
OLLAMA_MODEL=llama3.1:8b
OLLAMA_TIMEOUT_SECONDS=120
OLLAMA_MAX_RETRIES=3
OLLAMA_SUMMARY_MIN_COMMENTS=3
OLLAMA_SUMMARY_MAX_TOKENS=500

# ── API ──
API_BASE_URL=http://api:5000
CORS_ORIGINS=http://localhost:3000,http://localhost:3001

# ── Nginx ──
DOMAIN=localhost
```

### 6.3 Docker Compose Topology

```
                    ┌────────────────────────────┐
                    │         Nginx (:80/:443)    │
                    │    Reverse Proxy / TLS      │
                    └──────┬─────┬─────┬─────────┘
                           │     │     │
              ┌────────────┘     │     └────────────┐
              ▼                  ▼                   ▼
     ┌────────────────┐ ┌──────────────┐  ┌─────────────────┐
     │  Storefront    │ │   Admin      │  │   API           │
     │  React (:3000) │ │   React(:3001)│  │   .NET 10(:5000)│
     └────────────────┘ └──────────────┘  └──┬─────┬────────┘
                                              │     │
                                    ┌─────────┘     └─────────┐
                                    ▼                         ▼
                           ┌──────────────┐          ┌──────────────┐
                           │ PostgreSQL   │          │    Redis     │
                           │   (:5432)    │          │   (:6379)    │
                           └──────────────┘          └──────────────┘

                                    ┌─────────────────────────┐
                  API ──────────────▶  Ollama (External)      │
                  (HTTP REST)       │  (via OLLAMA_BASE_URL)  │
                                    └─────────────────────────┘
```

### 6.4 Volume Mounts

| Volume | Mount Point | Purpose |
|--------|------------|---------|
| `pgdata` | /var/lib/postgresql/data | PostgreSQL persistence |
| `redisdata` | /data | Redis persistence |
| `uploads` | /app/uploads | User-uploaded product images |
| `nginx-conf` | /etc/nginx/conf.d | Nginx configuration |

### 6.5 Build & Run Commands

```bash
# Development
docker compose up --build

# Production
docker compose -f docker-compose.yml -f docker-compose.prod.yml up -d

# Database migration (run inside API container)
docker compose exec api dotnet ef database update

# Seed data
docker compose exec api dotnet run -- --seed
```

---

## 7. API Contract Summary

### 7.1 Response Envelope

All API responses follow this structure:

```json
{
  "data": { },
  "meta": {
    "page": 1,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5
  },
  "errors": []
}
```

### 7.2 Error Response

```json
{
  "data": null,
  "meta": null,
  "errors": [
    {
      "code": "VALIDATION_ERROR",
      "field": "price",
      "message": "Price must be greater than zero."
    }
  ]
}
```

### 7.3 HTTP Status Code Convention

| Code | Usage |
|------|-------|
| 200 | Successful GET, PUT |
| 201 | Successful POST (resource created) |
| 204 | Successful DELETE |
| 400 | Validation error |
| 401 | Unauthorized (missing/invalid token) |
| 403 | Forbidden (insufficient role) |
| 404 | Resource not found |
| 409 | Conflict (duplicate slug, etc.) |
| 429 | Rate limited |
| 500 | Internal server error |
| 503 | Service unavailable (Ollama down, etc.) |

---

## 8. Comment Summarizer — Detailed Specification

### 8.1 Prompt Template

```
You are a product review summarizer. Given the following customer reviews for
the product "{ProductName}", produce a concise summary (max 3 paragraphs) that:

1. Highlights the most commonly praised features.
2. Notes frequently mentioned drawbacks or complaints.
3. Gives an overall sentiment assessment.

Be neutral, factual, and reference specific feature themes (not individual users).

---
Reviews:
{FormattedReviews}
---

Summary:
```

Each review in `{FormattedReviews}` is formatted as:
```
[★★★★☆] {Title}
{Body}
```

### 8.2 Background Job Flow

```
1. New comment saved → CommentSummary.IsStale = true for that product
2. Background worker polls every 60 seconds (configurable via SUMMARY_JOB_INTERVAL_SECONDS)
3. Finds all products where IsStale = true AND CommentCount >= OLLAMA_SUMMARY_MIN_COMMENTS
4. Acquires a Redis distributed lock per product (prevents duplicate work)
5. Fetches all approved comments for the product
6. Builds prompt from template
7. Calls Ollama API: POST {OLLAMA_BASE_URL}/api/generate
8. On success: upserts CommentSummary, sets IsStale = false
9. On failure: logs error, retries with exponential backoff
10. Releases Redis lock
```

### 8.3 Ollama API Integration

```http
POST {OLLAMA_BASE_URL}/api/generate
Content-Type: application/json

{
  "model": "{OLLAMA_MODEL}",
  "prompt": "{constructed_prompt}",
  "stream": false,
  "options": {
    "num_predict": {OLLAMA_SUMMARY_MAX_TOKENS},
    "temperature": 0.3
  }
}
```

Response parsing extracts the `response` field from the Ollama JSON response.

---

## 9. Seed Data Strategy

The `mock-products.json` file serves as the seed data source. On first run (or via `--seed` flag):

1. Parse the JSON file.
2. Upsert categories from `meta.categories`.
3. Upsert products with their nested variants, images, specs, and tags.
4. Create a default admin user: `admin@gamevault.local` / `Admin123!`.
5. Create a sample customer user: `customer@gamevault.local` / `Customer123!`.
6. Optionally generate mock comments (10–20 per product) for testing the summarizer.

---

## 10. Out of Scope (v1)

- Real payment gateway integration (Stripe, PayPal, etc.).
- Email delivery service (all emails are logged to console in v1).
- Product image upload to cloud storage (S3, Azure Blob) — local file system only.
- Multi-language / i18n support.
- Ollama deployment and management (external responsibility).
- CI/CD pipeline definitions.
- Kubernetes / Helm chart definitions.
- Analytics and reporting dashboards beyond the admin summary stats.
- Real-time features (WebSocket notifications, live inventory updates).
- Social login (Google, GitHub OAuth).

---

## 11. Glossary

| Term | Definition |
|------|-----------|
| **Ollama** | Open-source tool for running LLMs locally. Used as an external service for comment summarization. |
| **CommentSummary** | An AI-generated text summarizing all approved comments for a product. |
| **Stale Summary** | A summary that was generated before new comments were added and needs regeneration. |
| **Monorepo** | A single Git repository containing all project components (backend, frontend, infrastructure). |
| **Storefront** | The customer-facing React SPA where users browse and purchase products. |
| **Admin Panel** | The internal React SPA for managing products, orders, comments, and users. |
| **Seed Data** | Initial data loaded from mock-products.json to populate the database on first run. |
