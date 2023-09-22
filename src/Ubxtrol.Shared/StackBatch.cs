namespace System
{
    /// <summary>
    /// 栈结构.
    /// </summary>
    /// <typeparam name="T">元素数据类型.</typeparam>
    public sealed class StackBatch<T>
    {
        /// <summary>
        /// 获取栈顶元素.
        /// </summary>
        public IStackComponent<T> Component { get; private set; }

        /// <summary>
        /// 获取栈元素个数.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// 丢弃栈内所有元素.
        /// </summary>
        public void Discard()
        {
            this.Component = null;
            this.Count = 0x0;
        }

        /// <summary>
        /// 将元素值入栈.
        /// </summary>
        /// <param name="value">需要入栈的元素值.</param>
        public void Push(T value)
        {
            StackComponent<T> component = new StackComponent<T>(value)
            {
                Component = this.Component
            };
            this.Component = component;
            this.Count += 0x1;
        }

        /// <summary>
        /// 尝试将栈顶元素出栈.
        /// </summary>
        /// <param name="value">栈顶元素值.</param>
        /// <returns>出栈操作是否成功.</returns>
        public bool TryPop(out T value)
        {
            value = default;
            bool result = false;
            IStackComponent<T> component = this.Component;
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
