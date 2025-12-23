using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class VisitEditDialog : Window, IDataErrorInfo
    {
        public int PatientId { get; set; }
        public string Diagnosis { get; set; }
        public string Symptoms { get; set; }
        public string Temperature { get; set; }
        public string Recommendations { get; set; }
        public string Status { get; set; } = "Запланирован";

        public List<string> Statuses { get; set; } = new List<string>
        {
            "Запланирован", "В процессе", "Завершён", "Отменён"
        };

        private readonly VeterContext _context = new VeterContext();

        public VisitEditDialog(Visit visit = null)
        {
            InitializeComponent();

            LoadData();

            if (visit != null)
            {
                PatientId = visit.PatientId;
                Diagnosis = visit.Diagnosis;
                Symptoms = visit.Symptoms;
                Temperature = visit.Temperature?.ToString();
                Recommendations = visit.Recommendations;
                Status = visit.Status;

                if (visit.Status == "Завершён" || visit.Status == "Отменён")
                {
                    cmbStatus.IsEnabled = false;
                    txtDiagnosis.IsReadOnly = true;
                    txtTemperature.IsReadOnly = true;
                    txtRecommendations.IsReadOnly = true;
                    txtSymptoms.IsReadOnly = true;
                    cmbPatients.IsEnabled = false;
                    btnSave.Content = "Закрыть";
                }
            }

            DataContext = this;

            txtTemperature.PreviewTextInput += TxtTemperature_PreviewTextInput;
        }

        private void LoadData()
        {
            _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.AnimalType)
                .Load();
            cmbPatients.ItemsSource = _context.Patients.Local;
        }

        private void TxtTemperature_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
            }

            string currentText = txtTemperature.Text + e.Text;
            if ((e.Text == "." || e.Text == ",") &&
                (currentText.Count(c => c == '.' || c == ',') > 1))
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
                    case nameof(PatientId):
                        if (PatientId <= 0)
                            error = "Выберите пациента";
                        break;

                    case nameof(Diagnosis):
                        if (string.IsNullOrWhiteSpace(Diagnosis))
                            error = "Диагноз обязателен";
                        else if (Diagnosis.Length > 500)
                            error = "Диагноз не должен превышать 500 символов";
                        break;

                    case nameof(Temperature):
                        if (!string.IsNullOrWhiteSpace(Temperature))
                        {
                            if (decimal.TryParse(Temperature.Replace(',', '.'), out decimal tempValue))
                            {
                                if (tempValue < 20 || tempValue > 50)
                                    error = "Температура должна быть между 20 и 50 градусами";
                            }
                            else
                            {
                                error = "Введите корректное значение температуры";
                            }
                        }
                        break;

                    case nameof(Status):
                        if (string.IsNullOrWhiteSpace(Status))
                            error = "Выберите статус";
                        else if (Status.Length > 50)
                            error = "Статус не должен превышать 50 символов";
                        break;

                    case nameof(Symptoms):
                        if (Symptoms?.Length > 1000)
                            error = "Симптомы не должны превышать 1000 символов";
                        break;

                    case nameof(Recommendations):
                        if (Recommendations?.Length > 1000)
                            error = "Рекомендации не должны превышать 1000 символов";
                        break;
                }

                return error;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (btnSave.Content.ToString() == "Закрыть")
            {
                DialogResult = false;
                Close();
                return;
            }

            if (Status == "Отменён" || Status == "Завершён")
            {
                var result = MessageBox.Show(
                    $"Вы уверены, что хотите изменить статус приёма на '{Status}'?\n\n" +
                    "Это действие может быть необратимо. Продолжить?",
                    "Подтверждение смены статуса",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }
            }

            txtDiagnosis.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtTemperature.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            cmbPatients.GetBindingExpression(ComboBox.SelectedValueProperty)?.UpdateSource();
            cmbStatus.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateSource();

            if (Validation.GetHasError(txtDiagnosis) ||
                Validation.GetHasError(txtTemperature) ||
                Validation.GetHasError(cmbPatients) ||
                Validation.GetHasError(cmbStatus))
            {
                MessageBox.Show("Исправьте ошибки в форме перед сохранением",
                              "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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