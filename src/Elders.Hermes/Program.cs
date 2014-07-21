using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Elders.Hermes
{
    class Program
    {
        static object locker = new object();
        static List<Thread> threads = new List<Thread>();
        static List<List<int>> items = new List<List<int>>();
        static List<ISequentialIntService> serviceslist = new List<ISequentialIntService>();

        static int GetNextValueFromRandomService(Guid gd, int value)
        {
            var svc = new Random().Next(0, 9);
            return serviceslist[svc].GetNextRevision(gd, value);
        }

        static void Main(string[] args)
        {
            var initialService = new SequentialIntService();

            serviceslist.Add(initialService);

            for (int i = 1; i < 9; i++)
            {
                var newSvc = new SequentialIntService(initialService);
                serviceslist.Add(newSvc);
            }

            var gd = Guid.NewGuid();

            for (int i = 0; i < 10; i++)
            {
                //var thread = new Thread(new ThreadStart(() =>
                //{
                    var nexValue = 0;
                    var list = new List<int>();
                    var initialStart = DateTime.UtcNow;

                    while (nexValue < 1000000)
                    {
                        nexValue = GetNextValueFromRandomService(gd, 0);

                        list.Add(nexValue);
                    }
                    Console.WriteLine(DateTime.UtcNow - initialStart);
                    lock (locker)
                    {
                        items.Add(list);
                    }
                //}
                //));

                //thread.Start();
                //threads.Add(thread);
            }

            Thread.Sleep(10000);

            CheckIntervals();

            Dictionary<int, object> awww = new Dictionary<int, object>();

            foreach (var item in items)
            {
                foreach (var ints in item)
                {
                    awww.Add(ints, item);
                }
            }

            Console.WriteLine(awww.Count());

            ContainsAllInts(awww);

            CheckIfEveryNumberIsAvailable();

            Console.WriteLine("Success");
            Console.ReadLine();
        }

        private static void CheckIfEveryNumberIsAvailable()
        {
            for (int i = int.MinValue; i <= int.MaxValue; i++)
            {
                var nodes = new List<ISequentialIntServiceNode>();

                foreach (var service in serviceslist)
                {
                    var node = service.GetServiceNode(i);

                    if (node != null)
                        nodes.Add(node);
                }

                if (nodes.Count == 0)
                    Console.WriteLine("Can't find node for: " + i);

                if (nodes.Count > 1)
                    Console.WriteLine("There are multiple nodes for:" + i);
            }
        }

        private static void CheckIntervals()
        {
            var dict = new Dictionary<int, string>();

            foreach (var service in serviceslist)
            {
                var nodes = service.GetServiceNodes();

                foreach (var node in nodes)
                {
                    foreach (var interval in node.Intervals)
                    {
                        dict.Add(interval.Start, "Start");
                        dict.Add(interval.End, "End");
                    }
                }
            }
        }

        private static List<int> ContainsAllInts(Dictionary<int, object> dict)
        {
            var ints = new List<int>();

            for (int i = 1; i < dict.Count; i++)
            {
                if (!dict.ContainsKey(i))
                {
                    ints.Add(i);
                }
            }

            return ints;
        }
    }
}