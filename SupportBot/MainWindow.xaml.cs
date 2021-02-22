using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using Telegram.Bot;

namespace SupportBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// 
    /// Bot, which works like support bot. 
    /// Admin is able to see every person, who wrote to bot and can give them an answer.
    /// All message history is saved in log file for each user and downloads every time, so that admin can see all message history even if user has deleted chat.
    /// 
    /// </summary>
    
    public partial class MainWindow : Window
    {
        ObservableCollection<TelegramUser> users;
        TelegramBotClient botClient;
        private static bool firstEntrance;

        public MainWindow()
        {
            InitializeComponent();
            InitBot();
            firstEntrance = true;
        }

        private void InitBot()
        {
            users = new ObservableCollection<TelegramUser>();
            usersList.ItemsSource = users;

            string token = "1657205115:AAHGgY_DTgBFC_j2_fI_igk_q-o8ZKKtqE0";

            botClient = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(5) };
            botClient.OnMessage += botClient_OnMessage;
            botClient.StartReceiving();
            btnSend.Click += SendMessage;
        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            var concreteUser = users[users.IndexOf(usersList.SelectedItem as TelegramUser)];

            string responseMsg = $"{DateTime.Now} - Support: {textToSend.Text}";
            concreteUser.AddMessage(responseMsg);

            await botClient.SendTextMessageAsync(concreteUser.ChatId, textToSend.Text);

            File.AppendAllText(@$"logs\\{concreteUser.NickName}.log", $"{responseMsg}\n");

            textToSend.Text = string.Empty;
        }

        private void botClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string message = $"{DateTime.Now} - {e.Message.Chat.FirstName}: {e.Message.Text}";
            string[] messagesArr = new string[] { };

            if (firstEntrance && File.Exists(@$"logs\\{e.Message.Chat.FirstName}.log"))
            {
                messagesArr = File.ReadAllLines(@$"logs\\{e.Message.Chat.FirstName}.log");
            }

            this.Dispatcher.Invoke(() =>
            {
                var person = new TelegramUser(e.Message.Chat.FirstName, e.Message.Chat.Id);

                if (!users.Contains(person))
                    users.Add(person);

                if (firstEntrance)
                {
                    foreach (var item in messagesArr)
                    {
                        users[users.IndexOf(person)].AddMessage(item);
                    }

                    firstEntrance = false;
                }

                users[users.IndexOf(person)].AddMessage(message);
            });

            File.AppendAllText(@$"logs\\{e.Message.Chat.FirstName}.log", $"{message}\n");
        }
    }
}
