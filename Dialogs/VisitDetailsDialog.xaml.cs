using System.Data.Entity;
using System.Windows;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class VisitDetailsDialog : Window
    {
        private readonly VeterContext _context = new VeterContext();

        public VisitDetailsDialog(Visit visit)
        {
            InitializeComponent();
            LoadVisitData(visit);
        }

        private void LoadVisitData(Visit visit)
        {
            _context.Entry(visit)
                .Reference(v => v.Patient)
                .Load();

            _context.Entry(visit.Patient)
                .Reference(p => p.Owner)
                .Load();

            txtVisitDate.Text = visit.VisitDate.ToString("dd.MM.yyyy HH:mm");
            txtPatient.Text = $"{visit.Patient.Name} ({visit.Patient.Owner.FullName})";
            txtDiagnosis.Text = visit.Diagnosis;
            txtSymptoms.Text = visit.Symptoms ?? "Не указаны";
            txtRecommendations.Text = visit.Recommendations ?? "Не указаны";
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}