using System;
using System.Data.SQLite;

class SimpleFix
{
    static void Main()
    {
        string dbPath = @"ITRepairService\itrepair.db";
        
        Console.WriteLine("========================================");
        Console.WriteLine("Adding DriveAccessDepartment Column");
        Console.WriteLine("========================================");
        Console.WriteLine();
        
        if (!System.IO.File.Exists(dbPath))
        {
            Console.WriteLine($"ERROR: Database not found at: {dbPath}");
            return;
        }
        
        Console.WriteLine($"Database: {dbPath}");
        Console.WriteLine();
        
        try
        {
            string connStr = $"Data Source={dbPath};Version=3;";
            using (var conn = new SQLiteConnection(connStr))
            {
                conn.Open();
                Console.WriteLine("✓ Connected to database");
                Console.WriteLine();
                
                // Check current columns
                Console.WriteLine("Current columns in RepairTicket:");
                using (var cmd = new SQLiteCommand("PRAGMA table_info(RepairTicket)", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    var columns = new System.Collections.Generic.List<string>();
                    while (reader.Read())
                    {
                        columns.Add(reader.GetString(1));
                        Console.WriteLine($"  {reader.GetString(1)} ({reader.GetString(2)})");
                    }
                    
                    if (columns.Contains("DriveAccessDepartment"))
                    {
                        Console.WriteLine();
                        Console.WriteLine("✓ DriveAccessDepartment column already exists!");
                        return;
                    }
                }
                
                // Add the column
                Console.WriteLine();
                Console.WriteLine("Adding DriveAccessDepartment column...");
                using (var cmd = new SQLiteCommand("ALTER TABLE RepairTicket ADD COLUMN DriveAccessDepartment TEXT", conn))
                {
                    cmd.ExecuteNonQuery();
                }
                Console.WriteLine("✓ Column added successfully!");
                Console.WriteLine();
                
                // Verify
                Console.WriteLine("Updated columns in RepairTicket:");
                using (var cmd = new SQLiteCommand("PRAGMA table_info(RepairTicket)", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"  {reader.GetString(1)} ({reader.GetString(2)})");
                    }
                }
            }
            
            Console.WriteLine();
            Console.WriteLine("========================================");
            Console.WriteLine("✓ SUCCESS! Database fix completed.");
            Console.WriteLine("========================================");
            Console.WriteLine();
            Console.WriteLine("You can now start the application.");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine();
            Console.WriteLine("Make sure System.Data.SQLite is installed:");
            Console.WriteLine("  dotnet add package System.Data.SQLite");
        }
    }
}