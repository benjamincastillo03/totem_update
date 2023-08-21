using System;
using System.Threading.Tasks;
using System.Windows.Forms;
//using WhatsAppApi;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;

namespace TOTEM_FARMACIA.controlador {
    internal class Whatsapp_NET {
        //private WhatsApp wa { get; set; }
        public Whatsapp_NET(){   }
        public async Task tele_Me(){
            //Console.WriteLine("tele_Me");
            var botClient = new TelegramBotClient("6198312858:AAH1BUAIPuIlRUQWcQ1kjVIL2O2sBl1pb0M");
            var me = await botClient.GetMeAsync();
            Console.WriteLine($"Hello, World! I am user {me.Id} and my name is {me.FirstName}.");
        }

        public async Task Envio_Mensaje(){
          
            var botClient   =   new TelegramBotClient("6198312858:AAH1BUAIPuIlRUQWcQ1kjVIL2O2sBl1pb0M");
            var chatId      =    new ChatId("583391555");
            var message     =   "Hola, este es un mensaje de prueba.";
            await botClient.SendTextMessageAsync(chatId, message);
            
        }

    }
}
