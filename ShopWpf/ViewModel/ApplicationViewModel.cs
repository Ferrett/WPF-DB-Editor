using MvvmHelpers;
using ShopWpf.Logic;
using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ShopWpf.ViewModel
{
    public class ApplicationViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableRangeCollection<Developer> _Developers;
        private dynamic? _selectedItem;
        private Visibility _dataGridVisibility;
        private string _errorText;
        private Visibility _errorTextVisibility;

        #region Properties
        public ObservableRangeCollection<Developer> Developers
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

        public Visibility DataGridVisibility
        {
            get { return _dataGridVisibility; }
            set
            {
                _dataGridVisibility = value;
                OnPropertyChanged("DataGridVisibility");
            }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set
            {
                _errorText = value;
                OnPropertyChanged("ErrorText");
            }
        }

        public Visibility ErrorTextVisibility
        {
            get { return _errorTextVisibility; }
            set
            {
                _errorTextVisibility = value;
                OnPropertyChanged("ErrorTextVisibility");
            }
        }
        #endregion

        public ApplicationViewModel()
        {
            GetDevelopersFromDB();
        }

        public async void GetDevelopersFromDB()
        {
            DataGridVisibility = Visibility.Collapsed;
            ErrorTextVisibility = Visibility.Visible;
            ErrorText = "Loading...";

            HttpResponseMessage HttpResponse = await Requests.GetRequest(TableNames.Developer);
            Developers = new ObservableRangeCollection<Developer>();

            if (HttpResponse.StatusCode != System.Net.HttpStatusCode.OK)
            {
                ErrorText = ($"Error: {(int)HttpResponse.StatusCode} ({HttpResponse.StatusCode})\n{await HttpResponse.Content.ReadAsStringAsync()}");
                DataGridVisibility = Visibility.Visible;
                return;
            }
            Developers.AddRange(JsonSerializer.Deserialize<List<Developer>>(HttpResponse.Content.ReadAsStringAsync().Result)!);

            DataGridVisibility = Visibility.Visible;
            ErrorTextVisibility = Visibility.Collapsed;
        }

        public async void DeleteDeveloperFromDB(Developer developer)
        {
            HttpResponseMessage responseMessage = await Requests.DeleteRequest(TableNames.Developer, developer.id);
            GetDevelopersFromDB();
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
                      DeleteDeveloperFromDB(SelectedItem);
                  }));
            }
        }

        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      Developers.Add(new Developer { id = 1888, name = "fee", logoURL = "logo", registrationDate = DateTime.UtcNow });
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
                      GetDevelopersFromDB();
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
