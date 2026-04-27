// <copyright file="MessageTemplateTypes.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models;

public static class MessageTemplateTypes
{
    public sealed class Delegation(string name, Guid value)
        : SmartEnum<Delegation, Guid>(name, value)
    {
        public static readonly Delegation DelegationInvitee = new(nameof(DelegationInvitee), Guid.Parse("9edac4c9-9b29-4ea6-9e37-6ed32776a943"));

        public static readonly Delegation DelegationInviteeConfirmation = new(nameof(DelegationInviteeConfirmation), Guid.Parse("a7a9d6f6-8174-441e-aeaf-1c70f82c799b"));

        public static readonly Delegation DelegationInviterConfirmation = new(nameof(DelegationInviterConfirmation), Guid.Parse("47be35e4-264f-4811-ba75-05838db07d94"));
    }
}
