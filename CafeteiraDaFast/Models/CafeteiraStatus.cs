using System;

namespace CafeteiraDaFast.Models
{
    public class CafeteiraStatus
    {
        public enum eStatus
        {
            None = 0,
            Iniciado,
            Pronto
        }

        public const int TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS = 10;
        public const int TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS = 10;
        public const int TEMPO_MEDIO_CAFE_TERMINADO_EM_MINUTOS = 120;

        public DateTime Data { get; set; }
        public eStatus Status { get; set; }
        public string Mensagem { get; set; }
    }
}