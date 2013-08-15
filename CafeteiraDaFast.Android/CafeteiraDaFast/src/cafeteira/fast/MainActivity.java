package cafeteira.fast;

import java.util.Timer;

import android.annotation.TargetApi;
import android.app.Activity;
import android.app.AlarmManager;
import android.app.PendingIntent;
import android.content.Context;
import android.content.Intent;
import android.os.Build;
import android.os.Bundle;
import android.os.Handler;
import android.os.Handler.Callback;
import android.os.Message;
import android.os.SystemClock;
import android.widget.TextView;

@TargetApi(Build.VERSION_CODES.CUPCAKE)
public class MainActivity extends Activity {
	Timer timer;
	
	TextView textView;
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.main);
		
		textView = (TextView)findViewById(R.id.MessageLabel);
		
		timer = new Timer();
		timer.schedule(new VerificaStatusCafeteira(handler), 0, 60000);

		if(!CafeteiraService.isInstanceCreated())
		{
			PendingIntent serviceIntent = PendingIntent.getService(this,
	                0, new Intent(this, CafeteiraService.class), 0);
			long firstTime = SystemClock.elapsedRealtime();
			
			AlarmManager am = (AlarmManager)this.getSystemService(Context.ALARM_SERVICE);
			long intervalInSec = 4;
			am.setRepeating(AlarmManager.ELAPSED_REALTIME, firstTime, intervalInSec*1000, serviceIntent);
		}
	}
	
	final Handler handler = new Handler(new Callback() {
		public boolean handleMessage(Message arg0) {
			CafeteiraStatus status = (CafeteiraStatus)arg0.obj;
			textView.setText(status.Mensagem);
			return false;
		};
    });
}
