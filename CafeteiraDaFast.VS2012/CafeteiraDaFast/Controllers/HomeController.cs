using System;
using System.Web.Mvc;
using System.Xml.Serialization;
using CafeteiraDaFast.Controllers.Filters;
using CafeteiraDaFast.Models;
using SysIO = System.IO;

namespace CafeteiraDaFast.Controllers
{
    public class HomeController : Controller
    {
        const string DATA_LOCATION = "App_Data";
        const string FILE_NAME = "CafeteiraStatus.xml";

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [NoCache]
        public ActionResult GetStatus()
        {
            var statusCafeteira = ObterCateteiraStatus();
            if (statusCafeteira.Status == CafeteiraStatus.eStatus.Iniciado && (DateTime.Now - statusCafeteira.Data).TotalMinutes > CafeteiraStatus.TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS)
            {
                return Pronto();
            }
            return Json(ObterCateteiraStatus(), JsonRequestBehavior.AllowGet);
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
            return Json(statusCafeteira);
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

                MvcApplication.MobileService.GetTable<Item>().InsertAsync(new Item { Text = "Café Pronto", Data = DateTime.Now });
            }
            catch (Exception ex)
            {
                TempData["ErroMessage"] = ex.Message;
            }
            return Json(statusCafeteira);
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

        private CafeteiraStatus ObterCateteiraStatus()
        {
            var status = new CafeteiraStatus();
            var baseDirectory = SysIO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATA_LOCATION);
            if (SysIO.Directory.Exists(baseDirectory))
            {
                var filename = SysIO.Path.Combine(baseDirectory, FILE_NAME);
                if (SysIO.File.Exists(filename))
                {
                    using (var file = SysIO.File.Open(filename, SysIO.FileMode.Open))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(CafeteiraStatus));
                        status = xmlSerializer.Deserialize(file) as CafeteiraStatus;
                    }
                }
            }

            status.GerarMensagem();
            status.VerificarPermissoes();
            return status;
        }

        private void SalvarCateteiraStatus(CafeteiraStatus status)
        {
            status.GerarMensagem();
            status.VerificarPermissoes();

            var baseDirectory = SysIO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DATA_LOCATION);
            if (!SysIO.Directory.Exists(baseDirectory))
            {
                SysIO.Directory.CreateDirectory(baseDirectory);
            }

            var filename = SysIO.Path.Combine(baseDirectory, FILE_NAME);
            using (var file = System.IO.File.Open(filename, SysIO.FileMode.Create))
            {
                var xmlSerializer = new XmlSerializer(typeof(CafeteiraStatus));
                xmlSerializer.Serialize(file, status);
            }
        }
    }
}
