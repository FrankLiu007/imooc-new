using System.Windows;
using Awesomium.Core;

namespace IMOOC
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var window = new LoginWindow();
            window.ShowDialog();
            
            if (window.State == "online")
            {
                base.OnStartup(e);
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else if (window.State=="offline")
            {
                base.OnStartup(e);
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Shutdown();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Make sure we shutdown the core last.
            if (WebCore.IsInitialized)
                WebCore.Shutdown();

            base.OnExit(e);
        }
    }
}
