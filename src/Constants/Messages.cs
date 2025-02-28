﻿// ===============================
// Author: Miroslav Novák (xnovak1k@stud.fit.vutbr.cz)
// Create date: 4.5.2019
// ===============================

namespace PimBot
{
    /// <summary>
    /// Every Chatbot messages - localization. Note that is supported Markdown.
    /// </summary>
    public static class Messages
    {
        public const string IntroducingMessage =
            @"Let me introduce myself. My name is **PimBot** 🤖 and I'm your virtual assistant. 
            My main purpose is to help you with finding goods to fit your demands and manage your orders.";

        public const string HelpMessage =
            @"You can find goods by write **find** and put item you are looking for. If you are not sure what exactly you are looking for, I can 
            help - just write **show categories** and I'm going to provide you all goods categories. 
            Then you can add item to your cart by write **add** and then number (No) of your item.   
            Also I can show your cart simple by write **show cart** and your order by write **show orders**. Finally, you can confirm your order by writing **confirm**.
            Don't remember, you can anytime show this help by type **help**.";

        public const string WhatCanIDo = "So, what can I do for you?";

        public const string NotUnderstand =
            @"Sorry, I don't **understand** your message 😕.";

        public const string ServerIssue =
            @"Sorry, PIM server is currently down. Please contact the administrator.";

        public const string SomethingWrong =
            @"Sorry, it looks like something went wrong.";

        public const string WhatToDoPrompt = @"What to do?";

        public const string CancelPrompt = @" Don't remember, you can cancel this action by write **cancel**.";

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
        public const string AddItemNoItem = "You have to provide No of item which you want order.";
        public const string AddItemHowManyPrompt = "How many **{0}** do you want order?";

        // ---------------Show cart ---------------
        public const string ShowCartFullPrice = @"Full price is";
        public const string EmptyCart =
            @"Your cart is **empty** now. To add item to your cart simple type **add** and then your desirable item.";

        public const string ShowCartTitle =
            @"Here is your ";

        public const string ShowCartAfter =
            @"You can confirm your order simple by write **confirm**. Also you can remove some item by typing **remove** and No.";

        // ---------------Show orders ---------------
        public const string ShowOrdersNoOrder = @"Sorry, you don't have any active orders.";
        public const string ShowOrdersCreatedAt = @"**Created at**: {0}";
        public const string ShowOrdersStatus = "**Status**: processing";

        // ---------------Show categories ---------------
        public const string ShowCategoriesAvaliableCategories = @"This is all available **categories**: ";

        // ---------------Detail item ---------------
        public const string DetailItemForgotItem = @"You forgot add item which you are interested in.";

        // ---------------Remove item ---------------
        public const string RemoveItemForgotItem = @"Sorry, I cannot find in your cart. You can show your cart simple by write *show cart*.";
        public const string RemoveItemRemoved = @"Ok, removed.";
        public const string RemoveItemChangeMind = @"Ok, never mind.";
        public const string RemoveItemNotNumber = @"Sorry, please add just number.";
        public const string RemoveItemGreater = @"Sorry, count has to be greater than 0.";
        public const string RemoveItemConfirm = "Are you sure?";

        // ---------------Find item ---------------
        public const string FindItemDidYouMean = "Did you mean {0}?";
        public const string FindItemFound = "I've found {0} potentional **{1}**. ";
        public const string FindItemAddToCart = @"You can simple add item to your cart by write **add** and item No. Also if you want to show more details about item just type **detail** and No.";
        public const string FindItemShowAllItem = "Show all items.";
        public const string FindItemSpecialize = "Let you ask me.";
        public const string FindItemSpecializeContinue = "Continue searching.";
        public const string FindItemNothingToAsk = "There is nothing more to ask.";
        public const string FindItemStartSpecialize = "Ok, let's found out what is in your mind.";
        public const string FindItemFindItem = "There is one item which corresponds your answers.";
        public const string FindItemNotFound = "Sorry, I haven't found any item to match your criteria. For new search please write **find**.";
        public const string FindItemForgotItem = "You forgot add what you are looking for. I can provide you a little help.";
        public const string FindItemEmptyCart = "Your cart is **empty** now. Please add something to your cart to create new **order**. You can find goods by write **find**. ";
        public const string FindItemAddToCartButton = "Add to cart";
        public const string FindItemShowDetailButton = "Show details";

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
        public const string GetUserInfoPromptIsOrderOk = @"Are all the information correct?";
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
        public const string GetUserInfoConfirmOrder = @"Are you **confirm** the order?";
        public const string GetUserInfoProcessingOrder = @"Your order has been sent for processing. You will receive an email with the next instructions. You can show your orders by writing **show orders**.";
        public const string GetUserInfoNotConfirmedOrder = @"Your order has been aborted.";
    }
}
