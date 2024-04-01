using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modbus.Device;
using System.Windows.Controls.Ribbon;
using System.IO.Ports;
using System.Windows.Input;
using ModbusTool.Services;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using ModbusTool.control;
using System.Net;
using System.Windows.Threading;

namespace ModbusTool.ViewModels
{
    public partial class MainWindowViewModel:ObservableObject
    {
        #region Master

        public IEnumerable<string> ConnectionSetting
        {
            get;
        } = ["Modbus Ascii","Modbus RTU","Modbus TCP","Modbus UDP"];

        [ObservableProperty]
        private int masterConnectionIndex = 0;

        private enum MasterConnectionType
        {
            Ascii=0,
            RTU=1,
            TCP=2,
            UDP=3
        }


        public IPAddressTextBox iPAddressTextBox;

        public IEnumerable<string> SerialCOMPorts
        {
            get;
        }

        [ObservableProperty]
        private int masterSerialPortConnectionIndex = 0;

        [ObservableProperty]
        private string tCPPort = "";


        public IEnumerable<string> MasterFunc
        {
            get;
        } = ["01","03","05","06","15","16"];

        [ObservableProperty]
        private int funcSelectIndex = 0;


        [ObservableProperty]
        private string slaveAddress = "";



        private ObservableCollection<MasterTable> masterTables;
        public ObservableCollection<MasterTable> MasterTables
        {
            get=>masterTables;
            set
            {
                SetProperty(ref masterTables, value);
            }
        }


        public ICommand ConnectionCommand { get; }


        private ModbusMaster? modbusMaster;

        private CancellationTokenSource? tokenSource;

        private async void Connection(bool isConnection)
        {
            if (isConnection)
            {
                switch (MasterConnectionIndex)
                {
                    case (int)MasterConnectionType.RTU:
                        {
                            try
                            {
                                modbusMaster = ModbusFacService.GetModbusSerialRTUMaster(SerialCOMPorts.ToArray()[MasterSerialPortConnectionIndex]);
                                tokenSource = new CancellationTokenSource();
                                CancellationToken ct = tokenSource.Token;

                                var fun=typeof(FuncFac).GetMethod("MasterFunc" + (MasterFunc.ToArray()[FuncSelectIndex]));

                                Task? t=(Task?)fun?.Invoke(null, new object[] { modbusMaster, byte.Parse(SlaveAddress), ct, MasterTables });

                                await t;
                            }
                            catch(Exception e)
                            {
                                MessageBox.Show(e.Message);
                                MessageBox.Show("Disconnection!");
                                return;
                            }
                            return;
                        }
                    case (int)MasterConnectionType.TCP:
                        {
                            try
                            {
                                IPAddress iPAddress = new IPAddress(iPAddressTextBox.GetByteArray());
                                modbusMaster = ModbusFacService.GetModbusTcpMaster(iPAddress, int.Parse(TCPPort));
                                tokenSource = new CancellationTokenSource();
                                CancellationToken ct = tokenSource.Token;

                                var fun = typeof(FuncFac).GetMethod("MasterFunc" + (MasterFunc.ToArray()[FuncSelectIndex]));

                                Task? t = (Task?)fun?.Invoke(null, new object[] { modbusMaster, byte.Parse(SlaveAddress), ct, MasterTables });

                                await t;

                            }
                            catch(Exception e)
                            {
                                MessageBox.Show(e.Message);
                                MessageBox.Show("Disconnection!");
                                return;
                            }

                            return;
                        }
                    default:
                        return;
                }
            }

            if (!isConnection)
            {
                if(tokenSource != null)
                {
                    tokenSource.Cancel();
                    tokenSource.Dispose();
                }
                if (modbusMaster != null)
                { 
                    modbusMaster.Dispose();
                    modbusMaster = null;
                }
            }

        }

        #endregion
        public MainWindowViewModel()
        {

            SerialCOMPorts = new ObservableCollection<string>(SerialPort.GetPortNames());

            ConnectionCommand = new RelayCommand<bool>(Connection);

            MasterTables = new ObservableCollection<MasterTable>
            {
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
                new MasterTable{},
            };
        }


    }




    public partial class MasterTable:ObservableObject
    {

        [ObservableProperty]
        public string alias="";

        [ObservableProperty]
        public ushort value=0;


    }

}
