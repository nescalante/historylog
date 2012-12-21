using HistoryLog;
using System;
using System.Configuration;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestEntity
            {
                Id = 1,
                Date = DateTime.Now,
                Name = "test1",
                Number = 1,
            };
            var test2 = new TestEntity
            {
                Id = 2,
                Date = DateTime.Now,
                Name = "test2",
                Number = 1,
            };
            var test3 = new TestEntity
            {
                Id = 2,
                Date = DateTime.Now,
                Name = "test3",
                Number = 1,
            };

            var lm = new LogManager("testapp", ConfigurationManager.ConnectionStrings["LM"].ConnectionString);
            lm.Log(test);
            lm.Log(test2);
            lm.Log(test3);

            var h1 = lm.GetHistory(test);
            var h2 = lm.GetHistory(test2);
        }
    }
}
