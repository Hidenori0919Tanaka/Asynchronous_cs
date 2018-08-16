using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Task_3
    {
        static async Task SampleMethod1Async()
        {
            await Task.Delay(200);
            Console.WriteLine("SampleMethod1Asyncで例外を発生");
            throw new InvalidOperationException("SampleMethod1Asyncの例外");
        }

        static async Task SampleMethod2Async()
        {
            await Task.Delay(100);
            Console.WriteLine("SampleMethod2Asyncで例外を発生");
            throw new InvalidOperationException("SampleMethod2Asyncの例外");
        }

        static async void SampleMethod3Async()
        {
            await Task.Delay(100);
            Console.WriteLine("SampleMethod3Asyncで例外を発生");
            throw new InvalidOperationException("SampleMethod3Asyncの例外");
        }

        static async Task Main(string[] args)
        {
            try
            {
                await SampleMethod1Async();
                // 出力：SampleMethod1Asyncで例外を発生
                await SampleMethod2Async(); // この行は実行されない
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"{e.GetType().Name} - {e.Message}");
                // 出力：InvalidOperationException - SampleMethod1Asyncの例外
            }

            var task1_1 = SampleMethod1Async();
            try
            {
                task1_1.Wait();
                // 出力：SampleMethod1Asyncで例外を発生
            }
            catch (AggregateException ae)
            {
                Console.WriteLine($"{ae.GetType().Name} - {ae.Message}");
                // 出力：AggregateException - 1 つ以上のエラーが発生しました。

                foreach (Exception e in ae.InnerExceptions)
                    Console.WriteLine($"{e.GetType().Name} - {e.Message}");
                // 出力：InvalidOperationException - SampleMethod1Asyncの例外
            }

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                Console.WriteLine($"UnhandledException - {ex.Message}");
                // 出力：UnhandledException - SampleMethod3Asyncの例外
#if DEBUG
                Console. ReadKey();
#endif
                Environment.Exit(1); // プログラム終了
            };

            try
            {
                SampleMethod3Async();
                // 出力：SampleMethod3Asyncで例外を発生
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType().Name} - {e.Message}");
                // ↑この行は実行されない
            }

            var task2_1 = SampleMethod1Async();
            var task2_2 = SampleMethod2Async();
            try
            {
                await Task.WhenAll(task2_1, task2_2);
                // 出力：SampleMethod2Asyncで例外を発生
                // 出力：SampleMethod1Asyncで例外を発生
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine($"{e.GetType().Name} - {e.Message}");
                // 出力：InvalidOperationException - SampleMethod1Asyncの例外
            }

            var task3_1 = SampleMethod1Async();
            var task3_2 = SampleMethod2Async();
            try
            {
                Task.WhenAll(task3_1, task3_2).Wait();
                // 出力：SampleMethod2Asyncで例外を発生
                // 出力：SampleMethod1Asyncで例外を発生
            }
            catch (AggregateException ae)
            {
                Console.WriteLine($"{ae.GetType().Name} - {ae.Message}");
                // 出力：AggregateException - 1 つ以上のエラーが発生しました。

                foreach (Exception e in ae.InnerExceptions)
                    Console.WriteLine($"{e.GetType().Name} - {e.Message}");
                // 出力：InvalidOperationException - SampleMethod1Asyncの例外
                // 出力：InvalidOperationException - SampleMethod2Asyncの例外
            }

            var task4_1 = SampleMethod1Async();
            var task4_2 = SampleMethod2Async();
            var allTasks = Task.WhenAll(task4_1, task4_2);
            try
            {
                await allTasks;
                // 出力：SampleMethod2Asyncで例外を発生
                // 出力：SampleMethod1Asyncで例外を発生
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ex] {ex.GetType().Name} - {ex.Message}");
                // 出力：[ex] InvalidOperationException - SampleMethod1Asyncの例外

                foreach (Exception e in allTasks.Exception.InnerExceptions)
                    Console.WriteLine($"[InnerExceptions] {e.GetType().Name} - {e.Message}");
                // 出力：[InnerExceptions] InvalidOperationException - SampleMethod1Asyncの例外
                // 出力：[InnerExceptions] InvalidOperationException - SampleMethod2Asyncの例外
            }
#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
