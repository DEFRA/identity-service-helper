// <copyright file="ExpressionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Extensions;

using System.Linq.Expressions;

public static class ExpressionExtensions
{
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            var parameter = Expression.Parameter(typeof(T), "x");

            var leftBody = new ReplaceParameterVisitor(left.Parameters[0], parameter).Visit(left.Body)!;
            var rightBody = new ReplaceParameterVisitor(right.Parameters[0], parameter).Visit(right.Body)!;

            return Expression.Lambda<Func<T, bool>>(
                Expression.AndAlso(leftBody, rightBody),
                parameter);
        }

        private sealed class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression from;
            private readonly ParameterExpression to;

            public ReplaceParameterVisitor(ParameterExpression from, ParameterExpression to)
            {
                this.from = from;
                this.to = to;
            }

            protected override Expression VisitParameter(ParameterExpression node)
                => node == from ? to : base.VisitParameter(node);
        }
    }
