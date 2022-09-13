using Vkontakte.Bot;

namespace Example
{
    internal class Simple
    {
        static VkontakteBot VkBot;
        static void Main(string[] args)
        {

            VkBot = new VkontakteBot();
            VkBot.Authorization("YOUR_VK_GROUP_API_TOKENN");
            VkBot.Start(Handle);

            Console.ReadLine();
        }

        static void Handle(long id, string text)
        {
            VkBot.Send(id, $"Hello Id: {id}\nYou Say: {text}");
            VkBot.Send(id, $"Test Inline Keyboard", "PositiveButton/P,NegativeButton/N;DefaultButton/D,PrimaryButton/M", true);
            VkBot.Send(id, $"Test Outline Keyboard", "Yes/P,No/N;/LOCATION");
        }

    }
}