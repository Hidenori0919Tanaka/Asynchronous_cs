using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Task_1
    {
        public async Task HelloWorldAsync()
        {
            Console.WriteLine("[Async] HelloWorldAsync Start...\n");
            await Task.Run(() => {
                Console.WriteLine("[Async] 1");
                Thread.Sleep(1000);
                Console.WriteLine("[Async] 2");
                Thread.Sleep(1000);
                Console.WriteLine("[Async] 3");
                Thread.Sleep(1000);
            });
            Console.WriteLine("[Async] HelloWorldAsync End...\n");
        }
        public void Main(string[] args)
        {
            Console.WriteLine("[Main] Start...\n");
            Task task = HelloWorldAsync();
            Console.WriteLine("[Main] HelloWorldAsync Called...\n");
            task.Wait();
            Console.WriteLine("[Main] End...\n");
            while (true) ;
        }
    }
}
