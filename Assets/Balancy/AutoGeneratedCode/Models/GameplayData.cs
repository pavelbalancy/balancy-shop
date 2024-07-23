using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GameplayData : ParentBaseData
	{

		[JsonProperty]
		private Data.GameplayInfo gameplayInfo;


		[JsonIgnore]
		public Data.GameplayInfo GameplayInfo => gameplayInfo;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref gameplayInfo);
		}

		public static GameplayData Instantiate()
		{
			return Instantiate<GameplayData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "GameplayInfo", GameplayInfo, null, cache);
		}
	}
#pragma warning restore 649
}