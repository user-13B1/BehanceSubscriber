using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class LikeBot : Bot
    {
        public LikeBot(Writer Cons, FileReader fileReader) : base(Cons, fileReader)
        {
            Name = "LikeBot";
        }

        internal override void Start(int limit)
        {
            int like_counter = 0;
            for (int j = 0; j < 30; j++)
            {
                OpenRandomPage();
                for (int i = 3; i < 3000; i++)
                {
                    string xpathUser = user_xpath + i + "]";

                    if (like_counter >= limit)
                        return;

                    if (IsBlock())
                        return;

                    if (!chrome.Scroll(xpathUser))
                    {
                        console.WriteLine("End following list");
                        break;
                    }

                    if (CheckUser(xpathUser, out string userUrl))
                    {
                        LikePhoto(userUrl);
                        like_counter++;
                        console.WriteLine($"Like! Number of likes = {like_counter}");
                    }

                    Thread.Sleep(200);
                }
            }


            void LikePhoto(string userUrl)
            {
                chrome.OpenUrlNewTab(userUrl);

                IWebElement Element_photo = chrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));
                string url_photo = Element_photo.GetAttribute("href");
                chrome.OpenUrl(url_photo);

                Thread.Sleep(300);

                if (!chrome.ClickButtonXPath(@"//div[.='Оценить']"))
                {
                    console.WriteLine($"Error like");
                }

                Thread.Sleep(500);
                chrome.CloseAndReturnTab();
            }

        }
    }
}
