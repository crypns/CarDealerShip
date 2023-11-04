using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Windows.Forms;
using System.Drawing.Printing;


namespace CarDealerShip
{
    public partial class Form1 : Form
    {
        private DatabaseManager databaseManager;
        private ClientManager clientManager;
        private CarManager carManager;
        private SalesManager salesManager;
        private EmployeeManager employeeManager;


        private SQLiteConnection connection;
        private SQLiteDataAdapter dataAdapter;
        private DataTable dataTable;
        private DataTable carDataTable;


        private int clientId;
        private int carId;
        private int saleId;
        private int employeeId;


        public Form1()
        {
            InitializeComponent();
            databaseManager = new DatabaseManager();
            clientManager = new ClientManager();
            carManager = new CarManager();
            salesManager = new SalesManager();
            employeeManager = new EmployeeManager();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            clientManager.CreateTableIfNotExists();
            clientManager.LoadData();
            dataGridViewClient.DataSource = clientManager.GetDataTable();

            carManager.CreateTableIfNotExists();
            carManager.LoadData();
            dataGridViewCar.DataSource = carManager.GetDataTable();

            salesManager.CreateTableIfNotExists();
            salesManager.LoadData();
            dataGridViewSales.DataSource = salesManager.GetDataTable();

            employeeManager.CreateTableIfNotExists();
            employeeManager.LoadData();
            dataGridViewEmployee.DataSource = employeeManager.GetDataTable();


            comboBoxClientLastName.DataSource = clientManager.GetDataTable();
            carDataTable = carManager.GetDataTable(); // Предполагается, что у вас есть метод GetDataTable() в классе CarManager

            LoadClientLastNames(); // Заполнение выпадающего списка
            LoadSaleCarNumbers();

            foreach (DataGridViewColumn column in dataGridViewSales.Columns)
            {
                // Исключить столбец "ID" из списка
                if (column.Name != "ID")
                {
                    comboBoxSearchSale.Items.Add(column.HeaderText);
                }
            }

            foreach (DataGridViewColumn column in dataGridViewEmployee.Columns)
            {
                // Исключить столбец "ID" из списка
                if (column.Name != "ID")
                {
                    comboBoxSearchEmployee.Items.Add(column.HeaderText);
                }
            }

        }
        private void LoadClientLastNames()
        {
            comboBoxClientLastName.DataSource = null; // Очистить свойство DataSource

            List<string> lastNames = clientManager.GetClientLastNames();

            comboBoxClientLastName.DataSource = lastNames; // Установить новый источник данных
        }

        private void LoadSaleCarNumbers()
        {
            comboBoxSaleCarNumber.DataSource = null; // Очистить свойство DataSource

            List<string> carNumbers = carManager.GetCarNumbers();

            comboBoxSaleCarNumber.DataSource = carNumbers; // Установить новый источник данных
        }


        private void buttonAddClient_Click(object sender, EventArgs e)
        {
            string passport = textBoxPassport.Text;
            string surname = textBoxSurname.Text;
            string firstName = textBoxFirstName.Text;
            string patronymic = textBoxPatronymic.Text;
            string phone = textBoxPhone.Text;
            string address = textBoxAddress.Text;

            // Проверить правильность заполнения данных
            if (!clientManager.ValidateClientData(passport, surname, firstName, patronymic, phone, address))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            clientManager.AddClient(passport, surname, firstName, patronymic, phone, address);

            // Очистка полей после добавления клиента
            textBoxPassport.Clear();
            textBoxSurname.Clear();
            textBoxFirstName.Clear();
            textBoxPatronymic.Clear();
            textBoxPhone.Clear();
            textBoxAddress.Clear();

            // Обновление DataGridView
            clientManager.LoadData();
            dataGridViewClient.DataSource = clientManager.GetDataTable();
            UpdateClientLastNameComboBox();
        }

