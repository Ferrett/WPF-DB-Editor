using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ShopWpf.Logic
{
    public static class Tools
    {
        public static byte[] ImageToHttpContent(Image img)
        {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(img.Source as BitmapImage));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                data = ms.ToArray();
            }

            return data;
        }

        public static int DataGridSelectedID(DataGrid grid)
        {
            if (grid.SelectedIndex == -1)
                return -1;

            string a = grid.SelectedValue.ToString()!;
            a = a.Substring(a.IndexOf("=") + 1);
            a = a.Substring(0, a.IndexOf(","));
            return int.Parse(a);
        }
    }
}
