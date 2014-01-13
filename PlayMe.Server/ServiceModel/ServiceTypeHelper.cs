namespace PlayMe.Server.ServiceModel
{
    using System.Linq;
    using System.ServiceModel;

    /// <summary>
    /// Helper class to decide if a service is a singleton.
    /// </summary>
    internal static class ServiceTypeHelper
    {
        /// <summary>
        /// Determines whether the given service is a singleton service.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <returns>
        ///     <c>true</c> if the service is a singleton; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSingletonService(object service)
        {
            var serviceBehaviorAttribute =
                service.GetType().GetCustomAttributes(typeof(ServiceBehaviorAttribute), true)
                .Cast<ServiceBehaviorAttribute>()
                .SingleOrDefault();
            return serviceBehaviorAttribute != null && serviceBehaviorAttribute.InstanceContextMode == InstanceContextMode.Single;
        }
    }
}