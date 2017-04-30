using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Threading;

namespace PipeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            // create pipe client to receive data from unity server
            NamedPipeClientStream pipeclient =
                new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut);

            Console.WriteLine("Connecting to server ......");
            pipeclient.Connect();
            Console.WriteLine("Server connected");

            // use COM interface to invoke Matlab function
            MLApp.MLApp matlab = new MLApp.MLApp();
            int width, height;
            /*
            width = pipeclient.ReadByte() * 256;
            width += pipeclient.ReadByte();
            height = pipeclient.ReadByte() * 256;
            height += pipeclient.ReadByte();
            */
            width = 1280;// Here we use default screen size
            height = 600;
            // Get background data from preload workspace data
            object background1 = null;
            object background2 = null;
            matlab.Execute("load('background.mat');");
            matlab.GetWorkspaceData("bb2", "base", out background1);
            matlab.GetWorkspaceData("bb3", "base", out background2);
            // Start transfer
            while (pipeclient.IsConnected)
            {
                int result = 0;// save distance result
                Thread.Sleep(500);
                int size = width * height;
                byte[] image1Red = new byte[size];
                byte[] image1Green = new byte[size];
                byte[] image1Blue = new byte[size];
                pipeclient.Read(image1Red, 0, size);
                pipeclient.Read(image1Green, 0, size);
                pipeclient.Read(image1Blue, 0, size);

                byte[] image2Red = new byte[size];
                byte[] image2Green = new byte[size];
                byte[] image2Blue = new byte[size];
                pipeclient.Read(image2Red, 0, size);
                pipeclient.Read(image2Green, 0, size);
                pipeclient.Read(image2Blue, 0, size);
                // when transfer data is null end transfer
                for (int i = 0; i < 100; i++)
                {
                    if (image1Red[i] == 0)
                    {
                        result++;
                    }
                }
                if (image1Red == null || image1Red.Length == 0 || result == 100)
                {
                    break;
                }
                // Get transfer data use Matlab to calculate distance
                object result1OfMatlab = null;
                matlab.Feval("getdistance", 1, out result1OfMatlab, image1Red, image1Green, image1Blue, height, width, background1);
                object[] res = result1OfMatlab as object[];
                int distance1 = Convert.ToInt32(res[0]);
                object result2OfMatlab = null;
                matlab.Feval("getdistance", 1, out result2OfMatlab, image2Red, image2Green, image2Blue, height, width, background2);
                res = result2OfMatlab as object[];
                int distance2 = Convert.ToInt32(res[0]);
                Console.Write("result1 = " + distance1 + " ");
                Console.WriteLine("result2 = " + distance2);
                // Transfer back the distance to Unity server
                pipeclient.WriteByte((byte)(distance1 / 256));
                pipeclient.WriteByte((byte)(distance1 & 255));
                pipeclient.Flush();
                pipeclient.WriteByte((byte)(distance2 / 256));
                pipeclient.WriteByte((byte)(distance2 & 255));
                pipeclient.Flush();
            }
            pipeclient.Close();

        }
    }
}
