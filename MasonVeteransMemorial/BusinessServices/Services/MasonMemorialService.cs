using System;
using MasonVeteransMemorial.BusinessServices.Base;
using System.Collections.Generic;
using System.Linq;
using MasonVeteransMemorial.Models;
using System.Threading.Tasks;
using MasonVeteransMemorial.BusinessServices.CommonUtils;

namespace MasonVeteransMemorial.BusinessServices.Services
{
    public interface IMasonMemorialService
    {
        Task<ICollection<Brick>> GetBricks(bool reload = false);
        SectionQuadrant GetQuadrantForMasonBrickCode(string section, string location, int position);
        void LoadMasonMemorialData();
    }

    public class MasonMemorialService : BaseService<MasonMemorialService, IMasonMemorialService>, IMasonMemorialService
    {
        bool _loadingFile = false;
        bool _fileLoaded = false;

        public MasonMemorialService()
        {
            LoadMasonMemorialData();
        }

        public void LoadMasonMemorialData()
        {
            //*** Load Bricks Data
            if (null == MasonMemorialData.MasonBricks || MasonMemorialData.MasonBricks.Count == 0)
                MasonMemorialData.MasonBricks = FileDownloaderService.Current.LoadBrickResourceFile();
        }

        public async Task<ICollection<Brick>> GetBricks(bool reload = false)
        {
            ICollection<Brick> bricks = new List<Brick>();


            if (!_fileLoaded && !_loadingFile)
            {
                try
                {
                    _loadingFile = true;
                    var response = await FileDownloaderService.Current.DownloadFile("C:\\Users\\Lachimalla Gopi\\Downloads\\Projects\\MasonVeteransMemorial\\MasonVeteransMemorial\\Data\\MemorialBricks.csv");
                    // htt ps://www.imaginemason.org/download/PDFs/About/MemorialBricks.csv
                    // C:\\Projects\\MobileApp\\MasonVeteransMemorialMobile\\MasonVeteransMemorial\\Data\\MemorialBricks.csv
                    if (response.Item2)
                    {
                        MasonMemorialData.MasonBricks = FileDownloaderService.Current.LoadBrickCSVLocalStorageFile(response.Item1);

                        _fileLoaded = true;
                    }
                }
                catch
                {
                    // eat the exception and run from the local brick resource csv
                } finally {
                    _loadingFile = false;
                }
            }

            bricks = MasonMemorialData.MasonBricks;
            return await Task.Run(() => { return bricks; });
        }

        public SectionQuadrant GetQuadrantForMasonBrickCode(string section, string location, int position)
        {
            var quadrant = new SectionQuadrant();

            if (section.ToUpper() != "HONOR")
            {
                quadrant = MasonMemorialData.SectionGridQuadrants.FirstOrDefault(x => x.Section == section && x.Location == location);
                return quadrant;
            }

            var brick = new GridItemDimension()
            {
                Top = MasonMemorialData.MasonBrickDimension.Height * (BusinessServicesSettings.HonorSectionRowBrickCount - 1 - GetHonorGridRowIndex(location)),//Get reversed since it goes from mm-a from top to bottom
                Left = (position - 1) * MasonMemorialData.MasonBrickDimension.Width,
                Width = MasonMemorialData.MasonBrickDimension.Width,
                Height = MasonMemorialData.MasonBrickDimension.Height
            };

            MasonMemorialData.SectionGridQuadrants.Select(x => { x.OverLapArea = 0; return x; }).ToList();
            foreach (var q in MasonMemorialData.SectionGridQuadrants.Where(_ => _.Section == section))
            {
                float topOverlap = 0, leftOverLap = 0;
                bool isContainedTop = false, isContainedLeft = false;

                /// Top check - use explicit parenthesis to make logic clearer and easy to read
                if (brick.Top <= q.Top && (brick.Top + brick.Height) > q.Top)
                    topOverlap = Math.Abs((brick.Top + brick.Height) - q.Top);
                else if (brick.Top < (q.Top + q.Height) && (brick.Top + brick.Height) >= (q.Top + q.Height))
                    topOverlap = Math.Abs((brick.Top + brick.Height) - (q.Top + q.Height));
                else if (brick.Top >= q.Top && (brick.Top + brick.Height) <= (q.Top + q.Height))
                    isContainedTop = true;

                /// left check  use explicit parenthesis to make logic clearer and easy to read
                if (brick.Left <= q.Left && (brick.Left + brick.Width) > q.Left)
                    leftOverLap = Math.Abs((brick.Left + brick.Width) - q.Left);
                else if (brick.Left < (q.Left + q.Width) && (brick.Left + brick.Width) >= (q.Left + q.Width))
                    leftOverLap = Math.Abs((brick.Left + brick.Width) - (q.Left + q.Width));
                else if (brick.Left >= q.Left && (brick.Left + brick.Width) <= (q.Left + q.Width))
                    isContainedLeft = true;

                if (isContainedTop && isContainedLeft)
                {
                    quadrant = q;
                    return quadrant;
                }
                else
                    q.OverLapArea = (isContainedTop ? brick.Height : topOverlap) * (isContainedLeft ? brick.Width : leftOverLap);
            }

            quadrant = MasonMemorialData.SectionGridQuadrants.OrderByDescending(x => x.OverLapArea).Take(1).FirstOrDefault();
            return quadrant;
        }

        private int GetHonorGridRowIndex(string loc)
        {
            var code = loc.Trim();
            var rowindex = Utils.GetIndexInAlphabet(code.ToCharArray()[0]);
            if (code.Length > 1)
                rowindex += 26;

            return rowindex;
        }
    }
}
