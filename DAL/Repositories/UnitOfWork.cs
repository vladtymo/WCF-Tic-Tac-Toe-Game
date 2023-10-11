using DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    // UnitOfWork - клас конкретного контексту бази даних,
    // який інкапсулює репозиторії кожної таблиці
    // та реалізує lazy loading для кожної
    public class UnitOfWork : IUnitOfWork
    {
        private Model1 context;
        private GenericRepository<GameInfo> gameRepository;

        public UnitOfWork(Model1 context)
        {
            this.context = context;
        }

        public IRepository<GameInfo> GameRepository
        {
            get
            {
                // lazy loading - екземпляр об'єкта створюється лише в момент
                // коли відбувається до нього доступ
                if (gameRepository == null)
                    gameRepository = new GenericRepository<GameInfo>(context);
                return gameRepository;
            }
        }
        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
