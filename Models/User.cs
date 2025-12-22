using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace VetClinic.Models
{
    [Table("users")]
    public class User : INotifyPropertyChanged, System.ComponentModel.IDataErrorInfo
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

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = null;

                switch (columnName)
                {
                    case nameof(Login):
                        if (string.IsNullOrWhiteSpace(Login))
                            error = "Логин обязателен";
                        else if (Login.Length > 50)
                            error = "Логин не должен превышать 50 символов";
                        else if (!System.Text.RegularExpressions.Regex.IsMatch(Login, @"^[a-zA-Z0-9_]+$"))
                            error = "Логин может содержать только буквы, цифры и подчеркивания";
                        break;

                    case nameof(LastName):
                        if (string.IsNullOrWhiteSpace(LastName))
                            error = "Фамилия обязательна";
                        else if (LastName.Length > 50)
                            error = "Фамилия не должна превышать 50 символов";
                        break;

                    case nameof(FirstName):
                        if (string.IsNullOrWhiteSpace(FirstName))
                            error = "Имя обязательно";
                        else if (FirstName.Length > 50)
                            error = "Имя не должно превышать 50 символов";
                        break;

                    case nameof(PhoneNumber):
                        if (string.IsNullOrWhiteSpace(PhoneNumber))
                            error = "Телефон обязателен";
                        else
                        {
                            string digits = System.Text.RegularExpressions.Regex.Replace(PhoneNumber, @"[^\d]", "");
                            if (digits.Length < 10 || digits.Length > 15)
                                error = "Введите корректный номер телефона (10-15 цифр)";
                        }
                        break;

                    case nameof(DateOfBirth):
                        if (DateOfBirth > DateTime.Now)
                            error = "Дата рождения не может быть в будущем";
                        else if (DateOfBirth < DateTime.Now.AddYears(-100))
                            error = "Некорректная дата рождения";
                        break;

                    case nameof(DateOfHire):
                        if (DateOfHire > DateTime.Now)
                            error = "Дата приема не может быть в будущем";
                        else if (DateOfHire < DateOfBirth)
                            error = "Дата приема не может быть раньше даты рождения";
                        break;

                    case nameof(RoleId):
                        if (RoleId <= 0)
                            error = "Выберите роль пользователя";
                        break;
                }

                return error;
            }
        }
    }
}