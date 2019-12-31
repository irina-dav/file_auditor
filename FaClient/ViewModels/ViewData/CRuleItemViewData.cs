using FaClient.Controllers;
using Infrastructure;
using System;
using Infrastructure.Models;

namespace FaClient.ViewModels
{
    public class CRuleItemViewData : CBaseObservableObject
    {
        private bool _isChecked;

        private readonly IRulesController _rulesController;

        public CRuleItemViewData(IRulesController controller, CRule rule)
        {
            _rulesController = controller;
            Rule = rule;
        }

        public CRule Rule { get; }

        public Guid RuleId => Rule.RuleId;

        public string Folder => Rule.Folder;

        public string IncludeSubfolders => Rule.IncludeSubfolders ? "Yes" : "";

        public string Email => Rule.Email;

        public string Notify => Rule.Notify ? "Yes" : "";

        public string MasksInclude => Rule.MasksInclude?.Replace("\n", ", ");

        public string MasksExclude => Rule.MasksExclude?.Replace("\n", ", ");

        public string FileEvents => string.Join(", ", Rule.FileEvents);

        public string State
        {
            get => Rule.State.ToString();
            set
            {
                Rule.State = (ERuleState)Enum.Parse(typeof(ERuleState), value);
                OnPropertyChanged();
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                _isChecked = value;
                OnPropertyChanged();
                _rulesController.Mediator.NotifyColleagues(_isChecked ? EMessageTypes.MsgRuleChecked : EMessageTypes.MsgRuleUnchecked, null);
            }
        }
    }
}
