// <copyright file="TestData.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Defra.Identity.Postgres.Database.Entities;

[ExcludeFromCodeCoverage]
public static class TestData
{
    private static readonly Guid OperatorId = new Guid("1d276c53-7abd-46d4-9888-230e0239b234");
    private static readonly DateTime CreateDateTime = DateTime.UtcNow.AddDays(-10);
    private static readonly DateTime ExpiredDateTime = DateTime.UtcNow.AddDays(-5);
    private static readonly DateTime DeletedDateTime = DateTime.UtcNow;
    private static readonly DateTime InvitationFutureExpiryDateTime = DateTime.UtcNow.AddDays(2);
    private static readonly DateTime InvitationPastExpiryDateTime = DateTime.UtcNow.AddDays(-2);
    private static readonly DateTime InvitationPastAcceptedDateTime = DateTime.UtcNow.AddDays(-3);
    private static readonly DateTime InvitationPastRejectedDateTime = DateTime.UtcNow.AddDays(-3);
    private static readonly DateTime DelegationRevokedPastDateTime = DateTime.UtcNow.AddDays(-2);
    private static readonly DateTime DelegationExpiryFutureDateTime = DateTime.UtcNow.AddDays(10);
    private static readonly DateTime DelegationExpiryPastDateTime = DateTime.UtcNow.AddDays(-2);

    public static class Role
    {
        public static Postgres.Database.Entities.Roles Role1 => new()
        {
            Id = new Guid("3f0155fd-6c92-484e-8707-fd5a68a58712"),
            Name = "Role 1",
            Description = "Role 1 Description",
        };

        public static Postgres.Database.Entities.Roles Role2 => new()
        {
            Id = new Guid("e31a4d87-7959-4ef4-8893-f0bdc5ae00a5"),
            Name = "Role 2",
            Description = "Role 2 Description",
        };
    }

    public static class Application
    {
        public static Postgres.Database.Entities.Applications Application1NotDeleted => new()
        {
            Id = new Guid("3b53b6a0-97d7-42b6-87e4-de91a55a8bfd"),
            Name = "App 1",
            ClientId = new Guid("7060a218-2736-4fd2-b580-0bbd128df30c"),
            TenantName = "Tenant 1",
            Scopes = "scope1;scope2",
            RedirectUris = "https://callback1",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };

        public static Postgres.Database.Entities.Applications Application2NotDeleted => new()
        {
            Id = new Guid("1e9d1cf4-5fa2-4544-b0fd-ca3827956595"),
            Name = "App 2",
            ClientId = new Guid("185ec0f8-5521-4ad8-86cd-5ef65711a045"),
            TenantName = "Tenant 2",
            Scopes = "scope3",
            RedirectUris = "https://callback2",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };

        public static Postgres.Database.Entities.Applications Application3Deleted => new()
        {
            Id = new Guid("3126090f-95b7-4dea-ab22-79e822984228"),
            Name = "App 3",
            ClientId = new Guid("5d84a716-2c27-4303-aae5-731b38bc0488"),
            TenantName = "Tenant 3",
            Scopes = "scope4",
            RedirectUris = "https://callback3",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
            DeletedAt = DeletedDateTime,
            DeletedById = OperatorId,
        };
    }

    public static class User
    {
        public static UserAccounts User1NotDeleted => new()
        {
            Id = new Guid("5001b84a-acdf-483b-9072-99669ea1b994"),
            EmailAddress = "user1@test.com",
            FirstName = "User 1 First Name",
            LastName = "User 1 Last Name",
            DisplayName = "User 1 Display Name",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };

        public static UserAccounts User2NotDeleted => new()
        {
            Id = new Guid("059a8562-04b6-4a76-9928-6e41a00baec5"),
            EmailAddress = "user2@test.com",
            FirstName = "User 2 First Name",
            LastName = "User 2 Last Name",
            DisplayName = "User 2 Display Name",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };

        public static UserAccounts User3Deleted => new()
        {
            Id = new Guid("6d4be6a4-4d17-47b5-97d8-d2306cac4d9d"),
            EmailAddress = "user3@test.com",
            FirstName = "User 3 First Name",
            LastName = "User 3 Last Name",
            DisplayName = "User 3 Display Name",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
            DeletedAt = DeletedDateTime,
            DeletedById = OperatorId,
        };

