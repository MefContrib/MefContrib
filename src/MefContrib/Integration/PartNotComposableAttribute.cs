namespace MefContrib.Integration
{
    using System;

    /// <summary>
    /// Suppresses MEF composition for a class created by the IoC container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PartNotComposableAttribute : Attribute
    {
    }
}