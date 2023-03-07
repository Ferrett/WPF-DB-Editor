using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ShopWpf.Models
{
    [Serializable]
    public record User : INotifyPropertyChanged
    {
        private int _id { get; set; }
        private string _login { get; set; } = null!;
        private string _passwordHash { get; set; } = null!;
        private string _nickname { get; set; } = null!;
        private string _avatarURL { get; set; } = null!;
        private string? _email { get; set; }
        private DateTime _creationDate { get; set; }

        public int id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("id");
            }
        }
        public string login
        {
            get { return _login; }
            set
            {
                _login = value;
                OnPropertyChanged("login");
            }
        }
        public string passwordHash
        {
            get { return _passwordHash; }
            set
            {
                _passwordHash = value;
                OnPropertyChanged("passwordHash");
            }
        }
        public string nickname
        {
            get { return _nickname; }
            set
            {
                _nickname = value;
                OnPropertyChanged("nickname");
            }
        }
        public string avatarURL
        {
            get { return _avatarURL; }
            set
            {
                _avatarURL = value;
                OnPropertyChanged("avatarURL");
            }
        }
        public string? email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged("email");
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