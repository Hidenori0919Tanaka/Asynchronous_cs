using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thread_cs
{
    public class Task_5
    {
        static System.Threading.SemaphoreSlim _semaphore
  = new System.Threading.SemaphoreSlim(1, 1);

        static async Task LongTimeMethod2Async1(string id)
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

        static AsyncLock _asyncLock = new AsyncLock();

        static async Task LongTimeMethod2Async2(string id)
        {
            using (await _asyncLock.LockAsync())
            {
                Console.WriteLine("{0} - ({1}) START ", DateTime.Now.ToString("ss.fff"), id);
                await Task.Delay(1000);
                Console.WriteLine("{0} - ({1}) END", DateTime.Now.ToString("ss.fff"), id);
            }
        }
    }

    public sealed class AsyncLock
    {
        private readonly System.Threading.SemaphoreSlim m_semaphore
          = new System.Threading.SemaphoreSlim(1, 1);
        private readonly Task<IDisposable> m_releaser;

        public AsyncLock()
        {
            m_releaser = Task.FromResult((IDisposable)new Releaser(this));
        }

        public Task<IDisposable> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            return wait.IsCompleted ?
                    m_releaser :
                    wait.ContinueWith(
                      (_, state) => (IDisposable)state,
                      m_releaser.Result,
                      System.Threading.CancellationToken.None,
                      TaskContinuationOptions.ExecuteSynchronously,
                      TaskScheduler.Default
                    );
        }
        private sealed class Releaser : IDisposable
        {
            private readonly AsyncLock m_toRelease;
            internal Releaser(AsyncLock toRelease) { m_toRelease = toRelease; }
            public void Dispose() { m_toRelease.m_semaphore.Release(); }
        }
    }
}
