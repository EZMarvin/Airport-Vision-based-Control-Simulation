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
            object background = null;
            matlab.Execute("load('background.mat');");
            matlab.GetWorkspaceData("bb2", "base", out background);
            // Start transfer
            while (pipeclient.IsConnected)
            {
                int result = 0;// save distance result
                Thread.Sleep(500);
                int size = width * height;
                byte[] imageRed = new byte[size];
                byte[] imageGreen = new byte[size];
                byte[] imageBlue = new byte[size];
                pipeclient.Read(imageRed, 0, size);
                pipeclient.Read(imageGreen, 0, size);
                pipeclient.Read(imageBlue, 0, size);
                // when transfer data is null end transfer
                for (int i = 0; i < 100; i++)
                {
                    if (imageRed[i] == 0)
                    {
                        result++;
                    }
                }
                if (imageRed == null || imageRed.Length == 0 || result == 100)
                {
                    break;
                }
                // Get transfer data use Matlab to calculate distance
                object resultOfMatlab = null;
                matlab.Feval("getdistance", 1, out resultOfMatlab, imageRed, imageGreen, imageBlue, height, width, background);
                object[] res = resultOfMatlab as object[];
                int distance = Convert.ToInt32(res[0]);
                Console.WriteLine("result = " + distance);
                // Transfer back the distance to Unity server
                pipeclient.WriteByte((byte)(distance / 256));
                pipeclient.WriteByte((byte)(distance & 255));
                pipeclient.Flush();
            }
            pipeclient.Close();

        }
    }
}
