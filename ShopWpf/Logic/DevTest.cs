using ShopWpf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ShopWpf.Logic
{
    public static class DevTest
    {
        public static string Result = string.Empty;
        public static async Task PutReq(DataGrid grid, string content,Image logo,string devName, string tableName)
        {
            Result = string.Empty;
            MultipartFormDataContent? multipartContent = null;

            List<HttpResponseMessage> list = new List<HttpResponseMessage>() {
                new HttpResponseMessage(),
                new HttpResponseMessage()
            };

            var a = list[0];
            if (logo.Source != null)
            {
                multipartContent = new MultipartFormDataContent();
                multipartContent.Add(new ByteArrayContent(Tools.ImageToHttpContent(logo)), "logo", "filename");
                
                list[0] = await Requests.PutRequest(tableName, Routes.PutLogoRequest, Tools.DataGridSelectedID(grid), null, multipartContent);
            }
            
                list[1] = await Requests.PutRequest(tableName, Routes.PutNameRequest, Tools.DataGridSelectedID(grid), content);
            

            if (list.Any(x => x.StatusCode != HttpStatusCode.OK))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].StatusCode != HttpStatusCode.OK)
                        Result += $"Error: {(int)list[i].StatusCode} ({list[i].StatusCode})   |   {await list[i].Content.ReadAsStringAsync()}";
                }
            }
            else
            {
                Result = "Data Updated successfuy";
            }

        }

        public static bool IsFieldEmpty(Developer dev,string newdevName)
        {
            if (String.IsNullOrEmpty(newdevName))
            {
                return true;
            }
            return false;
        }

        public static bool IsAnyChanges(Developer dev,Image logo, string newdevName)
        {
            if (logo.Source == null && newdevName == dev.name)
            {
                return true;
            }
            return false;
        }
    }
}
