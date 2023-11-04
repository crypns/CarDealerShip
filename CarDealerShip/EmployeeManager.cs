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
    public class EmployeeManager
    {
        private const string DatabasePath = "autosalon.db";
        private const string ConnectionString = "Data Source=" + DatabasePath + ";Version=3;";

        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;

        public EmployeeManager()
        {
            connection = new SQLiteConnection(ConnectionString);
        }

        public void CreateTableIfNotExists()
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "CREATE TABLE IF NOT EXISTS Сотрудники (ID INTEGER PRIMARY KEY AUTOINCREMENT, Фамилия TEXT, Имя TEXT, Должность TEXT, Зарплата REAL)";
                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void LoadData()
        {
            using (dataAdapter = new SQLiteDataAdapter("SELECT * FROM Сотрудники", connection))
            {
                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
            }
        }

        public void AddEmployee(string lastName, string firstName, string position, double salary)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "INSERT INTO Сотрудники (Фамилия, Имя, Должность, Зарплата) VALUES (@LastName, @FirstName, @Position, @Salary)";
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@Salary", salary);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void DeleteEmployee(int employeeId)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "DELETE FROM Сотрудники WHERE ID = @EmployeeId";
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

                connection.Open();
                cmd.ExecuteNonQuery();
                connection.Close();
            }
        }

        public DataTable GetDataTable()
        {
            return dataTable;
        }

        public void UpdateEmployee(int employeeId, string lastName, string firstName, string position, double salary)
        {
            using (var cmd = new SQLiteCommand(connection))
            {
                cmd.CommandText = "UPDATE Сотрудники SET Фамилия = @LastName, Имя = @FirstName, Должность = @Position, Зарплата = @Salary WHERE ID = @EmployeeId";
                cmd.Parameters.AddWithValue("@LastName", lastName);
                cmd.Parameters.AddWithValue("@FirstName", firstName);
                cmd.Parameters.AddWithValue("@Position", position);
                cmd.Parameters.AddWithValue("@Salary", salary);
                cmd.Parameters.AddWithValue("@EmployeeId", employeeId);

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
                cmd.CommandText = "SELECT * FROM Сотрудники WHERE " + column + " LIKE @SearchValue";
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

        public bool ValidateEmployeeData(string lastName, string firstName, string position, double salary)
        {
            if (string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(position))
            {
                MessageBox.Show("Пожалуйста, заполните все обязательные поля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (salary <= 0)
            {
                MessageBox.Show("Зарплата должна быть больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Дополнительные проверки, если необходимо

            return true;
        }
    }
}
