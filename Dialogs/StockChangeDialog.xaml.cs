using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public enum StockOperation { Add, Remove }

    public partial class StockChangeDialog : Window, IDataErrorInfo
    {
        private readonly Medicine _medicine;
        private readonly StockOperation _operation;

        public string Amount { get; set; }
        public string Comment { get; set; }

        public StockChangeDialog(Medicine medicine, StockOperation operation)
        {
            InitializeComponent();
            _medicine = medicine;
            _operation = operation;

            txtName.Text = medicine.Name;
            txtCurrentStock.Text = GetCurrentStock().ToString();
            Title = _operation == StockOperation.Add ? "Приход лекарства" : "Расход лекарства";

            txtAmount.PreviewTextInput += TxtAmount_PreviewTextInput;
            DataContext = this;
        }

        private int GetCurrentStock()
        {
            try
            {
                using (var context = new VeterContext())
                {
                    return context.MedicineStocks
                        .Where(ms => ms.MedicineId == _medicine.Id)
                        .Sum(ms => (int?)ms.Quantity) ?? 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private void TxtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Amount):
                        if (string.IsNullOrWhiteSpace(Amount))
                            error = "Введите количество";
                        else if (!int.TryParse(Amount, out int amount) || amount <= 0)
                            error = "Количество должно быть положительным числом";
                        else if (amount > 100000)
                            error = "Количество не может превышать 100,000";
                        else if (_operation == StockOperation.Remove)
                        {
                            int currentStock = GetCurrentStock();
                            if (amount > currentStock)
                                error = $"Недостаточно на складе. Доступно: {currentStock}";
                        }
                        break;

                    case nameof(Comment):
                        if (!string.IsNullOrWhiteSpace(Comment) && Comment.Length > 500)
                            error = "Комментарий не должен превышать 500 символов";
                        break;
                }

                return error;
            }
        }

        private string GetFullExceptionDetails(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Тип: {ex.GetType().FullName}");
            sb.AppendLine($"Сообщение: {ex.Message}");

            Exception inner = ex.InnerException;
            int level = 1;
            while (inner != null)
            {
                sb.AppendLine();
                sb.AppendLine($"Внутренняя ошибка #{level}:");
                sb.AppendLine($"Тип: {inner.GetType().FullName}");
                sb.AppendLine($"Сообщение: {inner.Message}");

                if (inner is System.Data.SqlClient.SqlException sqlEx)
                {
                    sb.AppendLine($"Ошибка SQL Server: Номер ошибки {sqlEx.Number}");
                    sb.AppendLine($"Сообщение SQL: {sqlEx.Message}");
                }

                inner = inner.InnerException;
                level++;
            }

            return sb.ToString();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            txtAmount.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtComment.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            if (Validation.GetHasError(txtAmount) || Validation.GetHasError(txtComment))
            {
                MessageBox.Show("Исправьте ошибки в форме перед сохранением",
                    "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(Amount, out int amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректное положительное количество", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtAmount.Focus();
                txtAmount.SelectAll();
                return;
            }

            try
            {
                using (var newContext = new VeterContext())
                {
                    var stock = newContext.MedicineStocks
                        .FirstOrDefault(ms => ms.MedicineId == _medicine.Id);

                    if (stock == null)
                    {
                        if (_operation == StockOperation.Remove)
                        {
                            MessageBox.Show("Нельзя израсходовать лекарство, которого нет на складе", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        stock = new MedicineStock
                        {
                            MedicineId = _medicine.Id,
                            Quantity = 0,
                            Location = "Основной склад",
                            ExpiryDate = DateTime.Now.AddYears(1)
                        };
                        newContext.MedicineStocks.Add(stock);
                    }

                    if (_operation == StockOperation.Add)
                    {
                        stock.Quantity += amount;
                    }
                    else
                    {
                        if (stock.Quantity < amount)
                        {
                            MessageBox.Show($"Нельзя израсходовать больше, чем есть на складе. Доступно: {stock.Quantity}", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        stock.Quantity -= amount;
                    }

                    newContext.SaveChanges();
                }

                // Обновляем счетчик уведомлений (триггер создаст уведомление если нужно)
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.LoadNotificationsCount();
                }

                MessageBox.Show($"Операция успешно выполнена!", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                string errorDetails = GetFullExceptionDetails(ex);

                string userMessage = "Ошибка при сохранении.\n\n" +
                    "Возможные причины:\n" +
                    "1. Нарушение ограничений базы данных\n" +
                    "2. Проблемы с подключением к БД\n" +
                    "3. Конфликты с триггерами БД\n\n" +
                    $"Детали ошибки:\n{errorDetails}";

                MessageBox.Show(userMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

                // Логируем полные детали в консоль для отладки
                Console.WriteLine("=== ОШИБКА В StockChangeDialog ===");
                Console.WriteLine(errorDetails);
                Console.WriteLine("===================================");
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}