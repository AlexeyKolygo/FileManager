using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Text;

namespace FileManager
{


    public class Folders
    {
        public void Copy(DirectoryInfo path, DirectoryInfo paste)
        {

            var IsExist_original = path.Exists;
            var IsExist_paste = paste.Exists;
            if (!IsExist_paste) { var newfolder = Directory.CreateDirectory(paste.FullName); }

            if (IsExist_original)
            {
                foreach (var subfolder in Directory.GetDirectories(path.FullName))
                {
                    Directory.CreateDirectory(subfolder.Replace(path.FullName, paste.FullName));
                }

                foreach (string file in Directory.GetFiles(path.FullName))
                {
                    File.Copy(file, file.Replace(path.FullName, paste.FullName), true);
                }
                Console.WriteLine($"Folder {path} and all its contents has been copied to {paste}");
                Console.ReadLine();
            }
            else { Console.WriteLine($"Folder {path} or {paste}  doesn't exist"); Console.ReadLine(); }//я не придумал как можно сделать проверку на paste, потому что Directory создают в соурс папке директорию, если не может найти указанный путь
        }

        public void Delete(DirectoryInfo folder)
        {

            bool IsExist = folder.Exists;
            if (IsExist)
            {
                Directory.Delete(folder.FullName, true);
                Console.WriteLine($"{folder} is deleted for good");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("Folder doesn't exist");
                Console.ReadLine();
            }
        }
        public void Info(DirectoryInfo folder)
        {
            try
            {
                var size = GetSize(folder);

                Console.WriteLine(@$"Basic info about this directory:
FileName:  {folder.FullName}
Created: {folder.CreationTime}
Parent: {folder.Parent}
FolderSize: {size / 1000000} megabytes");
            }
            catch (UnauthorizedAccessException) { }
        }

        static float GetSize(DirectoryInfo folder)
        {
            float size = 0;
            FileInfo[] files = folder.GetFiles();
            try
            {
                foreach (FileInfo file in files)
                {
                    size += file.Length;
                }
                DirectoryInfo[] directories = folder.GetDirectories();
                foreach (DirectoryInfo directory in directories)
                {
                    size += GetSize(directory);
                }
            }
            catch (UnauthorizedAccessException) { }
            return size;
        }
    }
}
    public class Files
    {
        public void Copy(FileInfo file, DirectoryInfo paste)
        {

            var IsExist_original = file.Exists;
            var IsExist_paste = paste.Exists;
            if (!IsExist_original) { Console.WriteLine("File you want to copy doesn't exist"); }
            if (!IsExist_paste) { Directory.CreateDirectory(paste.FullName); }
            var DestFileName = Path.Combine(paste.FullName, file.Name);
            file.CopyTo(DestFileName, true);
            Console.WriteLine($"File {file} has been copied to {paste}");
            Console.ReadLine();
        }

        public void Delete(FileInfo file)
        {

            bool IsExist = file.Exists;
            if (IsExist)
            {
                File.Delete(file.FullName);
                Console.WriteLine($"{file} is deleted for good");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("File doesn't exist");
                Console.ReadLine();
            }
        }

        public void Info(FileInfo file)
        {
            Console.WriteLine(@$"Basic info about this file:
FileName:  {file.FullName}
Attribute: {file.Attributes}
Extension: {file.Extension}
Created:{file.CreationTime}
Parent: {file.Directory}
FolderSize: {file.Length} bytes
");

        }
    }

