using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Data;
using System.Data.Linq.Mapping;
using System.Runtime.InteropServices.WindowsRuntime;
using LiteDB;


namespace BehanceBot
{
    class DBmanager
    {
        readonly Writer cons;
        private readonly LiteDatabase liteDB;
        private readonly ILiteCollection<BehanceUsers> collection;
        public DBmanager(Writer cons)
        {
            this.cons = cons;
            liteDB = new LiteDatabase(Directory.GetCurrentDirectory() + @"\Data.db");
            collection = liteDB.GetCollection<BehanceUsers>("ParseBehanceUsers");
        }

        public void CloseBase()
        {
            liteDB.Dispose();
        }

        internal void AddUser(string url,Byte donorsubs,Byte formerfriend, Byte friend)
        {
            if(IsRepeat(url))
                return;

            var newUser = new BehanceUsers
            {
                Url = url,
                Date = DateTime.Now,
                Donorsubs = donorsubs,
                Formerfriend = formerfriend,
                Friend = friend
            };
            collection.Insert(newUser);
        }

        internal string GetRandomUrl()
        {
            string url;
            var query = collection.Query()
               .Where(u => u.Donorsubs == 1 )
               .Limit(1)
               .ToList();
           
            if (query.Count() != 0)
            {
                var user = query.First();
                url = user.Url;
                user.Donorsubs = 0;
                collection.Update(user);
            }
            else
            {
                url = @"https://www.behance.net/Drmmz94";
            }

            return url;
        }

        internal bool IsRepeat(string url)
        {
            var query = collection.Query()
                .Where(u => u.Url == url);

            if (query.Count() != 0)
            {
                return true;
            }
            else
                return false;
        }

        internal bool UpdateUser(string url, Byte donorsubs, Byte formerfriend, Byte friend)
        {
            var query = collection.Query()
              .Where(u => u.Url == url)
              .Limit(1)
              .ToList();

            if (query.Count() != 0)
            {
                var user = query.First();
                user.Donorsubs = donorsubs;
                user.Formerfriend = formerfriend;
                user.Friend = friend;

                if (collection.Update(user))
                {
                    cons.WriteLine("User - update.");
                    return true;
                }
                else
                {
                    cons.WriteLine("User - error update.");
                    return false;
                }
            }
            else
            {
                cons.WriteLine("Update user not found.Add new user.");
                AddUser(url,donorsubs,formerfriend,friend);
                return false;
            }
        }

        [Table(Name = "ParseInstagramUsers")]
        public class BehanceUsers
        {
            [Column(IsPrimaryKey = true, IsDbGenerated = true)]
            public int Id { get; set; }
            public string Url { get; set; }
            public DateTime Date { get; set; }
            public Byte Donorsubs { get; set; }
            public Byte Formerfriend { get; set; }
            public Byte Friend { get; set; }
        }


    }
}

