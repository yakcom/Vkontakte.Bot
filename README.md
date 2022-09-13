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
### Keyboard generated from string with 3 major delimiters:
> Symbol [ ; ] separates the vertical lines of the buttons

> Symbol [ , ] separates buttons on a line

> Symbol [ / ] separates the text of the button and its characteristics

## Example regular keyboard
```c#
VkBot.Send(id, $"Example Text", "Button1Line1;Button1Line2,Button2Line2;Button1Line3,Button2Line3,Button3Line3");
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Buttons.png"/>

# Keyboard button characteristics

| Expression |  Button  |
| ---------- | -------- |
|     /D     | Default  |
|     /M     | Primary  |
|     /P     | Positive |
|     /N     | Negative |
|  /LOCATION | Location |

## Example keyboard with button characteristics
```c#
VkBot.Send(id, $"Test", "Primary/M;Default/D;Positive/P;Negative/N;/LOCATION");
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Buttons2.png"/>
