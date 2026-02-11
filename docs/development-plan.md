# GameVault E-Commerce Platform — Development Plan

**Version:** 1.0  
**Created:** 2026-02-11  
**Last Updated:** 2026-02-11  
**Status:** 🟡 In Progress  
**Requirements Reference:** requirements.md v1.0

---

## Table of Contents

1. [Development Overview](#1-development-overview)
2. [Phase Summary](#2-phase-summary)
3. [Phase 1: Project Foundation & Infrastructure](#3-phase-1-project-foundation--infrastructure)
4. [Phase 2: Backend Core — Data Layer & Authentication](#4-phase-2-backend-core--data-layer--authentication)
5. [Phase 3: Backend API — Product & Category Management](#5-phase-3-backend-api--product--category-management)
6. [Phase 4: Backend API — Comments & AI Summarizer](#6-phase-4-backend-api--comments--ai-summarizer)
7. [Phase 5: Backend API — Orders & Admin Features](#7-phase-5-backend-api--orders--admin-features)
8. [Phase 6: Frontend Storefront — Core Features](#8-phase-6-frontend-storefront--core-features)
9. [Phase 7: Frontend Storefront — Product & Checkout](#9-phase-7-frontend-storefront--product--checkout)
10. [Phase 8: Frontend Admin Panel](#10-phase-8-frontend-admin-panel)
11. [Phase 9: Integration, Testing & Polish](#11-phase-9-integration-testing--polish)
12. [Phase 10: Documentation & Deployment Preparation](#12-phase-10-documentation--deployment-preparation)
13. [Risk Register](#13-risk-register)
14. [Progress Tracking](#14-progress-tracking)
15. [Change Log](#15-change-log)

---

## 1. Development Overview

### 1.1 Project Summary

GameVault is a full-stack e-commerce platform for gaming consoles and accessories featuring:
- **Customer Storefront** (React 19 SPA)
- **Admin Panel** (React 19 SPA)
- **Backend API** (.NET 10 Web API)
- **AI-Powered Comment Summarizer** (Ollama LLM integration)
- **Docker Compose** orchestration for deployment

### 1.2 Development Principles

| Principle | Implementation |
|-----------|----------------|
| **API-First** | Backend endpoints fully functional before frontend integration |
| **Iterative Delivery** | Each phase produces testable, deployable artifacts |
| **Clean Architecture** | Vertical slice pattern with clear separation of concerns |
| **Test-Driven** | Unit and integration tests for critical business logic |
| **Documentation** | Swagger/OpenAPI specs, code comments, README files |

### 1.3 Technology Stack Quick Reference

| Component | Technology |
|-----------|-----------|
| Backend | .NET 10, ASP.NET Core, EF Core 10 |
| Database | PostgreSQL 16+ |
| Cache | Redis 7+ |
| Frontend | React 19, Vite 6, TanStack Query v5, React Router v7 |
| UI | Tailwind CSS 4, shadcn/ui |
| AI/LLM | Ollama (external) |
| Infrastructure | Docker, Docker Compose, Nginx |

---

## 2. Phase Summary

| Phase | Name | Est. Duration | Status | Dependencies |
|-------|------|---------------|--------|--------------|
| 1 | Project Foundation & Infrastructure | 2-3 days | ⬜ Not Started | None |
| 2 | Backend Core — Data Layer & Auth | 3-4 days | ⬜ Not Started | Phase 1 |
| 3 | Backend API — Products & Categories | 2-3 days | ⬜ Not Started | Phase 2 |
| 4 | Backend API — Comments & Summarizer | 3-4 days | ⬜ Not Started | Phase 3 |
| 5 | Backend API — Orders & Admin | 2-3 days | ⬜ Not Started | Phase 4 |
| 6 | Frontend Storefront — Core | 3-4 days | ⬜ Not Started | Phase 5 |
| 7 | Frontend Storefront — Product & Checkout | 3-4 days | ⬜ Not Started | Phase 6 |
| 8 | Frontend Admin Panel | 4-5 days | ⬜ Not Started | Phase 5 |
| 9 | Integration, Testing & Polish | 3-4 days | ⬜ Not Started | Phases 7, 8 |
| 10 | Documentation & Deployment | 2-3 days | ⬜ Not Started | Phase 9 |

**Total Estimated Duration:** 27-37 days

---

## 3. Phase 1: Project Foundation & Infrastructure

**Objective:** Establish the monorepo structure, Docker Compose configuration, and development environment.

### 3.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 1.1 | Create monorepo directory structure | High | ⬜ | Per requirements section 2 |
| 1.2 | Initialize Git repository with `.gitignore` | High | ⬜ | |
| 1.3 | Create `docker-compose.yml` with all services | High | ⬜ | postgres, redis, nginx |
| 1.4 | Create `docker-compose.override.yml` for development | Medium | ⬜ | Volume mounts, hot reload |
| 1.5 | Create `.env.example` with all environment variables | High | ⬜ | Per section 6.2 |
| 1.6 | Setup Nginx reverse proxy configuration | Medium | ⬜ | Routes to api, storefront, admin |
| 1.7 | Create backend solution structure | High | ⬜ | Api, Core, Infrastructure, Tests |
| 1.8 | Create frontend project scaffolds (Vite + React) | High | ⬜ | storefront, admin, shared |
| 1.9 | Configure ESLint, Prettier, EditorConfig | Medium | ⬜ | |
| 1.10 | Create initial README.md with setup instructions | Medium | ⬜ | |

### 3.2 Deliverables

- [ ] Complete monorepo structure as per architecture diagram
- [ ] Working `docker compose up` with PostgreSQL and Redis running
- [ ] Backend solution compiles and runs
- [ ] Frontend projects serve development builds
- [ ] All configuration files in place

### 3.3 Acceptance Criteria

1. `docker compose up --build` successfully starts all infrastructure services
2. Backend API responds to health check at `/health`
3. Frontend dev servers accessible at configured ports
4. Environment variables properly loaded from `.env`

### 3.4 Progress Notes

*Notes will be added as development progresses...*

---

## 4. Phase 2: Backend Core — Data Layer & Authentication

**Objective:** Implement EF Core data models, database migrations, and JWT authentication system.

### 4.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 2.1 | Define domain entities in Core project | High | ⬜ | All 11 entities |
| 2.2 | Create EF Core DbContext with configurations | High | ⬜ | Fluent API mappings |
| 2.3 | Configure PostgreSQL connection with Npgsql | High | ⬜ | Connection pooling |
| 2.4 | Create initial EF Core migration | High | ⬜ | |
| 2.5 | Implement seed data loader from JSON | High | ⬜ | FR: Seed Data Strategy |
| 2.6 | Implement BCrypt password hashing service | High | ⬜ | NFR-SEC01 |
| 2.7 | Implement JWT token generation service | High | ⬜ | Access + refresh tokens |
| 2.8 | Create auth endpoints (register, login, refresh, logout) | High | ⬜ | FR-B02 |
| 2.9 | Implement refresh token rotation | High | ⬜ | FR-A01, NFR-SEC02 |
| 2.10 | Add rate limiting to auth endpoints | Medium | ⬜ | NFR-SEC06 |
| 2.11 | Setup Redis connection for caching | Medium | ⬜ | |
| 2.12 | Create default admin and customer seed users | High | ⬜ | |
| 2.13 | Write unit tests for auth services | Medium | ⬜ | NFR-T01 |

### 4.2 Entity Implementation Checklist

| Entity | Model | Configuration | Migration | Seed | Tests |
|--------|-------|---------------|-----------|------|-------|
| Category | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| Product | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| ProductVariant | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| ProductImage | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| ProductSpec | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| ProductTag | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| User | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| Comment | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| CommentSummary | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| Order | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |
| OrderItem | ⬜ | ⬜ | ⬜ | ⬜ | ⬜ |

### 4.3 Deliverables

- [ ] All domain entities with proper relationships
- [ ] Database migration successfully applied
- [ ] Working auth endpoints with JWT tokens
- [ ] Seed data loaded including mock products
- [ ] Unit tests passing for auth logic

### 4.4 Acceptance Criteria

1. Database schema matches ERD in requirements
2. `POST /api/v1/auth/register` creates new user
3. `POST /api/v1/auth/login` returns valid JWT
4. Refresh token rotation works correctly
5. Seed command populates database with mock data

### 4.5 Progress Notes

*Notes will be added as development progresses...*

---

## 5. Phase 3: Backend API — Product & Category Management

**Objective:** Implement public and admin endpoints for products and categories.

### 5.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 3.1 | Create API response envelope DTOs | High | ⬜ | data, meta, errors |
| 3.2 | Implement global exception handling middleware | High | ⬜ | |
| 3.3 | Create category DTOs and mappings | High | ⬜ | |
| 3.4 | Implement public category endpoints | High | ⬜ | FR-B03 |
| 3.5 | Implement admin category endpoints | High | ⬜ | FR-B03 |
| 3.6 | Create product DTOs with nested entities | High | ⬜ | |
| 3.7 | Implement public product list with filtering | High | ⬜ | FR-B04 |
| 3.8 | Implement public product detail endpoint | High | ⬜ | FR-B04 |
| 3.9 | Implement admin product CRUD endpoints | High | ⬜ | FR-B04 |
| 3.10 | Implement product bulk import endpoint | Medium | ⬜ | FR-A04 |
| 3.11 | Add pagination support (offset-based) | High | ⬜ | FR-B01 |
| 3.12 | Implement Redis caching for product lists | Medium | ⬜ | NFR-S03 |
| 3.13 | Add FluentValidation for product DTOs | High | ⬜ | NFR-SEC04 |
| 3.14 | Setup Swagger/OpenAPI documentation | Medium | ⬜ | NFR-M02 |
| 3.15 | Write integration tests for product API | Medium | ⬜ | NFR-T02 |

### 5.2 API Endpoints Checklist

| Endpoint | Method | Auth | Status | Tests |
|----------|--------|------|--------|-------|
| `/api/v1/categories` | GET | Public | ⬜ | ⬜ |
| `/api/v1/categories/{slug}` | GET | Public | ⬜ | ⬜ |
| `/api/v1/admin/categories` | POST | Admin | ⬜ | ⬜ |
| `/api/v1/admin/categories/{id}` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/admin/categories/{id}` | DELETE | Admin | ⬜ | ⬜ |
| `/api/v1/products` | GET | Public | ⬜ | ⬜ |
| `/api/v1/products/{slug}` | GET | Public | ⬜ | ⬜ |
| `/api/v1/admin/products` | POST | Admin | ⬜ | ⬜ |
| `/api/v1/admin/products/{id}` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/admin/products/{id}` | DELETE | Admin | ⬜ | ⬜ |
| `/api/v1/admin/products/import` | POST | Admin | ⬜ | ⬜ |

### 5.3 Deliverables

- [ ] All category endpoints functional
- [ ] All product endpoints functional with filtering/sorting/pagination
- [ ] Redis caching implemented
- [ ] Swagger documentation generated
- [ ] Integration tests passing

### 5.4 Acceptance Criteria

1. Product list supports all filter parameters (category, brand, price, stock, tags)
2. Product list supports sorting by price, rating, name, newest
3. Pagination metadata correctly returned
4. Admin endpoints require valid Admin JWT
5. Swagger UI accessible at `/swagger`

### 5.5 Progress Notes

*Notes will be added as development progresses...*

---

## 6. Phase 4: Backend API — Comments & AI Summarizer

**Objective:** Implement comment system and Ollama-powered AI summarization.

### 6.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 4.1 | Create comment DTOs and validation | High | ⬜ | |
| 4.2 | Implement public comment list endpoint | High | ⬜ | FR-B05 |
| 4.3 | Implement comment creation endpoint | High | ⬜ | FR-B05 |
| 4.4 | Auto-update product rating/reviewCount on comment | High | ⬜ | |
| 4.5 | Implement admin comment moderation endpoints | High | ⬜ | FR-B05 |
| 4.6 | Create Ollama HTTP client service | High | ⬜ | FR-CS02 |
| 4.7 | Implement prompt template builder | High | ⬜ | Section 8.1 |
| 4.8 | Create summary generation service | High | ⬜ | FR-CS01 |
| 4.9 | Implement BackgroundService for stale summaries | High | ⬜ | Section 8.2 |
| 4.10 | Add Redis distributed lock for summary jobs | High | ⬜ | NFR-S04 |
| 4.11 | Implement exponential backoff retry logic | Medium | ⬜ | FR-CS04 |
| 4.12 | Create summary API endpoints | High | ⬜ | FR-B06 |
| 4.13 | Implement Ollama health check endpoint | Medium | ⬜ | FR-B06 |
| 4.14 | Mark summary as stale on new comment | High | ⬜ | FR-CS01 |
| 4.15 | Write unit tests for summarizer service | Medium | ⬜ | |
| 4.16 | Generate mock comments for testing | Medium | ⬜ | |

### 6.2 Comment Summarizer Flow Implementation

```
┌─────────────────────────────────────────────────────────────────┐
│                    SUMMARIZER IMPLEMENTATION                     │
├─────────────────────────────────────────────────────────────────┤
│ Step 1: Comment Created                                    ⬜   │
│   └── Mark CommentSummary.IsStale = true                       │
│                                                                  │
│ Step 2: Background Worker (60s interval)                   ⬜   │
│   └── Query stale summaries with min comment count             │
│                                                                  │
│ Step 3: Per Product (with Redis lock)                      ⬜   │
│   ├── Fetch approved comments                                   │
│   ├── Build prompt from template                                │
│   └── Call Ollama API                                           │
│                                                                  │
│ Step 4: On Success                                         ⬜   │
│   └── Upsert CommentSummary, set IsStale = false               │
│                                                                  │
│ Step 5: On Failure                                         ⬜   │
│   └── Log error, retry with backoff                             │
└─────────────────────────────────────────────────────────────────┘
```

### 6.3 API Endpoints Checklist

| Endpoint | Method | Auth | Status | Tests |
|----------|--------|------|--------|-------|
| `/api/v1/products/{productId}/comments` | GET | Public | ⬜ | ⬜ |
| `/api/v1/products/{productId}/comments` | POST | User | ⬜ | ⬜ |
| `/api/v1/admin/comments/{id}/approve` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/admin/comments/{id}/reject` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/products/{productId}/summary` | GET | Public | ⬜ | ⬜ |
| `/api/v1/admin/summaries/{productId}/generate` | POST | Admin | ⬜ | ⬜ |
| `/api/v1/admin/summaries/generate-stale` | POST | Admin | ⬜ | ⬜ |
| `/api/v1/admin/summaries/health` | GET | Admin | ⬜ | ⬜ |

### 6.4 Deliverables

- [ ] Comment CRUD operations working
- [ ] Ollama integration functional
- [ ] Background summarization job running
- [ ] Manual summarization triggers working
- [ ] Health check reporting Ollama status

### 6.5 Acceptance Criteria

1. New comment triggers `IsStale = true` on product's summary
2. Background job regenerates stale summaries every 60 seconds
3. Ollama health endpoint returns connectivity status
4. Summary includes model name and generation timestamp
5. Failed generations retry with exponential backoff

### 6.6 Progress Notes

*Notes will be added as development progresses...*

---

## 7. Phase 5: Backend API — Orders & Admin Features

**Objective:** Implement order management (mockup) and admin dashboard endpoints.

### 7.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 5.1 | Create order DTOs and validation | High | ⬜ | |
| 5.2 | Implement order creation endpoint | High | ⬜ | FR-B07 |
| 5.3 | Implement order history endpoints | High | ⬜ | FR-B07 |
| 5.4 | Implement admin order list endpoint | High | ⬜ | FR-B07 |
| 5.5 | Implement order status update endpoint | High | ⬜ | FR-B07 |
| 5.6 | Create user management DTOs | High | ⬜ | |
| 5.7 | Implement user profile endpoints | High | ⬜ | FR-B08 |
| 5.8 | Implement admin user management endpoints | High | ⬜ | FR-B08 |
| 5.9 | Implement admin dashboard stats endpoint | High | ⬜ | FR-B09 |
| 5.10 | Implement recent orders endpoint | Medium | ⬜ | FR-B09 |
| 5.11 | Implement stale summaries endpoint | Medium | ⬜ | FR-B09 |
| 5.12 | Add health check endpoints | High | ⬜ | NFR-R01 |
| 5.13 | Configure CORS for frontend origins | High | ⬜ | NFR-SEC05 |
| 5.14 | Write integration tests for order flow | Medium | ⬜ | |

### 7.2 API Endpoints Checklist

| Endpoint | Method | Auth | Status | Tests |
|----------|--------|------|--------|-------|
| `/api/v1/orders` | POST | User | ⬜ | ⬜ |
| `/api/v1/orders` | GET | User | ⬜ | ⬜ |
| `/api/v1/orders/{id}` | GET | User | ⬜ | ⬜ |
| `/api/v1/admin/orders` | GET | Admin | ⬜ | ⬜ |
| `/api/v1/admin/orders/{id}/status` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/users/me` | GET | User | ⬜ | ⬜ |
| `/api/v1/users/me` | PUT | User | ⬜ | ⬜ |
| `/api/v1/users/me/password` | PUT | User | ⬜ | ⬜ |
| `/api/v1/admin/users` | GET | Admin | ⬜ | ⬜ |
| `/api/v1/admin/users/{id}/role` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/admin/users/{id}/status` | PUT | Admin | ⬜ | ⬜ |
| `/api/v1/admin/dashboard/stats` | GET | Admin | ⬜ | ⬜ |
| `/api/v1/admin/dashboard/recent-orders` | GET | Admin | ⬜ | ⬜ |
| `/api/v1/admin/dashboard/stale-summaries` | GET | Admin | ⬜ | ⬜ |
| `/health` | GET | Public | ⬜ | ⬜ |
| `/health/ready` | GET | Public | ⬜ | ⬜ |

### 7.3 Deliverables

- [ ] Order creation and management working
- [ ] User profile management working
- [ ] Admin dashboard endpoints returning data
- [ ] Health endpoints configured
- [ ] Backend API feature-complete

### 7.4 Acceptance Criteria

1. Order creation from cart items works correctly
2. Order status transitions follow defined flow
3. Dashboard stats return accurate counts
4. Health endpoints return proper status
5. All admin endpoints protected by role check

### 7.5 Progress Notes

*Notes will be added as development progresses...*

---

## 8. Phase 6: Frontend Storefront — Core Features

**Objective:** Setup storefront React app with authentication, routing, and shared components.

### 8.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 6.1 | Configure Vite + React 19 project | High | ⬜ | |
| 6.2 | Setup Tailwind CSS 4 + shadcn/ui | High | ⬜ | |
| 6.3 | Configure React Router v7 | High | ⬜ | |
| 6.4 | Setup TanStack Query v5 | High | ⬜ | |
| 6.5 | Create API client with axios/fetch | High | ⬜ | |
| 6.6 | Generate TypeScript types from OpenAPI | Medium | ⬜ | NFR-M04 |
| 6.7 | Implement auth context/provider | High | ⬜ | |
| 6.8 | Create login page | High | ⬜ | FR-S01 |
| 6.9 | Create registration page | High | ⬜ | FR-S01 |
| 6.10 | Implement JWT token management | High | ⬜ | |
| 6.11 | Create protected route wrapper | High | ⬜ | |
| 6.12 | Build header/navigation component | High | ⬜ | |
| 6.13 | Build footer component | Medium | ⬜ | |
| 6.14 | Create breadcrumb component | Medium | ⬜ | FR-S03 |
| 6.15 | Setup localStorage for cart | High | ⬜ | FR-S06 |
| 6.16 | Create cart context/provider | High | ⬜ | |

### 8.2 Shared Components

| Component | Status | Notes |
|-----------|--------|-------|
| Button | ⬜ | Via shadcn/ui |
| Input | ⬜ | Via shadcn/ui |
| Card | ⬜ | Via shadcn/ui |
| Dialog/Modal | ⬜ | Via shadcn/ui |
| Select | ⬜ | Via shadcn/ui |
| Badge | ⬜ | For AI summary, stock status |
| Skeleton | ⬜ | Loading states |
| Toast | ⬜ | Notifications |
| StarRating | ⬜ | Custom component |
| PriceDisplay | ⬜ | Custom component |
| Pagination | ⬜ | Custom component |

### 8.3 Deliverables

- [ ] Storefront project configured with all dependencies
- [ ] Authentication flow working
- [ ] Shared components library ready
- [ ] Cart functionality working (localStorage)
- [ ] Basic routing structure in place

### 8.4 Acceptance Criteria

1. User can register and login
2. JWT tokens stored and refreshed automatically
3. Cart persists across page reloads
4. All shadcn/ui components styled consistently
5. Responsive layout works on mobile

### 8.5 Progress Notes

*Notes will be added as development progresses...*

---

## 9. Phase 7: Frontend Storefront — Product & Checkout

**Objective:** Implement product browsing, detail pages, and checkout flow.

### 9.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 7.1 | Create home page with featured content | High | ⬜ | FR-S02 |
| 7.2 | Implement recently viewed (localStorage) | Medium | ⬜ | FR-S02 |
| 7.3 | Create category listing page | High | ⬜ | FR-S03 |
| 7.4 | Create product listing page | High | ⬜ | FR-S04 |
| 7.5 | Build filter sidebar component | High | ⬜ | FR-S04 |
| 7.6 | Build sort dropdown component | High | ⬜ | FR-S04 |
| 7.7 | Create product card with AI summary badge | High | ⬜ | FR-S04 |
| 7.8 | Implement search with debounce | High | ⬜ | FR-S04 |
| 7.9 | Create product detail page | High | ⬜ | FR-S05 |
| 7.10 | Build image gallery with variant switching | High | ⬜ | FR-S05 |
| 7.11 | Build variant selector (color swatches) | High | ⬜ | FR-S05 |
| 7.12 | Create specs table component | Medium | ⬜ | FR-S05 |
| 7.13 | Build AI summary display component | High | ⬜ | FR-CS03 |
| 7.14 | Create comments section with pagination | High | ⬜ | FR-S05 |
| 7.15 | Build comment submission form | High | ⬜ | FR-S05 |
| 7.16 | Create shopping cart page | High | ⬜ | FR-S06 |
| 7.17 | Create checkout page | High | ⬜ | FR-S07 |
| 7.18 | Build shipping address form | High | ⬜ | FR-S07 |
| 7.19 | Create order confirmation page | High | ⬜ | FR-S07 |
| 7.20 | Add mockup payment banner | High | ⬜ | FR-S07 |
| 7.21 | Create user profile page | Medium | ⬜ | FR-S08 |
| 7.22 | Build order history component | Medium | ⬜ | FR-S08 |

### 9.2 Page Implementation Checklist

| Page | Route | Status | Responsive | Tests |
|------|-------|--------|------------|-------|
| Home | `/` | ⬜ | ⬜ | ⬜ |
| Categories | `/categories` | ⬜ | ⬜ | ⬜ |
| Category Products | `/categories/:slug` | ⬜ | ⬜ | ⬜ |
| Product Search | `/products` | ⬜ | ⬜ | ⬜ |
| Product Detail | `/products/:slug` | ⬜ | ⬜ | ⬜ |
| Cart | `/cart` | ⬜ | ⬜ | ⬜ |
| Checkout | `/checkout` | ⬜ | ⬜ | ⬜ |
| Order Confirmation | `/order/:id/confirmation` | ⬜ | ⬜ | ⬜ |
| Login | `/login` | ⬜ | ⬜ | ⬜ |
| Register | `/register` | ⬜ | ⬜ | ⬜ |
| Profile | `/profile` | ⬜ | ⬜ | ⬜ |
| Order History | `/orders` | ⬜ | ⬜ | ⬜ |
| Order Detail | `/orders/:id` | ⬜ | ⬜ | ⬜ |

### 9.3 Deliverables

- [ ] Complete product browsing experience
- [ ] Product detail page with AI summary
- [ ] Comment viewing and submission
- [ ] Full checkout flow (mockup)
- [ ] User profile and order history

### 9.4 Acceptance Criteria

1. Product filtering and sorting work correctly
2. AI summary displays with badge and metadata
3. Variant selection updates displayed image
4. Checkout creates order successfully
5. Mockup payment banner clearly visible
6. All pages responsive on mobile devices

### 9.5 Progress Notes

*Notes will be added as development progresses...*

---

## 10. Phase 8: Frontend Admin Panel

**Objective:** Build complete admin panel for managing the platform.

### 10.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 8.1 | Configure admin Vite + React project | High | ⬜ | |
| 8.2 | Setup admin-specific Tailwind config | High | ⬜ | |
| 8.3 | Create admin layout with sidebar | High | ⬜ | |
| 8.4 | Implement admin auth (Admin role check) | High | ⬜ | FR-A01 |
| 8.5 | Build dashboard page with stats cards | High | ⬜ | FR-A02 |
| 8.6 | Build recent orders widget | Medium | ⬜ | FR-A02 |
| 8.7 | Build stale summaries widget | Medium | ⬜ | FR-A02 |
| 8.8 | Create category management page | High | ⬜ | FR-A03 |
| 8.9 | Implement drag-and-drop category reorder | Medium | ⬜ | FR-A03 |
| 8.10 | Create product list page with filters | High | ⬜ | FR-A04 |
| 8.11 | Build product create/edit form | High | ⬜ | FR-A04 |
| 8.12 | Implement variant management inline | High | ⬜ | FR-A04 |
| 8.13 | Implement image upload with drag-drop | High | ⬜ | FR-A04 |
| 8.14 | Build specs key-value editor | Medium | ⬜ | FR-A04 |
| 8.15 | Build tag input component | Medium | ⬜ | FR-A04 |
| 8.16 | Create bulk import page | Medium | ⬜ | FR-A04 |
| 8.17 | Create comment moderation page | High | ⬜ | FR-A05 |
| 8.18 | Implement bulk approve/reject | Medium | ⬜ | FR-A05 |
| 8.19 | Create order management page | High | ⬜ | FR-A06 |
| 8.20 | Build order detail view | High | ⬜ | FR-A06 |
| 8.21 | Create user management page | High | ⬜ | FR-A07 |
| 8.22 | Build user detail view | Medium | ⬜ | FR-A07 |
| 8.23 | Create summary management page | High | ⬜ | FR-A08 |
| 8.24 | Implement manual re-summarization | High | ⬜ | FR-A08 |
| 8.25 | Build Ollama health status indicator | Medium | ⬜ | FR-A08 |

### 10.2 Admin Page Implementation Checklist

| Page | Route | Status | Responsive | Tests |
|------|-------|--------|------------|-------|
| Login | `/login` | ⬜ | ⬜ | ⬜ |
| Dashboard | `/` | ⬜ | ⬜ | ⬜ |
| Categories | `/categories` | ⬜ | ⬜ | ⬜ |
| Category Create/Edit | `/categories/:id?` | ⬜ | ⬜ | ⬜ |
| Products | `/products` | ⬜ | ⬜ | ⬜ |
| Product Create | `/products/new` | ⬜ | ⬜ | ⬜ |
| Product Edit | `/products/:id` | ⬜ | ⬜ | ⬜ |
| Product Import | `/products/import` | ⬜ | ⬜ | ⬜ |
| Comments | `/comments` | ⬜ | ⬜ | ⬜ |
| Orders | `/orders` | ⬜ | ⬜ | ⬜ |
| Order Detail | `/orders/:id` | ⬜ | ⬜ | ⬜ |
| Users | `/users` | ⬜ | ⬜ | ⬜ |
| User Detail | `/users/:id` | ⬜ | ⬜ | ⬜ |
| Summaries | `/summaries` | ⬜ | ⬜ | ⬜ |

### 10.3 Deliverables

- [ ] Admin panel with all management features
- [ ] Product CRUD with all nested entities
- [ ] Comment moderation with bulk actions
- [ ] Order status management
- [ ] AI summary management and monitoring

### 10.4 Acceptance Criteria

1. Only Admin users can access the panel
2. All CRUD operations work correctly
3. Bulk operations complete successfully
4. Ollama health status displayed accurately
5. Manual summary regeneration works

### 10.5 Progress Notes

*Notes will be added as development progresses...*

---

## 11. Phase 9: Integration, Testing & Polish

**Objective:** End-to-end testing, bug fixes, performance optimization, and UI polish.

### 11.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 9.1 | End-to-end flow testing (registration to order) | High | ⬜ | |
| 9.2 | End-to-end admin flow testing | High | ⬜ | |
| 9.3 | Test comment submission → summary regeneration | High | ⬜ | |
| 9.4 | Test Ollama failure scenarios | High | ⬜ | |
| 9.5 | Test refresh token rotation edge cases | High | ⬜ | |
| 9.6 | Performance testing with load simulation | Medium | ⬜ | NFR-P05 |
| 9.7 | Optimize slow database queries | High | ⬜ | NFR-P03 |
| 9.8 | Add database indexes for common queries | High | ⬜ | |
| 9.9 | Review and optimize Redis caching | Medium | ⬜ | |
| 9.10 | Fix UI/UX issues discovered in testing | High | ⬜ | |
| 9.11 | Ensure consistent error handling/display | High | ⬜ | |
| 9.12 | Mobile responsiveness testing | High | ⬜ | |
| 9.13 | Cross-browser testing | Medium | ⬜ | |
| 9.14 | Accessibility audit (basic) | Medium | ⬜ | |
| 9.15 | Security review (auth, CORS, input validation) | High | ⬜ | |
| 9.16 | Write additional unit/integration tests | Medium | ⬜ | Target 70% coverage |
| 9.17 | Frontend component tests (Vitest) | Medium | ⬜ | NFR-T03 |

### 11.2 Test Coverage Tracking

| Module | Unit Tests | Integration Tests | Target Coverage | Current |
|--------|-----------|-------------------|-----------------|---------|
| Auth Service | ⬜ | ⬜ | 80% | - |
| Product Service | ⬜ | ⬜ | 70% | - |
| Comment Service | ⬜ | ⬜ | 70% | - |
| Summarizer Service | ⬜ | ⬜ | 70% | - |
| Order Service | ⬜ | ⬜ | 70% | - |
| Storefront Components | ⬜ | - | 50% | - |
| Admin Components | ⬜ | - | 50% | - |

### 11.3 Bug Tracking

| ID | Description | Severity | Status | Notes |
|----|-------------|----------|--------|-------|
| - | *Bugs will be logged here* | - | - | - |

### 11.4 Deliverables

- [ ] All critical bugs fixed
- [ ] Performance targets met
- [ ] Test coverage targets achieved
- [ ] Mobile/responsive issues resolved
- [ ] Security review completed

### 11.5 Acceptance Criteria

1. Complete user flow works without errors
2. API response times within NFR targets
3. No critical or high-severity bugs outstanding
4. Test coverage ≥ 70% for backend business logic
5. All security checks pass

### 11.6 Progress Notes

*Notes will be added as development progresses...*

---

## 12. Phase 10: Documentation & Deployment Preparation

**Objective:** Complete documentation and prepare for production deployment.

### 12.1 Tasks

| ID | Task | Priority | Status | Notes |
|----|------|----------|--------|-------|
| 10.1 | Create comprehensive README.md | High | ⬜ | |
| 10.2 | Document local development setup | High | ⬜ | |
| 10.3 | Document environment variables | High | ⬜ | |
| 10.4 | Document Docker Compose usage | High | ⬜ | |
| 10.5 | Document database migration procedures | High | ⬜ | |
| 10.6 | Document seeding procedures | Medium | ⬜ | |
| 10.7 | Create API documentation beyond Swagger | Medium | ⬜ | |
| 10.8 | Document Ollama setup requirements | High | ⬜ | |
| 10.9 | Create troubleshooting guide | Medium | ⬜ | |
| 10.10 | Create docker-compose.prod.yml | High | ⬜ | |
| 10.11 | Configure production Nginx settings | High | ⬜ | |
| 10.12 | Review and harden security settings | High | ⬜ | |
| 10.13 | Create deployment checklist | High | ⬜ | |
| 10.14 | Configure structured logging (Serilog) | Medium | ⬜ | NFR-O01 |
| 10.15 | Document backup/restore procedures | Medium | ⬜ | |
| 10.16 | Final code review and cleanup | High | ⬜ | |

### 12.2 Documentation Checklist

| Document | Location | Status |
|----------|----------|--------|
| Main README | `/README.md` | ⬜ |
| Backend README | `/src/backend/README.md` | ⬜ |
| Storefront README | `/src/frontend/storefront/README.md` | ⬜ |
| Admin README | `/src/frontend/admin/README.md` | ⬜ |
| API Documentation | `/docs/api.md` | ⬜ |
| Deployment Guide | `/docs/deployment.md` | ⬜ |
| Ollama Integration Guide | `/docs/ollama-setup.md` | ⬜ |
| Troubleshooting | `/docs/troubleshooting.md` | ⬜ |

### 12.3 Deliverables

- [ ] Complete documentation suite
- [ ] Production-ready Docker configuration
- [ ] Deployment checklist ready
- [ ] Code cleaned and reviewed
- [ ] Project ready for handoff

### 12.4 Acceptance Criteria

1. New developer can set up environment following docs
2. Production deployment steps clearly documented
3. Ollama integration requirements specified
4. All code follows established patterns
5. No commented-out or debug code remaining

### 12.5 Progress Notes

*Notes will be added as development progresses...*

---

## 13. Risk Register

| ID | Risk | Impact | Probability | Mitigation | Status |
|----|------|--------|-------------|------------|--------|
| R1 | Ollama unavailability during development | Medium | Medium | Use mock responses for testing; implement graceful degradation | ⬜ Open |
| R2 | .NET 10 / React 19 breaking changes | High | Low | Pin dependencies; review release notes | ⬜ Open |
| R3 | Complex product entity relationships | Medium | Medium | Thorough EF Core configuration testing | ⬜ Open |
| R4 | Performance issues with large datasets | Medium | Medium | Implement caching early; optimize queries | ⬜ Open |
| R5 | JWT token security vulnerabilities | High | Low | Follow OWASP guidelines; security review | ⬜ Open |
| R6 | AI summary quality inconsistency | Medium | Medium | Refine prompts; allow admin override | ⬜ Open |
| R7 | Frontend/backend API contract drift | Medium | Medium | Generate types from OpenAPI spec | ⬜ Open |

---

## 14. Progress Tracking

### 14.1 Overall Progress

```
Phase 1:  [░░░░░░░░░░] 0%
Phase 2:  [░░░░░░░░░░] 0%
Phase 3:  [░░░░░░░░░░] 0%
Phase 4:  [░░░░░░░░░░] 0%
Phase 5:  [░░░░░░░░░░] 0%
Phase 6:  [░░░░░░░░░░] 0%
Phase 7:  [░░░░░░░░░░] 0%
Phase 8:  [░░░░░░░░░░] 0%
Phase 9:  [░░░░░░░░░░] 0%
Phase 10: [░░░░░░░░░░] 0%
──────────────────────────
TOTAL:    [░░░░░░░░░░] 0%
```

### 14.2 Milestone Summary

| Milestone | Target Date | Actual Date | Status |
|-----------|-------------|-------------|--------|
| Project Foundation Complete | TBD | - | ⬜ |
| Backend API Feature Complete | TBD | - | ⬜ |
| Storefront Feature Complete | TBD | - | ⬜ |
| Admin Panel Feature Complete | TBD | - | ⬜ |
| Integration & Testing Complete | TBD | - | ⬜ |
| Production Ready | TBD | - | ⬜ |

---

## 15. Change Log

| Date | Version | Author | Changes |
|------|---------|--------|---------|
| 2026-02-11 | 1.0 | Claude | Initial development plan created |

---

## Legend

| Symbol | Meaning |
|--------|---------|
| ⬜ | Not Started |
| 🟡 | In Progress |
| ✅ | Completed |
| ❌ | Blocked |
| ⏸️ | On Hold |

---

*This document is a living document and will be updated throughout the development process.*
