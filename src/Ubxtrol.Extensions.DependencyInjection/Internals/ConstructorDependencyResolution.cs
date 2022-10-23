using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ConstructorDependencyResolution : ServiceDependencyResolution, IEnumerableResolution
    {
        private readonly IDependencyResolution[] arguments;

        private readonly ServiceCreation creation;

        private readonly IDependencyResolution[] properties;

        protected override void ExecuteServiceCreation(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            int length = this.arguments.Length;
            object[] arguments = new object[length];
            for (int index = 0x0; index < length; index += 0x1)
            {
                this.arguments[index].Resolve(context);
                arguments[index] = context.Result;
            }
            length = this.properties.Length;
            object[] properties = new object[length];
            for (int index = 0x0; index < length; index += 0x1)
            {
                this.properties[index].Resolve(context);
                properties[index] = context.Result;
            }
            context.Result = this.creation.Invoke(arguments, properties);
        }

        public ConstructorDependencyResolution(ServiceIdentity identity, ServiceCreation creation, IDependencyResolution[] arguments, IDependencyResolution[] properties)
            : base(identity)
        {
            if (creation == null)
                throw Error.ArgumentNull(nameof(creation));

            if (arguments == null)
                throw Error.ArgumentNull(nameof(arguments));

            if (properties == null)
                throw Error.ArgumentNull(nameof(properties));

            this.creation = creation;
            this.arguments = arguments;
            this.properties = properties;
        }

        public IEnumerator<IDependencyResolution> GetResolutionEnumerator()
        {
            int length = this.arguments.Length;
            for (int index = 0x0; index < length; index += 0x1)
                yield return this.arguments[index];

            length = this.properties.Length;
            for (int index = 0x0; index < length; index += 0x1)
                yield return this.properties[index];

            yield break;
        }
    }
}
