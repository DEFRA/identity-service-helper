// <copyright file="ResponseBindings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Bindings;

using System.Net;
using Defra.Identity.Endpoint.Tests.Contexts;
using Defra.Identity.Endpoint.Tests.Support;
using Newtonsoft.Json.Linq;
using Reqnroll;

/// <summary>
/// Methods of response bindings.
/// </summary>
[Binding]
public class ResponseBindings(
    ExecutionContext executionContext,
    CaughtExceptionContext exceptionContext,
    FeatureContext featureContext,
    StringProcessor stringProcessor)
{
    /// <summary>
    /// Check that the HTTP status code is the passed in value.
    /// </summary>
    /// <param name="expectedHttpStatusCode">The HTTP status code to check.</param>
    [Then(@"I receive the HTTP status code '{HttpStatusCode}'")]
    public void ThenIReceiveTheHttpStatusCode(HttpStatusCode expectedHttpStatusCode)
    {
        executionContext.ShouldNotBeNull();
        executionContext.ResponseStatus.ShouldBe(expectedHttpStatusCode);
        exceptionContext.CaughtException.ShouldBeNull();
    }

    [Then("I have received JSON data in the response")]
    public void ThenIHaveReceivedJsonDataInTheResponse()
    {
        executionContext.ResponseContent.ShouldNotBeNull();

        var tmp = executionContext.ResponseContent.ToString()!.Trim();
        executionContext.ResponseContent = tmp[0] switch
        {
            '{' => executionContext.ResponseContent = JObject.Parse(tmp),
            '[' => executionContext.ResponseContent = JArray.Parse(tmp),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    [Then("I have an empty response")]
    public void ThenIHaveAnEmptyResponse()
    {
        executionContext.ResponseContent.ShouldNotBeNull();
    }

    [Then("I have received an error response")]
    public void ThenIHaveReceivedAnErrorResponse()
    {
        executionContext.ResponseContent.ShouldNotBeNull();

        var tmp = executionContext.ResponseContent.ToString()!.Trim();
        executionContext.ResponseContent = tmp[0] switch
        {
            '{' => executionContext.ResponseContent = JObject.Parse(tmp),
            '[' => executionContext.ResponseContent = JArray.Parse(tmp),
            _ => throw new ArgumentOutOfRangeException(),
        };
    }

    [Then("I save the value of property {string} to the context using name {string}")]
    public void ThenISaveThePropertyToTheContext(string propertyName, string storageName)
    {
        propertyName.ShouldNotBeNullOrWhiteSpace();
        storageName.ShouldNotBeNullOrWhiteSpace();

        ExtractPropertyValueToContext(propertyName, executionContext.ResponseContent as JObject, storageName);
    }

    [Then("I save the value of property {string} from instance {int} to the context using name {string}")]
    public void ThenISaveTheValueOfPropertyStringFromInstanceIntToTheContextUsingNameString(
        string propertyName,
        int index,
        string storageName)
    {
        propertyName.ShouldNotBeNullOrWhiteSpace();
        index.ShouldBeGreaterThanOrEqualTo(0);
        storageName.ShouldNotBeNullOrWhiteSpace();

        var tmp = executionContext.ResponseContent as JArray;
        tmp.ShouldNotBeNull();
        index.ShouldBeLessThan(tmp.Count);

        ExtractPropertyValueToContext(propertyName, tmp[index] as JObject, storageName);
    }

    [Then("I the value of property {string} is the same as the context property {string} value")]
    public void ThenITheValueOfPropertyStringIsTheSameAsTheContextPropertyStringValue(
        string propertyName,
        string contextPropertyName)
    {
        propertyName.ShouldNotBeNullOrWhiteSpace();
        contextPropertyName.ShouldNotBeNullOrWhiteSpace();

        var tmp = executionContext.ResponseContent as JObject;
        tmp.ShouldNotBeNull();
        tmp.ShouldContainKey(propertyName);

        var propertyValue = tmp[propertyName]!.ToString();
        var contextValue = featureContext.Get<string>(contextPropertyName);

        propertyValue.ShouldBe(contextValue);
    }

    [Then("the value of property {string} is the same as {string}")]
    public void ThenTheValueOfPropertyStringIsTheSameAsString(
        string propertyName,
        string valueToCheck)
    {
        propertyName.ShouldNotBeNullOrWhiteSpace();
        valueToCheck.ShouldNotBeNull();

        var tmp = executionContext.ResponseContent as JObject;
        tmp.ShouldNotBeNull();
        tmp.ShouldContainKey(propertyName);

        var propertyValue = tmp[propertyName]!.ToString();
        propertyValue.ShouldBe(valueToCheck);
    }

    [Then("the response contains the following values:")]
    public void ThenTheResponseContainsTheFollowingValues(Reqnroll.Table table)
    {
        table.ShouldNotBeNull();
        table.ContainsColumn("path").ShouldBeTrue();
        table.ContainsColumn("value").ShouldBeTrue();
        executionContext.ResponseContent.ShouldNotBeNull();
        var tmp = JObject.Parse(executionContext.ResponseContent.ToString()!);
        tmp.ShouldNotBeNull();

        foreach (var row in table.Rows)
        {
            var jsonPath = row["path"];
            jsonPath.ShouldNotBeNullOrWhiteSpace();

            var expectedValue = stringProcessor.ProcessString(row["value"]);
            expectedValue.ShouldNotBeNull();

            var token = tmp.SelectToken(jsonPath);
            token.ShouldNotBeNull();

            var actualValue = token.ToString();
            actualValue.ShouldBe(expectedValue);
        }
    }

    private void ExtractPropertyValueToContext(string propertyName, JObject? tmp, string storageName)
    {
        tmp.ShouldNotBeNull();
        tmp.ShouldContainKey(propertyName);

        featureContext.Set(tmp[propertyName]!.ToString(), storageName);
    }
}
