// <copyright file="ExpressionExtensionsTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Extensions;

using System;
using System.Linq.Expressions;
using Defra.Identity.Services.Extensions;
using Shouldly;
using Xunit;

public class ExpressionExtensionsTests
{
    [Fact]
    public void AndAlso_TwoExpressions_CombinesThem()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> left = x => x.Id == 1;
        Expression<Func<TestEntity, bool>> right = x => x.Name == "Test";

        // Act
        var combined = left.AndAlso(right);
        var compiled = combined.Compile();

        // Assert
        compiled(new TestEntity { Id = 1, Name = "Test" }).ShouldBeTrue();
        compiled(new TestEntity { Id = 2, Name = "Test" }).ShouldBeFalse();
        compiled(new TestEntity { Id = 1, Name = "Other" }).ShouldBeFalse();
        compiled(new TestEntity { Id = 2, Name = "Other" }).ShouldBeFalse();
    }

    [Fact]
    public void AndAlso_LeftIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> left = null!;
        Expression<Func<TestEntity, bool>> right = x => x.Name == "Test";

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => left.AndAlso(right))
            .ParamName.ShouldBe("left");
    }

    [Fact]
    public void AndAlso_RightIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> left = x => x.Id == 1;
        Expression<Func<TestEntity, bool>> right = null!;

        // Act & Assert
        Should.Throw<ArgumentNullException>(() => left.AndAlso(right))
            .ParamName.ShouldBe("right");
    }

    [Fact]
    public void AndAlso_ComplexExpressions_CombinesThem()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> left = x => x.Id > 10 && x.Name.StartsWith("A");
        Expression<Func<TestEntity, bool>> right = x => x.Id < 20 || x.Name.EndsWith("Z");

        // Act
        var combined = left.AndAlso(right);
        var compiled = combined.Compile();

        // Assert
        compiled(new TestEntity { Id = 15, Name = "Apple" }).ShouldBeTrue(); // (15 > 10 && "Apple".StartsWith("A")) AND (15 < 20 || "Apple".EndsWith("Z")) => True AND True => True
        compiled(new TestEntity { Id = 5, Name = "Apple" }).ShouldBeFalse(); // (5 > 10 && "Apple".StartsWith("A")) => False
        compiled(new TestEntity { Id = 15, Name = "Banana" }).ShouldBeFalse(); // (15 > 10 && "Banana".StartsWith("A")) => False
        compiled(new TestEntity { Id = 25, Name = "AZ" }).ShouldBeTrue(); // (25 > 10 && "AZ".StartsWith("A")) AND (25 < 20 || "AZ".EndsWith("Z")) => True AND True => True
    }

    private class TestEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}
