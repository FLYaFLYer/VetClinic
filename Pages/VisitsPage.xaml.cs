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
				query = query.Where(v => DbFunctions.TruncateTime(v.VisitDate) == filterDate.Value.Date)
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
				LoadData(dpFilterDate.SelectedDate.Value);
			}
		}

		private void BtnClearFilter_Click(object sender, RoutedEventArgs e)
		{
			dpFilterDate.SelectedDate = null;
			LoadData();
		}
	}
}