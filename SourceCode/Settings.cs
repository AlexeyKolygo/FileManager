using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace FileManager
{
    public class Settings
    {
        public string DefaultDir = "C:\\";
        public string CurrentDir { get; set; }
        public int CurrentPage { get; set; }
        public int PageCounter { get; set; }

        public Settings() { }

        public Settings(string _deafultdir, string _currentdir, int _currentpage, int _page)
        {
            _deafultdir = this.DefaultDir;
            this.CurrentDir = _currentdir;
            this.CurrentPage = _currentpage;
            this.PageCounter = _page;
        }

        public void SettingsSetter()
        {
            try 
            { 
            Settings settings = new Settings(this.DefaultDir, this.CurrentDir, this.CurrentPage, this.PageCounter);
            string jsonsettings = JsonSerializer.Serialize(settings);
            File.WriteAllText("appsettings.json", jsonsettings);
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

    }
}
