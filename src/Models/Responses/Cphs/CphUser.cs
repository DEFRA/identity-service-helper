// <copyright file="CphUser.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

<<<<<<<< HEAD:src/Responses/Cphs/Users/CphAssociatedUser.cs
namespace Defra.Identity.Responses.Cphs.Users;
========
namespace Defra.Identity.Models.Responses.Cphs;

using System.ComponentModel;
>>>>>>>> 843c446 (Initial commit for openapi documentation):src/Models/Responses/Cphs/CphUser.cs

public class CphAssociatedUser
{
<<<<<<<< HEAD:src/Responses/Cphs/Users/CphAssociatedUser.cs
    public Guid AssociationId { get; set; }
========
    [Description(OpenApiMetadata.Id)]
    public Guid Id { get; set; }
>>>>>>>> 843c446 (Initial commit for openapi documentation):src/Models/Responses/Cphs/CphUser.cs

    [Description(OpenApiMetadata.Id)]
    public Guid UserId { get; set; }

    [Description(OpenApiMetadata.Id)]
    public Guid ApplicationId { get; set; }

    [Description(OpenApiMetadata.Id)]
    public Guid RoleId { get; set; }

<<<<<<<< HEAD:src/Responses/Cphs/Users/CphAssociatedUser.cs
    public required string RoleName { get; init; }

========
    [Description(OpenApiMetadata.Users.Email)]
>>>>>>>> 843c446 (Initial commit for openapi documentation):src/Models/Responses/Cphs/CphUser.cs
    public required string Email { get; set; }

    [Description(OpenApiMetadata.Users.DisplayName)]
    public required string DisplayName { get; set; }
}
