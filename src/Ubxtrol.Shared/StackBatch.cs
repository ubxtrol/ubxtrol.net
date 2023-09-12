namespace Ubxtrol
{
    internal sealed class StackBatch<T>
    {
        public StackComponent<T> Component { get; private set; }

        public int Count { get; private set; }

        public void Discard()
        {
            this.Component = null;
            this.Count = 0x0;
        }

        public void Push(T value)
        {
            StackComponent<T> component = new StackComponent<T>(value)
            {
                Component = this.Component
            };
            this.Component = component;
            this.Count += 0x1;
        }

        public bool TryPop(out T value)
        {
            value = default;
            bool result = false;
            StackComponent<T> component = this.Component;
            if (component != null)
            {
                this.Component = component.Component;
                value = component.Value;
                this.Count -= 0x1;
                result = true;
            }

            return result;
        }
    }
}
