using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Ubxtrol.Extensions.DependencyInjection
{
    internal static class ServiceCreationTools
    {
        private static readonly Lazy<Type[]> Cache = new Lazy<Type[]>(() => new Type[] { typeof(object[]), typeof(object[]) });

        public static DynamicMethod CreateDynamicMethod()
        {
            Guid id = Guid.NewGuid();
            Type[] arguments = ServiceCreationTools.Cache.Value;
            string result = string.Format("CreateService{0:N}", id);
            return new DynamicMethod(result, typeof(object), arguments);
        }

        public static ServiceCreation CreateServiceCreation(DynamicMethod method)
        {
            if (method == null)
                throw Error.ArgumentNull(nameof(method));

            Delegate creation = method.CreateDelegate(typeof(ServiceCreation));
            ServiceCreation result = creation as ServiceCreation;
            if (result == null)
                throw Error.Invalid("服务创建委托构建失败!");

            return result;
        }

        public static MethodInfo GetSetMethod(Type mImplementationType, PropertyInfo mPropertyInfo)
        {
            if (mImplementationType == null)
                throw Error.ArgumentNull(nameof(mImplementationType));

            if (mPropertyInfo == null)
                throw Error.ArgumentNull(nameof(mPropertyInfo));

            MethodInfo result = mPropertyInfo.GetSetMethod(true);
            if (result != null)
                return result;

            Type mDeclaringType = mPropertyInfo.DeclaringType;
            if (mDeclaringType == null || mDeclaringType.Equals(mImplementationType))
                return result;

            mPropertyInfo = mDeclaringType.GetProperty(mPropertyInfo.Name, BindingFlags.Instance | BindingFlags.Public);
            if (mPropertyInfo != null)
                result = mPropertyInfo.GetSetMethod(true);

            return result;
        }
    }
}
