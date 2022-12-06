using BinanceCandlestickDownloader;

internal class Program
{
    private static void Main(string[] args)
    {
        DownloaderBase downloader = new BtcusdtOneHourCandlestickDownloader();
        downloader.GetCandlestickFile();
        downloader.ConvertFileFormatForBacktesting();
    }
}