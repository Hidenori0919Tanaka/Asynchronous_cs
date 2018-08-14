using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Thread_4
    {
        // 戻り値とパラメータのあるデリゲート
        delegate DateTime ThreadMethodDelegate(string c); // （1）
        static ThreadMethodDelegate threadMethodDelegate; // （2）

        public void Thread_Method()
        {
            threadMethodDelegate
                = new ThreadMethodDelegate(ThreadMethod); // （3）

            // デリゲートによるスレッド処理呼び出し
            threadMethodDelegate.BeginInvoke(".",
              new AsyncCallback(MyCallback), DateTime.Now); // （4）

            Console.ReadLine();
        }

        // 別スレッドで呼び出されるメソッド
        private static DateTime ThreadMethod(string c)
        {
            // 10ミリ秒ごとに100回cを出力
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                Console.Write(c);
            }
            return DateTime.Now;
        }

        // スレッド処理終了後に呼び出されるコールバック・メソッド
        private static void MyCallback(IAsyncResult ar) // （5）
        {
            DateTime result = threadMethodDelegate.EndInvoke(ar); // （6）
            DateTime beginTime = (DateTime)ar.AsyncState; // （7）

            Console.WriteLine();
            Console.WriteLine(
              "{0}に処理を開始し、{1}に処理を完了しました。",
              beginTime, result);
        }
    }
}
