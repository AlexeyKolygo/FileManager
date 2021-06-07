using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace FileManager
{
    public class Settings
    {
        public string DefaultDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToString();
        public string CurrentDir { get; set; }
        public int CurrentPage { get; set; }
        public int PageCounter { get; set; }

        public Settings() { }

        public Settings(string _defaultdir, string _currentdir, int _currentpage, int _page)
        {
            _defaultdir = this.DefaultDir;
            this.CurrentDir = _currentdir;
            this.CurrentPage = _currentpage;
            this.PageCounter = _page;
        }

        public void SettingsSetter()
        {
            try
            {
                var usersettings = GetUserSettings();

                Settings settings = new Settings(this.DefaultDir, this.CurrentDir, this.CurrentPage, this.PageCounter);
                string jsonsettings = JsonSerializer.Serialize(settings);
                File.WriteAllText(usersettings, jsonsettings);
            }
            catch
            {
                Console.WriteLine("We couldn't append data into settings file. Please ensure that 'appsettings.json' exists");//тоже решил не логировать в файл. неудобно.
                Console.ReadLine();
            }
        }

        public void SetCommand(string com)
        {
            try
            {
                File.AppendAllText("history.txt", com + Environment.NewLine);
            }
            catch
            {
                Console.WriteLine("We couldn't append data into history file. Please ensure that 'history.txt' exists");//тоже решил не логировать в файл. неудобно.
                Console.ReadLine();
            }
        }

        public Settings ReadSettings(Settings currentsettings)
        {

            string usersettings = GetUserSettings();

            if (File.Exists(usersettings))
            {

                var reader = File.ReadAllText(usersettings);
                currentsettings = JsonSerializer.Deserialize<Settings>(reader);

                return currentsettings;
            }
            else
            {
                var newsettings = File.Create(usersettings);
                currentsettings.CurrentPage = 1;
                currentsettings.PageCounter = 10;
                newsettings.Dispose();
                string jsonsettings = JsonSerializer.Serialize(currentsettings);

                File.WriteAllText(usersettings, jsonsettings);
                return currentsettings;
            }
        }

        private string GetUserSettings()
        {
            var userfolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            string usersettings = Path.Combine(userfolder, "appsettings.json");
            return usersettings;

        }
        


    }
}
