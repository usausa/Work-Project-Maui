namespace ContainerExample;

using System.Collections;
using System.Diagnostics;

using Smart.Resolver;

internal sealed class DebugServiceProviderFactory : IServiceProviderFactory<ResolverConfig>
{
    private readonly SmartServiceProviderFactory serviceProviderFactory;

    public DebugServiceProviderFactory()
    {
        serviceProviderFactory = new SmartServiceProviderFactory();
    }

    public ResolverConfig CreateBuilder(IServiceCollection services)
    {
        foreach (var service in services)
        {
            if (service.ImplementationType is not null)
            {
                Debug.WriteLine($"[Register] {service.Lifetime} : {service.ServiceType} : {service.ImplementationType}");
            }
            else if (service.ImplementationInstance is not null)
            {
                Debug.WriteLine($"[Register] {service.Lifetime} : {service.ServiceType} : {service.ImplementationInstance.GetType()}");
            }
            else if (service.ImplementationFactory is not null)
            {
                Debug.WriteLine($"[Register] {service.Lifetime} : {service.ServiceType} : (factory)");
            }
        }

        return serviceProviderFactory.CreateBuilder(services);
    }

    public IServiceProvider CreateServiceProvider(ResolverConfig containerBuilder)
    {
        return new DebugServiceProvider(containerBuilder.ToResolver());
    }
}

internal sealed class DebugServiceProvider : IServiceProvider
{
    private readonly SmartResolver resolver;

    public DebugServiceProvider(SmartResolver resolver)
    {
        this.resolver = resolver;
    }

    public object GetService(Type serviceType)
    {
        var obj = resolver.GetService(serviceType);

        if (obj is IEnumerable ie)
        {
            foreach (var element in ie)
            {
                Debug.WriteLine($"[GetService] {serviceType} : {element.GetType()}");
            }
        }
        else
        {
            Debug.WriteLine($"[GetService] {serviceType} : {obj?.GetType()}");
        }

        return obj;
    }
}
