// <copyright file="UserMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Users;
using Defra.Identity.Postgres.Database.Entities;

public static class UserMapper
{
    public static User MapUserEntityToUser(UserAccounts userEntity)
    {
        return new User()
        {
            Id = userEntity.Id,
            Email = userEntity.EmailAddress,
            FirstName = userEntity.FirstName,
            LastName = userEntity.LastName,
            DisplayName = userEntity.DisplayName,
            Active = userEntity.DeletedAt == null,
        };
    }
}
