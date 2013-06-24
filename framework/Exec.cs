using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Windows.Threading;

namespace Utilities
{
    public static class Exec
    {
        private static bool initialized;
        private static Action<Action> mainThread;

        public static void Initialize(ISynchronizeInvoke control)
        {
            Guard.ArgumentNotNull(control, "control");
            Guard.ObjectNotInitialized(initialized);
            mainThread = (method) => control.BeginInvoke(method, null);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            initialized = true;
        }

        public static void Initialize(Dispatcher dispatcher)
        {
            Guard.ArgumentNotNull(dispatcher, "dispatcher");
            Guard.ObjectNotInitialized(initialized);
            mainThread = (method) => dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            initialized = true;
        }

        public static void OnMain(Action method)
        {
            Guard.ArgumentNotNull(method, "method");
            Guard.ObjectInitialized(initialized);
            mainThread(method);
        }

        public static void OnPool(Action method)
        {
            Guard.ArgumentNotNull(method, "method");
            ThreadPool.QueueUserWorkItem((state) =>
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
                method();
            });
        }

        public static void OnPool(Action method, Action callback)
        {
            Guard.ArgumentNotNull(method, "method");
            Guard.ArgumentNotNull(callback, "callback");
            Guard.ObjectInitialized(initialized);
            OnPool(() =>
            {
                method();
                OnMain(() => callback());
            });
        }

        public static void OnPool<T>(Func<T> method, Action<T> callback)
        {
            Guard.ArgumentNotNull(method, "method");
            Guard.ArgumentNotNull(callback, "callback");
            Guard.ObjectInitialized(initialized);
            OnPool(() =>
            {
                T result = method();
                OnMain(() => callback(result));
            });
        }
    }
}