using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PimBot.Messages
{
    public static class Messages
    {
        public const string IntroducingMessage =
            @"Let me introduce myself. My name is PimBot and obviously I'm a bot 🤖 and what is more important - your virtual assistent!";

        public const string HelpMessage =
            @"Right now, I'm not really smart. You can type `items` and I'm gonna show you all items from the PIM remote system.";

        public const string NotUnderstand =
            @"Sorry, I don't **understand** your message 😕.";

        public const string ServerIssue =
            @"Sorry, **I cannot connect** to the PIM server 😕. Admin should check if server is running.";

        public const string EmptyCart =
            @"Your cart is **empty** now. To add item to your cart simple type *add* and then your desirable item.";

        public const string ShowCartTitle =
            @"Here is your cart:";

        public const string ShowCartAfter =
            @"You can confirm your order simple by write *confirm*.";

        public const string SuggestHelp = @"For more help simple write *help*.";
    }
}
