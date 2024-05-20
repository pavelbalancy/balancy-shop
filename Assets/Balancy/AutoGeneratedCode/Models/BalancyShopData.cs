using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class BalancyShopData : ParentBaseData
	{

		[JsonProperty]
		private Data.BalancyShopGeneralInfo info;
		[JsonProperty]
		private Data.BattlePassInfo battlePassInfo;


		[JsonIgnore]
		public Data.BalancyShopGeneralInfo Info => info;

		[JsonIgnore]
		public Data.BattlePassInfo BattlePassInfo => battlePassInfo;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref info);
			ValidateData(ref battlePassInfo);
		}

		public static BalancyShopData Instantiate()
		{
			return Instantiate<BalancyShopData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Info", Info, null, cache);
			AddCachedItem(path + "BattlePassInfo", BattlePassInfo, null, cache);
		}
	}
#pragma warning restore 649
}