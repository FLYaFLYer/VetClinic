using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace VetClinic.Models
{
    [Table("users")]
    public class User : INotifyPropertyChanged
    {
        private string _login;
        private string _lastName;
        private string _firstName;
        private string _middleName;
        private string _phoneNumber;
        private DateTime _dateOfBirth;
        private DateTime _dateOfHire;
        private int _roleId;

        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(50, ErrorMessage = "Имя не должно превышать 50 символов")]
        [Column("first_name")]
        public string FirstName
        {
            get => _firstName;
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    OnPropertyChanged();
                }
            }
        }

        [StringLength(50, ErrorMessage = "Отчество не должно превышать 50 символов")]
        [Column("middle_name")]
        public string MiddleName
        {
            get => _middleName;
            set
            {
                if (_middleName != value)
                {
                    _middleName = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(50, ErrorMessage = "Фамилия не должна превышать 50 символов")]
        [Column("last_name")]
        public string LastName
        {
            get => _lastName;
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Дата рождения обязательна")]
        [Column("date_of_birth")]
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set
            {
                if (_dateOfBirth != value)
                {
                    _dateOfBirth = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Телефон обязателен")]
        [StringLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        [Column("phone_number")]
        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                if (_phoneNumber != value)
                {
                    _phoneNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Дата приема обязательна")]
        [Column("date_of_hire")]
        public DateTime DateOfHire
        {
            get => _dateOfHire;
            set
            {
                if (_dateOfHire != value)
                {
                    _dateOfHire = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Роль обязательна")]
        [Column("role_id")]
        public int RoleId
        {
            get => _roleId;
            set
            {
                if (_roleId != value)
                {
                    _roleId = value;
                    OnPropertyChanged();
                }
            }
        }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(64, ErrorMessage = "Пароль не должен превышать 64 символов")]
        [Column("password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, ErrorMessage = "Логин не должен превышать 50 символов")]
        [Column("login")]
        public string Login
        {
            get => _login;
            set
            {
                if (_login != value)
                {
                    _login = value;
                    OnPropertyChanged();
                }
            }
        }

        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}