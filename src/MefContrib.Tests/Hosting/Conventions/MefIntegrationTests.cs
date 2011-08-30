using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using MefContrib.Hosting.Conventions.Configuration;

namespace MefContrib.Hosting.Conventions.Tests
{
    using System.ComponentModel.Composition;
    using System.Linq;
    using MefContrib.Tests;
    using NUnit.Framework;

    [TestFixture]
    public class MefIntegrationTests
    {
        [Test]
        public void Registry_should_work()
        {
            var registry =
                new FakeConventionRegistry();

            var conventions =
                registry.GetConventions();

            conventions.Count().ShouldEqual(3);
        }

        [Test]
        public void ConventionCatalog_should_import_closed_generic()
        {
            var exportConvention =
                new ExportConvention
                {
                    Members = t => new[] { t },
                    ContractType = x => typeof(ConventionPart2),
                };

            var importConvention =
                new ImportConvention
                {
                    Members = t => new[] { ReflectionServices.GetProperty<ConventionPart2>(p => p.Repository) },
                    ContractType = x => typeof(IRepository<string>)
                };

            var convention =
                new PartConvention();

            convention.Imports.Add(importConvention);
            convention.Exports.Add(exportConvention);
            convention.Condition = t => t == typeof(ConventionPart2);

            var model =
                new FakePartRegistry2(convention);

            // Setup container
            ConventionCatalog conventionCatalog =
                new ConventionCatalog(model);

            var typeCatalog =
                new TypeCatalog(typeof(AttributedClosedRepository), typeof(AttributedPart2));

            var aggregated =
                new AggregateCatalog(typeCatalog, conventionCatalog);

            var container =
                new CompositionContainer(aggregated);

            var part = new AttributedPart2();

            var batch =
                new CompositionBatch();
            batch.AddPart(part);

            container.Compose(batch);

            // Assert
            part.Part.ShouldNotBeNull();
            part.Part.Repository.ShouldNotBeNull();
        }

        [Test]
        public void ConventionCatalog_should_export_conventionpart()
        {
            // Setup conventions using the semantic model
            // This is NOT the API that the user will be exposed to,
            // there will be a DSL at the front

            var exportConvention =
                new ExportConvention
                {
                    Members = t => new[] { t },
                    ContractType = x => typeof(IConventionPart),
                };

            var importConvention =
                new ImportConvention
                {
                    Members = t => new[] { ReflectionServices.GetProperty<IConventionPart>(p => p.Logger) },
                    ContractType = x => typeof(ILogger)
                };

            var convention =
                new PartConvention();

            convention.Imports.Add(importConvention);
            convention.Exports.Add(exportConvention);
            convention.Condition = t => t.GetInterfaces().Contains(typeof(IConventionPart));

            var exportConvention2 =
                new ExportConvention
                {
                    Members = t => new[] { typeof(NullLogger) },
                    ContractType = x => typeof(ILogger),
                };

            var convention2 =
                new PartConvention();
            convention2.Exports.Add(exportConvention2);
            convention2.Condition = t => t.GetInterfaces().Contains(typeof(ILogger));

            var model =
                new FakePartRegistry2(convention, convention2);

            // Setup container
            ConventionCatalog conventionCatalog =
                new ConventionCatalog(model);

            var typeCatalog =
                new TypeCatalog(typeof(AttributedPart));

            var aggregated =
                new AggregateCatalog(typeCatalog, conventionCatalog);

            var container =
                new CompositionContainer(aggregated);

            var part = new AttributedPart();

            var batch =
                new CompositionBatch();
            batch.AddPart(part);

            container.Compose(batch);

            // Assert
            part.Part.Count().ShouldEqual(2);
            part.Part[0].Logger.ShouldNotBeNull();
            part.Part[1].Logger.ShouldNotBeNull();
        }
    }

    public class FakePartRegistry2 : IPartRegistry<DefaultConventionContractService>
    {
        private readonly IPartConvention[] conventions;

        public FakePartRegistry2(params IPartConvention[] conventions)
        {
            this.conventions = conventions;
            ContractService = new DefaultConventionContractService();
            TypeScanner = new TypeScanner(new[]
            {
                typeof(ConventionPart),
                typeof(ConventionPart2),
                typeof(ConventionPart3),
                typeof(OtherConventionPart),
                typeof(NullLogger)
            });
        }

        public IEnumerable<IPartConvention> GetConventions()
        {
            return this.conventions;
        }

        public DefaultConventionContractService ContractService { get; private set; }

        public ITypeScanner TypeScanner { get; private set; }
    }

    public class AttributedPart
    {
        [ImportMany(typeof(IConventionPart))]
        public IConventionPart[] Part { get; set; }
    }

    public interface IConventionPart
    {
        string Name { get; }

        ILogger Logger { get; set; }
    }

    public class ConventionPart : IConventionPart
    {
        public string Name
        {
            get { return "ConventionPart"; }
        }

        public ILogger Logger { get; set; }
    }

    public class OtherConventionPart : IConventionPart
    {
        public string Name
        {
            get { return "OtherConventionPart"; }
        }

        public ILogger Logger { get; set; }
    }

    public interface ILogger
    {
        void Add();
    }

    public class NullLogger : ILogger
    {
        public void Add()
        {
        }
    }

    public interface IRepository<T> { }

    [Export(typeof(IRepository<string>))]
    public class AttributedClosedRepository : IRepository<string> { }

    [Export]
    public class AttributedPart2
    {
        [Import]
        public ConventionPart2 Part { get; set; }
    }

    public class ConventionPart2
    {
        public IRepository<string> Repository { get; set; }
    }

    public class ConventionPart3
    {
        public ConventionPart3(IRepository<string> repository)
        {
            Repository = repository;
        }

        public IRepository<string> Repository { get; private set; }
    }
}