using VetClinic.Data;
using VetClinic.Models;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class VisitEditDialog : Window
    {
        private readonly VeterContext _context = new VeterContext();
        public Visit NewVisit { get; private set; }

        public VisitEditDialog()
        {
            InitializeComponent();
            LoadData();
            NewVisit = new Visit
            {
                VisitDate = DateTime.Now,
                UserId = App.CurrentUser?.Id ?? 1,
                Status = "Завершён"
            };
            DataContext = NewVisit;
        }

        private void LoadData()
        {
            _context.Patients
                .Include(p => p.Owner)
                .Include(p => p.AnimalType)
                .Load();

            cmbPatients.ItemsSource = _context.Patients.Local;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            // Принудительно обновляем привязки для срабатывания валидации
            txtDiagnosis.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtTemperature.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();
            txtWeight.GetBindingExpression(System.Windows.Controls.TextBox.TextProperty)?.UpdateSource();

            // Проверяем наличие ошибок валидации
            if (System.Windows.Controls.Validation.GetHasError(txtDiagnosis) ||
                System.Windows.Controls.Validation.GetHasError(txtTemperature) ||
                System.Windows.Controls.Validation.GetHasError(txtWeight) ||
                NewVisit.PatientId <= 0)
            {
                MessageBox.Show("Исправьте ошибки в форме перед сохранением",
                    "Ошибки валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _context.Visits.Add(NewVisit);
                _context.SaveChanges();

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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