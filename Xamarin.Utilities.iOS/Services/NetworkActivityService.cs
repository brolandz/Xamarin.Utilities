using MonoTouch.UIKit;
using Xamarin.Utilities.Core.Services;
using System;
using System.Reactive.Disposables;

namespace Xamarin.Utilities.Services
{
    public class NetworkActivityService : INetworkActivityService
    {
        private static UIApplication MainApp = UIApplication.SharedApplication;

        // Since we are a multithreaded application and we could have many
        // different outgoing network connections (api.twitter, images,
        // searches) we need a centralized API to keep the network visibility
        // indicator state
        static readonly object NetworkLock = new object();
        static int _active;

        public void PushNetworkActive()
        {
            lock (NetworkLock)
            {
                _active++;
                MainApp.NetworkActivityIndicatorVisible = true;
            }
        }

        public void PopNetworkActive()
        {
            lock (NetworkLock)
            {
                if (_active == 0)
                    return;

                _active--;
                if (_active == 0)
                    MainApp.NetworkActivityIndicatorVisible = false;
            }
        }

        public IDisposable ActivateNetwork()
        {
            PushNetworkActive();
            return Disposable.Create(PopNetworkActive);
        }
    }
}