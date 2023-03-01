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
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ShopWpf.ViewModel
{
    public class ApplicationViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<Developer> _Developers;
        private Developer _newDeveloper;

        private ObservableCollection<Game> _Games;
        private Game _newGame;

        private dynamic? _selectedItem;
        private dynamic? _updatedItem;

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
        public ObservableCollection<Game> Games
        {
            get { return _Games; }
            set
            {
                _Games = value;
                OnPropertyChanged("Games");
            }
        }
        public dynamic? SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;

                if (_selectedItem != null)
                    UpdatedItem = CopyFromReferenceType(_selectedItem);

                OnPropertyChanged("SelectedItem");
            }
        }
        public Developer NewDeveloper
        {
            get { return _newDeveloper; }
            set
            {
                _newDeveloper = value;
                OnPropertyChanged("NewItem");
            }
        }
        public Game NewGame
        {
            get { return _newGame; }
            set
            {
                _newGame = value;
                OnPropertyChanged("NewGame");
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
        public dynamic? UpdatedItem
        {
            get { return _updatedItem; }
            set
            {
                _updatedItem = value;

                OnPropertyChanged("UpdatedItem");
            }
        }
        #endregion


        public ApplicationViewModel()
        {
            Init();
            HideTable();
            GetTable(TableNames.Developer);
        }

        private dynamic CopyFromReferenceType(dynamic selectedItem)
        {
            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        return (dynamic)new Developer
                        {
                            id = selectedItem.id,
                            logoURL = selectedItem.logoURL,
                            name = selectedItem.name,
                            registrationDate = selectedItem.registrationDate
                        };
                    }
                case TableNames.Game:
                    {
                        return (dynamic)new Game
                        {
                            id = selectedItem.id,
                            logoURL = selectedItem.logoURL,
                            name = selectedItem.name,
                            price = selectedItem.price,
                            developerID= selectedItem.developerID,
                            achievementsCount= selectedItem.achievementsCount,
                            publishDate= selectedItem.publishDate
                        };
                    }
                case TableNames.GameStats:
                    {
                        return (dynamic)new GameStats
                        {
                            id = selectedItem.id,
                            userID = selectedItem.userID,
                            gameID = selectedItem.gameID,
                            hoursPlayed = selectedItem.hoursPlayed,
                            achievementsGot = selectedItem.achievementsGot,
                            purchasehDate = selectedItem.purchasehDate,
                            lastLaunchDate = selectedItem.lastLaunchDate
                        };
                    }
                case TableNames.Review:
                    {
                        return (dynamic)new Review
                        {
                            id = selectedItem.id,
                            userID = selectedItem.userID,
                            gameID = selectedItem.gameID,
                            text = selectedItem.text,
                            isPositive = selectedItem.isPositive,
                            creationDate = selectedItem.creationDate,
                            lastEditDate = selectedItem.lastEditDate
                        };
                    }
                case TableNames.User:
                    {
                        return (dynamic)new User
                        {
                            id = selectedItem.id,
                            login = selectedItem.login,
                            avatarURL = selectedItem.avatarURL,
                            email = selectedItem.email,
                            passwordHash = selectedItem.passwordHash,
                            creationDate = selectedItem.creationDate,
                            nickame = selectedItem.nickame
                        };
                    }
                default:
                    return null;
            }
           
        }

        public void Init()
        {
            ItemMenuVisibility = Visibility.Collapsed;
            PutPostRequestMessageVisibility = Visibility.Collapsed;
            NewDeveloper = new Developer();
            NewGame = new Game();
        }

        public async void GetTable(string a = null)
        {
            HttpResponseMessage HttpResponse = await Requests.GetRequest(a??SelectedTable);

            if (HttpResponse.StatusCode != HttpStatusCode.OK)
            {
                DataGridVisibility = Visibility.Visible;
                GetRequestMessageVisibility = Visibility.Visible;
                GetRequestMessage = ($"Error: {(int)HttpResponse.StatusCode} ({HttpResponse.StatusCode})\n{await HttpResponse.Content.ReadAsStringAsync()}");
                return;
            }

            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        List<Developer> tmpList = new List<Developer>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<Developer>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Developers = new ObservableCollection<Developer>();
                        foreach (var item in tmpList)
                        {
                            Developers.Add(item);
                        }
                        break;
                    }
                case TableNames.Game:
                    {
                        List<Game> tmpList = new List<Game>();
                        tmpList.AddRange(JsonSerializer.Deserialize<List<Game>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        Games = new ObservableCollection<Game>();
                        foreach (var item in tmpList)
                        {
                            Games.Add(item);
                        }
                        break;
                    }
                case TableNames.GameStats:
                    {
                        //List<GameStats> tmpList = new List<GameStats>();
                        //tmpList.AddRange(JsonSerializer.Deserialize<List<GameStats>>(HttpResponse.Content.ReadAsStringAsync().Result)!);
                        //Games = new ObservableCollection<GameStats>();
                        //foreach (var item in tmpList)
                        //{
                        //    Games.Add(item);
                        //}
                        break;
                    }
                case TableNames.Review:
                    {
                        break;
                    }
                case TableNames.User:
                    {
                        break;
                    }
                default:
                    break;
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
            await Requests.DeleteRequest(SelectedTable, SelectedItem.id);
        }

        public async Task PostNewItem()
        {
            MultipartFormDataContent? multipartContent = null;
            string content = string.Empty;
            HttpResponseMessage response;

            if (OpenedImage != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(OpenedImage)), "logo", "filename");
            }

            switch (SelectedTabItem.Tag)
            {
                case TableNames.Developer:
                    {
                        content = Convert.ToString(NewDeveloper.name);
                        break;
                    }
                case TableNames.Game:
                    {
                        content = $"{NewGame.name}/{NewGame.price}/{NewGame.developerID}";
                        content += NewGame.achievementsCount != null ? $"?achCount={NewGame.achievementsCount}" : null;
                        break;
                    }
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


            if (OpenedImage != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(ImageToHttpContent(OpenedImage)), "logo", "filename");
                content.Add(Routes.PutLogoRequest, string.Empty);
            }

            switch (SelectedTabItem.Tag.ToString())
            {
                case TableNames.Developer:
                    {
                        if (SelectedItem.name != UpdatedItem.name)
                            content.Add(Routes.PutNameRequest, UpdatedItem.name);
                        break;
                    }
                case TableNames.Game:
                    {
                        if (SelectedItem.name != UpdatedItem.name)
                            content.Add(Routes.PutNameRequest, UpdatedItem.name);
                        if (SelectedItem.price != UpdatedItem.price.ToString())
                            content.Add(Routes.PutPriceRequest, UpdatedItem.price);
                        if (SelectedItem.achievementsCount != UpdatedItem.achievementsCount.ToString())
                            content.Add(Routes.PutAchievementsCountRequest, UpdatedItem.achievementsCount);
                        if (SelectedItem.developerID != UpdatedItem.developerID.ToString())
                            content.Add(Routes.PutDeveloperRequest, UpdatedItem.developerID.ToString());
                        break;
                    }
                //case TableNames.GameStats:
                //    {
                //        GameStats selectedGameStats = (Table[DataGrid.SelectedIndex] as GameStats)!;

                //        if (GameStatsIsGameLaunched.IsChecked == true)
                //            content.Add(Routes.PutGameLaunchedRequest, string.Empty);
                //        if (GameStatsGottenAchievements.Text != selectedGameStats.achievementsGot.ToString())
                //            content.Add(Routes.PutGottenAchievementsRequest, GameStatsGottenAchievements.Text);
                //        if (GameStatsHoursPlayed.Text != selectedGameStats.hoursPlayed.ToString())
                //            content.Add(Routes.PutHoursPlayedRequest, GameStatsHoursPlayed.Text);
                //        break;
                //    }
                //case TableNames.Review:
                //    {
                //        Review selectedReview = (Table[DataGrid.SelectedIndex] as Review)!;

                //        if (ReviewIsPositive.IsChecked != selectedReview.isPositive)
                //            content.Add(Routes.PutGameLaunchedRequest, ReviewIsPositive.IsChecked.ToString()!);
                //        if (ReviewText.Text != selectedReview.text)
                //            content.Add(Routes.PutTextRequest, ReviewText.Text);
                //        break;
                //    }
                //case TableNames.User:
                //    {
                //        User selectedUser = (Table[DataGrid.SelectedIndex] as User)!;

                //        if (UserEmail.Text != selectedUser.email)
                //            content.Add(Routes.PutEmailRequest, UserEmail.Text);
                //        if (UserNickname.Text != selectedUser.nickame)
                //            content.Add(Routes.PutNicknameRequest, UserNickname.Text);
                //        if (UserLogin.Text != selectedUser.login)
                //            content.Add(Routes.PutNicknameRequest, UserLogin.Text);
                //        if (UserPassword.Text != selectedUser.passwordHash)
                //            content.Add(Routes.PutNicknameRequest, UserPassword.Text);

                //        break;
                //    }
                default:
                    break;
            }

            if (content.Count == 0)
            {
                ShowRequestLog("No changes");
                return;
            }

            for (int i = 0; i < content.Count; i++)
            {
                if (content.ElementAt(i).Key == Routes.PutLogoRequest)
                    requestResponse = await Requests.PutRequest(SelectedTable, content.ElementAt(i).Key, UpdatedItem.id, null, multipartContent);
                else
                    requestResponse = await Requests.PutRequest(SelectedTable, content.ElementAt(i).Key, UpdatedItem.id, content.ElementAt(i).Value);

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
                      GetRequestMessageVisibility = Visibility.Visible;
                      GetRequestMessage = "Loading...";
                      ItemMenuVisibility = Visibility.Collapsed;
                      
                      GetTable();
                      OpenedImage = null;
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
