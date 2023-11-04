using System.Data;
using System.Data.SQLite;

namespace CarDealerShip
{
    public class DatabaseManager
    {
        private const string DatabasePath = "autosalon.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        private SQLiteConnection connection;

        public DatabaseManager()
        {
            connection = new SQLiteConnection(ConnectionString);
        }

        public void CreateDatabase()
        {
            if (!DatabaseExists())
            {
                SQLiteConnection.CreateFile(DatabasePath);
            }
        }

        public void CreateTableClients()
        {
            if (!TableExists("Клиенты"))
            {
                using (var cmd = new SQLiteCommand(connection))
                {
                    cmd.CommandText = "CREATE TABLE Клиенты (ID INTEGER PRIMARY KEY AUTOINCREMENT, Фамилия TEXT, НомерАвтомобиля TEXT, МаркаАвтомобиля TEXT, Цена REAL, Статус TEXT, ДатаПродажи DATE)";
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        private bool DatabaseExists()
        {
            return System.IO.File.Exists(DatabasePath);
        }

        private bool TableExists(string tableName)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@TableName";
                cmd.Parameters.AddWithValue("@TableName", tableName);
                connection.Open();
                int count = (int)cmd.ExecuteScalar();
                connection.Close();
                return count > 0;
            }
        }
    }
}
