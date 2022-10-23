namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ScopedServiceProvider : ServiceProvider
    {
        private readonly ServiceProvider provider;

        public override ServiceProvider Provider => this.provider;

        protected override DependencyResolveContext CreateResolveContext()
        {
            DependencyResolveContext result = new DependencyResolveContext(this);
            result.Container = this.provider.Container;
            return result;
        }

        public ScopedServiceProvider(ServiceProvider provider)
            : base(provider?.Configuration, EmptyServiceScopeValidator.Shared)
        {
            if (provider == null)
                throw Error.ArgumentNull(nameof(provider));

            this.provider = provider;
        }
    }
}
