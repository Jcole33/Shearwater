using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class FlightDataStorage : MonoBehaviour {  	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread; 	
	public bool initialPositionRecieved = false;
	private MarkerManager markerManager;
	private Dictionary<String, String> data = new Dictionary<String, String>() {
		{"time", ""},
		{"airspeed", ""},
		{"altitude", ""},
		{"longitude", ""},
		{"latitude", ""},
		{"roll", ""},
		{"pitch", ""},
		{"heading", "0"}
	};
	// Use this for initialization 	
	void Start () {
		ConnectToTcpServer(); 
		markerManager = GameObject.Find("MarkerManager").GetComponent<MarkerManager>();  
	}  	
	void Update () {
		data["time"] = System.DateTime.Now.ToString("HH:mm:ss");
	}
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}  	  
	private void ListenForData() { 		
		try { 			
			socketConnection = new TcpClient("localhost", 14550);  			
			Byte[] bytes = new Byte[1024];    
			Debug.Log("LISTENING!!!!");    
			using (NetworkStream stream = socketConnection.GetStream()) { 					
				while (true) { 				
				// Get a stream object for reading 				
					//int length; 
					MAVLink.MavlinkParse parser = new MAVLink.MavlinkParse();
					MAVLink.MAVLinkMessage message = parser.ReadPacket(stream);					
					
					/*// Read incomming stream into byte arrary. 					
											if (message.data is MAVLink.mavlink_vfr_hud_t hud) {
							data["heading"] = hud.heading.ToString();
							data["airspeed"] = hud.airspeed.ToString();
							data["altitude"] = hud.alt;
							
						}
						if (message.data is MAVLink.mavlink_high_latency_t newData) {
							data["longitude"] = newData.longitude.ToString();
						}
					} */
					
					switch (message.data) {
						case MAVLink.mavlink_vfr_hud_t newData:
							data["heading"] = newData.heading.ToString();
							data["airspeed"] = this.FormatNumber(newData.airspeed.ToString());
							break;
						case MAVLink.mavlink_global_position_int_t newData:
							var longitudeString = newData.lon.ToString(); 
							data["longitude"] = longitudeString.Substring(0, longitudeString.Length - 7) + "." + longitudeString.Substring(longitudeString.Length - 7);
							var latitudeString = newData.lat.ToString(); 
							data["latitude"] = latitudeString.Substring(0, latitudeString.Length - 7) + "." + latitudeString.Substring(latitudeString.Length - 7);
							var altitudeString = newData.alt.ToString(); 
							data["altitude"] = this.FormatNumber(altitudeString.Substring(0, altitudeString.Length - 4) + "." + altitudeString.Substring(altitudeString.Length - 4));
							//var relString = newData.relative_alt.ToString();
							//var rel = this.FormatNumber(relString.Substring(0, relString.Length - 4) + "." + relString.Substring(relString.Length - 4));
							if (!initialPositionRecieved && get("longitude") != "" && get("latitude") != "" && get("altitude") != "")
								initialPositionRecieved = true;
							break;
						case MAVLink.mavlink_attitude_t newData:
							data["roll"] = this.FormatNumber(newData.roll.ToString());
							data["pitch"] = this.FormatNumber(newData.pitch.ToString());
							data["yaw"] = this.FormatNumber(newData.yaw.ToString());
							break;
					}
					//Debug.Log(message.msgtypename);
				} 			
			}         
		}         
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}  	
	void OnDestroy() {
		clientReceiveThread.Abort();
	}
	public String get(String fieldProperty) {
		if (data.ContainsKey(fieldProperty))
			return data[fieldProperty];
		return "DATA NOT FOUND";
	}
	public void set(String fieldProperty, String newData) {
		if (data.ContainsKey(fieldProperty))
			data[fieldProperty] = newData;
		else
			data.Add(fieldProperty, newData);
	} 
	String FormatNumber(String fullNumber) {
		var index = fullNumber.IndexOf('.');
		if (index != -1) 
			return fullNumber.Substring(0, index + 3);
		return fullNumber;
	}
}