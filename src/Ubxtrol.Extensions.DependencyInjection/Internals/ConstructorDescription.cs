using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ConstructorDescription
    {
        private readonly ConstructorInfo constructor;

        private readonly List<IDependencyResolution> dependencies;

        private readonly ParameterInfo[] parameters;

        public ConstructorInfo Constructor => this.constructor;

        public List<IDependencyResolution> Dependencies => this.dependencies;

        public ParameterInfo[] Parameters => this.parameters;

        public bool UnResolved { get; set; }

        private ConstructorDescription(ConstructorInfo constructor, ParameterInfo[] parameters)
        {
            if (constructor == null)
                throw Error.ArgumentNull(nameof(constructor));

            if (parameters == null)
                throw Error.ArgumentNull(nameof(parameters));

            this.constructor = constructor;
            this.dependencies = new List<IDependencyResolution>();
            this.parameters = parameters;
        }

        public static IEnumerable<ConstructorDescription> FromBatch(IEnumerable<ConstructorInfo> batch)
        {
            if (batch is null)
                throw Error.ArgumentNull(nameof(batch));

            return batch.Select(current => ConstructorDescription.FromConstructor(current));
        }

        public static ConstructorDescription FromConstructor(ConstructorInfo constructor)
        {
            if (constructor == null)
                throw Error.ArgumentNull(nameof(constructor));

            ParameterInfo[] parameters = constructor.GetParameters();
            return new ConstructorDescription(constructor, parameters);
        }
    }
}
