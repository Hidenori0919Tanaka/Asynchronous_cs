using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Thread_1
    {
        public void Thread_Method()
        {
            Thread threadA = new Thread(
                new ThreadStart(ThreadMethod)); // （1）

            threadA.Start(); // （2）

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                Console.Write(" B ");
            }
        }

        private void ThreadMethod()
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                Console.Write(" A ");
            }
        }
    }
}
