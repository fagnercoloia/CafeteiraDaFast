package cafeteira.fast;


import java.util.Date;
import java.util.Timer;

import cafeteira.fast.CafeteiraStatus.eStatus;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.IntentService;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.SharedPreferences.Editor;
import android.media.MediaPlayer;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.os.Handler.Callback;
import android.support.v4.app.NotificationCompat;
import android.widget.Toast;

@TargetApi(Build.VERSION_CODES.CUPCAKE)
public class CafeteiraService extends IntentService  {
	
	public CafeteiraService()
	{
		super("CafeteiraService");
	}
	
	public CafeteiraService(String name) {
		super(name);
	}

	private static CafeteiraService instance = null;
	
	String mensagem = "Café Pronto!";
	Notification notification;
	MediaPlayer player;
	public static boolean isInstanceCreated() { 
	      return instance != null; 
	}
	
	public CafeteiraStatus RecuperarStatusCafeteiraLocal()
	{
		SharedPreferences settings = getSharedPreferences("StatusCafeteira", 0);
		
		CafeteiraStatus status = new CafeteiraStatus();
		status.Status = CafeteiraStatus.eStatus.fromInteger(
				settings.getInt("Status", 0));
		status.Data = new Date(
				settings.getLong("Data", 0));
		return status;
	}
	
	public void GuardarStatusCafeteiraLocal(CafeteiraStatus status)
	{
		SharedPreferences settings = getSharedPreferences("StatusCafeteira", 0);
		
		Editor editor = settings.edit();
		editor.putInt("Status", status.Status.ordinal());
		editor.putLong("Data", status.Data.getTime());
		editor.commit();
	}
	
	final Handler handler = new Handler(new Callback() {
		public boolean handleMessage(Message arg0) {
			
			CafeteiraStatus ultimoStatus = RecuperarStatusCafeteiraLocal();
			
			CafeteiraStatus status = (CafeteiraStatus)arg0.obj;
			if(ultimoStatus != null && status.Status == eStatus.Pronto &&
					(status.Data.getTime() - ultimoStatus.Data.getTime()) > 30000 )
			{
				Context context = CafeteiraService.instance.getApplicationContext();
				NotificationManager notificationManager = 
					(NotificationManager) context.getSystemService(Activity.NOTIFICATION_SERVICE);
				
				PendingIntent nullIntent = PendingIntent.getActivity(context, 0, null, 0);
				notification.setLatestEventInfo(context, getText(R.string.app_name), mensagem, nullIntent);
				
				notificationManager.notify(1, notification);
				player.start();
			}
			GuardarStatusCafeteiraLocal(status);
			
			return false;
		};
    });
    
	@Override
	protected void onHandleIntent(Intent intent) {
		new VerificaStatusCafeteira(handler);
	}
}
