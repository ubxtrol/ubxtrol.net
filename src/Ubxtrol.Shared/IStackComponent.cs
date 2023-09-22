namespace System
{
    /// <summary>
    /// 栈结构元素.
    /// </summary>
    /// <typeparam name="T">元素数据类型.</typeparam>
    public interface IStackComponent<T>
    {
        /// <summary>
        /// 获取结构中的下一个元素.
        /// </summary>
        IStackComponent<T> Component { get; }

        /// <summary>
        /// 获取当前元素值.
        /// </summary>
        T Value { get; }
    }
}
