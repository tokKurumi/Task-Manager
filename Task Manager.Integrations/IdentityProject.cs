namespace Task_Manager.Integrations;

public static class IdentityProject
{
    public static string PostgreSQLResource => "task-manager-identity-postgresql";
    public static string PostgreSQLVolume => "task-manager-identity-postgresql-volume";
    public static string PostgreSQLDatabase => "task-manager-identity-postgresql-database";

    public static string Migrator => "task-manager-identity-migrator";
    public static string API => "task-manager-identity-identityapi";
}
