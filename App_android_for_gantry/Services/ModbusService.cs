using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NModbus;
using NModbus.Device;

namespace App_android_for_gantry.Services
{
    class ModbusService
    {
        private TcpClient? _tcpClient;// 
        private IModbusMaster? _modbusMaster;// 
        private const string plcIpAddress = "192.168.1.3"; // IP của PLC
        private const int port = 502; // Cổng Modbus TCP
        public bool IsConnected { get; private set; } = false;// Bien kiem tra ket (Neu thanh cong thi T, con that bai thi F)
        private bool _isReading = false;
        private static double _fallbackValue = 0; // Biến lưu giá trị khi lỗi
        /// <summary>
        /// Kết nối PLC thông qua Modbus TCP
        /// </summary>
        public async Task<bool> ConnectPLCAsync(int timeoutMs = 2000)
        {
            try
            {
                _tcpClient = new TcpClient();

                // Sử dụng `await` để kết nối TCP bất đồng bộ
                var connectTask = _tcpClient.ConnectAsync(plcIpAddress, port);

                if (await Task.WhenAny(connectTask, Task.Delay(timeoutMs)) == connectTask)
                {
                    var factory = new ModbusFactory();
                    _modbusMaster = factory.CreateMaster(_tcpClient);

                    IsConnected = true;

                    return true; // Kết nối thành công
                }
                else
                {
                    throw new TimeoutException("Kết nối TCP/IP đến PLC bị timeout!");
                }

            }
            catch (Exception)
            {
                IsConnected = false;

                return false; // Kết nối thất bại
            }
        }

        /// <summary>
        /// Đọc trạng thái của Coil (bit ON/OFF)
        /// </summary>
        public async Task<bool[]> ReadCoilsAsync(byte slaveId, ushort startAddress, ushort count)
        {
            if (_modbusMaster == null || _tcpClient == null || !_tcpClient.Connected)
                throw new InvalidOperationException("Chưa kết nối PLC");

            return await Task.Run(() => _modbusMaster.ReadCoils(slaveId, startAddress, count));
        }

        /// <summary>
        /// Đọc giá trị từ PLC và cập nhật BoxView liên tục
        /// </summary>
        public async Task StartReadingCoilAsync(BoxView boxView, byte slaveId, ushort startAddress)
        {
            _isReading = true;

            while (IsConnected)
            {
                try
                {
                    if (IsConnected)
                    {
                        bool[] coils = await ReadCoilsAsync(slaveId, startAddress, 1);

                        // Cập nhật UI trên MainThread
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            boxView.Color = coils[0] ? Colors.Green : Colors.Red;
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi khi đọc coil: {ex.Message}");
                }

                await Task.Delay(500); // Đọc dữ liệu mỗi 500ms
            }
        }


        /// <summary>
        /// Ghi trạng thái Coil (Bật/Tắt)
        /// </summary>
        public async Task WriteCoilAsync(byte slaveId, ushort address, bool value)
        {
            if (_modbusMaster == null || _tcpClient == null || !_tcpClient.Connected)
                throw new InvalidOperationException("Chưa kết nối PLC");

            await Task.Run(() => _modbusMaster.WriteSingleCoil(slaveId, address, value));
        }




        /// <summary>
        /// Đọc giá trị của Holding Register
        /// </summary>
        public async Task<ushort[]> ReadHoldingRegistersAsync(byte slaveId, ushort startAddress, ushort count)
        {
            if (_modbusMaster == null || _tcpClient == null || !_tcpClient.Connected)
                throw new InvalidOperationException("Chưa kết nối PLC");

            return await Task.Run(() => _modbusMaster.ReadHoldingRegisters(slaveId, startAddress, count));
        }


        /// <summary>
        /// Ghi giá trị vào Holding Register
        /// </summary>
        public async Task WriteHoldingRegisterAsync(byte slaveId, ushort address, ushort value)
        {
            if (_modbusMaster == null || _tcpClient == null || !_tcpClient.Connected)
                throw new InvalidOperationException("Chưa kết nối PLC");

            await Task.Run(() => _modbusMaster.WriteSingleRegister(slaveId, address, value));
        }

        public async Task<double> ReadLREALAsync(ushort startAddress)
        {
            try
            {
                // Đọc 4 thanh ghi từ PLC (LREAL = 64-bit = 8 byte)
                ushort[] rawData = await _modbusMaster.ReadHoldingRegistersAsync(slaveAddress: 1, startAddress, numberOfPoints: 4);

                // Chuyển đổi thanh ghi thành số thực 64-bit
                return ConvertRegistersToDouble(rawData);
            }
            catch (Exception ex)
            {
                _fallbackValue += 1.0; // Tăng dần giá trị khi lỗi
                return _fallbackValue;
            }
        }

        private static double ConvertRegistersToDouble(ushort[] registers)
        {
            if (registers.Length < 4)
                throw new ArgumentException("Cần ít nhất 4 thanh ghi để chuyển đổi thành LREAL");

            // Chuyển 4 thanh ghi (16-bit) thành 8 byte (64-bit)
            byte[] bytes = new byte[8];

            // Modbus trả về theo Big-Endian -> Cần đảo lại
            bytes[0] = (byte)(registers[0]);
            bytes[1] = (byte)(registers[0] >> 8);
            bytes[2] = (byte)(registers[1]);
            bytes[3] = (byte)(registers[1] >> 8);
            bytes[4] = (byte)(registers[2]);
            bytes[5] = (byte)(registers[2] >> 8);
            bytes[6] = (byte)(registers[3]);
            bytes[7] = (byte)(registers[3] >> 8);

            // Chuyển byte[] thành double (IEEE 754)
            return BitConverter.ToDouble(bytes, 0);
        }



        // Đọc được rồi
        // Đọc đúng hay sai
        // Đọc rồi có cập nhật giao diện hay không how to use Binding vs MainPage.xamls và MainPage.xamls.cs

        /// <summary>
        /// Ngắt kết nối với PLC
        /// </summary>
        public async Task Disconnect()
        {
            await Task.Run(() =>
            {
                if (_tcpClient != null)
                {
                    _tcpClient.Close();
                    _tcpClient.Dispose();
                    _tcpClient = null;
                    _modbusMaster = null;
                    IsConnected = false;
                    _isReading = false;
                }
            });
        }



    }
}
