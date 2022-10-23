using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Ubxtrol.Extensions.DependencyInjection;

namespace Ubxtrol.Application;

internal class MyServiceA
{ }

internal class MyServiceB
{ }

internal abstract class MyServiceC
{
    [UbxtrolAutowired]
    public MyServiceA ServiceA { get; private set; }
}

internal class MyServiceD : MyServiceC
{
    [UbxtrolAutowired]
    public MyServiceB ServiceB { get; private set; }
}

internal class Program
{
    public static void Main()
    {
        var collection = new ServiceCollection();
        //注册MyServiceA...
        collection.AddTransient<MyServiceA>();
        //注册MyServiceB...
        collection.AddTransient<MyServiceB>();
        //注册MyServiceD...
        collection.AddTransient<MyServiceD>();

        //构建ServiceProvider...
        var provider = collection.BuildUbxtrolServiceProvider();

        //获取MyServiceD实例...
        var result = provider.GetRequiredService<MyServiceD>();

        //将自动注入ServiceA和ServiceB...
        Debug.Assert(result.ServiceA != null);
        Debug.Assert(result.ServiceB != null);

        Console.Write("Press any key to continue...");
        Console.ReadKey();
    }
}
