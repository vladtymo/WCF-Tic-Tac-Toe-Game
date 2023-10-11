using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI.GameServiceReference;

namespace UI
{
    // Реалізація методів зворотнього виклику, які буде викликати сервіс
    public class CallbackHandler : IGameServiceCallback
    {
        // Так як даний клас немає доступу до UI елементів,
        // були використані події, які повідомляють про виклик певного
        // метода та передаючи всі параметри обробникам подій.
        // Обробниками цих подій будуть методи класа MainWindow,
        // які мають доступ до UI елементів
        public event Action<bool, string> StartGameEvent;
        public event Action<string> MoveEvent;
        public event Action RestartEvent;

        // метод, який спрацьовує, коли противник виконав хід
        public void GetMove(string coords)
        {
            // викликається відповідна подія
            MoveEvent?.Invoke(coords);
        }
        // метод, який спрацьовує, коли прийшов запит на оновлення гри
        public void RestartNotify()
        {
            RestartEvent?.Invoke();
        }
        // метод, який спрацьовує, коли гра почалася
        public void StartGame(bool isX, string opponent)
        {
            StartGameEvent?.Invoke(isX, opponent);
        }
    }
   
    public partial class MainWindow : Window
    {
        // посилання на клас клієнта для доступа до сервіса
        GameServiceClient gameClient;

        // параметри необхідні для процесу гри
        private bool isX;
        private string opponentLogin;
        private string login;
        private bool isNext;
        
        // властивість, яка відповідає за можливість гравцю виконати хід
        public bool IsNext
        {
            get { return isNext; }
            private set
            {
                isNext = value;
                // при зміні параметра IsNext, змінюється колір клітинок ігрового поля
                Brush brush = value ? Brushes.White : Brushes.Gray;
                foreach (Border item in fieldGrid.Children.OfType<Border>())
                {
                    item.Background = brush;
                }   
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            // створення екземпляра Callback класа
            CallbackHandler callbackHandler = new CallbackHandler();

            // встановлення обробників на події, які викликає callback клас
            callbackHandler.StartGameEvent += StartGame;      // обробник початку гри
            callbackHandler.MoveEvent += SetMove;             // обробник ходу противника
            callbackHandler.RestartEvent += ClearCurrentGame; // обробник запита на оновлення гри

            // створення екземпляра клієнта для доступа до сервіса
            gameClient = new GameServiceClient(new InstanceContext(callbackHandler));

            // при запуску виконувати хід заборонено
            IsNext = false;
        }

        // метод, який обробляє хід противника
        private void SetMove(string coords)
        {
            // шукаємо клітинку, куди був здійснений хід
            foreach (Border b in fieldGrid.Children.OfType<Border>())
            {
                if (b.Tag.ToString() == coords)
                {
                    // встановлюємо символ противника
                    (b.Child as TextBlock).Text = this.isX ? "O" : "X";
                    // дозволяємо гравцю виконувати хід
                    IsNext = true;
                    break;
                }
            }
        }

        // метод, який встановлює параметри для початку гри
        private void StartGame(bool isX, string opponent)
        {
            this.isX = isX;
            this.opponentLogin = opponent;

            // дозволяється виконувати хід, якщо символ гравця - X
            IsNext = isX;

            symbolLabel.Content = isX ? "X" : "O";
            opponentNameTxtBox.Content = opponent;
        }

        // обробка нажаття на кнопку Start
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // встановлення логіну гравця
            login = nameTxtBox.Text;
            // запит на сервіс про початок гри
            gameClient.Start(login);
        }

        // обробка ходу гравця
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // якщо заборонено виконувати хід
            if (!IsNext) return;

            // встановлюємо символ у відповідну клітинку поля
            Border b = (sender as Border);
            (b.Child as TextBlock).Text = isX ? "X" : "O";

            // отримуємо координати клітинки
            string coords = b.Tag.ToString();
            // відправляємо інформацію про хід на сервіс
            gameClient.Move(login, coords);

            // забороняємо виконувати хід
            IsNext = false;
        }

        // метод, який оновлює гру
        private void ClearCurrentGame()
        {
            // заборона виконувати хід
            IsNext = false;

            // очищення всіх клітинок поля
            foreach (Border item in fieldGrid.Children.OfType<Border>())
            {
                (item.Child as TextBlock).Text = "";
            }

            // очищення параметрів гри
            login = "";
            opponentLogin = "";

            symbolLabel.Content = "";
            opponentNameTxtBox.Content = "";
        }

        // метод, який виконує запит на сервіс про оновлення гри 
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            gameClient.Restart();
        }
    }
}
