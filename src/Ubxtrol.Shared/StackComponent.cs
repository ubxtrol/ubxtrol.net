namespace System
{
    internal class StackComponent<T> : IStackComponent<T>
    {
        private readonly T value;

        public IStackComponent<T> Component { get; set; }

        public T Value => this.value;

        public StackComponent(T value) => this.value = value;
    }
}
