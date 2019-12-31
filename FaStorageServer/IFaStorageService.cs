using Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ServiceModel;

namespace FaStorageServer
{
    [ServiceContract]
    public interface IFaStorageService
    {
        [OperationContract]
        CClientInfo RegisterClient(string computerName, string ipAddress);

        [OperationContract]
        CEventInfo FindEventInfoById(Guid eventInfoId);

        [OperationContract]
        List<CRule> FindAllRules();

        [OperationContract]
        List<CRule> FindRulesByClientId(Guid clientId);

        [OperationContract]
        CRule FindRuleById(Guid ruleId);

        [OperationContract]
        List<CClientInfo> FindClientsWithLastDate();

        [OperationContract]
        CClientInfo FindClientInfoById(Guid clientInfoId);

        [OperationContract]
        List<CClientInfo> FindClientInfo(string computerName, string ipAddress);

        [OperationContract]
        bool WriteEventInfo(CEventInfo eventInfo);

        [OperationContract]
        bool SendNotification(CEventInfo eventInfo, CRule rule);

        [OperationContract]
        bool ExportEventsToCsv(List<CEventInfo> events, string path);

        [OperationContract]
        void DeleteRule(Guid ruleId);

        [OperationContract]
        void InsertRule(CRule rule);

        [OperationContract]
        void UpdateRule(CRule rule);

        [OperationContract]
        void UpdateClient(CClientInfo client);

        [OperationContract]
        ReadOnlyCollection<CEventInfo> SearchEvents(CSearchFilter filter);
    }
}
