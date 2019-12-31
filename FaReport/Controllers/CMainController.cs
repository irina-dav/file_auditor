using FaReport.FaStorageService;
using FaReport.ViewModels;
using Infrastructure.Messenger;
using NLog;

namespace FaReport.Controllers
{
    class CMainController : IMainController
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly IFaStorageService _storageService;
       
        public CMainController(IFaStorageService storageService)
        {
            _storageService = storageService;
        }

        public CMediator Mediator => CMediator.Instance;

        public void Start()
        {
            ShowViewClientsList();
        }

        private void ShowViewClientsList()
        {
            ClientsListWindow view = GetClientsListView();
            view.Show();
        }

        private ClientsListWindow GetClientsListView()
        {
            ClientsListWindow view = new ClientsListWindow();
            CClientsListViewModel viewModel = new CClientsListViewModel(this, _storageService, view);
            return view;
        }

        public void ShowReport()
        {
            ReportWindow view = GetReportView();
            view.Show();
        }

        private ReportWindow GetReportView()
        {
            ReportWindow view = new ReportWindow();
            CReportViewModel viewModel = new CReportViewModel(this, _storageService, view);
            return view;
        }
    }
}
