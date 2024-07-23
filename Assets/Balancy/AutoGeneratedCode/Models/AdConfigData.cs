using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class AdConfigData : ParentBaseData
	{

		[JsonProperty]
		private Data.AdsGeneralInfo adsGeneralInfo;


		[JsonIgnore]
		public Data.AdsGeneralInfo AdsGeneralInfo => adsGeneralInfo;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref adsGeneralInfo);
		}

		public static AdConfigData Instantiate()
		{
			return Instantiate<AdConfigData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "AdsGeneralInfo", AdsGeneralInfo, null, cache);
		}
	}
#pragma warning restore 649
}