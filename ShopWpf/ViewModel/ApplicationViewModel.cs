using ShopWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ShopWpf.ViewModel
{
    public class ApplicationViewModel
    {
        public ICommand ButtonCommand { get; private set; }

        public ApplicationViewModel()
        {
            ButtonCommand = new RelayCommand(o => MainButtonClick("MainButton"));
        }

        private void MainButtonClick(object sender)
        {
            
        }
    }
}
