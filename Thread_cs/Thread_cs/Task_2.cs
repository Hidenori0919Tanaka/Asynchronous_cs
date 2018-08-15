using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Task_2
    {
        private void HeavyMethod(int x)
        {
            Thread.Sleep(10 * (100 - x)); // てきとーに時間を潰す
            Console.WriteLine(x);
        }

        public void RunHeavyMethodSync() // 比較のため、ただの同期メソッド
        {
            for (var i = 0; i < 10; i++)
            {
                var x = i;
                HeavyMethod(x);
            }
        }

        public async Task RunHeavyMethodAsync1()
        {
            for (var i = 0; i < 10; i++)
            {
                var x = i;
                await Task.Run(() => HeavyMethod(x)); // 「HeavyMethodを実行する」というタスクを開始し、完了するまで待機
            } // を、10回繰り返す
        } // というタスクを表す
          // ので、これは順次動作であり、並列ではない。

        public async void RunHeavyMethodAsync2() // RunHeavyMethodAsync1の戻り値がvoidになっただけ
        {
            for (var i = 0; i < 10; i++)
            {
                var x = i;
                await Task.Run(() => HeavyMethod(x));
            }
        } // 動作はRunHeavyMethodAsync1と同じだけど、HeavyMethodの実行がいつ完了するのか知ることができない。つらい。

        public void RunHeavyMethodParallel1() // asyncじゃない
        {
            for (var i = 0; i < 10; i++)
            {
                var x = i;
                Task.Run(() => HeavyMethod(x)); // HeavyMethodを開始せよという命令
            } // を、10回繰り返すだけ
        } // なので、これは並列動作になる。Task.Runが投げっぱなしなので、HeavyMethodの状態がわからなくてつらい。

        public Task RunHeavyMethodParallel2() // asyncじゃないけど、戻り値がTask
        {
            var tasks = new List<Task>(); // TaskをまとめるListを作成
            for (var i = 0; i < 10; i++)
            {
                var x = i;
                var task = Task.Run(() => HeavyMethod(x)); // HeavyMethodを開始するというTask
                tasks.Add(task); // を、Listにまとめる
            }
            return Task.WhenAll(tasks); // 全てのTaskが完了した時に完了扱いになるたった一つのTaskを作成
        } // 非同期メソッドではないが、戻り値がTaskなので、このメソッドは一つのタスクを表しているといえる。
    }
}
