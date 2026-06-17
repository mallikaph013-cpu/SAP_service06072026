import sqlite3

with open('scripts/clear_sqlite_data.sql', encoding='utf-8') as f:
    sql = f.read()

conn = sqlite3.connect('myapp.db')
cursor = conn.cursor()
try:
    cursor.executescript(sql)
    conn.commit()
    print('Data cleared successfully.')
except Exception as e:
    print('Error:', e)
finally:
    conn.close()
