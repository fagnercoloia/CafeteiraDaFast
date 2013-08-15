﻿using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using CafeteiraDaFast.Models;
using CafeteiraDaFast.TaskAgent;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Newtonsoft.Json;

namespace CafeteiraDaFast
{
    public partial class MainPage : PhoneApplicationPage
    {
        PeriodicTask periodicTask;
        ResourceIntensiveTask resourceIntensiveTask;

        string resourceIntensiveTaskName = "ResourceIntensiveAgent";
        public bool agentsAreEnabled = true;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            var status = default(CafeteiraStatus);

            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<CafeteiraStatus>("ultimoStatus", out status))
            {
                lblMensagem.Text = status.Mensagem;
            }

            // TODO: Ler dados do servico
            var webClient = new WebClient();
            webClient.DownloadStringCompleted += new DownloadStringCompletedEventHandler(webClient_DownloadStringCompleted);
            webClient.DownloadStringAsync(new Uri(@"http://moonha/CafeteiraDaFast/Home/GetStatus/"));

            lblMensagem.Text = "Obtendo status da Cafeteira da Fast";

            periodicTask = ScheduledActionService.Find(ScheduledAgent.TASK_NAME) as PeriodicTask;
            if (periodicTask == null)
            {
                StartPeriodicAgent();
            }
        }

        private void StartPeriodicAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            // Obtain a reference to the period task, if one exists
            periodicTask = ScheduledActionService.Find(ScheduledAgent.TASK_NAME) as PeriodicTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule
            if (periodicTask != null)
            {
                RemoveAgent(ScheduledAgent.TASK_NAME);
            }

            periodicTask = new PeriodicTask(ScheduledAgent.TASK_NAME);

            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            periodicTask.Description = "This demonstrates a periodic task.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(periodicTask);
#if DEBUG
                ScheduledActionService.LaunchForTest(ScheduledAgent.TASK_NAME, TimeSpan.FromSeconds(1));
#endif
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }

                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                { 
                }
            }
            catch (SchedulerServiceException)
            {
            }
        }

        private void RemoveAgent(string name)
        {
            try
            {
                ScheduledActionService.Remove(name);
            }
            catch (Exception)
            {
            }
        }

        private void StartResourceIntensiveAgent()
        {
            // Variable for tracking enabled status of background agents for this app.
            agentsAreEnabled = true;

            resourceIntensiveTask = ScheduledActionService.Find(resourceIntensiveTaskName) as ResourceIntensiveTask;

            // If the task already exists and background agents are enabled for the
            // application, you must remove the task and then add it again to update 
            // the schedule.
            if (resourceIntensiveTask != null)
            {
                RemoveAgent(resourceIntensiveTaskName);
            }

            resourceIntensiveTask = new ResourceIntensiveTask(resourceIntensiveTaskName);
            // The description is required for periodic agents. This is the string that the user
            // will see in the background services Settings page on the device.
            resourceIntensiveTask.Description = "This demonstrates a resource-intensive task.";

            // Place the call to Add in a try block in case the user has disabled agents.
            try
            {
                ScheduledActionService.Add(resourceIntensiveTask);
            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    MessageBox.Show("Background agents for this application have been disabled by the user.");
                    agentsAreEnabled = false;
                }
            }
            catch (SchedulerServiceException)
            {
            }
        }

        void webClient_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                try
                {
                    var status = JsonConvert.DeserializeObject<CafeteiraStatus>(e.Result);
                    status.Data = status.Data.AddHours(-3);
                    lblMensagem.Text = status.Mensagem;

                    ScheduledAgent.UpdateAppTile(status);
                }
                catch
                {
                    lblMensagem.Text = "Não foi possivel obter dados da Cafeteira da Fast";
                }
            }
        }
    }
}