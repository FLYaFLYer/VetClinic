using System;
using System.ComponentModel;
using System.Linq;
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
        private readonly VeterContext _context = new VeterContext();

        public string Amount { get; set; }
        public string Comment { get; set; }

        public StockChangeDialog(Medicine medicine, StockOperation operation)
        {
            InitializeComponent();
            _medicine = medicine;
            _operation = operation;

            txtName.Text = medicine.Name;
            Title = _operation == StockOperation.Add ? "Приход лекарства" : "Расход лекарства";

            txtAmount.PreviewTextInput += TxtAmount_PreviewTextInput;
            DataContext = this;
        }

        private void TxtAmount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры
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
                        break;

                    case nameof(Comment):
                        if (!string.IsNullOrWhiteSpace(Comment) && Comment.Length > 500)
                            error = "Комментарий не должен превышать 500 символов";
                        break;
                }

                return error;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtAmount.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtComment.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            // Проверяем ошибки валидации
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
                var stock = _context.MedicineStocks.FirstOrDefault(ms => ms.MedicineId == _medicine.Id);

                if (stock == null)
                {
                    if (_operation == StockOperation.Remove)
                    {
                        MessageBox.Show("Нельзя списать лекарство, которого нет на складе", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    stock = new MedicineStock
                    {
                        MedicineId = _medicine.Id,
                        Quantity = 0,
                        Location = "Основной склад",
                        ExpiryDate = null
                    };
                    _context.MedicineStocks.Add(stock);
                }

                if (_operation == StockOperation.Add)
                {
                    stock.Quantity += amount;
                }
                else
                {
                    if (stock.Quantity < amount)
                    {
                        MessageBox.Show($"Недостаточно лекарства на складе. Доступно: {stock.Quantity}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    stock.Quantity -= amount;
                }

                _context.SaveChanges();

                if (stock.Quantity < _medicine.MinStock)
                {
                    var notification = new Notification
                    {
                        MedicineId = _medicine.Id,
                        Message = $"Низкий запас {_medicine.Name}. Осталось: {stock.Quantity} {_medicine.Unit}",
                        CreatedDate = DateTime.Now,
                        IsRead = false
                    };
                    _context.Notifications.Add(notification);
                    _context.SaveChanges();
                }

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}