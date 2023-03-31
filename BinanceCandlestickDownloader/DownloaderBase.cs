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
    public class DownloaderBase
    {
        protected string coinName;
        protected string dataPeriod;
        protected DateTime startDate;
        protected DateTime endDate;

        public DownloaderBase() 
        {
        }

        public DownloaderBase(string coinName_AllCapital, Period period, string startTime_yyyyMMdd, string endTime_yyyyMMdd)
        {
            coinName = coinName_AllCapital;
            dataPeriod = period.Value;
            startDate = DateTime.ParseExact(startTime_yyyyMMdd, "yyyyMMdd", CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact(endTime_yyyyMMdd, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        public void GetCandlestickFile()
        {
            var startDateForWhileLoop = startDate;
            var endDateForWhileLoop = endDate;

            while (startDateForWhileLoop <= endDateForWhileLoop)
            {
                GetZipFileFromBinance(coinName + "USDT-" + dataPeriod + "-" + startDateForWhileLoop.ToString("yyyy-MM-dd"));
                startDateForWhileLoop = startDateForWhileLoop.AddDays(1);

                //每次下載休息數毫秒，以免要求存取過於頻繁
                Thread.Sleep(100);
            }
        }

        private void GetZipFileFromBinance(string fileName)
        {
            //假如檔案已經存在就不重新下載
            if (File.Exists("tempFile/" + fileName + ".zip"))
                return;

            using (var client = new WebClient())
            {
                try
                {
                    string downloadUrl = "https://data.binance.vision/data/futures/um/daily/klines/" + coinName + "USDT/" + dataPeriod + "/" + fileName + ".zip";
                    client.DownloadFile(downloadUrl, "tempFile/" + fileName + ".zip");
                }
                catch
                {
                    //假如檔案不存在，就catch但不throw exception，讓程式繼續運作即可
                }

            }
        }

        //將Tempfile的檔案解壓縮，並轉換成交易回測所需的格式
        public void ConvertFileFormatForBacktesting()
        {
            var startDateForWhileLoop = startDate;
            var endDateForWhileLoop = endDate;

            while (startDateForWhileLoop <= endDateForWhileLoop)
            {
                string fileName = coinName + "USDT-" + dataPeriod + "-" + startDateForWhileLoop.ToString("yyyy-MM-dd");
                string zipPath = "tempFile/" + fileName + ".zip";
                string extractPath = "tempFile/";

                //解壓縮檔案
                using (ZipArchive archive = ZipFile.Open(zipPath, ZipArchiveMode.Update))
                {
                    if (!File.Exists(extractPath + fileName + ".csv"))
                    {
                        archive.ExtractToDirectory(extractPath);
                    }
                }

                //將解壓縮後的檔案轉換成回測用的格式並寫入其檔案
                ConvertPrimaryCsvToBacktestingFormat(fileName + ".csv");

                startDateForWhileLoop = startDateForWhileLoop.AddDays(1);
            }
        }

        //將下載到的原始檔案，轉換成回測可用的格式
        private void ConvertPrimaryCsvToBacktestingFormat(string fileName)
        {
            using (var reader = new StreamReader("tempFile/" + fileName))
            {
                int i = 0;

                while (!reader.EndOfStream)
                {
                    var unfomattedDataRow = reader.ReadLine();
                    //假如第一列有標頭的話，就再讀下一列
                    if (i == 0 && IsCsvHasTitleRow(unfomattedDataRow))
                    {
                        unfomattedDataRow = reader.ReadLine();
                    }

                    //寫入此列資料至新檔案
                    string formattedDataRow = FormatBinanceDatarowToBacktestingDataRow(unfomattedDataRow);
                    AppendDataRowToBacktestingFormatCSV(formattedDataRow);

                    i++;
                }
            }
        }

        //將幣安的原始資料，轉換成回測可用的格式
        private string FormatBinanceDatarowToBacktestingDataRow(string unFormattedDataRow)
        {
            string[] rowArray = unFormattedDataRow.Split(',');

            var formattedString = new StringBuilder();

            //使用Model來裝資料，較有可讀性
            CandlestickModel candlestickModel = new CandlestickModel();
            candlestickModel.OpenPrice = Decimal.Parse(rowArray[1]);
            candlestickModel.ClosePrice = Decimal.Parse(rowArray[4]);
            candlestickModel.HighPrice = Decimal.Parse(rowArray[2]);
            candlestickModel.LowPrice = Decimal.Parse(rowArray[3]);
            candlestickModel.Volume = Decimal.Parse(rowArray[5]);

            //把Unix時間格式轉為yyyyMMddHHmmss
            DateTime dateTime = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(rowArray[6])).DateTime;
            string closeTime = dateTime.ToString("yyyyMMddHHmmss");

            //column依序分別為開盤價,收盤價,最高價,最低價,成交量,時間
            var formattedDataRow = string.Format("{0},{1},{2},{3},{4},{5}", candlestickModel.OpenPrice, candlestickModel.ClosePrice, candlestickModel.HighPrice, candlestickModel.LowPrice, candlestickModel.Volume, closeTime + "\r\n");

            return formattedDataRow;
        }

        //將dataRow Append至最新一列
        private void AppendDataRowToBacktestingFormatCSV(string formattedDataRow)
        {
            string fileName = coinName + "usdt" + dataPeriod + "CandlestickCsv.csv";
            string filePath = "tempFile/" + fileName;
            //SetHeadToFormattedCSV(fileName);
            File.AppendAllText(filePath, formattedDataRow);
        }

        private bool IsCsvHasTitleRow(string firstRow)
        {
            decimal decimalForTryParse = 0;
            string[] rowArray = firstRow.Split(',');
            return !Decimal.TryParse(rowArray[0], out decimalForTryParse);
        }
    }
}
