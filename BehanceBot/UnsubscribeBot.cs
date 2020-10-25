using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class UnsubscribeBot : Bot
    {

        public UnsubscribeBot(Writer Cons, FileReaderWriter fileReader) : base(Cons, fileReader)
        {
            Name = "UnFollowingBot";
        }

        internal override void Start(int limit)
        {
            int subs_count = OpenMySubs();
            if (subs_count <= 500)
            {
                Cons.WriteLine("The number of subscriptions is not enough to start unsubscribing.");
                return;
            }

            for (int i = 2; i < subs_count; i++) 
            {
                string xpath = UserXpath + i + "]";
                Сhrome.Scroll(xpath);
            }

            for (int i = subs_count - 3; i > 0; i--) 
            {
                if (IsBlock())
                    return;

                string xpath = UserXpath + i + "]";
                Сhrome.Scroll(xpath);
                Thread.Sleep(3000);
                Unsubscribe(i);
                limit--;
                if (limit <= 0)
                    return;
            }

        }

        internal int OpenMySubs()
        {
            Сhrome.OpenUrl(@"https://www.behance.net/balakir/projects");
            string mySubs_xPath = @"//*[@id='site-content']/div/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[4]/td[2]/a";

            int subs_count = ParsToInt(Сhrome.FindWebElement(By.XPath(mySubs_xPath)).Text);
            Cons.WriteLine($"Number of our subscriptions {subs_count}");
            Сhrome.ClickButtonXPath(mySubs_xPath);
            return subs_count;
        }

        internal void Unsubscribe(int j)
        {
            j += 2;
            Cons.WriteLine($"Unsubscribe: {j}", false);
            string btn_1 = UserXpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[2]";
            string btn_2 = UserXpath + $@"{j}]/div/div/div/div[2]/div[1]/div/a[3]";
            IWebElement elem_button_1 = Сhrome.FindWebElement(By.XPath(btn_1));
            IWebElement elem_button_2 = Сhrome.FindWebElement(By.XPath(btn_2));

            if (elem_button_1.Displayed)
            {
                Сhrome.ClickButtonXPath(btn_1);
                return;
            }
            if (elem_button_2.Displayed)
            {
                Сhrome.ClickButtonXPath(btn_2);
                return;
            }
            Cons.WriteLine($"Account: {j}, error unsubscribe");
        }
    }

}
