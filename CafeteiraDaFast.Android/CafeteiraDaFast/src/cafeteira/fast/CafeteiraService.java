package cafeteira.fast;


import java.util.Timer;

import cafeteira.fast.CafeteiraStatus.eStatus;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Notification;
import android.app.NotificationManager;
import android.app.PendingIntent;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.media.MediaPlayer;
import android.os.Build;
import android.os.Handler;
import android.os.IBinder;
import android.os.Message;
import android.os.Handler.Callback;
import android.support.v4.app.NotificationCompat;
import android.widget.Toast;

@TargetApi(Build.VERSION_CODES.CUPCAKE)
public class CafeteiraService extends Service {
	private static CafeteiraService instance = null;
	String mensagem = "Café Pronto!";
	Timer timer;
	Notification notification;
	MediaPlayer player;
	public static boolean isInstanceCreated() { 
	      return instance != null; 
	}
	
	@Override 
	public void onCreate()
	{
		instance = this;
		
		player = MediaPlayer.create(this, R.raw.coffeeready);
		player.setLooping(false);
		
		notification = new Notification(R.drawable.icon, mensagem,  System.currentTimeMillis()); 
	}
	
	@Override 
	public void onDestroy() 
	{
		instance = null;
		player.stop();
	}
	
	@Override
	public IBinder onBind(Intent arg0) {
		// TODO Auto-generated method stub
		return null;
	}
	
	@Override
	public void onStart(Intent intent, int startId) {
		timer = new Timer();
		timer.schedule(new VerificaStatusCafeteira(handler), 0, 30000);
	}
	
	CafeteiraStatus ultimoStatus;
	final Handler handler = new Handler(new Callback() {
		public boolean handleMessage(Message arg0) {
			
			CafeteiraStatus status = (CafeteiraStatus)arg0.obj;
			if(ultimoStatus != null && status.Status == eStatus.Pronto &&
					(status.Data.getTime() - ultimoStatus.Data.getTime()) > 30000 )
			{
				Context context = CafeteiraService.instance.getApplicationContext();
				NotificationManager notificationManager = 
					(NotificationManager) context.getSystemService(Activity.NOTIFICATION_SERVICE);
				
				PendingIntent nullIntent = PendingIntent.getActivity(context, 0, null, 0);
				notification.setLatestEventInfo(context, getText(R.string.app_name), mensagem, nullIntent);
				// nm.cancel(550);
				notificationManager.notify(1, notification);
				/* Toast.makeText(CafeteiraService.instance, 
						"Cafeteira da FAST: Café Pronto", Toast.LENGTH_LONG).show(); */
				player.start();
			}
			ultimoStatus = status;
			return false;
		};
    });
}
