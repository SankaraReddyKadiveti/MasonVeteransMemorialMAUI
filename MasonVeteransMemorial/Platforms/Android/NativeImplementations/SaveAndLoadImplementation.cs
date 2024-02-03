using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MasonVeteransMemorial.BusinessServices.DependencyServiceInterfaces;
using MasonVeteransMemorial.Droid.NativeImplementations;

[assembly: Dependency(typeof(SaveAndLoad))]
namespace MasonVeteransMemorial.Droid.NativeImplementations
{
    public class SaveAndLoad : ISaveAndLoad
    {
        public void SaveTextToFile(string filename, string text)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            System.IO.File.WriteAllText(filePath, text);
        }

        public string LoadTextFile(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return System.IO.File.ReadAllText(filePath);
        }

        public string[] LoadTextFileLines(string filename)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            return System.IO.File.ReadAllLines(filePath);
        }

        public string SaveTextFile(string filename, string content)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string filePath = Path.Combine(path, filename);
            StreamWriter stream = File.CreateText(filePath);
            stream.Write(content);
            stream.Close();

            return filePath;
        }

        public string SaveFile(string filename, byte[] contents)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);

            try
            {
                System.IO.File.WriteAllBytes(filePath, contents);
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                filePath = "";
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error saving image!" + Environment.NewLine + "Message:" + e.Message);
            }

            return filePath;
        }

        public string RenameFile(string origFileName, string newFileName)
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var origFilePath = Path.Combine(documentsPath, origFileName);
            var newFilePath = Path.Combine(documentsPath, newFileName);

            try
            {
                var fi = new System.IO.FileInfo(origFilePath);
                if (fi.Exists)
                    fi.MoveTo(newFilePath);
                else
                    newFilePath = "";
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                newFilePath = "";
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error renaming image!" + Environment.NewLine + "Message:" + e.Message);
            }

            return newFilePath;
        }

        public bool DeleteFile(string fileName)
        {
            var success = true;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filepath = Path.Combine(documentsPath, fileName);

            try
            {
                var fi = new System.IO.FileInfo(filepath);
                if (fi.Exists)
                    fi.Delete();

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                success = false;
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error deleting image!" + Environment.NewLine + "Message:" + e.Message);
            }

            return success;
        }

        public byte[] GetFile(string filePath)
        {
            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving image: {filePath}" + Environment.NewLine + ex);
            }
        }

        public List<DirectoryInfoType> GetDirectorySizes(bool personalOnly = true)
        {
            var personalSize = Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).TotalSpace;
            var personalFreeSize = Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Personal)).FreeSpace;
            var documentsSize = Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).TotalSpace;
            var documentsFreeSize = Android.OS.Environment.GetExternalStoragePublicDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)).FreeSpace;

            var dirs = new List<DirectoryInfoType>();

            dirs.Add(new DirectoryInfoType { DirectoryName = nameof(Environment.SpecialFolder.Personal), DirectorySize = personalSize / 1024, DirectoryFreeSize = personalFreeSize / 1024 });

            if (!personalOnly)
                dirs.Add(new DirectoryInfoType { DirectoryName = nameof(Environment.SpecialFolder.MyDocuments), DirectorySize = documentsSize / 1024, DirectoryFreeSize = documentsFreeSize / 1024 });

            return dirs;
        }

        public FileInfoType[] GetFiles(string filter, string targetFolder = "")
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!string.IsNullOrEmpty(targetFolder))
                documentsPath = Path.Combine(documentsPath, targetFolder);

            var directoryInfo = new DirectoryInfo(documentsPath);

            if (!directoryInfo.Exists)
                directoryInfo.Create();

            FileInfoType[] fileDetails = null;

            try
            {
                var files = new DirectoryInfo(documentsPath).GetFiles(filter);

                if (null != files)
                    fileDetails = files.Select(x => new FileInfoType { FileName = x.Name, FullFileName = x.FullName, FilePath = x.Directory.FullName, FileSize = x.Length }).ToArray();

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error getting files!" + Environment.NewLine + "Message:" + e.Message);
            }

            return fileDetails;
        }

        public DirectoryInfoType[] GetDirectories(bool isolatedStorage = false, string targetFolder = "")
        {
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!string.IsNullOrEmpty(targetFolder))
                documentsPath = Path.Combine(documentsPath, targetFolder);

            DirectoryInfoType[] dirDetails = null;

            //documentsPath = Path.Combine(documentsPath, ".config", ".isolated-storage", "ImageLoaderCache");
            if (isolatedStorage)
                documentsPath = Path.Combine(documentsPath, ".config", ".isolated-storage");

            try
            {
                var dirs = new DirectoryInfo(documentsPath).GetDirectories();
                var files = new DirectoryInfo(documentsPath).GetFiles("*");

                if (null != dirs)
                    dirDetails = dirs.Select(x => new DirectoryInfoType { DirectoryName = x.Name, FullDirectoryName = x.FullName, DirectoryPath = x.FullName }).ToArray();

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error getting directories!" + Environment.NewLine + "Message:" + e.Message);
            }

            return dirDetails;
        }

        public FileInfoType SaveFileReturnInfoType(string filename, byte[] contents)
        {
            if (filename.Contains("/"))
            {
                var name = filename.Split('/');
                filename = name[0] + '/' + name[1].Replace('-', '_').Replace('@', '_');
            }

            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var filePath = Path.Combine(documentsPath, filename);
            FileInfoType fileInfoType = null;

            try
            {
                System.IO.File.WriteAllBytes(filePath, contents);

                var fileinfo = new FileInfo(filePath);

                if (fileinfo.Exists)
                    fileInfoType = new FileInfoType
                    {
                        FileName = fileinfo.Name,
                        FullFileName = fileinfo.FullName,
                        FilePath = fileinfo.Directory.FullName,
                        FileSize = fileinfo.Length
                    };

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.ToString());
                filePath = "";
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error saving image!" + Environment.NewLine + "Message:" + e.Message);
            }

            return fileInfoType;
        }

        public bool DeleteAllFilesInFolder(string folder)
        {
            var success = true;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var targetPath = Path.Combine(documentsPath, folder);

            try
            {
                if (!Directory.Exists(targetPath))
                    return success;

                var files = new DirectoryInfo(targetPath).GetFiles("*.*");

                if (null != files)
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                }
            }
            catch (System.Exception e)
            {
                success = false;
                System.Console.WriteLine(e.ToString());
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error deleting files!" + Environment.NewLine + "Message:" + e.Message);
            }

            return success;
        }


        public bool DeleteAllFiles(string filter, string targetFolder = "")
        {
            var success = true;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!string.IsNullOrEmpty(targetFolder))
                documentsPath = Path.Combine(documentsPath, targetFolder);

            try
            {
                var files = new DirectoryInfo(documentsPath).GetFiles(filter);

                if (null != files)
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                }
            }
            catch (System.Exception e)
            {
                success = false;
                System.Console.WriteLine(e.ToString());
                //*** THROW EXCEPTION FOR NOW
                throw new Exception("Error deleting files!" + Environment.NewLine + "Message:" + e.Message);
            }

            return success;
        }

        public bool DeleteAllFilesHandleException(string filter, string targetFolder = "")
        {
            var success = true;
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!string.IsNullOrEmpty(targetFolder))
                documentsPath = Path.Combine(documentsPath, targetFolder);

            try
            {
                var files = new DirectoryInfo(documentsPath).GetFiles(filter);

                if (null != files)
                {
                    foreach (var file in files)
                    {
                        if (file.Exists)
                            file.Delete();
                    }
                }
            }
            catch (System.Exception e)
            {
                success = false;
                System.Console.WriteLine(e.ToString());

            }

            return success;
        }

        public string GetNextFileName(string directory, string filename, bool AppendNumber = true)
        {
            string formatedTemplate = "{0}{1}.{2}";
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var fileNameWithoutExtension = filename.Split('.')[0];
            var fileExtension = filename.Split('.')[1];

            string newname = String.Format(formatedTemplate, fileNameWithoutExtension, string.Empty, fileExtension);
            string path = Path.Combine(documentsPath, directory, newname);
            int AppendDigit = 0;

            while (AppendNumber && new FileInfo(path).Exists)
            {
                ++AppendDigit;
                newname = String.Format(formatedTemplate, fileNameWithoutExtension, "_" + AppendDigit, fileExtension);
                path = Path.Combine(documentsPath, directory, newname);
            }

            return newname;
        }

        public bool IsFileOnSystem(string filePath)
        {
            return File.Exists(filePath);
        }
    }
}