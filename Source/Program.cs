using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace FileManager
{
    class Program
    {
        public static AppServiceReader appfilereader = new AppServiceReader();
        public static CommandsProccesor processor = new CommandsProccesor();
        public static Folders curdirinfo = new Folders();
        public static Settings settingsmodel = new Settings();
        public static int pagecounter;
        public static void Main()

        {
            appfilereader.AppFilesChk();
            var settings = settingsmodel.ReadSettings(settingsmodel);
            pagecounter = settings.PageCounter;
            string folder = String.IsNullOrEmpty(settings.CurrentDir) ? settings.DefaultDir : settings.CurrentDir;
            var startfolder = String.Empty;
            if (Directory.Exists(folder)) { startfolder = folder; }
            else 
            { 
                startfolder = settings.DefaultDir; 
                Console.WriteLine($"Directory is no longer exists.Application will start with default dir {settings.DefaultDir}");
                Console.ReadLine();
            }
            string[] objects = appfilereader.GetObjects(startfolder);
            ReadObjects(objects, startfolder, pagecounter, settings.CurrentPage);
        }
        public static void ReadObjects(string[] folders, string currentdir, int pagecounter, int p)
        {
            var folder_list = appfilereader.ChunkToPages(folders, pagecounter);
            var last_page = folder_list[folder_list.GetLength(0) - 1, 1];
            DirectoryInfo d = new DirectoryInfo(currentdir);
            while (true)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"YOU ARE HERE: {currentdir.ToUpper()}");
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                appfilereader.ReadPages(folder_list, folders.Length, p);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------------------------");
                Console.WriteLine("CURRENT DIRECTORY INFO");
                Console.WriteLine("---------------------------------");
                curdirinfo.Info(d);
                Console.WriteLine("---------------------------------");
                Console.WriteLine("PRESS ENTER TO INPUT COMMAND IN CONSOLE");
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.White;
                var input = Console.ReadKey();

                if (input.Key == ConsoleKey.RightArrow)
                {
                    var pagecontrol = p <= Convert.ToInt32(last_page) - 1 ? p++ : p;
                    Console.Clear();
                }

                if (input.Key == ConsoleKey.LeftArrow)
                {
                    var pagecontrol = p > 1 ? p-- : 1;
                    Console.Clear();
                }

                if (input.Key == ConsoleKey.Enter)
                {
                    var history = appfilereader.HistoryReader();
                    Console.Write("Command>: ");
                    var commandkey = Console.ReadKey(Console.KeyAvailable);
                    var command = String.Empty;
                    if (commandkey.Key == ConsoleKey.UpArrow || commandkey.Key == ConsoleKey.DownArrow)
                    {
                        command = appfilereader.ScrollCommand(history).Trim();
                    }
                    else
                    {
                        var bugreadkey = Console.ReadLine();//эта тварь все еще читает commandkey. и если сделать command = Console.ReadLine(), то он сожрет первую букву
                        command = commandkey.Key.ToString().ToLower() + bugreadkey;
                    }
                    if (!String.IsNullOrEmpty(command)) { File.AppendAllText("history.txt", command + ';' + Environment.NewLine); }
                    processor.ExecuteCommandParser(command, pagecounter, currentdir);
                }

                if (input.Key == ConsoleKey.Escape)
                {

                    var settingsset = new Settings("", currentdir, p, pagecounter);
                    settingsset.SettingsSetter();
                    Environment.Exit(0);

                }
                Console.Clear();
            }

        }
    }
}




