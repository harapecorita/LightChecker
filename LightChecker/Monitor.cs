using System;
using System.Threading.Tasks;

namespace LightChecker
{
    class Monitor
    {
        public delegate Task NowOnEventHandler();
        public delegate Task NowOffEventHandler(TimeSpan overSpan);
        public delegate Task StillOnEventHandler(TimeSpan span);

        public event NowOnEventHandler NowOn;
        public event NowOffEventHandler NowOff;
        public event StillOnEventHandler StillOn;

        public int LimitSeconds { get; }
        public Light Light { get; }
        public Monitor(int limitSeconds, Light light)
        {
            LimitSeconds = limitSeconds;
            Light = light;
        }
        public async Task Start()
        {
            // 最新の点灯開始日時
            var latestOn = DateTime.Now;
            // 通知済みフラグ
            var hasNotifyed = false;

            while (true)
            {
                // 超過分のTimeSpanを作成
                var overSpan = DateTime.Now - latestOn.AddSeconds(LimitSeconds);

                Light.UpdateStatus();

                switch (Light.RunningStatus)
                {
                    case RunningStatus.NowOn:
                        // 点いた日時をセット
                        latestOn = DateTime.Now;
                        // 通知
                        if(NowOn != null) await NowOn();
                        break;
                    case RunningStatus.NowOff:
                        // 上限点灯時間を超えている場合
                        if (0 < overSpan.TotalSeconds)
                        {
                            // 通知
                            if (NowOff != null) await NowOff(overSpan);
                            // 通知済みをリセット
                            hasNotifyed = false;
                        }
                        break;
                    case RunningStatus.StillOn:
                        // 上限点灯時間を超えているかつ、通知済みで無い場合
                        if (0 < overSpan.TotalSeconds && !hasNotifyed)
                        {
                            // 通知
                            if (StillOn != null) await StillOn(DateTime.Now - latestOn);
                            // 通知済みをセット
                            hasNotifyed = true;
                        }
                        break;
                }
            }
        }
    }
}
