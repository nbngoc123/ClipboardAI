using System;
using Microsoft.Extensions.DependencyInjection;

namespace ClipboardAI.Infrastructure;

public static class ServiceLocator
{
    public static IServiceProvider? Provider { get; private set; }

    public static void Initialize(IServiceProvider provider)
    {
        Provider = provider;
    }

    public static T GetService<T>() where T : notnull
    {
        if (Provider == null) throw new InvalidOperationException("ServiceLocator is not initialized.");
        return Provider.GetRequiredService<T>();
    }
}