        public static UserAccounts User4NotDeleted => new()
        {
            Id = new Guid("3ba79aa6-e8ca-49af-a272-1b937f15d7c3"),
            EmailAddress = "user4@test.com",
            FirstName = "User 4 First Name",
            LastName = "User 4 Last Name",
            DisplayName = "User 4 Display Name",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };

        public static UserAccounts User5NotDeleted => new()
        {
            Id = new Guid("dcb1521e-e71e-4e80-8d03-6f527a419b9f"),
            EmailAddress = "user5@test.com",
            FirstName = "User 5 First Name",
            LastName = "User 5 Last Name",
            DisplayName = "User 5 Display Name",
            CreatedAt = CreateDateTime,
            CreatedById = OperatorId,
        };
    }

    public static class Species
    {
        public static AnimalSpecies AnimalSpecies1Active => new() { Id = "SP1", Name = "Species 1", IsActive = true, };

        public static AnimalSpecies AnimalSpecies2Active => new() { Id = "SP2", Name = "Species 2", IsActive = true, };

        public static AnimalSpecies AnimalSpecies3Inactive => new()
        {
            Id = "SP3", Name = "Species 3", IsActive = false,
        };
    }

    public static class Cph
    {
        public static CountyParishHoldings Cph1WithAllowedSpeciesNotExpiredOrDeleted
        {
            get
            {
                var animalSpecies1Active = Species.AnimalSpecies1Active;
                var animalSpecies3Inactive = Species.AnimalSpecies3Inactive;

                var cph = new CountyParishHoldings()
                {
                    Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
                    Identifier = "44/100/0001",
                    ExpiredAt = null,
                    DeletedAt = null,
                    DeletedById = null,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

                cph.CountyParishHoldingAnimalSpecies =
                    new List<CountyParishHoldingAnimalSpecies>
                    {
                        new()
                        {
                            CountyParishHoldingId = cph.Id,
                            CountyParishHolding = cph,
                            AnimalSpecies = animalSpecies1Active,
                            AnimalSpeciesId = animalSpecies1Active.Id,
                        },
                        new()
                        {
                            CountyParishHoldingId = cph.Id,
                            CountyParishHolding = cph,
                            AnimalSpecies = animalSpecies3Inactive,
                            AnimalSpeciesId = animalSpecies3Inactive.Id,
                        },
                    };

                return cph;
            }
        }

        public static CountyParishHoldings Cph2ExpiredButNotDeleted => new()
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"),
            Identifier = "44/100/0002",
            ExpiredAt = ExpiredDateTime,
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph3DeletedButNotExpired => new()
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"),
            Identifier = "44/100/0003",
            DeletedById = OperatorId,
            DeletedAt = DeletedDateTime,
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph4NotExpiredOrDeleted => new()
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"),
            Identifier = "44/100/0004",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph5ExpiredButNotDeleted => new()
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"),
            Identifier = "44/100/0005",
            ExpiredAt = ExpiredDateTime,
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph6DeletedButNotExpired => new()
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"),
            Identifier = "44/100/0006",
            DeletedById = OperatorId,
            DeletedAt = DeletedDateTime,
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph7NotExpiredOrDeleted => new()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"),
            Identifier = "44/100/0007",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph8ExpiredAndDeleted => new()
        {
            Id = new Guid("d48e9292-a3d5-4191-9e8f-578e2eb1a32e"),
            Identifier = "44/100/0008",
            ExpiredAt = ExpiredDateTime,
            DeletedById = OperatorId,
            DeletedAt = DeletedDateTime,
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph9NotExpiredOrDeleted => new()
        {
            Id = new Guid("36a7fecb-1e4e-4e53-97af-87836d1854f2"),
            Identifier = "44/100/0009",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph10NotExpiredOrDeleted => new()
        {
            Id = new Guid("08d07366-1904-45a1-8972-1b748b738bdb"),
            Identifier = "44/100/0010",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph11NotExpiredOrDeleted => new()
        {
            Id = new Guid("83db4550-931b-4526-885d-59b286b3bc3f"),
            Identifier = "44/100/0011",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph12NotExpiredOrDeleted => new()
        {
            Id = new Guid("ade88411-f789-48e2-9e57-054702e54d24"),
            Identifier = "44/100/0012",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph13NotExpiredOrDeleted => new()
        {
            Id = new Guid("93b115a6-0b3c-45c7-a7d5-8e78f729e8e7"),
            Identifier = "44/100/0013",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph14NotExpiredOrDeleted => new()
        {
            Id = new Guid("001b22b8-0626-43fb-b820-cc98d0f6a154"),
            Identifier = "44/100/0014",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph15NotExpiredOrDeleted => new()
        {
            Id = new Guid("89117653-7147-46c9-8a64-c597894c559b"),
            Identifier = "44/100/0015",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };

        public static CountyParishHoldings Cph16NotExpiredOrDeleted => new()
        {
            Id = new Guid("3f592749-17dc-4e2a-ad15-b220bbb2b5a7"),
            Identifier = "44/100/0016",
            CreatedById = OperatorId,
            CreatedAt = CreateDateTime,
        };
    }

    public static class CphAssignments
    {
        public static class ToUser1NotDeleted
        {
            public static UserAccountCountyParishHoldingAssignments ToCph1Valid =>
                new()
                {
                    Id = new Guid("d10e15ac-8dc5-46eb-945a-09462f0b52f2"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments
                ToCph2ReferencedCphExpired =>
                new()
                {
                    Id = new Guid("b577f98d-f9d4-4bbc-a52b-26977bafa905"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph2ExpiredButNotDeleted.Id,
                    CountyParishHolding = Cph.Cph2ExpiredButNotDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments
                ToCph3ReferencedCphDeleted =>
                new()
                {
                    Id = new Guid("e6ff0fb3-da6b-4f07-bc05-87bd3782ed8b"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph2ExpiredButNotDeleted.Id,
                    CountyParishHolding = Cph.Cph2ExpiredButNotDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph4Valid =>
                new()
                {
                    Id = new Guid("0a46f578-aeca-49b5-aa62-7ab0688d6ae3"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph4NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph4NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph9Valid =>
                new()
                {
                    Id = new Guid("f896329f-2bbc-437c-a4d0-65a3ab891b6c"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph9NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph9NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph10Valid =>
                new()
                {
                    Id = new Guid("77a95b82-c473-4da9-9a2a-68ec05f46b71"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph10NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph10NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph11Valid =>
                new()
                {
                    Id = new Guid("24b54f38-e306-46c0-976e-310f01154510"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph11NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph11NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph7Deleted =>
                new()
                {
                    Id = new Guid("e0559d73-f215-4856-9bc4-113ab1024d2b"),
                    UserAccountId = User.User1NotDeleted.Id,
                    UserAccount = User.User1NotDeleted,
                    CountyParishHoldingId = Cph.Cph7NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph7NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                    DeletedAt = DeletedDateTime,
                    DeletedById = OperatorId,
                };
        }

        public static class ToUser2NotDeleted
        {
            public static UserAccountCountyParishHoldingAssignments ToCph13Valid =>
                new()
                {
                    Id = new Guid("9bcce9fd-3e64-4a28-a9f1-54037a2117f0"),
                    UserAccountId = User.User2NotDeleted.Id,
                    UserAccount = User.User2NotDeleted,
                    CountyParishHoldingId = Cph.Cph13NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph13NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph14Valid =>
                new()
                {
                    Id = new Guid("b34f386a-333f-4fb0-9e4d-d96c36aa9cd6"),
                    UserAccountId = User.User2NotDeleted.Id,
                    UserAccount = User.User2NotDeleted,
                    CountyParishHoldingId = Cph.Cph14NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph14NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };
        }

        public static class ToUser3Deleted
        {
            public static UserAccountCountyParishHoldingAssignments ToCph15Valid =>
                new()
                {
                    Id = new Guid("1a5866ee-0418-43b8-a74f-3445935c7688"),
                    UserAccountId = User.User3Deleted.Id,
                    UserAccount = User.User3Deleted,
                    CountyParishHoldingId = Cph.Cph15NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph15NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };

            public static UserAccountCountyParishHoldingAssignments ToCph16Valid =>
                new()
                {
                    Id = new Guid("badfb985-11ee-485e-aad3-7763ed7ac95c"),
                    UserAccountId = User.User3Deleted.Id,
                    UserAccount = User.User3Deleted,
                    CountyParishHoldingId = Cph.Cph16NotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph16NotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };
        }

        public static class ToUser5NotDeleted
        {
            public static UserAccountCountyParishHoldingAssignments ToCph1Valid =>
                new()
                {
                    Id = new Guid("811d0ecf-50c5-45f5-a149-c814d2b68e3f"),
                    UserAccountId = User.User5NotDeleted.Id,
                    UserAccount = User.User5NotDeleted,
                    CountyParishHoldingId = Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted.Id,
                    CountyParishHolding = Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
                    RoleId = Role.Role1.Id,
                    Role = Role.Role1,
                    CreatedById = OperatorId,
                    CreatedAt = CreateDateTime,
                };
        }
    }

    public static class CphDelegations
    {
        public static class FromUser1NotDeleted
        {
            public static class ToUser2NotDeleted
            {
                public static CountyParishHoldingDelegations
                    ToCph1ValidPendingWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("6fe7b77f-4a21-4119-bd8d-7b7e9ac35ceb"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations ToCph4ValidAcceptedWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph4NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("2e4f2852-9b50-43b0-9094-0cf66cb9870a"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations ToCph9ValidRejected
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph9NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("3344c2e2-07df-412a-b669-23a4236c211a"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationPastExpiryDateTime,
                            InvitationRejectedAt = InvitationPastRejectedDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations ToCph10ValidAcceptedWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph10NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("23961b4b-86f9-4043-98d6-4c19b03b844a"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationPastAcceptedDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidAcceptedInviteNowExpired
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("c1715376-94c3-4da1-b407-4dc8c766408b"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationPastExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidPendingInviteExpired
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("03425b53-f8e4-439f-a2d7-cf3c3293fa2f"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationPastExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidPendingDelegationExpired
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("12ef039e-7e18-4692-bd6e-bedf3ef23504"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = DelegationExpiryPastDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidAcceptedDelegationExpired
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("5bb6ba32-1c0d-4502-b329-6ad2fe7ff2fd"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = DelegationExpiryPastDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidPendingDelegationRevoked
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("22e3ea9c-c1cf-473f-a3b3-33bb27ad593d"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            RevokedAt = DelegationRevokedPastDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidAcceptedDelegationRevoked
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("f2068267-1230-4435-8377-d7eaa5b312fe"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            RevokedAt = DelegationRevokedPastDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidPendingDelegationDeleted
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("e802b8aa-d53a-43f0-8845-012efe1afcf4"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                            DeletedById = OperatorId,
                            DeletedAt = DeletedDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph11ValidAcceptedDelegationDeleted
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph11NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("5b6996ee-33ae-4915-98f4-334176564769"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                            DeletedById = OperatorId,
                            DeletedAt = DeletedDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph7CphAssignmentDeletedPending
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph7NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("80f8f96f-10b5-4d29-85b2-98c4f0f73937"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph2ReferencedCphExpiredPending
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph2ExpiredButNotDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("f8511d0a-0c63-493c-8a18-5a35c4c73293"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph3ReferencedCphDeletedPending
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph3DeletedButNotExpired.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("22156e9e-57c2-4fb2-b542-d28944e87654"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph12NoCphAssignmentsPending
                {
                    get
                    {
                        var targetCph = Cph.Cph12NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("731c6b9e-9081-4e0a-8acf-0b4af9cd9e63"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations
                    ToCph16CphAssignedToDeletedUser3Pending
                {
                    get
                    {
                        var targetCph = Cph.Cph16NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("69cb8a62-0631-4904-8d30-7b5769d1b0dc"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }
            }

            public static class ToUser4NotDeleted
            {
                public static CountyParishHoldingDelegations
                    ToCph1ValidPendingWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("1cacc1f0-6734-4bae-978f-ab06918c05fa"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User1NotDeleted.Id,
                            DelegatingUser = User.User1NotDeleted,
                            DelegatedUserId = User.User4NotDeleted.Id,
                            DelegatedUser = User.User4NotDeleted,
                            DelegatedUserEmail = User.User4NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }
            }
        }

        public static class FromUser2NotDeleted
        {
            public static class ToUser1NotDeleted
            {
                public static CountyParishHoldingDelegations
                    ToCph13ValidPendingWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph13NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("5ee1eeb0-755e-4fb2-86ba-306d561052d2"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User2NotDeleted.Id,
                            DelegatingUser = User.User2NotDeleted,
                            DelegatedUserId = User.User1NotDeleted.Id,
                            DelegatedUser = User.User1NotDeleted,
                            DelegatedUserEmail = User.User1NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }

                public static CountyParishHoldingDelegations ToCph14ValidAcceptedWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph14NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("2b9145c7-d946-4b44-ab52-efb3e6e9e4de"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User2NotDeleted.Id,
                            DelegatingUser = User.User2NotDeleted,
                            DelegatedUserId = User.User1NotDeleted.Id,
                            DelegatedUser = User.User1NotDeleted,
                            DelegatedUserEmail = User.User1NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            InvitationAcceptedAt = InvitationPastAcceptedDateTime,
                            ExpiresAt = DelegationExpiryFutureDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }
            }

            public static class ToUser3Deleted
            {
                public static CountyParishHoldingDelegations ToCph14ValidPendingWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph14NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("36a4e590-7a87-4b47-b930-54606c2357b4"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User2NotDeleted.Id,
                            DelegatingUser = User.User2NotDeleted,
                            DelegatedUserId = User.User3Deleted.Id,
                            DelegatedUser = User.User3Deleted,
                            DelegatedUserEmail = User.User3Deleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            ExpiresAt = null,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }
            }
        }

        public static class FromUser3Deleted
        {
            public static class ToUser2NotDeleted
            {
                public static CountyParishHoldingDelegations
                    ToCph15ValidPendingWithinInviteExpiry
                {
                    get
                    {
                        var targetCph =
                            Cph.Cph15NotExpiredOrDeleted.WithCphAssignmentsFromTestData();

                        return new CountyParishHoldingDelegations
                        {
                            Id = new Guid("e9903585-af7f-4fce-8da6-3577b275e9d3"),
                            CountyParishHoldingId = targetCph.Id,
                            CountyParishHolding = targetCph,
                            DelegatingUserId = User.User3Deleted.Id,
                            DelegatingUser = User.User3Deleted,
                            DelegatedUserId = User.User2NotDeleted.Id,
                            DelegatedUser = User.User2NotDeleted,
                            DelegatedUserEmail = User.User2NotDeleted.EmailAddress,
                            DelegatedUserRoleId = Role.Role1.Id,
                            DelegatedUserRole = Role.Role1,
                            InvitationToken = string.Empty,
                            InvitationExpiresAt = InvitationFutureExpiryDateTime,
                            CreatedById = OperatorId,
                            CreatedAt = CreateDateTime,
                        };
                    }
                }
            }
        }
    }

#pragma warning disable SA1201
    public static List<T> GetEntitiesOfType<T>()
#pragma warning restore SA1201
        where T : class
    {
        var entities = new List<T>();

        GetEntitiesOfType(typeof(TestData), entities);

        return entities;
    }

    private static void GetEntitiesOfType<T>(
        Type sourceDataType,
        List<T> cphAssignments)
        where T : class
    {
        const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;

        cphAssignments.AddRange(from prop in sourceDataType.GetProperties(bindingFlags)
            where prop.PropertyType == typeof(T)
            select prop.GetValue(null) as T
            into propertyValue
            select propertyValue!);

        foreach (var nestedType in sourceDataType.GetNestedTypes(bindingFlags))
        {
            GetEntitiesOfType(nestedType, cphAssignments);
        }
    }

    private static CountyParishHoldings WithCphAssignmentsFromTestData(this CountyParishHoldings cphEntity)
    {
        var cphAssignmentEntities = GetEntitiesOfType<UserAccountCountyParishHoldingAssignments>()
            .Where(x => x.CountyParishHoldingId == cphEntity.Id);

        cphEntity.ApplicationUserAccountHoldingAssignments = cphAssignmentEntities.ToList();

        return cphEntity;
    }
}
