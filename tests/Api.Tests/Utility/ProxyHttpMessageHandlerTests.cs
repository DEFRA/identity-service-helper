// <copyright file="ProxyHttpMessageHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Utility;

using Defra.Identity.Api.Utility.Utils.Http;

public class ProxyHttpMessageHandlerTests
{
    [Fact]
    public void ExtractsCredentialsFromUri()
    {
        var creds = ProxyHttpMessageHandler.GetCredentialsFromUri(
            new UriBuilder("http://username:password@www.example.com"));
        Assert.NotNull(creds);
        Assert.Equal("username", creds.UserName);
        Assert.Equal("password", creds.Password);
    }

    [Fact]
    public void DoNotExtractCredentialsFromUriWithoutThem()
    {
        var creds = ProxyHttpMessageHandler.GetCredentialsFromUri(
            new UriBuilder("http://www.example.com"));
        Assert.Null(creds);
    }
}
