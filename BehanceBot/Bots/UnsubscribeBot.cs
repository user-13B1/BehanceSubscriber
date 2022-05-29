using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class UnsubscribeBot : Bot
    {

        public UnsubscribeBot(Writer Cons, DBmanager db) : base(Cons, db)
        {
            Name = "UnFollowingBot";
        }

        internal override void Start(int limit)
        {
            Thread.Sleep(10000);

            Сhrome.OpenUrl(@"https://www.behance.net/balakir/projects");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            
            string mySubs_xPath = "//div[contains(@class,'UserInfo-column-')]/table/tbody/tr[4]/td[2]/a";



            int subsCount = ParsToInt(Сhrome.FindWebElement(By.XPath(mySubs_xPath)).Text);
            Cons.WriteLine($"{Name} Number of our subscriptions {subsCount}");
            if (subsCount <= 1200)
            {
                Cons.WriteLine("The number of subscriptions is not enough to start unsubscribing.");
                return;
            }

            Сhrome.ClickButtonXPath(mySubs_xPath);
            Thread.Sleep(TimeSpan.FromSeconds(1));

            if (Сhrome.FindWebElement(By.XPath(FollowingUserXpath)) == null)
            {
                Cons.WriteLine("UserXpath not found.");
                return;
            }

            for (int i = 2; i < subsCount; i++)
            {
                Сhrome.Scroll(string.Format($"{FollowingUserXpath}/li[{i}]"));

                if (i % 100 == 0)
                {
                    Cons.WriteLine($"Scroll following list.{i}");
                }
            }

            for (int i = subsCount - 3; i > 0; i--)
            {
                if (IsBlock())
                    return;
               

                Сhrome.Scroll(string.Format($"{FollowingUserXpath}/li[{i}]"));
                Thread.Sleep(3000);
                Unsubscribe(i);
                limit--;
                if (limit <= 0)
                    return;
            }

        }

        internal void Unsubscribe(int j)
        {
            j += 2;
           
            IWebElement Element = Сhrome.FindWebElement(By.XPath($"{FollowingUserXpath}/li[{j}]//div/div/div/div[1]/h3/a"));
            string userUrl = Element.GetAttribute("href");
            Cons.WriteLine($"Unsubscribe: {j}) {userUrl}", false);
            db.UpdateUser(userUrl, 0, 1, 0);

            string btn_1 = $"{FollowingUserXpath}/li[{j}]/div/div/div/div[2]/div[1]/div/a[2]";
            string btn_2 = $"{FollowingUserXpath}/li[{j}]/div/div/div/div[2]/div[1]/div/a[3]";

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
