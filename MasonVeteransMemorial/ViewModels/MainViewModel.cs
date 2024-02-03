using System;
using MasonVeteransMemorial;
using MasonVeteransMemorial.Data;
using MasonVeteransMemorial.Models;

namespace MasonVeteransMemorial.ViewModels
{
    public interface IMainViewModelViewModelDelegate
    {
        void OnLoadSuccess();
        void OnLoadFailure(string title, string message);
    }

    public class MainViewModel : BaseViewModel
    {
        public IMainViewModelViewModelDelegate Delegate { get; set; }


        private string _pageName;
        public string PageName
        {
            get => _pageName;
            set => SetProperty(ref _pageName, value);
        }

        public AppCommand LoadCommand => new AppCommand(Load);

        public MainViewModel()
        {

        }

        protected void Load()
        {
            Delegate?.OnLoadSuccess();
        }
    }
}
