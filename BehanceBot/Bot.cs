using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal abstract class Bot
    {
        private protected ChromeBrowser Сhrome { get; set; }
        private protected Writer Cons { get; set; }
        private protected FileReaderWriter FileReader { get; set; }
        internal string Name { get; set; }
        internal string UserXpath { get; set; }
        static int profileCounter = 0;
        internal int botNum;

        public Bot(Writer console, FileReaderWriter fileReader)
        {
        
            UserXpath = "//*[@id='app']/div/div/div[19]/div/div[1]/div/div[2]/div/div[2]/div/div[1]/ul/li[";
            Cons = console;
            FileReader = fileReader;
            Сhrome = new ChromeBrowser((++profileCounter).ToString());
            botNum = profileCounter;
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
            timeSleep /= 10;
#endif
            Cons.WriteLine($"{Name}: authorization.");                             
            Сhrome.OpenUrl(@"https://www.behance.net/search");

            if (Сhrome.FindWebElement(By.XPath("//a[@aria-label ='Создать проект']")) != null)
            {
                Cons.WriteLine($"{Name}: Пользователь авторизован.");
                return true;
            }

            //---Enter---
            IWebElement Element = Сhrome.FindWebElement(By.XPath("//*[@id='app']/div/div/div[1]/div/div[2]/div[2]/ul/li[1]/div/button"));
            Element.Click();
            
            Thread.Sleep(timeSleep);
            Сhrome.SendKeysXPath(@".//input[@id='EmailPage-EmailField']", v1);
            Thread.Sleep(timeSleep);
            Сhrome.ClickButtonXPath(@".//*[@id='EmailForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            Сhrome.SendKeysXPath(@".//input[@id='PasswordPage-PasswordField']", v2);
            Thread.Sleep(timeSleep);
            Сhrome.ClickButtonXPath(@".//*[@id='PasswordForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            
            if (!Сhrome.ClickButtonXPath(@"./html/body/nav/ul[2]/li[4]/a"))
            {
                Cons.WriteLine($"{Name}: Authorisation Error.");
                return false;
            }

            Сhrome.ClickButtonXPath(@".//*[@id='app']/div/div/div[1]/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[3]/td[2]/a");
            Cons.WriteLine($"{Name}: Authorized.");
            return true;
        }

        internal void OpenRandomPage()
        {
            string s_randPage = FileReader.GetRandomUrl();
            Cons.WriteLine($"Open random page {s_randPage}");
            Сhrome.OpenUrl(s_randPage);
        }

        internal int ParsToInt(string text)
        {
            if (!Int32.TryParse(text, out _))
            {
                text = text.Replace(" ", string.Empty);
                text = text.Replace("тыс.", "000");
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

        internal bool CheckUser(string xpath, out string userUrl)
        {
            int likePersona = ParsToInt(Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span")).Text);
            int numViews = ParsToInt(Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text);
            string buttonText = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

            IWebElement Element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
            userUrl = Element.GetAttribute("href");

            if (likePersona < 100 && likePersona > 10 && numViews < 999 && numViews > 30 && buttonText == "Подписаться")
                return true;

            return false;
        }

        internal void Close()
        {
            Cons.WriteLine($"{Name}: Close.");
            Сhrome.Quit();
        }
    }
}