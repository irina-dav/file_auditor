using Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Models;

namespace FaClient.ViewModels
{
    public class CRulesListViewData : CBaseObservableObject
    {
        private ObservableCollection<CRuleItemViewData> _rulesItems;

        private bool _isAnyChecked;

        private bool _enableStart = true;

        public CRulesListViewData(ObservableCollection<CRuleItemViewData> ruleItems)
        {
            _rulesItems = ruleItems;
        }

        public ObservableCollection<CRuleItemViewData> RuleItems
        {
            get => _rulesItems;
            set
            {
                if (value != _rulesItems)
                {
                    _rulesItems = value;
                    OnPropertyChanged();
                }
            }
        }

        public IReadOnlyCollection<CRuleItemViewData> CheckedRuleItems => _rulesItems.Where(i => i.IsChecked).ToList();

        public bool IsAnyChecked
        {
            get => _isAnyChecked;
            set
            {
                _isAnyChecked = value;
                OnPropertyChanged();
            }
        }

        public bool IsAnyActive => _rulesItems.Any(i => i.Rule.State == ERuleState.Active);

        public bool EnableStart
        {
            get => _enableStart;
            set
            {
                _enableStart = value;
                OnPropertyChanged();
            }
        }
    }
}
