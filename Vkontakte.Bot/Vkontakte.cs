using VkNet;
using VkNet.Model;
using VkNet.Model.Keyboard;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

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

        //-------------------
        private VkApi VkApi;
        private bool Break;
        private bool Busy;
        //-------------------

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

            Enable = true;
            Task.Run(() =>
            {
                while (true)
                {
                    List<Message> UnreadMessages = VkApi.Messages.GetConversations(new GetConversationsParams { Filter = GetConversationFilter.Unread }).Items.Select(u => u.LastMessage).ToList();
                    foreach (Message Message in UnreadMessages)
                    {
                        if (Break) { Enable = Break = false; return; }
                        Handler(Message.FromId.Value, Message.Text);
                        VkApi.Messages.MarkAsRead(Message.FromId.ToString());
                    }
                }
            });
        }

        /// <summary>
        /// Stop bot handler
        /// </summary>
        public void Stop()
        {
            Break = true;
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
            while (Busy);Busy = true;
            if (!Authorized) throw new Exception("The current bot instance is not authorized !");

            KeyboardBuilder KeyboardBuilder = new KeyboardBuilder();
            if (Keyboard != null)
            {
                if (Inline) KeyboardBuilder.SetInline();
                if (!Inline && OneTime) KeyboardBuilder.SetOneTime();

                foreach (string ButtonLine in Keyboard.Split(ks.a))
                {
                    foreach (string Button in ButtonLine.Split(ks.b))
                    {
                        KeyboardButtonColor Color = KeyboardButtonColor.Default;
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
                            }
                        }
                        string TextButton = Button.Split(ks.c)[0];
                        if (TextButton.Length == 0) TextButton = "null";
                        KeyboardBuilder.AddButton(TextButton, null, Color);
                        if (Text == "")throw new Exception("Text cannot be empty !");
                    }
                    KeyboardBuilder.AddLine();
                }
            }
            VkApi.Messages.Send(new MessagesSendParams { RandomId = new Random().Next(), UserId = Id, Message = Text, Keyboard = KeyboardBuilder.Build() });
            Busy = false;
        }


        /// <summary>
        /// Symbols used to create a keyboard
        /// </summary>
        /// <param name="a">Button lines splitter</param>
        /// <param name="b">Button columns splitter</param>
        /// <param name="c">Button parameter splitter</param>
        public void SetKeyboardSplitters(char a, char b, char c) { ks = (a, b, c); }
        private (char a, char b, char c) ks = (';', ',', '/');

    }
}