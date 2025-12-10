using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class MedicineEditDialog : Window
    {
        public string MedicineName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; } = "шт";
        public int MinStock { get; set; } = 10;

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

            DataContext = this;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MedicineName))
            {
                MessageBox.Show("Введите название лекарства");
                return;
            }

            if (Price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом");
                return;
            }

            if (MinStock < 0)
            {
                MessageBox.Show("Минимальный запас не может быть отрицательным");
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