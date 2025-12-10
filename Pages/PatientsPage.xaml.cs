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
            var dialog = new PatientEditDialog(patient);
            if (dialog.ShowDialog() == true)
            {
                patient.Name = dialog.PatientName;
                patient.OwnerId = dialog.OwnerId;
                patient.AnimalTypeId = dialog.AnimalTypeId;
                patient.BreedId = dialog.BreedId;
                patient.BirthDate = dialog.BirthDate;
                patient.Weight = dialog.Weight;
                patient.Color = dialog.Color;
                patient.DistinctiveFeatures = dialog.DistinctiveFeatures;
                patient.ChipNumber = dialog.ChipNumber;

                _context.SaveChanges();
                LoadData();
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
            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить пациента '{patient.Name}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
                LoadData();
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