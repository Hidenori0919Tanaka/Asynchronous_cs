using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Thread_2
    {
        public void Thread_Method()
        {
            ThreadedTextPrinter printer = new ThreadedTextPrinter(); // （1）
            printer.Print("A"); // （2）

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                Console.Write(" B ");
            }
        }
    }

    public class ThreadedTextPrinter // （3）
    {
        private string text;

        public void Print(string s) // （4）
        {
            text = s; // （5）

            Thread thread = new Thread(new ThreadStart(ThreadMethod));
            thread.Start();
        }

        // 別スレッドで動作させるメソッド
        private void ThreadMethod() // （6）
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                Console.Write(" {0} ", text); // （7）
            }
        }
    }
}
