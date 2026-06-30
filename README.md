# High-Throughput E-Commerce Engine

## Overview

This project is a proof-of-concept high-throughput e-commerce engine built with **ABP Framework**, **.NET**, **Angular**, **PostgreSQL**, and **Redis**.

It demonstrates the main requirements from the technical assessment:

- Bulk product ingestion from CSV files.
- Atomic inventory reservation during concurrent order creation.
- Redis-backed autocomplete for search-as-you-type.
- Angular RxJS handling for reactive search input.
- Angular Signals for global cart state.
- Recovery of expired unpaid orders.
- Integration testing for concurrent order safety.

---

## Tech Stack

- ABP Framework
- .NET
- Entity Framework Core
- PostgreSQL
- Redis
- Angular
- RxJS
- Angular Signals
- Docker Compose
- Swagger / OpenAPI
- xUnit / Shouldly

---

## Running with Docker Compose

The project includes a `docker-compose.yml` file for running the full solution:

- PostgreSQL
- Redis
- Database migrator
- Backend API
- Angular frontend

### 1. Create environment file

Copy the example environment file:

```bash
cp .env.example .env
```

On Windows PowerShell:

```powershell
Copy-Item .env.example .env
```

### 2. Start all services

```bash
docker compose up --build
```

Or run in detached mode:

```bash
docker compose up -d --build
```

### 3. Service URLs

```txt
Angular:  http://localhost:4200
API:      http://localhost:8080
Swagger:  http://localhost:8080/swagger
Postgres: localhost:5432
Redis:    localhost:6379
```

The `db-migrator` service runs database migrations before the API starts.

### 4. Stop services

```bash
docker compose down
```

To also remove database volume data:

```bash
docker compose down -v
```

---

## Running Locally Without Docker for API/Angular

You can also run PostgreSQL and Redis with Docker, then run the backend and frontend locally.

### 1. Start infrastructure

```bash
docker compose up -d postgres redis
```

### 2. Build backend

```bash
dotnet restore
dotnet build
```

### 3. Run database migrations

```bash
dotnet run --project ./src/Apptech.Assessment.DbMigrator
```

### 4. Run API

```bash
dotnet run --project ./src/Apptech.Assessment.HttpApi.Host
```

Open Swagger using the port shown in the API console:

```txt
https://localhost:<backend-port>/swagger
```

### 5. Run Angular

```bash
cd angular
npm install
npm start
```

Or with Yarn:

```bash
cd angular
yarn install
yarn start
```

Open:

```txt
http://localhost:4200
```

Make sure the Angular environment backend URL points to the running API.

---

## Environment Configuration

The solution uses `.env` for Docker Compose variables.

Example values:

```env
POSTGRES_DB=Assessment
POSTGRES_USER=root
POSTGRES_PASSWORD=myPassword
POSTGRES_PORT=5432

REDIS_PORT=6379

API_HTTP_PORT=8080
ANGULAR_PORT=4200

API_SELF_URL=http://localhost:8080
ANGULAR_URL=http://localhost:4200
AUTHORITY=http://localhost:8080
REQUIRE_HTTPS_METADATA=false
```

Backend connection string example:

```json
"ConnectionStrings": {
  "Default": "Host=localhost;Port=5432;Database=Assessment;Username=root;Password=myPassword;"
}
```

Redis configuration example:

```json
"Redis": {
  "IsEnabled": "true",
  "Configuration": "localhost:6379"
}
```

---

## Main Features

## Problem A: High-Volume CSV Product Import

Implemented a CSV import endpoint that processes files without loading the full file into memory.

### Endpoint

```txt
POST /api/products/import-csv
```

### Example request

```bash
curl.exe -k -X POST "https://localhost:<backend-port>/api/products/import-csv" ^
  -H "accept: application/json" ^
  -F "file=@products.csv;type=text/csv"
```

### Expected CSV format

```csv
Name,Description,Price,StockQuantity
Keyboard,Mechanical keyboard,50.99,100
Mouse,Wireless mouse,25.50,200
Monitor,27 inch monitor,199.99,50
```

### Implementation notes

- The uploaded file is read as a stream.
- Rows are processed incrementally.
- Products are inserted in batches.
- EF Core tracking is cleared after each batch.
- Imported products are indexed into Redis for autocomplete.

### Production note

The proof-of-concept parser uses simple CSV parsing. A production version should use a robust CSV parser such as `CsvHelper` to handle quoted fields, escaped characters, commas in descriptions, and malformed rows.

