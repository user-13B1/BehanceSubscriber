using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;


namespace BehanceBot
{
    internal class SubscriberBot : Bot
    {
        int folow_counter;
      
        public SubscriberBot(Writer Cons,  DBmanager db) : base(Cons, db)
        {
            Name = "FollowingBot";
            folow_counter = 0;
        }

    internal override void Start(int limit)
        {
            if (!OpenRandomFollowerPage())
                return;

            for (int i = 3; i < 3000; i++)
            {
                string xpathNextUser = UserXpath + "/li[" +  i + "]";
                
                if (IsBlock())
                    return;
               
                Сhrome.Scroll(xpathNextUser);
                if (!ParseAndFollowing(xpathNextUser, limit)) 
                    return;
            }
        }

        internal bool ParseAndFollowing(string xpathNextUser, int follow_max_count)
        {
            Thread.Sleep(300);

            if(CheckUser(xpathNextUser, out string userUrl, out int userCountLike, out int userCountViews, out string userName))
            {
                if (Сhrome.ClickButtonXPath(xpathNextUser + @"/div/div/div/div[2]/div[1]/div/a[1]"))
                {
                    Cons.WriteLine($"{Name} Follow:{folow_counter}) {userCountLike} {userCountViews} {userName} ");
                    folow_counter++;
                    db.AddUser(userUrl, 0, 0, 1);       //Add to friend
                }
                else
                    Cons.WriteLine($"{Name} Error follow {userName}");
                return true;
            }

            if (folow_counter >= follow_max_count)
                return false;

            return true;
        }

    }

}
