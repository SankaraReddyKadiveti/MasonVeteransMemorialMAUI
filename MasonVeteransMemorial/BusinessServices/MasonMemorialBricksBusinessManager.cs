using System;
using MasonVeteransMemorial.BusinessServices.Base;
using System.Collections.Generic;
using System.Linq;
using MasonVeteransMemorial.Models;
using MasonVeteransMemorial.BusinessServices.Services;
using System.Threading.Tasks;

namespace MasonVeteransMemorial.BusinessServices
{
    public interface IMasonMemorialBricksBusinessManager
    {
        Task<ICollection<Brick>> GetBricks();
        SectionQuadrant GetQuadrantForMasonBrickCode(string section, string location, int position);
        void LoadMasonMemorialData();
    }

    public class MasonMemorialBricksBusinessManager : BaseBusinessManager<MasonMemorialBricksBusinessManager, IMasonMemorialBricksBusinessManager>, IMasonMemorialBricksBusinessManager
    {
        public MasonMemorialBricksBusinessManager()
        {
        }

        public async Task<ICollection<Brick>> GetBricks()
        {
            return await MasonMemorialService.Current.GetBricks(true);
        }

        public SectionQuadrant GetQuadrantForMasonBrickCode(string section, string location, int position)
        {
            return MasonMemorialService.Current.GetQuadrantForMasonBrickCode(section, location, position);
        }

        public void LoadMasonMemorialData()
        {
            MasonMemorialService.Current.LoadMasonMemorialData();
        }
    }
}
