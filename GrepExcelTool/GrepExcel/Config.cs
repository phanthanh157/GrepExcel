using System;
using System.Collections.Generic;
using System.Configuration;

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

        private static ConfigTable[] configs = new ConfigTable[]
        {
           //new ConfigTable("PATH_SPEC_INTERNAL",Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)),
           new ConfigTable("VERSION","0.0.1"),
           new ConfigTable("MAX_FILE","100"),
           new ConfigTable("MAX_FOLDER","100"),
           new ConfigTable("MAX_SEARCH","1000"),
           new ConfigTable("TAB_CURRENT_ACTIVE","0"),
           new ConfigTable("NUMBER_RECENTS","10"),
           new ConfigTable("COLUMNS_HIDE","Path:3:1,Sheet:4:1,Cell:5:0")
        };

        public List<ConfigTable> ConfigTables { get => _configTables; set => _configTables = value; }

        public Config()
        {

        }

        public void Load()
        {
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
                    }
                }
            }
            catch (ConfigurationErrorsException)
            {

            }
        }

        public static string ReadSetting(string key)
        {
            string result = string.Empty;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
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

            }
        }
    }
}
