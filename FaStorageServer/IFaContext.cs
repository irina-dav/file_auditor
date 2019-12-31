using Infrastructure.Models;
using System;
using System.Data;
using System.Data.Entity;

namespace FaStorageServer
{
    public interface IFaContext
    {
        DbSet<CClientInfo> Clients { get; set; }
        DbSet<CRule> Rules { get; set; }
        DbSet<CEventInfo> Events { get; set; }

        bool TryUpdate<T>(T entity) where T: class;

        bool TryAdd<T>(T entity) where T : class;

        bool TryExecute(string sqlCommand, object[] parameters);

        bool TryCall<T>(Func<IFaContext, T> func, out T result);

        void CallWithTransaction(Func<IFaContext, bool> func, IsolationLevel isolationLevel);

    }
}
