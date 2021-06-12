using System;
using System.IO;
using System.Collections.Generic;

namespace FileManager
{
    class AppServiceReader
    {
        private int history_cursor;
        public string ScrollCommand(List<string> history)
        {

            string result = String.Empty;
            while (true)
            {

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow:
                        result= MoveCursorUp(history,history_cursor);
                        Console.Write(result);
                        history_cursor = history_cursor - 1;
                        continue;
                    case ConsoleKey.DownArrow:
                        result = MoveCursorDown(history, history_cursor);
                        Console.Write(result);
                        history_cursor = history_cursor + 1;
                        continue;

                    case ConsoleKey.Enter:
                        return result;
                }
            }
        }

        private string MoveCursorUp(List<string> history,int history_cursor)
        {
            string result = string.Empty;
            history_cursor = history_cursor - 1;
            history_cursor = history_cursor < 0 ? 0 : history_cursor;
            history_cursor = history_cursor >= history.Count ? history.Count - 1 : history_cursor;
            result = history[history_cursor];
            return result;
        }

        private string MoveCursorDown(List<string> history, int history_cursor)
        {
            string result = string.Empty;
            history_cursor = history_cursor + 1;
            history_cursor = history_cursor < 0 ? 0 : history_cursor;
            history_cursor = history_cursor >= history.Count ? history.Count - 1 : history_cursor;
            result = history[history_cursor];
            return result;
        }
        public void AppFilesChk()
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

        public List<string> HistoryReader()
        {
            var history = new List<string>();

            var historyentries = File.ReadAllText("history.txt").Trim().Split(';');
            foreach (var entry in historyentries)
            {
                if (!String.IsNullOrEmpty(entry)) { history.Add(entry); }
            }

            return history;
        }

        public string[] GetObjects(string defaultdir)
        {

            string[] folders = Directory.GetDirectories(defaultdir);
            string[] files = Directory.GetFiles(defaultdir);
            int[] obj = new int[folders.Length + files.Length];
            string[] objects = new string[obj.Length];
            Array.Copy(folders, objects, folders.Length);
            Array.Copy(files, 0, objects, folders.Length, files.Length);

            return objects;
  
        }
        public void ReadPages(string[,] i, int lenght, int page)
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
        public string[,] ChunkToPages(string[] i, int page)
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
