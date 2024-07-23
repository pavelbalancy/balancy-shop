using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class AdsGeneralInfo : BaseData
	{

		[JsonProperty]
		private string currentConfig;
		[JsonProperty, JsonConverter(typeof(SmartListConverter<string>))]
		private SmartList<string> configList;
		[JsonProperty]
		private bool isNoAdsPurchased;


		[JsonIgnore]
		public string CurrentConfig
		{
			get => currentConfig;
			set {
				if (UpdateValue(ref currentConfig, value))
					_cache?.UpdateStorageValue(_path + "CurrentConfig", currentConfig);
			}
		}

		[JsonIgnore]
		public SmartList<string> ConfigList => configList;

		[JsonIgnore]
		public bool IsNoAdsPurchased
		{
			get => isNoAdsPurchased;
			set {
				if (UpdateValue(ref isNoAdsPurchased, value))
					_cache?.UpdateStorageValue(_path + "IsNoAdsPurchased", isNoAdsPurchased);
			}
		}

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref configList);
		}

		public static AdsGeneralInfo Instantiate()
		{
			return Instantiate<AdsGeneralInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "CurrentConfig", currentConfig, newValue => CurrentConfig = (string)newValue, cache);
			AddCachedItem(path + "ConfigList", ConfigList, null, cache);
			AddCachedItem(path + "IsNoAdsPurchased", isNoAdsPurchased, newValue => IsNoAdsPurchased = (bool)newValue, cache);
		}
	}
#pragma warning restore 649
}