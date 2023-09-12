namespace Ubxtrol
{
    internal class StackComponent<T>
    {
        private readonly T value;

        public StackComponent<T> Component { get; set; }

        public T Value => this.value;

        public StackComponent(T value) => this.value = value;
    }
}
