using System;
using Microsoft.Data.Sqlite;

class FixDatabase
{
    static void Main(string[] args)
    {
        string databasePath = "ITRepairService/itrepair.db";
        
        if (!System.IO.File.Exists(databasePath))
        {
            Console.WriteLine($"Database not found at: {databasePath}");
            Console.WriteLine("Searching for database files...");
            foreach (string file in System.IO.Directory.GetFiles(".", "*.db", System.IO.SearchOption.AllDirectories))
            {
                Console.WriteLine($"Found: {file}");
            }
            return;
        }
        
        Console.WriteLine($"Connecting to database: {databasePath}");
        
        using (var connection = new SqliteConnection($"Data Source={databasePath}"))
        {
            try
            {
                connection.Open();
                Console.WriteLine("✓ Connected to database");
                
                var command = connection.CreateCommand();
                
                // Check current columns
                Console.WriteLine("\nCurrent columns in RepairTicket table:");
                command.CommandText = "PRAGMA table_info(RepairTicket)";
                using (var reader = command.ExecuteReader())
                {
                    var columns = new System.Collections.Generic.List<string>();
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(1));
                        Console.WriteLine($"  {reader.GetString(1)} ({reader.GetString(2)})");
                    }
                    
                    // Check if DriveAccessDepartment column exists
                    if (columns.Contains("DriveAccessDepartment"))
                    {
                        Console.WriteLine("\n✓ DriveAccessDepartment column already exists!");
                    }
                    else
                    {
                        Console.WriteLine("\n✗ DriveAccessDepartment column is missing. Adding it now...");
                        command.CommandText = "ALTER TABLE RepairTicket ADD COLUMN DriveAccessDepartment TEXT";
                        command.ExecuteNonQuery();
                        Console.WriteLine("✓ DriveAccessDepartment column added successfully!");
                    }
                }
                
                // Verify the change
                Console.WriteLine("\nUpdated columns in RepairTicket table:");
                command.CommandText = "PRAGMA table_info(RepairTicket)";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"  {reader.GetString(1)} ({reader.GetString(2)})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
            }
        }
        
        Console.WriteLine("\nDatabase connection closed.");
    }
}