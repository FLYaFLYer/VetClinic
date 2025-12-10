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
        private VeterContext _context = new VeterContext();
        public Visit NewVisit { get; private set; }

        public VisitEditDialog()
        {
            InitializeComponent();
            LoadData();
            NewVisit = new Visit
            {
                VisitDate = DateTime.Now,
                UserId = App.CurrentUser?.Id ?? 1
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
            if (NewVisit.PatientId <= 0)
            {
                MessageBox.Show("Выберите пациента");
                return;
            }

            if (string.IsNullOrWhiteSpace(NewVisit.Diagnosis))
            {
                MessageBox.Show("Введите диагноз");
                return;
            }

            _context.Visits.Add(NewVisit);
            _context.SaveChanges();

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