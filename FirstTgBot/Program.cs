using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace FirstTgBot
{
    /*
       Bot, which downloads stickers and photos and saves it to directories with Usernames. 
       If entered message is text - gets the answer on rus or eng, depending on entered language.
       Sends first message(welcoming) with photo.
    */
    class Program
    {
        private static ITelegramBotClient botClient;

        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("1684424143:AAGGgp58cKTZ8yZSa2ik_5cff595gnvrEJU") { Timeout = TimeSpan.FromSeconds(10) };

            botClient.OnMessage += BotClient_OnMessage;
            botClient.StartReceiving();

            Console.ReadLine();
        }

        private async static void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var recievedPhoto = e?.Message.Photo;
            var recievedSticker = e?.Message.Sticker;

            // check for stickers
            if (recievedSticker != null)
            {
                var username = e.Message.Chat.Username;

                if (!Directory.Exists(@$"recievedStickers\\username"))
                {
                    Directory.CreateDirectory(@$"recievedStickers\\{username}");
                }

                var file = botClient.GetFileAsync(e.Message.Sticker.FileId);
                var download_url = @"https://api.telegram.org/file/bot1684424143:AAGGgp58cKTZ8yZSa2ik_5cff595gnvrEJU/" + file.Result.FilePath;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(download_url), @$"recievedStickers\\{username}\\{e.Message.Chat.FirstName}-{new Random().Next(0, 9)}.png");
                }
            }

            // check for photos
            if (recievedPhoto != null)
            {
                var username = e.Message.Chat.Username;

                if(!Directory.Exists(@$"recievedPhotos\\username"))
                {
                    Directory.CreateDirectory(@$"recievedPhotos\\{username}");
                }

                var file = botClient.GetFileAsync(e.Message.Photo[e.Message.Photo.Count() - 1].FileId);
                var download_url = @"https://api.telegram.org/file/bot1684424143:AAGGgp58cKTZ8yZSa2ik_5cff595gnvrEJU/" + file.Result.FilePath;
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(new Uri(download_url), @$"recievedPhotos\\{username}\\{e.Message.Chat.FirstName}-{new Random().Next(0,9)}.png");
                }
            }

            // check for text
            var text = e?.Message?.Text;
            if (text == null)
            {
                return;
            }

            Console.WriteLine($"Recives text: {text} in chat {e.Message.Chat.Id}");

            var patternRus = new Regex(@"[а-яА-Я]");
            var patternEng = new Regex(@"[a-zA-Z]");
            var patternStart = new Regex(@"/start");

            if (patternStart.IsMatch(text))
            {
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Welcome to dashka_first_bot.\nRefer your friend and do a charity (for more information contact creator queen)").ConfigureAwait(false);

                var photoFileUrl = @"C:\\Users\\Daria\\Downloads\\Telegram Desktop\\photo_2021-02-16_09-57-16.jpg";
                using (var stream = File.Open(photoFileUrl, FileMode.Open))
                {
                    //var content = stream;
                    //var filename = photoFileUrl.Split('\\').Last();

                    //await botClient.SendPhotoAsync(e.Message.Chat, new Telegram.Bot.Types.InputFiles.InputOnlineFile(content, filename));
                    await botClient.SendPhotoAsync(e.Message.Chat, new Telegram.Bot.Types.InputFiles.InputOnlineFile(stream));
                }
            }
            else if (patternRus.IsMatch(text))
            {
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: $"Ты написал: \'{text}\'").ConfigureAwait(false);
            }
            else if (patternEng.IsMatch(text))
            {
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: $"You said: \'{text}\'").ConfigureAwait(false);
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: e.Message.Chat, text: $"There are no letters in line: \'{text}\'").ConfigureAwait(false);
            }
        }
    }
}
