<a href="https://github.com/yakcom/Vkontakte.Bot/releases/">
<p align="center"><img  width="200" src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Vk.png"/></p>
<h1 align="center">Vkontakte.Bot</h1></a>
<h3 align="center">Library shell <a href="https://github.com/vknet/vk" target="_blank">VkNet</a> for easy creation of group chat bots Vkontakte</h3><br>
<a href="https://www.nuget.org/packages/Vkontakte.Bot"><img src="https://readme-typing-svg.herokuapp.com?font=Fira+Code&size=25&pause=100000&duration=3000&color=4392E7&center=true&vCenter=true&width=1000&lines=Download+NuGet+Release" alt="Typing SVG" /></a>

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
            VkBot.Send(id, "Test Inline Keyboard", "PositiveButton/P,NegativeButton/N;DefaultButton/D,PrimaryButton/M", true);
            VkBot.Send(id, "Test Outline Keyboard", "Yes/P,No/N;/LOCATION");
        }

    }
}
```
<br><br><br>

# Keyboard general
### Keyboard generated from string with 3 main separators:
> Symbol [ ; ] separates the vertical lines of the buttons

> Symbol [ , ] separates buttons on a line

> Symbol [ / ] separates the text of the button and its options

## Example regular keyboard
```c#
VkBot.Send(id, "Example Text", "Button1Line1;Button1Line2,Button2Line2;Button1Line3,Button2Line3,Button3Line3");
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Buttons.png"/><br><br><br>

# Keyboard button options

| Expression |  Button  |
| ---------- | -------- |
|     /D     | Default  |
|     /M     | Primary  |
|     /P     | Positive |
|     /N     | Negative |
|  /LOCATION | Location |

## Example keyboard with button options
```c#
VkBot.Send(id, "Test", "Primary/M;Default/D;Positive/P;Negative/N;/LOCATION");
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Buttons2.png"/><br><br><br>

# Additional functions arguments
### Send function has 2 additional arguments
> Inline - keyboard embedded in message

> OneTime - keyboard hides on next message

## Example inline keyboard
```c#
VkBot.Send(id, "Test inline keyboard", "Primary/M,Default/D;Positive/P,Negative/N",true);
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/ButtonsInline.png"/><br><br><br>

# Changing keyboard split characters
### Function ***SetKeyboardSplitters()*** allows you to specify your own characters as separators
## Example
```c#
VkBot.SetKeyboardSplitters(':','.','|');
VkBot.Send(id, "Test inline keyboard", "Primary|M.Default|D:Positive|P.Negative|N");
```
<img src="https://github.com/yakcom/Vkontakte.Bot/blob/master/.github/Buttons3.png"/>
