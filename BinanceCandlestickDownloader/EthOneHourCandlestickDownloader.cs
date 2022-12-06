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
    public class EthOneHourCandlestickDownloader : DownloaderBase
    {
        public EthOneHourCandlestickDownloader()
        {
            coinName = "ETH";
            dataPeriod = "1h";
            startDate = DateTime.ParseExact("20200329", "yyyyMMdd", CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact("20221204", "yyyyMMdd", CultureInfo.InvariantCulture);
        }
    }
}
