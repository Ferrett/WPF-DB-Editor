using Microsoft.Win32;
using ShopWpf.Logic;
using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShopWpf
{
    public partial class MainWindow : Window
    {
        List<dynamic> Table = new List<dynamic>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Put_Click(object sender, RoutedEventArgs e)
        {
            post = false;
            ItemMenu.Visibility = Visibility.Visible;
            
        }

        private async void Put()
        {
            Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;

            if (DevTest.IsFieldEmpty(selectedDev, DeveloperName.Text))
            {
                ShowRequestLog("Error: field is empty");
                return;
            }

            if (DevTest.IsAnyChanges(selectedDev, Logo, DeveloperName.Text))
            {
                ShowRequestLog($"Error: No changes");
                return;
            }

            HideDataGrid();

            string reqContent = $"{DeveloperName.Text}";
            //ShowRequestLog(await DevTest.PutReq(reqContent, Logo, DeveloperName.Text, (TabControl.SelectedItem as TabItem)!.Tag.ToString()!).Result);
            await DevTest.PutReq(DataGrid, reqContent, Logo, DeveloperName.Text, (TabControl.SelectedItem as TabItem)!.Tag.ToString()!);
            ShowRequestLog(DevTest.Result);
            UpdateDataGrid();
        }

        private async void Post()
        {
            if (String.IsNullOrEmpty(DeveloperName.Text))
            {
                ShowRequestLog("Error: Developer name is empty");

                return;
            }

            HideDataGrid();

            string content = $"{DeveloperName.Text}";
            MultipartFormDataContent? multipartContent = null;

            HttpResponseMessage a;
            if (Logo.Source == null)
            {
                a = await Requests.PostRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!, content);
            }
            else
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(Tools.ImageToHttpContent(Logo)), "logo", "filename");
                a = await Requests.PostRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!, content, multipartContent);
            }

            if (a.StatusCode != HttpStatusCode.OK)
            {
                ShowRequestLog($"Error: {(int)a.StatusCode} ({a.StatusCode})   |   {await a.Content.ReadAsStringAsync()}");
                return;
            }
            else
            {
                ShowRequestLog("Data Posted successfuy");
            }

            UpdateDataGrid();
        }
        private async void MenuSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if(post)
            {
                Post();
            }
            else
            {
                Put();
            }
        }

        bool post;
        private void Post_Click(object sender, RoutedEventArgs e)
        {
            post= true;
            ItemMenu.Visibility = Visibility.Visible;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            HideDataGrid();
            HttpResponseMessage a = await Requests.DeleteRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!, Tools.DataGridSelectedID(DataGrid));
            if (a.StatusCode != HttpStatusCode.OK)
            {
                HideDataGrid($"Error: {(int)a.StatusCode} ({a.StatusCode})   |   {await a.Content.ReadAsStringAsync()}");
                return;
            }
            UpdateDataGrid();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemMenu.Visibility = Visibility.Collapsed;
            GetMenu.Visibility = Visibility.Collapsed;
            UpdateDataGrid();
        }

        private void MenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemMenu.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogLoad = new OpenFileDialog();

            if (openFileDialogLoad.ShowDialog() == true)
            {
                LogoDefault.Source = null;
                Logo.Source = new BitmapImage(new Uri(openFileDialogLoad.FileName));
            }
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            switch ((TabControl.SelectedItem as TabItem)!.Tag.ToString()!)
            {
                case "Developer":
                    {
                        Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;

                        var fullFilePath = selectedDev.logoURL;

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                        bitmap.EndInit();

                        GetLogo.Source = bitmap;
                        GetDeveloperName.Text = selectedDev.name;
                        break;
                    }

                default:
                    break;
            }

            GetMenu.Visibility = Visibility.Visible;
        }

        private void StoreDataInTable(string content)
        {
            switch ((TabControl.SelectedItem as TabItem)!.Tag.ToString()!)
            {
                case "Developer":
                    Table.AddRange(JsonSerializer.Deserialize<List<Developer>>(content)!);
                    break;

                case "Game":
                    Table.AddRange(JsonSerializer.Deserialize<List<Game>>(content)!);
                    break;

                case "GameStats":
                    Table.AddRange(JsonSerializer.Deserialize<List<GameStats>>(content)!);
                    break;

                case "Review":
                    Table.AddRange(JsonSerializer.Deserialize<List<Review>>(content)!);
                    break;

                case "User":
                    Table.AddRange(JsonSerializer.Deserialize<List<User>>(content)!);
                    break;

                default:
                    break;
            }
        }



        

        private async void UpdateDataGrid()
        {
            HideDataGrid();

            DataGrid.ItemsSource = null;

            Table = new List<dynamic>();
            HttpResponseMessage a = await Requests.GetRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!);

            if (a.StatusCode != HttpStatusCode.OK)
            {
                HideDataGrid($"Error: {(int)a.StatusCode} ({a.StatusCode})   |   {await a.Content.ReadAsStringAsync()}");
                return;
            }
            StoreDataInTable(a.Content.ReadAsStringAsync().Result);
            DataGrid.ItemsSource = Table;

            ShowDataGrid();
            UpdateCRUDButtons();
        }

        private void ShowDataGrid()
        {
            DataGrid.Visibility = Visibility.Visible;
            ErrorText.Visibility = Visibility.Collapsed;
            MenuSubmitBtn.IsEnabled = true;
        }

        private void ShowRequestLog(string errorText)
        {
            ErrorLogPanel.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = errorText;
        }

        private void HideDataGrid(string errorText = "Loading...")
        {
            CRUDButtons.Visibility = Visibility.Hidden;
            RefreshBtn.Visibility = Visibility.Hidden;
            DataGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Visible;
            MenuSubmitBtn.IsEnabled = false;
            ErrorText.Content = errorText;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
        }

        private void UpdateCRUDButtons()
        {
            CRUDButtons.Visibility = Visibility.Visible;
            RefreshBtn.Visibility = Visibility.Visible;
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
            {
                DeleteBtn.IsEnabled = false;
                GetBtn.IsEnabled = false;
                PutBtn.IsEnabled = false;
            }
            else
            {
                DeleteBtn.IsEnabled = true;
                GetBtn.IsEnabled = true;
                PutBtn.IsEnabled = true;
            }
        }

        private void GetMenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            GetMenu.Visibility = Visibility.Collapsed;
        }
    }
}
