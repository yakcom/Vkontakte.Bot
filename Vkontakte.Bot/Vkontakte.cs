using VkNet;
using VkNet.Model;
using Newtonsoft.Json;
using VkNet.Model.Keyboard;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace Vkontakte.Bot
{
    public class VkontakteBot
    {
        /// <summary>
        /// VkApi object use to expand functionality
        /// See more https://vknet.github.io/vk/
        /// </summary>
        public VkApi VkApi;

        /// <summary>
        /// Is bot successfully authorized
        /// </summary>
        public bool Authorized { get; private set; }

        /// <summary>
        /// Is bot working
        /// </summary>
        public bool Enable { get; private set; }

        /// <summary>
        /// Authorization in the VKontakte group
        /// </summary>
        /// <param name="Token">VKontakte group api token</param>
        /// <returns></returns>
        public bool Authorization(string Token)
        {
            VkApi = new VkApi();
            VkApi.Authorize(new ApiAuthParams { AccessToken = Token });
            try
            {
                VkApi.Groups.GetById(null, null, null);
                return Authorized = true;
            }
            catch
            {
                return Authorized = false;
            }
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
                    Thread.Sleep(100);
                    foreach (var Message in GetMessages())
                    {
                        if (!Enable)return;
                        Handler(Message.Key, Message.Value);
                        MarkAsRead(Message.Key);
                    }
                }
            });
        }

        /// <summary>
        /// Stop bot handler
        /// </summary>
        public void Stop()=>Enable = false;

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

            KeyboardBuilder KeyboardBuilder = new KeyboardBuilder();
            KeyboardButtonColor Color = KeyboardButtonColor.Default;

            if (Keyboard != null)
            {
                if (Inline) KeyboardBuilder.SetInline();
                if (!Inline && OneTime) KeyboardBuilder.SetOneTime();

                foreach (string ButtonLine in Keyboard.Split(ks.a))
                {
                    foreach (string Button in ButtonLine.Split(ks.b))
                    {
                        string TextButton = Button.Split(ks.c)[0];
                        if (Button.Split(ks.c).Length > 1)
                        {
                            switch (Button.Split(ks.c)[1])
                            {
                                case "D":
                                    Color = KeyboardButtonColor.Default;
                                    break;
                                case "M":
                                    Color = KeyboardButtonColor.Primary;
                                    break;
                                case "P":
                                    Color = KeyboardButtonColor.Positive;
                                    break;
                                case "N":
                                    Color = KeyboardButtonColor.Negative;
                                    break;
                                case "LOCATION":
                                    KeyboardBuilder.AddButton(new AddButtonParams { ActionType = KeyboardButtonActionType.Location });
                                    continue;
                                case "URL":
                                    KeyboardBuilder.AddButton(new AddButtonParams {Label=TextButton, ActionType = KeyboardButtonActionType.OpenLink, Link = Button.Split(ks.c)[2] });
                                    continue;
                            }
                        }
                        Text = Text!="" ? Text : "null";
                        TextButton = TextButton != "" ? TextButton : "null";
                        KeyboardBuilder.AddButton(TextButton, null, Color);
                    }
                    KeyboardBuilder.AddLine();
                }
            }
            try {VkApi.Messages.Send(new MessagesSendParams { RandomId = new Random().Next(), UserId = Id, Message = Text, Keyboard = KeyboardBuilder.Build() }); } catch { }
        }


        /// <summary>
        /// Symbols used to create a keyboard
        /// </summary>
        /// <param name="a">Button lines splitter</param>
        /// <param name="b">Button columns splitter</param>
        /// <param name="c">Button parameter splitter</param>
        public void SetKeyboardSplitters(char a, char b, char c) { ks = (a, b, c); }
        private (char a, char b, char c) ks = (';', ',', '|');

        private void MarkAsRead(long id) 
        { 
            Request($"https://api.vk.com/method/messages.markAsRead?peer_id={id}&v=5.131&access_token={VkApi.Token}"); 
        }
        private Dictionary<long, string> GetMessages()
        {
            Dictionary<long, string> Messages = new Dictionary<long, string>();
            string Result = Request($"https://api.vk.com/method/messages.getConversations?filter=unread&extended=0&v=5.131&access_token={VkApi.Token}");
            if (Result == null) return Messages;
            List<dynamic> MessageData = JsonConvert.DeserializeObject<List<dynamic>>(((dynamic)JsonConvert.DeserializeObject(Result))["response"]["items"].ToString());
            foreach (dynamic Message in MessageData)
                Messages.Add(long.Parse(Message["last_message"]["from_id"].ToString()), Message["last_message"]["text"].ToString());
            return Messages;
        }
        private string Request(string url)
        {
            try{
                using (HttpClient Web = new HttpClient())
                using (HttpResponseMessage Res = Web.GetAsync(url).Result)
                using (HttpContent Content = Res.Content)
                    return Content.ReadAsStringAsync().Result;
            }catch { return null; }
        }
    }
}