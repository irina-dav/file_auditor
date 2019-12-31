using FaClient.Controllers;
using Infrastructure;
using System.Windows;
using System.Windows.Input;

namespace FaClient.ViewModels
{
    public class CRuleViewModel : CBaseObservableObject
    {
        private CRuleViewData _viewData;
        private readonly Window _view;
        private readonly IRulesController _rulesController;

        public CRuleViewModel(IRulesController controller, Window view, CRuleViewData viewData)
        {
            view.DataContext = this;
            _rulesController = controller;
            _view = view;
            _viewData = viewData;
        }

        public CRuleViewData ViewData
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

        public ICommand SaveCommand => new CRelayCommand(a => RunSaveCommand(), f => true);

        public ICommand CloseCommand => new CRelayCommand(a => RunCloseCommand(), f => true);

        private void RunCloseCommand()
        {
            _view.Close();
        }

        private void RunSaveCommand()
        {
            if (_viewData.IsNew)
                _rulesController.CreateRule(ViewData);
            else
                _rulesController.UpdateRule(ViewData);
            _view.Close();
        }
    }
}
