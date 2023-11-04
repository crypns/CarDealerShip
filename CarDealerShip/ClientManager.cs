using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CarDealerShip
{
    public class ClientManager
    {
        private const string DatabasePath = "autosalon.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;

        public ClientManager()
        {
            connection = new SQLiteConnection(ConnectionString);
        }

        public void CreateTableIfNotExists()
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Клиенты (ID INTEGER PRIMARY KEY AUTOINCREMENT, Паспорт TEXT, Фамилия TEXT, Имя TEXT, Отчество TEXT, Телефон TEXT, Адрес TEXT)";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }


        public void LoadData()
        {
            using (dataAdapter = new SQLiteDataAdapter("SELECT * FROM Клиенты", connection))
            {
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
            }
        }

        public void AddClient(string passport, string surname, string firstName, string patronymic, string phone, string address)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO Клиенты (Паспорт, Фамилия, Имя, Отчество, Телефон, Адрес) VALUES (@Passport, @Surname, @FirstName, @Patronymic, @Phone, @Address)";
                cmd.Parameters.AddWithValue("@Passport", passport);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@Patronymic", patronymic);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void DeleteClient(int clientId)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "DELETE FROM Клиенты WHERE ID = @ClientId";
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public List<string> GetClientLastNames()
        {
            List<string> lastNames = new List<string>();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT Фамилия FROM Клиенты";

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string lastName = reader.GetString(0);
                        lastNames.Add(lastName);
                    }
                }

                connection.Close();
            }

            return lastNames;
        }

        public DataTable GetDataTable()
        {
            return dataTable;
        }

        public void UpdateClient(int clientId, string passport, string surname, string firstName, string patronymic, string phone, string address)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Клиенты SET Паспорт = @Passport, Фамилия = @Surname, Имя = @FirstName, Отчество = @Patronymic, Телефон = @Phone, Адрес = @Address WHERE ID = @ClientId";
                cmd.Parameters.AddWithValue("@Passport", passport);
                cmd.Parameters.AddWithValue("@Surname", surname);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@Patronymic", patronymic);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@ClientId", clientId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public DataTable SearchByColumn(string column, string searchValue)
        {
            DataTable dataTable = new DataTable();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT * FROM Клиенты WHERE " + column + " LIKE @SearchValue";
                cmd.Parameters.AddWithValue("@SearchValue", "%" + searchValue + "%");

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }

                connection.Close();
            }

            return dataTable;
        }

        public bool ValidateClientData(string passport, string surname, string firstName, string patronymic, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(passport) || string.IsNullOrWhiteSpace(surname) || string.IsNullOrWhiteSpace(firstName)
                || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Дополнительные проверки, если необходимо

            return true;
        }
    }
}