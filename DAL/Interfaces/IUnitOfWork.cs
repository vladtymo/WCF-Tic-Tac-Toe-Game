using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    // IUnitOfWork - інтерфейс класа, який об'єднує репозиторії для всіх таблиць
    // також може містити окремий метод Save, який зберігає зміни в базі даних
    public interface IUnitOfWork : IDisposable
    {
        // репозиторій таблиці GameInfo
        IRepository<GameInfo> GameRepository { get; }
        // метод для збереження змін в базі даних
        void Save();
    }
}
