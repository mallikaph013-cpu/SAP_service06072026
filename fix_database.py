import sqlite3
import os

database_path = 'ITRepairService/itrepair.db'

if not os.path.exists(database_path):
    print(f"Database not found at: {database_path}")
    print("Searching for database files...")
    for root, dirs, files in os.walk('.'):
        for file in files:
            if file.endswith('.db'):
                full_path = os.path.join(root, file)
                print(f"Found: {full_path}")
    exit(1)

print(f"Connecting to database: {database_path}")
conn = sqlite3.connect(database_path)
cursor = conn.cursor()

# Check current columns
cursor.execute('PRAGMA table_info(RepairTicket)')
columns = cursor.fetchall()
print('\nCurrent columns in RepairTicket table:')
for col in columns:
    print(f"  {col[1]} ({col[2]})")

# Check if DriveAccessDepartment column exists
column_names = [col[1] for col in columns]
if 'DriveAccessDepartment' in column_names:
    print('\n✓ DriveAccessDepartment column already exists!')
else:
    print('\n✗ DriveAccessDepartment column is missing. Adding it now...')
    cursor.execute('ALTER TABLE RepairTicket ADD COLUMN DriveAccessDepartment TEXT')
    conn.commit()
    print('✓ DriveAccessDepartment column added successfully!')

# Verify the change
cursor.execute('PRAGMA table_info(RepairTicket)')
columns = cursor.fetchall()
print('\nUpdated columns in RepairTicket table:')
for col in columns:
    print(f"  {col[1]} ({col[2]})")

conn.close()