using VetClinic.Data;
using VetClinic.Dialogs;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Pages
{
    public partial class VisitsPage : Page
    {
        private readonly VeterContext _context = new VeterContext();

        public VisitsPage()
        {
            InitializeComponent();
            CheckPermissions();
            LoadData();
        }

        private void CheckPermissions()
        {
            if (!AccessManager.CanEditVisits(App.CurrentRole))
            {
                btnAddVisit.IsEnabled = false;
                btnEditVisit.IsEnabled = false;
            }
        }

        private void LoadData(DateTime? filterDate = null)
        {
            var query = _context.Visits
                .Include(v => v.Patient)
                .Include(v => v.Patient.Owner)
                .Include(v => v.User)
                .OrderByDescending(v => v.VisitDate);

            if (filterDate.HasValue)
            {
                var date = filterDate.Value.Date;
                // Исправлено (45 проблема) - правильная фильтрация по дате
                query = query.Where(v => System.Data.Entity.DbFunctions.TruncateTime(v.VisitDate) == date)
                    .OrderByDescending(v => v.VisitDate);
            }

            query.Load();
            dataGrid.ItemsSource = _context.Visits.Local;
        }

        private void BtnAddVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditVisits(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для добавления приёмов");
                return;
            }

            var dialog = new VisitEditDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnEditVisit_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditVisits(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для редактирования приёмов");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите приём");
                return;
            }

            var visit = (Visit)dataGrid.SelectedItem;
            var dialog = new VisitEditDialog(visit);
            if (dialog.ShowDialog() == true)
            {
                LoadData();
            }
        }

        private void BtnViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите приём");
                return;
            }

            var visit = (Visit)dataGrid.SelectedItem;
            var dialog = new VisitDetailsDialog(visit);
            dialog.ShowDialog();
        }

        private void DpFilterDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dpFilterDate.SelectedDate.HasValue)
            {
                // Исправлено (45 проблема) - фильтрация по выбранной дате
                LoadData(dpFilterDate.SelectedDate.Value);
            }
            else
            {
                LoadData();
            }
        }

        private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
        {
            dpFilterDate.SelectedDate = null;
            LoadData();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}