using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShopWpf.Models
{
    [Serializable]
    public record GameStats : INotifyPropertyChanged
    {
        private int _id { get; set; }
        private int _userID { get; set; }
        private int _gameID { get; set; }
        private float _hoursPlayed { get; set; }
        private int _achievementsGot { get; set; }
        private DateTime _purchaseDate { get; set; }

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
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
        public int gameID
        {
            get { return _gameID; }
            set
            {
                _gameID = value;
                OnPropertyChanged("gameID");
            }
        }
        public float hoursPlayed
        {
            get { return _hoursPlayed; }
            set
            {
                _hoursPlayed = value;
                OnPropertyChanged("hoursPlayed");
            }
        }
        public int achievementsGot
        {
            get { return _achievementsGot; }
            set
            {
                _achievementsGot = value;
                OnPropertyChanged("achievementsGot");
            }
        }
        public DateTime purchaseDate
        {
            get { return _purchaseDate; }
            set
            {
                _purchaseDate = value;
                OnPropertyChanged("purchaseDate");
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
