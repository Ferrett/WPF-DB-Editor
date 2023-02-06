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
using System.Windows.Shapes;

namespace ShopWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string APIurl = @"https://xhvlop3q7v55snb2tvjh7dt57a0jswko.lambda-url.eu-north-1.on.aws";

        List<dynamic> Table = new List<dynamic>();

        private async Task GetRequest(string tableName)
        {
            Table = new List<dynamic>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync($"{APIurl}/{tableName}/GetAll"))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    ShowError($"Error: {(int)response.StatusCode} ({response.StatusCode})   |   {await response.Content.ReadAsStringAsync()}");
                    return;
                }

                StoreDataInTable(response.Content.ReadAsStringAsync().Result);
            }
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


        private void Post_Click(object sender, RoutedEventArgs e)
        {
            //Http post
            //Table.Add(new Developer { id = 0, logoURL = "temp", name = "temp", registrationDate = DateTime.UtcNow });

            UpdateDataGrid();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

            // delete selected index
            //Http delete
            //Table.Remove(Table.Last());

            UpdateDataGrid();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowError("Loading...");
            UpdateDataGrid();
        }

        private async void UpdateDataGrid()
        {
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

        private void ShowError(string errorText)
        {
            DataGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Visible;
            ErrorText.Content = errorText;
        }
    }
}
