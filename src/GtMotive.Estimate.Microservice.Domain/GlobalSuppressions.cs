// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Security Hotspot", "S4834:Controlling permissions is security-sensitive", Justification = "Needed for Microsoft.AspNetCore.Authorization.IAuthorizationService", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Domain.Interfaces.IAuthorizationService")]
[assembly: SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "T is necessary for dependency injection.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Domain.Interfaces.IAppLogger`1")]
[assembly: SuppressMessage("Design", "CA1716:Identifiers should not match keywords", Justification = "Return is domain vocabulary for returning a rented vehicle.", Scope = "module")]
[assembly: SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "RentalCollection is a first-class collection of the Rental entity.", Scope = "type", Target = "~T:GtMotive.Estimate.Microservice.Domain.Entities.RentalCollection")]
[assembly: SuppressMessage("Design", "CA1024:Use properties where appropriate", Justification = "GetRentals returns a defensive snapshot of the rentals.", Scope = "member", Target = "~M:GtMotive.Estimate.Microservice.Domain.Entities.RentalCollection.GetRentals")]
