using VetClinic.Data;
using VetClinic.Dialogs;
using VetClinic.Models;
using VetClinic.Utils;
using System;
using System.Collections.Generic;
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
            try
            {
                List<Visit> visits;

                if (filterDate.HasValue)
                {
                    // Способ 1: Загружаем все данные и фильтруем в памяти
                    var allVisits = _context.Visits
                        .Include(v => v.Patient)
                        .Include(v => v.Patient.Owner)
                        .Include(v => v.User)
                        .ToList();

                    var targetDate = filterDate.Value.Date;
                    visits = allVisits
                        .Where(v => v.VisitDate.Date == targetDate)
                        .OrderByDescending(v => v.VisitDate)
                        .ToList();
                }
                else
                {
                    // Без фильтра
                    visits = _context.Visits
                        .Include(v => v.Patient)
                        .Include(v => v.Patient.Owner)
                        .Include(v => v.User)
                        .OrderByDescending(v => v.VisitDate)
                        .ToList();
                }

                dataGrid.ItemsSource = visits;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                dataGrid.ItemsSource = null;
            }
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
                LoadData(dpFilterDate.SelectedDate);
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
                LoadData(dpFilterDate.SelectedDate);
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
            LoadData(dpFilterDate.SelectedDate);
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