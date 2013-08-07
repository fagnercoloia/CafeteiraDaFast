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

        [System.Xml.Serialization.XmlIgnore]
        public string Mensagem
        {
            get
            {
                var elapsedTime = Status != eStatus.None ? DateTime.Now - Data : TimeSpan.Zero;

                var dias = elapsedTime.Days;
                var horas = elapsedTime.Hours;
                var minutos = elapsedTime.Minutes;

                var mensagemRetorno = string.Empty;
                switch (this.Status)
                {
                    case eStatus.Iniciado:
                        mensagemRetorno = string.Format("Cafeteira começou a fazer o café a {0} dia(s) {1} hora(s) e {2} minuto(s).", dias, horas, minutos);
                        break;
                    case eStatus.Pronto:
                        mensagemRetorno = string.Format("Cafeteira terminou de fazer o café a {0} dia(s) {1} hora(s) e {2} minuto(s).", dias, horas, minutos);
                        if (elapsedTime.TotalMinutes > TEMPO_MEDIO_CAFE_TERMINADO_EM_MINUTOS)
                            mensagemRetorno += " Provavelmente o café acabou. Venha fazer mais!!!";
                        break;
                    default:
                        break;
                }
                return mensagemRetorno;
            }
        }
    }
}