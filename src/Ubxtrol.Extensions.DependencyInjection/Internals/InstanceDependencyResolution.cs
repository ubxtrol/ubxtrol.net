namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class InstanceDependencyResolution : IDependencyResolution
    {
        private readonly object value;

        public InstanceDependencyResolution(object value)
        {
            if (value == null)
                throw Error.ArgumentNull(nameof(value));

            this.value = value;
        }

        public void Resolve(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            context.Result = this.value;
        }
    }
}
