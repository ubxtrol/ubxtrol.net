using System;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal class ServiceKey
    {
        private readonly int index;

        private readonly Type value;

        public int Index => this.index;

        public Type Value => this.value;

        public ServiceKey(Type value)
            : this(0x0, value)
        { }

        public ServiceKey(int index, Type value)
        {
            if (value == null)
                throw Error.ArgumentNull(nameof(value));

            this.index = index;
            this.value = value;
        }
    }
}
