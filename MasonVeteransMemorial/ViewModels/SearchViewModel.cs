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
    public interface ISearchViewModelDelegate
    {
        void OnSearchComplete();
        void OnLoadSuccess();
        void OnLoadFailure(string title, string message);
    }

    public class SearchViewModel : BaseViewModel
    {
        public ISearchViewModelDelegate Delegate { get; set; }

        private ICollection<Brick> _bricks = null;
        public ICollection<Brick> Bricks
        {
            get => _bricks;
            set => SetProperty(ref _bricks, value);
        }

        private ObservableCollection<Brick> _bricksFiltered = null;
        public ObservableCollection<Brick> BricksFiltered
        {
            get => _bricksFiltered;
            set => SetProperty(ref _bricksFiltered, value);
        }

        private Brick _selectedBrick = null;
        public Brick SelectedBrick
        {
            get => _selectedBrick;
            set => SetProperty(ref _selectedBrick, value);
        }

        private string _searchText = null;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _isVisibleSearchResults = false;
        public bool IsVisibleSearchResults
        {
            get => _isVisibleSearchResults;
            set => SetProperty(ref _isVisibleSearchResults, value);
        }

        private bool _useMapPage = false;
        public bool UseMapPage
        {
            get => _useMapPage;
            set => SetProperty(ref _useMapPage, value);
        }

        private MemorialMapViewModel _mapViewModel;
        public MemorialMapViewModel MapViewModel
        {
            get => _mapViewModel;
            set => SetProperty(ref _mapViewModel, value);
        }

        //public AppCommand LoadCommand => new AppCommand(Load);
        public Command LoadCommand => new Command(Load);
        public Command SearchCommand => new Command(async () => await Search(SearchText));
        public Command ResetSearchCommand => new Command(ResetSearch);
        public Command LocateBrickCommand => new Command(LocateBrick);

        public SearchViewModel()
        {
            BricksFiltered = new ObservableCollection<Brick>();
            Bricks = new List<Brick>();
            MapViewModel = new MemorialMapViewModel();
        }

        protected async void Load()
        {
            var bricks = await MasonMemorialBricksBusinessManager.Current.GetBricks();

            Bricks = bricks;

            Delegate?.OnLoadSuccess();
        }

        protected void ResetSearch()
        {
            BricksFiltered.Clear();
        }

        protected void LocateBrick()
        {
            MapViewModel.SelectedBrick = SelectedBrick;
            MapViewModel.MapBrickCommand.Execute(null);
        }

        async Task<ObservableCollection<Brick>> Search(string searchText)
        {
            IsBusyMessage = "Searching...";

            if (!UseMapPage)
                IsVisibleSearchResults = false;

            ObservableCollection<Brick> results = null;
            var message = new StringBuilder();

            var coll = Bricks;

            if (Bricks != null)
            {
                if (searchText == "")
                    results = new ObservableCollection<Brick>(Bricks);
                else
                    results = new ObservableCollection<Brick>(Bricks.Where(t => t.FullName.IndexOf(searchText ?? "", StringComparison.CurrentCultureIgnoreCase) != -1).ToList());
            }

            BricksFiltered = results;

            Delegate?.OnSearchComplete();

            return await Task.Run(() => { return results; });
        }
    }
}
