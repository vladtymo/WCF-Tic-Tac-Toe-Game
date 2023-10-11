using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Service
{
    // ServiceContract - позначає контракт сервіса,
    // який описує методи доступні клієнту
    // CallbackContract - встановлює інтерфейс методів клієнта
    [ServiceContract(CallbackContract = typeof(ICallback))]
    public interface IGameService
    {
        // OperationContract - позначає метод, який доступний 
        // для виклику зі сторони клієнта
        // IsOneWay - параметр, який помічає метод як односторонній,
        // коли клієнт не буде очікувати його завершення.
        // В такому випадку метод не повинен повертати значення
        [OperationContract(IsOneWay = true)]
        void Start(string login);               // метод, який відправляє запит на старт гри
        [OperationContract(IsOneWay = true)]
        void Move(string login, string coords); // метод, який повідомляє про хід гравця
        [OperationContract(IsOneWay = true)]
        void Restart();                         // метод, який відправляє запит на оновлення гри
    }

    // Інтерфейс, що описує методи двусторонньої передачі,
    // які реалізує клієнт та які може викликати сервіс
    public interface ICallback
    {
        [OperationContract(IsOneWay = true)]
        void StartGame(bool isX, string opponent);  // метод, який повідомляє клієнта
                                                    // про початок гри
        [OperationContract(IsOneWay = true)]
        void GetMove(string coords);                // метод, який повідомляє клієнта
                                                    // про хід противника
        [OperationContract(IsOneWay = true)]
        void RestartNotify();                       // метод, який повідомляє клієнта
                                                    // про запит на оновлення гри
    }

    // Клас, який зберігає необхідну інформацію про гравця
    class PlayerInfo
    {
        public string Login { get; set; }
        public bool IsX { get; set; }
        // посилання на callback для виклику методів гравця
        public ICallback Callback { get; set; }
    }

    // Клас, який зберігає інформацію про ігрову сесію
    class GameInfo
    {
        public PlayerInfo Player1 { get; set; }
        public PlayerInfo Player2 { get; set; }
    }
}
 