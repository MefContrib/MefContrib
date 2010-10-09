namespace MefContrib.Hosting.Conventions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Moq;
    using NUnit.Framework;

    public class AggregatedTypeScannerTests
    {
        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_null_param_array()
        {
            var exception =
                Catch.Exception(() => new AggregatedTypeScanner((ITypeScanner[])null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Ctor_should_throw_argument_null_exception_when_called_with_null_enumerable()
        {
            var exception =
                Catch.Exception(() => new AggregatedTypeScanner((IEnumerable<ITypeScanner>)null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void GetTypes_should_evaluate_predicate_on_all_aggregated_type_scanners()
        {
            var firstScanner =
                new Mock<ITypeScanner>();
            firstScanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>())).Verifiable();

            var secondScanner =
                new Mock<ITypeScanner>();
            secondScanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>())).Verifiable();

            var scanner =
                new AggregatedTypeScanner(firstScanner.Object, secondScanner.Object);

            scanner.GetTypes(p => true);

            firstScanner.Verify();
            secondScanner.Verify();
        }

        [Test]
        public void GetTypes_should_return_types_from_all_scanners_that_match_the_predicate()
        {
            var firstScanner =
                new Mock<ITypeScanner>();
            firstScanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>()))
                .Returns(new List<Type> { typeof(object) });

            var secondScanner =
                new Mock<ITypeScanner>();
            secondScanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>()))
                .Returns(new List<Type> { typeof(int), typeof(string) }); ;

            var scanner =
                new AggregatedTypeScanner(firstScanner.Object, secondScanner.Object);

            var results =
                scanner.GetTypes(p => true);

            results.Count().ShouldEqual(3);
        }

        [Test]
        public void GetTypes_should_return_distinct_types()
        {
            var scanner =
                new AggregatedTypeScanner();

            scanner.Add(new TypeScanner(new[] { typeof(string) }));
            scanner.Add(new TypeScanner(new[] { typeof(string) }));

            var results =
                scanner.GetTypes(x => true);

            results.Count().ShouldEqual(1);
        }

        [Test]
        public void Add_should_throw_argumentnullexception_when_called_with_null()
        {
            var scanner =
                new AggregatedTypeScanner();

            var exception =
                Catch.Exception(() => scanner.Add(null));

            exception.ShouldBeOfType<ArgumentNullException>();
        }

        [Test]
        public void Add_should_add_type_scanner_to_collection_of_scanners_that_are_queried_by_gettypes()
        {
            var scanner =
                new AggregatedTypeScanner();

            var mockScanner =
                new Mock<ITypeScanner>();
            mockScanner.Setup(x => x.GetTypes(It.IsAny<Predicate<Type>>())).Verifiable();

            scanner.Add(mockScanner.Object);

            var results =
                scanner.GetTypes(p => true);

            mockScanner.Verify();
        }
    }
}