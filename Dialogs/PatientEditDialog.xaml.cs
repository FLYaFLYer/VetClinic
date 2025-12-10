using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class PatientEditDialog : Window
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

        private void CmbAnimalTypes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cmbAnimalTypes.SelectedItem is AnimalType selectedType)
            {
                var breeds = _context.Breeds
                    .Where(b => b.AnimalTypeId == selectedType.Id)
                    .ToList();
                cmbBreeds.ItemsSource = breeds;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PatientName))
            {
                MessageBox.Show("Введите кличку животного");
                return;
            }

            if (OwnerId <= 0)
            {
                MessageBox.Show("Выберите владельца");
                return;
            }

            if (AnimalTypeId <= 0)
            {
                MessageBox.Show("Выберите вид животного");
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