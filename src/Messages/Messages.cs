namespace PimBot
{
    public static class Messages
    {
        public const string IntroducingMessage =
            @"Let me introduce myself. My name is **PimBot** 🤖 and I'm your virtual assistant. 
            My main purpose is to help you with finding goods to fit your demands and manage your orders.";

        public const string HelpMessage =
            @"You can find goods by write **find** and put item you are looking for. If you are not sure what exactly you
            are looking for, I can help - just write **show categories** and I'm going to provide you all goods categories. 
            Then you can add item to your cart by write **add** and then number (No) of your item.   
            Also I can show your cart simple by write **show cart**. Finally, you can confirm your order by writing **confirm**.
            Don't remamber, you can anytime show this help by type **help**.";

        public const string WhatCanIDo = "So, what can I do for you?";

        public const string NotUnderstand =
            @"Sorry, I don't **understand** your message 😕.";

        public const string ServerIssue =
            @"Sorry, **I cannot connect** to the PIM server 😕. Admin should check if server is running.";

        public const string SuggestHelp = @"For more help simple write **help**.";

        // ---------------General ---------------
        public const string Greetings = @"Greetings";
        public const string Name = @"Name";
        public const string Login = @"Login";
        public const string Email = @"Email";
        public const string PhoneNumber = @"Phone number";
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
        public const string IsThereAnythingICanDo = @"Is there anything else I can help you with?";
        public const string NotFound = @"Sorry, I haven't found";
        public const string Description = @"Description";
        public const string Number = @"No";
        public const string Count = @"Count";
        public const string UnitPrice = @"Unit price";
        public const string Price = @"Price";
        public const string Skip = @"Skip";

        // ---------------Interuption ---------------
        public const string InteruptionCancelConfirm = @"Ok. I've canceled our last activity.";
        public const string InteruptionCancelNotConfirm = @"I don't have anything to cancel.";

        // ---------------Add to cart ---------------
        public const string AddToCartAdded = @"Added to cart 👍. You can show your current cart by writting **show cart**.";
        public const string AddToCartValidationOnlyNumber = @"Sorry, please add just number.";
        public const string AddToCartValidationPositiveNumber = @"Sorry, count has to be greater than 0.";
        public const string AddItemForgotItem = "You forgot add item what you want order.";

        // ---------------Show cart ---------------
        public const string ShowCartFullPrice = @"Full price is";
        public const string EmptyCart =
            @"Your cart is **empty** now. To add item to your cart simple type **add** and then your desirable item.";

        public const string ShowCartTitle =
            @"Here is your cart:";

        public const string ShowCartAfter =
            @"You can confirm your order simple by write **confirm**. Also you can remove some item by typing **remove** and No.";

        // ---------------Show categories ---------------
        public const string ShowCategoriesAvaliableCategories = @"This is all available **categories**: ";

        // ---------------Detail item ---------------
        public const string DetailItemForgotItem = @"You forgot add item which you are interested in.";

        // ---------------Remove item ---------------
        public const string RemoveItemForgotItem = @"Sorry, I cannot find in your cart. You can show your cart simple by write *show cart*.";

        // ---------------Find item ---------------
        public const string FindItemAddToCart = @"You can simple add item to your cart by write **add** and item No. Also if you want to show more details about item just type **detail** and No.";
        public const string FindItemShowAllItem = "Show all items.";
        public const string FindItemSpecialize = "Let you ask me.";
        public const string FindItemSpecializeContinue = "Continue searching.";
        public const string FindItemNothingToAsk = "There is nothing more to ask.";
        public const string FindItemStartSpecialize = "Ok, let's found out what is in your mind.";
        public const string FindItemFindItem = "There is one item which corresponds your answers.";
        public const string FindItemNotFound = "Sorry, I haven't found any item to match your criteria. For new search please write **find**.";
        public const string FindItemForgotItem = "You forgot add what you are looking for. I can provide you a little help.";

        public static readonly string[] FindItemQuestionStart = new string[]
            {
                "What about",
                "Ok, and",
                "What do you think about",
                "",
            };


        // ---------------Get User Info ---------------
        public const string GetUserInfoNewOrder = @"To make a new order I need to contact information about you.";
        public const string GetUserInfoPromptName = @"What is your **full name**? Or company name?";
        public const string GetUserInfoPromptEmail = @"What is your **email address**?";
        public const string GetUserInfoPromptPhoneNumber = @"What is your **phone number**?";
        public const string GetUserInfoPromptAddress = @"What is your **address**?";
        public const string GetUserInfoPromptPostCode = @"What is your **post code**?";
        public const string GetUserInfoPromptCity = @"What is your **city**?";
        public const string GetUserInfoPromptShippingMatch = @"Is this address also your shipping address?";
        public const string GetUserInfoPromptIsOrderOk = @"Is all information correct";
        public const string GetUserInfoContinueWithOrder = @"Okey, let's continue with the order";
        public const string GetUserInfoPromptChange = @"What customer information do you want to change?";
        public const string GetUserInfoPromptEdit = @"Please, choose what do you want change.";
        public const string GetUserInfoServerIssue = @"Sorry, there is some problem with server. Please try it later.";
        public const string GetUserInfoCustomerInformation = @"This is your customer information:";
        public const string GetUserInfoIsThisYourShippingInformation = @"Is address match shipping address:";
        public const string GetUserInfoPromptShippingName = @"What is your **shipping full name**?";
        public const string GetUserInfoPromptShippingAddress = @"What is your **shipping address**?";
        public const string GetUserInfoPromptShippingPostCode = @"What is your **shipping post code**?";
        public const string GetUserInfoPromptShippingCity = @"What is your **shipping city**?";
        public const string GetUserInfoEmailIsNotValid = @"Sorry, your email is not valid. Please, try it again.";

    }
}
