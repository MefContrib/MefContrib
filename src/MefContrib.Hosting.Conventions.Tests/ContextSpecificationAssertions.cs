namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using NUnit.Framework;

    /*
    public static class ContextSpecificationAssertions
    {
        public static void ShouldMatch<T>(this T actual, Func<T, bool> condition)
        {
            Assert.True(condition.Invoke(actual));
        }

        public static void ShouldContainType<T>(this IList collection)
        {
            var selection =
                from c in collection.Cast<object>()
                where c.GetType().IsAssignableFrom(typeof(T))
                select c;

            Assert.True(selection.Count() > 0);
        }

        public static void ShouldHaveCount<T>(this IList<T> list, int expected)
        {
            list.Count.ShouldEqual(expected);
        }

        public static void ShouldBeTrue(this bool actual)
        {
            Assert.True(actual);
        }

        public static void ShouldBeFalse(this bool actual)
        {
            Assert.False(actual);
        }

        public static void ShouldEqual(this object actual, object expected)
        {
            Assert.AreEqual(expected, actual);
        }

        public static void ShouldNotEqual(this object actual, object expected)
        {
            Assert.AreNotEqual(expected, actual);
        }

        public static void ShouldNotBeSameAs(this object actual, object expected)
        {
            Assert.AreNotSame(expected, actual);
        }

        public static void ShouldBeSameAs(this object actual, object expected)
        {
            Assert.AreSame(expected, actual);
        }

        public static void ShouldBeNull(this object actual)
        {
            Assert.Null(actual);
        }

        public static void ShouldNotBeNull(this object actual)
        {
            Assert.NotNull(actual);
        }

        public static void ShouldBeOfType<T>(this Type asserted)
        {
            Assert.True(asserted == typeof(T));
        }

        public static void ShouldBeOfType<T>(this object asserted)
        {
            asserted.ShouldBeOfType(typeof(T));
        }

        public static void ShouldBeOfType(this object asserted, Type expected)
        {
            Assert.IsInstanceOf(expected, asserted);
        }

        public static void ShouldNotBeOfType<T>(this object assertedType)
        {
            if (assertedType != null)
            {
                Assert.IsInstanceOf(typeof(T), assertedType);
            }
        }

        public static void ShouldBeThrownBy(this Type expectedType, Action context)
        {
            Exception exception = null;

            try
            {
                context();
            }
            catch (Exception thrownException)
            {
                exception = thrownException;
                Assert.AreEqual(expectedType, thrownException.GetType());
            }
        }
    }
     * */
}