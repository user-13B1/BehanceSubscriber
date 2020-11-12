using OpenQA.Selenium;
using SeleniumLib;
using System;
using System.Threading;


namespace BehanceBot
{
    internal class SubscriberBot : Bot
    {
        int folow_counter;
      
        public SubscriberBot(Writer Cons, FileReaderWriter fileReader, DBmanager db) : base(Cons, fileReader,db)
        {
            Name = "FollowingBot";
            folow_counter = 0;
        }

    internal override void Start(int limit)
        {
            OpenRandomPage();
            for (int i = 3; i < 3000; i++)
            {
                string xpath = UserXpath + i + @"]";
               
                if (IsBlock())
                    return;
               
                Сhrome.Scroll(xpath);

                if (!ParseAndFollowing(xpath, limit)) 
                    return;
            }
        }

        internal bool ParseAndFollowing(string xpath, int follow_max_count)
        {
            Thread.Sleep(300);
            try
            {
                IWebElement Element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[3]/span"));
                if(Element == null)
                    return true;

                string s_like = Element.Text;
                int like_persona = ParsToInt(s_like);

                string s_num_views = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[4]/span")).Text;
                int num_views = ParsToInt(s_num_views);
                string button_text = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]/span")).Text;

                Element = Сhrome.FindWebElement(By.XPath(xpath + @"/div/div/div/div[1]/h3/a"));
                string name_persona = Element.Text;
                string urlUserAdress = Element.GetAttribute("href");
                
                if(db.IsRepeat(urlUserAdress))
                {
                    Cons.WriteLine($"{Name}:Repeat account.");
                }

                if (like_persona < 100 && like_persona > 5 && num_views < 999 && num_views > 20 && button_text == "Подписаться")
                {
                    Cons.WriteLine($"{Name} Follow:{folow_counter}) {like_persona} {num_views} {name_persona} ");
                    Сhrome.ClickButtonXPath(xpath + @"/div/div/div/div[2]/div[1]/div/a[1]");
                    folow_counter++;
                    db.AddToFriend(urlUserAdress);
                }

                if (num_views > 10000)
                {
                    Cons.WriteLine($"{Name}: Add for subscribe - {name_persona} ");
                    FileReader.WriteUrltoFile(urlUserAdress + @"/followers");
                    db.AddForSubsList(urlUserAdress);
                }

                if (folow_counter >= follow_max_count)
                {
                    Cons.WriteLine($"{Name}: Follow limit. End work");
                    return false;
                }

            }
            catch
            {
                Cons.WriteLine($"{Name} Error parsing account.");
                return false;
            }

            return true;
        }

    }

}
