using Flurl.Http;
using Newtonsoft.Json;

namespace Vkontakte.Bot
{
    public class VkontakteBot
    {
        /// <summary>
        /// Is bot successfully authorized
        /// </summary>
        public bool Authorized { get; private set; }

        /// <summary>
        /// Is bot working
        /// </summary>
        public bool Enable { get; private set; }

        public string Token { get; private set; }

        /// <summary>
        /// Authorization in the VKontakte group
        /// </summary>
        /// <param name="Token">VKontakte group api token</param>
        /// <returns></returns>
        public bool Authorization(string Token)
        {
            var json = Request($"https://api.vk.com/method/messages.getConversations?filter=unanswered&extended=0&v=5.131&access_token={Token}");
            if (json == null) return false;
            if (json["response"] == null) return false;
            Authorized = true;
            this.Token = Token;
            return true;
        }

        /// <summary>
        /// Start bot handler
        /// </summary>
        /// <param name="Handler">Function to which received messages will be sent</param>
        public void Start(Action<long, string> Handler)
        {

            if (Enable) throw new Exception("The current bot instance is already running !"); ;
            if (!Authorized) throw new Exception("The current bot instance is not authorized !");

            Task.Run(() =>
            {
                Enable = true;
                while (Enable)
                {
                    Thread.Sleep(200);
                    foreach (var Message in GetMessages())
                    {
                        if (!Enable) return;
                        Handler(Message.Key, Message.Value);
                        MarkAsAnswered(Message.Key);
                    }
                }
            });
        }

        public void Send(long Id, string Text, string Keyboard = null, bool Inline = false, bool OneTime = false)
        {
            if (!Authorized) throw new Exception("The current bot instance is not authorized !");

            //Request($"https://api.vk.com/method/messages.send?user_id={Id}&message={Text}&random_id={}");
        }

            private Dictionary<long, string> GetMessages()
        {
            Dictionary<long, string> Messages = new Dictionary<long, string>();
            var json = Request($"https://api.vk.com/method/messages.getConversations?filter=unanswered&extended=0&v=5.131&access_token={Token}");
            if (json == null) return Messages;
            List<dynamic> MessageData = JsonConvert.DeserializeObject<List<dynamic>>(json["response"]["items"].ToString());
            foreach (dynamic Message in MessageData)
                Messages.Add(long.Parse(Message["last_message"]["from_id"].ToString()), Message["last_message"]["text"].ToString());
            return Messages;
        }
        private void MarkAsAnswered(long id)
        {
            Request($"https://api.vk.com/method/messages.markAsAnsweredConversation?answered=1&peer_id={id}&v=5.131&access_token={Token}");
        }
        
        private dynamic Request(string Url) { try { return Url.GetJsonAsync<dynamic>().Result; } catch { return null; } }

    }
}