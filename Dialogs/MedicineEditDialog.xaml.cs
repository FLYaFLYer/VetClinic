using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        }

        private void TxtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Разрешаем только цифры, точку и запятую
            if (!char.IsDigit(e.Text, 0) && e.Text != "." && e.Text != ",")
            {
                e.Handled = true;
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

                return error;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки
            txtName.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtPrice.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtMinStock.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            // Обновляем привязки для ComboBox
            cmbUnit.GetBindingExpression(System.Windows.Controls.Primitives.Selector.SelectedValueProperty)?.UpdateSource();

            // Проверяем ошибки валидации
            if (Validation.GetHasError(txtName) ||
                Validation.GetHasError(txtPrice) ||
                Validation.GetHasError(txtMinStock) ||
                Validation.GetHasError(cmbUnit))
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