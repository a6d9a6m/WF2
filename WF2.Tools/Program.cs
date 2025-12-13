using WF2.Tools;

namespace WF2.Tools;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Starting database migration...");
        DatabaseMigrationTool.MigrateDatabase();
        Console.WriteLine("Database migration completed.");
    }
}