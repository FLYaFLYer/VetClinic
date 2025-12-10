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
	public partial class MedicinesPage : Page
	{
		private readonly VeterContext _context = new VeterContext();

		public MedicinesPage()
		{
			InitializeComponent();
			CheckPermissions();
			LoadData();
		}

		private void CheckPermissions()
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				btnAddMedicine.IsEnabled = false;
				btnEditMedicine.IsEnabled = false;
				btnDeleteMedicine.IsEnabled = false;
				btnAddStock.IsEnabled = false;
				btnRemoveStock.IsEnabled = false;
			}
		}

		private void LoadData()
		{
			_context.Medicines.Load();
			var medicines = _context.Medicines.Local;

			foreach (var medicine in medicines)
			{
				medicine.TotalQuantity = _context.MedicineStocks
					.Where(ms => ms.MedicineId == medicine.Id)
					.Sum(ms => (int?)ms.Quantity) ?? 0;
			}

			dataGrid.ItemsSource = medicines;
		}

		private void BtnAddMedicine_Click(object sender, RoutedEventArgs e)
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				MessageBox.Show("Нет прав для добавления лекарств");
				return;
			}

			var dialog = new MedicineEditDialog();
			if (dialog.ShowDialog() == true)
			{
				var newMedicine = new Medicine
				{
					Name = dialog.MedicineName,
					Category = dialog.Category,
					Price = dialog.Price,
					Unit = dialog.Unit,
					Description = dialog.Description,
					MinStock = dialog.MinStock
				};

				_context.Medicines.Add(newMedicine);
				_context.SaveChanges();
				LoadData();
			}
		}

		private void BtnEditMedicine_Click(object sender, RoutedEventArgs e)
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				MessageBox.Show("Нет прав для изменения лекарств");
				return;
			}

			if (dataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите лекарство");
				return;
			}

			var medicine = (Medicine)dataGrid.SelectedItem;
			var dialog = new MedicineEditDialog(medicine);
			if (dialog.ShowDialog() == true)
			{
				medicine.Name = dialog.MedicineName;
				medicine.Category = dialog.Category;
				medicine.Price = dialog.Price;
				medicine.Unit = dialog.Unit;
				medicine.Description = dialog.Description;
				medicine.MinStock = dialog.MinStock;

				_context.SaveChanges();
				LoadData();
			}
		}

		private void BtnDeleteMedicine_Click(object sender, RoutedEventArgs e)
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				MessageBox.Show("Нет прав для удаления лекарств");
				return;
			}

			if (dataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите лекарство");
				return;
			}

			var medicine = (Medicine)dataGrid.SelectedItem;
			var result = MessageBox.Show(
				$"Вы уверены, что хотите удалить лекарство '{medicine.Name}'?",
				"Подтверждение удаления",
				MessageBoxButton.YesNo,
				MessageBoxImage.Question);

			if (result == MessageBoxResult.Yes)
			{
				_context.Medicines.Remove(medicine);
				_context.SaveChanges();
				LoadData();
			}
		}

		private void BtnAddStock_Click(object sender, RoutedEventArgs e)
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				MessageBox.Show("Нет прав для изменения запасов");
				return;
			}

			if (dataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите лекарство");
				return;
			}

			var medicine = (Medicine)dataGrid.SelectedItem;
			var dialog = new StockChangeDialog(medicine, StockOperation.Add);
			if (dialog.ShowDialog() == true)
			{
				LoadData();
			}
		}

		private void BtnRemoveStock_Click(object sender, RoutedEventArgs e)
		{
			if (!AccessManager.CanEditMedicines(App.CurrentRole))
			{
				MessageBox.Show("Нет прав для изменения запасов");
				return;
			}

			if (dataGrid.SelectedItem == null)
			{
				MessageBox.Show("Выберите лекарство");
				return;
			}

			var medicine = (Medicine)dataGrid.SelectedItem;
			var dialog = new StockChangeDialog(medicine, StockOperation.Remove);
			if (dialog.ShowDialog() == true)
			{
				LoadData();
			}
		}
	}
}