        private void btnDeleteClient_Click(object sender, EventArgs e)
        {
            if (dataGridViewClient.SelectedRows.Count > 0)
            {
                // Получить ID выбранного клиента из выделенной строки dataGridViewClient
                int selectedClientId = Convert.ToInt32(dataGridViewClient.SelectedRows[0].Cells["ID"].Value);

                // Удалить клиента из базы данных
                clientManager.DeleteClient(selectedClientId);

                // Обновить dataGridViewClient
                clientManager.LoadData();
                dataGridViewClient.DataSource = clientManager.GetDataTable();
                UpdateClientLastNameComboBox();
            }
            else
            {
                MessageBox.Show("Выберите клиента для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditClient_Click(object sender, EventArgs e)
        {
            if (dataGridViewClient.SelectedRows.Count > 0)
            {
                // Получить выбранного клиента из выделенной строки DataGridView
                DataGridViewRow selectedRow = dataGridViewClient.SelectedRows[0];

                // Получить значения полей клиента
                clientId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string passport = Convert.ToString(selectedRow.Cells["Паспорт"].Value);
                string surname = Convert.ToString(selectedRow.Cells["Фамилия"].Value);
                string firstName = Convert.ToString(selectedRow.Cells["Имя"].Value);
                string patronymic = Convert.ToString(selectedRow.Cells["Отчество"].Value);
                string phone = Convert.ToString(selectedRow.Cells["Телефон"].Value);
                string address = Convert.ToString(selectedRow.Cells["Адрес"].Value);

                // Заполнить элементы управления с данными клиента
                textBoxPassport.Text = passport;
                textBoxSurname.Text = surname;
                textBoxFirstName.Text = firstName;
                textBoxPatronymic.Text = patronymic;
                textBoxPhone.Text = phone;
                textBoxAddress.Text = address;

                // Дополнительные действия, если необходимо
            }
            else
            {
                MessageBox.Show("Выберите клиента для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSaveClient_Click(object sender, EventArgs e)
        {
            // Получить значения из элементов управления
            string passport = textBoxPassport.Text;
            string surname = textBoxSurname.Text;
            string firstName = textBoxFirstName.Text;
            string patronymic = textBoxPatronymic.Text;
            string phone = textBoxPhone.Text;
            string address = textBoxAddress.Text;

            // Проверить правильность заполнения данных
            if (!clientManager.ValidateClientData(passport, surname, firstName, patronymic, phone, address))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            // Обновить данные клиента в базе данных
            clientManager.UpdateClient(clientId, passport, surname, firstName, patronymic, phone, address);

            // Очистить элементы управления после сохранения
            textBoxPassport.Clear();
            textBoxSurname.Clear();
            textBoxFirstName.Clear();
            textBoxPatronymic.Clear();
            textBoxPhone.Clear();
            textBoxAddress.Clear();

            // Сбросить значение переменной clientId
            clientId = 0;

            // Обновить DataGridView
            clientManager.LoadData();
            dataGridViewClient.DataSource = clientManager.GetDataTable();
            UpdateClientLastNameComboBox();
        }

        private void btnSearchClient_Click(object sender, EventArgs e)
        {
            string searchColumn = comboBoxSearchClient.SelectedItem?.ToString();
            string searchPhrase = txtBoxSearchClient.Text;

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchPhrase))
            {
                // Используйте фильтрацию по столбцу и фразе для выполнения поиска
                (dataGridViewClient.DataSource as DataTable).DefaultView.RowFilter = $"[{searchColumn}] LIKE '%{searchPhrase}%'";
            }
            else
            {
                // Если не выбран столбец или не введена фраза, отобразите все данные
                (dataGridViewClient.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            foreach (DataGridViewColumn column in dataGridViewClient.Columns)
            {
                // Исключить столбец "ID" из списка
                if (column.Name != "ID")
                {
                    comboBoxSearchClient.Items.Add(column.HeaderText);
                }
            }

            foreach (DataGridViewColumn column in dataGridViewCar.Columns)
            {
                // Исключить столбец "ID" из списка
                if (column.Name != "ID")
                {
                    comboBoxSearchCar.Items.Add(column.HeaderText);
                }
            }
        }

        private void txtBoxSearchClient_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataGridViewClient.Text))
            {
                // Если фраза поиска пуста, отобразите все данные
                (dataGridViewClient.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void buttonAddCar_Click(object sender, EventArgs e)
        {
            string brand = textBoxBrand.Text;
            string model = textBoxModel.Text;
            int year = Convert.ToInt32(numericYear.Value);
            string body = comboBoxBody.SelectedItem?.ToString();
            string carNumber = textBoxCarNumber.Text;

            // Проверить правильность заполнения данных
            if (!carManager.ValidateCarData(brand, model, year, body, carNumber))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            carManager.AddCar(brand, model, year, body, carNumber);

            // Очистка полей после добавления автомобиля
            textBoxBrand.Clear();
            textBoxModel.Clear();
            numericYear.Value = DateTime.Now.Year;
            comboBoxBody.SelectedIndex = -1;

            textBoxCarNumber.Clear();

            // Обновление DataGridView
            carManager.LoadData();
            dataGridViewCar.DataSource = carManager.GetDataTable();
            UpdateSaleCarNumberComboBox();

        }

        private void btnDeleteCar_Click(object sender, EventArgs e)
        {
            if (dataGridViewCar.SelectedRows.Count > 0)
            {
                // Получить ID выбранного автомобиля из выделенной строки dataGridViewCar
                int selectedCarId = Convert.ToInt32(dataGridViewCar.SelectedRows[0].Cells["ID"].Value);

                // Удалить автомобиль из базы данных
                carManager.DeleteCar(selectedCarId);

                // Обновить dataGridViewCar
                carManager.LoadData();
                dataGridViewCar.DataSource = carManager.GetDataTable();
            }
            else
            {
                MessageBox.Show("Выберите автомобиль для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditCar_Click(object sender, EventArgs e)
        {
            if (dataGridViewCar.SelectedRows.Count > 0)
            {
                // Получить выбранный автомобиль из выделенной строки DataGridView
                DataGridViewRow selectedRow = dataGridViewCar.SelectedRows[0];

                // Получить значения полей автомобиля
                carId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string brand = Convert.ToString(selectedRow.Cells["Марка"].Value);
                string model = Convert.ToString(selectedRow.Cells["МодельныйРяд"].Value);
                int year = Convert.ToInt32(selectedRow.Cells["Год"].Value);
                string body = Convert.ToString(selectedRow.Cells["Кузов"].Value);
                string carNumber = Convert.ToString(selectedRow.Cells["НомерМашины"].Value);

                // Заполнить элементы управления данными автомобиля
                textBoxBrand.Text = brand;
                textBoxModel.Text = model;
                numericYear.Value = year;
                comboBoxBody.SelectedItem = body;
                textBoxCarNumber.Text = carNumber;

                // Дополнительные действия, если необходимо
            }
            else
            {
                MessageBox.Show("Выберите автомобиль для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSaveCar_Click(object sender, EventArgs e)
        {
            // Получить значения из элементов управления
            string brand = textBoxBrand.Text;
            string model = textBoxModel.Text;
            int year = Convert.ToInt32(numericYear.Value);
            string body = comboBoxBody.SelectedItem?.ToString();
            string carNumber = textBoxCarNumber.Text;

            // Проверить правильность заполнения данных
            if (!carManager.ValidateCarData(brand, model, year, body, carNumber))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            // Обновить данные автомобиля в базе данных
            carManager.UpdateCar(carId, brand, model, year, body, carNumber);

            // Очистить элементы управления после сохранения
            textBoxBrand.Clear();
            textBoxModel.Clear();
            numericYear.Value = DateTime.Now.Year;
            comboBoxBody.SelectedIndex = -1;
            textBoxCarNumber.Clear();

            // Сбросить значение переменной carId
            carId = 0;

            // Обновить DataGridView
            carManager.LoadData();
            dataGridViewCar.DataSource = carManager.GetDataTable();
            UpdateSaleCarNumberComboBox();
        }

        private void btnSearchCar_Click(object sender, EventArgs e)
        {
            string searchColumn = comboBoxSearchCar.SelectedItem?.ToString();
            string searchPhrase = txtBoxSearchCar.Text;

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchPhrase))
            {
                // Используйте фильтрацию по столбцу и фразе для выполнения поиска
                (dataGridViewCar.DataSource as DataTable).DefaultView.RowFilter = $"[{searchColumn}] LIKE '%{searchPhrase}%'";
            }
            else
            {
                // Если не выбран столбец или не введена фраза, отобразите все данные
                (dataGridViewCar.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void txtBoxSearchCar_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataGridViewCar.Text))
            {
                // Если фраза поиска пуста, отобразите все данные
                (dataGridViewCar.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void buttonAddSale_Click(object sender, EventArgs e)
        {
            string clientLastName = comboBoxClientLastName.SelectedItem.ToString();
            string carNumber = comboBoxSaleCarNumber.SelectedItem.ToString();
            string brand = textBoxSaleBrand.Text;
            int price = Convert.ToInt32(numericPrice.Value);
            string status = comboBoxSaleStatus.SelectedItem.ToString();
            DateTime saleDate = dateTimePickerSale.Value;



            // Проверить правильность заполнения данных
            if (!salesManager.ValidateSaleData(clientLastName, carNumber, brand, price, status, saleDate))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            salesManager.AddSale(clientLastName, carNumber, brand, price, status, saleDate);

            // Очистка полей после добавления продажи
            comboBoxClientLastName.SelectedIndex = -1;
            textBoxCarNumber.Clear();
            textBoxBrand.Clear();
            numericPrice.Value = 0;

            // Обновление DataGridView
            salesManager.LoadData();
            dataGridViewSales.DataSource = salesManager.GetDataTable();
        }

        private void comboBoxSaleCarNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedCarNumber = comboBoxSaleCarNumber.SelectedItem?.ToString();
            if (selectedCarNumber != null)
            {
                string carModel = carManager.GetCarModelByNumber(selectedCarNumber);

                // Заполнение поля "Модель" соответствующим значением
                textBoxSaleBrand.Text = carModel;
            }
        }

        private void btnEditSale_Click(object sender, EventArgs e)
        {
            if (dataGridViewSales.SelectedRows.Count > 0)
            {
                // Получить выбранную продажу из выделенной строки DataGridView
                DataGridViewRow selectedRow = dataGridViewSales.SelectedRows[0];

                // Получить значения полей продажи
                saleId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string clientLastName = Convert.ToString(selectedRow.Cells["КлиентФамилия"].Value);
                string carNumber = Convert.ToString(selectedRow.Cells["НомерАвтомобиля"].Value);
                string brand = Convert.ToString(selectedRow.Cells["Модель"].Value);
                int price = Convert.ToInt32(selectedRow.Cells["Стоимость"].Value);
                string status = Convert.ToString(selectedRow.Cells["Статус"].Value);
                DateTime saleDate = dateTimePickerSale.Value;


                // Заполнить элементы управления с данными продажи
                comboBoxClientLastName.Text = clientLastName;
                comboBoxSaleCarNumber.Text = carNumber;
                textBoxBrand.Text = brand;
                numericPrice.Value = price;
                comboBoxSaleStatus.Text = status;
                dateTimePickerSale.Value = saleDate;

                // Дополнительные действия, если необходимо
            }
            else
            {
                MessageBox.Show("Выберите продажу для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSaveSale_Click(object sender, EventArgs e)
        {
            // Получить значения из элементов управления
            string clientLastName = comboBoxClientLastName.Text;
            string carNumber = comboBoxSaleCarNumber.Text;
            string brand = textBoxBrand.Text;
            int price = Convert.ToInt32(numericPrice.Value);
            string status = comboBoxSaleStatus.Text;
            DateTime saleDate = dateTimePickerSale.Value;

            // Проверить правильность заполнения данных
            if (!salesManager.ValidateSaleData(clientLastName, carNumber, brand, price, status, saleDate))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            // Обновить данные продажи в базе данных
            salesManager.UpdateSale(saleId, clientLastName, carNumber, brand, price, status, saleDate);

            // Очистить элементы управления после сохранения
            comboBoxClientLastName.SelectedIndex = -1;
            comboBoxSaleCarNumber.SelectedIndex = -1;
            textBoxBrand.Clear();
            numericPrice.Value = 0;
            dateTimePickerSale.Value = DateTime.Now;

            // Сбросить значение переменной saleId
            saleId = 0;

            // Обновить DataGridView
            salesManager.LoadData();
            dataGridViewSales.DataSource = salesManager.GetDataTable();
        }

        private void btnDeleteSale_Click(object sender, EventArgs e)
        {
            if (dataGridViewSales.SelectedRows.Count > 0)
            {
                // Получить ID выбранной продажи из выделенной строки dataGridViewSales
                int selectedSaleId = Convert.ToInt32(dataGridViewSales.SelectedRows[0].Cells["ID"].Value);

                // Удалить продажу из базы данных
                salesManager.DeleteSale(selectedSaleId);

                // Обновить dataGridViewSales
                salesManager.LoadData();
                dataGridViewSales.DataSource = salesManager.GetDataTable();
            }
            else
            {
                MessageBox.Show("Выберите продажу для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void comboBoxClientLastName_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateClientLastNameComboBox()
        {
            // Получить список клиентских фамилий из базы данных или другого источника данных
            List<string> clientLastNames = salesManager.GetClientLastNames();

            // Присвоить список клиентских фамилий в качестве источника данных comboBoxClientLastName
            comboBoxClientLastName.DataSource = clientLastNames;
        }

        private void UpdateSaleCarNumberComboBox()
        {
            // Получить список номеров автомобилей из базы данных или другого источника данных
            List<string> carNumbers = salesManager.GetCarNumbers();

            // Присвоить список номеров автомобилей в качестве источника данных comboBoxSaleCarNumber
            comboBoxSaleCarNumber.DataSource = carNumbers;
        }

        private void btnSearchSale_Click(object sender, EventArgs e)
        {
            string searchColumn = comboBoxSearchSale.SelectedItem?.ToString();
            string searchPhrase = txtBoxSearchSale.Text;

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchPhrase))
            {
                // Используйте фильтрацию по столбцу и фразе для выполнения поиска
                (dataGridViewSales.DataSource as DataTable).DefaultView.RowFilter = $"[{searchColumn}] LIKE '%{searchPhrase}%'";
            }
            else
            {
                // Если не выбран столбец или не введена фраза, отобразите все данные
                (dataGridViewSales.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void txtBoxSearchSale_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataGridViewSales.Text))
            {
                // Если фраза поиска пуста, отобразите все данные
                (dataGridViewSales.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void btnSearchPriceSale_Click(object sender, EventArgs e)
        {
            int minPrice = (int)numericMinPrice.Value;
            int maxPrice = (int)numericMaxPrice.Value;

            DataTable searchResults = salesManager.SearchByPriceRange(minPrice, maxPrice);

            dataGridViewSales.DataSource = searchResults;
        }

        private void btnReserSearchSale_Click(object sender, EventArgs e)
        {
            salesManager.LoadData();
            dataGridViewSales.DataSource = salesManager.GetDataTable();
        }

        private void btnFilterDateSale_Click(object sender, EventArgs e)
        {
            DateTime startDate = dateTimePickerStartDate.Value;
            DateTime endDate = dateTimePickerEndDate.Value;

            DataTable searchResult = salesManager.SearchByDateRange(startDate, endDate);
            dataGridViewSales.DataSource = searchResult;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            

        }

        private void buttonAddEmployee_Click(object sender, EventArgs e)
        {
            string lastName = textBoxLastNameEmpl.Text;
            string firstName = textBoxFirstNameEmpl.Text;
            string position = textBoxPosition.Text;
            double salary = Convert.ToDouble(textBoxSalary.Text);

            // Проверить правильность заполнения данных
            if (!employeeManager.ValidateEmployeeData(lastName, firstName, position, salary))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            employeeManager.AddEmployee(lastName, firstName, position, salary);

            // Очистка полей после добавления сотрудника
            textBoxFirstNameEmpl.Clear();
            textBoxLastNameEmpl.Clear();
            textBoxPosition.Clear();
            textBoxSalary.Clear();

            // Обновление DataGridView
            employeeManager.LoadData();
            dataGridViewEmployee.DataSource = employeeManager.GetDataTable();
        }

        private void btnDeleteEmployee_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                // Получить ID выбранного сотрудника из выделенной строки dataGridViewEmployee
                int selectedEmployeeId = Convert.ToInt32(dataGridViewEmployee.SelectedRows[0].Cells["ID"].Value);

                // Удалить сотрудника из базы данных
                employeeManager.DeleteEmployee(selectedEmployeeId);

                // Обновить dataGridViewEmployee
                employeeManager.LoadData();
                dataGridViewEmployee.DataSource = employeeManager.GetDataTable();
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для удаления.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnEditEmployee_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployee.SelectedRows.Count > 0)
            {
                // Получить выбранного сотрудника из выделенной строки DataGridView
                DataGridViewRow selectedRow = dataGridViewEmployee.SelectedRows[0];

                // Получить значения полей сотрудника
                employeeId = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string lastName = Convert.ToString(selectedRow.Cells["Фамилия"].Value);
                string firstName = Convert.ToString(selectedRow.Cells["Имя"].Value);
                string position = Convert.ToString(selectedRow.Cells["Должность"].Value);
                double salary = Convert.ToDouble(selectedRow.Cells["Зарплата"].Value);

                // Заполнить элементы управления с данными сотрудника
                textBoxLastNameEmpl.Text = lastName;
                textBoxFirstNameEmpl.Text = firstName;
                textBoxPosition.Text = position;
                textBoxSalary.Text = salary.ToString();

                // Дополнительные действия, если необходимо
            }
            else
            {
                MessageBox.Show("Выберите сотрудника для редактирования.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            // Получить значения из элементов управления
            string lastName = textBoxLastNameEmpl.Text;
            string firstName = textBoxFirstNameEmpl.Text;
            string position = textBoxPosition.Text;
            double salary = Convert.ToDouble(textBoxSalary.Text);

            // Проверить правильность заполнения данных
            if (!employeeManager.ValidateEmployeeData(lastName, firstName, position, salary))
            {
                return; // Прекратить выполнение, если данные некорректны
            }

            // Обновить данные сотрудника в базе данных
            employeeManager.UpdateEmployee(employeeId, lastName, firstName, position, salary);

            // Очистить элементы управления после сохранения
            textBoxLastNameEmpl.Clear();
            textBoxFirstNameEmpl.Clear();
            textBoxPosition.Clear();
            textBoxSalary.Clear();

            // Сбросить значение переменной employeeId
            employeeId = 0;

            // Обновить DataGridView
            employeeManager.LoadData();
            dataGridViewEmployee.DataSource = employeeManager.GetDataTable();
        }

        private void btnSearchEmployee_Click(object sender, EventArgs e)
        {
            string searchColumn = comboBoxSearchEmployee.SelectedItem?.ToString();
            string searchPhrase = txtBoxSearchEmployee.Text;

            if (!string.IsNullOrEmpty(searchColumn) && !string.IsNullOrEmpty(searchPhrase))
            {
                // Используйте фильтрацию по столбцу и фразе для выполнения поиска
                (dataGridViewEmployee.DataSource as DataTable).DefaultView.RowFilter = $"[{searchColumn}] LIKE '%{searchPhrase}%'";
            }
            else
            {
                // Если не выбран столбец или не введена фраза, отобразите все данные
                (dataGridViewEmployee.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void txtBoxSearchEmployee_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(dataGridViewEmployee.Text))
            {
                // Если фраза поиска пуста, отобразите все данные
                (dataGridViewEmployee.DataSource as DataTable).DefaultView.RowFilter = "";
            }
        }

        private void btnPrintSale_Click(object sender, EventArgs e)
        {
            if (dataGridViewSales.SelectedRows.Count > 0)
            {
                // Получить выбранную продажу из выделенной строки DataGridView
                DataGridViewRow selectedRow = dataGridViewSales.SelectedRows[0];

                // Получить значения полей продажи
                string clientLastName = Convert.ToString(selectedRow.Cells["КлиентФамилия"].Value);
                string carNumber = Convert.ToString(selectedRow.Cells["НомерАвтомобиля"].Value);
                string brand = Convert.ToString(selectedRow.Cells["Модель"].Value);
                int price = Convert.ToInt32(selectedRow.Cells["Стоимость"].Value);
                string status = Convert.ToString(selectedRow.Cells["Статус"].Value);
                DateTime saleDate = Convert.ToDateTime(selectedRow.Cells["ДатаПродажи"].Value);

                // Создать строку с данными продажи для печати
                string saleData = $"Фамилия клиента: {clientLastName}\nНомер автомобиля: {carNumber}\nМарка: {brand}\nЦена: {price}\nСтатус: {status}\nДата продажи: {saleDate}";

                // Создать объект PrintDocument
                PrintDocument printDocument = new PrintDocument();

                // Установить обработчик события PrintPage для определения содержимого, которое нужно напечатать
                printDocument.PrintPage += (s, args) =>
                {
                    // Получить графический объект, на котором будет выполняться печать
                    Graphics graphics = args.Graphics;

                    // Определить шрифт и размер шрифта для печати
                    Font font = new Font("Arial", 12);

                    // Определить координаты верхнего левого угла области печати
                    float x = 10;
                    float y = 10;

                    // Напечатать данные продажи
                    graphics.DrawString(saleData, font, Brushes.Black, x, y);
                };

                // Открыть диалоговое окно печати и печатать данные
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            else
            {
                MessageBox.Show("Выберите продажу для печати.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
