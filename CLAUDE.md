# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build Hinario.sln

# Run the API (HTTP on port 5252)
dotnet run --project src/Hinario.API --launch-profile http

# Run the API (HTTPS on ports 7243/5252)
dotnet run --project src/Hinario.API --launch-profile https

# Apply EF Core migrations
dotnet ef database update --project src/Hinario.Infra --startup-project src/Hinario.API

# Add a new migration
dotnet ef migrations add <MigrationName> --project src/Hinario.Infra --startup-project src/Hinario.API
```

There are no test projects in this solution.

## Architecture

Clean Architecture with four layers. Dependency flow: `API → Application → Domain ← Infra`.

- **Hinario.Domain** — entities (`Hino`), DTOs, interfaces, and utilities. No external dependencies. All contracts (IHinoService, IHinoRepository, ICampinaGrandeMineracaoService) live here.
- **Hinario.Application** — implements domain service interfaces. Contains `HinoService` (core business logic) and `CampinaGrandeMineracaoService` (web scraper using HtmlAgilityPack).
- **Hinario.Infra** — EF Core 10 with Npgsql (PostgreSQL). `HinarioApiContext` and `HinoRepository`. Contains migrations.
- **Hinario.API** — ASP.NET Core 10 Web API. Single controller: `HinoController`. DI wiring and CORS configured in `Program.cs`.

## Domain Model

`Hino` (table: `hinos`): the single core entity.
- `Identificador` — unique string key like `"C-1"` or `"H-10"` (nullable). Prefix defines hymn type: H=Hinos, C=Cânticos.
- `LetraIdx` — PostgreSQL `tsvector` column auto-generated server-side for full-text search with Portuguese language.

## Search

Full-text search is implemented at the PostgreSQL level via tsvector + GIN index on `LetraIdx`. Search results are ranked by: exact phrase match > all words present > word count. The `TrechoPesquisaHelper` (in `Hinario.Domain/Utils/`) generates preview excerpts with matched terms wrapped in `<b>` tags.

`TextNormalizer` (in `Hinario.Domain/Utils/`) removes accents and lowercases text — used when searching by `Identificador` (hyphens are also stripped, so `"C-1"` and `"c1"` resolve to the same hymn).

## Key API Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/hino` | All hymns |
| GET | `/api/hino/{id}` | By database ID |
| GET | `/api/hino/identificador/{identificador}` | By identifier (e.g., `C-1`) |
| GET | `/api/hino/pesquisar?texto=...` | Full-text search |
| POST | `/api/hino` | Create |
| PUT | `/api/hino/{id}` | Update |
| POST | `/api/hino/importar` | Bulk import (HolyRycs JSON format) |
| GET | `/api/hino/minerar/canticos/{numero}` | Scrape hymn from external website (1–100) |
| GET | `/api/hino/{tipo}/{numero}/proximo` | Next hymn in type sequence |
| GET | `/api/hino/{tipo}/{numero}/anterior` | Previous hymn in type sequence |

## Database

PostgreSQL hosted on Aiven. Connection string is in `appsettings.json` / `appsettings.Development.json`. EF Core query splitting is enabled globally on the DbContext. The `Identificador` column has a partial unique index (allowing multiple NULLs).

## Web Scraper

`CampinaGrandeMineracaoService` scrapes hymns from an external Igreja em Campina Grande website. It handles multiple character encodings (UTF-8, UTF-16, ISO-8859-1) and parses HTML with HtmlAgilityPack. Accessed via the `/api/hino/minerar/canticos/{numero}` endpoint.
