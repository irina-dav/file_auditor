using Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using FaReport.Controllers;
using FaReport.FaStorageService;

namespace FaReport.ViewModels
{
    class CClientsListViewModel : CBaseObservableObject
    {
        private readonly IMainController _controller;
        private readonly IFaStorageService _storageService;
        private readonly Window _view;

        public CClientsListViewModel(IMainController controller, IFaStorageService storageService, Window view)
        {
            view.DataContext = this;
            _storageService = storageService;
            _controller = controller;
            _view = view;
            Clients = _storageService.FindClientsWithLastDate().Select(c => new CClientItemViewModel(c)).ToList();
        }

        public IReadOnlyCollection<CClientItemViewModel> Clients { get; }

        public ICommand ShowReportCommand => new CRelayCommand(a => RunShowReportCommand(), f => true);

        private void RunShowReportCommand()
        {
           _controller.ShowReport();
        }
    }
}