---

## Problem B: Atomic Inventory Management

Order creation reserves product stock atomically.

### Flow

1. Validate basket input.
2. Normalize duplicate product entries.
3. Load product pricing information.
4. Reserve stock for each product.
5. Fail the entire order if any product has insufficient stock.
6. Create the order only after all stock reservations succeed.

### Atomic reservation logic

The product repository performs a database-side conditional update equivalent to:

```txt
UPDATE Products
SET StockQuantity = StockQuantity - requestedQuantity
WHERE Id = productId
AND StockQuantity >= requestedQuantity
```

If the update affects zero rows, the product does not have enough stock and order creation fails.

This prevents stock from dropping below zero under concurrent flash-sale requests.

---

## Problem C: Redis-Backed Autocomplete

Autocomplete is implemented using Redis prefix indexing.

### Behavior

- Queries shorter than 3 characters return no results.
- Queries with 3 or more characters search Redis.
- Imported products are indexed during CSV import.

Example product:

```txt
Keyboard
```

Generated prefixes:

```txt
key
keyb
keybo
keyboa
keyboar
keyboard
```

Redis keys follow this format:

```txt
autocomplete:products:{prefix}
```

### Endpoint

ABP exposes the autocomplete application service through auto API controllers.

Check Swagger for the exact route. It appears under `/api/app/...`.

Example route:

```txt
GET /api/app/product-autocomplete?query=key
```

---

## Problem D: Angular Reactive Search with RxJS

The Angular search input uses RxJS to handle fast typing and network latency.

The search stream uses:

- `filter`
- `debounceTime`
- `distinctUntilChanged`
- `switchMap`
- `catchError`

This ensures:

- no request is sent before 3 characters
- rapid typing does not flood the backend
- stale requests are cancelled
- failed requests do not break the UI stream

---

## Problem E: Angular Signals Cart State

The frontend uses Angular Signals for global cart state.

Implemented state includes:

- cart items
- order status
- total quantity
- total price
- empty cart state

The cart store uses writable signals for state and computed signals for derived values.

Example:

```ts
private readonly _items = signal<CartItem[]>([]);
readonly items = this._items.asReadonly();

readonly totalQuantity = computed(() =>
  this._items().reduce((sum, item) => sum + item.quantity, 0)
);
```

### RxJS vs Signals decision

- RxJS is used for asynchronous event streams such as search input and HTTP requests.
- Signals are used for synchronous UI state such as cart items, totals, and order status.

Signals provide fine-grained reactivity, so only consumers of changed signal values update.

---

## Problem F: Expired Order Recovery

Orders are created as pending payment and reserve stock immediately.

A background worker periodically:

1. Finds expired pending-payment orders.
2. Restores reserved stock.
3. Marks the order as expired.

This prevents inventory from remaining stuck when an order is created but never paid.

---

## Tests

Run all tests:

```bash
dotnet test
```

Important integration test:

```txt
OrderServiceIntegrationTests
```

Test method:

```txt
CreateAsync_WhenTwoConcurrentOrdersRequestLastRemainingItem_ShouldCreateExactlyOneOrderAndRejectOne
```

This test verifies:

- initial stock is 1
- two concurrent order requests are executed
- exactly one order succeeds
- exactly one order fails
- final stock remains 0

This proves the system prevents overselling the last remaining item.

---

## Architecture Notes

The solution keeps ABP's layered structure and applies a vertical-slice organization inside the application layer.

Important folders:

```txt
Domain/
  Products/
  Orders/

EntityFrameworkCore/
  Products/
  Orders/

Application/
  Features/
    Products/
      ImportProducts/
      Autocomplete/
    Orders/
      CreateOrder/
      Recovery/
```

### Repository usage

Application services do not directly use `DbContext`.

Instead, persistence is accessed through repository abstractions:

```txt
Application Service
    -> Repository Interface
        -> EF Core Repository Implementation
```

### Redis separation

Redis autocomplete indexing is separated from product persistence because Redis is used as a search/cache index, while PostgreSQL remains the source of truth.

---

## Known Limitations

- CSV parsing is simplified for the proof of concept.
- EF Core batch inserts are used instead of database-native bulk import.
- Redis indexing happens after database insertion; PostgreSQL remains the source of truth.
- Real payment processing is not implemented.
- Real-time order status updates are represented in frontend state; production could use SignalR, WebSocket, or polling.

---