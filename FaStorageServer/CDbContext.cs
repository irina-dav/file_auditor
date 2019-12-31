using Infrastructure;
using Infrastructure.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using NLog;

namespace FaStorageServer
{
    public class CDbContext : DbContext, IFaContext
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private static string s_connectionString;

        public static string ConnectionString
        {
            get
            {
                if (s_connectionString == null)
                {
                    if (SSettings.ConnectionStringDb != null)
                        s_connectionString = SSettings.ConnectionStringDb.ConnectionString;
                }

                return s_connectionString;
            }
            set => s_connectionString = value;
        }

        public CDbContext() : base(ConnectionString)
        {
        }

        public virtual DbSet<CRule> Rules { get; set; }

        public virtual DbSet<CEventInfo> Events { get; set; }

        public virtual DbSet<CClientInfo> Clients { get; set; }

        public override int SaveChanges()
        {
            int savedItems = 0;
            try
            {
                savedItems = base.SaveChanges();
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }

            return savedItems;
        }

        public bool TryUpdate<T>(T element) where T : class
        {
            try
            {
                Set<T>().Attach(element);
                DbEntityEntry dbEntry = Entry(element);
                dbEntry.CurrentValues.SetValues(element);
                dbEntry.State = EntityState.Modified;
                if (SaveChanges() == 0)
                    s_logger.Warn($"The number of updated elements<{typeof(T)}> is 0");
                return true;
            }
            catch (Exception ex)
            {
               s_logger.Error(ex);
                return false;
            }
        }

        public bool TryAdd<T>(T element) where T : class
        {
            try
            {
                Set<T>().Add(element);
                DbEntityEntry dbEntry = Entry(element);
                dbEntry.State = EntityState.Added;
                if (SaveChanges() == 0)
                    s_logger.Warn($"The number of added elements<{typeof(T)}> is 0");
                return true;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                return false;
            }
        }

        public bool TryExecute(string sqlCommand, object[] parameters)
        {
            try
            {
                this.Database.ExecuteSqlCommand(sqlCommand, parameters);
                return true;
            }
            catch (Exception ex)
            {
                s_logger.Error($"An error occurred during the execution {sqlCommand}: {ex}");
                return false;
            }
        }

        public bool TryCall<T>(Func<IFaContext, T> func, out T result)
        {
            try
            {
                result = func(this);
                return true;
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                result = default(T);
                return false;
            }
        }

        public void CallWithTransaction(Func<IFaContext, bool> func, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            try
            {
                using (var tx = this.Database.BeginTransaction(isolationLevel: isolationLevel))
                {
                    if (func(this))
                        tx.Commit();
                    else
                        tx.Rollback();
                }
            }
            catch (Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }
    }
}

