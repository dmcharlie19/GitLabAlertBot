using EmptyBot.Contracts.Repositories;
using GitLabAlertBot.Domain;
using GitLabAlertBot.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace GitLabAlertBot.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        var settings = config
                 .GetRequiredSection(PostgresSection.SectionName)
                 .Get<PostgresSection>();

        services.AddDbContext<BotDbContext>(
            x => x.UseNpgsql(settings?.DbConnectionString,
                             b => b.MigrationsAssembly("GitLabAlertBot.API")));

        return services;
    }
}
