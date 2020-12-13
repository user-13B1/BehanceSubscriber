using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;

namespace BehanceBot
{
    internal class LikeBot : Bot
    {
        static int like_counter;
        public LikeBot(Writer Cons,  DBmanager db) : base(Cons, db)
        {
            Name = "LikeBot";
            
        }

        internal override void Start(int limit)
        {
            while(true)
            {
                if(!OpenRandomFollowerPage())
                    return;

                for (int i = 3; i < 3000; i++)
                {
                    string xpathNextUser = UserXpath + "/li[" + i + "]";

                    if (like_counter >= limit)
                        return;
                    
                    if (IsBlock())
                        return;
                  
                    if (!Сhrome.Scroll(xpathNextUser))
                        break;
                    if (CheckUser(xpathNextUser, out string userUrl, out _, out _, out _))
                    {
                        LikePhoto(userUrl);
                        like_counter++;
                        Cons.WriteLine($"Like!#{like_counter}");
                        db.AddUser(userUrl,0,0,0);
                    }

                }
                Cons.WriteLine("End following list");
            }


            void LikePhoto(string userUrl)
            {
                Сhrome.OpenUrlNewTab(userUrl);

                IWebElement Element_photo = Сhrome.FindWebElement(By.XPath(@"//*[@id='site-content']/div/main/div[2]/div[2]/div/div/div/div/div[1]/div[1]/div/div/div[2]/a"));
                Сhrome.OpenUrl(Element_photo.GetAttribute("href"));
                Thread.Sleep(300);
                
                if (!Сhrome.ClickButtonXPath(@"//div[.='Оценить']"))
                {
                    Cons.WriteLine($"Error like!");
                }

                Thread.Sleep(1000);
                Сhrome.CloseAndReturnTab();
            }

        }
    }
}
