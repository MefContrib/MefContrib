namespace MefContrib.Web.Mvc
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Web.Mvc;
    using MefContrib.Hosting.Conventions;
    using MefContrib.Hosting.Conventions.Configuration;

    /// <summary>
    /// MvcApplicationRegistry
    /// </summary>
    public class MvcApplicationRegistry : PartRegistry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MvcApplicationRegistry"/> class.
        /// </summary>
        public MvcApplicationRegistry()
        {
            Scan(x =>
            {
                x.Assembly(Assembly.GetExecutingAssembly());
                x.Directory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin"));
            });

            Part()
                .ForTypesAssignableFrom<IController>()
                .MakeNonShared()
                .ExportTypeAs<IController>()
                .ExportType()
                .Imports(x =>
                {
                    x.Import().Members(
                        m => new[] { m.GetConstructors().FirstOrDefault(c => c.GetCustomAttributes(typeof(ImportingConstructorAttribute), false).Length > 0) ?? m.GetGreediestConstructor() });
                    x.Import().Members(
                        m => m.GetMembers().Where(mbr => mbr.GetCustomAttributes(typeof(ImportAttribute), false).Length > 0).ToArray());
                });
        }
    }
}
