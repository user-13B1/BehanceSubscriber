using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal abstract class Bot
    {
        internal readonly Chrome chrome;
        internal readonly Writer console;
        internal readonly FileReader fileReader;
        internal string Name { get; set; }
        internal string user_xpath;

        public Bot(Writer console, FileReader fileReader)
        {
            this.console = console;
            this.fileReader = fileReader;
            chrome = new Chrome();
            chrome.SetWindowSize(1280, 1000);
            user_xpath = "//*[@id='app']/div/div/div[18]/div/div[1]/div/div[2]/div/div[1]/ul/li[";
        }

        internal bool IsBlock()
        {
            string xpath = $@"/html/body/div[26]/div/div[2]/a";
            if (chrome.IsElementPage(By.XPath(xpath)))
            {
                console.WriteLine($"Limit error.");
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
            console.WriteLine($"{Name}: authorization.");                             
            chrome.OpenUrl(@"https://www.behance.net/search");

            //---Enter---
            string xpath = "//button[@class='Btn-button-BGn Btn-base-M-O Btn-normal-hI4 js-adobeid-signin PrimaryNav-a11yButton-2Cl']";
            IWebElement Element = chrome.FindWebElement(By.XPath(xpath));
            Element.Click();
            
            Thread.Sleep(timeSleep);
            chrome.SendKeysXPath(@".//input[@id='EmailPage-EmailField']", v1);
            Thread.Sleep(timeSleep);
            chrome.ClickButtonXPath(@".//*[@id='EmailForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            chrome.SendKeysXPath(@".//input[@id='PasswordPage-PasswordField']", v2);
            Thread.Sleep(timeSleep);
            chrome.ClickButtonXPath(@".//*[@id='PasswordForm']/section[2]/div[2]/button");
            Thread.Sleep(timeSleep);
            
            if (!chrome.ClickButtonXPath(@"./html/body/nav/ul[2]/li[4]/a"))
            {
                console.WriteLine($"{Name}: Authorisation Error.");
                return false;
            }

            chrome.ClickButtonXPath(@".//*[@id='app']/div/div/div[1]/main/div[2]/div[1]/div[2]/div[1]/div[2]/table/tbody/tr[3]/td[2]/a");
            console.WriteLine($"{Name}: Authorized.");
            return true;
        }

        internal void OpenRandomPage()
        {
            string s_randPage = fileReader.GetRandomUrl();
            console.WriteLine($"Open random page {s_randPage}");
            chrome.OpenUrl(s_randPage);
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
                console.WriteLine($"Errоr parsing string to int  - {text}");

            return num;
        }

        internal virtual void Start(int limit) { }

        internal bool CheckUser(string xpath, out string userUrl)
        {
            int likePersona = ParsToInt(chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span")).Text);
            int numViews = ParsToInt(chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text);
            string buttonText = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

            IWebElement Element = chrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
            userUrl = Element.GetAttribute("href");

            if (likePersona < 100 && likePersona > 10 && numViews < 999 && numViews > 30 && buttonText == "Подписаться")
                return true;

            return false;
        }

        internal void Close()
        {
            console.WriteLine($"{Name}: Close.");
            chrome.Quit();
        }
    }
}