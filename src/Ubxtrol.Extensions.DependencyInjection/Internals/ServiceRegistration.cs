using Microsoft.Extensions.DependencyInjection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceRegistration
    {
        public ServiceDescription Description { get; set; }

        public ServiceDescriptor Descriptor => this.Description?.Descriptor;

        public void Append(ServiceDescriptor descriptor)
        {
            ServiceDescription description = new ServiceDescription(descriptor);
            if (this.Description != null)
                description.Description = this.Description;

            this.Description = description;
        }
    }
}
