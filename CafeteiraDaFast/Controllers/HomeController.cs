using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Xml.Serialization;
using CafeteiraDaFast.Components;
using CafeteiraDaFast.Models;
using CafeteiraDaFast.ViewModels;
using SysIO = System.IO;

namespace CafeteiraDaFast.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        const string INDEX_ACTIONNAME = "Index";

        public ActionResult Index()
        {
            var statusCafeteira = ObterCateteiraStatus();
            if (statusCafeteira.Status == CafeteiraStatus.eStatus.Iniciado && (DateTime.Now - statusCafeteira.Data).TotalMinutes > CafeteiraStatus.TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS)
            {
                return Pronto();
            }

            var tempoTotalCafePronto = CafeteiraStatus.TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS + CafeteiraStatus.TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS;
            var tempoStatus = (DateTime.Now - statusCafeteira.Data);
            var viewModel = new IndexViewModel
            {
                Mensagem = statusCafeteira.Mensagem,
                MensagemErro = TempData["ErroMessage"] as string,
                BotaoIniciarVisivel = statusCafeteira.Status == CafeteiraStatus.eStatus.None ||
                    (statusCafeteira.Status == CafeteiraStatus.eStatus.Pronto && tempoStatus.TotalMinutes > CafeteiraStatus.TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS),
                BotaoProntoVisivel = statusCafeteira.Status != CafeteiraStatus.eStatus.Pronto || tempoStatus.TotalMinutes > tempoTotalCafePronto
            };
            return View(viewModel);
        }

        public ActionResult Iniciar()
        {
            var statusCafeteira = ObterCateteiraStatus();
            try
            {
                ValidarIniciar(statusCafeteira);
                statusCafeteira = new CafeteiraStatus { Data = DateTime.Now, Status = CafeteiraStatus.eStatus.Iniciado };
                SalvarCateteiraStatus(statusCafeteira);
            }
            catch (Exception ex)
            {
                TempData["ErroMessage"] = ex.Message;
            }
            return RedirectToAction(INDEX_ACTIONNAME);
        }

        private void ValidarIniciar(CafeteiraStatus statusCafeteira)
        {
            if (statusCafeteira.Status == CafeteiraStatus.eStatus.Iniciado)
            {
                throw new Exception("Não pode iniciar se já esta iniciado");
            }
            else if (statusCafeteira.Status == CafeteiraStatus.eStatus.Pronto &&
                (DateTime.Now - statusCafeteira.Data).TotalMinutes < CafeteiraStatus.TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS)
            {
                throw new Exception(string.Format("Não pode iniciar se já esta pronto nos próximos {0} minutos", CafeteiraStatus.TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS));
            }
        }

        public ActionResult Pronto()
        {
            var statusCafeteira = ObterCateteiraStatus();

            try
            {
                ValidarPronto(statusCafeteira);

                var data = DateTime.Now;
                if (statusCafeteira.Status == CafeteiraStatus.eStatus.Iniciado)
                {
                    data = data.AddMinutes(-(DateTime.Now - statusCafeteira.Data.AddMinutes(CafeteiraStatus.TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS)).TotalMinutes);
                    if (data >= DateTime.Now)
                    {
                        data = DateTime.Now;
                    }
                }
                statusCafeteira = new CafeteiraStatus { Data = data, Status = CafeteiraStatus.eStatus.Pronto };
                SalvarCateteiraStatus(statusCafeteira);
                EnviarNotificacao(statusCafeteira);
            }
            catch (Exception ex)
            {
                TempData["ErroMessage"] = ex.Message;
            }
            return RedirectToAction(INDEX_ACTIONNAME);
        }

        private void ValidarPronto(CafeteiraStatus statusCafeteira)
        {
            var tempoTotalCafePronto = CafeteiraStatus.TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS + CafeteiraStatus.TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS;
            if (statusCafeteira.Status == CafeteiraStatus.eStatus.Pronto &&
                (DateTime.Now - statusCafeteira.Data).TotalMinutes < tempoTotalCafePronto)
            {
                throw new Exception(string.Format("Não pode ficar pronto nos próximos {0} minutos", tempoTotalCafePronto));
            }
        }

        [HttpGet]
        public ActionResult GetStatus()
        {
            return Json(ObterCateteiraStatus(), JsonRequestBehavior.AllowGet);
        }

        private CafeteiraStatus ObterCateteiraStatus()
        {
            var baseDirectory = SysIO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            if (SysIO.Directory.Exists(baseDirectory))
            {
                var filename = SysIO.Path.Combine(baseDirectory, "CafeteiraStatus.xml");
                if (SysIO.File.Exists(filename))
                {
                    using (var file = SysIO.File.Open(filename, SysIO.FileMode.Open))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(CafeteiraStatus));
                        return xmlSerializer.Deserialize(file) as CafeteiraStatus;
                    }
                }
            }

            return new CafeteiraStatus();
        }

        private void SalvarCateteiraStatus(CafeteiraStatus status)
        {
            var baseDirectory = SysIO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            if (!SysIO.Directory.Exists(baseDirectory))
            {
                SysIO.Directory.CreateDirectory(baseDirectory);
            }

            var filename = SysIO.Path.Combine(baseDirectory, "CafeteiraStatus.xml");
            using (var file = System.IO.File.Open(filename, SysIO.FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(CafeteiraStatus));
                xmlSerializer.Serialize(file, status);
            }
        }

        public void EnviarNotificacao(CafeteiraStatus statusCafeteira)
        {
            var baseDirectory = SysIO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AppData");
            var filenameSampleMail = SysIO.Path.Combine(baseDirectory, "CafeProntoSample.htm");
            var filenameEmails = SysIO.Path.Combine(baseDirectory, "EmailsCafePronto.txt");
            if (SysIO.File.Exists(filenameEmails) && SysIO.File.Exists(filenameSampleMail))
            {
                var emails = new List<string>();
                using (var emailReader = new StreamReader(filenameEmails))
                {
                    while(!emailReader.EndOfStream)
                    {
                        emails.Add(emailReader.ReadLine());
                    }
                }

                if (emails.Count > 0)
                {
                    string mensagem;
                    using (var sampleMailReader = new StreamReader(filenameSampleMail))
                    {
                        mensagem = sampleMailReader.ReadToEnd();
                    }
                    mensagem = mensagem.Replace("#Data#", statusCafeteira.Data.ToString());

                    foreach (var destinatario in emails)
                    {
                        SendMail.Send(destinatario, "Café Pronto", mensagem);
                    }
                }
            }
        }
    }
}
