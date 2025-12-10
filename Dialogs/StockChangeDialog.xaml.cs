using VetClinic.Data;
using VetClinic.Models;
using System;
using System.Linq;
using System.Windows;

namespace VetClinic.Dialogs
{
    public enum StockOperation { Add, Remove }

    public partial class StockChangeDialog : Window
    {
        private readonly Medicine _medicine;
        private readonly StockOperation _operation;
        private VeterContext _context = new VeterContext();

        public StockChangeDialog(Medicine medicine, StockOperation operation)
        {
            InitializeComponent();
            _medicine = medicine;
            _operation = operation;

            txtName.Text = medicine.Name;
            Title = _operation == StockOperation.Add ? "Приход лекарства" : "Расход лекарства";
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtAmount.Text, out int amount) || amount <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            var stock = _context.MedicineStocks.FirstOrDefault(ms => ms.MedicineId == _medicine.Id);

            if (stock == null)
            {
                if (_operation == StockOperation.Remove)
                {
                    MessageBox.Show("Нельзя списать лекарство, которого нет на складе");
                    return;
                }

                stock = new MedicineStock
                {
                    MedicineId = _medicine.Id,
                    Quantity = 0,
                    Location = "Основной склад"
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
                    MessageBox.Show("Недостаточно лекарства на складе");
                    return;
                }
                stock.Quantity -= amount;
            }

            _context.SaveChanges();

            // Создаем уведомление при низком запасе
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

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}