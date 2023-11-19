using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UserPermission.Domain.Core
{
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new List<Expression<Func<T, object>>>();
        public List<string> IncludeStrings { get; } = new List<string>();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public Expression<Func<T, object>> GroupBy { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; } = false;

        protected virtual Expression<Func<T, bool>> OrCriteria(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null && right == null) throw new ArgumentException("At least one argument must not be null");
            if (left == null) return right;
            if (right == null) return left;

            var parameter = Expression.Parameter(typeof(T), "p");
            var combined = new ParameterReplacer(parameter).Visit(Expression.OrElse(left.Body, right.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        protected virtual Expression<Func<T, bool>> AndCriteria(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            if (left == null && right == null) throw new ArgumentException("At least one argument must not be null");
            if (left == null) return right;
            if (right == null) return left;

            var parameter = Expression.Parameter(typeof(T), "p");
            var combined = new ParameterReplacer(parameter).Visit(Expression.AndAlso(left.Body, right.Body));
            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }

        protected virtual void SetCriteria(Expression<Func<T, bool>> criteria)
        {
            this.Criteria = criteria;
        }

        protected virtual void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            this.Includes.Add(includeExpression);
        }

        protected virtual void AddInclude(string includeString)
        {
            this.IncludeStrings.Add(includeString);
        }

        protected virtual void ApplyPaging(int skip, int take)
        {
            this.Skip = skip;
            this.Take = take;
            this.IsPagingEnabled = true;
        }

        protected virtual void SetOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            this.OrderBy = orderByExpression;
        }

        protected virtual void SetOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
        {
            this.OrderByDescending = orderByDescendingExpression;
        }

        protected virtual void SetGroupBy(Expression<Func<T, object>> groupByExpression)
        {
            this.GroupBy = groupByExpression;
        }

        class ParameterReplacer : ExpressionVisitor
        {
            readonly ParameterExpression parameter;

            internal ParameterReplacer(ParameterExpression parameter)
            {
                this.parameter = parameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return this.parameter;
            }
        }
    }
}
