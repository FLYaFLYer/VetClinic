using VetClinic.Data;
using VetClinic.Models;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        private void ViewDiagnosis_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag is string diagnosis)
            {
                if (!string.IsNullOrEmpty(diagnosis))
                {
                    var dialog = new Window
                    {
                        Title = "Диагноз",
                        Width = 400,
                        Height = 300,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        Owner = this
                    };

                    var textBox = new TextBox
                    {
                        Text = diagnosis,
                        IsReadOnly = true,
                        TextWrapping = TextWrapping.Wrap,
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                        Margin = new Thickness(10),
                        FontSize = 14
                    };

                    var closeButton = new Button
                    {
                        Content = "Закрыть",
                        Width = 100,
                        Height = 36,
                        Margin = new Thickness(0, 10, 10, 10),
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Style = (Style)FindResource("NavButtonStyle")
                    };

                    closeButton.Click += (s, args) => dialog.Close();

                    var stackPanel = new StackPanel();
                    stackPanel.Children.Add(textBox);
                    stackPanel.Children.Add(closeButton);

                    dialog.Content = stackPanel;
                    dialog.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Диагноз не указан",
                        "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            _context?.Dispose();
        }
    }
}