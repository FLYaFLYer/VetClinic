using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VetClinic.Dialogs
{
    public partial class MedicineEditDialog : Window, IDataErrorInfo
    {
        public string MedicineName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; } = "шт";
        public int MinStock { get; set; } = 10;

        public List<string> Categories { get; set; } = new List<string>
        {
            "Антибиотики", "Витамины", "Вакцины", "Антипаразитарные",
            "Обезболивающие", "ЖКТ", "Перевязочные", "Другое"
        };

        public List<string> Units { get; set; } = new List<string>
        {
            "шт", "уп", "флакон", "упак", "туба", "доза", "мл", "г", "кг"
        };

        private Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        public MedicineEditDialog(Models.Medicine medicine = null)
        {
            InitializeComponent();

            if (medicine != null)
            {
                MedicineName = medicine.Name;
                Description = medicine.Description;
                Category = medicine.Category;
                Price = medicine.Price;
                Unit = medicine.Unit;
                MinStock = medicine.MinStock;
            }
            else
            {
                MedicineName = "";
                Description = "";
                Category = "Витамины";
                Price = 0;
                Unit = "шт";
                MinStock = 10;
            }

            DataContext = this;

            txtPrice.PreviewTextInput += TxtPrice_PreviewTextInput;
            txtMinStock.PreviewTextInput += TxtMinStock_PreviewTextInput;

            // Добавляем обработчики для проверки в реальном времени
            txtName.TextChanged += ValidateTextBox;
            txtPrice.TextChanged += ValidateTextBox;
            txtMinStock.TextChanged += ValidateTextBox;
            cmbUnit.SelectionChanged += ValidateComboBox;
        }

        private void TxtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры, точку и запятую
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
                return;
            }

            // Проверяем, чтобы точка или запятая была только одна
            string currentText = txtPrice.Text + e.Text;
            if ((e.Text == "." || e.Text == ",") &&
                (currentText.Count(c => c == '.' || c == ',') > 1))
            {
                e.Handled = true;
            }
        }

        private void TxtMinStock_PreviewTextInput(object sender, TextCompositionEventArgs e)
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
                    case nameof(MedicineName):
                        if (string.IsNullOrWhiteSpace(MedicineName))
                            error = "Название лекарства обязательно";
                        else if (MedicineName.Length > 100)
                            error = "Название не должно превышать 100 символов";
                        break;

                    case nameof(Description):
                        if (Description != null && Description.Length > 500)
                            error = "Описание не должно превышать 500 символов";
                        break;

                    case nameof(Price):
                        if (Price < 0)
                            error = "Цена не может быть отрицательной";
                        else if (Price > 1000000)
                            error = "Цена слишком высокая (максимум 1,000,000)";
                        break;

                    case nameof(Unit):
                        if (string.IsNullOrWhiteSpace(Unit))
                            error = "Выберите единицу измерения";
                        else if (Unit.Length > 20)
                            error = "Единица измерения не должна превышать 20 символов";
                        break;

                    case nameof(MinStock):
                        if (MinStock < 0)
                            error = "Минимальный запас не может быть отрицательным";
                        else if (MinStock > 100000)
                            error = "Минимальный запас слишком большой (максимум 100,000)";
                        break;
                }

                _validationErrors[columnName] = error;
                return error;
            }
        }

        private void ValidateTextBox(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                string propertyName = textBox.Name.Replace("txt", "");

                // Принудительно вызываем валидацию
                if (propertyName == "Name")
                    Validation.GetErrors(textBox);
                else if (propertyName == "Price")
                    Validation.GetErrors(textBox);
                else if (propertyName == "MinStock")
                    Validation.GetErrors(textBox);
            }
        }

        private void ValidateComboBox(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox != null && comboBox.Name == "cmbUnit")
            {
                Validation.GetErrors(comboBox);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtName.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtPrice.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtMinStock.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            cmbUnit.GetBindingExpression(System.Windows.Controls.Primitives.Selector.SelectedValueProperty)?.UpdateSource();

            // Проверяем ошибки валидации
            bool hasErrors = Validation.GetHasError(txtName) ||
                             Validation.GetHasError(txtPrice) ||
                             Validation.GetHasError(txtMinStock) ||
                             Validation.GetHasError(cmbUnit);

            if (hasErrors)
            {
                // Собираем все сообщения об ошибках
                StringBuilder errorMessages = new StringBuilder();
                errorMessages.AppendLine("Обнаружены ошибки в форме:");
                errorMessages.AppendLine();

                if (Validation.GetHasError(txtName))
                {
                    errorMessages.AppendLine($"• Название: {Validation.GetErrors(txtName)[0].ErrorContent}");
                }

                if (Validation.GetHasError(txtPrice))
                {
                    errorMessages.AppendLine($"• Цена: {Validation.GetErrors(txtPrice)[0].ErrorContent}");
                }

                if (Validation.GetHasError(cmbUnit))
                {
                    errorMessages.AppendLine($"• Единица измерения: {Validation.GetErrors(cmbUnit)[0].ErrorContent}");
                }

                if (Validation.GetHasError(txtMinStock))
                {
                    errorMessages.AppendLine($"• Минимальный запас: {Validation.GetErrors(txtMinStock)[0].ErrorContent}");
                }

                MessageBox.Show(errorMessages.ToString(),
                    "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);

                // Устанавливаем фокус на первое поле с ошибкой
                if (Validation.GetHasError(txtName))
                    txtName.Focus();
                else if (Validation.GetHasError(txtPrice))
                    txtPrice.Focus();
                else if (Validation.GetHasError(cmbUnit))
                    cmbUnit.Focus();
                else if (Validation.GetHasError(txtMinStock))
                    txtMinStock.Focus();

                return;
            }

            // Дополнительные проверки
            if (string.IsNullOrWhiteSpace(MedicineName))
            {
                MessageBox.Show("Название лекарства обязательно", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtName.Focus();
                return;
            }

            if (Price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrice.Focus();
                return;
            }

            if (MinStock <= 0)
            {
                MessageBox.Show("Минимальный запас должен быть положительным числом", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                txtMinStock.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(Unit))
            {
                MessageBox.Show("Выберите единицу измерения", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                cmbUnit.Focus();
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