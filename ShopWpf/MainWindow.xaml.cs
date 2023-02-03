using ShopWpf.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Loaded += MyWindow_Loaded;
            InitializeComponent();
        }


        private async void MyWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await GetAllData();
        }

        private async Task GetAllData()
        {
            string json = string.Empty;
            string url = @"https://xhvlop3q7v55snb2tvjh7dt57a0jswko.lambda-url.eu-north-1.on.aws/GetAllDevelopers";
            List<Developer> devs = new List<Developer>();

            using (HttpClient client = new HttpClient())
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    ErrorText.Content= $"Error: {(int)response.StatusCode} ({response.StatusCode})   |   {await response.Content.ReadAsStringAsync()}";
                    return;
                }

                devs.AddRange(JsonSerializer.Deserialize<List<Developer>>(await response.Content.ReadAsStringAsync())!);
            }


            TableGrid.ItemsSource = devs;

            

            TableGrid.Visibility = Visibility.Visible;
            ErrorText.Visibility= Visibility.Collapsed;


        }

    }
}
