package cafeteira.fast;

import java.util.Date;

public class CafeteiraStatus
{
	public enum eStatus
    {
        None,
        Iniciado,
        Pronto;
        
        public static eStatus fromInteger(int x) {
            switch(x) {
            case 0:
                return None;
            case 1:
                return Iniciado;
            case 2:
                return Pronto;
            }
            return null;
        }
    }

    public static int TEMPO_MEDIO_CAFE_PRONTO_EM_MINUTOS = 10;
    public static int TEMPO_MEDIO_CAFE_ESPERA_INICIAR_DEPOIS_DE_PRONTO_EM_MINUTOS = 10;
    public static int TEMPO_MEDIO_CAFE_TERMINADO_EM_MINUTOS = 120;

    public Date Data;
    public eStatus Status;
    public String Mensagem;
}
