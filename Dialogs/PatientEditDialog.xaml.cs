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
    public partial class PatientEditDialog : Window, IDataErrorInfo
    {
        public string PatientName { get; set; }
        public int OwnerId { get; set; }
        public int AnimalTypeId { get; set; }
        public int? BreedId { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? Weight { get; set; }
        public string Color { get; set; }
        public string DistinctiveFeatures { get; set; }
        public string ChipNumber { get; set; }

        private readonly VeterContext _context = new VeterContext();

        public PatientEditDialog(Patient patient = null)
        {
            InitializeComponent();

            LoadData();

            if (patient != null)
            {
                PatientName = patient.Name;
                OwnerId = patient.OwnerId;
                AnimalTypeId = patient.AnimalTypeId;
                BreedId = patient.BreedId;
                BirthDate = patient.BirthDate;
                Weight = patient.Weight;
                Color = patient.Color;
                DistinctiveFeatures = patient.DistinctiveFeatures;
                ChipNumber = patient.ChipNumber;
            }

            DataContext = this;

            cmbAnimalTypes.SelectionChanged += CmbAnimalTypes_SelectionChanged;
            txtWeight.PreviewTextInput += TxtWeight_PreviewTextInput;
            txtWeight.TextChanged += TxtWeight_TextChanged;

            // Добавляем обработчики валидации
            txtName.TextChanged += ValidateTextBox;
            txtWeight.TextChanged += ValidateTextBox;
            txtColor.TextChanged += ValidateTextBox;
            txtChipNumber.TextChanged += ValidateTextBox;
            txtDistinctiveFeatures.TextChanged += ValidateTextBox;

            cmbOwners.SelectionChanged += ValidateComboBox;
            cmbAnimalTypes.SelectionChanged += ValidateComboBox;
        }

        private void LoadData()
        {
            _context.Owners.Load();
            cmbOwners.ItemsSource = _context.Owners.Local;

            _context.AnimalTypes.Load();
            cmbAnimalTypes.ItemsSource = _context.AnimalTypes.Local;

            _context.Breeds.Load();
            cmbBreeds.ItemsSource = _context.Breeds.Local;
        }

        private void CmbAnimalTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAnimalTypes.SelectedItem is AnimalType selectedType)
            {
                var breeds = _context.Breeds
                    .Where(b => b.AnimalTypeId == selectedType.Id)
                    .ToList();
                cmbBreeds.ItemsSource = breeds;
                cmbBreeds.SelectedIndex = -1;
            }
        }

        private void TxtWeight_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры, точку и запятую
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
            }

            // Проверяем, чтобы точка или запятая была только одна
            string currentText = txtWeight.Text + e.Text;
            if ((e.Text == "." || e.Text == ",") &&
                (currentText.Count(c => c == '.' || c == ',') > 1))
            {
                e.Handled = true;
            }
        }

        private void TxtWeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtWeight.Text))
            {
                Weight = null;
                return;
            }

            string text = txtWeight.Text;
            string cleaned = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

            cleaned = cleaned.Replace(',', '.');

            if (cleaned != text)
            {
                txtWeight.Text = cleaned;
                txtWeight.CaretIndex = cleaned.Length;
            }

            if (decimal.TryParse(cleaned, out decimal result))
            {
                Weight = result;
            }
            else
            {
                Weight = null;
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
                    case nameof(PatientName):
                        if (string.IsNullOrWhiteSpace(PatientName))
                            error = "Кличка обязательна";
                        else if (PatientName.Length > 50)
                            error = "Кличка не должна превышать 50 символов";
                        break;

                    case nameof(OwnerId):
                        if (OwnerId <= 0)
                            error = "Выберите владельца";
                        break;

                    case nameof(AnimalTypeId):
                        if (AnimalTypeId <= 0)
                            error = "Выберите вид животного";
                        break;

                    case nameof(Weight):
                        if (Weight.HasValue)
                        {
                            if (Weight <= 0)
                                error = "Вес должен быть положительным числом";
                            else if (Weight > 1000)
                                error = "Вес не может превышать 1000 кг";
                        }
                        break;

                    case nameof(Color):
                        if (!string.IsNullOrWhiteSpace(Color) && Color.Length > 50)
                            error = "Цвет не должен превышать 50 символов";
                        break;

                    case nameof(ChipNumber):
                        if (!string.IsNullOrWhiteSpace(ChipNumber) && ChipNumber.Length > 50)
                            error = "Номер чипа не должен превышать 50 символов";
                        break;

                    case nameof(DistinctiveFeatures):
                        if (!string.IsNullOrWhiteSpace(DistinctiveFeatures) && DistinctiveFeatures.Length > 500)
                            error = "Особые приметы не должны превышать 500 символов";
                        break;

                    case nameof(BirthDate):
                        if (BirthDate.HasValue)
                        {
                            if (BirthDate > DateTime.Now)
                                error = "Дата рождения не может быть в будущем";
                            else if (BirthDate < new DateTime(1900, 1, 1))
                                error = "Некорректная дата рождения";
                        }
                        break;
                }

                return error;
            }
        }

        private void ValidateTextBox(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                Validation.GetErrors(textBox);
            }
        }

        private void ValidateComboBox(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                Validation.GetErrors(comboBox);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtName.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtWeight.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtColor.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtChipNumber.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            txtDistinctiveFeatures.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

            // Обновляем привязки для ComboBox
            var ownerBinding = cmbOwners.GetBindingExpression(ComboBox.SelectedValueProperty);
            var animalTypeBinding = cmbAnimalTypes.GetBindingExpression(ComboBox.SelectedValueProperty);
            ownerBinding?.UpdateSource();
            animalTypeBinding?.UpdateSource();

            // Собираем все ошибки
            var errors = new List<string>();

            if (!string.IsNullOrEmpty(this["PatientName"]))
                errors.Add($"Кличка: {this["PatientName"]}");
            if (!string.IsNullOrEmpty(this["OwnerId"]))
                errors.Add($"Владелец: {this["OwnerId"]}");
            if (!string.IsNullOrEmpty(this["AnimalTypeId"]))
                errors.Add($"Вид животного: {this["AnimalTypeId"]}");
            if (!string.IsNullOrEmpty(this["Weight"]))
                errors.Add($"Вес: {this["Weight"]}");
            if (!string.IsNullOrEmpty(this["Color"]))
                errors.Add($"Цвет: {this["Color"]}");
            if (!string.IsNullOrEmpty(this["ChipNumber"]))
                errors.Add($"Номер чипа: {this["ChipNumber"]}");
            if (!string.IsNullOrEmpty(this["DistinctiveFeatures"]))
                errors.Add($"Особые приметы: {this["DistinctiveFeatures"]}");
            if (!string.IsNullOrEmpty(this["BirthDate"]))
                errors.Add($"Дата рождения: {this["BirthDate"]}");

            // Проверяем ошибки валидации
            if (errors.Any())
            {
                string errorMessage = "Обнаружены ошибки:\n\n" + string.Join("\n", errors);
                MessageBox.Show(errorMessage,
                    "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Устанавливаем фокус на первое поле с ошибкой
                if (!string.IsNullOrEmpty(this["PatientName"]))
                    txtName.Focus();
                else if (!string.IsNullOrEmpty(this["OwnerId"]))
                    cmbOwners.Focus();
                else if (!string.IsNullOrEmpty(this["AnimalTypeId"]))
                    cmbAnimalTypes.Focus();

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