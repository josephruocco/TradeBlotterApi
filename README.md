# TradeBlotterApi

Production-style **Repo Pre-Trade Analytics** Web API built as a portfolio project (focus: software architecture, testing discipline, and clean interfaces across modules).

This repository is organized as a multi-project solution with clear separation between API, domain logic, infrastructure/persistence, and tests.

## What it does

TradeBlotterApi provides REST endpoints for repo / fixed-income trade analytics and booking — tooling that would traditionally live in spreadsheets, including:

- Repo funding economics using the **ACT/360** day-count convention
- **Haircut-adjusted** cash proceeds
- Repo interest and total repayment cashflows (pre-trade quoting)
- Full trade lifecycle: book, amend, soft-cancel
- Net position aggregation by symbol
- Structured, audit-friendly persistence (soft-cancel instead of hard delete)

## Tech stack

- **ASP.NET Core** (Web API) on **.NET 10**
- **C#**
- **PostgreSQL** via **EF Core** (Npgsql), code-first migrations
- **Docker** (containerized Postgres)
- **Swagger / OpenAPI**
- **Serilog** structured logging
- **xUnit** test project included in the solution

## Solution structure

- `TradeBlotterApi.Api/` — Web API (controllers, request/response models, OpenAPI, startup)
- `TradeBlotterApi.Domain/` — Core pricing/analytics logic and models (framework-free, domain-first design)
- `TradeBlotterApi.Infrastructure/` — Persistence + integrations (EF Core / PostgreSQL)
- `TradeBlotterApi.Tests/` — Unit/component tests for core components
- `TradeBlotterApi.sln` — Solution file

The domain layer has no dependency on EF Core or ASP.NET; dependencies point inward toward the core pricing logic, keeping it independently unit-testable.

## API surface

### Trades — `/api/trades`
- `GET /api/trades/{id}` — fetch a single trade
- `GET /api/trades` — filtered, paged list (filters: desk, trader, product, symbol, from/to date range)
- `POST /api/trades` — book a trade
- `PUT /api/trades/{id}` — partial amend (blocked on cancelled trades)
- `DELETE /api/trades/{id}` — soft-cancel (idempotent)

### Repo — `/api/repo`
- `POST /api/repo/quote` — price repo cashflows (no persistence)
- `POST /api/repo/book` — price and book a confirmed repo trade
- `GET /api/repo/positions` — net positions aggregated by symbol

### Health
- `GET /health` — liveness
- `GET /health/ready` — readiness (verifies database connectivity)

## Running locally

### Option A: Docker for Postgres + .NET for the API

1. Start PostgreSQL:
   ```bash
   docker compose up -d
   ```
2. Restore, build, and run the API (migrations are applied automatically on startup):
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project TradeBlotterApi.Api
   ```
3. Open Swagger UI (Development) at `https://localhost:7023/swagger` (or `http://localhost:5125/swagger`).

The default connection string (`TradeBlotterApi.Api/appsettings.json`) targets the Docker Postgres
instance defined in `docker-compose.yml`:
`Host=localhost;Port=5432;Database=tradeblotter;Username=blotter;Password=blotter`.

## Running the tests

```bash
dotnet test
```

Tests use xUnit with the EF Core in-memory provider, covering the repo cashflow math
(`RepoCalculator`) and controller behavior (booking, amend, soft-cancel, paging, positions).
