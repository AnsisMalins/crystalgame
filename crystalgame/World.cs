using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;

namespace crystalgame
{
    public class World : IDisposable
    {
        int fpsTemp;
        long prevSecond;
        private EventWaitHandle running;

        public World()
        {
            Entities = new Collection<Entity>();
            running = new EventWaitHandle(false, EventResetMode.ManualReset);
            var thread = new Thread(DoWork);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        public Collection<Entity> Entities { get; set; }

        public int Fps { get; private set; }

        public Vector Gravity { get; set; }

        public bool IsRunning { get; set; }

        public Pegasus Pegasus { get; set; }

        public void Dispose()
        {
            running.Close();
        }

        public void Start()
        {
            running.Set();
            IsRunning = true;
        }

        public void Stop()
        {
            running.Reset();
            IsRunning = false;
        }

        private void DoWork(object state)
        {
            try
            {
                while (true)
                {
                    running.WaitOne();
                    long nowTicks = DateTime.Now.Ticks;
                    fpsTemp++;
                    StepSimulation();
                    if (nowTicks - prevSecond > 10000000)
                    {
                        Fps = fpsTemp;
                        fpsTemp = 0;
                        prevSecond += 10000000;
                    }
                    Thread.Sleep(1);
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void StepSimulation()
        {
            Pegasus.Run(this);
            foreach (Entity i in Entities) i.Run(this);
        }
    }
}