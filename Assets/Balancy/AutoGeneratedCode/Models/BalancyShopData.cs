using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class BalancyShopData : ParentBaseData
	{

		[JsonProperty]
		private Data.BalancyShopGeneralInfo info;


		[JsonIgnore]
		public Data.BalancyShopGeneralInfo Info => info;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref info);
		}

		public static BalancyShopData Instantiate()
		{
			return Instantiate<BalancyShopData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Info", Info, null, cache);
		}
	}
#pragma warning restore 649
}