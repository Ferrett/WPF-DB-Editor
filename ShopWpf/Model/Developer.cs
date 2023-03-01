using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ShopWpf.Models
{
    [Serializable]
    public record Developer : INotifyPropertyChanged
    {
        private int _id { get; set; }
        private string _name { get; set; } = null!;
        private string _logoURL { get; set; } = null!;
        private DateTime _registrationDate { get; set; }

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }
        public string name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("name");
            }
        }
        public string logoURL
        {
            get { return _logoURL; }
            set
            {
                _logoURL = value;
                OnPropertyChanged("logoURL");
            }
        }
        public DateTime registrationDate
        {
            get { return _registrationDate; }
            set
            {
                _registrationDate = value;
                OnPropertyChanged("registrationDate");
            }
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
