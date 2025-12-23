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
        private DateTime? _currentFilterDate = null;

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

        // ИЗМЕНЕНИЕ: Изменил private на public для доступа из MainWindow
        public void LoadData(DateTime? filterDate = null)
        {
            try
            {
                _currentFilterDate = filterDate;

                IQueryable<Visit> query = _context.Visits
                    .Include(v => v.Patient)
                    .Include(v => v.Patient.Owner)
                    .Include(v => v.User);

                if (filterDate.HasValue)
                {
                    var targetDate = filterDate.Value.Date;
                    var nextDay = targetDate.AddDays(1);
                    query = query.Where(v => v.VisitDate >= targetDate && v.VisitDate < nextDay);
                }

                var visits = query
                    .OrderByDescending(v => v.VisitDate)
                    .ToList();

                dataGrid.ItemsSource = visits;
                tbStatus.Text = $"Загружено приёмов: {visits.Count}";

                // Автоматически обновляем счетчик уведомлений
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.LoadNotificationsCount();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                dataGrid.ItemsSource = new List<Visit>();
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
                try
                {
                    var newVisit = new Visit
                    {
                        PatientId = dialog.PatientId,
                        UserId = App.CurrentUser.Id,
                        VisitDate = DateTime.Now,
                        Diagnosis = dialog.Diagnosis,
                        Symptoms = dialog.Symptoms,
                        Temperature = !string.IsNullOrWhiteSpace(dialog.Temperature) ?
                            decimal.Parse(dialog.Temperature.Replace(',', '.')) : (decimal?)null,
                        Recommendations = dialog.Recommendations,
                        Status = dialog.Status
                    };

                    _context.Visits.Add(newVisit);
                    _context.SaveChanges();

                    LoadData(_currentFilterDate);
                    MessageBox.Show("Приём успешно добавлен", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении приёма: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

            // Проверяем, можно ли редактировать приём
            if (!visit.CanBeEdited)
            {
                MessageBox.Show("Нельзя редактировать завершённые или отменённые приёмы",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new VisitEditDialog(visit);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    var visitToUpdate = _context.Visits.Find(visit.Id);
                    if (visitToUpdate != null)
                    {
                        visitToUpdate.Diagnosis = dialog.Diagnosis;
                        visitToUpdate.Symptoms = dialog.Symptoms;
                        visitToUpdate.Temperature = !string.IsNullOrWhiteSpace(dialog.Temperature) ?
                            decimal.Parse(dialog.Temperature.Replace(',', '.')) : (decimal?)null;
                        visitToUpdate.Recommendations = dialog.Recommendations;
                        visitToUpdate.Status = dialog.Status;
                        visitToUpdate.NextVisitDate = null;

                        _context.SaveChanges();
                        LoadData(_currentFilterDate);
                        MessageBox.Show("Приём успешно обновлён", "Успех",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении приёма: {ex.Message}", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
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

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData(_currentFilterDate);
            MessageBox.Show("Данные обновлены", "Обновление",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            _context?.Dispose();
        }
    }
}