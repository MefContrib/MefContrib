namespace MefContrib.Integration.Autofac.Tests
{
    using System.ComponentModel.Composition;

    public interface IMefComponent
    {
        void Foo();
    }

    [Export(typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent1 : IMefComponent
    {
        public static int InstanceCount;

        public MefComponent1()
        {
            InstanceCount++;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent2 : IMefComponent
    {
        public void Foo()
        {
        }
    }

    public interface IMefComponentWithAutofacDependencies
    {
        IAutofacOnlyComponent AutofacOnlyComponent { get; set; }
        IMefComponent MefOnlyComponent { get; set; }
        void Foo();
    }

    [Export(typeof(IMefComponentWithAutofacDependencies))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponentWithAutofacDependencies1 : IMefComponentWithAutofacDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IAutofacOnlyComponent AutofacOnlyComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithAutofacDependencies1(
            IAutofacOnlyComponent autofacOnlyComponent,
            IMefComponent mefComponent)
        {
            MefOnlyComponent = mefComponent;
            AutofacOnlyComponent = autofacOnlyComponent;
        }

        public void Foo()
        {
        }
    }

    [Export("component2", typeof(IMefComponentWithAutofacDependencies))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponentWithAutofacDependencies2 : IMefComponentWithAutofacDependencies
    {
        public IMefComponent MefOnlyComponent { get; set; }
        public IAutofacOnlyComponent AutofacOnlyComponent { get; set; }
        public IAutofacComponent MixedAutofacMefComponent { get; set; }

        [ImportingConstructor]
        public MefComponentWithAutofacDependencies2(
            IAutofacOnlyComponent autofacOnlyComponent,
            IMefComponent mefComponent,
            IAutofacComponent mixedAutofacMefComponent)
        {
            MefOnlyComponent = mefComponent;
            AutofacOnlyComponent = autofacOnlyComponent;
            MixedAutofacMefComponent = mixedAutofacMefComponent;
        }

        public void Foo()
        {
        }
    }

    [InheritedExport]
    public interface IMultipleMefComponent
    {
        void Foo();
    }

    public class MultipleMefComponent1 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    public class MultipleMefComponent2 : IMultipleMefComponent
    {
        public void Foo()
        {
        }
    }

    [Export]
    public class MefMixedComponent
    {
        [ImportingConstructor]
        public MefMixedComponent(IMefComponent mefComponent, IAutofacOnlyComponent autofacComponent)
        {
            MefComponent = mefComponent;
            AutofacComponent = autofacComponent;
        }

        public IAutofacOnlyComponent AutofacComponent { get; set; }

        public IMefComponent MefComponent { get; set; }
    }
}