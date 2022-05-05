using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;

public static class PredicateBuilder
{
    public static Expression<Func<T, bool>> True<T>() { return f => true; }
    public static Expression<Func<T, bool>> False<T>() { return f => false; }

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
              (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                         Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        return Expression.Lambda<Func<T, bool>>
              (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }

    /// <summary>
    /// Returns the where clause expression for LINQ to entity queries. 
    /// It constructs property comparison expression for the dynamically generated properties from the reflected types of the property.
    /// Takes three arguments, First argument is value to be compared, Second argument is the optional property argument, 
    /// and third argument is the optional string type property name. Either of second or third argument is compulsary else method would return null.
    /// </summary>
    /// <typeparam name="TItem">The containing type of the property. passed using generic invokation of the method.</typeparam>
    /// <param name="value">Value of the property to be compared at runtime</param>
    /// <param name="prop">Optional, actual PropertyInfo object of the containing type.</param>
    /// <param name="propName">Optional, name of the property to check</param>
    /// <returns>Returns LINQ expression to be utilised in the where clause.</returns>
    public static Expression<Func<T, bool>> expressPropertyCompare<T>(object value, PropertyInfo prop = null, string propName = "")
    {
        if ((prop == null && string.IsNullOrEmpty(propName)) || value == null) return PredicateBuilder.False<T>();//return null;

        if (prop == null)
        {
            prop = typeof(T).GetProperty(propName);
            if (prop == null) throw new InvalidOperationException("Property not found in the given type, please check to see the property name passed is present in the Entity.");
        }

        Type t = prop.PropertyType;//Nullable.GetUnderlyingType(prop.PropertyType)??
        //var changedVal = value == null ? null : Convert.ChangeType(value, t);

        //if (changedVal == null) throw new InvalidCastException("Value cast failed to the type of property passed as argument.");

        var param = Expression.Parameter(typeof(T));
        var expProp = Expression.Property(param, prop);
        var val = Expression.Constant(value);
        var expressionVal = Expression.Convert(val, t);
        var body = Expression.Equal(expProp, expressionVal);

        return Expression.Lambda<Func<T, bool>>(body, param);
    }

    public static Expression<Func<T, bool>> BuildPredicateFromList<T>(List<KeyValuePair<string, object>> lstPredicates)
    {
        //Returning true so that the empty list of predicates doesn't affect the query output.
        if (lstPredicates == null || lstPredicates.Count == 0) return True<T>();

        var predExpression = True<T>();

        foreach (var predicate in lstPredicates)
        {
            predExpression = predExpression.And(expressPropertyCompare<T>(predicate.Value, null, predicate.Key));
        }

        return predExpression;
    }
}

//private static Expression<Func<T, bool>> constructPredicate<T>(SelectionCriteria selectionCriteria)
//{
//    var predicate = PredicateBuilderEx.True<T>();
//    var foo = PredicateBuilder.True<T>();

//    foreach (var item in selectionCriteria.andList)
//    {
//        var fieldName = item.fieldName;
//        var fieldValue = item.fieldValue;

//        var parameter = Expression.Parameter(typeof(T), "t");
//        var property = Expression.Property(parameter, fieldName);
//        var value = Expression.Constant(fieldValue);
//        var converted = Expression.Convert(value, property.Type);

//        var comparison = Expression.Equal(property, converted);
//        var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);

//        predicate = PredicateBuilderEx.And(predicate, lambda);
//    }

//    return predicate;
//}