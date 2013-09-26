#define DEBUG_AGENT

using System;
using System.Linq;
using CafeteiraDaFast.Models;
using Microsoft.Phone.Shell;

namespace CafeteiraDaFast.TaskAgent
{
    public class ScheduledAgent // : ScheduledTaskAgent
    {
        public const string TASK_NAME = "CafeteiraDaFastTask";
        public const string URLSTATUS = "http://cafeteiradafast.azurewebsites.net/Home/GetStatus";
        public const string URLINICIAR = "http://cafeteiradafast.azurewebsites.net/Home/Iniciar";
        public const string URLPRONTO = "http://cafeteiradafast.azurewebsites.net/Home/Pronto";
        /*
        private static volatile bool _classInitialized;
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        public ScheduledAgent()
        {
            if (!_classInitialized)
            {
                _classInitialized = true;

                // Subscribe to the managed exception handler
                Deployment.Current.Dispatcher.BeginInvoke(delegate
                {
                    Application.Current.UnhandledException += ScheduledAgent_UnhandledException;
                });
            }
        }

        /// Code to execute on Unhandled Exceptions
        private void ScheduledAgent_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        protected override void OnInvoke(ScheduledTask task)
        {
            try
            {
                var webClient = new WebClient();
                webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
                webClient.DownloadStringAsync(new Uri(URLSTATUS));
            }
            catch
            {
                NotifyComplete();
            }
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                var ultimoStatus = ObterUltimoStatus();
                //IsolatedStorageSettings.ApplicationSettings.TryGetValue<CafeteiraStatus>("ultimoStatus", out ultimoStatus);

                var status = JsonConvert.DeserializeObject<CafeteiraStatus>(e.Result);
                status.Data = status.Data.AddHours(-3);
                UpdateAppTile(status);

                if (status.Status == CafeteiraStatus.eStatus.Pronto &&
                    (status.Data - ultimoStatus.Data) > TimeSpan.FromMilliseconds(30000))
                {
                    var toast = new ShellToast();
                    toast.Title = "Cafeteira da FAST";
                    toast.Content = "Cafe Pronto";
                    toast.Show();
                }

                SalvarUltimoStatus(status);
            }
            finally
            {
                NotifyComplete();
            }
        }

        private static CafeteiraStatus ObterUltimoStatus()
        {
            var fileName = "CafeteiraStatus.xml";
            var storage = IsolatedStorageFile.GetUserStoreForApplication();

            var status = new CafeteiraStatus();
            if (storage.FileExists(fileName))
            {
                using (var file = storage.OpenFile(fileName, FileMode.Open))
                {
                    var xml = new XmlSerializer(typeof(CafeteiraStatus));
                    status = xml.Deserialize(file) as CafeteiraStatus;
                }
            }
            else
            {
                status.Data = DateTime.Now;
            }
            return status;
        }

        private static void SalvarUltimoStatus(CafeteiraStatus status)
        {
            var fileName = "CafeteiraStatus.xml";
            var storage = IsolatedStorageFile.GetUserStoreForApplication();
            using (var file = storage.OpenFile(fileName, FileMode.Create))
            {
                var xml = new XmlSerializer(typeof(CafeteiraStatus));
                xml.Serialize(file, status);
            }
        }
        */

        public static void UpdateAppTile(CafeteiraStatus status)
        {
            var message = status.Status + Environment.NewLine + status.Data.ToString("dd/MM/yyyy HH:mm");

            var appTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (appTile != null)
            {
                var tileData = new StandardTileData
                {
                    BackContent = message
                };

                appTile.Update(tileData);
            }
        }
        
    }
}