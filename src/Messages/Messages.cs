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
            @"Sorry, I don't understand your message 😕.";

        public const string ServerIssue =
            @"Sorry, I cannot connect to the PIM server 😕. Admin should check if server is running.";
    }
}
