namespace MefContrib.Hosting.Conventions.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Defines the functionality of a type scanner configurator.
    /// </summary>
    public interface ITypeScannerConfigurator : IHideObjectMembers
    {
        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="assembly"/> parameter.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(Assembly assembly);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path to the <see cref="Assembly(System.Reflection.Assembly)"/> that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(string path);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> in the current <see cref="AppDomain"/> that matches the
        /// condition specified by the <paramref name="condition"/> parameter.
        /// </summary>
        /// <param name="condition">A condition that an <see cref="Assembly(System.Reflection.Assembly)"/> has to meet in order to be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Assembly(Func<Assembly, bool> condition);

        /// <summary>
        /// Adds the <see cref="Assembly(System.Reflection.Assembly)"/> instances that can be found in the directory that is
        /// specified by the <paramref name="path"/> parameter.
        /// </summary>
        /// <param name="path">A <see cref="string"/> containing the path of the directory that should be inspected for <see cref="Assembly(System.Reflection.Assembly)"/> instances to add.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Directory(string path);

        /// <summary>
        /// Adds a scanner to the configurator.
        /// </summary>
        /// <param name="scanner">An <see cref="ITypeScanner"/> instance to add.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Scanner(ITypeScanner scanner);

        /// <summary>
        /// Adds the types specified by the <paramref name="types"/> parameter.
        /// </summary>
        /// <param name="types">An <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Types(IEnumerable<Type> types);

        /// <summary>
        /// Adds the types that are returned by the function specified by the <paramref name="values"/> function.
        /// </summary>
        /// <param name="values">A function that returns an <see cref="IEnumerable{T}"/> of <see cref="Type"/> instances that should be added.</param>
        /// <returns>A reference to the current <see cref="ITypeScannerConfigurator"/> instance.</returns>
        ITypeScannerConfigurator Types(Func<IEnumerable<Type>> values);
    }
}