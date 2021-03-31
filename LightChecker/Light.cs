namespace LightChecker
{
    enum Status
    {
        On,
        Off,
        Unknown
    }
    enum RunningStatus
    {
        NowOn,
        StillOn,
        NowOff,
        StillOff,
        Unknown
    }
    class Light
    {
        public Sensor Sensor { get; }
        public Status Status { get; private set; } = Status.Unknown;
        public RunningStatus RunningStatus { get; private set; } = RunningStatus.Unknown;
        public Light(Sensor sensor)
        {
            Sensor = sensor;
        }
        public void UpdateStatus()
        {
            var status = Sensor.GetStatus();

            // 今回オンで、前回オフだった場合（ついた時）
            if (status == Status.On && Status == Status.Off)
            {
                RunningStatus = RunningStatus.NowOn;
            }
            // 今回オンで、前回もオンだった場合（点きっぱなし）
            else if (status == Status.On && Status == Status.On)
            {
                RunningStatus = RunningStatus.StillOn;
            }
            // 今回オフで、前回オンだった場合（消えた時）
            else if (status == Status.Off && Status == Status.On)
            {
                RunningStatus = RunningStatus.NowOff;
            }
            // 今回オフで、前回もオフだった場合（消えっぱなし）
            else if (status == Status.Off && Status == Status.Off)
            {
                RunningStatus = RunningStatus.StillOff;
            }
            else
            {
                RunningStatus = RunningStatus.Unknown;
            }
            Status = status;
        }
    }
}
