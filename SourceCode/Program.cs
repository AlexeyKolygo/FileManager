using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading;

namespace FileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            var appfile = File.ReadAllText("appsettings.json");
            var settings = JsonSerializer.Deserialize<Settings>(appfile);
            string folder = String.IsNullOrEmpty(settings.CurrentDir) ? settings.DefaultDir : settings.CurrentDir;
            DirectoryInfo folderpath = new DirectoryInfo(folder);
            string[] objects = GetObjects(folderpath.FullName);
            ReadObjects(objects, folder, settings.PageCounter, settings.CurrentPage);
        }

        public static void ReadObjects(string[] folders, string currentdir, int pagecounter, int p)
        {

            var folder_list = ChunkToPages(folders, pagecounter);
            var last_page = folder_list[folder_list.GetLength(0) - 1, 1];
            var appfile = File.ReadAllText("appsettings.json");
            var settings = JsonSerializer.Deserialize<Settings>(appfile);
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
                    var history = File.ReadAllText("history.txt").Trim().Split(';');
                    Console.Write("Command>: ");
                    string command = ScrollCommand(history, history.Length - 1).Trim();
                    File.AppendAllText("history.txt", command + ';' + Environment.NewLine);
                    CommandParser(command, pagecounter, currentdir);

                }
                if (input.Key == ConsoleKey.Escape)
                {

                    var settingsset = new Settings("", currentdir, p, pagecounter);
                    settingsset.SettingsSetter();
                    File.WriteAllText("history.txt", ";");
                    Environment.Exit(0);

                }
                Console.Clear();
            }


        }
        static string ScrollCommand(string[] history, int i)
        {
            string result = Console.ReadLine();
            if (String.IsNullOrEmpty(result))
            {
                var cominput = Console.ReadKey();

                if (cominput.Key == ConsoleKey.UpArrow)
                {
                    for (int k = 0; k < history.Length; k++)
                    {
                        k = (k == history.Length - 1) ? 0 : k;
                        result = history[k];
                        Console.Write(result);
                        var res = Console.ReadKey();
                        if (res.Key == ConsoleKey.DownArrow)
                        {
                            k = k == 0 ? 1 : k;
                            Console.Write(history[k - 1]);
                            Console.ReadKey();
                        }
                        if (res.Key == ConsoleKey.Enter)
                        {
                            return result;
                        }
                        if (res.Key == ConsoleKey.Escape)
                        {
                            return result;
                        }

                    }

                }
            }
            else
            {
                return result;
            }
            return result;
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
            int pagecount = 1;
            for (int k = 0; k < i.Length; k++)
            {
                result[k, 0] = i[k];
                result[k, 1] = pagecount.ToString();
                if ((k + 1) % page == 0) { pagecount++; }
            }
            return result;
        }


        private static void CommandParser(string command, int page, string currentdir)
        {
            var parser = new CommandParser();
            var todo = parser.GetCommand(command);
            if (todo == Command.cd.ToString())
            {
                string obj = GetObjAfterParser(command, todo);

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
                catch (UnauthorizedAccessException ue)
                {
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {ue.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Access to this folder is denied");
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {e.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Something went wrong! Please refer to the error log file for details");
                    Console.ReadLine();

                }
            }

            if (todo == Command.copy.ToString())
            {
                try
                {
                    string obj = GetObjAfterParser(command, todo);
                    var originpath = obj.Split(">");

                    if (!obj.Contains('.'))
                    {
                        var objtype = new Folders();
                        DirectoryInfo pathfolder = new DirectoryInfo(originpath[0]);
                        DirectoryInfo pastepath = new DirectoryInfo(originpath[1]);
                        objtype.Copy(pathfolder, pastepath);
                    }
                    else
                    {
                        var objtype = new Files();
                        FileInfo origin = new FileInfo(originpath[0]);
                        DirectoryInfo pastepath = new DirectoryInfo(originpath[1]);
                        objtype.Copy(origin, pastepath);
                    };

                }
                catch (UnauthorizedAccessException ue)
                {
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {ue.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Access to this object is denied");
                    Console.ReadLine();
                }

                catch (Exception e)
                {
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {e.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Something went wrong! Please refer to the error log file for details");
                    Console.ReadLine();

                }


            }

            if (todo == Command.del.ToString())
                try
                {
                    {
                        string obj = GetObjAfterParser(command, todo);
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
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {e.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Something went wrong! Please refer to the error log file for details");
                    Console.ReadLine();

                }
            if (todo == Command.info.ToString())
            {
                try
                {
                    string obj = GetObjAfterParser(command, todo);
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
                    File.ReadAllText("ErrorLog.txt");
                    string exmessage = $"Error! {DateTime.Now}: {e.Message}";
                    File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
                    Console.WriteLine("Something went wrong! Please refer to the error log file for details");
                    Console.ReadLine();

                }
            }
        }
        static string GetObjAfterParser(string command, string splitter)
        {
            try { 

            string[] comArray = command.Split(splitter + ':');
            string obj = comArray[1].Trim();
            return obj;
            }
            catch 
            {
               
                Console.WriteLine("Can't get valid command");
                Console.ReadLine();
                return string.Empty;
            }
        }

    }
}




