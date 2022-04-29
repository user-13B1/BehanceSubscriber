﻿using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;


namespace BehanceBot
{
    internal class SubscriberBot : Bot
    {
        static int folow_counter = 0;
      
        public SubscriberBot(Writer Cons,  DBmanager db) : base(Cons, db)
        {
            Name = "FollowingBot";
        }

        internal override void Start(int limit)
        {
            for (int j = 0; j < 5; j++)
            {
                if (!OpenRandomFollowerPage())
                    return;


                for (int i = 3; i < 3000; i++)
                {
                    string xpathNextUser = UserXpath + "/li[" + i + "]";

                    if (IsBlock())
                        return;
                    
                    Сhrome.Scroll(xpathNextUser);

                    if (!ParseAndFollowing(xpathNextUser, limit))
                        return;
                   // Thread.Sleep(100);
                }
                Cons.WriteLine($"___ Смена списка подписок");
            }
        }


        internal bool ParseAndFollowing(string xpathNextUser, int follow_max_count)
        {
            if (CheckUser(xpathNextUser, out string userUrl, out int userCountLike, out int userCountViews, out string userName))
            {
                if (Сhrome.ClickButtonXPath(xpathNextUser + @"/div/div/div/div[2]/div[1]/div/a[1]"))
                {
                    Cons.WriteLine($"{Name} Follow:{folow_counter}) {userCountLike} {userCountViews} {userName} ");
                    folow_counter++;
                    db.AddUser(userUrl, 0, 0, 1);       //Add to friend
                    Cons.WriteLine("Subscribe!");
                }
                else
                    Cons.WriteLine($"{Name} Error follow {userName}");
              
                return true;
            }

            if (folow_counter >= follow_max_count)
            {
                Cons.WriteLine($"Сounter reached. Completed {follow_max_count} subscriptions");
                return false;
            }

            return true;
        }

    }

}
