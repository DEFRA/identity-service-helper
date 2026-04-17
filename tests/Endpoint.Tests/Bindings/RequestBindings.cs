// <copyright file="RequestBindings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Bindings;

using System;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Threading.Tasks;
using Defra.Identity.Endpoint.Tests.Configuration;
using Defra.Identity.Endpoint.Tests.Contexts;
using Defra.Identity.Endpoint.Tests.Extensions;
using Defra.Identity.Endpoint.Tests.Support;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Reqnroll;

/// <summary>
/// Collection of Request bindings.
/// </summary>
/// <param name="executionContext">The flurl context.</param>
/// <param name="exceptionContext">The exception context.</param>
/// <param name="configuration">The configuration instance.</param>
/// <param name="authContext">The auth context.</param>
/// <param name="stringProcessor">The string processor.</param>
/// <param name="outputHelper">The reqnroll output helper.</param>
[Binding]
public class RequestBindings(
    EndpointTestApplicationFactory applicationFactory,
    ExecutionContext executionContext,
    CaughtExceptionContext exceptionContext,
    IOptions<ApiConfiguration> configuration,
    AuthContext authContext,
    StringProcessor stringProcessor,
    IReqnrollOutputHelper outputHelper)
{
    /// <summary>
    /// Setup the .
    /// </summary>
    [Given(@"I am a {UserType} user")]
    [Given(@"I am an {UserType} user")]
    public void GivenIAmAUser(UserType userType)
    {
        authContext.SetupUser(userType);
    }

    [Given("the request url is {string}")]
    public void GivenTheRequestUrlIsString(string endpoint)
    {
        endpoint = stringProcessor.ProcessString(endpoint);
        outputHelper.WriteLine($"Parsed request URL: {endpoint}");

        executionContext.HttpRequestMessage = new HttpRequestMessage(
            new HttpMethod("GET"),
            endpoint);
    }

    [Given("I am using the default header settings")]
    public void GivenIAmUsingTheDefaultHeaderSettings()
    {
        var config = configuration.Value;
        executionContext.HttpRequestMessage.Headers.AddOrReplace("x-correlation-id", config.CorrelationId.ToString("D"));
        executionContext.HttpRequestMessage.Headers.AddOrReplace("x-api-key", config.ApiKey);
        executionContext.HttpRequestMessage.Headers.AddOrReplace("x-operator-id", config.OperatorId);
    }

    [Given("I am using the following headers:")]
    public void GivenIAmUsingTheFollowingHeaders(Table table)
    {
        table.ContainsColumn("header").ShouldBeTrue();
        table.ContainsColumn("value").ShouldBeTrue();

        foreach (var row in table.Rows)
        {
            executionContext.HttpRequestMessage.Headers.AddOrReplace(row["header"], row["value"]);
        }
    }

    [When("I '{VerbType}' this request")]
    public async Task WhenIActionTheRequest(VerbType requestedVerb)
    {
        try
        {
            var client = applicationFactory.CreateClient();
            executionContext.HttpRequestMessage.Method = new HttpMethod(requestedVerb.ToString());

            var response = await client
                .SendAsync(executionContext.HttpRequestMessage)
                .ConfigureAwait(false);

            executionContext.ResponseStatus = response.StatusCode;

            executionContext.ResponseContent = await response
                .Content
                .ReadAsStringAsync()
                .ConfigureAwait(false);

            outputHelper.WriteLine($"Received data:\n{executionContext.ResponseContent}\n*******END");
        }
        catch (Exception e)
        {
            exceptionContext.CaughtException = e;
            exceptionContext.ExceptionType = e.GetType();
        }
    }

    [Given("the JSON payload is:")]
    public void GivenTheJsonPayloadIs(string multilineText)
    {
        multilineText.ShouldNotBeNullOrWhiteSpace();
        JObject.Parse(multilineText); // This will throw if the JSON is invalid
        executionContext.RequestContentType = MediaTypeNames.Application.Json;
        executionContext.RequestContent = JsonContent.Create(multilineText);
    }

    [Given("the FORM encoded payload is:")]
    public void GivenTheFormEncodedPayloadIs(Table table)
    {
        table.ShouldNotBeNull();
        var tmp = table.Rows.Select(row => new KeyValuePair<string, string>(row["Key"], row["Value"]));
        executionContext.RequestContentType = MediaTypeNames.Multipart.FormData;
        executionContext.RequestContent = new FormUrlEncodedContent(tmp);
    }
}
