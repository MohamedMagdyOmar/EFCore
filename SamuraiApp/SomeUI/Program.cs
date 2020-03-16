using SamuraiApp.Data;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SomeUI
{
    class Program
    {
        static void Main(string[] args)
        {
            InsertSamurai();
            Console.ReadLine();
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            using (var context = new SamuraiContext())
            {
                // below add command makes the "context" track the samurai object
                context.Samurais.Add(samurai);

                // internal workflow is as follows:
                    //1- Examine each tracked object
                    //2- Read state of object
                    //3- workout sql commands(construct correct sql statements)
                    //4- execute each sql command in the database
                    //5- capture any returned data
                context.SaveChanges();
            }
        }
    }
}
