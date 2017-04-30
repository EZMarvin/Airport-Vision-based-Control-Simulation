using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO.Pipes;
using System.Text;
using System.IO;
using System.Threading;

public class CameraSwitch : MonoBehaviour
{
	public Camera[] m_Camera; // save all the camera you use
	private Thread pipeThread; // use to create new thread
	private byte[][] imageDataCamera2; // save picture data
	private byte[][] imageDataCamera3;
	public int distance = 400; // save safe distance

	void Start()
	{
		pipeThread = new Thread (ServerThread); //create new thread
		pipeThread.Start ();
		Debug.Log ("StartThread");
		InvokeRepeating("Getdistance", 1f, 1f); //repeatly get picture data from camera
	}

	void Update()
	{

	}

	private void Getdistance()
	{
		imageDataCamera2 = CaptureCamera (m_Camera[2]); // get picture data
		imageDataCamera3 = CaptureCamera (m_Camera[3]);

		Debug.Log("wait 1s for picture");
	}

	private void ServerThread()//pipe server thread
	{
		NamedPipeServerStream pipeServer =
			new NamedPipeServerStream ("testpipe", PipeDirection.InOut, 1);

		Debug.Log ("Wait For Client");
		pipeServer.WaitForConnection ();
		Debug.Log ("Client connected ");

		int width = Screen.width;
		int height = Screen.height;
		/*
		// these code use to tansfer pictue size, here we use default size
		pipeServer.WriteByte ((byte)(width / 256));
		pipeServer.WriteByte ((byte)(width & 255));
		pipeServer.WriteByte ((byte)(height / 256));
		pipeServer.WriteByte ((byte)(height & 255));
		pipeServer.Flush ();
		*/
		while (pipeServer.IsConnected) 
		{
			byte[][] transferdata1 = imageDataCamera2;
			byte[][] transferdata2 = imageDataCamera3;
			int len = transferdata1 [0].Length;
			Debug.Log ("Picture length: "+ len);
			// transfer R G B matrix of picture to the client
			pipeServer.Write (transferdata1[0], 0, len);
			pipeServer.Flush ();

			pipeServer.Write (transferdata1[1], 0, len);
			pipeServer.Flush ();

			pipeServer.Write (transferdata1[2], 0, len);
			pipeServer.Flush ();

			pipeServer.Write (transferdata2[0], 0, len);
			pipeServer.Flush ();

			pipeServer.Write (transferdata2[1], 0, len);
			pipeServer.Flush ();

			pipeServer.Write (transferdata2[2], 0, len);
			pipeServer.Flush ();
			// receive distance data from client
			int distance1 = -1;
			int distance2 = -1;
			distance1 = pipeServer.ReadByte () * 256;
			distance1 += pipeServer.ReadByte ();
			distance2 = pipeServer.ReadByte () * 256;
			distance2 += pipeServer.ReadByte ();
			int safemin = Math.Min (distance1, distance2);
			if (safemin != -1) 
			{
				distance = safemin;
			}
			Debug.Log ("distance = " + distance);

			Thread.Sleep(500);
		}
		pipeServer.Close ();
	}

	public byte[][] CaptureCamera (Camera camera) // Get picture data, here we focus on analyzing Third camera which is camera[2]
	{
		Rect rect = new Rect (Screen.width * 0f, Screen.height * 0f, Screen.width * 1f, Screen.height * 1f);
		RenderTexture rt = new RenderTexture((int)(rect.width) , (int)(rect.height) , 24);

		camera.targetTexture = rt;
		camera.Render ();
		// capture every pixcel data
		RenderTexture.active = rt;  
		Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
		screenShot.ReadPixels(rect, 0, 0);
		screenShot.Apply(); 

		int width = (int)rect.width;
		int height = (int)rect.height;
		int size = width * height;
		byte[][] result = new byte[3][];
		result [0] = new byte[size];
		result [1] = new byte[size];
		result [2] = new byte[size];
		Debug.Log ("width * height: " + width + "*" + height );
		camera.targetTexture = null;
		RenderTexture.active = null;
		GameObject.Destroy(rt); 


		byte[] bytes = screenShot.GetRawTextureData ();
		// seperate raw data into R G B matrix data
		for (int j = 0; j < size; j++) 
		{
			result[0][j] = bytes [j * 3];
			result[1][j] = bytes [j * 3 + 1];
			result[2][j] = bytes [j * 3 + 2];
		}
		// save the backup screenshot picture (if needed)
		//byte[] picture = screenShot.EncodeToPNG();
		//string filename = @"E:\GRP\Screenshot.png";
		//System.IO.File.WriteAllBytes(filename, picture);
		//Debug.Log(string.Format("capture a picture: {0}", filename));

		return result;
	}

	public void  Background() //Capture picture from 4 cameras
	{
		Rect rect = new Rect (Screen.width * 0f, Screen.height * 0f, Screen.width * 1f, Screen.height * 1f);
		RenderTexture rt = new RenderTexture((int)(rect.width) , (int)(rect.height) , 24);
		for (int i = 0; i < 4; i++)
		{
			m_Camera [i].targetTexture = rt;
			m_Camera [i].Render ();

			RenderTexture.active = rt;  
			Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24,false);  
			screenShot.ReadPixels(rect, 0, 0);
			screenShot.Apply(); 

			m_Camera [i].targetTexture = null;
			RenderTexture.active = null;
			GameObject.Destroy(rt); 

			byte[] picture = screenShot.EncodeToPNG();

			string filename = @"E:\GRP\back"+ i +".png";
			System.IO.File.WriteAllBytes(filename, picture);
		}
	}
	void OnApplicationQuit()
	{
		pipeThread.Abort ();
		Debug.Log ("End Thread");
	} 
}
