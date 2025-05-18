namespace Task_Manager.Integrations;

public static class IdentityProject
{
    public static string PostgreSQLResource => "identity-postgresql";
    public static string PostgreSQLVolume => "identity-postgresql-volume";
    public static string PostgreSQLDatabase => "identity-postgresql-database";

    public static string Migrator => "task-manager-identity-migrator";
    public static string API => "task-manager-identity-identityapi";
}
