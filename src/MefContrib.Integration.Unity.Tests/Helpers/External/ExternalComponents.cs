namespace MefContrib.Integration.Unity.Tests.Helpers.External
{
    public interface IExternalComponent
    {
        void Foo();
    }

    public class ExternalComponent1 : IExternalComponent
    {
        public void Foo()
        {
        }
    }

    public class ExternalComponent2 : IExternalComponent
    {
        public void Foo()
        {
        }
    }
}