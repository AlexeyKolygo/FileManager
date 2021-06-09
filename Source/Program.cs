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
        static void Main()

        {
            AppFilesChk();
            var settingsmodel = new Settings();
            var settings = settingsmodel.ReadSettings(settingsmodel);
            string folder = String.IsNullOrEmpty(settings.CurrentDir) ? settings.DefaultDir : settings.CurrentDir;
            if (Directory.Exists(folder))
            {
                string[] objects = GetObjects(folder);

                ReadObjects(objects, folder, settings.PageCounter, settings.CurrentPage);
            }
            else
            {
                Console.WriteLine($"Directory is no longer exists.Application will start with default dir {settings.DefaultDir}");
                Console.ReadLine();
                string[] objects = GetObjects(settings.DefaultDir);
                ReadObjects(objects, settings.DefaultDir, settings.PageCounter, settings.CurrentPage);
            }
        }
        public static void ReadObjects(string[] folders, string currentdir, int pagecounter, int p)
        {

            var folder_list = ChunkToPages(folders, pagecounter);
            var last_page = folder_list[folder_list.GetLength(0) - 1, 1];
            DirectoryInfo d = new DirectoryInfo(currentdir);
            var curdirinfo = new Folders();

            while (true)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("---------------------------------");
                Console.WriteLine($"YOU ARE HERE: {currentdir.ToUpper()}");
                Console.WriteLine("---------------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                ReadPages(folder_list, folders.Length, p);
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
                    var history = HistoryReader();
                    Console.Write("Command>: ");
                    var commandkey = Console.ReadKey();
                    var command = String.Empty;
                    if (commandkey.Key == ConsoleKey.UpArrow || commandkey.Key == ConsoleKey.DownArrow)
                    {
                        command = ScrollCommand(history).Trim();
                    }
                    else
                    {
                        command = Console.ReadLine();
                    }
                    if (!String.IsNullOrEmpty(command)) { File.AppendAllText("history.txt", command + ';' + Environment.NewLine); }
                    CommandParser(command, pagecounter, currentdir);


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
        static string ScrollCommand(List<string> history)
        {

            string result = String.Empty;

            var k = 0;
            while (true)
            {
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:

                        k = k - 1;
                        k = k < 0 ? 0 : k;
                        result = history[k];
                        Console.Write(result);
                        k = k - 1;
                        continue;
                    case ConsoleKey.DownArrow:

                        k = k + 1;
                        k = k >= history.Count ? history.Count - 1 : k;
                       result = history[k];
                       Console.Write(result);

                        continue;

                    case ConsoleKey.Enter:
                        return result;
                }
            }
        }
            static List<string> HistoryReader()
            {
                var history = new List<string>();

                var historyentries = File.ReadAllText("history.txt").Trim().Split(';');
                foreach (var entry in historyentries)
                {
                    if (!String.IsNullOrEmpty(entry)) { history.Add(entry); }
                }

                return history;
            }

            static string[] GetObjects(string defaultdir)
            {
                string[] folders = Directory.GetDirectories(defaultdir);
                string[] files = Directory.GetFiles(defaultdir);
                int[] obj = new int[folders.Length + files.Length];
                string[] objects = new string[obj.Length];
                Array.Copy(folders, objects, folders.Length);
                Array.Copy(files, 0, objects, folders.Length, files.Length);

                return objects;

            }
            static void ReadPages(string[,] i, int lenght, int page)
            {
                for (int k = 0; k < lenght; k++)
                {

                    string elem = i[k, 0];
                    int p = Convert.ToInt32(i[k, 1]);
                    if (p == page)
                    {
                        Console.WriteLine(elem);
                    }

                }
                Console.WriteLine($"Page: {page}");
            }
            static string[,] ChunkToPages(string[] i, int page)
            {
                string[,] result = new string[i.Length, 2];
                try
                {
                    int pagecount = 1;
                    for (int k = 0; k < i.Length; k++)
                    {
                        result[k, 0] = i[k];
                        result[k, 1] = pagecount.ToString();
                        if ((k + 1) % page == 0) { pagecount++; }
                    }

                }
                catch (Exception e)
                {
                    ShowEx(e, "ChunkToPages");

                }
                return result;
            }
            private static void CommandParser(string input, int page, string currentdir)
            {
                var parser = new CommandParser();
                var command = parser.GetCommand(input);
                PerformOperation(command, page, input);
            }
            public static void PerformOperation(string command, int page, string input)

            {
                string obj = GetObjAfterParser(input, command);
                switch (command)
                {
                    case "cd":
                        GoToDirectory(page, obj);
                        break;
                    case "copy":
                        Copy(obj);
                        break;
                    case "del":
                        Delete(obj);
                        break;
                    case "info":
                        Info(obj);
                        break;
                }
            }

            static string GetObjAfterParser(string command, string splitter)
            {
                try
                {
                    string[] comArray = command.Split(splitter + ':');
                    string obj = comArray[1].Trim();
                    return obj;
                }
                catch
                {
                    return String.Empty;
                }
            }
            private static void Info(string obj)
            {
                try
                {
                    if (!obj.Contains('.'))
                    {
                        var objtype = new Folders();
                        DirectoryInfo folder = new DirectoryInfo(obj);
                        objtype.Info(folder);
                        Console.ReadLine();
                    }
                    else
                    {
                        var objtype = new Files();
                        FileInfo file = new FileInfo(obj);
                        objtype.Info(file);
                    }
                }
                catch (Exception e)
                {
                    ShowEx(e, "info");
                }
            }

            private static void Delete(string obj)
            {
                try
                {
                    {
                        if (!obj.Contains('.'))
                        {
                            var objtype = new Folders();
                            DirectoryInfo folder = new DirectoryInfo(obj);
                            objtype.Delete(folder);
                        }
                        else
                        {
                            var objtype = new Files();
                            FileInfo file = new FileInfo(obj);
                            objtype.Delete(file);
                        }
                    }
                }
                catch (Exception e)
                {
                    ShowEx(e, "del");
                }
            }
            private static void Copy(string obj)
            {
                try
                {

                    var originpath = obj.Split(">");

                    if (!obj.Contains('.'))
                    {
                        var objtype = new Folders();
                        DirectoryInfo pathfolder = new DirectoryInfo(originpath[0]);
                        DirectoryInfo pastepath = new DirectoryInfo(originpath[1]);
                        objtype.Copy(pathfolder, pastepath);
                        Console.Clear();
                        Program.Main();
                    }
                    else
                    {
                        var objtype = new Files();
                        FileInfo origin = new FileInfo(originpath[0]);
                        DirectoryInfo pastepath = new DirectoryInfo(originpath[1]);
                        objtype.Copy(origin, pastepath);
                        Console.Clear();
                        Program.Main();
                    };

                }
                catch (Exception e)
                {
                    ShowEx(e, "copy");
                }
            }
            private static void GoToDirectory(int page, string obj)
            {
                try
                {
                    DirectoryInfo folderpath = new DirectoryInfo(obj);
                    if (folderpath.Exists)
                    {
                        string[] objects = GetObjects(folderpath.FullName);
                        Console.Clear();
                        ReadObjects(objects, obj, page, 1);
                    }
                    else
                    {
                        Console.WriteLine("Empty path detected.Press Enter to get on previous page");//не стал это логировать в файл. просто потому что не увидел смысла:)но если прям сильно нужно, могу и в лог записать, пример ниже
                        Console.ReadLine();
                    }
                }
                catch (Exception e)
                {
                    ShowEx(e, "cd");
                    return;
                }
            }

            static private void ShowEx(Exception ex, string operation)
            {
                string message = ex.Message;
                File.ReadAllText("ErrorLog.txt");
                string logstring = $"Error! {DateTime.Now}: {message}";
                File.AppendAllText("ErrorLog.txt", logstring + Environment.NewLine);
                Console.WriteLine($"Error while processing comand/method '{operation}'. Reason: {message}");
                Console.ReadLine();
            }

            static private void AppFilesChk()
            {
                var history = Path.Combine(Directory.GetCurrentDirectory(), "history.txt");
                if (!File.Exists(history))
                {
                    using (StreamWriter sr = File.CreateText(history)) { sr.WriteLine(";"); }
                }

                var errorlog = Path.Combine(Directory.GetCurrentDirectory(), "ErrorLog.txt");
                if (!File.Exists(errorlog))
                {
                    using (StreamWriter sr = File.CreateText(errorlog)) { sr.WriteLine(";"); }
                }

            }
        }

    }




