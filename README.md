# TradeBlotterApi

Production-style **Repo Pre-Trade Analytics** Web API built as a portfolio project and completed for my **Advanced C++** coursework (focus: software architecture, testing discipline, and clean interfaces across modules).

This repository is organized as a multi-project solution with clear separation between API, domain logic, infrastructure/persistence, and tests.

## What it does

TradeBlotterApi provides REST endpoints for repo / fixed-income trade analytics and tooling that would traditionally live in spreadsheets, including:

- Bond **dirty price** + **accrued interest** support
- Repo funding economics using **ACT/360**
- **Haircut-adjusted** cash proceeds
- **Carry** calculations and related pre-trade metrics
- Structured persistence and audit-friendly data modeling

## Tech stack

- **ASP.NET Core** (Web API)
- **C#**
- **PostgreSQL**
- **Docker**
- **Swagger / OpenAPI**
- Unit testing project included in the solution

## Solution structure

- `TradeBlotterApi.Api/` — Web API (controllers, request/response models, OpenAPI)
- `TradeBlotterApi.Domain/` — Core pricing/analytics logic (domain-first design)
- `TradeBlotterApi.Infrastructure/` — Persistence + integrations (EF Core / PostgreSQL, logging)
- `TradeBlotterApi.Tests/` — Unit tests for core components
- `TradeBlotterApi.sln` — Solution file

## Running locally

### Option A: .NET (no containers)
1. Restore & build:
   ```bash
   dotnet restore
   dotnet build
