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
    public partial class PatientsPage : Page
    {
        private readonly VeterContext _context = new VeterContext();

        public PatientsPage()
        {
            InitializeComponent();
            CheckPermissions();
            LoadData();
        }

        private void CheckPermissions()
        {
            if (!AccessManager.CanEditPatients(App.CurrentRole))
            {
                btnAddPatient.IsEnabled = false;
                btnEditPatient.IsEnabled = false;
                btnDeletePatient.IsEnabled = false;
                btnViewHistory.IsEnabled = false;
            }
        }

        private void LoadData()
        {
            _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.AnimalType)
                .Include(p => p.Breed)
                .Load();

            dataGrid.ItemsSource = _context.Patients.Local;
        }

        private void BtnAddPatient_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditPatients(App.CurrentRole))
            {
                MessageBox.Show(AccessManager.GetNoPermissionMessage("добавления пациентов"));
                return;
            }

            var dialog = new PatientEditDialog();
            if (dialog.ShowDialog() == true)
            {
                var newPatient = new Patient
                {
                    Name = dialog.PatientName,
                    OwnerId = dialog.OwnerId,
                    AnimalTypeId = dialog.AnimalTypeId,
                    BreedId = dialog.BreedId,
                    BirthDate = dialog.BirthDate,
                    Weight = dialog.Weight,
                    Color = dialog.Color,
                    DistinctiveFeatures = dialog.DistinctiveFeatures,
                    ChipNumber = dialog.ChipNumber
                };

                _context.Patients.Add(newPatient);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void BtnEditPatient_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditPatients(App.CurrentRole))
            {
                MessageBox.Show(AccessManager.GetNoPermissionMessage("изменения пациентов"));
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var patient = dataGrid.SelectedItem as Patient;

            // Создаем новый контекст для редактирования
            using (var editContext = new VeterContext())
            {
                // Загружаем пациента с связанными данными
                var patientToEdit = editContext.Patients
                    .Include(p => p.Owner)
                    .Include(p => p.AnimalType)
                    .Include(p => p.Breed)
                    .FirstOrDefault(p => p.Id == patient.Id);

                if (patientToEdit == null)
                {
                    MessageBox.Show("Пациент не найден в базе данных");
                    return;
                }

                var dialog = new PatientEditDialog(patientToEdit);
                if (dialog.ShowDialog() == true)
                {
                    try
                    {
                        // Обновляем свойства
                        patientToEdit.Name = dialog.PatientName;
                        patientToEdit.OwnerId = dialog.OwnerId;
                        patientToEdit.AnimalTypeId = dialog.AnimalTypeId;
                        patientToEdit.BreedId = dialog.BreedId;
                        patientToEdit.BirthDate = dialog.BirthDate;
                        patientToEdit.Weight = dialog.Weight;
                        patientToEdit.Color = dialog.Color;
                        patientToEdit.DistinctiveFeatures = dialog.DistinctiveFeatures;
                        patientToEdit.ChipNumber = dialog.ChipNumber;

                        editContext.SaveChanges();

                        // Обновляем данные в основном контексте
                        LoadData();

                        MessageBox.Show("Данные пациента успешно обновлены",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
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
                        string errorMessage = $"Ошибка обновления: {ex.Message}";
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
        }

        private void BtnDeletePatient_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditPatients(App.CurrentRole))
            {
                MessageBox.Show(AccessManager.GetNoPermissionMessage("удаления пациентов"));
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var patient = dataGrid.SelectedItem as Patient;

            // Проверяем, есть ли связанные записи
            try
            {
                using (var checkContext = new VeterContext())
                {
                    bool hasVisits = checkContext.Visits.Any(v => v.PatientId == patient.Id);
                    if (hasVisits)
                    {
                        MessageBox.Show("Нельзя удалить пациента, у которого есть приёмы. Сначала удалите все приёмы пациента.",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка проверки связанных записей: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить пациента '{patient.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Загружаем пациента в текущий контекст для удаления
                    var patientToDelete = _context.Patients
                        .FirstOrDefault(p => p.Id == patient.Id);

                    if (patientToDelete != null)
                    {
                        _context.Patients.Remove(patientToDelete);
                        _context.SaveChanges();
                        LoadData();

                        MessageBox.Show("Пациент успешно удален",
                            "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                {
                    string errorMessage = "Ошибка при удалении пациента:\n";
                    errorMessage += ex.Message;

                    if (ex.InnerException != null)
                    {
                        errorMessage += $"\n\nВнутренняя ошибка: {ex.InnerException.Message}";

                        // Проверяем, является ли ошибкой нарушение внешнего ключа
                        if (ex.InnerException.InnerException != null)
                        {
                            var innerEx = ex.InnerException.InnerException;
                            if (innerEx.Message.Contains("FK_") || innerEx.Message.Contains("REFERENCE"))
                            {
                                errorMessage += "\n\nПациент связан с другими записями в базе данных. " +
                                               "Удалите сначала связанные записи.";
                            }
                        }
                    }

                    MessageBox.Show(errorMessage, "Ошибка удаления",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}",
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnViewHistory_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            var patient = dataGrid.SelectedItem as Patient;
            var dialog = new PatientHistoryDialog(patient);
            dialog.ShowDialog();
        }
    }
}