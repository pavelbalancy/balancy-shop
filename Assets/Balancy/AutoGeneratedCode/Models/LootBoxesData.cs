using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class LootBoxesData : ParentBaseData
	{

		[JsonProperty]
		private Data.Counter counters;


		[JsonIgnore]
		public Data.Counter Counters => counters;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref counters);
		}

		public static LootBoxesData Instantiate()
		{
			return Instantiate<LootBoxesData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Counters", Counters, null, cache);
		}
	}
#pragma warning restore 649
}