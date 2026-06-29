# PokeGrading Backend Layered Architecture (Phase 5)

## Goal

Keep controllers as thin HTTP orchestrators and centralize domain/infrastructure logic in repositories and services.

## Layers

1. Controllers layer
- Responsibility: routing, request binding, orchestrating services, returning HTTP responses.
- Must not contain: SQL statements, file-system persistence logic, OCR scoring internals.

2. Services layer
- Responsibility: business logic and cross-cutting flows.
- Current services:
  - CardValidationService
  - ImageStorageService
  - AuditService
  - OcrService
  - OcrParsingService
  - SearchScoringService
  - GradingPersistenceService

3. Repositories layer
- Responsibility: persistence access to DB entities and SQL mapping.
- Current repositories:
  - CardRepository
  - UserRepository

4. Utilities layer
- Responsibility: low-level shared primitives and technical helpers.
- Current utilities include:
  - DatabaseService
  - SQL_connection
  - PasswordService
  - ImageValidationService
  - ControllerErrorExtensions

## Dependency Rules

Allowed dependencies:
- Controllers -> Services, Repositories, DTO models
- Services -> Repositories, Utilities
- Repositories -> Utilities

Avoid:
- Controllers -> DatabaseService directly
- Controllers -> direct File/Directory/FileStream handling
- Controllers -> inline SQL strings

## Error and Trace Standardization

- Controllers should set trace context through `EnsureTraceId()`.
- Error responses should use shared helpers:
  - `BadRequestWithTrace(...)`
  - `UnauthorizedWithTrace(...)`
  - `NotFoundWithTrace(...)`
  - `ConflictWithTrace(...)`
  - `StatusCodeWithTrace(...)`
- Trace header: `X-Trace-Id`.

## HTTP Contract Compatibility

This refactor preserves endpoint routes and response contracts.
Examples:
- `/Card/*`
- `/Login/login`
- `/Register/register`
- `/Grading/submit`
- `/api/b2b/catalog/coverage`

## Maintenance Checklist

Before merging controller changes:
1. Verify controller has no SQL literals.
2. Verify controller has no direct file I/O operations.
3. Verify trace header is attached on handled error paths.
4. Verify dependencies are injected via interfaces where available.
5. Run `dotnet build` and smoke test critical endpoints.
