using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Models;
using System;

namespace FaClient.ViewModels
{
    public class CRuleViewData : CBaseObservableObject
    {
        private Guid _ruleId;
        private string _folder;
        private bool _includeSubfolders;
        private string _email;
        private bool _notify;
        private string _masksToInclude;
        private string _masksToExclude;
        private EFileEvents _fileEvents;
        private string _state;
        private bool _isNew;

        public Guid RuleId
        {
            get => _ruleId;
            set
            {
                _ruleId = value;
                OnPropertyChanged();
            }
        }

        public string Folder
        {
            get => _folder;
            set
            {
                _folder = value;
                OnPropertyChanged();
            }
        }

        public bool IncludeSubfolders
        {
            get => _includeSubfolders;
            set
            {
                _includeSubfolders = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => _email;
            set
            {
               _email = value;
                OnPropertyChanged();
            }
        }

        public bool Notify
        {
            get => _notify;
            set
            {
                _notify = value;
                OnPropertyChanged();
            }
        }

        public string MasksInclude
        {
            get => _masksToInclude;
            set
            {
                _masksToInclude = value;
                OnPropertyChanged();
            }
        }

        public string MasksExclude
        {
            get => _masksToExclude;
            set
            {
                _masksToExclude = value;
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

        public string State
        {
            get => _state;
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public bool IsNew
        {
            get => _isNew;
            set
            {
                _isNew = value;
                OnPropertyChanged();
            }
        }
    }
}
