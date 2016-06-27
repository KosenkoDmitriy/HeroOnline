using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LOLClient
{
    class Program
    {
        static void Main()
        {
            List<Worker> workerList = new List<Worker>();

            for (int i = 200; i < 400; i++)
            {
                Worker workerObject = new Worker(i);
                workerList.Add(workerObject);
                workerObject.Start();
            }

            Thread.Sleep(1000);

            bool allStop = false;
            while (!allStop)
            {
                allStop = true;
                foreach (Worker workerObject in workerList)
                {
                    if (workerObject.IsAlive)
                    {
                        allStop = false;
                        break;
                    }
                }
            }

            Console.Write("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
