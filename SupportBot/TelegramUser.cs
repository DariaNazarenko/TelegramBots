using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace SupportBot
{
    // Model for telegram user
    class TelegramUser : IEquatable<TelegramUser>, INotifyPropertyChanged
    {
        private string nickName;
        private long chatId;
        public ObservableCollection<string> messages { get; set; }//to keep all messages for chat

        public TelegramUser(string nickName, long chatId)
        {
            this.nickName = nickName;
            this.chatId = chatId;
            messages = new ObservableCollection<string>();
        }

        public string NickName
        {
            get { return nickName; }
            set
            {
                nickName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.nickName)));
            }
        }

        public long ChatId
        {
            get { return chatId; }
            set
            {
                chatId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.chatId)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Equals(TelegramUser other) => other.ChatId == this.ChatId;

        public void AddMessage(string text) => messages.Add(text);
    }
}
