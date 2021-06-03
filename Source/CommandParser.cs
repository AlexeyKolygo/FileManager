using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;


namespace FileManager
{
    public enum Command
    {
        cd,//команда перехода к директории в формате cd:{folderPath}
        copy,//команда копирования директории или файла в формате copy:{sourceObject}>{destPath}
        move,//заготовка. в проекте не реализовано, но перемещение гораздо проще копирования. при необходимости могу и это реализовать
        del,//команда удаления директории или файла в формате del:{sourceObject}
        info,//команда вывода информации о директории или файле в формате info:{sourceObject}
    }
    public class CommandParser
        
    {
        public string GetCommand(string str)
        {
            string command = "";
            try { 
            string[] comArray = str.Split(':');
            command = comArray[0];
            
            bool valid = ValidateCommand(command);
            if (!valid) { Console.WriteLine("Unkknown command.Press enter to get on previous page"); Console.ReadLine(); }
            }
            catch
            {
                File.ReadAllText("ErrorLog.txt");
                string exmessage = $"Error! {DateTime.Now}: You should use ':' as delimetr";
                File.AppendAllText("ErrorLog.txt", exmessage + Environment.NewLine);
            }
            return command;
        }


        private bool ValidateCommand(string command)
        {
            var comandlist = Enum.GetValues(typeof(Command));
            foreach(var x in comandlist) { 
            if (command == x.ToString()) { return true;}
            }
            return false;
        }
    }
}
