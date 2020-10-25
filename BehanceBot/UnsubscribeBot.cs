using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class UnsubscribeBot : Bot
    {

        public UnsubscribeBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
        {
            Name = "UnFollowingBot";
        }

        internal override void Start(int limit)
        {
            int subs_count = OpenMySubs();
            if (subs_count <= 500)
            {
                console.WriteLine("The number of subscriptions is not enough to start unsubscribing.");
                return;
            }

            for (int i = 2; i < subs_count; i++) 
            {
                string xpath = user_xpath + i + "]";
                chrome.Scroll(xpath);
            }

            for (int i = subs_count - 3; i > 0; i--) 
            {
                if (IsBlock())
                    return;

                string xpath = user_xpath + i + "]";
                chrome.Scroll(xpath);
                Thread.Sleep(3000);
                Unsubscribe(i);
                limit--;
                if (limit <= 0)
                    return;
            }

        }

        internal int OpenMySubs()
        {
            chrome.OpenUrl(@"https://www.behance.net/balakir/projects");
            string mySubs_xPath = @"//*[@id='site-content']/div/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[4]/td[2]/a";

            int subs_count = ParsToInt(chrome.FindWebElement(By.XPath(mySubs_xPath)).Text);
            console.WriteLine($"Number of our subscriptions {subs_count}");
            chrome.ClickButtonXPath(mySubs_xPath);
            return subs_count;
        }

        internal void Unsubscribe(int j)
        {
            j += 2;
            console.WriteLine($"Unsubscribe: {j}", false);
            string btn_1 = user_xpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[2]";
            string btn_2 = user_xpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[3]";
            IWebElement elem_button_1 = chrome.FindWebElement(By.XPath(btn_1));
            IWebElement elem_button_2 = chrome.FindWebElement(By.XPath(btn_2));

            if (elem_button_1.Displayed)
            {
                chrome.ClickButtonXPath(btn_1);
                return;
            }
            if (elem_button_2.Displayed)
            {
                chrome.ClickButtonXPath(btn_2);
                return;
            }
            console.WriteLine($"Account: {j}, error unsubscribe");
        }
    }

}
