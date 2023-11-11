using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Skymey_main_lib.Models.ETF.Tinkoff;
using Skymey_main_lib.Models.Futures.Tinkoff;
using Skymey_stock_tinkoff_futureslist.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinkoff.InvestApi;

namespace Skymey_stock_tinkoff_futureslist.Actions.GetFutures
{
    public class GetFutures
    {
        private MongoClient _mongoClient;
        private ApplicationContext _db;
        private string _apiKey;
        public GetFutures()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);

            IConfiguration config = builder.Build();

            _apiKey = config.GetSection("ApiKeys:Tinkoff").Value;
            _mongoClient = new MongoClient("mongodb://127.0.0.1:27017");
            _db = ApplicationContext.Create(_mongoClient.GetDatabase("skymey"));
        }
        public void GetFuturesFromTinkoff()
        {
            var client = InvestApiClientFactory.Create(_apiKey);
            var response = client.Instruments.Futures();
            var ticker_finds = (from i in _db.Futures select i);
            foreach (var item in response.Instruments)
            {
                Console.WriteLine(item.Ticker);
                var ticker_find = (from i in ticker_finds where i.ticker == item.Ticker && i.figi == item.Figi select i).FirstOrDefault();
                if (ticker_find == null)
                {
                    TinkoffFuturesInstrument tfi = new TinkoffFuturesInstrument();
                    tfi._id = ObjectId.GenerateNewId();
                    tfi.figi = item.Figi;
                    if (tfi.figi == null) tfi.figi = "";
                    tfi.ticker = item.Ticker;
                    if (tfi.ticker == null) tfi.ticker = "";
                    tfi.classCode = item.ClassCode;
                    if (tfi.classCode == null) tfi.classCode = "";
                    tfi.lot = item.Lot;
                    if (tfi.lot == null) tfi.lot = 0;
                    tfi.currency = item.Currency;
                    if (tfi.currency == null) tfi.currency = "";
                    if (item.Klong != null)
                    {
                        TinkoffFuturesKlong tfkl = new TinkoffFuturesKlong();
                        tfkl.units = item.Klong.Units;
                        tfkl.nano = item.Klong.Nano;
                        tfi.klong = tfkl;
                    }
                    else
                    {
                        tfi.klong = new TinkoffFuturesKlong();
                    }
                    if (item.Kshort != null)
                    {
                        TinkoffFuturesKshort tfks = new TinkoffFuturesKshort();
                        tfks.units = item.Kshort.Units;
                        tfks.nano = item.Kshort.Nano;
                        tfi.kshort = tfks;
                    }
                    else
                    {
                        tfi.kshort = new TinkoffFuturesKshort();
                    }
                    if (item.Dlong != null)
                    {
                        TinkoffFuturesDlong tfdl = new TinkoffFuturesDlong();
                        tfdl.units = item.Dlong.Units;
                        tfdl.nano = item.Dlong.Nano;
                        tfi.dlong = tfdl;
                    }
                    else
                    {
                        tfi.dlong = new TinkoffFuturesDlong();
                    }
                    if (item.Dshort != null )
                    {
                        TinkoffFuturesDshort tfds = new TinkoffFuturesDshort();
                        tfds.units = item.Dshort.Units;
                        tfds.nano = item.Dshort.Nano;
                        tfi.dshort = tfds;
                    }
                    else
                    {
                        tfi.dshort = new TinkoffFuturesDshort();
                    }
                    if (item.DlongMin != null)
                    {
                        TinkoffFuturesDlongMin tfdlm = new TinkoffFuturesDlongMin();
                        tfdlm.units = item.DlongMin.Units;
                        tfdlm.nano = item.DlongMin.Nano;
                        tfi.dlongMin = tfdlm;
                    }
                    else
                    {
                        tfi.dlongMin = new TinkoffFuturesDlongMin();
                    }
                    if (item.DshortMin != null)
                    {
                        TinkoffFuturesDshortMin tfdsm = new TinkoffFuturesDshortMin();
                        tfdsm.units = item.DshortMin.Units;
                        tfdsm.nano = item.DshortMin.Nano;
                        tfi.dshortMin = tfdsm;
                    }
                    else
                    {
                        tfi.dshortMin = new TinkoffFuturesDshortMin();
                    }
                    tfi.shortEnabledFlag = item.ShortEnabledFlag;
                    if (tfi.shortEnabledFlag == null) tfi.shortEnabledFlag = false;
                    tfi.name = item.Name;
                    if (tfi.name == null) tfi.name = "";
                    tfi.exchange = item.Exchange;
                    if (tfi.exchange == null) tfi.exchange = "";
                    tfi.first1minCandleDate = item.First1MinCandleDate;
                    if (tfi.first1minCandleDate == null) tfi.first1minCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tfi.first1dayCandleDate = item.First1DayCandleDate;
                    if (tfi.first1dayCandleDate == null) tfi.first1dayCandleDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tfi.futuresType = item.FuturesType;
                    if (tfi.futuresType == null) tfi.futuresType = "";
                    tfi.assetType = item.AssetType;
                    if (tfi.assetType == null) tfi.assetType = "";
                    tfi.basicAsset = item.BasicAsset;
                    if (tfi.basicAsset == null) tfi.basicAsset = "";
                    if (item.BasicAssetSize != null)
                    {
                        TinkoffFuturesBasicAssetSize tfbas = new TinkoffFuturesBasicAssetSize();
                        tfbas.units = item.BasicAssetSize.Units;
                        tfbas.nano = item.BasicAssetSize.Nano;
                        tfi.basicAssetSize = tfbas;
                    }
                    else
                    {
                        tfi.basicAssetSize = new TinkoffFuturesBasicAssetSize();
                    }
                    tfi.countryOfRisk = item.CountryOfRisk;
                    if (tfi.countryOfRisk == null) tfi.countryOfRisk = "";
                    tfi.countryOfRiskName = item.CountryOfRiskName;
                    if (tfi.countryOfRiskName == null) tfi.countryOfRiskName = "";
                    tfi.sector = item.Sector;
                    if (tfi.sector == null) tfi.sector = "";
                    tfi.expirationDate = item.ExpirationDate;
                    if (tfi.expirationDate == null) tfi.expirationDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tfi.tradingStatus = item.TradingStatus.ToString();
                    if (tfi.tradingStatus == null) tfi.tradingStatus = "";
                    tfi.otcFlag = item.OtcFlag;
                    if (tfi.otcFlag == null) tfi.otcFlag = false;
                    tfi.buyAvailableFlag = item.BuyAvailableFlag;
                    if (tfi.buyAvailableFlag == null) tfi.buyAvailableFlag = false;
                    tfi.sellAvailableFlag = item.SellAvailableFlag;
                    if (tfi.sellAvailableFlag == null) tfi.sellAvailableFlag = false;
                    if (item.MinPriceIncrement != null)
                    {
                        TinkoffFuturesMinPriceIncrement tfmpi = new TinkoffFuturesMinPriceIncrement();
                        tfmpi.units = item.MinPriceIncrement.Units;
                        tfmpi.nano = item.MinPriceIncrement.Nano;
                        tfi.minPriceIncrement = tfmpi;
                    }
                    else
                    {
                        tfi.minPriceIncrement = new TinkoffFuturesMinPriceIncrement();
                    }
                    tfi.apiTradeAvailableFlag = item.ApiTradeAvailableFlag;
                    if (tfi.apiTradeAvailableFlag == null) tfi.apiTradeAvailableFlag = false;
                    tfi.uid = item.Uid;
                    if (tfi.uid == null) tfi.uid = "";
                    tfi.realExchange = item.RealExchange.ToString();
                    if (tfi.realExchange == null) tfi.realExchange = "";
                    tfi.positionUid = item.PositionUid;
                    if (tfi.positionUid == null) tfi.positionUid = "";
                    tfi.basicAssetPositionUid = item.BasicAssetPositionUid;
                    if (tfi.basicAssetPositionUid == null) tfi.basicAssetPositionUid = "";
                    tfi.forIisFlag = item.ForIisFlag;
                    if (tfi.forIisFlag == null) tfi.forIisFlag = false;
                    tfi.forQualInvestorFlag = item.ForQualInvestorFlag;
                    if (tfi.forQualInvestorFlag == null) tfi.forQualInvestorFlag = false;
                    tfi.weekendFlag = item.WeekendFlag;
                    if (tfi.weekendFlag == null) tfi.weekendFlag = false;
                    tfi.blockedTcaFlag = item.BlockedTcaFlag;
                    if (tfi.blockedTcaFlag == null) tfi.blockedTcaFlag = false;
                    tfi.lastTradeDate = item.LastTradeDate;
                    if (tfi.lastTradeDate == null) tfi.lastTradeDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tfi.firstTradeDate = item.FirstTradeDate;
                    if (tfi.firstTradeDate == null) tfi.firstTradeDate = Timestamp.FromDateTime(DateTime.UtcNow);
                    tfi.Update = DateTime.UtcNow;
                    _db.Futures.Add(tfi);
                }
            }
            _db.SaveChanges();
        }
    }
}
