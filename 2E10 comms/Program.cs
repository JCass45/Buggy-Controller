
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;


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
            
            //Setting PAN ID
            mySerialPort.Open();
            mySerialPort.Write("+++");
            System.Threading.Thread.Sleep(1100); // Guard Time
            mySerialPort.WriteLine("ATID 3305, CH C, CN");

            Console.ReadLine();
            mySerialPort.WriteLine("g1"); //start buggy
            Console.WriteLine("Buggy has started");

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler); //The event handler. A new thread of code is started up which waits for data to come into the serial port

            Console.WriteLine();
            Console.ReadKey();
            mySerialPort.Close(); // closes serial port on computer

            //the Main thread has finished, but the even handler thread is constantly ongoing
        }

        private static void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) // event handler object means the input can be of any type (serialport, mouse, keyboard, string, int etc)
        {
            SerialPort sp = (SerialPort)sender; //sender is converted to serialport type
            string indata = sp.ReadExisting();  //indata string takes in whatever the data received was
                                                //Console.WriteLine("Data Received:");
                                                // next two lines: writes string to the console and then sends it back out to the display chat thing
            if (indata.Length == 5)
            {
                switch (indata[0])
                {
                    case 'g':
                        //if(indata.Length == 5)
                        parseMsg(indata, sp);
                        break;

                    case 'o':
                        //if (indata.Length == 5)
                        parseMsg(indata, sp);
                        break;

                    case 'a':
                        //if(indata.Length == 5)
                        if (indata.Length == 5)
                            parseMsg(indata, sp);
                        break;

                    default:
                        Console.WriteLine("UNKNOWN COMMAND");
                        break;
                }
            }
    
        }

        //parseCommand takes in a string of 5 characters from the buffer, decodes it and sends a response
        //g = gantry detected
        //o = obstacle detected
        //a = message from Arduino (may be redundant)
        //what other reasons might the arduino communicate to the controller???

        private static void parseMsg(string msg, SerialPort sp)
        {
            Console.WriteLine("parseCommand entered. Command: " + msg);

            char typeCommand = msg[0];
            char typeCommandId = msg[1];        //Jack(11/2) - replaced gantryNum with typeCommandId
            char buggyNum = msg[3];             //in case of typeCommand == 'g' typeCommandId tells us gantry number
                                                //in case of typeCommand == 'o' typeCommandId will be 1 if obstacle is in the way and 0 if obstacle has cleared

            if (typeCommand == 'g')             // if buggy has entered a gantry
            {
                if(buggyNum == '1')
                {
                    sp.WriteLine("s1");        //stop buggy 1
                    System.Threading.Thread.Sleep(3000);
                    sp.WriteLine("g1");
                }

                else if(buggyNum == '2')
                {
                    sp.WriteLine("s2");        //stop buggy 2
                    System.Threading.Thread.Sleep(3000);
                    sp.WriteLine("g2");
                }

                else
                {
                    Console.WriteLine("UNKNOWN BUGGY");
                }

                Console.WriteLine("Buggy:" + buggyNum + " told to stop");
            }

            else if (typeCommand == 'o')             //if buggy has seen an obstacle
            {
                if (typeCommandId == 1)
                {
                    if (buggyNum == 1)
                    {
                        sp.WriteLine("s1");
                    }

                    else if (buggyNum == '2')
                    {
                        sp.WriteLine("s2");
                    }
                }

                else if (typeCommandId == 0)
                {
                    if (buggyNum == 1)
                    {
                        sp.WriteLine("g1");         //go buggy 1
                    }

                    else if (buggyNum == '2')
                    {
                        sp.WriteLine("g2");         //go buggy 2
                    }
                }
            }

        }//end of parseMsg
    }//end of class
}//end of namespace


