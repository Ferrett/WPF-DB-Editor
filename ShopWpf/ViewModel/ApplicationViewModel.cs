using Microsoft.Win32;
using MvvmHelpers;
using ShopWpf.Logic;
using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShopWpf.ViewModel
{
    public class ApplicationViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<Developer> _Developers;

        private dynamic? _selectedItem;
        private dynamic? _newItem;
        private Visibility _dataGridVisibility;
        private string _getRequestMessage;
        private Visibility _getRequestMessageVisibility;
        private Visibility _itemMenuVisibility;
        private string _putPostRequestMessage;
        private Visibility _putPostRequestMessageVisibility;
        private TabItem _selectedTabItem;
        private BitmapImage _openedImage;
        private bool _postOptionSelected;

        private string SelectedTable;
        private int SelectedItemID;

        #region Properties
        public ObservableCollection<Developer> Developers
        {
            get { return _Developers; }
            set
            {
                _Developers = value;
                OnPropertyChanged("Developers");
            }
        }
        public dynamic? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                SelectedItemID = _selectedItem?? GetSelectedItemID(_selectedItem);

                OnPropertyChanged("SelectedItem");
            }
        }
        public dynamic? NewItem
        {
            get { return _newItem; }
            set
            {
                _newItem = value;
                OnPropertyChanged("NewItem");
            }
        }
        public Visibility DataGridVisibility
        {
            get { return _dataGridVisibility; }
            set
            {
                _dataGridVisibility = value;
                OnPropertyChanged("DataGridVisibility");
            }
        }
        public string GetRequestMessage
        {
            get { return _getRequestMessage; }
            set
            {
                _getRequestMessage = value;
                OnPropertyChanged("GetRequestMessage");
            }
        }
        public Visibility GetRequestMessageVisibility
        {
            get { return _getRequestMessageVisibility; }
            set
            {
                _getRequestMessageVisibility = value;
                OnPropertyChanged("GetRequestMessageVisibility");
            }
        }
        public Visibility ItemMenuVisibility
        {
            get { return _itemMenuVisibility; }
            set
            {
                _itemMenuVisibility = value;
                OnPropertyChanged("ItemMenuVisibility");
            }
        }
        public string PutPostRequestMessage
        {
            get { return _putPostRequestMessage; }
            set
            {
                _putPostRequestMessage = value;
                OnPropertyChanged("PutPostRequestMessage");
            }
        }
        public Visibility PutPostRequestMessageVisibility
        {
            get { return _putPostRequestMessageVisibility; }
            set
            {
                _putPostRequestMessageVisibility = value;
                OnPropertyChanged("PutPostRequestMessageVisibility");
            }
        }
        public TabItem SelectedTabItem
        {
            get { return _selectedTabItem; }
            set
            {
                _selectedTabItem = value;
                SelectedTable = _selectedTabItem.Tag.ToString()!;
                OnPropertyChanged("SelectedTabItem");
            }
        }
        public BitmapImage OpenedImage
        {
            get { return _openedImage; }
            set
            {
                _openedImage = value;
                OnPropertyChanged("OpenedImage");
            }
        }
        public bool PostOptionSelected
        {
            get { return _postOptionSelected; }
            set
            {
                _postOptionSelected = value;
                OnPropertyChanged("PostOptionSelected");
            }
        }
        #endregion

        public ApplicationViewModel()
        {
            Init();
            HideTable();
            GetTable();
        }

        public int GetSelectedItemID(dynamic item)
        {
            switch (SelectedTable)
            {
                case TableNames.Developer:
                    {
                        return (SelectedItem as Developer).id;
                    }
                case TableNames.Game:
                    {
                        return (SelectedItem as Game).id;
                    }
                case TableNames.GameStats:
                    {
                        return (SelectedItem as GameStats).id;
                    }
                case TableNames.Review:
                    {
                        return (SelectedItem as Review).id;
                    }
                case TableNames.User:
                    {
                        return (SelectedItem as User).id;
                    }
                default:
                    return -1;
            }
        }

        public void Init()
        {
            ItemMenuVisibility = Visibility.Collapsed;
            PutPostRequestMessageVisibility = Visibility.Collapsed;
            NewItem = new Developer();
        }

        public async void GetTable()
        {
            HttpResponseMessage HttpResponse = await Requests.GetRequest(TableNames.Developer);
            Developers = new ObservableCollection<Developer>();

            if (HttpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                DataGridVisibility = Visibility.Visible;
                GetRequestMessage = ($"Error: {(int)HttpResponse.StatusCode} ({HttpResponse.StatusCode})\n{await HttpResponse.Content.ReadAsStringAsync()}");
                return;
            }

            List<Developer> list = new List<Developer>();
            list.AddRange(JsonSerializer.Deserialize<List<Developer>>(HttpResponse.Content.ReadAsStringAsync().Result)!);

            foreach (var item in list)
            {
                Developers.Add(item);
            }

            ShowTable();
        }

        private void HideTable()
        {
            DataGridVisibility = Visibility.Collapsed;
            GetRequestMessageVisibility = Visibility.Visible;
            GetRequestMessage = "Loading...";
        }

        private void ShowTable()
        {
            GetRequestMessageVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
        }

        public async Task DeleteSelectedItem()
        {
            await Requests.DeleteRequest(SelectedTable, SelectedItemID);
        }

        public async Task PostNewItem()
        {
            MultipartFormDataContent? multipartContent = null;
            string content = string.Empty;
            HttpResponseMessage response;

            if (OpenedImage != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(OpenedImage)), "logo");
            }

            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        Developer developer = NewItem as Developer;
                        content = $"{developer.name}";
                        break;
                    }
                //case TableNames.Game:
                //    {
                //        content = $"{GameName.Text}/{GamePrice.Text}/{GameDevID.Text}";
                //        content += GameAchCount.Text != string.Empty ? $"?achCount={GameAchCount.Text}" : string.Empty;
                //        break;
                //    }
                //case TableNames.GameStats:
                //    {
                //        content = $"{GameStatsUserID.Text}/{GameStatsGameID.Text}";
                //        break;
                //    }
                //case TableNames.Review:
                //    {
                //        content = $"{ReviewIsPositive.IsChecked}/{ReviewUserID.Text}/{ReviewGameID.Text}";
                //        content += ReviewText.Text != string.Empty ? $"?text={ReviewText.Text}" : string.Empty;
                //        break;
                //    }
                //case TableNames.User:
                //    {
                //        content = $"{UserLogin.Text}/{UserPassword.Text}/{UserNickname.Text}";
                //        content += UserEmail.Text != string.Empty ? $"?email={UserEmail.Text}" : string.Empty;
                //        break;
                //    }
                default:
                    break;
            }

            response = await Requests.PostRequest(SelectedTable, content, multipartContent);

            ShowRequestLog(response.StatusCode == HttpStatusCode.OK ? "Data Posted successfuly" :
                            $"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");
        }

        public async Task UpdateSelectedItem()
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            MultipartFormDataContent? multipartContent = null;

            HttpResponseMessage requestResponse;
            string responseMessage = string.Empty;


            //if (Logo.Source.ToString().Split('/').Last() != Routes.DefaultLogoName)
            //{
            //    multipartContent = new MultipartFormDataContent();
            //    multipartContent.Add(new ByteArrayContent(ImageToHttpContent(Logo)), "logo");
            //    content.Add(Routes.PutLogoRequest, string.Empty);
            //}

            //switch (SelectedTabItem.Tag.ToString())
            //{
            //    case TableNames.Developer:
            //        {
            //            Developer selectedDev = (Table[DataGrid.SelectedIndex] as Developer)!;

            //            if (DeveloperName.Text != selectedDev.name)
            //                content.Add(Routes.PutNameRequest, DeveloperName.Text);
            //            break;
            //        }
            //    case TableNames.Game:
            //        {
            //            Game selectedGame = (Table[DataGrid.SelectedIndex] as Game)!;

            //            if (GameName.Text != selectedGame.name)
            //                content.Add(Routes.PutNameRequest, GameName.Text);
            //            if (GamePrice.Text != selectedGame.price.ToString())
            //                content.Add(Routes.PutPriceRequest, GamePrice.Text);
            //            if (GameAchCount.Text != selectedGame.achievementsCount.ToString())
            //                content.Add(Routes.PutAchievementsCountRequest, GameAchCount.Text);
            //            break;
            //        }
            //    case TableNames.GameStats:
            //        {
            //            GameStats selectedGameStats = (Table[DataGrid.SelectedIndex] as GameStats)!;

            //            if (GameStatsIsGameLaunched.IsChecked == true)
            //                content.Add(Routes.PutGameLaunchedRequest, string.Empty);
            //            if (GameStatsGottenAchievements.Text != selectedGameStats.achievementsGot.ToString())
            //                content.Add(Routes.PutGottenAchievementsRequest, GameStatsGottenAchievements.Text);
            //            if (GameStatsHoursPlayed.Text != selectedGameStats.hoursPlayed.ToString())
            //                content.Add(Routes.PutHoursPlayedRequest, GameStatsHoursPlayed.Text);
            //            break;
            //        }
            //    case TableNames.Review:
            //        {
            //            Review selectedReview = (Table[DataGrid.SelectedIndex] as Review)!;

            //            if (ReviewIsPositive.IsChecked != selectedReview.isPositive)
            //                content.Add(Routes.PutGameLaunchedRequest, ReviewIsPositive.IsChecked.ToString()!);
            //            if (ReviewText.Text != selectedReview.text)
            //                content.Add(Routes.PutTextRequest, ReviewText.Text);
            //            break;
            //        }
            //    case TableNames.User:
            //        {
            //            User selectedUser = (Table[DataGrid.SelectedIndex] as User)!;

            //            if (UserEmail.Text != selectedUser.email)
            //                content.Add(Routes.PutEmailRequest, UserEmail.Text);
            //            if (UserNickname.Text != selectedUser.nickame)
            //                content.Add(Routes.PutNicknameRequest, UserNickname.Text);
            //            if (UserLogin.Text != selectedUser.login)
            //                content.Add(Routes.PutNicknameRequest, UserLogin.Text);
            //            if (UserPassword.Text != selectedUser.passwordHash)
            //                content.Add(Routes.PutNicknameRequest, UserPassword.Text);

            //            break;
            //        }
            //    default:
            //        break;
            //}

            //for (int i = 0; i < content.Count; i++)
            //{
            //    if (content.ElementAt(i).Key == Routes.PutLogoRequest)
            //        requestResponse = await Requests.PutRequest(selectedTableName, content.ElementAt(i).Key, DataGridSelectedID(), null, multipartContent);
            //    else
            //        requestResponse = await Requests.PutRequest(selectedTableName, content.ElementAt(i).Key, DataGridSelectedID(), content.ElementAt(i).Value);

            //    if (requestResponse.StatusCode != HttpStatusCode.OK)
            //    {
            //        responseMessage += $"Error: {(int)requestResponse.StatusCode} ({requestResponse.StatusCode})\n{await requestResponse.Content.ReadAsStringAsync()}\n\n";
            //    }
            //}

            if (responseMessage == string.Empty)
            {
                ShowRequestLog("Data Updated successfuly");
            }
            else
                ShowRequestLog(responseMessage);



        }

        public byte[] ImageToHttpContent(BitmapImage img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        private void ShowRequestLog(string errorText)
        {
            PutPostRequestMessageVisibility = Visibility.Visible;
            PutPostRequestMessage = errorText;
        }

        #region Commands
        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await DeleteSelectedItem();
                      GetTable();

                  }));
            }
        }

        private RelayCommand postCommand;
        public RelayCommand PostCommand
        {
            get
            {
                return postCommand ??
                  (postCommand = new RelayCommand(obj =>
                  {
                      PostOptionSelected = true;
                      ItemMenuVisibility = Visibility.Visible;
                  }));
            }
        }

        private RelayCommand putCommand;
        public RelayCommand PutCommand
        {
            get
            {
                return putCommand ??
                  (putCommand = new RelayCommand(obj =>
                  {
                      PostOptionSelected = false;
                      ItemMenuVisibility = Visibility.Visible;
                  }));
            }
        }

        private RelayCommand refreshCommand;
        public RelayCommand RefreshCommand
        {
            get
            {
                return refreshCommand ??
                  (refreshCommand = new RelayCommand(obj =>
                  {
                      HideTable();
                      GetTable();
                  }));
            }
        }

        private RelayCommand closeItemMenuCommand;
        public RelayCommand CloseItemMenuCommand
        {
            get
            {
                return closeItemMenuCommand ??
                  (closeItemMenuCommand = new RelayCommand(obj =>
                  {
                      ItemMenuVisibility = Visibility.Collapsed;
                  }));
            }
        }

        private RelayCommand postItemCommand;
        public RelayCommand PostItemCommand
        {
            get
            {
                return postItemCommand ??
                  (postItemCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await PostNewItem();
                      GetTable();
                  }));
            }
        }

        private RelayCommand updateItemCommand;
        public RelayCommand UpdateItemCommand
        {
            get
            {
                return updateItemCommand ??
                  (updateItemCommand = new RelayCommand(async obj =>
                  {
                      HideTable();
                      await UpdateSelectedItem();
                      GetTable();
                  }));
            }
        }

        private RelayCommand openImageFromFileCommand;
        public RelayCommand OpenImageFromFileCommand
        {
            get
            {
                return openImageFromFileCommand ??
                  (openImageFromFileCommand = new RelayCommand(async obj =>
                  {
                      OpenFileDialog openFileDialogLoad = new OpenFileDialog();

                      if (openFileDialogLoad.ShowDialog() == true)
                      {
                          OpenedImage = new BitmapImage(new Uri(openFileDialogLoad.FileName));
                      }
                  }));
            }
        }



        private RelayCommand tabChangedCommand;
        public RelayCommand TabChangedCommand
        {
            get
            {
                return tabChangedCommand ??
                  (tabChangedCommand = new RelayCommand(async obj =>
                  {

                  }));
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableRangeCollection<Developer> editedOrRemovedItems = new ObservableRangeCollection<Developer>();
            foreach (Developer newItem in e.NewItems)
            {
                editedOrRemovedItems.Add(newItem);
            }

            foreach (Developer oldItem in e.OldItems)
            {
                editedOrRemovedItems.Add(oldItem);
            }

            NotifyCollectionChangedAction action = e.Action;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
