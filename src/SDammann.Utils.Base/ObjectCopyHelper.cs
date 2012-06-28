namespace SDammann.Utils {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Linq.Expressions;


    /// <summary>
    ///   Static helper class for copying the values of one object's properties to another object
    /// </summary>
    public static class ObjectCopyHelper {
        /// <summary>
        /// Copies the public properties from the <paramref name="source"/> object to the <paramref name="destination"/> object.
        /// This method does not copy properties without public getters and setters or properties excluded by the <paramref name="excludedPropertySelectors"/> parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="excludedPropertySelectors">The excluded properties not to copy.</param>
        public static void Copy<T>(T source, T destination, params Expression<Func<T, object>>[] excludedPropertySelectors) where T : class {
            Type contextType = typeof (T);

            PropertyInfo[] allProperties = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            
            if (excludedPropertySelectors != null) {
                allProperties = allProperties.Except(
                    excludedPropertySelectors.Select(ExpressionHelper.GetPropertyInfoFromExpression)).ToArray();
            }

            foreach (PropertyInfo property in allProperties) {
                if (property.GetAccessors(false).Length == 2) {
                    try {
                        property.SetValue(destination, property.GetValue(source, null), null);
                    } catch (ArgumentException) {
                        // getter or setter problem
                    } catch (MethodAccessException) {
                        // getter or setter problem
                    }
                }
            }
        }

        /// <summary>
        /// Compares the properties of the objects specified and returns the names of the properties that are not equal
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item1"></param>
        /// <param name="item2"></param>
        /// <param name="excludedPropertySelectors"></param>
        /// <returns></returns>
        public static string[] Compare<T>(T item1, T item2, params Expression<Func<T, object>>[] excludedPropertySelectors) where T : class {
            Type comparerType = typeof(EqualityComparer<>);
            Type contextType = typeof(T);

            PropertyInfo[] allProperties = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (excludedPropertySelectors != null) {
                allProperties = allProperties.Except(
                    excludedPropertySelectors.Select(ExpressionHelper.GetPropertyInfoFromExpression)).ToArray();
            }

            List<string> notEqualProperties = new List<string>(allProperties.Length);

            foreach (PropertyInfo property in allProperties) {

                try {
                    Type specializedComparer = comparerType.MakeGenericType(property.PropertyType);
                    PropertyInfo defaultProperty = specializedComparer.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);

                    object comparer = defaultProperty.GetValue(null, null);
                    MethodInfo equalsMethod = specializedComparer.GetMethod("Equals",
                                                  BindingFlags.Public | BindingFlags.Instance,
                                                  null,
                                                  new[] {property.PropertyType, property.PropertyType},
                                                  null);

                    object item1Value = property.GetValue(item1, null);
                    object item2Value = property.GetValue(item2, null);

                    // do a
                    bool comparisonResult = (bool) equalsMethod.Invoke(comparer, new[] {item1Value, item2Value});
                    
                    if (!comparisonResult) {
                        notEqualProperties.Add(property.Name);
                    }
                } catch (ArgumentException) {
                    // getter problem
                } catch (MethodAccessException) {
                    // getter problem
                }
            }

            return notEqualProperties.ToArray();
        }
    }
}