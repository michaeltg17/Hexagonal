# Hexagonal — Renting Microservice (.NET)

.NET 9 microservice for a renting company, built on the provided hexagonal (ports & adapters) template. Persistence: MongoDB. Must be runnable locally by anyone with only docker/docker-compose.

## Task spec (exam)

REST endpoints:

- Create a new vehicle in the fleet
- List the available vehicles of the fleet
- Rent a vehicle (for a person)
- Return a vehicle

Constraints:

- A person can not have more than 1 active rental at the same time.
- The fleet can not contain vehicles with a manufacture date older than 5 years.

Required test examples (one of each is enough):

- Unit: validate one implemented method without its dependencies.
- Functional: integration test excluding the host.
- Infrastructure: one REST endpoint at host level only (call reception + model validation), not complete.

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

## Roadmap (step by step)

1. Domain: `Vehicle` (plate, make, model, manufacture date), `Rental` (person), invariants (age ≤ 5 years, one active rental per person).
2. ApplicationCore: use cases `CreateVehicle`, `ListVehicles`, `RentVehicle`, `ReturnVehicle` + inputs/outputs/ports; wire in `AddUseCases`.
3. Infrastructure: Mongo entities/collections, `VehicleRepository`, `EntityFactory`, `UnitOfWork`; wire in `AddBaseInfrastructure`.
4. Api: `VehiclesController` endpoints, MediatR requests/handlers, presenters, view models.
5. Tests: one unit, one functional, one infrastructure (per task spec).
6. Verify: `dotnet test src/microservice.sln --configuration Release` green; `docker-compose up --build` + swagger smoke test.

## Workflow

Commit on `dev` → push `dev` → open (or update) the `dev` → `main` PR.

When creating or updating the `dev` → `main` PR:
1. Run `git fetch origin main` first
2. Compare `origin/main..dev` to identify only the actual new changes.
3. Check if a PR already exists (`gh pr list --repo michaeltg17/Hexagonal --head dev --state open`).
4. If none exists, create one with title and description summarizing the changes.
5. If one exists, update its title and description to reflect the actual current diff.
