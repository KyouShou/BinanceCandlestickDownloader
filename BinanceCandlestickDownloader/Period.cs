using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinanceCandlestickDownloader
{
    public struct Period
    {
        private Period(string value)
        {
            this.Value = value;
        }
        public static Period TwelveHours { get => new Period("12h"); }
        public static Period FifteenMinutes { get => new Period("15m"); }
        public static Period OneDay { get => new Period("1d"); }
        public static Period OneHour { get => new Period("1h"); }
        public static Period OneMinute { get => new Period("1m"); }
        public static Period OneMonth { get => new Period("1mo"); }
        public static Period OneWeek { get => new Period("1w"); }
        public static Period TwoHours { get => new Period("2h"); }
        public static Period ThirtyMinutes { get => new Period("30m"); }
        public static Period FourHours { get => new Period("4h"); }
        public static Period FiveMinutes { get => new Period("5m"); }
        public static Period SixHours { get => new Period("6h"); }
        public static Period EightHours { get => new Period("8h"); }
        public string Value { get; private set; }
        public override string ToString() => this.Value.ToString();
    }
}
