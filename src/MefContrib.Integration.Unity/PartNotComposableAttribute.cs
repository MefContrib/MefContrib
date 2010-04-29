using System;

namespace MefContrib.Integration.Unity
{
    /// <summary>
    /// Suppresses MEF composition for a class created by the Unity container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PartNotComposableAttribute : Attribute
    {
    }
}