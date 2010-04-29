using System.ComponentModel.Composition;

namespace MefContrib.Integration.Unity.Tests.Helpers.External
{
    public interface IMefComponent
    {
        void Foo();

        IExternalComponent Component1 { get; }

        IExternalComponent Component1A { get; set; }
    }

    [Export(typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent1 : IMefComponent
    {
        private readonly IExternalComponent m_Component1;

        [ImportingConstructor]
        public MefComponent1(IExternalComponent component1)
        {
            m_Component1 = component1;
        }

        public void Foo()
        {
        }

        public IExternalComponent Component1
        {
            get { return m_Component1; }
        }

        [Import]
        public IExternalComponent Component1A { get; set; }
    }

    [Export("component2", typeof(IMefComponent))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MefComponent2 : IMefComponent
    {
        private readonly IExternalComponent m_Component1;

        [ImportingConstructor]
        public MefComponent2([Import("external2")] IExternalComponent component1)
        {
            m_Component1 = component1;
        }

        public void Foo()
        {
        }

        public IExternalComponent Component1
        {
            get { return m_Component1; }
        }

        [Import("external2")]
        public IExternalComponent Component1A { get; set; }
    }
}