# Ubxtrol.Extensions.DependencyInjection

## 支持属性注入的 .NET Core 依赖注入框架。

### 特点

-   兼容官方依赖注入实现；
-   支持属性注入（同样支持构造方法注入）。

### 安装

-   [NuGet](https://www.nuget.org/packages/Ubxtrol.Extensions.DependencyInjection)

### 使用

-   将普通属性变为依赖属性仅需满足两个条件：

    1.  公共属性（`public`）支持写入（写入访问修饰符不限，推荐使用`private`）；
    2.  对属性添加`UbxtrolAutowiredAttribute`特性。

-   其他功能使用方法与官方依赖注入框架基本一致。
-   注意：依赖属性将在对象实例创建后注入，因此在对象构造方法中无法访问依赖属性，若要在构造方法中使用依赖，请使用构造方法注入。

### 示例

```csharp
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using Ubxtrol.Extensions.DependencyInjection;

namespace HelloWorld
{
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
}
```

### 其他

#### 替换 ASP.NET Core 默认服务容器

```csharp
public static void Main(string[] args)
{
    CreateHostBuilder(args).Build().Run();
}

public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        //使用框架服务提供对象工厂...
        .UseServiceProviderFactory(new UbxtrolServiceProviderFactory())
        .ConfigureWebHostDefaults(builder =>
        {
            builder.UseStartup<Startup>();
        });
```

#### 控制器属性注入

默认情况下，ASP.NET Core 控制器类型（`Controllers`）不会作为服务注册到容器，因此为控制器添加依赖属性将不会被自动注入。

若希望在控制器类型中使用依赖属性，需要在`Startup.ConfigureServices`方法中将控制器类型作为服务注册到容器：

```csharp
public void ConfigureServices(IServiceCollection services)
{
    var builder = services.AddControllersWithViews();
    //将控制器类型作为服务注册到容器...
    builder.AddControllersAsServices();
}
```

#### 服务作用域验证

启用服务作用域验证将检测并禁止具有局部作用域的服务在全局范围创建；具体而言，启用该选项将检测并禁止以下两种情况：

1. 注册为`Scoped`的服务直接或间接从根服务提供对象创建；
2. 注册为`Scoped`的服务被注册为`Singleton`的服务所依赖。

默认情况下`不验证`服务作用域，以下示例演示如何启用服务作用域验证：

-   .NET Core

```csharp
var collection = new ServiceCollection();

//...注册服务...

//创建ServiceProvider并启用服务作用域验证...
var provider = collection.BuildUbxtrolServiceProvider(options => options.EnableScopeValidation = true);
```

-   ASP.NET Core

```csharp
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        //使用框架服务提供对象工厂...
        .UseServiceProviderFactory(context =>
        {
            var result = new UbxtrolServiceProviderFactory();
            //推荐在开发环境启用服务作用域验证(这也是官方服务容器在ASP.NET Core的默认行为)...
            result.Options.EnableScopeValidation = context.HostingEnvironment.IsDevelopment();
            return result;
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
```

#### 可选的依赖属性

若依赖属性不是必须的，或应用程序具备不依赖服务的默认行为，可能需要将某个依赖属性设置为可选的。

默认情况下，所有的依赖属性都是必须的，以下示例演示如何设置可选的依赖属性：

```csharp
internal class MyServiceA
{
    [UbxtrolAutowired(IgnoreWhenNotFound = true)]
    public MyServiceB ServiceB { get; private set; }
}
```

对特性`UbxtrolAutowired`设置`IgnoreWhenNotFound = true`后，当`MyServiceB`未被注册为服务时，将忽略该属性的注入行为。
