using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MasonVeteransMemorial.BusinessServices.Base;
using MasonVeteransMemorial.BusinessServices.DependencyServiceInterfaces;
using MasonVeteransMemorial.Models;
using MasonVeteransMemorial.Pages;
//using Xamarin.Forms;

namespace MasonVeteransMemorial.BusinessServices.Services
{
    public interface IFileDownloaderService
    {
        Task<Tuple<string, bool, string>> DownloadFile(string fileURL, bool autoFileName = false, string ext = "txt");
        string SaveFile(string fileName, byte[] contents);
        string BuildFileName(string extension);
        string BuildImageFileName(string filename);
        List<Brick> LoadBrickResourceFile();
        List<Brick> LoadBrickCSVLocalStorageFile(string filePath);
        string[] SplitCSV(string input);
    }


    public class FileDownloaderService : BaseService<FileDownloaderService, IFileDownloaderService>, IFileDownloaderService
    {
        private const int _downloadImageTimeoutInSeconds = 15;

        public async Task<Tuple<string, bool, string>> DownloadFile(string fileURL, bool autoFileName = false, string ext = "txt")
        {

            var success = true;
            var message = "";
            Uri file = new Uri(fileURL);
            string filename = "";

            if (file.Segments.Count() > 0)
            {
                filename = file.Segments.Last();
            }
            else
                filename = BuildFileName(string.IsNullOrEmpty(ext) ? "csv" : ext);

            var res = await DownloadFileAsync(fileURL);
            success = res.Item2;
            message = res.Item3;

            var saveFileName = filename;

            if (autoFileName)
                saveFileName = BuildFileName(string.IsNullOrEmpty(ext) ? "csv" : ext);

            var filePath = SaveFile(saveFileName, res.Item1);

            return new Tuple<string, bool, string>(filePath, success, message);
        }

        public string[] SplitCSV(string input)
        {
            Regex csvSplit = new Regex("(?:^|,)(\"(?:[^\"]+|\"\")*\"|[^,]*)", RegexOptions.Compiled);
            List<string> list = new List<string>();
            string curr = null;
            foreach (Match match in csvSplit.Matches(input))
            {
                curr = match.Value;
                if (0 == curr.Length)
                {
                    list.Add("");
                }

                list.Add(curr.TrimStart(','));
            }

            return list.ToArray();
        }

