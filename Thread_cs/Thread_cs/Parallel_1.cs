using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Parallel_1
    {
        // 並列処理させたいメソッド
        public int Work(int n)
        {
            string startTime = $"{DateTimeOffset.Now:ss.fff}";
            Console.WriteLine($"n={n}, TaskID={Task.CurrentId}, start:{startTime}");

            int delayCount = (new Random()).Next(10000, 1000000);
            System.Threading.Thread.SpinWait(delayCount); // ランダムな時間を待機する

            if (n == 7) // この例外をキャッチする方法は後述する
                throw new ApplicationException($"{nameof(n)}={n}は計算できません");

            string endTime = $"{DateTimeOffset.Now:ss.fff}";
            Console.WriteLine($"n={n}, TaskID={Task.CurrentId},   end:{endTime}");

            return n;
        }

        public void Main(string[] args)
        {
            Console.WriteLine("通常のfor文");
            for (int n = 1; n < 6; n++)
            {
                Work(n);
            }
            // 出力例：
            // 通常のfor文
            // n=1, TaskID=, start:29.283
            // n=1, TaskID=,   end:29.334
            // n=2, TaskID=, start:29.334
            // n=2, TaskID=,   end:29.433
            // n=3, TaskID=, start:29.433
            // n=3, TaskID=,   end:29.519
            // n=4, TaskID=, start:29.522
            // n=4, TaskID=,   end:29.532
            // n=5, TaskID=, start:29.533
            // n=5, TaskID=,   end:29.594

            Console.WriteLine("Parallel.For()");
            Parallel.For(1, 6, n => {
                Work(n);
            });
            Console.WriteLine("Parallel.For()終了");
            // 出力例：
            // Parallel.For()
            // n=1, TaskID=1, start:29.637
            // n=2, TaskID=2, start:29.640
            // n=2, TaskID=2,   end:29.643
            // n=3, TaskID=2, start:29.644
            // n=1, TaskID=1,   end:29.647
            // n=4, TaskID=1, start:29.699
            // n=3, TaskID=2,   end:29.665
            // n=5, TaskID=3, start:29.700
            // n=5, TaskID=3,   end:29.799
            // n=4, TaskID=1,   end:29.799
            // Parallel.For()終了

            Console.WriteLine("Parallel.ForEach()");
            Parallel.ForEach(Enumerable.Range(1, 5), n => {
                Work(n);
            });
            // 出力例：
            // Parallel.ForEach()
            // n=1, TaskID=9, start:47.834
            // n=2, TaskID=10, start:47.836
            // n=3, TaskID=11, start:47.836
            // n=2, TaskID=10,   end:47.868
            // n=3, TaskID=11,   end:47.872
            // n=4, TaskID=12, start:47.849
            // n=5, TaskID=13, start:47.851
            // n=1, TaskID=9,   end:47.910
            // n=5, TaskID=13,   end:48.031
            // n=4, TaskID=12,   end:48.056

            Console.WriteLine("PLINQ：Parallel.ForEach()と同じ");
            Enumerable.Range(1, 5).AsParallel().ForAll(n => {
                Work(n);
            });
            // 出力例：
            // PLINQ：Parallel.ForEach()と同じ
            // n=1, TaskID=21, start:48.080
            // n=2, TaskID=19, start:48.080
            // n=4, TaskID=20, start:48.088
            // n=1, TaskID=21,   end:48.135
            // n=5, TaskID=21, start:48.173
            // n=3, TaskID=18, start:48.081
            // n=3, TaskID=18,   end:48.231
            // n=2, TaskID=19,   end:48.207
            // n=4, TaskID=20,   end:48.193
            // n=5, TaskID=21,   end:48.308

            Console.WriteLine("例外の捕まえ方-その1（ループ内）");
            Parallel.For(6, 11, n => { // n=7のとき例外が出る
                try
                {
                    Work(n);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"n={n}で例外：{ex.GetType().Name}（{ex.Message}）");
                    Console.WriteLine(ex.StackTrace);
                }
            });
            // 出力例：
            // 例外の捕まえ方-その1（ループ内）
            // n=6, TaskID=17, start:01.816
            // n=7, TaskID=18, start:01.828
            // n=8, TaskID=19, start:01.828
            // n=9, TaskID=20, start:01.832
            // n=6, TaskID=17,   end:01.890
            // n=9, TaskID=20,   end:01.905
            // n=10, TaskID=21, start:01.852
            // n=8, TaskID=19,   end:01.923
            // n=7で例外：ApplicationException（n=7は計算できません）
            // n=10, TaskID=21,   end:01.995
            //    場所 Program.Work(Int32 n) 場所 C:\……省略……\Program.cs:行 17
            //    場所 Program.<>c.<Main>b__1_3(Int32 n) 場所……省略……\Program.cs:行 112

            Console.WriteLine("例外の捕まえ方-その2（ループ外）");
            try
            {
                Parallel.For(6, 11, n => { // n=7のとき例外が出る
                    Work(n);
                });
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions)
                {
                    Console.WriteLine($"ループ内で例外：{ex.GetType().Name}（{ex.Message}）");
                    Console.WriteLine(ex.StackTrace);
                }
            }
            // 出力例：
            // 例外の捕まえ方-その2（ループ外）
            // n=6, TaskID=26, start:32.594
            // n=7, TaskID=27, start:32.599
            // n=8, TaskID=28, start:32.618
            // n=10, TaskID=30, start:32.646
            // n=9, TaskID=29, start:32.627
            // n=10, TaskID=30,   end:32.712
            // n=6, TaskID=26,   end:32.630
            // n=9, TaskID=29,   end:32.754
            // n=8, TaskID=28,   end:32.671
            // ループ内で例外：ApplicationException（n=7は計算できません）
            //    場所 Program.Work(Int32 n) 場所 C:\……省略……\Program.cs:行 17
            //    場所 Program.<>c.<Main>b__1_4(Int32 n) 場所……省略……\Program.cs:行 130
            //    場所 System.Threading.Tasks.Parallel.<>……省略…….<ForWorker>b__1()
            //    場所 System.Threading.Tasks.Task.InnerInvoke()
            //    場所 System.Threading.Tasks.Task.InnerInvokeWithArg(Task childTask)
            //    場所 System.Threading.Tasks.Task.<>……省略…….<ExecuteSelfReplicating>b__0(Object )

            Console.WriteLine("Task.Runで並列実行する例");
            var taskList = new List<Task>(); // 複数の非同期処理を管理するためのコレクション
            for (int n = 6; n < 11; n++)
            {
                int i = n; // ループ変数はキャッシュしてから非同期処理に渡さねばならない
                taskList.Add(Task.Run(() => Work(i)));
            }
            Console.WriteLine("並列実行中…");
            try
            {
                Task.WaitAll(taskList.ToArray()); // 全ての処理が終わるまで待機する
            }
            catch (AggregateException aex)
            {
                foreach (var ex in aex.InnerExceptions)
                {
                    Console.WriteLine($"ループ内で例外：{ex.GetType().Name}（{ex.Message}）");
                    Console.WriteLine(ex.StackTrace);
                }
            }

            Console.WriteLine($"end of Main:{DateTimeOffset.Now:ss.fff}");
            // 出力例：
            // Task.Runで並列実行する例
            // 並列実行中…
            // n=7, TaskID=2, start:33.590
            // n=8, TaskID=3, start:33.596
            // n=9, TaskID=4, start:33.598
            // n=6, TaskID=1, start:33.603
            // n=8, TaskID=3,   end:33.635
            // n=6, TaskID=1,   end:33.662
            // n=10, TaskID=5, start:33.678
            // n=9, TaskID=4,   end:33.628
            // n=10, TaskID=5,   end:33.692
            // ループ内で例外：ApplicationException（n=7は計算できません）
            //    場所 Program.Work(Int32 n) 場所 ……省略……\Program.cs:行 17
            //    場所 Program.……省略…….<Main>b__0() 場所 ……省略……\Program.cs: 行 32
            //    場所 System.Threading.Tasks.Task`1.InnerInvoke()
            //    場所 System.Threading.Tasks.Task.Execute()
            // end of Main:33.763

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
