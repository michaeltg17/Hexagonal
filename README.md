# Sample Implementation of Hexagonal Architecture in a Microservice
## Index
### [Introduction](#introduction)
### [Clean Architecture](#clean-architecture)
### [Hexagonal Architecture Style](#hexagonal-architecture-style)
- [The Left side](#the-left-side)
- [The Right Side](#the-right-side)
- [Ports](#ports)
- [Adapters](#adapters)
### [Use Cases](#use-cases)
### [Design Patterns](#design-patterns)
- [Controller and Command Pattern](#controller)
- [ViewModel](#view-model)
- [Presenter](#presenter)
- [Unit of Work](#unit-of-work)
- [First-Class Collections](#first-class-collections)
- [Factory](#factory)
### [DDD Patterns](#ddd-patterns)
- [Value Object](#value-object)
- [Entity](#entity)
- [Aggregate Root](#aggregate-root)
- [Repository](#repository)
- [Use Case](#use-case)
- [Bounded Context](#bounded-context)
- [Entity Factory](#entity-factory)
- [Domain Service](#domain-service)
### [Separation of Concerns](#separation-of-concerns)
- [Domain](#domain)
- [Application](#application)
- [Infrastructure](#infrastructure)
- [User interface](#user-interface)
### [.NET Core Web API](#net-core-web-api)
- [Swagger](#swagger)
- [Microsoft Extensions](#microsoft-extensions)
- [Logging](#logging)
- [Authentication](#authentication)
- [Authorization](#authorization)

## Introduction
Sample implementation of the Clean Architecture Principles with .NET Core. Use cases as central organizing structure, decoupled from frameworks and technology details. Built with small components that are developed and tested in isolation.

This template is a Renting Company microservice: a fleet manager registers new vehicles in the fleet, a customer lists the available vehicles, rents one of them and later returns it. The fleet can not contain vehicles older than 5 years and a person can not have more than 1 active rental at the same time.

## Clean Architecture
The Clean Architecture style focus on a loosely coupled implementation of use cases and it is summarized as:
- It is an architecture style that the Use Cases are the central organizing structure.
- Follows the Ports and Adapters pattern.
- The implementation is guided by tests (TDD Outside-In).
- Decoupled from technology details.

## Hexagonal Architecture Style
Hexagonal Architecture (aka Ports and Adapters) is one strategy to decouple the use cases from the external details. 
It is an architectural style that allows an application to equally be driven by users, programs, automated test or batch scripts, and to be developed and tested in isolation from its eventual run-time devices and databases. It has another beneficial effect: it allows to isolate the core business of an application and automatically test its behaviour independently of everything else.

This architectural style uses a notion of port and adapter. Thus, it is also referred to as **Ports and Adapters**. Port is an abstract concept, whereas the adapter is the concrete implementation of a port.

The hexagon is the application itself. Saying “hexagon” and saying “application” is the same thing. Inside the hexagon we just have the things that are important for the business problem that the application is trying to solve. The hexagon contains the business logic, with no references to any technology, framework or real world device. So the application is technology agnostic.

Outside the hexagon we have any real world thing that the application interacts with. These things include humans, other applications, or any hardware/software device. They are the actors. Actors are arranged around the hexagon depending on who triggers the interaction between the application and the actor.

### The Left Side
Actors on the left/top side are Drivers, or Primary Actors. The interaction is triggered by the actor. A driver is an actor that interacts with the application to achieve a goal. Primary Actors are usually the user interface or the Test Suit.

### The Right Side
Actors on the right/bottom side are Driven Actors, or Secondary Actors. The interaction is triggered by the application. A driven actor provides some functionality needed by the application for implementing the business logic. The Secondary Actors are usually Databases, Cloud Services or other systems.

### Ports
Ports are the application boundary, in the picture a port is an edge of the hexagon. From the outside world, actors can only interact with the hexagon ports, they shouldn’t be able to access the inside of the hexagon. Ports are interfaces that the application offers to the outside world for allowing actors interact with the application.

### Adapters
Actors interact with hexagon ports through adapters using a specific technology. An adapter is a software component that allows a technology to interact with a port of the hexagon. Given a port, there may be an adapter for each desired technology that we want to use. Adapters are outside the application. A driver adapter uses a driver port interface, converting a specific technology request into a technology agnostic request to a driver port.

## Use Cases
Use Cases are algorithms that interpret the input to generate the output data, their implementation should be closer as possible to the business vocabulary.

When talking about a use case, it does not matter if it a Mobile or a Desktop application, use cases are delivery independent. The most important about use cases is how they interact with the actors.

- Primary actors initiate a use case. They can be the End User, another system or a clock.
- Secondary actors are affected by use cases.

On this template the Customer is the primary actor on the left who rents and returns vehicles, and the Fleet Manager creates the new vehicles of the fleet. In the middle the Renting microservice and the secondary actors (e.g. the MongoDB database) on the right. A set of use cases is used to describe the software: create a vehicle, list available vehicles, rent a vehicle and return a vehicle.

## Design Patterns
The following design patterns will help us continue to implement use cases consistently. 

### Controller and Command Pattern
In a Command-based architecture, Controllers receive Requests and send commands to perform some operation and we have a separate handler of command that makes the separation of concern and improves the single responsibility as well. 

To implement this architecture, we can use a third-party library, like **MediatR** (Mediator Pattern) which does a lot of groundwork for us. The mediator pattern defines an object that encapsulates how a set of objects interact.

```
    public sealed class VehiclesController : ControllerBase
    {

        // code omitted to simplify

        public async Task<IActionResult> Post([FromBody][Required] CreateVehicleRequest request)
        {
            _appLogger.LogDebug(
                "Entering Post. Request: {@request}",
                request);

            var presenter = await _mediator.Send(request);
            return presenter.ActionResult;
        }
    }
```
#### MediatR
- The mediator pattern defines an object that encapsulates how a set of objects interact.
- It promotes loose coupling by keeping the objects from referring to each other explicitly.
- It promotes the Single Responsibility Principle by allowing the communication to be offloaded to a class that handles just that.

MediatR allows us to decouple our controllers from our business logic by having our controller actions send a request message to a handler. The MediatR library supports two type of operations.
- Request/response messages, dispatched to a single handler.
- Notification messages, dispatched to multiple handlers.

The request/response interface handles both command and query scenarios. First, create a message:
```
    public class CreateVehicleRequest : IRequest<IWebApiPresenter>
    {
        public CreateVehicleRequest(string plate, string make, string model, DateTime manufactureDate)
        {
            Plate = plate;
            Make = make;
            Model = model;
            ManufactureDate = manufactureDate;
        }

        // code omitted to simplify
    }
```

Next, the handler. Build the Input message then call the Use Case, you should notice that the handler do not build the Response, instead this responsibility is delegated to the presenter object.:
```
    public class CreateVehicleRequestHandler : IRequestHandler<CreateVehicleRequest, IWebApiPresenter>
    {
        private readonly ICreateVehicleUseCase _useCase;
        private readonly CreateVehiclePresenter _presenter;

        // code omitted to simplify

        public async Task<IWebApiPresenter> Handle(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            var input = new CreateVehicleInput(
                new Plate(request.Plate),
                new Make(request.Make),
                new Model(request.Model),
                new ManufactureDate(request.ManufactureDate));

            await _useCase.Execute(input);
            return _presenter;
        }
    }
```

### View Model
ViewModels are data transfer objects, they will be rendered by the framework so we need to follow the framework guidelines. You have to add comments describing each property and the [Required] attribute so swagger generators could know the properties that are not nullable.

```
    public sealed class VehicleResponse
    {
        public VehicleResponse(
            Guid vehicleId,
            Plate plate,
            Make make,
            Model model,
            ManufactureDate manufactureDate,
            bool isAvailable)
        {
            VehicleId = vehicleId;
            Plate = plate.ToString();
            Make = make.ToString();
            Model = model.ToString();
            ManufactureDate = manufactureDate.ToString();
            IsAvailable = isAvailable;
        }

        /// <summary>
        /// Gets vehicle ID.
        /// </summary>
        [Required]
        public Guid VehicleId { get; }

        /// <summary>
        /// Gets plate.
        /// </summary>
        [Required]
        public string Plate { get; }

        /// <summary>
        /// Gets make.
        /// </summary>
        [Required]
        public string Make { get; }

        /// <summary>
        /// Gets model.
        /// </summary>
        [Required]
        public string Model { get; }

        /// <summary>
        /// Gets manufacture date.
        /// </summary>
        [Required]
        public string ManufactureDate { get; }

        /// <summary>
        /// Gets whether the vehicle is available.
        /// </summary>
        [Required]
        public bool IsAvailable { get; }
    }
```
### Presenter
Presenters are called by the application Use Cases and build the Response objects.
```
    public sealed class CreateVehiclePresenter : IWebApiPresenter, ICreateVehicleOutputPort
    {
        public IActionResult ActionResult { get; private set; }

        public void VehicleAlreadyExists(string message)
        {
            ActionResult = new BadRequestObjectResult(message);
        }

        public void StandardHandle(CreateVehicleOutput output)
        {
            var vehicle = new VehicleResponse(
                output.Vehicle.Id.ToGuid(),
                output.Vehicle.Plate,
                output.Vehicle.Make,
                output.Vehicle.Model,
                output.Vehicle.ManufactureDate,
                output.Vehicle.IsAvailable);

            ActionResult = new CreatedAtRouteResult(
                "GetVehicle",
                new
                {
                    VehicleId = vehicle.VehicleId,
                },
                vehicle);
        }
    }
```
It is important to understand that from the Application perspective the use cases see an OutputPort with custom methods to call dependent on the message, and from the Web Api perspective the Controller only see the ViewModel property.

**Standard Output**

The output port for the use case regular behavior.

**Error Output**

Called when a blocking error happens.

**Alternative Output**

Called when an alternative flow happens.

### Unit Of Work
Unit of work is the pattern that allows handling transactions during data manipulation using the repository pattern. It maintains a list of the objects affected by a business transaction and coordinates the writing of the changes and the resolution of concurrency problems.

A basic example of the implementation of the unit of work pattern can be seen in the following code of the template:

```
    public interface IUnitOfWork
    {
        Task Save();
    }
```

```
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly IMongoClient _client;
        private IClientSessionHandle? _session;
        private bool _disposed = false;

        public UnitOfWork(IMongoClient client)
        {
            _client = client ??
                throw new ArgumentNullException(nameof(client));
        }

        public async Task Save()
        {
            await (_session ??= _client.StartSession()).CommitTransactionAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _session?.Dispose();
                    _client.Dispose();
                }
            }

            _disposed = true;
        }
    }
```

### First-Class Collections
Application of this rule is simple: any class that contains a collection should contain no other member variables. Each collection gets wrapped in its own class, so now behaviors related to the collection have a home. You may find that filters become a part of this new class. Also, your new class can handle activities like joining two groups together or applying a rule to each element of the group.

```
    public sealed class RentalCollection
    {
        private readonly IList<IRental> _rentals;

        public RentalCollection()
        {
            this._rentals = new List<IRental>();
        }

        public void Add(IRental rental) => this._rentals.Add(rental);

        public IReadOnlyCollection<IRental> GetRentals()
        {
            return new ReadOnlyCollection<IRental>(this._rentals);
        }

        public IRental? GetActive()
        {
            return this._rentals.FirstOrDefault(r => r.IsActive);
        }

        public bool HasActiveRental()
        {
            return this.GetActive() is not null;
        }
    }
```

### Factory
In Factory pattern, we create object without exposing the creation logic to the client and refer to newly created object using a common interface.

```
    public interface IVehicleFactory
    {
        IVehicle NewVehicle(Plate plate, Make make, Model model, ManufactureDate manufactureDate);
    }
```
```
    public interface IRentalFactory
    {
        IRental NewRental(VehicleId vehicleId, Person person);
    }
```    
```
    public sealed class EntityFactory : IVehicleFactory, IRentalFactory
    {
        public IVehicle NewVehicle(
            Plate plate,
            Make make,
            Model model,
            ManufactureDate manufactureDate) => new VehicleEntity(plate, make, model, manufactureDate);

        public IRental NewRental(
            VehicleId vehicleId,
            Person person) => new RentalEntity(vehicleId, person);
    }
```

## DDD Patterns
Since, as we have seen, the hexagonal architecture is still a domain-oriented architecture (DDD), the following DDD patterns will also be used (all of them are implemented in the template):

### Value Object
Encapsulate the tiny domain business rules. Structures that are unique by their properties and the whole object is immutable, once it is created its state can not change.

As seen in the example, the VOs are where we have the field validations.

```
    public readonly struct ManufactureDate
    {
        private readonly DateTime _value;

        public ManufactureDate(DateTime value)
        {
            if (value > DateTime.UtcNow)
            {
                throw new ManufactureDateShouldBeInThePastException(
                    "The manufacture date can not be in the future.");
            }

            if (value.AddYears(5) < DateTime.UtcNow)
            {
                throw new VehicleShouldNotBeOlderThanFiveYearsException(
                    "The vehicle is older than the maximum allowed age of 5 years.");
            }

            this._value = value;
        }

        public override string ToString()
        {
            return this._value.ToString("O");
        }
    }
```

Rules of thumb:

- The developer should make the Value Object serializable and deserializable.
- A Value Object can not reference an Entity or another mutable object.

### Entity
Highly abstract and mutable objects unique identified by its IDs

```
    public abstract class Rental : IRental
    {
        public RentalId Id { get; protected set; }

        public VehicleId VehicleId { get; protected set; }

        public Person Person { get; protected set; }

        public DateTime? RentDate { get; protected set; }

        public DateTime? ReturnDate { get; protected set; }

        public bool IsActive => this.ReturnDate is null;

        public void Rent(DateTime rentDate)
        {
            this.RentDate = rentDate;
        }

        public void Return(DateTime returnDate)
        {
            if (this.ReturnDate is not null)
            {
                throw new RentalAlreadyReturnedException("The rental has already been returned.");
            }

            this.ReturnDate = returnDate;
        }
    }
```

Rules of Thumb:

- Entities are mutable.
- Entities are highly abstract.
- Entities do not need to be serializable.
- The entity state should be encapsulated to external access.

### Aggregate Root
Similar to Entities with the addition that Aggregate Root are responsible to keep the graph of objects consistent.

- Owns entities object graph.
- Ensure the child entities state are always consistent.
- Define the transaction scope.

```
    public abstract class Vehicle : IVehicle
    {
        protected Vehicle()
        {
            this.Rentals = new RentalCollection();
        }

        public VehicleId Id { get; protected set; }

        public Plate Plate { get; protected set; }

        public Make Make { get; protected set; }

        public Model Model { get; protected set; }

        public ManufactureDate ManufactureDate { get; protected set; }

        public RentalCollection Rentals { get; protected set; }

        public bool IsAvailable => this.Rentals.GetActive() is null;

        public IRental Rent(IRentalFactory rentalFactory, Person person, DateTime rentDate)
        {
            if (!this.IsAvailable)
            {
                throw new VehicleAlreadyRentedException("The vehicle is already rented.");
            }

            var rental = rentalFactory.NewRental(this.Id, person);
            rental.Rent(rentDate);
            this.Rentals.Add(rental);

            return rental;
        }

        public void Return(DateTime returnDate)
        {
            var active = this.Rentals.GetActive()
                ?? throw new VehicleNotRentedException("The vehicle is not rented.");

            active.Return(returnDate);
        }
    }
```

Rules of thumb:

- Protect business invariants inside Aggregate boundaries.
- Design small Aggregates.
- Reference other Aggregates by identity only.
- Update other Aggregates using eventual consistency.

### Repository
Provides persistence capabilities to Aggregate Roots or Entities.

```
    public sealed class VehicleRepository : IVehicleRepository
    {
        private readonly IMongoCollection<VehicleEntity> _vehicles;

        public VehicleRepository(IMongoDatabase database)
        {
            _vehicles = database.GetCollection<VehicleEntity>("vehicles");
        }

        public async Task Add(IVehicle vehicle)
        {
            await _vehicles.InsertOneAsync((VehicleEntity)vehicle);
        }

        public async Task<IVehicle> GetBy(VehicleId vehicleId)
        {
            var vehicle = await _vehicles
                .Find(v => v.Id.Equals(vehicleId))
                .FirstOrDefaultAsync();

            if (vehicle is null)
            {
                throw new VehicleNotFoundException($"The vehicle {vehicleId} does not exist or is not processed yet.");
            }

            return vehicle;
        }

        public async Task Update(IVehicle vehicle)
        {
            await _vehicles
                .ReplaceOneAsync(v => v.Id.Equals(vehicle.Id), (VehicleEntity)vehicle);
        }
    }
```    
Rules of thumb:

- The repository is designed around the aggregate root or entity.
- A repository for every entity is a code smell.

### Use Case
It is the application entry point for an user interaction. It accepts an input message, executes the algorithm then it should give the output message to the Output port.

They are also called **Application Service** in DDD architectures.

```
    public sealed class RentVehicleUseCase : IRentVehicleUseCase
    {
        private readonly VehicleService _vehicleService;
        private readonly IRentVehicleOutputPort _outputPort;
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RentVehicleUseCase(
            VehicleService vehicleService,
            IRentVehicleOutputPort outputPort,
            IVehicleRepository vehicleRepository,
            IUnitOfWork unitOfWork)
        {
            this._vehicleService = vehicleService;
            this._outputPort = outputPort;
            this._vehicleRepository = vehicleRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task Execute(RentVehicleInput input)
        {
            try
            {
                var vehicle = await this._vehicleRepository.Get(input.VehicleId);
                var rental = await this._vehicleService.Rent(vehicle, input.Person);

                await this._unitOfWork.Save();

                this.BuildOutput(rental, vehicle);
            }
            catch (VehicleNotFoundException notFoundEx)
            {
                this._outputPort.NotFoundHandle(notFoundEx.Message);
                return;
            }
            catch (VehicleAlreadyRentedException alreadyRentedEx)
            {
                this._outputPort.AlreadyRentedHandle(alreadyRentedEx.Message);
                return;
            }
            catch (PersonAlreadyHasActiveRentalException activeRentalEx)
            {
                this._outputPort.AlreadyHasRentalHandle(activeRentalEx.Message);
                return;
            }
        }

        private void BuildOutput(IRental rental, IVehicle vehicle)
        {
            var output = new RentVehicleOutput(rental, vehicle);

            this._outputPort.StandardHandle(output);
        }
    }
```

Rules of thumb:

- The use case implementation are close to a human readable language.
- Ideally a class has a single use case.
- Invokes transaction operations (eg. Unit Of Work).

### Bounded Context
It is a logical boundary, similar to a module in a system. In this project the single Domain project is the single bounded context we designed.

As a rule of thumb, in most cases a microservice will have only a bounded context.

### Entity Factory
Creates new instances of Entities and Aggregate Roots. Should be implemented by the Infrastructure layer. See [Factory](#factory)

### Domain Service
Useful functions across Entities.

```
    public class VehicleService
    {
        private readonly IVehicleFactory _vehicleFactory;
        private readonly IRentalFactory _rentalFactory;
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleService(
            IVehicleFactory vehicleFactory,
            IRentalFactory rentalFactory,
            IVehicleRepository vehicleRepository)
        {
            this._vehicleFactory = vehicleFactory;
            this._rentalFactory = rentalFactory;
            this._vehicleRepository = vehicleRepository;
        }

        public async Task<IVehicle> Create(
            Plate plate,
            Make make,
            Model model,
            ManufactureDate manufactureDate)
        {
            var vehicle = this._vehicleFactory.NewVehicle(plate, make, model, manufactureDate);
            await this._vehicleRepository.Add(vehicle);

            return vehicle;
        }

        public async Task<IRental> Rent(IVehicle vehicle, Person person)
        {
            if (await this._vehicleRepository.PersonHasActiveRental(person))
            {
                throw new PersonAlreadyHasActiveRentalException("The person already has an active rental.");
            }

            var rental = vehicle.Rent(this._rentalFactory, person, DateTime.UtcNow);
            await this._vehicleRepository.Update(vehicle);

            return rental;
        }

        public async Task Return(IVehicle vehicle)
        {
            vehicle.Return(DateTime.UtcNow);
            await this._vehicleRepository.Update(vehicle);
        }
    }
```

Rules of thumb:

- It should not call transaction methods, should not call a Unit Of Work method.

## Separation of Concerns
Applications can be logically built to follow this principle by separating core business behavior from infrastructure and user interface logic. Ideally, business rules and logic should reside in a separate project, which should not depend on other projects in the application. This helps ensure that the business model is easy to test and can evolve without being tightly coupled to low-level implementation details. Separation of concerns is a key consideration behind the use of layers in application architectures.

### Domain
The package that contains the High Level Modules which describe the Domain via **Aggregate Roots**, **Entities** and **Value Objects**. By design this project is Highly Abstract and Stable, in other terms this package contains a considerable amount of interfaces and should not depend on external libraries and frameworks. 

### Application
A project that contains the Application **Use Cases** which orchestrate the high level business rules. By design the orchestration will depend on abstractions of external services (eg. Repositories). The package exposes **Boundaries Interfaces** (in other terms Contracts or **Ports**) which are used by the user interface. Should not depend on external libraries and frameworks.

### Infrastructure
The infrastructure layer is responsible to implement the **Adapters** to the **Secondary Actors**(Right Side). For instance an SQL Server Database is a secondary actor which is affected by the application use cases, all the implementation and dependencies required to consume the SQL Server is created on infrastructure. By design the infrastructure depends on application layer.

### User interface
Responsible for rendering the Graphical User Interface (GUI) to interact with the User or other systems. Made of **Controllers** which receive HTTP Requests and **Presenters** which converts the application outputs into **ViewModels** that are rendered as HTTP Responses. 