        public List<Brick> LoadBrickCSVLocalStorageFile(string filePath)
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FileDownloaderService)).Assembly;

            var lines = DependencyService.Get<ISaveAndLoad>().LoadTextFileLines(filePath);

            var bricks = new List<Brick>();

            //*** New CSV Header: 
            //*** Name, Section, Location, Position, 1st Line, 2nd Line, 3rd Line, Comments

            foreach(string line in lines)
            {
                if (line.Contains("Section") && line.Contains("Location"))
                    continue;

                var brick = new Brick();

                var newValues = SplitCSV(line);

                if (!string.IsNullOrEmpty(newValues[0]) && !string.IsNullOrEmpty(newValues[1]))
                {
                    brick.FullName = newValues[0].Replace("\"", "");
                    brick.Section = newValues[1];
                    brick.Location = newValues[2];
                    brick.Position = int.Parse(newValues[3]);
                    brick.Line1st = newValues[4].Replace("\"", "");
                    brick.Line2nd = newValues[5].Replace("\"", "");
                    brick.Line3rd = newValues[6].Replace("\"", "");
                    brick.Comments = newValues[7].Replace("\"", "");

                    bricks.Add(brick);
                }
            }

            return bricks;
        }

        public List<Brick> LoadBrickResourceFile()
        {
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FileDownloaderService)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("MasonVeteransMemorial.Resources.Raw.MemorialBricks.csv");            
            var bricks = new List<Brick>();

            //*** New CSV Header: 
            //*** Name, Section, Location, Position, 1st Line, 2nd Line, 3rd Line, Comments

            using (var reader = new StreamReader(stream))
            {
                string line = "";
                long counter = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    if (counter > 0)
                    {
                        var brick = new Brick();

                        var newValues = SplitCSV(line);

                        if (!string.IsNullOrEmpty(newValues[0]) && !string.IsNullOrEmpty(newValues[1]))
                        {
                            brick.FullName = newValues[0].Replace("\"", "");
                            brick.Section = newValues[1];
                            brick.Location = newValues[2];
                            brick.Position = int.Parse(newValues[3]);
                            brick.Line1st = newValues[4].Replace("\"", "");
                            brick.Line2nd = newValues[5].Replace("\"", "");
                            brick.Line3rd = newValues[6].Replace("\"", "");
                            brick.Comments = newValues[7].Replace("\"", "");

                            bricks.Add(brick);
                        }
                    }

                    counter++;
                }
            }

            return bricks;
        }

        //public List<SectionQuadrant> LoadQuadrantsResourceFile()
        //{
        //    var assembly = IntrospectionExtensions.GetTypeInfo(typeof(FileDownloaderService)).Assembly;
        //    Stream stream = assembly.GetManifestResourceStream("MasonVeteransMemorial.Data.MasonMapQuardrants.csv");
        //    var quadrants = new List<SectionQuadrant>();

        //    //*** New CSV Header: 
        //    //*** Name,Prefix,Row,Col,Height,Width,Top,Left,ImageSource

        //    using (var reader = new System.IO.StreamReader(stream))
        //    {
        //        string line = "";
        //        long counter = 0;

        //        while ((line = reader.ReadLine()) != null)
        //        {
        //            if (counter > 0)
        //            {
        //                var quadrant = new SectionQuadrant();

        //                var newValues = SplitCSV(line);

        //                if (!string.IsNullOrEmpty(newValues[0]) && !string.IsNullOrEmpty(newValues[1]))
        //                {
        //                    quadrant.Name = newValues[0].Replace("\"", "");
        //                    quadrant.Section = newValues[1];
        //                    quadrant.Location = newValues[2];
        //                    quadrant.Prefix = newValues[3];
        //                    quadrant.Row = int.Parse(newValues[4]);
        //                    quadrant.Col = int.Parse(newValues[5]);
        //                    quadrant.Height = float.Parse(newValues[6]);
        //                    quadrant.Width = float.Parse(newValues[7]);
        //                    quadrant.Top = float.Parse(newValues[8]);
        //                    quadrant.Left = float.Parse(newValues[9]);
        //                    quadrant.ImageSource = newValues[10].Replace("\"", "");

        //                    quadrants.Add(quadrant);
        //                }
        //            }

        //            counter++;
        //        }
        //    }

        //    return quadrants;
        //}

        private async Task<Tuple<byte[], bool, string>> DownloadFileAsync(string fileUrl)
        {
            var success = true;
            var message = "";
            byte[] bytes = null;
            HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(_downloadImageTimeoutInSeconds) };

            try
            {
                using (var httpResponse = await httpClient.GetAsync(fileUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        bytes = await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return new Tuple<byte[], bool, string>(bytes, false, "Invalid URL");
                    }
                }
            }
            catch (Exception ex)
            {
                //Handle Exception
                success = false;
                message = ex.Message;
            }

            return new Tuple<byte[], bool, string>(bytes, success, message);
        }

        public string SaveFile(string fileName, byte[] contents)
        {
            string imgPath = "";

            var saveAndLoad = DependencyService.Get<ISaveAndLoad>();
            if (saveAndLoad != null)
            {
                var fileinfo = saveAndLoad.SaveFileReturnInfoType(fileName, contents);

                imgPath = fileinfo.FullFileName;
            }

            return imgPath;
        }

        public string BuildFileName(string extension)
        {
            var name = $"file_{Guid.NewGuid()}.{extension}";
            return name;
        }

        public string BuildImageFileName(string filename)
        {
            var name = filename;
            var date = DateTime.Now;
            name = $"{filename}-{date.Year.ToString("00")}-{date.Month.ToString("00")}-{date.Day.ToString("00")}-{date.Hour.ToString("00")}-{date.Minute.ToString("00")}-{date.Second.ToString("00")}.jpg";

            return name;
        }
    }
}
