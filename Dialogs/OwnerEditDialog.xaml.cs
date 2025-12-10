using System.Windows;
using VetClinic.Data;
using VetClinic.Models;

namespace VetClinic.Dialogs
{
    public partial class OwnerEditDialog : Window
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        private readonly VeterContext _context = new VeterContext();

        public OwnerEditDialog(Models.Owner owner = null)
        {
            InitializeComponent();

            if (owner != null)
            {
                LastName = owner.LastName;
                FirstName = owner.FirstName;
                Phone = owner.Phone;
                Email = owner.Email;
                Address = owner.Address;
            }

            DataContext = this;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastName))
            {
                MessageBox.Show("Введите фамилию владельца");
                return;
            }

            if (string.IsNullOrWhiteSpace(FirstName))
            {
                MessageBox.Show("Введите имя владельца");
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone))
            {
                MessageBox.Show("Введите телефон владельца");
                return;
            }

            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}