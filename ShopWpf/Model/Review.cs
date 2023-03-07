using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShopWpf.Models
{
    [Serializable]
    public record Review : INotifyPropertyChanged
    {
        private int _id { get; set; }
        private string? _text { get; set; }
        private bool _isPositive { get; set; }
        private DateTime _creationDate { get; set; }
        private DateTime _lastEditDate { get; set; }
        private int _gameID { get; set; }
        private int _userID { get; set; }

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }
        public string? text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("text");
            }
        }
        public bool isPositive
        {
            get { return _isPositive; }
            set
            {
                _isPositive = value;
                OnPropertyChanged("isPositive");
            }
        }
        public DateTime creationDate
        {
            get { return _creationDate; }
            set
            {
                _creationDate = value;
                OnPropertyChanged("creationDate");
            }
        }
        public DateTime lastEditDate
        {
            get { return _lastEditDate; }
            set
            {
                _lastEditDate = value;
                OnPropertyChanged("lastEditDate");
            }
        }
        public int gameID
        {
            get { return _gameID; }
            set
            {
                _gameID = value;
                OnPropertyChanged("gameID");
            }
        }
        public int userID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                OnPropertyChanged("userID");
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
