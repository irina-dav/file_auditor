using FaClient.FaStorageService;
using FaClient.ViewModels;
using Infrastructure;
using Infrastructure.Messenger;
using Infrastructure.Models;
using NLog;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace FaClient.Controllers
{
    class CRuleController : IRulesController
    {
        private readonly CClientInfo _client;
        private readonly IFaStorageService _storageService;
        private CBackgroundWorker _worker;

        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        public CRuleController(
            IFaStorageService storageService,
            CClientInfo client)
        {
            _storageService = storageService;
            _client = client;
        }

        public CMediator Mediator => CMediator.Instance;

        public void Start()
        {
            ShowViewRulesList();
        }

        public void EditRule(Guid ruleId, Window baseView)
        {
            RuleSettingsWindow view = GetRuleEditView(ruleId);
            view.Owner = baseView;
            view.ShowDialog();
        }

        public void AddNewRule(Window baseView)
        {
            RuleSettingsWindow view = GetRuleNewView();
            view.Owner = baseView;
            view.ShowDialog();
        }

        public void SaveNewRule(CRule rule)
        {
            rule.ClientInfoId = _client.ClientInfoId;
            rule.State = ERuleState.Paused;
            _storageService.InsertRule(rule);
        }

        private void ShowViewRulesList()
        {
            RulesListView view = GetRulesListView();
            view.Show();
        }

        private RulesListView GetRulesListView()
        {
            RulesListView view = new RulesListView();
            CClientViewModel viewModel = new CClientViewModel(this, view);
            return view;
        }

        private RuleSettingsWindow GetRuleEditView(Guid ruleId)
        {
            RuleSettingsWindow view = new RuleSettingsWindow();
            CRuleViewData viewData = GetRuleViewData(ruleId);
            CRuleViewModel viewModel = new CRuleViewModel(this, view, viewData);
            return view;
        }

        private RuleSettingsWindow GetRuleNewView()
        {
            RuleSettingsWindow view = new RuleSettingsWindow();
            CRuleViewData viewData = GetRuleNewViewData();
            CRuleViewModel viewModel = new CRuleViewModel(this, view, viewData);
            return view;
        }

        public CRuleViewData GetRuleViewData(Guid ruleId)
        {
            var rule = _storageService.FindRuleById(ruleId);
            return new CRuleViewData()
            {
                RuleId = rule.RuleId,
                Folder = rule.Folder,
                IncludeSubfolders = rule.IncludeSubfolders,
                Email = rule.Email,
                Notify = rule.Notify,
                MasksInclude = rule.MasksInclude,
                MasksExclude = rule.MasksExclude,
                FileEvents = rule.FileEvents,
                State = rule.State.ToString()
            };
        }

        public CRuleViewData GetRuleNewViewData()
        {
            CRuleViewData newRule = new CRuleViewData {IsNew = true};
            return newRule;
        }

        public void UpdateRule(CRuleViewData data)
        {
            CRule item = new CRule
            {
                RuleId = data.RuleId,
                Folder = data.Folder,
                IncludeSubfolders = data.IncludeSubfolders,
                Email = data.Email,
                Notify = data.Notify,
                MasksInclude = data.MasksInclude,
                MasksExclude = data.MasksExclude,
                State = string.IsNullOrEmpty(data.State)
                    ? default(ERuleState)
                    : (ERuleState) Enum.Parse(typeof(ERuleState), data.State),
                FileEvents = data.FileEvents,
                ClientInfoId = _client.ClientInfoId
            };
            _storageService.UpdateRule(item);
            Mediator.NotifyColleagues(EMessageTypes.MsgRuleSaved, data);
        }

        public void CreateRule(CRuleViewData data)
        {
            CRule item = new CRule
            {
                Folder = data.Folder,
                IncludeSubfolders = data.IncludeSubfolders,
                Email = data.Email,
                Notify = data.Notify,
                MasksInclude = data.MasksInclude,
                MasksExclude = data.MasksExclude,
                State = string.IsNullOrEmpty(data.State)
                    ? default(ERuleState)
                    : (ERuleState)Enum.Parse(typeof(ERuleState), data.State),
                FileEvents = data.FileEvents,
                ClientInfoId = _client.ClientInfoId
            };
            _storageService.InsertRule(item);
            Mediator.NotifyColleagues(EMessageTypes.MsgRuleSaved, data);
        }

        public void ChangeStateCheckedRules(CRulesListViewData data, ERuleState newState)
        {
            foreach (var item in data.CheckedRuleItems)
            {
                item.Rule.State = newState;
                _storageService.UpdateRule(item.Rule);
                Mediator.NotifyColleagues(EMessageTypes.MsgRuleSaved, data);
            }
        }

        public void DeleteCheckedRules(CRulesListViewData data)
        {
            foreach (var item in data.CheckedRuleItems)
            {
                _storageService.DeleteRule(item.RuleId);
                Mediator.NotifyColleagues(EMessageTypes.MsgRuleDeleted, data);
            }
        }

        public CRulesListViewData GetRulesListViewData()
        {
            var ruleItems = new ObservableCollection<CRuleItemViewData>();
            var rules = _storageService.FindRulesByClientId(_client.ClientInfoId);
            foreach (var rule in rules)
            {
                ruleItems.Add(new CRuleItemViewData(this, rule));
            }
            CRulesListViewData vd = new CRulesListViewData(ruleItems);
            return vd;
        }

        public void RuleSelectedForEdit(CRuleItemViewData data, Window baseVm)
        {
            if (data?.Rule != null)
            {
                EditRule(data.Rule.RuleId, baseVm);
            }
        }

        // TODO: add feature to stop watching, re-launch
        public CBackgroundWorker StartWatcher()
        {
            try
            {
                _worker = new CBackgroundWorker(_storageService, _client);
                _worker.Start();
                _client.State = EClientState.Active;
                _storageService.UpdateClient(_client);
                return _worker;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }

        public void Close()
        {
            if (_worker != null)
            {
                try
                {
                    _worker.Stop();
                    _worker.Dispose();
                }
                catch (Exception ex)
                {
                    s_logger.Error(ex);
                    throw;
                }
            }
        }
    }
}
