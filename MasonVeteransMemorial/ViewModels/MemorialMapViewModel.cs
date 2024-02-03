using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MasonVeteransMemorial.BusinessServices;
using MasonVeteransMemorial;
using MasonVeteransMemorial.Data;
using MasonVeteransMemorial.Models;

namespace MasonVeteransMemorial.ViewModels
{
    public interface IMemorialMapViewModelDelegate
    {
        void OnSearchComplete();
        void OnLoadSuccess();
        void OnLoadFailure(string title, string message);
    }

    public class MemorialMapViewModel : BaseViewModel
    {
        public IMemorialMapViewModelDelegate Delegate { get; set; }

        private bool _brickFound = false;
        public bool BrickFound
        {
            get => _brickFound;
            set => SetProperty(ref _brickFound, value);
        }

        private string _foundQuadrantImageSource = String.Empty;
        public string FoundQuadrantImageSource
        {
            get => _foundQuadrantImageSource;
            set => SetProperty(ref _foundQuadrantImageSource, value);
        }

        private Brick _selectedBrick = null;
        public Brick SelectedBrick
        {
            get => _selectedBrick;
            set => SetProperty(ref _selectedBrick, value);
        }

        public AppCommand MapBrickCommand => new AppCommand(MapBrick);

        public MemorialMapViewModel(Brick brick)
        {
            SelectedBrick = brick;
        }

        public MemorialMapViewModel()
        {

        }

        private void MapBrick()
        {
            if (null == SelectedBrick || string.IsNullOrEmpty(SelectedBrick.Section))
                return;

            FoundQuadrantImageSource = "";
            var quadrant = MasonMemorialBricksBusinessManager.Current.GetQuadrantForMasonBrickCode
                                                             (SelectedBrick.Section, SelectedBrick.Location, SelectedBrick.Position);

            if (null != quadrant)
            {
                FoundQuadrantImageSource = quadrant.ImageSource;
            }
        }
    }
}
