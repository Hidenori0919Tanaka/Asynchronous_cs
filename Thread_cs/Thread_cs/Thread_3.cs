using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Thread_3
    {
        public void Thread_Method()
        {
            // ThreadMethodをスレッドプールで実行できるように
            // WaitCallbackデリゲートを作成
            WaitCallback waitCallback
              = new WaitCallback(ThreadMethod); // （1）

            // スレッドプールに登録
            ThreadPool.QueueUserWorkItem(waitCallback, "A"); // （2）
            ThreadPool.QueueUserWorkItem(waitCallback, "B"); // （3）

            //何かキーが押されるまで、プログラムを実行
            Console.ReadLine();

        }

        private static void ThreadMethod(object state) // （4）
        {
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(5);
                Console.Write(" {0} ", state);
            }
        }
    }
}
