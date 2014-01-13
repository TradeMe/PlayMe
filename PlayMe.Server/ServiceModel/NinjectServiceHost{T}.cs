namespace PlayMe.Server.ServiceModel
{
    using System;
    using System.ServiceModel.Description;

    /// <summary>
    /// A service host that uses Ninject to create the service instances.
    /// </summary>
    /// <typeparam name="T">The type of the service</typeparam>
    public class NinjectServiceHost<T> : NinjectAbstractServiceHost<T>
    {
        /// <summary>
        /// Initializes a new instance of the NinjectServiceHost class.
        /// </summary>
        /// <param name="serviceBehavior">The service behavior.</param>
        /// <param name="instance">The instance.</param>
        public NinjectServiceHost(IServiceBehavior serviceBehavior, T instance)
            : base(serviceBehavior, instance, new Uri[0])
        {
        }
    }
}