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

namespace ShopWpf.ViewModel
{
    public class DeveloperViewModel : INotifyCollectionChanged, INotifyPropertyChanged
    {
        private ObservableCollection<Developer> _Developers;
        public ObservableCollection<Developer> Developers
        {
            get { return _Developers; }
            set
            {
                _Developers = value;
                OnPropertyChanged("Developers");
            }
        }

        public DeveloperViewModel()
        {
            Developers = new ObservableCollection<Developer>();

            Ass();

        }

        public async void Ass()
        {
            HttpResponseMessage response = await Requests.GetRequest("Developer");

            Developers.Add(JsonSerializer.Deserialize<List<Developer>>(response.Content.ReadAsStringAsync().Result).FirstOrDefault()!);
        }


        public event NotifyCollectionChangedEventHandler CollectionChanged;

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ObservableCollection<Developer> editedOrRemovedItems = new ObservableCollection<Developer>();
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
    }
}
