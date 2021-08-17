using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Sunley.Network.LAN
{
    public static class Message
    {
        // Private //
        private const int bufferSize = 1024;

        private static void SendBytes(byte[] bytes, IPEndPoint endPoint)
        {
            int numPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(bytes.Length) / Convert.ToDouble(bufferSize)));
            byte[][] packetArray = CreatePackets(bytes);

            TcpClient client = new TcpClient(endPoint);
            Socket s = client.Client;

            s.Send(Encoding.UTF8.GetBytes(numPackets.ToString()));

            for (int i = 0; i < numPackets; i++)
            {
                s.Send(packetArray[i]);
            }

            client.Close();
            s.Close();
        }
        private static void SendBytesWithTimeout(byte[] bytes, IPEndPoint endPoint, int timeoutMS)
        {
            Thread t = new Thread(() => SendBytes(bytes, endPoint));
            t.Start();

            Thread.Sleep(timeoutMS);
            t.Abort();
        }
        private static byte[][] CreatePackets(byte[] bytes)
        {
            int numPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(bytes.Length) / Convert.ToDouble(bufferSize)));
            var bufferArray = new byte[numPackets][];

            for (int i = 0; i < numPackets; i++)
            {
                List<byte> chunk = new List<byte>();

                for (int index = 0; index < bufferSize; index++)
                {
                    int num = (i * bufferSize) + index;

                    if (num < bytes.Length)
                    {
                        chunk.Add(bytes[num]);
                    }
                }

                byte[] chunkArr = chunk.ToArray();
                bufferArray[i] = chunkArr;
            }

            return bufferArray;
        }

        private static byte[] ReceiveBytes(IPEndPoint endPoint)
        {
            byte[] packetCount = new byte[1024];
            int numPackets;

            TcpListener listener = new TcpListener(endPoint);
            listener.Start();
            Socket s = listener.AcceptSocket();

            s.Receive(packetCount);
            numPackets = Convert.ToInt32(Encoding.UTF8.GetString(packetCount));

            var splitBytes = new byte[numPackets][];

            for (int i = 0; i < numPackets; i++)
            {
                splitBytes[i] = new byte[1024];
                s.Receive(splitBytes[i]);
            }

            List<byte> byteList = new List<byte>();

            for (int i = 0; i < numPackets; i++)
            {
                byteList.AddRange(splitBytes[i]);
            }

            byte[] allBytes = byteList.ToArray();

            listener.Stop();
            s.Close();

            return allBytes;
        }
        private static byte[] ReceiveBytesWithTimeout(IPEndPoint endPoint, int timeoutMS)
        {
            byte[] packetCount = new byte[1024];
            int numPackets;
            int count = 0;

            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            while (count < timeoutMS && !listener.Pending()) { Thread.Sleep(100); count += 100; }

            if (listener.Pending())
            {
                Socket s = listener.AcceptSocket();

                s.Receive(packetCount);
                numPackets = Convert.ToInt32(Encoding.UTF8.GetString(packetCount));

                var splitBytes = new byte[numPackets][];

                for (int i = 0; i < numPackets; i++)
                {
                    splitBytes[i] = new byte[1024];
                    s.Receive(splitBytes[i]);
                }

                List<byte> byteList = new List<byte>();

                for (int i = 0; i < numPackets; i++)
                {
                    byteList.AddRange(splitBytes[i]);
                }

                byte[] allBytes = byteList.ToArray();

                listener.Stop();
                s.Close();

                return allBytes;
            }
            else { throw new TimeoutException(); }
        }


        // Public //

        /// <summary>
        /// Sends a message to the specified Machinexx
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="endPoint">The machine to send the message to</param>
        /// <returns>Returns 'true' if the message was successfully sent</returns>
        public static bool Send(string message, IPEndPoint endPoint)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(message);
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Sends a message to the specified Machine
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="ipAddress">The IPv4 Address of the machine to send the message to</param>
        /// <param name="port">The port number if the machine to send the message to</param>
        /// <returns></returns>
        public static bool Send(string message, string ipAddress, int port)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(message);
                IPAddress iP = IPAddress.Parse(ipAddress);
                IPEndPoint endPoint = new IPEndPoint(iP, port);
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return true; }
        }
        /// <summary>
        /// Sends a message to the specified Machine
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="endPoint">The machine to send the message to</param>
        /// <returns>Returns 'true' if the message was successfully sent</returns>
        public static bool Send(object message, IPEndPoint endPoint)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(message.ToString());
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Sends a message to the specified Machine
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="ipAddress">The IPv4 Address of the machine to send the message to</param>
        /// <param name="port">The port number if the machine to send the message to</param>
        /// <returns></returns>
        public static bool Send(object message, string ipAddress, int port)
        {
            try
            {
                byte[] b = Encoding.UTF8.GetBytes(message.ToString());
                IPAddress iP = IPAddress.Parse(ipAddress);
                IPEndPoint endPoint = new IPEndPoint(iP, port);
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return true; }
        }


        public static string Receive(IPEndPoint endPoint)
        {
            byte[] bytes = ReceiveBytes(endPoint);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
        public static string Receive(IPEndPoint endPoint, int timeout)
        {
            byte[] bytes = ReceiveBytesWithTimeout(endPoint, timeout);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
        public static string Receive(int port)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            byte[] bytes = ReceiveBytes(endPoint);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
        public static string Receive(int port, int timeout)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            byte[] bytes = ReceiveBytesWithTimeout(endPoint, timeout);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
    }

    public static class File
    {
        // Private //
        private const int bufferSize = 1024;

        private static void SendBytes(byte[] bytes, IPEndPoint endPoint)
        {
            int numPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(bytes.Length) / Convert.ToDouble(bufferSize)));
            byte[][] packetArray = CreatePackets(bytes);

            TcpClient client = new TcpClient(endPoint);
            Socket s = client.Client;

            s.Send(Encoding.UTF8.GetBytes(numPackets.ToString()));

            for (int i = 0; i < numPackets; i++)
            {
                s.Send(packetArray[i]);
            }

            client.Close();
            s.Close();
        }
        private static void SendBytesWithTimeout(byte[] bytes, IPEndPoint endPoint, int timeoutMS)
        {
            Thread t = new Thread(() => SendBytes(bytes, endPoint));
            t.Start();

            Thread.Sleep(timeoutMS);
            t.Abort();
        }
        private static byte[][] CreatePackets(byte[] bytes)
        {
            int numPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(bytes.Length) / Convert.ToDouble(bufferSize)));
            var bufferArray = new byte[numPackets][];

            for (int i = 0; i < numPackets; i++)
            {
                List<byte> chunk = new List<byte>();

                for (int index = 0; index < bufferSize; index++)
                {
                    int num = (i * bufferSize) + index;

                    if (num < bytes.Length)
                    {
                        chunk.Add(bytes[num]);
                    }
                }

                byte[] chunkArr = chunk.ToArray();
                bufferArray[i] = chunkArr;
            }

            return bufferArray;
        }

        private static byte[] ReceiveBytes(IPEndPoint endPoint)
        {
            byte[] packetCount = new byte[1024];
            int numPackets;

            TcpListener listener = new TcpListener(endPoint);
            listener.Start();
            Socket s = listener.AcceptSocket();

            s.Receive(packetCount);
            numPackets = Convert.ToInt32(Encoding.UTF8.GetString(packetCount));

            var splitBytes = new byte[numPackets][];

            for (int i = 0; i < numPackets; i++)
            {
                splitBytes[i] = new byte[1024];
                s.Receive(splitBytes[i]);
            }

            List<byte> byteList = new List<byte>();

            for (int i = 0; i < numPackets; i++)
            {
                byteList.AddRange(splitBytes[i]);
            }

            byte[] allBytes = byteList.ToArray();

            listener.Stop();
            s.Close();

            return allBytes;
        }
        private static byte[] ReceiveBytesWithTimeout(IPEndPoint endPoint, int timeoutMS)
        {
            byte[] packetCount = new byte[1024];
            int numPackets;
            int count = 0;

            TcpListener listener = new TcpListener(endPoint);
            listener.Start();

            while (count < timeoutMS && !listener.Pending()) { Thread.Sleep(100); count += 100; }

            if (listener.Pending())
            {
                Socket s = listener.AcceptSocket();

                s.Receive(packetCount);
                numPackets = Convert.ToInt32(Encoding.UTF8.GetString(packetCount));

                var splitBytes = new byte[numPackets][];

                for (int i = 0; i < numPackets; i++)
                {
                    splitBytes[i] = new byte[1024];
                    s.Receive(splitBytes[i]);
                }

                List<byte> byteList = new List<byte>();

                for (int i = 0; i < numPackets; i++)
                {
                    byteList.AddRange(splitBytes[i]);
                }

                byte[] allBytes = byteList.ToArray();

                listener.Stop();
                s.Close();

                return allBytes;
            }
            else { throw new TimeoutException(); }
        }


        // Public //
        public static bool Send(string fileLocation, IPEndPoint endPoint)
        {
            try
            {
                byte[] b = System.IO.File.ReadAllBytes(fileLocation);
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return false; }
        }
        public static bool Send(string fileLocation, string ipAddress, int port)
        {
            try
            {
                byte[] b = System.IO.File.ReadAllBytes(fileLocation);
                IPAddress iP = IPAddress.Parse(ipAddress);
                IPEndPoint endPoint = new IPEndPoint(iP, port);
                SendBytesWithTimeout(b, endPoint, 10000);
                return true;
            }
            catch { return false; }
        }

        private static bool Receive(string saveLocation, IPEndPoint endPoint)
        {
            try
            {
                byte[] bytes = ReceiveBytes(endPoint);
                System.IO.File.WriteAllBytes(saveLocation, bytes);
                return true;
            }
            catch { return false; }
        }
        private static bool Receive(string saveLocation, IPEndPoint endPoint, int timeout)
        {
            try
            {
                byte[] bytes = ReceiveBytesWithTimeout(endPoint, timeout);
                System.IO.File.WriteAllBytes(saveLocation, bytes);
                return true;
            }
            catch { return false; }
        }
        private static bool Receive(string saveLocation, int port)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                byte[] bytes = ReceiveBytes(endPoint);
                System.IO.File.WriteAllBytes(saveLocation, bytes);
                return true;
            }
            catch { return false; }
        }
        private static bool Receive(string saveLocation, int port, int timeout)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
                byte[] bytes = ReceiveBytesWithTimeout(endPoint, timeout);
                System.IO.File.WriteAllBytes(saveLocation, bytes);
                return true;
            }
            catch { return false; }
        }
    }
}
