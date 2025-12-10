using VetClinic.Data;
using VetClinic.Models;
using System.Data.Entity;
using System.Linq;
using System.Windows;

namespace VetClinic.Dialogs
{
    public partial class PatientHistoryDialog : Window
    {
        private VeterContext _context = new VeterContext();

        public PatientHistoryDialog(Patient patient)
        {
            InitializeComponent();
            LoadPatientHistory(patient);
        }

        private void LoadPatientHistory(Patient patient)
        {
            txtPatientInfo.Text = $"История болезни: {patient.Name} ({patient.AnimalType.Name})";

            _context.Visits
                .Include(v => v.User)
                .Where(v => v.PatientId == patient.Id)
                .OrderByDescending(v => v.VisitDate)
                .Load();

            dataGrid.ItemsSource = _context.Visits.Local;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}