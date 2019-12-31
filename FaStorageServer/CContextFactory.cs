using NLog;
using System;
using System.Data.Entity;

namespace FaStorageServer
{
    public class CContextFactory
    {
        private static readonly Logger s_logger = LogManager.GetCurrentClassLogger();

        private Type _dbContextType;
        private DbContext _dbContext;

        public void Register<TDbContext>(TDbContext dbContext) where TDbContext : DbContext, IFaContext, new()
        {
            _dbContextType = typeof(TDbContext);
            _dbContext = dbContext;
        }

        public void Register<TDbContext>() where TDbContext : DbContext, new()
        {
            _dbContextType = typeof(TDbContext);
        }

        public TDbContext Get<TDbContext>() where TDbContext : DbContext, IFaContext, new()
        {
            try
            {
                if (_dbContext == null || _dbContextType != typeof(TDbContext))
                {
                    return new TDbContext();
                }
                return (TDbContext)_dbContext;
            }
            catch(Exception ex)
            {
                s_logger.Error(ex);
                throw;
            }
        }

        public TDbContext Get<TDbContext>(string connectionString) where TDbContext : DbContext, new()
        {
            return new TDbContext();
        }
    }
}
