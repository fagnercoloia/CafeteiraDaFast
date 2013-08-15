package cafeteira.fast;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.Date;
import java.util.TimerTask;

import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.StatusLine;
import org.apache.http.client.HttpClient;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.impl.client.DefaultHttpClient;
import org.json.JSONException;
import org.json.JSONObject;

import android.os.Handler;
import android.os.Message;
import android.util.Log;

public class VerificaStatusCafeteira extends TimerTask {
	Handler handler;
	
	public VerificaStatusCafeteira(Handler handler) {
		this.handler = handler;
	}
	
	@Override
	public void run() {
		CafeteiraStatus status = getCafeteiraStatus();
 	    if(status != null) {
 	    	Message msg = new Message();
 	    	msg.obj = status;
 	    	handler.sendMessage(msg);
 	    }
	}
	
	public String readJSONFeed(String URL) {
        StringBuilder stringBuilder = new StringBuilder();
        HttpClient httpClient = new DefaultHttpClient();
        HttpGet httpGet = new HttpGet(URL);
        try {
            HttpResponse response = httpClient.execute(httpGet);
            StatusLine statusLine = response.getStatusLine();
            int statusCode = statusLine.getStatusCode();
            if (statusCode == 200) {
                HttpEntity entity = response.getEntity();
                InputStream inputStream = entity.getContent();
                BufferedReader reader = new BufferedReader(
                        new InputStreamReader(inputStream));
                String line;
                while ((line = reader.readLine()) != null) {
                    stringBuilder.append(line);
                }
                inputStream.close();
            } else {
                Log.d("JSON", "Failed to download file");
            }
        } catch (Exception e) {
            Log.d("readJSONFeed", e.getLocalizedMessage());
        }        
        return stringBuilder.toString();
    }
	
	public static Date JsonDateToDate(String jsonDate)
	{
	    //  "/Date(1321867151710)/"
	    int idx1 = jsonDate.indexOf("(");
	    int idx2 = jsonDate.indexOf(")");
	    String s = jsonDate.substring(idx1+1, idx2);
	    long l = Long.valueOf(s);
	    return new Date(l);
	}
	
	private CafeteiraStatus getCafeteiraStatus() {
		
		String jsonString = readJSONFeed("http://moonha.fastsolucoes.com.br/CafeteiraDaFast/Home/GetStatus/");
        try {
            JSONObject cafeteiraStatusJson = new JSONObject(jsonString); 
            CafeteiraStatus cafeteiraStatus = new CafeteiraStatus();
            
            String jsonDateString = cafeteiraStatusJson.getString("Data");
            cafeteiraStatus.Data = JsonDateToDate(jsonDateString);
            
            int jsonStatusInt = cafeteiraStatusJson.getInt("Status");
            cafeteiraStatus.Status = CafeteiraStatus.eStatus.fromInteger(jsonStatusInt);
            cafeteiraStatus.Mensagem = cafeteiraStatusJson.getString("Mensagem");
            
            return cafeteiraStatus;
        } catch (JSONException e) {
            Log.e("DEVMEDIA", "Erro no parsing do JSON", e);
        }       
        return null;
    }

}
