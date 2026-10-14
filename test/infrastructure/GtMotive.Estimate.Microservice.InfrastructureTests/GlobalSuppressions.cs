// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For avoid xUnit1027.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure.TestServerCollectionFixture")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.InfrastructureTests.Specs.VehiclesControllerSpec")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure.InfrastructureTestBase")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.InfrastructureTests.Infrastructure.GenericInfrastructureTestServerFixture")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "xUnit test method naming convention.")]
