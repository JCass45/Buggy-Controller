using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace ConsoleApplication16
{
    class PortDataReceived
    {
        public static void Main()
        {
            SerialPort mySerialPort = new SerialPort("COM12");

            mySerialPort.BaudRate = 9600; //Not required to know the following
            mySerialPort.Parity = Parity.None;//
            mySerialPort.StopBits = StopBits.One;//
            mySerialPort.DataBits = 8;//
            mySerialPort.Handshake = Handshake.None;//
            mySerialPort.RtsEnable = true;// to here

            //Setting PAN ID C#
            mySerialPort.Open();
            mySerialPort.Write("+++");
            System.Threading.Thread.Sleep(1100); // Guard Time
            mySerialPort.WriteLine("ATID 3305, CH C, CN");
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); //The event handler. A new thread of code is started up which waits for data to come into the serial port

            string msgOut = "";

            while (msgOut != "exit")
            {
                msgOut = Console.ReadLine();

                if (msgOut != "exit")
                {
                    mySerialPort.WriteLine(msgOut);
                    Console.WriteLine(">" + msgOut);
                }
            }

            mySerialPort.Close(); // closes serial port on computer

            //the Main thread has finished, but the even handler thread is constantly ongoing
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) // event handler object means the input can be of any type (serialport, string, int etc)
        {
            SerialPort sp = (SerialPort)sender;
            string msgIn = sp.ReadExisting();
            Console.WriteLine("<" + msgIn);

            parseObstacle(msgIn, sp);
            parseGantry(msgIn, sp);
        }

        private static void parseGantry(string msgIn, SerialPort sp)
        {
            //https://msdn.microsoft.com/en-us/library/k8b1470s.aspx
            //See above link for explanation of IndexOf(string)
            const char buggyID = '1';
            int index1 = msgIn.IndexOf("g1b" + buggyID);
            int index2 = msgIn.IndexOf("g2b" + buggyID);
            int index3 = msgIn.IndexOf("g3b" + buggyID);

            if (index1 >= 0 || index2 >= 0 || index3 >= 0)
            {
                Thread.Sleep(2000);
                sp.WriteLine("g" + buggyID);
                Console.WriteLine(">" + "g" + buggyID);
            }
        }

        private static void parseObstacle(string msgIn, SerialPort sp)
        {
            const char buggyID = '1';
            int index1 = msgIn.IndexOf("o0b" + buggyID);
            int index2 = msgIn.IndexOf("o1b" + buggyID);

            if (index1 >= 0)
            {
                sp.WriteLine("g" + buggyID);
                Console.WriteLine(">" + "g" + buggyID);
            }

            else if (index2 >= 0)
            {
                sp.WriteLine("s" + buggyID);
                Console.WriteLine(">" + "s" + buggyID);
            }
        }

    }
}