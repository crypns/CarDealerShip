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
    public class SalesManager
    {
        private const string DatabasePath = "autosalon.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;
        private ComboBox searchComboBox;

        public SalesManager()
        {
            connection = new SQLiteConnection(ConnectionString);
        }
        public SalesManager(ComboBox searchComboBox)
        {
            connection = new SQLiteConnection(ConnectionString);
            this.searchComboBox = searchComboBox;
        }

        public void CreateTableIfNotExists()
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Продажи (ID INTEGER PRIMARY KEY AUTOINCREMENT, КлиентФамилия TEXT, НомерАвтомобиля TEXT, Модель TEXT, Стоимость INTEGER, Статус TEXT, ДатаПродажи TEXT)";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void LoadData()
        {
            using (dataAdapter = new SQLiteDataAdapter("SELECT * FROM Продажи", connection))
            {
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
            }
        }

        public void AddSale(string clientLastName, string carNumber, string brand, int price, string status, DateTime saleDate)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO Продажи (КлиентФамилия, НомерАвтомобиля, Модель, Стоимость, Статус, ДатаПродажи) VALUES (@ClientLastName, @CarNumber, @Brand, @Price, @Status, @SaleDate)";
                cmd.Parameters.AddWithValue("@ClientLastName", clientLastName);
                cmd.Parameters.AddWithValue("@CarNumber", carNumber);
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@SaleDate", saleDate.ToString("yyyy-MM-dd"));

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteSale(int saleId)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "DELETE FROM Продажи WHERE ID = @SaleId";
                cmd.Parameters.AddWithValue("@SaleId", saleId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }


        public DataTable GetDataTable()
        {
            return dataTable;
        }

        public void UpdateSale(int saleId, string clientLastName, string carNumber, string brand, int price, string status, DateTime saleDate)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Продажи SET КлиентФамилия = @ClientLastName, НомерАвтомобиля = @CarNumber, Модель = @Brand, Стоимость = @Price, Статус = @Status, ДатаПродажи = @SaleDate WHERE ID = @SaleId";
                cmd.Parameters.AddWithValue("@ClientLastName", clientLastName);
                cmd.Parameters.AddWithValue("@CarNumber", carNumber);
                cmd.Parameters.AddWithValue("@Brand", brand);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@Status", status);
                cmd.Parameters.AddWithValue("@SaleDate", saleDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@SaleId", saleId);

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
                cmd.CommandText = "SELECT * FROM Продажи WHERE " + column + " LIKE @SearchValue";
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

        public bool ValidateSaleData(string lastName, string carNumber, string brand, int price, string status, DateTime saleDate)
        {
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(carNumber) || string.IsNullOrWhiteSpace(brand)
                || price <= 0 || string.IsNullOrWhiteSpace(status) || saleDate == null)
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Дополнительные проверки, если необходимо

            return true;
        }

        public List<string> GetClientListFromDatabase()
        {
            List<string> clientList = new List<string>();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT DISTINCT КлиентФамилия FROM Продажи";

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string clientLastName = reader.GetString(0);
                        clientList.Add(clientLastName);
                    }
                }

                connection.Close();
            }

            return clientList;
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

        public List<string> GetCarNumbers()
        {
            List<string> carNumbers = new List<string>();

            using (SQLiteConnection connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string query = "SELECT НомерМашины FROM Автомобили";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string carNumber = reader.GetString(0);
                            carNumbers.Add(carNumber);
                        }
                    }
                }
            }

            return carNumbers;
        }

        public List<string> GetSalesTableColumns()
        {
            List<string> columns = new List<string>();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "PRAGMA table_info(Продажи)";

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string columnName = reader.GetString(1);
                        columns.Add(columnName);
                    }
                }

                connection.Close();
            }

            if (searchComboBox != null)
            {
                searchComboBox.Items.Clear();
                searchComboBox.Items.AddRange(columns.ToArray());
            }

            return columns;
        }

        public DataTable SearchByPriceRange(int minPrice, int maxPrice)
        {
            DataTable dataTable = new DataTable();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT * FROM Продажи WHERE Стоимость >= @MinPrice AND Стоимость <= @MaxPrice";
                cmd.Parameters.AddWithValue("@MinPrice", minPrice);
                cmd.Parameters.AddWithValue("@MaxPrice", maxPrice);

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }

                connection.Close();
            }

            return dataTable;
        }

        public DataTable SearchByDateRange(DateTime startDate, DateTime endDate)
        {
            DataTable dataTable = new DataTable();

            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "SELECT * FROM Продажи WHERE ДатаПродажи BETWEEN @StartDate AND @EndDate";
                cmd.Parameters.AddWithValue("@StartDate", startDate.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@EndDate", endDate.ToString("yyyy-MM-dd"));

                connection.Open();

                using (SQLiteDataReader reader = cmd.ExecuteReader())
                {
                    dataTable.Load(reader);
                }

                connection.Close();
            }

            return dataTable;
        }

    }
}
