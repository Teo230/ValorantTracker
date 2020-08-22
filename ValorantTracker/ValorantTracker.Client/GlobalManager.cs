using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Valorant.Rest.API.ModelDTO;
using Valorant.Rest.API.Utilities;
using ValorantTracker.Client.Models;
using ValorantTracker.Client.Properties;
using ValorantTracker.Client.Utilities;
using static ValorantTracker.Client.Utilities.GlobalEnum;

namespace ValorantTracker.Client
{
    public class GlobalManager
    {
        #region Prop
        public static string BearerToken { get; set; }
        public static string X_Riot_Entitlements_JWT { get; set; }
        public static string clientVersion { get; set; } = "release-01.05-shipping-15-460559";
        public static PlayerDTO Player { get; set; }
        public static DateTime? ExpiresDateTime { get; set; }
        public static UserSettings settings { get; set; }
        public static EndpointsEnum Region { get; set; }
        #endregion

        #region Methods
        public static ValorantEnum.RegionEnum GetRightEndpoint(EndpointsEnum region)
        {
            switch(region)
            {
                case EndpointsEnum.Europe:
                    return ValorantEnum.RegionEnum.Europe;
                case EndpointsEnum.NorthAmerica:
                    return ValorantEnum.RegionEnum.NorthAmerica;
                case EndpointsEnum.Asia:
                    return ValorantEnum.RegionEnum.Asia;
                case EndpointsEnum.Korea:
                    return ValorantEnum.RegionEnum.Korea;
            }

            return ValorantEnum.RegionEnum.Europe;
        }

        public static void SaveUserInfo()
        {
            string TempPath = Path.GetTempPath();
            string FileName = "ValorantTrackerSettings";
            string FolderName = Application.Current.GetType().Assembly.GetName().Name + @"\AppSettings";
            string _fullPath = Path.Combine(TempPath, FolderName, FileName);

            var serializedObject = JsonConvert.SerializeObject(settings);
            var folderPath = Path.Combine(TempPath, FolderName);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            _fullPath = Path.Combine(folderPath, FileName);
            if (!File.Exists(_fullPath))
            {
                File.Create(_fullPath).Close();
            }

            File.WriteAllText(_fullPath, Crypter.Encrypt(serializedObject));

        }

        public static UserSettings GetSettingObject()
        {
            string TempPath = Path.GetTempPath();
            string FileName = "ValorantTrackerSettings";
            string FolderName = Application.Current.GetType().Assembly.GetName().Name + @"\AppSettings";
            string _fullPath = Path.Combine(TempPath, FolderName, FileName);

            UserSettings settings = default;
            if (File.Exists(_fullPath))
            {
                string cryptedText = File.ReadAllText(_fullPath);
                var jsonData = Crypter.Decrypt(cryptedText);
                if (!string.IsNullOrEmpty(jsonData))
                {
                    settings = JsonConvert.DeserializeObject<UserSettings>(jsonData);
                }
            }
            return settings;
        }
        #endregion

        #region Ctr
        public GlobalManager()
        {
        }
        #endregion
    }
}
