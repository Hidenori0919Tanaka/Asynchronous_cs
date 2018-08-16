using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Task_4
    {
        static void Main1(string[] args)
        {
            var task1_1 = Task.Run(() => LongTimeMethod1_1("A")); // 1つ目の処理を別スレッドで開始
            System.Threading.Thread.Sleep(100); // ←結果が前後しないように入れてある（なくてもよい）
            var task1_2 = Task.Run(() => LongTimeMethod1_1("B")); // 2つ目の処理を別スレッドで開始
#if DEBUG
            Console.ReadKey();
#endif
        }

        // 実行に約1秒かかるメソッド（同期的に実行される通常のメソッド）
        static void LongTimeMethod1_1(string id)
        {
            Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
        }

        static object lockObj1 = new object();

        static void LongTimeMethod2_1(string id)
        {
            lock (lockObj1)
            {
                Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
            }
        }

        static void Main2(string[] args)
        {
            var task3 = LongTimeMethod2Async1("C"); // 1つ目の処理を開始（別スレッドで実行される）
            System.Threading.Thread.Sleep(100); // ←結果が前後しないように入れてある（なくてもよい）
            var task4 = LongTimeMethod2Async1("D"); // 2つ目の処理を開始（別スレッドで実行される）
#if DEBUG
            Console.ReadKey();
#endif
        }

        // 実行に約1秒かかるメソッド（非同期的に実行されるメソッド）
        static async Task LongTimeMethod2Async1(string id)
        {
            Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
            await Task.Delay(1000); // この行は別スレッドで実行される
                                    // これ以降は、元と同じスレッドで実行されるとは限らない
            Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
        }

        //static object lockObj2 = new object();

        //static async Task LongTimeMethod2Async2(string id)
        //{
        //    lock (lockObj2) // エラー「'await' 演算子は、lock ステートメント本体では使用できません」
        //    {
        //        Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
        //        await Task.Delay(1000); // この行は別スレッドで実行される
        //                                // これ以降は、元と同じスレッドで実行されるとは限らない
        //        Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
        //    }
        //}

        static System.Threading.SemaphoreSlim _semaphore
  = new System.Threading.SemaphoreSlim(1, 1);

        static async Task LongTimeMethod2Async3(string id)
        {
            await _semaphore.WaitAsync(); // ロックを取得する
            try
            {
                Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
                await Task.Delay(1000); // この行は別スレッドで実行される
                                        // これ以降は、元と同じスレッドで実行されるとは限らない
                Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
            }
            finally
            {
                _semaphore.Release(); // 違うスレッドでロックを解放してもOK
            }
        }
    }
}
