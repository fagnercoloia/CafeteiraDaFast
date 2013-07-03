using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeiteiraFast.ViewModels
{
    public class IndexViewModel
    {
        public string MensagemErro { get; set; }
        public string Mensagem { get; set; }
        public bool BotaoIniciarVisivel { get; set; }
        public bool BotaoProntoVisivel { get; set; }
    }
}