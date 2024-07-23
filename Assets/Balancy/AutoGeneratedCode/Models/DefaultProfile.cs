using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class DefaultProfile : ParentBaseData
	{

		[JsonProperty]
		private Data.GeneralInfo info;


		[JsonIgnore]
		public Data.GeneralInfo Info => info;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref info);
		}

		public static DefaultProfile Instantiate()
		{
			return Instantiate<DefaultProfile>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Info", Info, null, cache);
		}
	}
#pragma warning restore 649
}