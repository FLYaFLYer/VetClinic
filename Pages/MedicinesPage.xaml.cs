using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VetClinic.Data;
using VetClinic.Dialogs;
using VetClinic.Models;
using VetClinic.Utils;

namespace VetClinic.Pages
{
    public partial class MedicinesPage : Page
    {
        private readonly VeterContext _context = new VeterContext();

        public MedicinesPage()
        {
            InitializeComponent();
            CheckPermissions();
            LoadData();
        }

        private void CheckPermissions()
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                btnAddMedicine.IsEnabled = false;
                btnEditMedicine.IsEnabled = false;
                btnDeleteMedicine.IsEnabled = false;
                btnAddStock.IsEnabled = false;
                btnRemoveStock.IsEnabled = false;
            }
        }

        public void LoadData()
        {
            try
            {
                // Сбрасываем изменения в контексте
                _context.ChangeTracker.Entries().ToList().ForEach(entry => entry.State = EntityState.Detached);

                // Загружаем данные заново
                var medicines = _context.Medicines.ToList();

                foreach (var medicine in medicines)
                {
                    medicine.TotalQuantity = _context.MedicineStocks
                        .Where(ms => ms.MedicineId == medicine.Id)
                        .Sum(ms => (int?)ms.Quantity) ?? 0;
                }

                // Очищаем и обновляем DataGrid
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = medicines;
                dataGrid.Items.Refresh();

                // Обновляем статус
                int lowStockCount = medicines.Count(m => m.TotalQuantity < m.MinStock);
                tbStatus.Text = $"Лекарств: {medicines.Count}, с низким запасом: {lowStockCount}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для добавления лекарств");
                return;
            }

            var dialog = new MedicineEditDialog();
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var newMedicine = new Medicine
                    {
                        Name = dialog.MedicineName,
                        Category = dialog.Category,
                        Price = dialog.Price,
                        Unit = dialog.Unit,
                        Description = dialog.Description,
                        MinStock = dialog.MinStock
                    };

                    _context.Medicines.Add(newMedicine);
                    _context.SaveChanges();

                    LoadData();

                    MessageBox.Show("Лекарство успешно добавлено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    string errorMessage = "Ошибки валидации:\n";
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            errorMessage += $"- {error.PropertyName}: {error.ErrorMessage}\n";
                        }
                    }
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    string errorMessage = $"Ошибка обновления базы данных: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\nВнутренняя ошибка: {ex.InnerException.Message}";
                    }
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnEditMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для изменения лекарств");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите лекарство");
                return;
            }

            var medicine = (Medicine)dataGrid.SelectedItem;
            var dialog = new MedicineEditDialog(medicine);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Создаем новый контекст для редактирования
                    using (var editContext = new VeterContext())
                    {
                        var medicineToUpdate = editContext.Medicines.Find(medicine.Id);
                        if (medicineToUpdate != null)
                        {
                            medicineToUpdate.Name = dialog.MedicineName;
                            medicineToUpdate.Category = dialog.Category;
                            medicineToUpdate.Price = dialog.Price;
                            medicineToUpdate.Unit = dialog.Unit;
                            medicineToUpdate.Description = dialog.Description;
                            medicineToUpdate.MinStock = dialog.MinStock;

                            editContext.SaveChanges();
                        }
                    }

                    LoadData();

                    MessageBox.Show("Лекарство успешно обновлено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    string errorMessage = "Ошибки валидации:\n";
                    foreach (var validationError in ex.EntityValidationErrors)
                    {
                        foreach (var error in validationError.ValidationErrors)
                        {
                            errorMessage += $"- {error.PropertyName}: {error.ErrorMessage}\n";
                        }
                    }
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    string errorMessage = $"Ошибка обновления базы данных: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\nВнутренняя ошибка: {ex.InnerException.Message}";
                    }
                    MessageBox.Show(errorMessage, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteMedicine_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для удаления лекарств");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите лекарство");
                return;
            }

            var medicine = (Medicine)dataGrid.SelectedItem;
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить лекарство '{medicine.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Используем отдельный контекст для проверки
                    using (var checkContext = new VeterContext())
                    {
                        bool hasStocks = checkContext.MedicineStocks.Any(ms => ms.MedicineId == medicine.Id);
                        bool hasPrescriptions = checkContext.Prescriptions.Any(p => p.MedicineId == medicine.Id);
                        bool hasNotifications = checkContext.Notifications.Any(n => n.MedicineId == medicine.Id);

                        if (hasStocks || hasPrescriptions || hasNotifications)
                        {
                            string message = "Нельзя удалить лекарство, которое:\n";
                            if (hasStocks) message += "• Есть на складе\n";
                            if (hasPrescriptions) message += "• Назначено пациентам\n";
                            if (hasNotifications) message += "• Есть связанные уведомления\n";
                            message += "\nСначала удалите все связанные записи.";

                            MessageBox.Show(message,
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    // Используем отдельный контекст для удаления
                    using (var deleteContext = new VeterContext())
                    {
                        var medicineToDelete = deleteContext.Medicines.Find(medicine.Id);
                        if (medicineToDelete != null)
                        {
                            deleteContext.Medicines.Remove(medicineToDelete);
                            deleteContext.SaveChanges();
                        }
                    }

                    LoadData();

                    MessageBox.Show("Лекарство успешно удалено", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnAddStock_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для изменения запасов");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите лекарство");
                return;
            }

            var medicine = (Medicine)dataGrid.SelectedItem;
            var dialog = new StockChangeDialog(medicine, StockOperation.Add);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnRemoveStock_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditMedicines(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для изменения запасов");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите лекарство");
                return;
            }

            var medicine = (Medicine)dataGrid.SelectedItem;
            var dialog = new StockChangeDialog(medicine, StockOperation.Remove);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
            MessageBox.Show("Данные обновлены", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            AutoRefreshHelper.StopAutoRefresh();
            _context?.Dispose();
        }
    }
}