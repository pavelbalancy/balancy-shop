using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class BalancyShopData : ParentBaseData
	{

		[JsonProperty]
		private Data.BalancyShopGeneralInfo shopInfo;


		[JsonIgnore]
		public Data.BalancyShopGeneralInfo ShopInfo => shopInfo;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref shopInfo);
		}

		public static BalancyShopData Instantiate()
		{
			return Instantiate<BalancyShopData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "ShopInfo", ShopInfo, null, cache);
		}
	}
#pragma warning restore 649
}