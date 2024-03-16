using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SerialPort.ViewModels
{
    public class MainViewViewModel:ObservableObject
    {

        #region left Binding
        private bool isChangedSetting = true;

        public bool IsChangedSetting
        {
            get => isChangedSetting;
            set
            {
                SetProperty(ref isChangedSetting, value);
            }
        
        }



        private List<string> internalPorts= System.IO.Ports.SerialPort.GetPortNames().ToList();


        public ObservableCollection<string> InternalPortNames { get;private set; }


        private List<int> portRates=new List<int> {9600,14400,19200,38400,57600,115200};

        public IEnumerable<int> PortRates { get => portRates; }


        private List<int> dataBits = new List<int> { 8, 7, 6, 5 };

        public IEnumerable<int> DataBits { get => dataBits; }


        public IEnumerable<System.IO.Ports.Parity> Parities
        { 
            get=>(System.IO.Ports.Parity[])Enum.GetValues(typeof(System.IO.Ports.Parity));                    
        }

        public IEnumerable<System.IO.Ports.StopBits> StopBits
        {
            get => (System.IO.Ports.StopBits[])Enum.GetValues(typeof(System.IO.Ports.StopBits));
        }




        #region Setting Index

        private int portNameSelectItem = 0;

        public int PortNameSelectItem
        {
            get => portNameSelectItem;
            set
            {
                SetProperty(ref portNameSelectItem, value);
            }
        
        }

        private int portRatesSelectItem = 0;

        public int PortRatesSelectItem
        {
            get => portRatesSelectItem;
            set
            {
                SetProperty(ref portRatesSelectItem, value);
            }

        }

        private int dataBitsSelectItem = 0;

        public int DataBitsSelectItem
        {
            get => dataBitsSelectItem;
            set
            {
                SetProperty(ref dataBitsSelectItem, value);
            }

        }

        private int paritiesSelectItem = 0;

        public int ParitiesSelectItem
        {
            get => paritiesSelectItem;
            set
            {
                SetProperty(ref paritiesSelectItem, value);
            }

        }

        private int stopBitsSelectItem = 1;

        public int StopBitsSelectItem
        {
            get => stopBitsSelectItem;
            set
            {
                SetProperty(ref stopBitsSelectItem, value);
            }

        }

        #endregion


        private System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort();

        public ICommand OpenSerialPortCommand { get; }


        public ICommand UpdateCommand { get; }


        public ICommand CloseCommand { get; }

        #endregion left Binding



        private string sendMessage = "Test";// string.Empty;

        public string SendMessage
        {
            get => sendMessage;
            set
            {
                SetProperty(ref sendMessage, value);
            }
        }

        private string bufferMessage = string.Empty;

        public string BufferMessage
        {
            get => bufferMessage;
            set
            {
                SetProperty(ref bufferMessage, value);
            }
        }

        public ICommand SerialPortSendCommand { get; }

        public MainViewViewModel()
        {

            //StringBuilder sb = new StringBuilder();

            
            //for(int i=0;i<100;i++)
            //{
            //    sb.AppendLine("Test");
            //}

            //BufferMessage=sb.ToString();


            InternalPortNames = new ObservableCollection<string>(internalPorts);

            OpenSerialPortCommand = new RelayCommand(async () => { OpenSerialPort(); });
            UpdateCommand = new RelayCommand(Update);
            CloseCommand = new RelayCommand(Close);
            SerialPortSendCommand = new RelayCommand(SendSerialData);

            serialPort.DataReceived += GetSerialData;

        }
        private void Close()
        {
            try
            {
                serialPort.Close();
                IsChangedSetting = true;
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void Update()
        {
            InternalPortNames.Clear() ;

            
            var updatedinternalPorts = System.IO.Ports.SerialPort.GetPortNames().ToList();

            foreach (var currentinternalPort in updatedinternalPorts)
            {
                
                InternalPortNames.Add(currentinternalPort);
                
            }
            if(InternalPortNames.Count>0)
                PortNameSelectItem = 0;

        }


        private void SendSerialData()
        {


            Task.Factory.StartNew(async() =>
            {
                while(true)
                {
                    await Task.Delay(300);
                    byte[] bytes = Encoding.UTF8.GetBytes(SendMessage);
                    if(serialPort.IsOpen)
                        serialPort.Write(bytes, 0, bytes.Length);
                }
                   
            });

        }

        private void GetSerialData(object o, SerialDataReceivedEventArgs e)
        {

            int len=serialPort.BytesToRead;

            byte[] datas = new byte[len];

            serialPort.Read(datas, 0, len);

            string text=Encoding.UTF8.GetString(datas);

            string timenow = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss: ");


            StringBuilder stringBuilder = new StringBuilder(BufferMessage+"\r\n");

            stringBuilder.Append(timenow);
            stringBuilder.Append(text);

            BufferMessage= stringBuilder.ToString();

            
        }

        private void OpenSerialPort()
        {

            var stopbitsList = (System.IO.Ports.StopBits[])Enum.GetValues(typeof(System.IO.Ports.StopBits));
            var paritiesList = (System.IO.Ports.Parity[])Enum.GetValues(typeof(System.IO.Ports.Parity));



            string currentPortName = InternalPortNames[portNameSelectItem];
            int currentPortRates = portRates[portRatesSelectItem];
            int currentDataBits = dataBits[dataBitsSelectItem];
            System.IO.Ports.StopBits currentStopBits = stopbitsList[stopBitsSelectItem];
            System.IO.Ports.Parity currentParity = paritiesList[paritiesSelectItem];


            serialPort.PortName=currentPortName;
            serialPort.BaudRate = currentPortRates;
            serialPort.DataBits = currentDataBits;
            serialPort.StopBits = currentStopBits;
            serialPort.Parity = currentParity;
            try
            {
                serialPort.Open();
                IsChangedSetting = false;          
            }

            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
