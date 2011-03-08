using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Primitives;

namespace MefContrib.Hosting.Interception.Tests
{
    public class PartFilter : IExportHandler
    {
        public void Initialize(ComposablePartCatalog interceptedCatalog)
        {
        }

        public IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition, IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> exports)
        {
            foreach (var export in exports)
            {
                if (export.Item1.Metadata.ContainsKey("ImportantPart") &&
                    export.Item1.Metadata["ImportantPart"].Equals(true))
                {
                    yield return export;
                }
            }
        }
    }

    public interface IPart
    {
        string Name { get; set; }
    }

    [Export(typeof(IPart))]
    [PartMetadata("ImportantPart", true)]
    public class Part0 : IPart
    {
        public string Name { get; set; }
    }

    [Export(typeof(IPart))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Part1 : IPart
    {
        public static int InstanceCount;

        public Part1()
        {
            InstanceCount++;
        }

        public string Name { get; set; }
    }

    [Export("part2", typeof(IPart))]
    [PartMetadata("metadata1", true)]
    public class Part2 : IPart
    {
        public string Name { get; set; }
    }

    [Export("part3", typeof(IPart))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Part3 : IPart
    {
        public static int InstanceCount;

        public Part3()
        {
            InstanceCount++;
        }

        public string Name { get; set; }
    }

    public class PartWrapper : IPart
    {
        public string Name
        {
            get { return Inner.Name; }
            set { Inner.Name = value; }
        }

        public IPart Inner { get; set; }
    }

    [Export]
    public class DisposablePart : IDisposable
    {
        public bool IsDisposed = false;
        
        public void Dispose()
        {
            IsDisposed = true;
        }
    }

    public class PartInterceptor : IExportedValueInterceptor
    {
        public object Intercept(object value)
        {
            return new PartWrapper { Inner = (IPart)value };
        }
    }

    public class GeneralInterceptor : IExportedValueInterceptor
    {
        public object Intercept(object value)
        {
            var part = value as IPart;
            if (part != null)
            {
                part.Name = "Name property is set be the interceptor.";
            }

            return value;
        }
    }

    #region Recomposable Parts

    public interface IRecomposablePart
    {
        int Count { get; set; }
    }

    [Export]
    public class RecomposablePartImporter
    {
        [ImportMany(AllowRecomposition = true)]
        public IRecomposablePart[] Parts { get; set; }
    }

    [Export(typeof(IRecomposablePart))]
    public class RecomposablePart1 : IRecomposablePart
    {
        public int Count { get; set; }
    }

    [Export(typeof(IRecomposablePart))]
    public class RecomposablePart2 : IRecomposablePart
    {
        public int Count { get; set; }
    }

    public class RecomposablePartInterceptor : IExportedValueInterceptor
    {
        public object Intercept(object value)
        {
            var part = value as IRecomposablePart;
            if (part != null)
            {
                part.Count++;
            }

            return value;
        }
    }

    #endregion

}