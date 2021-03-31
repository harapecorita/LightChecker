using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace LightChecker
{
    class Line
    {
        public class Notify
        {
            private string PushUri = "https://api.line.me/v2/bot/message/push";
            private HttpClient HttpClient { get; }
            public string To { get; }
            /// <summary>
            /// 消費電力(w)
            /// </summary>
            public double PowerConsumption { get; }
            /// <summary>
            /// 1kWhあたりの料金 (円)
            /// </summary>
            public double ElectricityRates { get; }
            public Notify(string channelAccessToken, string to, double powerConsumption, double electricityRates)
            {
                HttpClient = new HttpClient();
                To = to;
                HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

                PowerConsumption = powerConsumption;
                ElectricityRates = electricityRates;
            }
            public async Task NowOn()
            {
                var message = $"電気が点きました。";
                await SendPushMessage(message);
            }
            public async Task NowOff(TimeSpan span)
            {
                var wH = PowerConsumption * span.TotalHours;
                var kWh = wH / 1000;

                // 超過分の電気料金
                var charges = kWh * ElectricityRates;

                var message = $"電気が消えました。\r\n" +
                              $"{Math.Round(span.TotalMinutes)}分超過しました。\r\n" +
                              $"超過分の消費量は約{Math.Round(wH)}Wh、電気代は約{Math.Round(charges)}円です。";
                await SendPushMessage(message);
            }
            public async Task StillOn(TimeSpan span)
            {
                var message = $"電気が{Math.Round(span.TotalMinutes)}分間点きっぱなしです。";
                await SendPushMessage(message);
            }
            public async Task SendPushMessage(string text)
            {
                Log.Write(text);

                var message = new Model.Message("text", text);
                var messages = new List<Model.Message>
                {
                    message
                };
                var push = new Model.Push(To, messages);
                var json = JsonConvert.SerializeObject(push);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                try
                {
                    await HttpClient.PostAsync(PushUri, content);
                }
                catch
                {
                    // 10秒後にリトライ
                    await Task.Delay(10000);
                    try
                    {
                        await HttpClient.PostAsync(PushUri, content);
                    }
                    // 駄目なら抜ける
                    catch
                    {
                        return;
                    }
                }
            }
        }
        class Model
        {
            [JsonObject]
            public class Message
            {
                [JsonProperty("type")]
                public string Type { get; }
                [JsonProperty("text")]
                public string Text { get; }
                public Message(string type, string text)
                {
                    Type = type;
                    Text = text;
                }
            }
            [JsonObject]
            public class Push
            {
                [JsonProperty("to")]
                public string To { get; }
                [JsonProperty("messages")]
                public List<Message> Messages { get; }
                public Push(string to, List<Message> messages)
                {
                    To = to;
                    Messages = messages;
                }
            }
        }
    }
}
