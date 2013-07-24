using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Net;
using System;
using Newtonsoft.Json;
using CafeteiraFast.Models;

namespace CafeteiraDaFast.WP7
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // TODO: Ler dados do servico
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri(@"http://moonha/CafeteiraDaFast/Home/GetStatus/"));
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                try
                {
                    var status = JsonConvert.DeserializeObject<CafeteiraStatus>(e.Result);
                    lblMensagem.Text = status.Mensagem;
                }
                catch { }
            }
        }
    }
}