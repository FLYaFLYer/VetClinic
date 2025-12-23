using System;
using System.Threading;

namespace VetClinic.Utils
{
    public static class AutoRefreshHelper
    {
        public static bool AutoRefreshEnabled { get; set; } = true;
        public static int RefreshIntervalSeconds { get; set; } = 30;

        private static Timer _refreshTimer;
        private static Action _refreshAction;

        public static void StartAutoRefresh(Action refreshAction)
        {
            _refreshAction = refreshAction;

            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }

            if (AutoRefreshEnabled)
            {
                _refreshTimer = new Timer(
                    _ => RefreshData(),
                    null,
                    TimeSpan.FromSeconds(RefreshIntervalSeconds),
                    TimeSpan.FromSeconds(RefreshIntervalSeconds));
            }
        }

        public static void StopAutoRefresh()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Dispose();
                _refreshTimer = null;
            }
        }

        private static void RefreshData()
        {
            try
            {
                if (AutoRefreshEnabled && _refreshAction != null)
                {
                    _refreshAction.Invoke();
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем выполнение
                Console.WriteLine($"Ошибка автообновления: {ex.Message}");
            }
        }
    }
}