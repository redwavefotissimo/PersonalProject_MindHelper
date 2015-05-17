using System;
using System.Linq;
using System.Collections.Generic;

namespace MindHelper
{
    public class SystemInfo
    {
        public static List<string> GetDiskDrive()
        {
            return System.IO.DriveInfo.GetDrives().Where(Drive => Drive.IsReady == true && Drive.DriveType == System.IO.DriveType.Fixed).Select(Drive => Drive.Name).ToList<string>();
        }
    }
}
