using GitLabAlertBot.Infrastructure;
using GitLabAlertBot.PollingAgent;
using GitLabAlertBot.Telegram;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGitlabPollingServices(builder.Configuration);
builder.Services.AddTelegramServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));
});

var app = builder.Build();

app.Run();
