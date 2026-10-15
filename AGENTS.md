# Hexagonal — Renting Microservice (.NET)

.NET 9 microservice for a renting company, built on the provided hexagonal (ports & adapters) template. Persistence: MongoDB.

## Layout

Solution: `src/microservice.sln`

| Project | Where code goes |
|---|---|
| `src/GtMotive.Estimate.Microservice.Domain` | Entities, value objects (field validation lives here), domain exceptions, domain ports (`IUnitOfWork`, `IAppLogger`, `ITelemetry`). No external references. |
| `src/GtMotive.Estimate.Microservice.ApplicationCore` | Use cases: `IUseCase<TInput>`, `IUseCaseInput`/`IUseCaseOutput`, `IOutputPort*`. Registered in `ApplicationConfiguration.AddUseCases`. |
| `src/GtMotive.Estimate.Microservice.Api` | Controllers, MediatR requests/handlers, presenters (`IWebApiPresenter`), view models, filters. DI via `ApiConfiguration`. |
| `src/GtMotive.Estimate.Microservice.Infrastructure` | Adapters: MongoDB, repositories, entity factory, unit of work. DI via `InfrastructureConfiguration` (`AddBaseInfrastructure`). |
| `src/GtMotive.Estimate.Microservice.Host` | Entry point (`Program.cs`), Serilog, Swagger, auth. |
| `test/unit/...UnitTests` | xunit + Moq, no infrastructure. |
| `test/functional/...FunctionalTests` | Integration tests excluding the host (`FunctionalTestBase`). |
| `test/infrastructure/...InfrastructureTests` | Host-level tests via `Acheve.TestHost` (`InfrastructureTestBase`, `Startup.cs`). |
| `test/load` | JMeter load plan. |

Package versions are pinned centrally in `Directory.Build.targets` (tests: `test/Directory.Build.props`) — add `<PackageReference>` without `Version` in csproj files.

## Commands

- Build + run all tests (same as CI): `dotnet test src/microservice.sln --configuration Release`
- Run locally: `docker-compose up --build` → API at http://localhost:8080 (Swagger at `/swagger`). Env vars come from `.env` (template: `.env.example`).
- .NET SDK pinned to 9.0.200 (`global.json`); MongoDB 8 comes from docker-compose.

## Conventions

- Request flow: Controller → MediatR `Request : IRequest<IWebApiPresenter>` → handler builds the domain `Input` → `UseCase.Execute(input)` → output port (presenter) sets `ActionResult`.
- Field validation in domain value objects; business rules in entities/use cases; domain exceptions are caught in the use case and mapped to the output port error method (`BusinessExceptionFilter` covers the rest).
- A new use case = one class in ApplicationCore + input/output + output port, registered in `AddUseCases`. A new endpoint = controller action + MediatR request + handler + presenter + view model in Api.
- Style: StyleCop + Sonar analyzers with `EnforceCodeStyleInBuild`; warnings are errors in test projects; follow `.editorconfig` (file-scoped namespaces, CRLF, UTF-8 BOM).

## Workflow

Commit on `dev` → push `dev` → open (or update) the `dev` → `main` PR.

When creating or updating the `dev` → `main` PR:
1. Run `git fetch origin main` first
2. Compare `origin/main..dev` to identify only the actual new changes.
3. Check if a PR already exists (`gh pr list --repo michaeltg17/Hexagonal --head dev --state open`).
4. If none exists, create one with title and description summarizing the changes.
5. If one exists, update its title and description to reflect the actual current diff.
