<p align="center"><img  width="200" src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Vk.png"/></p>
<h1 align="center">Vkontakte.Bot</h1>
<h3 align="center">Library shell <a href="https://github.com/vknet/vk" target="_blank">VkNet</a> for easy creation of chat bots Vkontakte</h3>

# Using
```c#
using Vkontakte.Bot;
```
# Quick Start
```c#
VkontakteBot VkBot;

void Main()
{
    VkBot = new VkontakteBot();
    VkBot.Authorization("YOUR_VK_GROUP_API_TOKEN");
    VkBot.Start(Handle);
}

void Handle(long id, string text)
{
    VkBot.Send(id, "Hello World");
}
```
# Console Example
```C#
using Vkontakte.Bot;

namespace Example
{
    internal class Simple
    {
        static VkontakteBot VkBot;
        static void Main(string[] args)
        {

            VkBot = new VkontakteBot();
            VkBot.Authorization("YOUR_VK_GROUP_API_TOKEN");
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
```
# Keyboard
### Keyboard generated from string

