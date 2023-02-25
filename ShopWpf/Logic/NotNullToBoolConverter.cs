using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ShopWpf.Logic
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class NotNullToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter,CultureInfo culture)
        {
            return value==null?false:true;
        }

        public object ConvertBack(object value, Type targetType, object parameter,CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
