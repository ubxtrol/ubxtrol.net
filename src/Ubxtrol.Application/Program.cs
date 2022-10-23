using Microsoft.Extensions.DependencyInjection;
using System;
using Ubxtrol.Extensions.DependencyInjection;

namespace Ubxtrol.Application;

internal interface IMyServiceA
{ }

internal class MyServiceA : IMyServiceA
{
    private readonly IServiceProvider provider;

    public MyServiceA(IServiceProvider provider)
    {
        if (provider == null)
            throw Error.ArgumentNull(nameof(provider));

        this.provider = provider;
    }

    public void DisplayProviderInformation()
    {
        Type result = this.provider.GetType();
        Console.WriteLine(result.FullName);
    }
}

internal class Program
{
    public static void Main()
    {
        ServiceCollection collection = new ServiceCollection();
        collection.AddSingleton<MyServiceA>();
        collection.AddSingleton<IMyServiceA>(provider =>
        {
            Console.WriteLine("Factory...{0}", provider.GetType().FullName);
            return provider.GetRequiredService<MyServiceA>();
        });

        UbxtrolServiceProvider provider = collection.BuildUbxtrolServiceProvider();

        using (IServiceScope scope = provider.CreateScope())
        {
            MyServiceA a = scope.ServiceProvider.GetRequiredService<MyServiceA>();
            a.DisplayProviderInformation();

            _ = scope.ServiceProvider.GetRequiredService<IMyServiceA>();
        }

        MyServiceA b = provider.GetRequiredService<MyServiceA>();
        b.DisplayProviderInformation();

        _ = provider.GetRequiredService<IMyServiceA>();

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}
