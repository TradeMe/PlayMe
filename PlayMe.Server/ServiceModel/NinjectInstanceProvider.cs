using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Ninject;

namespace PlayMe.Server.ServiceModel
{
    /// <summary>
    /// An instance provider that uses Ninject to create the instance.
    /// </summary>
    public class NinjectInstanceProvider : IInstanceProvider
    {
        /// <summary>
        /// The type of the service.
        /// </summary>
        private readonly Type serviceType;

        /// <summary>
        /// The resolution root that is used to create the instance.
        /// </summary>
        private readonly IKernel kernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="NinjectInstanceProvider"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="kernel">The kernel.</param>
        public NinjectInstanceProvider(Type serviceType, IKernel kernel)
        {
            this.serviceType = serviceType;
            this.kernel = kernel;
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <returns>
        /// A user-defined service object.
        /// </returns>
        /// <param name="instanceContext">
        /// The current <see cref="T:System.ServiceModel.InstanceContext" />
        /// object.
        /// </param>
        public object GetInstance(InstanceContext instanceContext)
        {
            return this.GetInstance(instanceContext, null);
        }

        /// <summary>
        /// Returns a service object given the specified <see cref="T:System.ServiceModel.InstanceContext" /> object.
        /// </summary>
        /// <returns>
        /// The service object.
        /// </returns>
        /// <param name="instanceContext">
        /// The current <see cref="T:System.ServiceModel.InstanceContext" />
        /// object.
        /// </param>
        /// <param name="message">
        /// The message that triggered the creation of a service object.
        /// </param>
        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return this.kernel.Get(this.serviceType);
        }

        /// <summary>
        /// Called when an <see cref="T:System.ServiceModel.InstanceContext" />
        /// object recycles a service object.
        /// </summary>
        /// <param name="instanceContext">
        /// The service's instance context.
        /// </param>
        /// <param name="instance">
        /// The service object to be recycled.
        /// </param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }
    }
}