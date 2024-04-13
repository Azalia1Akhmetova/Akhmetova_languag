using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для LanguagePage.xaml
    /// </summary>
    public partial class LanguagePage : Page
    {
        int CountInPage = 10;
        int CountRecords;
        int CountPage;
        int CurrentPage = 0;

        List<Client> CurrentPageList = new List<Client>();
        List<Client> TableList;
        public LanguagePage()
        {
            InitializeComponent();
            var currentLanguage = Akhmetova_languageEntities.GetContext().Client.ToList();
            LanguageListView.ItemsSource = currentLanguage;
            FiltrBox.SelectedIndex = 0;
            strCount.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
            TBAllRecords.Text = Akhmetova_languageEntities.GetContext().Client.ToList().Count().ToString();
            Update();
        }
        public void Update()
        {
            var currentClient = Akhmetova_languageEntities.GetContext().Client.ToList();

            if (SortBox.SelectedIndex == 1)
            {
                currentClient = currentClient.OrderBy(p => p.LastName).ToList();
            }
            else if (SortBox.SelectedIndex == 2)
            {

                currentClient = currentClient.OrderByDescending(p => p.CountArrival == 0 ? DateTime.MinValue : Akhmetova_languageEntities.GetContext().
                ClientService.Where(x => x.ClientID == p.ID).Max(q => q.StartTime)).ToList();
            }
            else if (SortBox.SelectedIndex == 3)
            {
                // Сортируем список клиентов по количеству посещений (от большего к меньшему)
                currentClient = currentClient.OrderByDescending(p => p.CountArrival).ToList();
            }

            if (FiltrBox.SelectedIndex == 1)
            {
                currentClient = currentClient.Where(p => p.GenderCode == "2").ToList();
            }
            else if (FiltrBox.SelectedIndex == 2)
            {
                currentClient = currentClient.Where(p => p.GenderCode == "1").ToList();
            }

            currentClient = currentClient.Where(p => p.LastName.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.FirstName.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Patronymic.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Email.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Phone.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").ToLower().Contains(TBoxSearch.Text.Replace("+", "").Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").ToLower())).ToList();

            TBAllRecords.Text = Akhmetova_languageEntities.GetContext().Client.ToList().Count().ToString();
            TBCount.Text = currentClient.Count().ToString();

            LanguageListView.ItemsSource = currentClient;

            TableList = currentClient;

            if (strCount.SelectedIndex == 0)
            {
                CountInPage = 10;
            }
            else if (strCount.SelectedIndex == 1)
            {
                CountInPage = 50;
            }
            else if (strCount.SelectedIndex == 2)
            {
                CountInPage = 200;
            }
            else if (strCount.SelectedIndex == 3)
            {
                CountInPage = 0;
            }


            ChangePage(0, 0);
        }

        private void ChangePage(int direction, int? selectedPage)
        {
            CurrentPageList.Clear();
            CountRecords = TableList.Count;
            if (CountInPage != 0)
            {
                if (CountRecords % CountInPage > 0)
                {
                    CountPage = CountRecords / CountInPage + 1;
                }
                else
                {
                    CountPage = CountRecords / CountInPage;
                }

                Boolean Ifupdate = true;

                int min;

                if (selectedPage.HasValue)
                {
                    if (selectedPage >= 0 && selectedPage <= CountPage)
                    {
                        CurrentPage = (int)selectedPage;
                        min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                        for (int i = CurrentPage * CountInPage; i < min; i++)
                        {
                            CurrentPageList.Add(TableList[i]);
                        }
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case 1:
                            if (CurrentPage > 0)
                            {
                                CurrentPage--;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                        case 2:
                            if (CurrentPage < CountPage - 1)
                            {
                                CurrentPage++;
                                min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                                for (int i = CurrentPage * CountInPage; i < min; i++)
                                {
                                    CurrentPageList.Add(TableList[i]);
                                }
                            }
                            else
                            {
                                Ifupdate = false;
                            }
                            break;
                    }
                }
                if (Ifupdate)
                {
                    PageListBox.Items.Clear();
                    for (int i = 1; i <= CountPage; i++)
                    {
                        PageListBox.Items.Add(i);
                    }
                    PageListBox.SelectedIndex = CurrentPage;

                    //min = CurrentPage * CountInPage + CountInPage < CountRecords ? CurrentPage * CountInPage + CountInPage : CountRecords;
                    //TBCount.Text = min.ToString();
                    //TBAllRecords.Text = CountRecords.ToString();

                    LanguageListView.ItemsSource = CurrentPageList;

                    LanguageListView.Items.Refresh();
                }
            }
            else
            {
                PageListBox.Items.Clear();
                PageListBox.Items.Add(1);
            }
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update();
        }

        private void FiltrBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void strCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Update();
        }

        private void LeftDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(1, null);
        }

        private void PageListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ChangePage(0, Convert.ToInt32(PageListBox.SelectedItem.ToString()) - 1);
        }

        private void RightDirButton_Click(object sender, RoutedEventArgs e)
        {
            ChangePage(2, null);
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var currentClient = (sender as Button).DataContext as Client;

            if (currentClient.CountArrival == 0)
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Akhmetova_languageEntities.GetContext().Client.Remove(currentClient);
                    Akhmetova_languageEntities.GetContext().SaveChanges();
                    LanguageListView.ItemsSource = Akhmetova_languageEntities.GetContext().Client.ToList();
                    Update();
                }
            }
            else
            {
                MessageBox.Show("Невозможно выполнить удаление, так как клиент посещал школу!");
            }
        }
    }
}
