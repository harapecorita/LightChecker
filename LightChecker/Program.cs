using System;
using System.Threading.Tasks;

namespace LightChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            // 上限点灯時間 (s)
            var limitSeconds = int.Parse(args[0]);
            // 光センサー
            var sensor = new Sensor(1, 0x23, Sensor.ResolutionMode.HResolutionMode2, 1);
            // 電気
            var light = new Light(sensor);
            // 監視インスタンス
            var monitor = new Monitor(limitSeconds, light);
            // LINE通知
            var notify = new Line.Notify(Secret.ChannelAccessToken, Secret.GroupId, 7.3, 28.7);
            // 監視イベントハンドラにLINE通知のメソッドを登録
            monitor.NowOn += NowOn;
            monitor.NowOff += notify.NowOff;
            monitor.StillOn += notify.StillOn;
            // 監視開始
            Log.Write($"監視を開始します。(上限点灯時間: {limitSeconds}秒)");
            Task.WaitAll(monitor.Start());
        }
        public static async Task NowOn()
        {
            var message = $"電気が点きました。";
            await Task.Run(() => Log.Write(message));
        }
    }
    class Log
    {
        public static void Write(string text) => Console.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}: {text}");
    }
}
