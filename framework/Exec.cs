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
            if (initialized) throw new AlreadyInitializedException("Exec");
            mainThread = (method) => control.BeginInvoke(method, null);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            initialized = true;
        }

        public static void Initialize(Dispatcher dispatcher)
        {
            Guard.ArgumentNotNull(dispatcher, "dispatcher");
            if (initialized) throw new AlreadyInitializedException("Exec");
            mainThread = (method) => dispatcher.BeginInvoke(DispatcherPriority.Normal, method);
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            initialized = true;
        }

        public static void OnMain(Action method)
        {
            Guard.ArgumentNotNull(method, "method");
            if (!initialized) throw new NotInitializedException("Exec");
            mainThread(method);
        }

        public static void OnPool(Action method)
        {
            Guard.ArgumentNotNull(method, "method");
            if (!initialized) throw new NotInitializedException("Exec");
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
            if (!initialized) throw new NotInitializedException("Exec");
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
            if (!initialized) throw new NotInitializedException("Exec");
            OnPool(() =>
            {
                T result = method();
                OnMain(() => callback(result));
            });
        }
    }
}