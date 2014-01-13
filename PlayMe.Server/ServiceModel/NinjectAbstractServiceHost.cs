namespace PlayMe.Server.ServiceModel
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    /// <summary>
    /// Abstract base class for WebServiceHost that initializes based on the
    /// ServiceBehavior attribute as singleton or multi instance service
    /// </summary>
    /// <typeparam name="T">The type of the service</typeparam>
    public abstract class NinjectAbstractServiceHost<T> : NinjectServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the NinjectAbstractServiceHost class.
        /// </summary>
        /// <param name="serviceBehavior">The service behavior.</param>
        /// <param name="instance">The instance.</param>
        /// <param name="baseAddresses">The baseAddresses.</param>
        protected NinjectAbstractServiceHost(IServiceBehavior serviceBehavior, T instance, Uri[] baseAddresses)
            : base(serviceBehavior)
        {
            var addresses = new UriSchemeKeyedCollection(baseAddresses);

            if (ServiceTypeHelper.IsSingletonService(instance))
            {
                this.InitializeDescription(instance, addresses);
            }
            else
            {
                this.InitializeDescription(typeof(T), addresses);
            }
        }
    }
}