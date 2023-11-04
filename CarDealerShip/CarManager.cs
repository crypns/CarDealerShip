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
    public class CarManager
    {
        private const string DatabasePath = "autosalon.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;

        public CarManager()
        {
            connection = new SQLiteConnection(ConnectionString);
        }

        public void CreateTableIfNotExists()
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Автомобили (ID INTEGER PRIMARY KEY AUTOINCREMENT, Марка TEXT, МодельныйРяд TEXT, Год INTEGER, Кузов TEXT, НомерМашины TEXT)";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void LoadData()
        {
            using (dataAdapter = new SQLiteDataAdapter("SELECT * FROM Автомобили", connection))
            {
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
            }
        }

        public List<string> GetCarNumbers()
        {
            List<string> carNumbers = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT НомерМашины FROM Автомобили";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string carNumber = reader["НомерМашины"].ToString();
                    carNumbers.Add(carNumber);
                }
            }

            return carNumbers;
        }
        public void AddCar(string brand, string model, int year, string body, string carNumber)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO Автомобили (Марка, МодельныйРяд, Год, Кузов, НомерМашины) VALUES (@Brand, @Model, @Year, @Body, @CarNumber)";
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Model", model);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Body", body);
                cmd.Parameters.AddWithValue("@CarNumber", carNumber);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteCar(int carId)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "DELETE FROM Автомобили WHERE ID = @CarId";
                cmd.Parameters.AddWithValue("@CarId", carId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public DataTable GetDataTable()
        {
            return dataTable;
        }

        public void UpdateCar(int carId, string brand, string model, int year, string body, string carNumber)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Автомобили SET Марка = @Brand, МодельныйРяд = @Model, Год = @Year, Кузов = @Body, НомерМашины = @CarNumber WHERE ID = @CarId";
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Model", model);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Body", body);
                cmd.Parameters.AddWithValue("@CarNumber", carNumber);
                cmd.Parameters.AddWithValue("@CarId", carId);

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
                cmd.CommandText = "SELECT * FROM Автомобили WHERE " + column + " LIKE @SearchValue";
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

        public bool ValidateCarData(string brand, string model, int year, string body, string carNumber)
        {
            if (string.IsNullOrWhiteSpace(brand) || string.IsNullOrWhiteSpace(model) || year <= 0 || string.IsNullOrWhiteSpace(body)
                || string.IsNullOrWhiteSpace(carNumber))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Дополнительные проверки, если необходимо

            return true;
        }

        public string GetCarModelByNumber(string carNumber)
        {
            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT Марка FROM Автомобили WHERE НомерМашины = @CarNumber";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.Parameters.AddWithValue("@CarNumber", carNumber);
                string carModel = command.ExecuteScalar()?.ToString();
                connection.Close();

                return carModel;
            }
        }
    }
}
