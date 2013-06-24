using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace crystalgame
{
    public class World : ViewModel, IDisposable
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

        public World(Canvas view)
            : this()
        {
            Gravity = GetGravity(view);
            Speed = GetSpeed(view);

            foreach (UIElement i in view.Children)
            {
                var entityView = i as FrameworkElement;
                if (entityView == null) continue;
                Entity entity = null;

                if (entityView is CameraView) entity = new Camera(entityView);
                else if (entityView is CloudView) entity = new Cloud(entityView);
                else if (entityView is PegasusView) entity = new Pegasus(entityView);
                else continue;

                if (entity is Pegasus) Pegasus = entity as Pegasus;
                else Entities.Add(entity);
            }

            view.DataContext = this;
        }



        public Collection<Entity> Entities { get; private set; }

        public int Fps { get; private set; }

        public Vector Gravity { get; set; }

        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            set { Set(ref _IsRunning, value); }
        }

        public Pegasus Pegasus { get; set; }

        public Player Player { get; set; }

        public double Speed { get; set; }

        public void Dispose()
        {
            running.Close();
        }

        public static Vector GetGravity(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (Vector)view.GetValue(GravityProperty);
        }

        public static double GetSpeed(FrameworkElement view)
        {
            Guard.ArgumentNotNull(view, "view");
            return (double)view.GetValue(SpeedProperty);
        }

        public void Start()
        {
            prevSecond = DateTime.Now.Ticks;
            running.Set();
            IsRunning = true;
        }

        public static void SetGravity(FrameworkElement view, Vector value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(GravityProperty, value);
        }

        public static void SetSpeed(FrameworkElement view, double value)
        {
            Guard.ArgumentNotNull(view, "view");
            view.SetValue(SpeedProperty, value);
        }

        public void Stop()
        {
            running.Reset();
            IsRunning = false;
        }

        public static readonly DependencyProperty GravityProperty
            = DependencyProperty.RegisterAttached("Gravity", typeof(Vector), typeof(World));

        public static readonly DependencyProperty SpeedProperty
            = DependencyProperty.RegisterAttached("Speed", typeof(double), typeof(World));

        private void DoWork(object state)
        {
            try
            {
                while (true)
                {
                    running.WaitOne();
                    long nowTicks = DateTime.Now.Ticks;
                    fpsTemp++;
                    Simulate();
                    Exec.OnMain(() => Render());
                    if (nowTicks - prevSecond > 10000000)
                    {
                        Fps = fpsTemp;
                        fpsTemp = 0;
                        prevSecond += 10000000;
                        OnPropertyChanged("Fps");
                    }
                    Thread.Sleep(16);
                }
            }
            catch (ObjectDisposedException) { }
        }

        private void Simulate()
        {
            if (Player != null) Player.Simulate(this);
            if (Pegasus != null) Pegasus.Simulate(this);
            foreach (Entity i in Entities) i.Simulate(this);
        }

        private void Render()
        {
            if (Player != null) Player.Render();
            if (Pegasus != null) Pegasus.Render();
            foreach (Entity i in Entities) i.Render();
        }
    }
}