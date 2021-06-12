using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace FileManager
{
    class CommandsProccesor
    {
        private string currentdir;
        public void ExecuteCommandParser(string input, int page,string currentdir)
        {
            var parser = new CommandParser();
            this.currentdir = currentdir;
            var command = parser.GetCommand(input);
            ExecuteOperation(command, page, input);
        }
        public void ExecuteOperation(string command, int page, string input)

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

        private string GetObjAfterParser(string command, string splitter)
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
        private void Info(string obj)
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
        private void Delete(string obj)
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
        private void Copy(string obj)
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
                    var reader = new AppServiceReader();
                    var curobj = reader.GetObjects(this.currentdir);
                    Program.ReadObjects(curobj, this.currentdir, Program.pagecounter, 1);

                }
                else
                {
                    var objtype = new Files();
                    FileInfo origin = new FileInfo(originpath[0]);
                    DirectoryInfo pastepath = new DirectoryInfo(originpath[1]);
                    objtype.Copy(origin, pastepath);
                    Console.Clear();
                    var reader = new AppServiceReader();
                    var curobj = reader.GetObjects(this.currentdir);
                    Program.ReadObjects(curobj, this.currentdir, Program.pagecounter, 1);
                };

            }
            catch (Exception e)
            {
                ShowEx(e, "copy");
            }
        }
        private void GoToDirectory(int page, string obj)
        {
            try
            {
                DirectoryInfo folderpath = new DirectoryInfo(obj);
                if (folderpath.Exists)
                {
                    var appfilereader = new AppServiceReader();
                    string[] objects = appfilereader.GetObjects(obj);
                    Console.WriteLine(objects[0]);
                    Console.Clear();
                    Program.ReadObjects(objects, obj, page, 1);
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
        private void ShowEx(Exception ex, string operation)
        {
            string message = ex.Message;
            File.ReadAllText("ErrorLog.txt");
            string logstring = $"Error! {DateTime.Now}: {message}";
            File.AppendAllText("ErrorLog.txt", logstring + Environment.NewLine);
            Console.WriteLine($"Error while processing comand/method '{operation}'. Reason: {message}");
            Console.ReadLine();
        }

    }
}
