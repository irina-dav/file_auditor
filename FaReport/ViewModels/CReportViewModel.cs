using FaReport.Controllers;
using FaReport.FaStorageService;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using NLog;

namespace FaReport.ViewModels
{
    class CReportViewModel : CBaseObservableObject
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private readonly IMainController _controller;
        private readonly Window _view;
        private int _itemsLimit;
        private readonly IFaStorageService _storageService;

        private string _computerName;
        private string _ipAddress;
        private string _userName;
        private string _fileName;
        private DateTime? _periodStart;
        private DateTime? _periodEnd;
        private EFileEvents _fileEvents;

        private ObservableCollection<CEventViewModel> _events;

        public CReportViewModel(IMainController controller, IFaStorageService storageService, Window view)
        {
            view.DataContext = this;
            _storageService = storageService;
            _controller = controller;
            _view = view;
            int.TryParse(ConfigurationManager.AppSettings["ReportSearchLimitDefault"], out _itemsLimit);
            Events = new ObservableCollection<CEventViewModel>(_storageService.SearchEvents(new CSearchFilter { Limit = _itemsLimit })
                .Select(e => new CEventViewModel(e)));
        }

        public string ComputerName
        {
            get => _computerName;
            set
            {
                _computerName = value;
                OnPropertyChanged();
            }
        }

        public string IpAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string UserName
        { 
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged();
            }
        }

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged();
            }
        }

        public DateTime? PeriodStart
        {
            get => _periodStart;
            set
            {
                _periodStart = value;
                OnPropertyChanged();
            }
        }

        public DateTime? PeriodEnd
        {
            get => _periodEnd;
            set
            {
                _periodEnd = value;
                OnPropertyChanged();
            }
        }

        public EFileEvents FileEvents
        {
            get => _fileEvents;
            set
            {
                if (value == EFileEvents.None)
                    _fileEvents = EFileEvents.None;
                else if (_fileEvents.HasFlag(value))
                    _fileEvents = _fileEvents.RemoveValue(value);
                else
                    _fileEvents = _fileEvents.AddValue(value);
                OnPropertyChanged();
            }
        }

        public int ItemsLimit
        {
            get => _itemsLimit;
            set
            {
                _itemsLimit = value;
                OnPropertyChanged();
            }
        }


        public ObservableCollection<CEventViewModel> Events
        {
            get => _events;
            set
            {
                _events = value;
                OnPropertyChanged();
            }
        }

        public ICommand SearchCommand => new CRelayCommand(a => RunSearchCommand(), f => true);

        public ICommand ExportCsvCommand => new CRelayCommand(a => RunExportCsvCommand(), f => true);

        private void RunSearchCommand()
        {
            CSearchFilter filter = new CSearchFilter()
            {
                FileName = _fileName,
                IpAddress = _ipAddress,
                ComputerName = _computerName,
                UserName = _userName,
                PeriodStart = _periodStart,
                PeriodEnd = _periodEnd,
                FileEvents = _fileEvents,
                Limit = _itemsLimit
            };
            Events = new ObservableCollection<CEventViewModel>(_storageService.SearchEvents(filter).Select(e => new CEventViewModel(e)));
        }

        private void RunExportCsvCommand()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() {FileName = "export.csv"};
            if (saveFileDialog.ShowDialog() != true)
                return;
            
            try
            {
                bool exportSuccess = _storageService.ExportEventsToCsv(_events.Select(e=> e.EventInfo).ToList(), saveFileDialog.FileName);
                if (exportSuccess)
                    MessageBox.Show("Data has been successfully exported.", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
                else
                    MessageBox.Show("Data wasn't exported", "Export", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                MessageBox.Show($"An error occurred while exporting: {ex.Message}", "Export", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
