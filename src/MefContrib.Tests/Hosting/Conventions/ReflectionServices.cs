namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class ReflectionServices
    {
        public static FieldInfo GetField<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();

            var unary =
                expression.Body as UnaryExpression;

            return ((MemberExpression)unary.Operand).Member as FieldInfo;
        }

        public static MethodInfo GetMethod<T>(Expression <Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();

            var method =
                expression.Body as MethodCallExpression;

            return method.Method;
        }

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();

            var method =
                expression.Body as MethodCallExpression;

            return method.Method;
        }

        public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException();

            if (expression.Body.GetType().Equals(typeof(UnaryExpression)))
                return ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member as PropertyInfo;  

            return ((MemberExpression)expression.Body).Member as PropertyInfo;
        }
    }
}