using System;
using System.Windows;
using Buddy;

namespace BuddyTest
{
    public static class Utilities
    {
        public static void CrossThreadMessageBox(string message)
        {
            Utilities.CrossThread(() => { MessageBox.Show(message); });
        }

        public static void CrossThread(Action action)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                action();
            });
        }

        public static void HandleAsyncResults<ResultType>(ResultType resultType, BuddyCallbackParams callbackParams, Action action, string errorMessage = null) where ResultType : class
        {
            if (resultType == null || callbackParams.Exception != null)
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    Utilities.CrossThreadMessageBox(errorMessage);
                }
            }
            else
            {
                action();
            }
        }
    }
}
