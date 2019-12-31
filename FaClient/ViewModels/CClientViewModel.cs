using FaClient.Controllers;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

[assembly: InternalsVisibleTo("FaClient.Tests")]

namespace FaClient.ViewModels
{
    public class CClientViewModel : CBaseObservableObject
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private CRulesListViewData _viewData;

        private readonly Window _view;

        private readonly IRulesController _controller;

        private CRuleItemViewData _selectedItem;

        private int _lastEventsLimit =  int.Parse(ConfigurationManager.AppSettings["LastEventsLimit"]);

        public CClientViewModel(IRulesController controller, Window view)
        {
            view.DataContext = this;
            _view = view;
            LastEvents = new ObservableCollection<string>();
            _controller = controller;
            if (controller.Mediator != null)
            {
                controller.Mediator.Register(EMessageTypes.MsgRuleSaved, RefreshList);
                controller.Mediator.Register(EMessageTypes.MsgRuleDeleted, RefreshList);
                controller.Mediator.Register(EMessageTypes.MsgRuleChecked, UpdateCheck);
                controller.Mediator.Register(EMessageTypes.MsgRuleUnchecked, UpdateCheck);
            }
            RefreshList();
        }

        public CRuleItemViewData SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (value != _selectedItem)
                {
                    _selectedItem = value;
                    OnPropertyChanged();
                }
            }
        }

        public CRulesListViewData ViewData
        {
            get => _viewData;
            set
            {
                if (value != _viewData)
                {
                    _viewData = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> LastEvents { get; set; }
        

        public ICommand CloseCommand => new CRelayCommand(a => RunCloseCommand(), f => true);

        public ICommand AddNewRuleCommand => new CRelayCommand(a => AddRule(), f => true);

        public ICommand EditRuleCommand => new CRelayCommand(a => EditRule(), f => true);

        public CRelayCommand ActivateRulesCommand => new CRelayCommand(a => ActivateSelectedRules(), f => true);

        public CRelayCommand PauseRulesCommand => new CRelayCommand(a => PauseSelectedRules(), f => true);

        public CRelayCommand DeleteRulesCommand => new CRelayCommand(a => DeleteSelectedRules(), f => CanDelete());

        public CRelayCommand StartWatcherCommand => new CRelayCommand(a => StartWatcher(), f => CanStart());

        public CRelayCommand LoadRulesCommand => new CRelayCommand(a => LoadRulesFromFile(), f => true);

        private bool CanStart() => _viewData.RuleItems.Count > 0;

        private void StartWatcher()
        {
            var watcher = _controller.StartWatcher();
            watcher.OnStoredEventRecord += (s, arg) =>
            {
                Application.Current.Dispatcher?.BeginInvoke(new Action(() =>
                {
                    if (LastEvents.Count >= _lastEventsLimit)
                        LastEvents.RemoveAt(0);
                    LastEvents.Add($"{arg.EventInfo.TimeCreatedUtc} [{arg.EventInfo.FileEvent}] {arg.EventInfo.ObjectName}");
                }));
                
            };
            ViewData.EnableStart = false;
        }

        private bool CanDelete() => _viewData.IsAnyChecked;

        private void DeleteSelectedRules()
        {
            _controller.DeleteCheckedRules(ViewData);
        }

        private void AddRule()
        {
            _controller.AddNewRule(_view);
        }

        private void RefreshList()
        {
            ViewData = _controller.GetRulesListViewData();
        }

        private void ActivateSelectedRules()
        {
            _controller.ChangeStateCheckedRules(ViewData, ERuleState.Active);
        }

        private void PauseSelectedRules()
        {
            _controller.ChangeStateCheckedRules(ViewData, ERuleState.Paused);
        }

        private void EditRule()
        {
            _controller.RuleSelectedForEdit(this.SelectedItem, _view);
        }

        private void RunCloseCommand()
        {
            _controller.Close();
            Application.Current.Shutdown();
        }

        private void UpdateCheck()
        {
            _viewData.IsAnyChecked = _viewData.RuleItems.Any(i => i.IsChecked);
        }

        private void LoadRulesFromFile()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = false,
                InitialDirectory = Environment.CurrentDirectory ,
                Multiselect = true,
                Filters = { new CommonFileDialogFilter("JSON Files", "*.json") }
            };
            if (dialog.ShowDialog(_view) == CommonFileDialogResult.Ok)
            {
                List<string> dataFiles = new List<string>();
                foreach (string file in dialog.FileNames)
                {
                    dataFiles.Add(File.ReadAllText(file));
                }

                var loadedRules = DeserializeListOfRules(dataFiles);
                if (!loadedRules.Any())
                {
                    MessageBox.Show("Тo rules were loaded. Make sure the files contain the correct data", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    foreach (CRule rule in DeserializeListOfRules(dataFiles))
                    {
                        _controller.SaveNewRule(rule);
                    }
                    RefreshList();
                }
            }
        }

        internal List<CRule> DeserializeListOfRules(List<string> jsonDataList)
        {
            List<CRule> rules = new List<CRule>();
            foreach (string jsonData in jsonDataList)
            {
                try
                {
                    var loadedRules = JsonConvert.DeserializeObject<List<CRule>>(jsonData);
                    rules = rules.Union(loadedRules).ToList();
                }
                catch (Exception ex)
                {
                  s_logger.Error(ex);
                }
            }
            return rules;
        }
    }
}
