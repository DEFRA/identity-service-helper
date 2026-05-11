// <copyright file="IProfileService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Profiles;

using Defra.Identity.Models.Requests.Profiles.Queries;
using Defra.Identity.Models.Responses.Profiles;

public interface IProfileService
{
    Task<UserProfile> GetUserProfile(GetUserProfileByUserId request, CancellationToken cancellationToken = default);
}
