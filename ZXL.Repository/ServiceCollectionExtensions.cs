using Microsoft.Extensions.DependencyInjection;

namespace ZXL.Repository
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddZXLUnitorOfWork(this IServiceCollection services) 
            => services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
        public static IServiceCollection AddZXLRepository(this IServiceCollection services)
            => services
                .AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));

        public static IServiceCollection AddZXLUnitorOfWork(this IServiceCollection services, ServiceLifetime lifeTime)
        {
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork<>), typeof(UnitOfWork<>), lifeTime));
            return services;
        }
        public static IServiceCollection AddZXLRepository(this IServiceCollection services, ServiceLifetime lifeTime)
        {
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork<>), typeof(UnitOfWork<>), lifeTime));
            services.Add(new ServiceDescriptor(typeof(IRepository<>), typeof(Repository<>), lifeTime));
            return services;
        }
    }
}
