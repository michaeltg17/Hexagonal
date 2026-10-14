// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Maintainability", "CA1515:Consider making public types internal", Justification = "For avoid xUnit1027.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.CompositionRootCollectionFixture")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Specs.VehiclesSpec")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.FunctionalTestBase")]
[assembly: SuppressMessage("Design", "CA1515:Consider making public types internal", Justification = "xUnit test classes must be public (xUnit1000).", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.FunctionalTests.Infrastructure.CompositionRootTestFixture")]
[assembly: SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "xUnit test method naming convention.")]
