# MarketPulse API

> A production-style financial market data platform built with **ASP.NET
> Core 10**, **PostgreSQL**, **Entity Framework Core**, **Docker**, and
> **React**.

MarketPulse continuously imports live market data from the Finnhub API,
stores historical price data in PostgreSQL, and exposes a REST API for
querying financial instruments and historical prices. A background
worker performs scheduled imports, simulating the architecture of a real
market data ingestion service.

## Features

### Backend

-   Live market data integration using the Finnhub API
-   Scheduled background ingestion using `BackgroundService`
-   RESTful ASP.NET Core Web API
-   PostgreSQL persistence using Entity Framework Core
-   Historical price storage
-   Pagination and filtering
-   Health checks
-   Structured logging
-   Dependency Injection throughout
-   HttpClientFactory integration
-   Configuration using the Options pattern
-   Dockerised PostgreSQL development environment

### Frontend *(coming next)*

-   React dashboard
-   Live market overview
-   Historical price charts
-   Search and filtering
-   Responsive UI

## Architecture

``` text
                    +-------------------+
                    |    Finnhub API    |
                    +---------+---------+
                              |
                    IMarketDataClient
                              |
                +-------------+--------------+
                |                            |
                |  MarketPriceImportWorker   |
                |   (BackgroundService)      |
                +-------------+--------------+
                              |
                    MarketPriceImportService
                              |
                     Entity Framework Core
                              |
                         PostgreSQL
                              |
                     Financial API Endpoints
                              |
                           React UI
```

## Technology Stack

  Area                    Technology
  ----------------------- --------------------------
  Backend                 ASP.NET Core 10
  Language                C#
  Database                PostgreSQL
  ORM                     Entity Framework Core
  External API            Finnhub
  Background Processing   Hosted BackgroundService
  Containerisation        Docker
  API Documentation       Swagger / OpenAPI
  Frontend                React *(in progress)*

## Example API

``` http
GET /api/FinancialInstruments
```

``` http
GET /api/FinancialInstruments/AAPL/prices/latest
```

``` http
GET /api/FinancialInstruments/AAPL/prices/history?page=1&pageSize=20
```

``` http
POST /api/MarketPrices/import
```

``` http
GET /health
```

## Project Structure

``` text
src/
 └── MarketPulse.Api
     ├── Clients
     ├── Configuration
     ├── Controllers
     ├── Data
     ├── DTOs
     ├── HealthChecks
     ├── Interfaces
     ├── Mappers
     ├── Migrations
     ├── Models
     ├── Services
     └── Program.cs
```

## Running Locally

### Prerequisites

-   .NET 10 SDK
-   Docker Desktop
-   Finnhub API Key

``` bash
docker compose up -d
dotnet ef database update
dotnet user-secrets init
dotnet user-secrets set "Finnhub:ApiKey" "<your-api-key>"
dotnet run
```

Swagger:

``` text
https://localhost:xxxx/swagger
```

## Design Decisions

### Layered Architecture

Business logic is separated from controllers using dedicated service
classes, allowing endpoints to remain thin and easily testable.

### External Provider Abstraction

Market data is accessed through an `IMarketDataClient` interface,
allowing Finnhub to be replaced with another provider without affecting
the rest of the application.

### Background Processing

A hosted `BackgroundService` imports prices on a configurable schedule,
demonstrating production-style asynchronous processing.

### Idempotent Imports

A unique database constraint prevents duplicate market prices from being
stored when the importer is executed repeatedly.

## Future Improvements

-   React frontend dashboard
-   Interactive historical price charts
-   Authentication and authorisation
-   Multiple market data providers
-   Redis caching
-   Kubernetes deployment
-   GitHub Actions CI/CD
-   Cloud deployment (Azure or AWS)

## Engineering Highlights

-   REST API design
-   ASP.NET Core
-   Entity Framework Core
-   PostgreSQL
-   Docker
-   Background services
-   Dependency Injection
-   External API integration
-   Clean architecture principles
-   Production-style configuration
-   Logging and observability

## License

MIT
