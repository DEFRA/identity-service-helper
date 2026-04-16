// <copyright file="PagedResponseBindings.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Bindings;

using Defra.Identity.Endpoint.Tests.Contexts;
using Defra.Identity.Models.Responses.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Reqnroll;

[Binding]
public class PagedResponseBindings(
    ExecutionContext executionContext,
    FeatureContext featureContext)
{
    [Then("the paged response is on page {int} has a total count of {int} and contains {int} objects")]
    public void ThenThePagedResponseIsOnPageIntAndContainsIntObjects(int pageNumber, int total, int objectCount)
    {
        pageNumber.ShouldBeGreaterThanOrEqualTo(1);
        objectCount.ShouldBeGreaterThanOrEqualTo(0);

        var result = GetPagedResults();
        result.ShouldNotBeNull();
        result.PageNumber.ShouldBe(pageNumber);
        result.TotalCount.ShouldBe(total);
        result.Items.Count().ShouldBe(objectCount);
    }

    [Then(
        "from the paged response I save the value of property {string} from instance {int} to the context using name {string}")]
    public void ThenFromThePagedResponseISaveTheValueOfPropertyStringFromInstanceIntToTheContextUsingNameString(
        string propertyName,
        int index,
        string storageName)
    {
        propertyName.ShouldNotBeNullOrWhiteSpace();
        index.ShouldBeGreaterThanOrEqualTo(0);
        storageName.ShouldNotBeNullOrWhiteSpace();

        var result = GetPagedResults();
        result.ShouldNotBeNull();

        ExtractPropertyValueToContext(propertyName, result.Items.ToArray()[index] as JObject, storageName);
    }

    private PagedResults<object>? GetPagedResults()
    {
        var pageData = executionContext.ResponseContent as JObject;
        pageData.ShouldNotBeNull();

        var result = JsonConvert.DeserializeObject<PagedResults<object>>(
            pageData.ToString(),
            new JsonSerializerSettings
            {
                ContractResolver =
                    new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy(), },
            });
        return result;
    }

    private void ExtractPropertyValueToContext(string propertyName, JObject? tmp, string storageName)
    {
        tmp.ShouldNotBeNull();
        tmp.ShouldContainKey(propertyName);

        featureContext.Set(tmp[propertyName]!.ToString(), storageName);
    }
}
