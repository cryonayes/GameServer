namespace GameServer.Threading
{
    internal static class ThreadManager
    {
        private static readonly List<Action> executeOnMainThread = new();
        private static readonly List<Action> executeCopiedOnMainThread = new();
        private static bool actionToExecuteOnMainThread;

        public static void ExecuteOnMainThread(Action _action)
        {
            if (_action == null)
                return;

            lock (executeOnMainThread)
            {
                executeOnMainThread.Add(_action);
                actionToExecuteOnMainThread = true;
            }
        }

        public static void UpdateMain()
        {
            if (!actionToExecuteOnMainThread) return;
            executeCopiedOnMainThread.Clear();
            lock (executeOnMainThread)
            {
                executeCopiedOnMainThread.AddRange(executeOnMainThread);
                executeOnMainThread.Clear();
                actionToExecuteOnMainThread = false;
            }

            foreach (var _t in executeCopiedOnMainThread)
                _t();
        }
    }
}
