﻿using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using System.Net;
using System;
using Newtonsoft.Json;
using CafeteiraDaFast.Models;

namespace CafeteiraDaFast
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

            lblMensagem.Text = "Obtendo status da Cafeteira da Fast";
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
                catch {
                    lblMensagem.Text = "Não foi possivel obter dados da Cafeteira da Fast";
                }
            }
        }
    }
}