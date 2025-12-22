using System.Data.Entity;
using System.Linq;
using System.Windows;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class VisitDetailsDialog : Window
    {
        public VisitDetailsDialog(Visit visit)
        {
            InitializeComponent();
            LoadVisitData(visit.Id);
        }

        private void LoadVisitData(int visitId)
        {
            using (var context = new VeterContext())
            {
                var visit = context.Visits
                    .Include(v => v.Patient)
                    .Include(v => v.Patient.Owner)
                    .Include(v => v.User)
                    .FirstOrDefault(v => v.Id == visitId);

                if (visit != null)
                {
                    txtVisitDate.Text = visit.VisitDate.ToString("dd.MM.yyyy HH:mm");
                    txtPatient.Text = $"{visit.Patient?.Name} ({visit.Patient?.Owner?.FullName})";
                    txtDiagnosis.Text = visit.Diagnosis ?? "Не указан";
                    txtSymptoms.Text = visit.Symptoms ?? "Не указаны";
                    txtTemperature.Text = visit.TemperatureFormatted;
                    txtWeight.Text = visit.WeightFormatted;
                    txtRecommendations.Text = visit.Recommendations ?? "Не указаны";
                    txtNextVisit.Text = visit.NextVisitFormatted;
                    txtStatus.Text = visit.Status ?? "Не указан";
                    txtVeterinarian.Text = visit.User?.FullName ?? "Не указан";
                }
                else
                {
                    MessageBox.Show("Приём не найден", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}