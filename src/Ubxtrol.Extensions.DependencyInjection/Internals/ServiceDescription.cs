using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceDescription
    {
        private readonly ServiceDescriptor descriptor;

        public ServiceDescription Description { get; set; }

        public ServiceDescriptor Descriptor => this.descriptor;

        public ServiceDescription(ServiceDescriptor descriptor)
        {
            if (descriptor == null)
                throw Error.ArgumentNull(nameof(descriptor));

            this.descriptor = descriptor;
        }
    }
}
