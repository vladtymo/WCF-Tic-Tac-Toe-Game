using DAL.Interfaces;
using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Service
{
    // ServiceBehavior - дозволяє налаштувати параметри роботи сервіса
    // InstanceContextMode - налаштовує режим створення екземплярів класа сервіса
    // - PerCall:    для кожного запиту клієнта створюється новий екземпляр.
    // - PerSession: новий екземпляр створюється для кожного нового сеансу клієнта
    //               та існує протягом часу існування цього сеансу (для цього потрібно прив'язка, що підтримує сеанси).
    // - Single:     всі запити клієнтів за час існування програми обробляються одним екземпляром.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GameService : IGameService
    {
        private static Random rand = new Random();

        // посилання на клас з репозиторіями таблиць БД
        private readonly IUnitOfWork repositories;

        // посилання на об'єкт поточної гри
        private GameInfo game;

        public GameService()
        {
            // створення екземплярів
            game = new GameInfo();
            repositories = new UnitOfWork(new DAL.Model1());
        }

        // повідомлення гравців про початок гри
        void NotifyPlayers(GameInfo game)
        {
            // викликаємо метод кожного гравця передаючи його символ та ім'я суперника
            game.Player1.Callback.StartGame(game.Player1.IsX, game.Player2.Login);
            game.Player2.Callback.StartGame(game.Player2.IsX, game.Player1.Login);
        }

        // збереження інформації про гру в БД
        private void AddNewGameInfo(GameInfo game)
        {
            repositories.GameRepository.Insert(new DAL.GameInfo()
            {
                Date = DateTime.Now,
                Player1 = game.Player1.Login,
                Player2 = game.Player2.Login
            });
            repositories.Save();
        }

        // метод, який реєструє нового гравця, якщо є вільне місце
        public void Start(string login)
        {
            // якщо перший гравець не встановлений
            if (game.Player1 == null)
            {
                // створюємо об'єкт нового гравця
                game.Player1 = new PlayerInfo()
                {
                    // встановлюємо логін
                    Login = login,
                    // генеруємо випадковим чином символ гравця
                    IsX = Convert.ToBoolean(rand.Next(2)),
                    // отримуємо екземпляр callback-а гравця,
                    // який дозволить викликати методи на клієні
                    Callback = OperationContext.Current.GetCallbackChannel<ICallback>()
                };
            }
            // якщо другий гравець не встановлений
            else if (game.Player2 == null)
            {
                game.Player2 = new PlayerInfo()
                {
                    Login = login,
                    // символ гравця встановлюється протилежним супернику
                    IsX = !game.Player1.IsX, 
                    Callback = OperationContext.Current.GetCallbackChannel<ICallback>()
                };

                // повідомляємо гравців про початок гри
                NotifyPlayers(game);
                // додається інформацію про гру в БД
                AddNewGameInfo(game);
            }
        }

        // метод, який приймає хід гравця та повідомляє про нього суперника
        public void Move(string login, string coords)
        {
            if (game.Player1.Login == login)
                game.Player2.Callback.GetMove(coords);

            else if (game.Player2.Login == login)
                game.Player1.Callback.GetMove(coords);
        }

        // метод, який повідомляє гравців про запит на оновленя гри
        public void Restart()
        {
            game.Player1.Callback.RestartNotify();
            game.Player2.Callback.RestartNotify();
            // очищення поточної гри
            game = new GameInfo();
        }
    }
}
