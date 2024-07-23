using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GeneralInfo : BaseData
	{

		[JsonProperty]
		private int achievementScore;
		[JsonProperty]
		private int gems;
		[JsonProperty]
		private int gold;
		[JsonProperty]
		private int elixir;
		[JsonProperty]
		private int maxGold;
		[JsonProperty]
		private int maxElixir;
		[JsonProperty]
		private int darkElixir;
		[JsonProperty]
		private int maxDarkElixir;


		[JsonIgnore]
		public int AchievementScore
		{
			get => achievementScore;
			set {
				if (UpdateValue(ref achievementScore, value))
					_cache?.UpdateStorageValue(_path + "AchievementScore", achievementScore);
			}
		}

		[JsonIgnore]
		public int Gems
		{
			get => gems;
			set {
				if (UpdateValue(ref gems, value))
					_cache?.UpdateStorageValue(_path + "Gems", gems);
			}
		}

		[JsonIgnore]
		public int Gold
		{
			get => gold;
			set {
				if (UpdateValue(ref gold, value))
					_cache?.UpdateStorageValue(_path + "Gold", gold);
			}
		}

		[JsonIgnore]
		public int Elixir
		{
			get => elixir;
			set {
				if (UpdateValue(ref elixir, value))
					_cache?.UpdateStorageValue(_path + "Elixir", elixir);
			}
		}

		[JsonIgnore]
		public int MaxGold
		{
			get => maxGold;
			set {
				if (UpdateValue(ref maxGold, value))
					_cache?.UpdateStorageValue(_path + "MaxGold", maxGold);
			}
		}

		[JsonIgnore]
		public int MaxElixir
		{
			get => maxElixir;
			set {
				if (UpdateValue(ref maxElixir, value))
					_cache?.UpdateStorageValue(_path + "MaxElixir", maxElixir);
			}
		}

		[JsonIgnore]
		public int DarkElixir
		{
			get => darkElixir;
			set {
				if (UpdateValue(ref darkElixir, value))
					_cache?.UpdateStorageValue(_path + "DarkElixir", darkElixir);
			}
		}

		[JsonIgnore]
		public int MaxDarkElixir
		{
			get => maxDarkElixir;
			set {
				if (UpdateValue(ref maxDarkElixir, value))
					_cache?.UpdateStorageValue(_path + "MaxDarkElixir", maxDarkElixir);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static GeneralInfo Instantiate()
		{
			return Instantiate<GeneralInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "AchievementScore", achievementScore, newValue => AchievementScore = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Gems", gems, newValue => Gems = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Gold", gold, newValue => Gold = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Elixir", elixir, newValue => Elixir = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "MaxGold", maxGold, newValue => MaxGold = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "MaxElixir", maxElixir, newValue => MaxElixir = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "DarkElixir", darkElixir, newValue => DarkElixir = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "MaxDarkElixir", maxDarkElixir, newValue => MaxDarkElixir = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}