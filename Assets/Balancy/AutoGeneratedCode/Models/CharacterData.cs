using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class CharacterData : BaseData
	{

		[JsonProperty, JsonConverter(typeof(SmartListConverter<string>))]
		private SmartList<string> equip;
		[JsonProperty]
		private int currentExp;
		[JsonProperty]
		private int currentTier;
		[JsonProperty, JsonConverter(typeof(SmartListConverter<string>))]
		private SmartList<string> abilities;
		[JsonProperty, JsonConverter(typeof(SmartListConverter<string>))]
		private SmartList<string> activatedOffers;
		[JsonProperty]
		private string characterConfig;
		[JsonProperty]
		private int currentLevel;


		[JsonIgnore]
		public SmartList<string> Equip => equip;

		[JsonIgnore]
		public int CurrentExp
		{
			get => currentExp;
			set {
				if (UpdateValue(ref currentExp, value))
					_cache?.UpdateStorageValue(_path + "CurrentExp", currentExp);
			}
		}

		[JsonIgnore]
		public int CurrentTier
		{
			get => currentTier;
			set {
				if (UpdateValue(ref currentTier, value))
					_cache?.UpdateStorageValue(_path + "CurrentTier", currentTier);
			}
		}

		[JsonIgnore]
		public SmartList<string> Abilities => abilities;

		[JsonIgnore]
		public SmartList<string> ActivatedOffers => activatedOffers;

		[JsonIgnore]
		public string CharacterConfig
		{
			get => characterConfig;
			set {
				if (UpdateValue(ref characterConfig, value))
					_cache?.UpdateStorageValue(_path + "CharacterConfig", characterConfig);
			}
		}

		[JsonIgnore]
		public int CurrentLevel
		{
			get => currentLevel;
			set {
				if (UpdateValue(ref currentLevel, value))
					_cache?.UpdateStorageValue(_path + "CurrentLevel", currentLevel);
			}
		}

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref equip);
			ValidateData(ref abilities);
			ValidateData(ref activatedOffers);
		}

		public static CharacterData Instantiate()
		{
			return Instantiate<CharacterData>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Equip", Equip, null, cache);
			AddCachedItem(path + "CurrentExp", currentExp, newValue => CurrentExp = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "CurrentTier", currentTier, newValue => CurrentTier = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Abilities", Abilities, null, cache);
			AddCachedItem(path + "ActivatedOffers", ActivatedOffers, null, cache);
			AddCachedItem(path + "CharacterConfig", characterConfig, newValue => CharacterConfig = (string)newValue, cache);
			AddCachedItem(path + "CurrentLevel", currentLevel, newValue => CurrentLevel = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}