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
            @"You can find simple by write *find* and then your desirable item. Also you can add item to your cart by write *add item*.
            Also you can show your cart simple by write *show cart*. After that, you can confirm your order by writing *confirm*.
            So, what can I do for you? 😊";

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

        public const string Name = @"Name";
        public const string Address = @"Address";
        public const string PostCode = @"Post Code";
        public const string City = @"City";
        public const string Yes = @"Yes";
        public const string No = @"No";
        public const string ShippingName = @"Shipping name";
        public const string ShippingAddress = @"Shipping address";
        public const string ShippingPostCode = @"Shippping post code";
        public const string ShippingCity = @"Shipping city";
        public const string IsAddressMatchShippingAddress = @"Address match";


        // ---------------Get User Info ---------------
        public const string GetUserInfoNewOrder = @"To make a new order I need to contact information about you.";
        public const string GetUserInfoPromptName = @"What is your **full name**? Or company name?";
        public const string GetUserInfoPromptAddress = @"What is your **adress**?";
        public const string GetUserInfoPromptPostCode = @"What is your **post code**?";
        public const string GetUserInfoPromptCity = @"What is your **city**?";
        public const string GetUserInfoPromptShippingMatch = @"Is this address also shipping address?";
        public const string GetUserInfoPromptIsOrderOk = @"Is all information correct";
        public const string GetUserInfoContinueWithOrder = @"Okey, let's continue with the order";
        public const string GetUserInfoPromptChange = @"What customer information do you want to change?";
        public const string GetUserInfoPromptEdit = @"Please, choose what do you want change.";
        public const string GetUserInfoServerIssue = @"Sorry, there is some problem with server. Please try it later.";
        public const string GetUserInfoCustomerInformation = @"This is your customer information:";
        public const string GetUserInfoIsThisYourShippingInformation = @"Is address match shipping address:";
        public const string GetUserInfoPromptShippingName = @"What is your **shipping full name**?";
        public const string GetUserInfoPromptShippingAddress = @"What is your **shipping adress**?";
        public const string GetUserInfoPromptShippingPostCode = @"What is your **shipping post code**?";
        public const string GetUserInfoPromptShippingCity = @"What is your **shipping city**?";

    }
}
