
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Web.Http.OData;


namespace KMDRecociliationApp.Data.Common
{


   
        public class ObjectQuery<T>
        {

            public ObjectQuery()
            {
            }
            public IEnumerable<T> GetAllByFilter(int pageNumber, int pageSize, string sortProperty, string sortOrder, Delta filter, IQueryable<T> queryableObject, string objName, out int totalRecordCount)
            {
                try
                {
                    ParameterExpression pe = Expression.Parameter(typeof(T), objName);
                    Expression whereExpression = null;// objName == "waiver" ? null : objName == "User" ? BuildWhereExpression(pe, "IsActive", (byte)1, typeof(byte)) : BuildWhereExpression(pe, "IsActive", true, typeof(bool?));
                    if (filter != null)
                    {
                        foreach (string propertyName in filter.GetChangedPropertyNames())
                        {
                            object propertyValue;
                            Type propertyType;
                            bool found = filter.TryGetPropertyValue(propertyName, out propertyValue);
                            found = filter.TryGetPropertyType(propertyName, out propertyType);
                            Expression e1;
                            if (propertyType == typeof(string))
                            {
                                e1 = StringSearch(pe, propertyName, propertyValue);
                            }
                            else
                            {
                                e1 = BuildWhereExpression(pe, propertyName, propertyValue, propertyType);
                            }
                            if (whereExpression == null)
                            {
                                whereExpression = e1;
                            }
                            else
                            {
                                whereExpression = Expression.AndAlso(whereExpression, e1);
                            }
                        }
                    }
                    var objectFiltered = queryableObject;
                    if (whereExpression != null)
                    {
                        MethodCallExpression whereCallExpression = Expression.Call(
                        typeof(Queryable),
                        "Where",
                        new Type[] { queryableObject.ElementType },
                        queryableObject.Expression,
                        Expression.Lambda<Func<T, bool>>(whereExpression, new ParameterExpression[] { pe }));

                        objectFiltered = queryableObject.Provider.CreateQuery<T>(whereCallExpression);
                    }


                    var restrictions = SortOrder(objectFiltered, sortProperty, sortOrder);

                    totalRecordCount = restrictions.Count();

                    return pageSize == 0 ? restrictions.ToList() : restrictions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                }
                catch (Exception ex)
                {
                    Log.Fatal("Error " + ex);
                }

                totalRecordCount = 0;
                return Enumerable.Empty<T>();
            }


            private Expression BuildWhereExpression(ParameterExpression pe, string propertyName, object propertyValue, Type propertyType)
            {
                try
                {
                    Expression left = Expression.Property(pe, propertyName);
                    Expression right = Expression.Constant(propertyValue.ToString().ToLower(), propertyType == typeof(System.DateTime) ? typeof(DateTime?) : propertyType);
                    return Expression.Equal(left, right);
                }
                catch (Exception ex)
                {
                    Log.Fatal("Error " + ex);
                    return null;
                }
            }
            private Expression StringSearch(ParameterExpression pe, string propertyName, object propertyValue)
            {
                var type = typeof(T);
                var property = type.GetProperty(propertyName);
                MemberExpression propertyAccess = Expression.MakeMemberAccess(pe, property);
                // Call ToLower on the property value
                var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                var m = Expression.Call(propertyAccess, toLowerMethod);
                ConstantExpression c = Expression.Constant(propertyValue.ToString().ToLower(), typeof(string));
                MethodInfo mi = typeof(string).GetMethod("Contains", new Type[] { typeof(string)

            });
                Expression call = Expression.Call(m, mi, c);
                return call;
            }

            public string FirstCharSubstring(string input)
            {
                if (string.IsNullOrEmpty(input))
                {
                    return string.Empty;
                }
                return $"{input[0].ToString().ToUpper()}{input.Substring(1)}";
            }
            private IQueryable<T> SortOrder(IQueryable<T> source, string sortProperty, string sortOrder)
            {
                var type = typeof(T);
                var property = type.GetProperty(FirstCharSubstring(sortProperty));
                var parameter = Expression.Parameter(type, "param");
                var propertyAccess = Expression.MakeMemberAccess(parameter, property);
                var orderByExp = Expression.Lambda(propertyAccess, parameter);
                var typeArguments = new Type[] { type, property.PropertyType };
                var methodName = sortOrder == "asc" ? "OrderBy" : "OrderByDescending";
                var resultExp = Expression.Call(typeof(Queryable), methodName, typeArguments, source.Expression, Expression.Quote(orderByExp));
                return source.Provider.CreateQuery<T>(resultExp);
            }
        }
   

}
