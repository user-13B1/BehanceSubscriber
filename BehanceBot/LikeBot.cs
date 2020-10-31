using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class LikeBot : Bot
    {
        public LikeBot(Writer Cons, FileReaderWriter fileReader) : base(Cons, fileReader)
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
                    string xpathUser = UserXpath + i + "]";

                    if (like_counter >= limit)
                        return;
                   
                    
                    if (IsBlock())
                        return;
                  
                    if (!Сhrome.Scroll(xpathUser))
                    {
                        Cons.WriteLine("End following list");
                        break;
                    }

                    if (CheckUser(xpathUser, out string userUrl))
                    {
                        LikePhoto(userUrl);
                        like_counter++;
                        Cons.WriteLine($" Bot:{botNum} Like! Number of likes = {like_counter}");
                    }

                    Thread.Sleep(200);
                }
            }


            void LikePhoto(string userUrl)
            {
                Сhrome.OpenUrlNewTab(userUrl);

                IWebElement Element_photo = Сhrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));
                string url_photo = Element_photo.GetAttribute("href");
                Сhrome.OpenUrl(url_photo);

                Thread.Sleep(300);

                if (!Сhrome.ClickButtonXPath(@"//div[.='Оценить']"))
                {
                    Cons.WriteLine($"Error like");
                }

                Thread.Sleep(500);
                Сhrome.CloseAndReturnTab();
            }

        }
    }
}
