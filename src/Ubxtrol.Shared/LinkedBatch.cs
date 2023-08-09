namespace Ubxtrol
{
    internal sealed class LinkedBatch<T>
    {
        public LinkedNode<T> Node { get; private set; }

        public void Append(T value)
        {
            LinkedNode<T> node = this.Node;
            this.Node = new LinkedNode<T>(node, value);
        }

        public void Discard() => this.Node = null;
    }

    internal sealed class LinkedNode<T>
    {
        private readonly LinkedNode<T> node;

        private readonly T value;

        public LinkedNode<T> Node => this.node;

        public T Value => this.value;

        public LinkedNode(LinkedNode<T> node, T value)
        {
            this.node = node;
            this.value = value;
        }
    }
}
