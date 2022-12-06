using BinanceCandlestickDownloader.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BinanceCandlestickDownloader
{
    public class BtcusdtOneHourCandlestickDownloader : DownloaderBase
    {
        public BtcusdtOneHourCandlestickDownloader()
        {
            coinName = "BTC";
            dataPeriod = "1h";
            startDate = DateTime.ParseExact("20200101", "yyyyMMdd", CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact("20221204", "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}
