using Microsoft.Win32;
using ShopWpf.Logic;
using ShopWpf.Models;
using ShopWpf.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading;
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

            DataContext = new DeveloperViewModel();
        }

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
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(Logo)), "logo");
                content.Add(Routes.PutLogoRequest, string.Empty);
            }

            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;

                        if (DeveloperName.Text != selectedDev.name)
                            content.Add(Routes.PutNameRequest, DeveloperName.Text);
                        break;
                    }
                case TableNames.Game:
                    {
                        Game selectedGame = (Table[DataGrid.SelectedIndex] as Game)!;

                        if (GameName.Text != selectedGame.name)
                            content.Add(Routes.PutNameRequest, GameName.Text);
                        if (GamePrice.Text != selectedGame.price.ToString())
                            content.Add(Routes.PutPriceRequest, GamePrice.Text);
                        if (GameAchCount.Text != selectedGame.achievementsCount.ToString())
                            content.Add(Routes.PutAchievementsCountRequest, GameAchCount.Text);
                        break;
                    }
                case TableNames.GameStats:
                    {
                        GameStats selectedGameStats = (Table[DataGrid.SelectedIndex] as GameStats)!;

                        if (GameStatsIsGameLaunched.IsChecked == true)
                            content.Add(Routes.PutGameLaunchedRequest, string.Empty);
                        if (GameStatsGottenAchievements.Text != selectedGameStats.achievementsGot.ToString())
                            content.Add(Routes.PutGottenAchievementsRequest, GameStatsGottenAchievements.Text);
                        if (GameStatsHoursPlayed.Text != selectedGameStats.hoursPlayed.ToString())
                            content.Add(Routes.PutHoursPlayedRequest, GameStatsHoursPlayed.Text);
                        break;
                    }
                case TableNames.Review:
                    {
                        Review selectedReview = (Table[DataGrid.SelectedIndex] as Review)!;

                        if (ReviewIsPositive.IsChecked != selectedReview.isPositive)
                            content.Add(Routes.PutGameLaunchedRequest, ReviewIsPositive.IsChecked.ToString()!);
                        if (ReviewText.Text != selectedReview.text)
                            content.Add(Routes.PutTextRequest, ReviewText.Text);
                        break;
                    }
                case TableNames.User:
                    {
                        User selectedUser = (Table[DataGrid.SelectedIndex] as User)!;

                        if (UserEmail.Text != selectedUser.email)
                            content.Add(Routes.PutEmailRequest, UserEmail.Text);
                        if (UserNickname.Text != selectedUser.nickame)
                            content.Add(Routes.PutNicknameRequest, UserNickname.Text);
                        if (UserLogin.Text != selectedUser.login)
                            content.Add(Routes.PutNicknameRequest, UserLogin.Text);
                        if (UserPassword.Text != selectedUser.passwordHash)
                            content.Add(Routes.PutNicknameRequest, UserPassword.Text);

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
                    responseMessage += $"Error: {(int)requestResponse.StatusCode} ({requestResponse.StatusCode})\n{await requestResponse.Content.ReadAsStringAsync()}\n\n";
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
            MultipartFormDataContent? multipartContent = null;
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
                        break;
                    }
                case TableNames.Game:
                    {
                        content = $"{GameName.Text}/{GamePrice.Text}/{GameDevID.Text}";
                        content += GameAchCount.Text != string.Empty ? $"?achCount={GameAchCount.Text}" : string.Empty;
                        break;
                    }
                case TableNames.GameStats:
                    {
                        content = $"{GameStatsUserID.Text}/{GameStatsGameID.Text}";
                        break;
                    }
                case TableNames.Review:
                    {
                        content = $"{ReviewIsPositive.IsChecked}/{ReviewUserID.Text}/{ReviewGameID.Text}";
                        content += ReviewText.Text != string.Empty ? $"?text={ReviewText.Text}" : string.Empty;
                        break;
                    }
                case TableNames.User:
                    {
                        content = $"{UserLogin.Text}/{UserPassword.Text}/{UserNickname.Text}";
                        content += UserEmail.Text != string.Empty ? $"?email={UserEmail.Text}" : string.Empty;
                        break;
                    }
                default:
                    break;
            }
            response = await Requests.PostRequest(selectedTableName, content, multipartContent);

            ShowRequestLog(response.StatusCode == HttpStatusCode.OK ? "Data Posted successfuly" :
                            $"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");

            UpdateDataGrid();
        }

        private void ShowPutPostGrid()
        {
            //CollapseAllGrids();
            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        LogoMenu.Visibility = Visibility.Visible;
                        DeveloperMenu.Visibility = Visibility.Visible;
                        break;
                    }
                case TableNames.Game:
                    {
                        LogoMenu.Visibility = Visibility.Visible;

                        if (PostButtonPressed)
                        {
                            PostGameMenu.Visibility = Visibility.Visible;
                            PutGameMenu.Visibility = Visibility.Visible;
                        }
                        else
                            PutGameMenu.Visibility = Visibility.Visible;

                        break;
                    }
                case TableNames.GameStats:
                    {
                        if (PostButtonPressed)
                            PostGameStatsMenu.Visibility = Visibility.Visible;
                        else
                            PutGameStatsMenu.Visibility = Visibility.Visible;
                        break;
                    }
                case TableNames.Review:
                    {
                        if (PostButtonPressed)
                        {
                            PostReviewMenu.Visibility = Visibility.Visible;
                            PutReviewMenu.Visibility = Visibility.Visible;
                        }
                        else
                            PutReviewMenu.Visibility = Visibility.Visible;
                        break;
                    }
                case TableNames.User:
                    {
                        LogoMenu.Visibility = Visibility.Visible;
                        UserMenu.Visibility = Visibility.Visible;
                        break;
                    }

                default:
                    break;
            }
        }

        private void Post_Click(object sender, RoutedEventArgs e)
        {
            PostButtonPressed = true;
            ClearAllFields();
            ShowPutPostGrid();
            ItemMenu.Visibility = Visibility.Visible;
            ItemIDMenu.Visibility = Visibility.Collapsed;
            MenuSubmitBtn.Content = "Post";
            MenuSubmitBtn.Background = new SolidColorBrush(Colors.Lime);
        }

        private void ClearAllFields()
        {
            Logo.Source = null;
            DeveloperName.Text = null;
            GameDevID.Text = string.Empty;
            GameStatsUserID.Text = string.Empty;
            GameStatsGameID.Text = string.Empty;
            ReviewGameID.Text = string.Empty;
            ReviewUserID.Text = string.Empty;
            UserLogin.Text = string.Empty;
            UserEmail.Text = string.Empty;
            UserPassword.Text = string.Empty;
            UserNickname.Text = string.Empty;
        }

        private void Put_Click(object sender, RoutedEventArgs e)
        {
            PostButtonPressed = false;
            ShowPutPostGrid();
            ItemMenu.Visibility = Visibility.Visible;
            ItemIDMenu.Visibility = Visibility.Visible;
            MenuSubmitBtn.Content = "Put";
            MenuSubmitBtn.Background = new SolidColorBrush(Colors.Orange);
            ShowTableItem(false);
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
                Logo.Source = new BitmapImage(new Uri(openFileDialogLoad.FileName));
            }
        }

        private void Get_Click(object sender, RoutedEventArgs e)
        {
            ItemMenuGet.Visibility = Visibility.Visible;
            ItemIDMenuGet.Visibility = Visibility.Visible;
            ShowTableItem(true);
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            UpdateDataGrid();
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

                if (ItemMenu.Visibility == Visibility.Visible && PostButtonPressed == false)
                    ShowTableItem(false);

            }
        }

        private void GetMenuCloseBtn_Click(object sender, RoutedEventArgs e)
        {
            ItemMenuGet.Visibility = Visibility.Collapsed;
        }

        private void ItemMenu_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((sender as Border)!.Visibility == Visibility.Collapsed)
            {
                ErrorLogPanel.Visibility = Visibility.Collapsed;
                ErrorTextBlock.Text = string.Empty;
                Logo.Source = null;
            }
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
            int selectedIndex = DataGrid.SelectedIndex;
            DataGrid.ItemsSource = null;

            Table = new List<dynamic>();
            HttpResponseMessage response = await Requests.GetRequest(selectedTableName);

            ShowCRUDButtons();
            if (response.StatusCode != HttpStatusCode.OK)
            {
                HideDataGrid($"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");
                return;
            }
            StoreDataInTable(response.Content.ReadAsStringAsync().Result);

            DataGrid.ItemsSource = Table;
            DataGrid.SelectedIndex = selectedIndex >= DataGrid.Items.Count ? DataGrid.Items.Count - 1 : selectedIndex;
            if (DataGrid.Items.Count == 0)
                ItemMenu.Visibility = Visibility.Collapsed;

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

        private void HideCRUDButtons()
        {
            CRUDButtons.Visibility = Visibility.Hidden;
            RefreshBtn.Visibility = Visibility.Hidden;
            MenuSubmitBtn.IsEnabled = false;
            SelectImageBtn.IsEnabled = false;
        }
        private void ShowCRUDButtons()
        {
            CRUDButtons.Visibility = Visibility.Visible;
            RefreshBtn.Visibility = Visibility.Visible;
            MenuSubmitBtn.IsEnabled = true;
            SelectImageBtn.IsEnabled = true;
        }

        private void CollapseAllGrids()
        {
            ItemMenu.Visibility = Visibility.Collapsed;
            ItemMenuGet.Visibility = Visibility.Collapsed;
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

        public BitmapImage LogoUrlToBitmapImage(string logoUrl)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(logoUrl, UriKind.Absolute);
            bitmap.EndInit();

            return bitmap;
        }

        public void ShowTableItem(bool getClicked)
        {
            switch (selectedTableName)
            {
                case TableNames.Developer:
                    {
                        Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;
                        if (getClicked)
                        {
                            DeveloperMenuGet.Visibility = Visibility.Visible;
                            LogoGet.Source = LogoUrlToBitmapImage(selectedDev.logoURL);
                            ItemIDGet.Text = selectedDev.id.ToString();
                            DeveloperNameGet.Text = selectedDev.name;
                            DeveloperRegistrationDateGet.Text = selectedDev.registrationDate.ToString();
                        }
                        else
                        {
                            Logo.Source = LogoUrlToBitmapImage(selectedDev.logoURL);
                            DeveloperName.Text = selectedDev.name;
                            ItemID.Text = selectedDev.id.ToString();
                        }

                        break;
                    }
                case TableNames.Game:
                    {
                        Game selectedGame = (Table[DataGrid.SelectedIndex] as Game)!;
                        if (getClicked)
                        {
                            GameMenuGet.Visibility = Visibility.Visible;
                            LogoGet.Source = LogoUrlToBitmapImage(selectedGame.logoURL);
                            ItemIDGet.Text = selectedGame.id.ToString();
                            GameDevIDGet.Text = selectedGame.developerID.ToString();
                            GameNameGet.Text = selectedGame.name;
                            GamePriceGet.Text = selectedGame.price.ToString();
                            GameAchCountGet.Text = selectedGame.achievementsCount.ToString();
                            GamePublishDateGet.Text = selectedGame.publishDate.ToString();
                        }
                        else
                        {
                            Logo.Source = LogoUrlToBitmapImage(selectedGame.logoURL);
                            GameName.Text = selectedGame.name;
                            GamePrice.Text = selectedGame.price.ToString();
                            GameAchCount.Text = selectedGame.achievementsCount.ToString();
                            ItemID.Text = selectedGame.id.ToString();
                        }

                        break;
                    }
                case TableNames.GameStats:
                    {
                        GameStats selectedGameStats = (Table[DataGrid.SelectedIndex] as GameStats)!;
                        if (getClicked)
                        {
                            GameStatsMenuGet.Visibility = Visibility.Visible;
                            ItemIDGet.Text = selectedGameStats.id.ToString();
                            GameStatsUserIDGet.Text = selectedGameStats.userID.ToString();
                            GameStatsGameIDGet.Text = selectedGameStats.gameID.ToString();
                            GameStatsGottenAchievementsGet.Text = selectedGameStats.achievementsGot.ToString();
                            GameStatsHoursPlayedGet.Text = selectedGameStats.hoursPlayed.ToString();
                        }
                        else
                        {
                            GameStatsIsGameLaunched.IsChecked = false;
                            GameStatsGottenAchievements.Text = selectedGameStats.achievementsGot.ToString();
                            GameStatsHoursPlayed.Text = selectedGameStats.hoursPlayed.ToString();
                            ItemID.Text = selectedGameStats.id.ToString();
                        }

                        break;
                    }
                case TableNames.Review:
                    {
                        Review selectedReview = (Table[DataGrid.SelectedIndex] as Review)!;
                        if (getClicked)
                        {
                            ReviewMenuGet.Visibility = Visibility.Visible;
                            ItemIDGet.Text = selectedReview.id.ToString();
                            ReviewGameIDGet.Text = selectedReview.gameID.ToString();
                            ReviewUserIDGet.Text = selectedReview.userID.ToString();
                            ReviewIsPositiveGet.IsChecked = selectedReview.isPositive;
                            ReviewTextGet.Text = selectedReview.text;
                            ReviewCreationDateGet.Text = selectedReview.creationDate.ToString();
                            ReviewLastEditDateGet.Text = selectedReview.lastEditDate.ToString();
                        }
                        else
                        {
                            ReviewIsPositive.IsChecked = selectedReview.isPositive;
                            ReviewText.Text = selectedReview.text;
                            ItemID.Text = selectedReview.id.ToString();
                        }

                        break;
                    }
                case TableNames.User:
                    {
                        User selectedUser = (Table[DataGrid.SelectedIndex] as User)!;
                        if (getClicked)
                        {
                            UserMenuGet.Visibility = Visibility.Visible;
                            LogoGet.Source = LogoUrlToBitmapImage(selectedUser.avatarURL);
                            ItemIDGet.Text = selectedUser.id.ToString();
                            UserLoginGet.Text = selectedUser.login;
                            UserPasswordGet.Text = selectedUser.passwordHash;
                            UserNicknameGet.Text = selectedUser.nickame;
                            UserEmailGet.Text = selectedUser.email;
                            UserCreationDateGet.Text = selectedUser.creationDate.ToString();
                        }
                        else
                        {
                            Logo.Source = LogoUrlToBitmapImage(selectedUser.avatarURL);
                            UserEmail.Text = selectedUser.email;
                            UserLogin.Text = selectedUser.login;
                            UserNickname.Text = selectedUser.nickame;
                            UserPassword.Text = selectedUser.passwordHash;
                            ItemID.Text = selectedUser.id.ToString();
                        }
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
