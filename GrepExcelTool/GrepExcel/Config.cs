using System;
using System.Collections.Generic;
using System.Configuration;
using GrepExcel;

namespace GrepExcel
{
    public struct ConfigTable
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public ConfigTable(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }
    }
    public class Config
    {
        private static List<ConfigTable> _configTables = null;

        private static string _defaultDirectory;

        private static ConfigTable[] configs = new ConfigTable[]
        {
           new ConfigTable("PATH_SPEC_INTERNAL",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
           new ConfigTable("PATH_SPEC_EXTERNAL",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
           new ConfigTable("ENABLE_AUTOUPDATE_SPECIFICATION","True"),
           new ConfigTable("UDP_IP","127.0.0.1"),
           new ConfigTable("UDP_PORT","9095"),
           new ConfigTable("MODE_VIEW_XML","Tree")
        };

        public List<ConfigTable> ConfigTables { get => _configTables; set => _configTables = value; }

        public Config()
        {
            _defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            ConfigTables = new List<ConfigTable>();
            ReadAllSettings();

        }

        public static void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0)
                {
                    ShowDebug.Msg(F.FLMD(), "AppSettings is empty.");
                    ShowDebug.Msg(F.FLMD(), "AppSettings set loading...");
                    foreach (ConfigTable configTable in configs)
                    {
                        AddUpdateAppSettings(configTable.Key, configTable.Value);
                    }
                }
                else
                {
                    foreach (var key in appSettings.AllKeys)
                    {
                        _configTables.Add(new ConfigTable(key, appSettings[key]));
                        ShowDebug.Msg(F.FLMD(), "Key: {0} Value: {1}", key, appSettings[key]);
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {
                ShowDebug.Msg(F.FLMD(), "Error reading app settings");
            }
        }

        public static string ReadSetting(string key)
        {
            string result = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key];
                ShowDebug.Msg(F.FLMD(), "Read Setting Config: {0}", result);
            }
            catch (ConfigurationErrorsException)
            {
                ShowDebug.Msg(F.FLMD(), "Error reading app settings");
            }
            return result;
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                ShowDebug.Msg(F.FLMD(), "Error writing app settings");
            }
        }




    }
}
