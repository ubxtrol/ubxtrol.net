using System;
using System.Collections.Generic;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ArrayDependencyResolution : ServiceDependencyResolution, IEnumerableResolution
    {
        private readonly IDependencyResolution[] dependencies;

        private readonly Type mItemType;

        protected override void ExecuteServiceCreation(DependencyResolveContext context)
        {
            if (context == null)
                throw Error.ArgumentNull(nameof(context));

            int length = this.dependencies.Length;
            Array result = Array.CreateInstance(this.mItemType, length);
            for (int index = 0x0; index < length; index += 0x1)
            {
                this.dependencies[index].Resolve(context);
                result.SetValue(context.Result, index);
            }
            context.Result = result;
        }

        public ArrayDependencyResolution(ServiceIdentity identity, Type mItemType)
            : this(identity, mItemType, Array.Empty<IDependencyResolution>())
        { }

        public ArrayDependencyResolution(ServiceIdentity identity, Type mItemType, IDependencyResolution[] dependencies)
            : base(identity)
        {
            if (mItemType == null)
                throw Error.ArgumentNull(nameof(mItemType));

            if (dependencies == null)
                throw Error.ArgumentNull(nameof(dependencies));

            this.dependencies = dependencies;
            this.mItemType = mItemType;
        }

        public IEnumerator<IDependencyResolution> GetResolutionEnumerator()
        {
            int length = this.dependencies.Length;
            for (int index = 0x0; index < length; index += 0x1)
                yield return this.dependencies[index];

            yield break;
        }
    }
}
