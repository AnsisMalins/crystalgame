using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Utilities;

namespace crystalgame
{
    public class World : ViewModel, IDisposable
    {
        private int fpsTemp;
        private long prevSecond;
        private EventWaitHandle running;
        private Canvas view;

        public World(Canvas view)
        {
            Gravity = GetGravity(view);
            Speed = GetSpeed(view);

            Entities = new Collection<Entity>();
            List<FrameworkElement> toRemove = new List<FrameworkElement>(view.Children.Count);
            foreach (UIElement i in view.Children)
            {
                var entityView = i as FrameworkElement;
                if (entityView == null) continue;
                Type entityType = Entity.GetType(entityView);
                if (entityType == null) continue;

                var entity = Activator.CreateInstance(entityType, entityView) as Entity;
                if (entity == null) continue;

                var pegasus = entity as Pegasus;
                if (pegasus != null) Pegasus = pegasus;
                
                Entities.Add(entity);
                toRemove.Add(entityView);
            }

            foreach (var i in toRemove) view.Children.Remove(i);

            foreach (Entity entity in Entities)
            {
                FrameworkElement entityView = entity.CreateView();
                if (entityView != null) view.Children.Add(entityView);
            }

            view.DataContext = this;

            running = new EventWaitHandle(false, EventResetMode.ManualReset);
            var thread = new Thread(DoWork);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
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

        public Entity FindName(string name)
        {
            var foundView = view.FindName(name) as FrameworkElement;
            return foundView != null ? foundView.DataContext as Entity : null;
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
            foreach (Entity i in Entities) i.Simulate(this);
        }

        private void Render()
        {
            if (Player != null) Player.Render();
            foreach (Entity i in Entities) i.Render();
        }
    }
}