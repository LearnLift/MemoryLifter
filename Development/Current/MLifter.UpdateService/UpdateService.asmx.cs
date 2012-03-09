using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.IO;

namespace MLifterUpdateService
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://www.memorylifter.com/UpdateService/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class UpdateService : System.Web.Services.WebService
    {
        [WebMethod]
        public byte[] ServeLatestMLVersion(string version, bool onstick)
        {
            string mlPath;
            if (onstick)
                mlPath = Server.MapPath("~/App_Data/onstick/" + version + "/MLifter.Updater.dll");
            else
                mlPath = Server.MapPath("~/App_Data/" + version + "/MLifter.Updater.dll");

            FileStream mlStream = new FileStream(mlPath, FileMode.Open, FileAccess.Read);
            int len = (int)mlStream.Length;
            byte[] ML = new byte[len];
            mlStream.Read(ML, 0, len);
            mlStream.Close();

            return ML;
        }
    }
}
