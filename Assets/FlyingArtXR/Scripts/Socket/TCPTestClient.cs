using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;


public class TCPTestClient : MonoBehaviour
{
	#region private members 	
	private TcpClient socketConnection;
	private Thread clientReceiveThread;

	public string serverIp = "flyingart-server.8hlab.com";

	string serverMessage;

//	public string serverIp = "52.79.215.122";
	public Text text;

	#endregion
	// Use this for initialization 	
	void Start()
	{
		ConnectToTcpServer();
	}
	// Update is called once per frame
	void Update()
	{
		if (serverMessage != null)
        {
			text.text = serverMessage;
		}

	}
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer()
	{
		try
		{
			clientReceiveThread = new Thread(new ThreadStart(ListenForData));
			clientReceiveThread.IsBackground = true;
			clientReceiveThread.Start();
		}
		catch (Exception e)
		{
			text.text = "On client connect exception " + e;
			Debug.Log("On client connect exception " + e);
		}
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	private void ListenForData()
	{
		try
		{
			socketConnection = new TcpClient(this.serverIp, 8090);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream())
				{
					int length;
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						serverMessage = Encoding.ASCII.GetString(incommingData);
						Debug.Log("server message received as: " + serverMessage);
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
			text.text = "ListenForData exception: " + socketException;
		}
	}
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage()
	{
		if (socketConnection == null)
		{
			return;
		}
		try
		{
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream();
			if (stream.CanWrite)
			{
				string clientMessage = "This is a message from one of your clients.";
				// Convert string message to byte array.                 
				byte[] clientMessageAsByteArray = Encoding.UTF8.GetBytes(clientMessage);
				// Write byte array to socketConnection stream.                 
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
				Debug.Log("Client sent his message - should be received by server");
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("Socket exception: " + socketException);
			text.text = "SendMessage exception: " + socketException;

		}
	}

	private void ShowToastMessage(string message)
	{
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity.3d.player.UnityPlayer");
		AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		if (unityActivity != null)
		{
			AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
			unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
			{
				AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity, message, 0);
				toastObject.Call("show");
			}));
		}
	}
}