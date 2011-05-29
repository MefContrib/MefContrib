namespace MefContrib.Integration.Autofac.Tests
{
    using System;
    using System.ComponentModel.Composition;
    using MefContrib.Containers;

    public interface IAutofacComponent
    {
        IMefComponent MefComponent { get; }
        IMefComponent ImportedMefComponent { get; }
        void Foo();
    }

    public class AutofacComponent11 : IAutofacComponent
    {
        public Func<IMefComponent> MefComponentFactory { get; private set; }

        private readonly IMefComponent m_MefComponent;

        public AutofacComponent11(Func<IMefComponent> mefComponentFactory)
        {
            MefComponentFactory = mefComponentFactory;
            m_MefComponent = mefComponentFactory();
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public class AutofacComponent1 : IAutofacComponent
    {
        public static int InstanceCount;

        private readonly IMefComponent m_MefComponent;

        public AutofacComponent1(IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;

            InstanceCount++;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public class AutofacComponent2 : IAutofacComponent
    {
        private readonly IMefComponent m_MefComponent;

        public AutofacComponent2(/*[Dependency("component2")]*/ IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import("component2")]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    [PartNotComposable]
    public class AutofacComponent3 : IAutofacComponent
    {
        private readonly IMefComponent m_MefComponent;

        public AutofacComponent3(/*[Dependency("component2")]*/ IMefComponent mefComponent)
        {
            m_MefComponent = mefComponent;
        }

        public void Foo()
        {
        }

        public IMefComponent MefComponent
        {
            get { return m_MefComponent; }
        }

        [Import("component2")]
        public IMefComponent ImportedMefComponent { get; set; }
    }

    public interface IAutofacOnlyComponent
    {
        void Foo();
    }

    public class AutofacOnlyComponent1 : IAutofacOnlyComponent
    {
        public static int InstanceCount;

        public AutofacOnlyComponent1()
        {
            InstanceCount++;
        }

        public void Foo()
        {
        }
    }

    public class AutofacOnlyComponent2 : IAutofacOnlyComponent
    {
        public void Foo()
        {
        }
    }

    public class AutofacMixedComponent
    {
        public AutofacMixedComponent(IMefComponent mefComponent, IAutofacOnlyComponent autofacComponent)
        {
            MefComponent = mefComponent;
            AutofacComponent = autofacComponent;
        }

        public IAutofacOnlyComponent AutofacComponent { get; set; }

        public IMefComponent MefComponent { get; set; }
    }
}