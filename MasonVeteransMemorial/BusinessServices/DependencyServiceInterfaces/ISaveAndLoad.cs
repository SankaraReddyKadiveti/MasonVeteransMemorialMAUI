using System;
using System.Collections.Generic;

namespace MasonVeteransMemorial.BusinessServices.DependencyServiceInterfaces
{
    public class FileInfoType
    {
        public string FileName { get; set; }
        public string FullFileName { get; set; }
        public string FilePath { get; set; }
        public double FileSize { get; set; }
    }

    public class DirectoryInfoType
    {
        public string DirectoryName { get; set; }
        public string FullDirectoryName { get; set; }
        public string DirectoryPath { get; set; }
        public double DirectorySize { get; set; }
        public double DirectoryFreeSize { get; set; }
    }

    public interface ISaveAndLoad
    {
        void SaveTextToFile(string filename, string text);
        string LoadTextFile(string filename);
        string SaveFile(string filename, byte[] contents);
        string SaveTextFile(string filename, string content);
        string RenameFile(string origFileName, string newFileName);
        bool DeleteFile(string fileName);
        byte[] GetFile(string filePath);
        FileInfoType[] GetFiles(string filter, string targetFolder = "");
        DirectoryInfoType[] GetDirectories(bool isolatedStorage = false, string targetFolder = "");
        List<DirectoryInfoType> GetDirectorySizes(bool personalOnly = true);
        FileInfoType SaveFileReturnInfoType(string filename, byte[] contents);
        bool DeleteAllFiles(string filter, string targetFolder = "");
        bool DeleteAllFilesInFolder(string folder);
        string GetNextFileName(string directory, string filename, bool AppendNumber = true);
        bool IsFileOnSystem(string filePath);
        bool DeleteAllFilesHandleException(string filter, string targetFolder = "");
        string[] LoadTextFileLines(string filename);
    }
}
