using Microsoft.Win32;
using ShopWpf.Models;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Printing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;


namespace ShopWpf
{
    public partial class MainWindow : Window
    {
        string APIurl = @"https://xhvlop3q7v55snb2tvjh7dt57a0jswko.lambda-url.eu-north-1.on.aws";
        List<dynamic> Table = new List<dynamic>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async Task GetRequest(string tableName)
        {
            Table = new List<dynamic>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync($"{APIurl}/{tableName}/{Routes.GetRequest}"))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HideDataGrid($"Error: {(int)response.StatusCode} ({response.StatusCode})   |   {await response.Content.ReadAsStringAsync()}");
                    return;
                }

                StoreDataInTable(response.Content.ReadAsStringAsync().Result);
            }
        }

        private async Task DeleteRequest(string tableName, int ID)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.DeleteAsync($"{APIurl}/{tableName}/{Routes.DeleteRequest}/{ID}"))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    HideDataGrid($"Error: {(int)response.StatusCode} ({response.StatusCode})   |   {await response.Content.ReadAsStringAsync()}");
                    return;
                }
            }
        }

        private async Task PostRequest(string tableName, string content)
        {
            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.PostAsync($"{APIurl}/{tableName}/{Routes.PostRequest}/{content}", null))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    ShowRequestLog($"Error: {(int)response.StatusCode} ({response.StatusCode})   |   {await response.Content.ReadAsStringAsync()}");
                }
                else
                {
                    ShowRequestLog("Data Posted successfuy");
                }
            }
        }

        private async void MenuSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(DeveloperName.Text))
            {
                ShowRequestLog("Error: Developer name is empty");

                return;
            }

            string content = $"{DeveloperName.Text}";

            await PostRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!, content);
            UpdateDataGrid();
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

        int DataGridSelectedID;

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            Panelka.Visibility = Visibility.Visible;
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            HideDataGrid();
            await DeleteRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!, DataGridSelectedID);
            UpdateDataGrid();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrid();
        }

        private async void UpdateDataGrid()
        {
            HideDataGrid();

            DataGrid.ItemsSource = null;
            await GetRequest((TabControl.SelectedItem as TabItem)!.Tag.ToString()!);

            if (Table.Count != 0)
            {
                DataGrid.ItemsSource = Table;
                ShowDataGrid();
            }
        }

        private void ShowDataGrid()
        {
            DataGrid.Visibility = Visibility.Visible;
            ErrorText.Visibility = Visibility.Collapsed;
        }

        private void ShowRequestLog(string errorText)
        {
            ErrorLogPanel.Visibility = Visibility.Visible;
            ErrorTextBlock.Text = errorText; 
        }

        private void HideRequestLog()
        {
            ErrorLogPanel.Visibility = Visibility.Collapsed;
        }

        private void HideDataGrid(string errorText = "Loading...")
        {
            DataGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Visible;
            ErrorText.Content = errorText;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
        }



        private void DataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            string a = DataGrid.SelectedValue.ToString()!;
            a = a.Substring(a.IndexOf("=") + 1);
            a = a.Substring(0, a.IndexOf(","));
            DataGridSelectedID = int.Parse(a);
        }

        private void MenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Panelka.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialogLoad = new OpenFileDialog();

            if (openFileDialogLoad.ShowDialog() == true)
            {
                Logo.Source = new BitmapImage(new Uri(openFileDialogLoad.FileName));
            }
        }


    }
}
