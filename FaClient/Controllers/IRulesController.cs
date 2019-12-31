using FaClient.ViewModels;
using Infrastructure;
using System.Windows;
using Infrastructure.Models;

namespace FaClient.Controllers
{
    public interface IRulesController : IController
    {
        CRulesListViewData GetRulesListViewData();

        void RuleSelectedForEdit(CRuleItemViewData data, Window baseWindow);

        void AddNewRule(Window parent);

        void SaveNewRule(CRule rule);

        void UpdateRule(CRuleViewData data);

        void CreateRule(CRuleViewData data);

        void ChangeStateCheckedRules(CRulesListViewData data, ERuleState newState);

        void DeleteCheckedRules(CRulesListViewData data);

        CBackgroundWorker StartWatcher();

        void Close();
    }
}
