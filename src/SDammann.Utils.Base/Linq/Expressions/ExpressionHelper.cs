namespace SDammann.Utils.Linq.Expressions {
    using System;
    using System.Linq.Expressions;
    using System.Reflection;


    /// <summary>
    ///   Helper class for expression trees
    /// </summary>
    public static class ExpressionHelper {
        /// <summary>
        ///   Gets an <see cref="PropertyInfo" /> object representing the property referenced by the expression specified
        /// </summary>
        /// <typeparam name="TObject"> </typeparam>
        /// <param name="expr"> </param>
        /// <returns> </returns>
        /// <exception cref="ArgumentException">Invalid expression</exception>
        public static PropertyInfo GetPropertyInfoFromExpression<TObject> (
                this Expression<Func<TObject, object>> expr) {
            Type type = typeof (TObject);

            // the compiler tends to create a obj => Convert(obj.X) expression
            //  from a obj => obj.X expression if the expression is of type Expression<Func<TObject, object>>
            //  but it doesn't have to be like this
            UnaryExpression convertExpression = expr.Body as UnaryExpression;

            MemberExpression member;
            if (convertExpression != null) {
                member = convertExpression.Operand as MemberExpression;
            } else {
                member = expr.Body as MemberExpression;
            }
            
            if (member == null) {
                throw new ArgumentException(string.Format(
                                                          "Expression '{0}' refers to a method, not a property.",
                                                          expr));
            }

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null) {
                throw new ArgumentException(string.Format(
                                                          "Expression '{0}' refers to a field, not a property.",
                                                          expr));
            }

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType)) {
                throw new ArgumentException(string.Format(
                                                          "Expresion '{0}' refers to a property that is not from type {1}.",
                                                          expr,
                                                          type));
            }

            return propInfo;
        }
    }
}