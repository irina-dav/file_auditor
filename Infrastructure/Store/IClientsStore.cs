using Infrastructure.Models;
using System;
using System.Collections.Generic;

namespace Infrastructure.Store
{
    public interface IClientsStore
    {
        List<CClientInfo> FindClientsWithLastDate();

        CClientInfo FindClientInfoById(Guid clientInfoId);

        List<CClientInfo> FindClientInfo(string computerName, string ipAddress);

        CClientInfo RegisterClient(string computerName, string ipAddress);

        void UpdateClient(CClientInfo client);
    }
}
