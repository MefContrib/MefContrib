namespace MefContrib.Hosting.Conventions.Tests.Integration
{
    public class SampleExport
    {
        public string TextValue { get { return "this is some text"; } }

        public int IntValue = 1234;
    }

    public class SampleImport
    {
        public string TextValue { get; set; }

        public int IntValue = 1234;
    }
}