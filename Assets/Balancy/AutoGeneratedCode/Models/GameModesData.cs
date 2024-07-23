using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GameModesData : ParentBaseData
	{

		[JsonProperty]
		private Data.GameModesInfo gameModesInfo;


		[JsonIgnore]
		public Data.GameModesInfo GameModesInfo => gameModesInfo;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref gameModesInfo);
		}

		public static GameModesData Instantiate()
		{
			return Instantiate<GameModesData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "GameModesInfo", GameModesInfo, null, cache);
		}
	}
#pragma warning restore 649
}