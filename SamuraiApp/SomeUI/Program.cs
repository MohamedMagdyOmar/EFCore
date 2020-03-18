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
            QueringSimplaeData();
            Console.ReadLine();
        }

        private static void InsertSamurai()
        {
            var samurai = new Samurai { Name = "Julie" };
            var smallSamurai = new Samurai { Name = "mohamed" };
            var battle = new Battle{ Name = "Hettin", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(1) };

            using (var context = new SamuraiContext())
            {
                // below add command makes the "context" track the samurai object
                context.AddRange(samurai, smallSamurai, battle);

                // internal workflow is as follows:
                    //1- Examine each tracked object
                    //2- Read state of object
                    //3- workout sql commands(construct correct sql statements)
                    //4- execute each sql command in the database
                    //5- capture any returned data
                context.SaveChanges();
            }
        }

        private static void QueringSimplaeData()
        {
            using (var context = new SamuraiContext())
            {
                // will retrieve all samurais and insert them in the memory
                var allSamurais = context.Samurais.ToList();

                //this is another solution, but it will keep the connection to the database opened untill we retrieve all the data
                foreach(var samurai in context.Samurais)
                {
                    Console.WriteLine(samurai.Name);
                }
            }
        }
    }
}
