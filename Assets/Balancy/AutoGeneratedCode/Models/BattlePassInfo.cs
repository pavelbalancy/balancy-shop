using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class BattlePassInfo : BaseData
	{

		[JsonProperty]
		private int lastClaimedReward;
		[JsonProperty]
		private int scores;


		[JsonIgnore]
		public int LastClaimedReward
		{
			get => lastClaimedReward;
			set {
				if (UpdateValue(ref lastClaimedReward, value))
					_cache?.UpdateStorageValue(_path + "LastClaimedReward", lastClaimedReward);
			}
		}

		[JsonIgnore]
		public int Scores
		{
			get => scores;
			set {
				if (UpdateValue(ref scores, value))
					_cache?.UpdateStorageValue(_path + "Scores", scores);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static BattlePassInfo Instantiate()
		{
			return Instantiate<BattlePassInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "LastClaimedReward", lastClaimedReward, newValue => LastClaimedReward = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Scores", scores, newValue => Scores = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}