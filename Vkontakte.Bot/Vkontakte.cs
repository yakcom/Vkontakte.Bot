using Flurl.Http;
using Newtonsoft.Json;

namespace Vkontakte.Bot
{
    public class VkontakteBot
    {
        /// <summary>
        /// Is bot working
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Is bot successfully authorized
        /// </summary>
        public bool Authorized { get; private set; }

        /// <summary>
        /// VKontakte group api token
        /// </summary>
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

        /// <summary>
        /// Send Message for user by id
        /// </summary>
        /// <param name="Id">User id for send message</param>
        /// <param name="Text">Text for Message</param>
        /// <param name="Keyboard">Regular expression generating keyboard</param>
        /// <param name="Inline">Set Inline keyboard</param>
        /// <param name="OneTime">Keyboard open only for one message</param>
        public void Send(long Id, string Text, string Keyboard = null, bool Inline = false, bool OneTime = false)
        {
            if (!Authorized) throw new Exception("The current bot instance is not authorized !");
            if (Text == "") throw new Exception("The message cannot be empty !");

            KeyboardBuilder KeyboardBuild = new KeyboardBuilder();

            if (Keyboard != null)
            {
                KeyboardBuild.inline = Inline;
                KeyboardBuild.one_time = OneTime & !Inline;

                foreach (string ButtonLine in Keyboard.Split(ks.a))
                {
                    List<Button> Line = new List<Button>();
                    foreach (string Button in ButtonLine.Split(ks.b))
                    {
                        string TextButton = Button.Split(ks.c)[0] != "" ? Button.Split(ks.c)[0] : throw new Exception("The button label cannot be empty !");
                        if (Button.Split(ks.c).Length > 1)
                        {
                            switch (Button.Split(ks.c)[1])
                            {
                                case "D": Line.Add(new ButtonText(TextButton, "secondary")); continue;
                                case "M": Line.Add(new ButtonText(TextButton, "primary")); continue;
                                case "P": Line.Add(new ButtonText(TextButton, "positive")); continue;
                                case "N": Line.Add(new ButtonText(TextButton, "negative")); continue;
                                case "LOCATION": Line.Add(new ButtonLocation()); continue;
                                case "URL": Line.Add(new ButtonLink(TextButton, Button.Split(ks.c)[2])); continue;
                            }
                        }
                        Line.Add(new ButtonText(TextButton));
                    }
                    KeyboardBuild.buttons.Add(Line);
                }
            }
            Request($"https://api.vk.com/method/messages.send?user_id={Id}&message={Text}&random_id={new Random().Next()}&keyboard={Uri.EscapeDataString(JsonConvert.SerializeObject(KeyboardBuild))}&v=5.131&access_token={Token}");
        }

        /// <summary>
        /// Symbols used to create a keyboard
        /// </summary>
        /// <param name="a">Button lines splitter</param>
        /// <param name="b">Button columns splitter</param>
        /// <param name="c">Button parameter splitter</param>
        public void SetKeyboardSplitters(char a, char b, char c) { ks = (a, b, c); }


        private Dictionary<long, string> GetMessages()
        {
            Dictionary<long, string> Messages = new Dictionary<long, string>();
            var json = Request($"https://api.vk.com/method/messages.getConversations?filter=unanswered&extended=0&v=5.131&access_token={Token}");
            if (json == null) return Messages;
            if (json["response"] == null) return Messages;
            List<dynamic> MessageData = JsonConvert.DeserializeObject<List<dynamic>>(json["response"]["items"].ToString());
            foreach (dynamic Message in MessageData)
                if (Message["last_message"]["out"].ToString() == "0")
                    Messages.Add(long.Parse(Message["last_message"]["from_id"].ToString()), Message["last_message"]["text"].ToString());
                else
                    MarkAsAnswered(long.Parse(Message["conversation"]["peer"]["id"].ToString()));     
            return Messages;
        }
        private void MarkAsAnswered(long id)
        {
            Request($"https://api.vk.com/method/messages.markAsAnsweredConversation?answered=1&peer_id={id}&v=5.131&access_token={Token}");
        }
        private dynamic Request(string Url) 
        { 
            try { return Url.GetJsonAsync<dynamic>().Result; } catch { return null; } 
        }

        #region KeyboardBuilder
        private (char a, char b, char c) ks = (';', ',', '|');
        private class KeyboardBuilder
        {
            public bool one_time = false;
            public bool inline = false;
            public List<List<Button>> buttons = new List<List<Button>>();
        }
        private class Button
        { 
            public Action action; 
        }
        private class ButtonText : Button
        {
            public ButtonText(string text, string color = "primary")
            {
                action = new Text(text);
                this.color = color;
            }
            public string color;
        }
        private class ButtonLocation : Button
        {
            public ButtonLocation()
            {
                action = new Location();
            }
        }
        private class ButtonLink : Button
        {
            public ButtonLink(string text,string url)
            {
                action = new Link(text, url);
            }
        }
        public class Action
        {}
        public class Text : Action
        { 
            public Text(string text)
            {
                label = text;
            }
            public string type = "text";
            public string label;
        }
        public class Location : Action
        {
            public string type = "location";
        }
        public class Link : Action
        {
            public Link(string text, string url)
            {
                label = text;
                link = url;
            }
            public string type = "open_link";
            public string link;
            public string label;
        }
        #endregion

    }
}