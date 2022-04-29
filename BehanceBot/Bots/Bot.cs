﻿using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal abstract class Bot
    {
        private protected ChromeBrowser Сhrome { get; set; }
        private protected Writer Cons { get; set; }
        internal string Name { get; set; }
        internal string UserXpath { get; set; }
        internal string FollowingUserXpath;
        static int profileCounter = 0;
        protected DBmanager db;
        protected static int repeatCounter;

        public Bot(Writer console,DBmanager db)
        {
            //UserXpath = "//ul[@class ='Followers-list-dlZ']";
            UserXpath = "//ul[contains(@class,'Followers-list-')]";

            //FollowingUserXpath = "//ul[@class ='Following-list-GtQ']";
            FollowingUserXpath = "//ul[contains(@class,'Following-list-')]";

            Cons = console;
            this.db = db;
            string chromeProfileName = (++profileCounter).ToString() + "BehanceBot";
            Сhrome = new ChromeBrowser(chromeProfileName);
            Сhrome.SetWindowSize(1280, 1000);
        }

        internal bool IsBlock()
        {
            string xpath = $@"/html/body/div[26]/div/div[2]/a";
            if (Сhrome.IsElementPage(By.XPath(xpath)))
            {
                Cons.WriteLine($"Limit error.");
                return true;
            }
            return false;
        }

        internal bool Autorize(string v1, string v2)
        {
            int timeSleep = 2000;
#if DEBUG
            timeSleep /= 2;
#endif
            Cons.WriteLine($"{Name}: Check authorization.");           
            
            Сhrome.OpenUrl(@"https://www.behance.net/search");

            if (Сhrome.FindWebElement(By.XPath("//*[contains(text(), 'Поделиться проектом')]")) != null)
            {
                Cons.WriteLine($"{Name}: Authorization already passed.");
                return true;
            }

            Cons.WriteLine($"{Name}: Start authorization .");


            //---Enter---
            IWebElement Element = Сhrome.FindWebElement(By.XPath("//li/div/button[contains(.,'Вход')]"));
            Element.Click();

            Thread.Sleep(timeSleep);
            Сhrome.SendKeysXPath(".//input[@id='EmailPage-EmailField']", v1);
            Thread.Sleep(timeSleep);
            Сhrome.ClickButtonXPath(".//*[@id='EmailForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            Сhrome.SendKeysXPath(".//input[@id='PasswordPage-PasswordField']", v2);
            Thread.Sleep(timeSleep);
            Сhrome.ClickButtonXPath(".//*[@id='PasswordForm']/section[2]/div[2]/button");
            Thread.Sleep(TimeSpan.FromSeconds(7));

            Сhrome.OpenUrl(@"https://www.behance.net");
            if (Сhrome.FindWebElement(By.XPath("//*[contains(text(), 'Поделиться проектом')]"))==null)
            {
                Cons.WriteLine($"{Name}: Error autorize.");
                return false;
            }
            Cons.WriteLine($"{Name}: Authorized.");
            return true;
        }

        internal bool OpenRandomFollowerPage()
        {
            Cons.WriteLine($"{Name}: Open Random followers Page.");
            string randomPage = GetRandomFollowersPage();
            Сhrome.OpenUrl(randomPage);

            if (Сhrome.FindWebElement(By.XPath(UserXpath)) == null)
            {
                Cons.WriteLine("Error Open Random Follower Page");
                return false;
            }
            return true;
        }

        internal string GetRandomFollowersPage()
        {
            return db.GetRandomUrl() + "/followers";
        }


        internal int ParsToInt(string text)
        {
            if (!Int32.TryParse(text, out _))
            {
                text = text.Replace(" ", string.Empty);
                text = text.Replace(".", string.Empty);
                text = text.Replace("тыс", "000");
                text = text.Replace("млн", "000000");
              
                if (text.IndexOf(',') > 0)
                {
                    text = text.Replace(",", string.Empty);
                    text = text.Substring(0, text.Length - 1);
                }
            }

            if (!Int32.TryParse(text, out int num))
                Cons.WriteLine($"Errоr parsing string to int  - {text}");

            return num;
        }

        internal abstract void Start(int limit);

        internal bool CheckUser(string xpath, out string userUrl,out int userCountLike, out int userCountViews, out string userName)
        {
            string buttonText = String.Empty;
            var element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span"));
            if (element != null)
                userCountLike = ParsToInt(element.Text);
            else
                userCountLike = 0;

           element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span"));
            if (element != null)
                userCountViews = ParsToInt(element.Text);
            else
                userCountViews = 0;

            element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span"));
            if (element != null)
                buttonText = element.Text;

            IWebElement Element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
            if (Element == null)
            {
                userUrl = userName = null;
                return false;
            }
            userUrl = Element.GetAttribute("href");
            userName = Element.Text;

            if (db.IsRepeat(userUrl))
            {
                if (repeatCounter % 100 == 0)
                    Cons.WriteLine($"{Name}:Repeat account. {repeatCounter}");
                repeatCounter++;
                return false;
            }

            if (userCountViews > 20000)
            {
                Cons.WriteLine($"{Name}:{userName} - Add for subscribe.");
                db.AddUser(userUrl,1,0,0);
            }


            if (userCountLike < 200 && userCountLike > 10 && userCountViews < 900 && userCountViews > 30 && buttonText == "Подписаться")
            {
                Thread.Sleep(500);
                return true;
            }
            return false;
        }

        internal void Close()
        {
            Cons.WriteLine($"{Name}: Close.");
            Сhrome.Quit();
        }
    }
}