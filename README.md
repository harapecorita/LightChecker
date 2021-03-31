# LightChecker

光センサーGY-30を使用して、照明の監視を行うアプリケーションです。


使用する照明に応じて、分解能を変更できます。

https://www.mouser.com/datasheet/2/348/bh1750fvi-e-186247.pdf

ここでは0.5lxの'HResolutionMode2'を使用しています。

閾値は1-255の間で設定することができます。

```csharp:Program.cs
// 光センサー
var sensor = new Sensor(1, 0x23, Sensor.ResolutionMode.HResolutionMode2, 1);
// 電気
var light = new Light(sensor);
// 監視インスタンス
var monitor = new Monitor(limitSeconds, light);
```

'Sensor.GetValue()'を使用して現在の明るさを取得できます。閾値の設定に活用できます。

```csharp:Program.cs
// 光センサー
var sensor = new Sensor(1, 0x23, Sensor.ResolutionMode.HResolutionMode2, 1);
while (true)
{
    Console.WriteLine(sensor.GetValue());
}
```


照明のステータスが変更された際に呼び出されるイベントはいくつでも登録することができます。

ここではLINEの通知イベントを登録しています。

```csharp:Program.cs
// LINE通知
var notify = new Line.Notify("BOTのアクセストークン", "送信先のUserIdまたはGroupID", 7.3, 28.7);
// 監視イベントハンドラにLINE通知のメソッドを登録
monitor.NowOff += notify.NowOff;
monitor.StillOn += notify.StillOn;
```
監視はイベントハンドラの追加前、追加後どちらでも開始することができます。

```csharp:Program.cs
// 監視開始
Task.WaitAll(monitor.Start());
```
