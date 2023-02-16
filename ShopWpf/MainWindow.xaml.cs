using Microsoft.Win32;
using ShopWpf.Logic;
using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ShopWpf
{
    public partial class MainWindow : Window
    {
        List<dynamic> Table = new List<dynamic>();
        bool PostButtonPressed;
        string selectedTableName = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
        }

        private int DataGridSelectedID()
        {
            if (DataGrid.SelectedIndex == -1)
                return -1;

            string id = DataGrid.SelectedValue.ToString()!;
            id = id.Substring(id.IndexOf("=") + 1);
            id = id.Substring(0, id.IndexOf(","));
            return int.Parse(id);
        }

        private void Put_Click(object sender, RoutedEventArgs e)
        {
            PostButtonPressed = false;
            ItemMenu.Visibility = Visibility.Visible;
            MenuSubmitBtn.Content = "Put";
            MenuSubmitBtn.Background = new SolidColorBrush(Colors.Orange);
            ShowDev(Logo, DeveloperName);
        }

        //   ЗАБИНДЬ ТЭГ К ЭНАМУ 
        private async void Put()
        {
            HideDataGrid();
            Dictionary<string, string> content = new Dictionary<string, string>();
            MultipartFormDataContent? multipartContent = null;

            HttpResponseMessage requestResponse;
            string responseMessage = string.Empty;

            if (Logo.Source.ToString().Split('/').Last() != Routes.DefaultLogoName)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(Logo)), "logo", "filename");
            }

            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;

                        if (multipartContent != null)
                            content.Add(Routes.PutLogoRequest, string.Empty);

                        if (DeveloperName.Text != selectedDev.name)
                            content.Add(Routes.PutNameRequest, DeveloperName.Text);

                        break;
                    }
                default:
                    break;
            }

            for (int i = 0; i < content.Count; i++)
            {
                if (content.ElementAt(i).Key == Routes.PutLogoRequest)
                    requestResponse = await Requests.PutRequest(selectedTableName, content.ElementAt(i).Key, DataGridSelectedID(), null, multipartContent);
                else
                    requestResponse = await Requests.PutRequest(selectedTableName, content.ElementAt(i).Key, DataGridSelectedID(), content.ElementAt(i).Value);

                if (requestResponse.StatusCode != HttpStatusCode.OK)
                {
                    responseMessage+= $"Error: {(int)requestResponse.StatusCode} ({requestResponse.StatusCode})\n{await requestResponse.Content.ReadAsStringAsync()}\n\n";
                }
            }

            if (responseMessage == string.Empty)
            {
                ShowRequestLog("Data Updated successfuly");
            }
            else
                ShowRequestLog(responseMessage);

            UpdateDataGrid();
        }

        private async void Post()
        {
            HideDataGrid();
            MultipartFormDataContent multipartContent = null;
            string content = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();

            if (Logo.Source != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(Logo)), "logo", "filename");
            }

            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        content = $"{DeveloperName.Text}";
                        response = await Requests.PostRequest(selectedTableName, content, multipartContent);

                        break;
                    }
                default:
                    break;
            }
            ShowRequestLog(response.StatusCode == HttpStatusCode.OK ? "Data Posted successfuly" :
                            $"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");

            UpdateDataGrid();
        }

        private void MenuSubmitBtn_Click(object sender, RoutedEventArgs e)
        {
            HideCRUDButtons();
            if (PostButtonPressed)
            {
                Post();
            }
            else
            {
                Put();
            }
        }

        public void ClearFields()
        {
            Logo.Source = null;
            DeveloperName.Text = null;
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            PostButtonPressed = true;
            ItemMenu.Visibility = Visibility.Visible;
            MenuSubmitBtn.Content = "Post";
            MenuSubmitBtn.Background = new SolidColorBrush(Colors.Lime);
            ClearFields();
        }

        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (DataGrid.SelectedIndex == -1)
                return;

            HideDataGrid();
            HideCRUDButtons();

            HttpResponseMessage a = await Requests.DeleteRequest(selectedTableName, DataGridSelectedID());
            if (a.StatusCode != HttpStatusCode.OK)
            {
                HideDataGrid($"Error: {(int)a.StatusCode} ({a.StatusCode})\n{await a.Content.ReadAsStringAsync()}");
                return;
            }
            UpdateDataGrid();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CollapseAllGrids();
            selectedTableName = (TabControl.SelectedItem as TabItem)!.Tag.ToString()!;
            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        CollapseAllGrids();
                        LogoMenu.Visibility = Visibility.Visible;
                        DeveloperMenu.Visibility = Visibility.Visible;
                        break;
                    }
                case TableNames.Game:
                    {
                        CollapseAllGrids();
                        LogoMenu.Visibility = Visibility.Visible;
                        GameMenu.Visibility = Visibility.Visible;
                        break;
                    }

                default:
                    break;
            }
            UpdateDataGrid();
        }

        private void CollapseAllGrids()
        {
            ItemMenu.Visibility = Visibility.Collapsed;
            GetMenu.Visibility = Visibility.Collapsed;
            LogoMenu.Visibility = Visibility.Collapsed;
            DeveloperMenu.Visibility = Visibility.Collapsed;
            GameMenu.Visibility= Visibility.Collapsed;
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
                Logo.Source = new BitmapImage(new Uri(openFileDialogLoad.FileName));
                //LogoDefault.Height = Logo.Height;
            }
        }

        public byte[] ImageToHttpContent(Image img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img.Source as BitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public void ShowDev(Image logo, TextBox name)
        {
            Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;
            var fullFilePath = selectedDev.logoURL;

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
            bitmap.EndInit();

            logo.Source = bitmap;
            name.Text = selectedDev.name;
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        ShowDev(GetLogo, GetDeveloperName);
                        break;
                    }

                default:
                    break;
            }

            GetMenu.Visibility = Visibility.Visible;
        }


        private void StoreDataInTable(string content)
        {
            switch (selectedTableName)
            {
                case TableNames.Developer:
                    Table.AddRange(JsonSerializer.Deserialize<List<Developer>>(content)!);
                    break;

                case TableNames.Game:
                    Table.AddRange(JsonSerializer.Deserialize<List<Game>>(content)!);
                    break;

                case TableNames.GameStats:
                    Table.AddRange(JsonSerializer.Deserialize<List<GameStats>>(content)!);
                    break;

                case TableNames.Review:
                    Table.AddRange(JsonSerializer.Deserialize<List<Review>>(content)!);
                    break;

                case TableNames.User:
                    Table.AddRange(JsonSerializer.Deserialize<List<User>>(content)!);
                    break;

                default:
                    break;
            }
        }

        private async void UpdateDataGrid()
        {
            HideDataGrid();
            HideCRUDButtons();

            DataGrid.ItemsSource = null;

            Table = new List<dynamic>();
            HttpResponseMessage a = await Requests.GetRequest(selectedTableName);

            ShowCRUDButtons();
            if (a.StatusCode != HttpStatusCode.OK)
            {
                HideDataGrid($"Error: {(int)a.StatusCode} ({a.StatusCode})\n{await a.Content.ReadAsStringAsync()}");
                return;
            }
            StoreDataInTable(a.Content.ReadAsStringAsync().Result);
            DataGrid.ItemsSource = Table;

            ShowDataGrid();
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
            DataGrid.Visibility = Visibility.Collapsed;
            ErrorText.Visibility = Visibility.Visible;

            ErrorText.Content = errorText;
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
        }
        private void HideCRUDButtons()
        {
            CRUDButtons.Visibility = Visibility.Hidden;
            RefreshBtn.Visibility = Visibility.Hidden;
            MenuSubmitBtn.IsEnabled = false;
        }
        private void ShowCRUDButtons()
        {
            CRUDButtons.Visibility = Visibility.Visible;
            RefreshBtn.Visibility = Visibility.Visible;
            MenuSubmitBtn.IsEnabled = true;
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

        private void ItemMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as Border)!.Visibility == Visibility.Collapsed)
            {
                ErrorLogPanel.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Text = string.Empty;
            }
        }
    }
}
