using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KMDRecociliationApp.Data.Common
{
    public enum SearchMethod
    {
        Contains,
        StartsWith,
        EndsWith
    }
    public static class DbContextExtensions
    {
        public static IQueryable<T> Search<T>(this IQueryable<T> source, string searchValue,  params string[] propertyNames) where T : class
        {
            var searchMethods = new List<SearchMethod> { SearchMethod.Contains, SearchMethod.StartsWith, SearchMethod.EndsWith };
            var parameter = Expression.Parameter(typeof(T), "x");
            var searchValueExpression = Expression.Constant(searchValue, typeof(string));

            var methodCalls = new List<Expression>();

            foreach (var propertyName in propertyNames)
            {
                var property = Expression.Property(parameter, propertyName);

                foreach (var searchMethod in searchMethods)
                {
                    MethodInfo method = null;

                    switch (searchMethod)
                    {
                        case SearchMethod.Contains:
                            method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                            break;
                        case SearchMethod.StartsWith:
                            method = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                            break;
                        case SearchMethod.EndsWith:
                            method = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                            break;
                    }

                    if (method != null)
                    {
                        var methodCall = Expression.Call(property, method, searchValueExpression);
                        methodCalls.Add(methodCall);
                    }
                }
            }

            if (methodCalls.Count == 0)
            {
                throw new ArgumentException("No valid properties or search methods provided.");
            }

            // Combine all method calls with OR
            var orExpression = methodCalls.Aggregate(Expression.OrElse);

            var lambda = Expression.Lambda<Func<T, bool>>(orExpression, parameter);

            return source.Where(lambda);
        }
    }
}
