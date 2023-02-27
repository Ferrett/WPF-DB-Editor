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
            NewItem = new Developer();
            HideTable();
            GetTable();
        }

        public async void GetTable()
        {
            HttpResponseMessage HttpResponse = await Requests.GetRequest(TableNames.Developer);
            Developers = new ObservableCollection<Developer>();

            if (HttpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
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
            //var a = PostOptionSelected;
            DataGridVisibility = Visibility.Collapsed;
            GetRequestMessageVisibility = Visibility.Visible;
            GetRequestMessage = "Loading...";
        }

        private void ShowTable()
        {
            GetRequestMessageVisibility = Visibility.Collapsed;
            DataGridVisibility = Visibility.Visible;
        }
        public async void DeleteFromTable(Developer developer)
        {
            HttpResponseMessage responseMessage = await Requests.DeleteRequest(TableNames.Developer, developer.id);
        }

        public async Task PostNewItem()
        {
            MultipartFormDataContent? multipartContent = null;
            string content = string.Empty;
            HttpResponseMessage response = new HttpResponseMessage();

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
            response = await Requests.PostRequest(SelectedTabItem.Tag.ToString(), content, multipartContent);

            ShowRequestLog(response.StatusCode == HttpStatusCode.OK ? "Data Posted successfuly" :
                            $"Error: {(int)response.StatusCode} ({response.StatusCode})\n{await response.Content.ReadAsStringAsync()}");
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
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      HideTable();
                      DeleteFromTable(SelectedItem);
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
                      //Developers.Add(new Developer { id = 1888, name = "fee", logoURL = "logo", registrationDate = DateTime.UtcNow });
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
                      //Developers.Add(new Developer { id = 1888, name = "fee", logoURL = "logo", registrationDate = DateTime.UtcNow });
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

        private RelayCommand updateItemCommand;
        public RelayCommand UpdateItemCommand
        {
            get
            {
                return updateItemCommand ??
                  (updateItemCommand = new RelayCommand(async obj =>
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
