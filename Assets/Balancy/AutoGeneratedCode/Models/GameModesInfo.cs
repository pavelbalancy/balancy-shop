using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GameModesInfo : BaseData
	{

		[JsonProperty]
		private int arenaModeUnlockLevel;
		[JsonProperty]
		private int adventureModeUnlockLevel;
		[JsonProperty]
		private int clanWarUnlockLevel;


		[JsonIgnore]
		public int ArenaModeUnlockLevel
		{
			get => arenaModeUnlockLevel;
			set {
				if (UpdateValue(ref arenaModeUnlockLevel, value))
					_cache?.UpdateStorageValue(_path + "ArenaModeUnlockLevel", arenaModeUnlockLevel);
			}
		}

		[JsonIgnore]
		public int AdventureModeUnlockLevel
		{
			get => adventureModeUnlockLevel;
			set {
				if (UpdateValue(ref adventureModeUnlockLevel, value))
					_cache?.UpdateStorageValue(_path + "AdventureModeUnlockLevel", adventureModeUnlockLevel);
			}
		}

		[JsonIgnore]
		public int ClanWarUnlockLevel
		{
			get => clanWarUnlockLevel;
			set {
				if (UpdateValue(ref clanWarUnlockLevel, value))
					_cache?.UpdateStorageValue(_path + "ClanWarUnlockLevel", clanWarUnlockLevel);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static GameModesInfo Instantiate()
		{
			return Instantiate<GameModesInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "ArenaModeUnlockLevel", arenaModeUnlockLevel, newValue => ArenaModeUnlockLevel = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "AdventureModeUnlockLevel", adventureModeUnlockLevel, newValue => AdventureModeUnlockLevel = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "ClanWarUnlockLevel", clanWarUnlockLevel, newValue => ClanWarUnlockLevel = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}