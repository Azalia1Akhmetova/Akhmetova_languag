using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Akhmetova_language
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Client _currentLanguage = new Client();
        List<Gender> GenderTypes = Akhmetova_languageEntities.GetContext().Gender.ToList();
        private bool isDatePickerChanged = false;
        public AddEditPage(Client SelectedClient)
        {
            InitializeComponent();
            if(SelectedClient != null)
            {
                _currentLanguage = SelectedClient;
                this._currentLanguage = SelectedClient;
                ComboType.SelectedIndex = _currentLanguage.GenderCode - 1;
            }
                
            DataContext = _currentLanguage;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentLanguage.LastName))
                errors.AppendLine("Укажите фамилию клиента");
            else
            {
                if (_currentLanguage.LastName.Length > 50)
                    errors.AppendLine("Фамилия клиента не должна превышать 50 символов");
                if (!Regex.IsMatch(_currentLanguage.LastName, @"^[a-zA-Zа-яА-Я\s-]+$"))
                    errors.AppendLine("Фамилия клиента может содержать только буквы, пробелы и дефисы");
            }

            if (string.IsNullOrWhiteSpace(_currentLanguage.FirstName))
                errors.AppendLine("Укажите имя клиента");
            else
            {
                if (_currentLanguage.FirstName.Length > 50)
                    errors.AppendLine("Имя клиента не должна превышать 50 символов");
                if (!Regex.IsMatch(_currentLanguage.FirstName, @"^[a-zA-Zа-яА-Я\s-]+$"))
                    errors.AppendLine("Имя клиента может содержать только буквы, пробелы и дефисы");
            }
            if (string.IsNullOrWhiteSpace(_currentLanguage.Patronymic))
                errors.AppendLine("Укажите отчество клиента");
            else
            {
                if (_currentLanguage.Patronymic.Length > 50)
                    errors.AppendLine("Отчество клиента не должна превышать 50 символов");
                if (!Regex.IsMatch(_currentLanguage.Patronymic, @"^[a-zA-Zа-яА-Я\s-]+$"))
                    errors.AppendLine("Отчество клиента может содержать только буквы, пробелы и дефисы");
            }
            if (!isDatePickerChanged)
                errors.AppendLine("Необходимо выбрать дату рождения клиента");
            if (BirthDate == null)
                errors.AppendLine("Укажите дату рождения");
            if (string.IsNullOrWhiteSpace(_currentLanguage.Phone))
                errors.AppendLine("Укажите телефон клиента");
            else
            {
                string ph = _currentLanguage.Phone.Replace("(", "").Replace("-", "").Replace("+", "").Replace(")", "").Replace(" ", "");
                if (((ph[1] == '9' || ph[1] == '4' || ph[1] == '8') && ph.Length != 11)
                    || (ph[1] == '3' && ph.Length != 12))
                    errors.AppendLine("Укажите правильно телефон клиента");
            }
            if (string.IsNullOrWhiteSpace(_currentLanguage.Email))
                errors.AppendLine("Укажите email клиента");
            else
            {
                if (!Regex.IsMatch(_currentLanguage.Email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                    errors.AppendLine("Укажите корректный email клиента");
            }
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите пол");
            else
            {
                if (ComboType.SelectedIndex == 0) _currentLanguage.GenderCode = 1;
                else _currentLanguage.GenderCode = 2;


            }

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentLanguage.ID == 0)
                Akhmetova_languageEntities.GetContext().Client.Add(_currentLanguage);
            try
            {
                Akhmetova_languageEntities.GetContext().SaveChanges();
                MessageBox.Show("информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }

        }

        private void BtnChangePicture_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                _currentLanguage.PhotoPath = myOpenFileDialog.FileName;
                Photo.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
        }
        
        private void BirthDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            isDatePickerChanged = true;
        }
        
}
}
