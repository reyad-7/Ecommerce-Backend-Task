# E-Commerce Task API

A clean, production-ready e-commerce backend API built with .NET 8, featuring product catalog management, JWT authentication, order processing, and Redis caching.

---

##  Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Features](#features)
- [Getting Started](#getting-started)
- [API Documentation](#api-documentation)
- [Design Decisions](#design-decisions)
- [Time Spent](#time-spent)

---

##  Overview

This is a RESTful API for a minimal e-commerce system that allows:
- **Public users** to browse products (no authentication required)
- **Authenticated users** to place orders and view their order history
- **Ownership enforcement** ensuring users can only access their own orders
- **Redis caching** for optimized product catalog performance

---

## 🛠️ Tech Stack

- **.NET 8** - Web API framework
- **ASP.NET Core Identity** - User management
- **JWT Bearer Authentication** - Secure API endpoints
- **Entity Framework Core 8** - ORM for data access
- **SQL Server** - Relational database
- **Redis** - Distributed caching
- **Swagger/OpenAPI** - API documentation

---

##  Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:
```
Solution Structure:
│
├── API (E-Commerce)              # Presentation Layer
│   ├── Controllers               # HTTP endpoints
│   ├── Middleware                # Global exception handling
│   └── Program.cs                # Application entry point
│
├── Application                   # Business Logic Layer
│   ├── Services                  # Service implementations
│   │   ├── CategoryService.cs
│   │   ├── ProductService.cs
│   │   ├── OrderService.cs
│   │   └── AuthService.cs
│   └── (DTOs in Domain layer)
│
├── Domain                        # Core Layer
│   ├── Entities                  # Business entities
│   │   ├── Models                # Entity classes
│   │   ├── Enums                 # OrderStatus enum
│   │   └── GeneralResponse       # Response wrapper
│   ├── DTOs                      # Data Transfer Objects
│   │   ├── Auth
│   │   ├── Category
│   │   ├── Product
│   │   └── Order
│   └── Interfaces                # Service & Repository contracts
│
└── Infrastructure                # Data Access Layer
    ├── Data                      # DbContext & Migrations
    ├── Repository                # Repository pattern implementation
    ├── Configuration             # EF Core entity configurations
    ├── Services                  # Infrastructure services (Redis)
    └── DependencyInjection.cs    # Service registration
```

### **Dependency Flow**
```
API → Application → Domain ← Infrastructure
     (Services)   (Entities,    (Data Access,
                   Interfaces)    Repositories)
```

**Key Principle:** All layers depend on Domain (interfaces), but Domain depends on nothing.

---

## ✨ Features

### **1. Product Catalog (Public)**
- ✅ List all products with pagination
- ✅ Get product by ID
- ✅ Get product by name
- ✅ Filter by category
- ✅ Active products only
- ✅ Redis caching (30-minute TTL)

### **2. Authentication & Authorization**
- ✅ User registration with validation
- ✅ Login with email or username
- ✅ JWT token generation (90-day expiration)
- ✅ Password requirements (uppercase, lowercase, digit, 6+ chars)
- ✅ Automatic "Customer" role assignment

### **3. Order Management (Protected)**
- ✅ Create orders with multiple items
- ✅ Stock validation and automatic deduction
- ✅ View user's own orders (ownership enforced)
- ✅ Cancel pending orders (restores stock)
- ✅ Order status tracking
- ✅ Price and product name snapshots

### **4. Categories**
- ✅ CRUD operations for categories
- ✅ Prevent deletion of categories with products
- ✅ Unique name validation

### **5. Redis Caching**
- ✅ Product list caching (pagination-aware)
- ✅ Product details caching
- ✅ Configurable TTL (default: 30 minutes)
- ✅ Graceful degradation (falls back to DB)

### **6. Error Handling**
- ✅ Global exception handling middleware
- ✅ Consistent error response format
- ✅ DTO validation with detailed messages
- ✅ No stack trace leakage in production

---

## 🚀 Getting Started

### **Prerequisites**

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express/LocalDB/Developer)
- [Redis](https://redis.io/download) (or Docker)
- IDE: Visual Studio 2022 or VS Code

---

### **1. Clone the Repository**
```bash
git clone https://github.com/reyad-7/Ecommerce-Backend-Task.git
Ecommerce-Backend-Task
```

---

### **2. Configure Database Connection**

Update `API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WaffarXEcommerceDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Connection String Options:**

| Environment | Connection String |
|-------------|-------------------|
| **LocalDB** (default) | `Server=(localdb)\\mssqllocaldb;Database=WaffarXEcommerceDB;Trusted_Connection=True;` |
| **SQL Server Express** | `Server=.\\SQLEXPRESS;Database=WaffarXEcommerceDB;Trusted_Connection=True;` |
| **Full SQL Server** | `Server=localhost;Database=WaffarXEcommerceDB;Trusted_Connection=True;` |
| **Azure SQL** | `Server=tcp:yourserver.database.windows.net,1433;Database=WaffarXEcommerceDB;User ID=username;Password=password;` |

---

### **3. Configure JWT Settings**

Update `API/appsettings.json`:
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHereMustBeAtLeast32CharactersLong!",
    "Issuer": "WaffarXEcommerce",
    "Audience": "WaffarXEcommerceUsers",
    "DurationInDays": 90
  }
}
```


---

### **4. Configure Redis**

Update `API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  },
  "Redis": {
    "InstanceName": "WaffarXEcommerce:",
    "DefaultTTLMinutes": 30
  }
}
```

**Start Redis:**

**Option 1: Docker (Recommended)**
```bash
docker run -d --name redis -p 6379:6379 redis:latest
```

**Option 2: Local Installation**
- **Windows:** Download from [GitHub](https://github.com/microsoftarchive/redis/releases)
- **macOS:** `brew install redis && brew services start redis`
- **Linux:** `sudo apt-get install redis-server && sudo service redis-server start`

**Verify Redis is running:**
```bash
redis-cli ping
# Should return: PONG
```

---

### **5. Run Database Migrations**

Navigate to the solution folder and run:
```bash
# Using .NET CLI
dotnet ef database update --project Infrastructure --startup-project API

# OR using Package Manager Console in Visual Studio
Update-Database
```

This will:
- Create the database
- Apply all migrations
- Seed the "Customer" role

---

### **6. Run the Application**
```bash
cd API
dotnet run
```

The API will start at:
- **HTTPS:** `https://localhost:7001`
- **HTTP:** `http://localhost:5001`
- **Swagger:** `https://localhost:7001/swagger`

---

## 📚 API Documentation

### **Base URL**
```
https://localhost:7001/api
```

### **Swagger UI**
Access interactive API documentation at:
```
https://localhost:{your local host port}/swagger
```

---

### **Quick Reference**

#### **Authentication Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/auth/register` | Register new user | ❌ No |
| POST | `/auth/login` | Login and get JWT token | ❌ No |

#### **Product Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/product?pageNumber=1&pageSize=10` | Get all products (paginated) | ❌ No |
| GET | `/product/{id}` | Get product by ID | ❌ No |
| GET | `/product/by-name/{name}` | Get product by name | ❌ No |
| GET | `/product/category/{categoryId}` | Get products by category | ❌ No |
| GET | `/product/active` | Get active products only | ❌ No |
| POST | `/product` | Create product | ❌ No* |
| PUT | `/product/{id}` | Update product | ❌ No* |
| DELETE | `/product/{id}` | Soft delete product | ❌ No* |
| GET | `/product/{id}/check-stock?quantity=5` | Check stock availability | ❌ No |

*Can be protected with `[Authorize]` in production

#### **Category Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/category?pageNumber=1&pageSize=10` | Get all categories (paginated) | ❌ No |
| GET | `/category/{id}` | Get category by ID | ❌ No |
| POST | `/category` | Create category | ❌ No* |
| PUT | `/category/{id}` | Update category | ❌ No* |
| DELETE | `/category/{id}` | Delete category | ❌ No* |

#### **Order Endpoints**

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/order` | Create new order | ✅ Yes |
| GET | `/order/my-orders?pageNumber=1&pageSize=10` | Get user's orders | ✅ Yes |
| GET | `/order/{orderId}` | Get order by ID | ✅ Yes |
| GET | `/order/by-number/{orderNumber}` | Get order by number | ✅ Yes |
| POST | `/order/{orderId}/cancel` | Cancel order | ✅ Yes |
| PUT | `/order/{orderId}/status` | Update order status | ✅ Yes |

---

### **Example Requests**

#### **1. Register User**
```http
POST /api/auth/register
Content-Type: application/json

{
  "userName": "john_doe",
  "email": "john@example.com",
  "password": "Password123",
  "confirmPassword": "Password123"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Registration successful",
  "data": {
    "userId": "abc-123",
    "userName": "john_doe",
    "email": "john@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresAt": "2024-05-15T10:00:00Z"
  }
}
```

---

#### **2. Login**
```http
POST /api/auth/login
Content-Type: application/json

{
  "emailOrUsername": "john@example.com",
  "password": "Password123"
}
```

**Response:** Same as registration

---

#### **3. Get Products (Cached)**
```http
GET /api/product?pageNumber=1&pageSize=10
```

**Response:**
```json
{
  "success": true,
  "message": "Retrieved 10 products from cache (Page 1 of 5)",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "iPhone 15 Pro",
        "description": "Latest Apple smartphone",
        "price": 999.99,
        "stockQuantity": 50,
        "categoryId": 1,
        "categoryName": "Electronics",
        "isActive": true,
        "createdAt": "2024-02-14T10:00:00Z"
      }
    ],
    "totalCount": 50,
    "pageNumber": 1,
    "pageSize": 10,
    "totalPages": 5,
    "hasPreviousPage": false,
    "hasNextPage": true
  }
}
```

---

#### **4. Create Order (Protected)**
```http
POST /api/order
Authorization: Bearer {your_jwt_token}
Content-Type: application/json

{
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 3,
      "quantity": 1
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "id": 1,
    "orderNumber": "ORD-20240214120530-4567",
    "totalAmount": 2999.97,
    "orderStatus": 1,
    "orderStatusName": "Pending",
    "orderItems": [
      {
        "id": 1,
        "productId": 1,
        "productName": "iPhone 15 Pro",
        "quantity": 2,
        "unitPrice": 999.99,
        "totalPrice": 1999.98
      }
    ]
  }
}
```

---

## 🔐 Authentication & Authorization

### **How Authentication Works**

1. **Registration/Login:** User provides credentials
2. **Token Generation:** Server generates JWT with claims (userId, username, email, role)
3. **Token Storage:** Client stores token (localStorage, cookie, etc.)
4. **Protected Requests:** Client sends token in `Authorization` header
5. **Validation:** Server validates token signature and expiration

### **JWT Token Structure**
```json
{
  "nameid": "user-id-123",
  "unique_name": "john_doe",
  "email": "john@example.com",
  "role": "Customer",
  "nbf": 1707901200,
  "exp": 1715677200,
  "iat": 1707901200
}
```

### **Ownership Enforcement**

**Problem:** Users must only access their own orders.

**Solution:**
1. Extract `userId` from JWT token claims (`ClaimTypes.NameIdentifier`)
2. Filter database queries by `userId`
3. Return `403 Forbidden` if user tries to access another user's order

**Implementation:**
```csharp
// In OrderController
var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

// In OrderService
var order = await _unitOfWork.Orders.FindAsync(o => o.Id == orderId);
if (order.UserId != userId)
{
    return Forbidden("You are not authorized to view this order");
}
```

---

## 💾 Redis Caching Strategy

### **What is Cached**

| Endpoint | Cache Key Pattern | TTL |
|----------|-------------------|-----|
| GET `/product` | `WaffarXEcommerce:products:list:page_{N}:size_{M}` | 30 min |
| GET `/product/{id}` | `WaffarXEcommerce:products:detail:{id}` | 30 min |

### **Cache Key Strategy**

**Format:** `{InstanceName}:{resource}:{operation}:{parameters}`

**Examples:**
- `WaffarXEcommerce:products:list:page_1:size_10`
- `WaffarXEcommerce:products:list:page_2:size_20`
- `WaffarXEcommerce:products:detail:42`

**Why This Strategy?**
1. **Instance Isolation:** Prefix prevents collisions with other apps
2. **Pagination Awareness:** Each page is cached separately
3. **Granular Control:** Can invalidate specific patterns (e.g., all lists)
4. **Human Readable:** Easy to debug and monitor

### **Cache-Aside Pattern**
```
1. Request arrives
2. Check Redis cache
3. If HIT: Return cached data
4. If MISS: Query database → Cache result → Return data
```

### **TTL (Time-To-Live)**

- **Default:** 30 minutes
- **Configurable:** `appsettings.json` → `Redis:DefaultTTLMinutes`
- **Rationale:** Balances freshness vs. performance

### **No Active Invalidation**

As per requirements, only TTL-based expiration is used. No manual cache invalidation on updates.

**Future Enhancement:** Invalidate cache when products are created/updated/deleted.

### **Performance Impact**

- **First request:** ~200-500ms (database)
- **Cached requests:** ~5-20ms (Redis)
- **Database load reduction:** ~80-90%

---

## 🎨 Design Decisions

### **1. Clean Architecture**

**Why?**
- Clear separation of concerns
- Testable business logic
- Flexible infrastructure (easy to swap EF Core, Redis, etc.)
- Maintainable codebase

**Trade-off:** More initial setup, but pays off in long run.

---

### **2. Repository + Unit of Work Pattern**

**Why?**
- Abstraction over data access
- Centralized transaction management
- Easier to mock for testing

**Trade-off:** Extra layer, but improves maintainability.

---

### **3. GeneralResponse Wrapper**

**Why?**
- Consistent API response format
- Client-friendly error handling
- Success/failure indication

**Example:**
```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... }
}
```

---

### **4. DTOs (Data Transfer Objects)**

**Why?**
- Separate internal entities from API contracts
- Validation at API boundary
- Control over what data is exposed

**Location:** `Domain/DTOs` (not Application) to avoid circular dependencies.

---

### **5. Pagination**

**Implementation:**
```csharp
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
```

**Why?**
- Prevents loading entire datasets
- Client has metadata for pagination UI
- Reusable across endpoints

---

### **6. Order Stock Management**

**Business Rules:**
1. Validate product exists and is active
2. Check sufficient stock before order creation
3. Deduct stock atomically when order is created
4. Restore stock if order is cancelled (only Pending orders)
5. Snapshot price and product name at order time

**Why Snapshot?**
- Preserves historical data even if product is updated/deleted
- Order remains accurate representation of what was purchased

---

### **7. Soft Delete for Products**

**Implementation:**
- `IsActive` flag instead of hard delete
- Soft delete: Set `IsActive = false`
- Hard delete: Only if product has no order history

**Why?**
- Preserves referential integrity
- Historical orders remain valid
- Can reactivate if needed

---

### **8. Global Exception Handling**

**Implementation:**
```csharp
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
```

**Why?**
- Centralized error handling
- Prevents stack trace leakage
- Consistent error responses
- Logging at single point

---

## ⏱️ Time Spent

### **Estimated Time**
**15-20 hours**

### **Actual Time Spent**
**18 hours**

**Breakdown:**
- **Project Setup & Architecture:** 2 hours
- **Domain Layer (Entities, Interfaces, DTOs):** 2 hours
- **Infrastructure (DbContext, Repository, Migrations):** 2.5 hours
- **Authentication (JWT, Identity):** 2 hours
- **Services (Category, Product, Order):** 4 hours
- **Controllers & Validation:** 2 hours
- **Redis Caching:** 1.5 hours
- **Testing & Debugging:** 1.5 hours
- **Documentation:** 30 minutes


---


##  Author

**Mohamed Reyad**
- GitHub: [@reyad-7](https://github.com/reyad-7)
- Email: mohamed.a.reyad@gmail.com


---
```
