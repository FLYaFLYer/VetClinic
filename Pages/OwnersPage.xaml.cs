using VetClinic.Data;
using VetClinic.Dialogs;
using VetClinic.Models;
using VetClinic.Utils;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VetClinic.Pages
{
    public partial class OwnersPage : Page
    {
        private readonly VeterContext _context = new VeterContext();

        public OwnersPage()
        {
            InitializeComponent();
            CheckPermissions();
            LoadData();
        }

        private void CheckPermissions()
        {
            if (!AccessManager.CanEditOwners(App.CurrentRole))
            {
                btnAddOwner.IsEnabled = false;
                btnEditOwner.IsEnabled = false;
                btnDeleteOwner.IsEnabled = false;
            }
        }

        // Изменяем на public для доступа из MainWindow
        public void LoadData()
        {
            _context.Owners.Load();
            dataGrid.ItemsSource = _context.Owners.Local;
        }

        private void BtnAddOwner_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditOwners(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для добавления владельцев");
                return;
            }

            var dialog = new OwnerEditDialog();
            if (dialog.ShowDialog() == true)
            {
                var newOwner = new Owner
                {
                    FirstName = dialog.FirstName,
                    LastName = dialog.LastName,
                    Phone = dialog.Phone,
                    Email = dialog.Email,
                    Address = dialog.Address
                };

                _context.Owners.Add(newOwner);
                _context.SaveChanges();
                LoadData();

                MessageBox.Show("Владелец успешно добавлен", "Успех",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnEditOwner_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditOwners(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для изменения владельцев");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите владельца");
                return;
            }

            var owner = (Owner)dataGrid.SelectedItem;
            var dialog = new OwnerEditDialog(owner);
            if (dialog.ShowDialog() == true)
            {
                owner.FirstName = dialog.FirstName;
                owner.LastName = dialog.LastName;
                owner.Phone = dialog.Phone;
                owner.Email = dialog.Email;
                owner.Address = dialog.Address;

                _context.SaveChanges();
                LoadData();
            }
        }

        private void BtnDeleteOwner_Click(object sender, RoutedEventArgs e)
        {
            if (!AccessManager.CanEditOwners(App.CurrentRole))
            {
                MessageBox.Show("Нет прав для удаления владельцев");
                return;
            }

            if (dataGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите владельца");
                return;
            }

            var owner = (Owner)dataGrid.SelectedItem;

            // Проверяем, есть ли у владельца животные
            bool hasPatients = _context.Patients.Any(p => p.OwnerId == owner.Id);
            if (hasPatients)
            {
                MessageBox.Show("Нельзя удалить владельца, у которого есть животные. Сначала удалите или переназначьте животных.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show(
                $"Вы уверены, что хотите удалить владельца '{owner.FullName}'?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _context.Owners.Remove(owner);
                _context.SaveChanges();
                LoadData();
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = txtSearch.Text.ToLower();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                LoadData();
            }
            else
            {
                var filtered = _context.Owners.Local
                    .Where(o => o.LastName.ToLower().Contains(searchText) ||
                               o.FirstName.ToLower().Contains(searchText) ||
                               o.Phone.ToLower().Contains(searchText) ||
                               (o.Email != null && o.Email.ToLower().Contains(searchText)))
                    .ToList();
                dataGrid.ItemsSource = filtered;
            }
        }
    }
}