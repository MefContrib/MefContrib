namespace MefContrib.Web.Mvc
{
    /// <summary>
    /// PartCreationScope works together with PartCreationPolicy. PartCreationScope defines in which container the part should be registered. PartCreationPolicy defines if the part is Shared or NonShared for that container scope.
    /// </summary>
    public enum PartCreationScope
    {
        /// <summary>
        /// Default. Registers the part per child container. In default MefContrib.Mvc installations, this will mean a per-request lifetime scope.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Registers the part per child container. In default MefContrib.Mvc installations, this will mean a per-request lifetime scope.
        /// </summary>
        PerChildContainer = 0,

        /// <summary>
        /// Registers the part per master container. In default MefContrib.Mvc installations, this will mean a per-application lifetime scope.
        /// </summary>
        PerMasterContainer = 1
    }
}