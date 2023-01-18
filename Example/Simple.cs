using Vkontakte.Bot;

namespace Example
{
    internal class Simple
    {
        static VkontakteBot VkBot;
        static void Main(string[] args)
        {
            VkBot = new VkontakteBot();

            VkBot.Authorization("9e015e867f3832f758d84e8c3dfcea55568222be73cfea42fcfa041d095c604c117b48372866bfa239960");
            VkBot.Start(Handle);

            Console.ReadLine();
        }

        static void Handle(long id, string text)
        {
            //VkBot.Send(id, $"Hello Id: {id}\nYou Say: {text}");
            //VkBot.Send(id, $"Test Inline Keyboard", "PositiveButton/P,NegativeButton/N;DefaultButton/D,PrimaryButton/M", true);
            //VkBot.Send(id, $"Test Outline Keyboard", "Yes/P,No/N;/LOCATION");
        }

    }
}