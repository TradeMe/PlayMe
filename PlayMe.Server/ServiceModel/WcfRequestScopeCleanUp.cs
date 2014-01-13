using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Ninject;
using Ninject.Activation.Caching;

namespace PlayMe.Server.ServiceModel
{   
    /// <summary>
    /// Cleans up the ninject cache from the OperationContext.Current after each 
    /// </summary>
    public class WcfRequestScopeCleanup : GlobalKernelRegistration, IDispatchMessageInspector
    {
        /// <summary>
        /// Defines if the scope is released at the end of the request.
        /// </summary>
        private readonly bool releaseScopeAtRequestEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="WcfRequestScopeCleanup"/> class.
        /// </summary>
        /// <param name="releaseScopeAtRequestEnd">if set to <c>true</c> release scope at request end.</param>
        public WcfRequestScopeCleanup(bool releaseScopeAtRequestEnd)
        {
            this.releaseScopeAtRequestEnd = releaseScopeAtRequestEnd;
        }

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            return null;
        }

        /// <summary>
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (this.releaseScopeAtRequestEnd)
            {
                var context = OperationContext.Current;
                this.MapKernels(kernel => kernel.Components.Get<ICache>().Clear(context));
            }
        }
    }
}