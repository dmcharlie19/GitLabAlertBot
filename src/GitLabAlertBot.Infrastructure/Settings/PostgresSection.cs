namespace GitLabAlertBot.Infrastructure.Settings;

public class PostgresSection
{
    public const string SectionName = "Postgres";

    public string DbConnectionString { get; set; } = string.Empty;
}
