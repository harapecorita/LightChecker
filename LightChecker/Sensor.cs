using System.Device.I2c;

namespace LightChecker
{
    class Sensor
    {
        public enum ResolutionMode
        {
            // https://www.mouser.com/datasheet/2/348/bh1750fvi-e-186247.pdf
            /// <summary>
            /// 分解能1lx
            /// </summary>
            HResolutionMode = 0x10,
            /// <summary>
            /// 分解能0.5lx
            /// </summary>
            HResolutionMode2 = 0x11,
            /// <summary>
            /// 分解能4lx
            /// </summary>
            LResolutionMode = 0x13

        }
        private I2cDevice Device { get; set; }
        private byte Threshold { get; }
        public Sensor(int busId, int deviceAddress, ResolutionMode mode, byte threshold)
        {
            var settings = new I2cConnectionSettings(busId, deviceAddress);
            Device = I2cDevice.Create(settings);
            Threshold = threshold;

            Device.Write(new byte[] { (byte)mode });
        }
        public byte GetValue()
        {
            return Device.ReadByte();
        }
        public Status GetStatus()
        {
            // 閾値以上ならば点灯している
            return GetValue() >= Threshold ? Status.On : Status.Off;
        }
    }
}
