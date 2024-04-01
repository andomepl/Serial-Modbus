using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using Modbus.Device;
using System.Net.Sockets;
using System.Net;
namespace ModbusTool.Services
{
    public class ModbusFacService
    {


        public static ModbusSerialMaster GetModbusSerialRTUMaster(string portName)
        {
            SerialPort serialPort = new SerialPort();

            serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.Even;
            serialPort.StopBits = StopBits.One;

            serialPort.Open();

            ModbusSerialMaster modbusMaster = ModbusSerialMaster.CreateRtu(serialPort);

            modbusMaster.Transport.Retries = 3;
            modbusMaster.Transport.WriteTimeout = 100;
            modbusMaster.Transport.ReadTimeout = 100;

            return modbusMaster;
        }


        public static ModbusIpMaster GetModbusTcpMaster(IPAddress ipAddress,int port)
        {

            TcpClient tcpClient = new TcpClient();

            tcpClient.ReceiveTimeout = 3000;

            IAsyncResult ar = tcpClient.BeginConnect(ipAddress, port, null, null);

            bool success = ar.AsyncWaitHandle.WaitOne(3000);


            if (success)
            {
                tcpClient.EndConnect(ar);
                Console.WriteLine("TCP Connection success");
            }
            else
            {
                tcpClient.Close();
                throw new SocketException((int)SocketError.TimedOut);
            }

            var modbusIpMaster = ModbusIpMaster.CreateIp(tcpClient);

            modbusIpMaster.Transport.Retries = 0;
            modbusIpMaster.Transport.ReadTimeout = 1500;


            return modbusIpMaster;
            
        }


    }
}
