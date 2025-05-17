namespace Task_Manager.AppHost.Integrations;

public static class Integrations
{
    public static class Identity
    {
        public static string PostgreSQLResource => "identity-postgresql";
        public static string PostgreSQLVolume => "identity-postgresql-volume";
        public static string PostgreSQLDatabase => "identity-postgresql-database";

    }
}
