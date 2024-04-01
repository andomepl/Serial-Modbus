using Modbus.Device;
using ModbusTool.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ModbusTool.Services
{
    public class FuncFac
    {

        public static async Task MasterFunc03(IModbusMaster modbusMaster,byte slaveAddress,CancellationToken token,ObservableCollection<MasterTable> masterTables)
        {
            try
            {
                await Task.Run(async () =>
                {
                    ushort[]? datas = null;
                    while (true)
                    {
                        await Task.Delay(1000);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        datas = await modbusMaster.ReadHoldingRegistersAsync(slaveAddress, (ushort)0, (ushort)10);

                        if (datas == null)
                            continue;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                //MasterTable masterTable = new MasterTable { Value = datas[i] };
                                masterTables[i].Value = datas[i];
                            }
                        });
                    }
                }, token);
            }
            catch(Exception e)
            {
                throw;
            }
        }



        public static async Task MasterFunc06(IModbusMaster modbusMaster, byte slaveAddress, CancellationToken token, ObservableCollection<MasterTable> masterTables)
        {

            try
            {
                await Task.Run(async () =>
                {

                    while (true)
                    {
                        await Task.Delay(1000);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        ushort[] us = masterTables.Select(ms => ms.Value).ToArray();

                        await modbusMaster.WriteMultipleRegistersAsync(slaveAddress, 0, us);
                    }
                });
            }
            catch(Exception e)
            {
                throw;
            }

        }
    
    
        public static async Task MasterFunc01(IModbusMaster modbusMaster, byte slaveAddress, CancellationToken token, ObservableCollection<MasterTable> masterTables)
        {
            try
            {
                await Task.Run(async () =>
                {
                    bool[]? datas = null;
                    while (true)
                    {
                        await Task.Delay(1000);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        datas = await modbusMaster.ReadCoilsAsync(slaveAddress, (ushort)0, (ushort)10);

                        if (datas == null)
                            continue;

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                masterTables[i].Value =datas[i]?(ushort)1:(ushort)0;
                            }
                        });
                    }
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public static async Task MasterFunc05(IModbusMaster modbusMaster, byte slaveAddress, CancellationToken token, ObservableCollection<MasterTable> masterTables)
        {
            try
            {
                await Task.Run(async () =>
                {
                    while (true)
                    {
                        await Task.Delay(1000);

                        if (token.IsCancellationRequested)
                            token.ThrowIfCancellationRequested();

                        bool[] us = masterTables.Select(ms =>
                        {
                            if (ms.Value == 1)
                                return true;
                            else
                            {
                                return false;
                            }
                        }).ToArray();

                        await modbusMaster.WriteMultipleCoilsAsync(slaveAddress, (ushort)0, us);


                    }
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
