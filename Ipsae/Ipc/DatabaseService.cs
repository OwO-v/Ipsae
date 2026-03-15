using IpsaeShared;
using Microsoft.Data.Sqlite;
using Serilog;

namespace Ipsae.Ipc;

public class DatabaseService
{
    private static readonly Lazy<DatabaseService> _instance = new(() => new DatabaseService());
    public static DatabaseService Instance => _instance.Value;

    public static string DbPath => IpsaePaths.DbPath;

    private DatabaseService() { }

    public bool Initialize()
    {
        try
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();

            CreateTables(connection);

            Log.Information("Database initialized: {Path}", DbPath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database initialization failed");
            return false;
        }
    }

    public SqliteConnection CreateConnection()
    {
        var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();
        return connection;
    }

    private static void CreateTables(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS tb_threat_host (
                idx          INTEGER PRIMARY KEY AUTOINCREMENT,
                host_type    INTEGER NOT NULL DEFAULT 0,
                host_ip      INTEGER NOT NULL DEFAULT 0,
                host_domain  TEXT    NOT NULL DEFAULT '',
                source       TEXT    NOT NULL DEFAULT '',
                is_valid     INTEGER NOT NULL DEFAULT 1,
                create_date  INTEGER,
                last_date    INTEGER,
                threat_level INTEGER NOT NULL DEFAULT 0
            );

            CREATE TABLE IF NOT EXISTS tb_network_log (
                idx         INTEGER PRIMARY KEY AUTOINCREMENT,
                direction   INTEGER NOT NULL,
                protocol    INTEGER NOT NULL,
                remote_ip   INTEGER NOT NULL,
                remote_port INTEGER NOT NULL,
                local_port  INTEGER NOT NULL,
                length      INTEGER NOT NULL,
                is_threat   INTEGER NOT NULL DEFAULT 0,
                timestamp   INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS tb_dns_log (
                idx         INTEGER PRIMARY KEY AUTOINCREMENT,
                domain      TEXT    NOT NULL DEFAULT '',
                resolved_ip INTEGER NOT NULL DEFAULT 0,
                timestamp   INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS tb_process_log (
                idx         INTEGER PRIMARY KEY AUTOINCREMENT,
                network_idx INTEGER NOT NULL,
                pid         INTEGER NOT NULL,
                ppid        INTEGER,
                proc_name   TEXT NOT NULL,
                proc_path   TEXT NOT NULL,
                proc_user   TEXT,
                proc_create INTEGER,
                timestamp   INTEGER NOT NULL
            );

            CREATE TABLE IF NOT EXISTS tb_user_rule (
                idx          INTEGER PRIMARY KEY AUTOINCREMENT,
                rule_type    INTEGER NOT NULL DEFAULT 0,
                rule_target  INTEGER NOT NULL DEFAULT 0,
                rule_value   TEXT    NOT NULL,
                rule_reason  TEXT,
                is_valid     INTEGER NOT NULL DEFAULT 1,
                timestamp    INTEGER
            );
            """;
        command.ExecuteNonQuery();
    }
}
