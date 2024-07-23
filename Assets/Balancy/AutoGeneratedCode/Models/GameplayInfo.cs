using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class GameplayInfo : BaseData
	{

		[JsonProperty]
		private int coins;
		[JsonProperty]
		private int energy;
		[JsonProperty]
		private int time;
		[JsonProperty]
		private int moves;
		[JsonProperty]
		private long longValue;
		[JsonProperty]
		private bool openNewWorld;
		[JsonProperty]
		private int avgSessionTime;
		[JsonProperty]
		private bool superBossLevelUnlocked;
		[JsonProperty]
		private string gameLostReason;
		[JsonProperty]
		private int healthPoints;


		[JsonIgnore]
		public int Coins
		{
			get => coins;
			set {
				if (UpdateValue(ref coins, value))
					_cache?.UpdateStorageValue(_path + "Coins", coins);
			}
		}

		[JsonIgnore]
		public int Energy
		{
			get => energy;
			set {
				if (UpdateValue(ref energy, value))
					_cache?.UpdateStorageValue(_path + "Energy", energy);
			}
		}

		[JsonIgnore]
		public int Time
		{
			get => time;
			set {
				if (UpdateValue(ref time, value))
					_cache?.UpdateStorageValue(_path + "Time", time);
			}
		}

		[JsonIgnore]
		public int Moves
		{
			get => moves;
			set {
				if (UpdateValue(ref moves, value))
					_cache?.UpdateStorageValue(_path + "Moves", moves);
			}
		}

		[JsonIgnore]
		public long LongValue
		{
			get => longValue;
			set {
				if (UpdateValue(ref longValue, value))
					_cache?.UpdateStorageValue(_path + "LongValue", longValue);
			}
		}

		[JsonIgnore]
		public bool OpenNewWorld
		{
			get => openNewWorld;
			set {
				if (UpdateValue(ref openNewWorld, value))
					_cache?.UpdateStorageValue(_path + "OpenNewWorld", openNewWorld);
			}
		}

		[JsonIgnore]
		public int AvgSessionTime
		{
			get => avgSessionTime;
			set {
				if (UpdateValue(ref avgSessionTime, value))
					_cache?.UpdateStorageValue(_path + "AvgSessionTime", avgSessionTime);
			}
		}

		[JsonIgnore]
		public bool SuperBossLevelUnlocked
		{
			get => superBossLevelUnlocked;
			set {
				if (UpdateValue(ref superBossLevelUnlocked, value))
					_cache?.UpdateStorageValue(_path + "SuperBossLevelUnlocked", superBossLevelUnlocked);
			}
		}

		[JsonIgnore]
		public string GameLostReason
		{
			get => gameLostReason;
			set {
				if (UpdateValue(ref gameLostReason, value))
					_cache?.UpdateStorageValue(_path + "GameLostReason", gameLostReason);
			}
		}

		[JsonIgnore]
		public int HealthPoints
		{
			get => healthPoints;
			set {
				if (UpdateValue(ref healthPoints, value))
					_cache?.UpdateStorageValue(_path + "HealthPoints", healthPoints);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static GameplayInfo Instantiate()
		{
			return Instantiate<GameplayInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Coins", coins, newValue => Coins = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Energy", energy, newValue => Energy = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Time", time, newValue => Time = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "Moves", moves, newValue => Moves = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "LongValue", longValue, newValue => LongValue = Utils.ToLong(newValue), cache);
			AddCachedItem(path + "OpenNewWorld", openNewWorld, newValue => OpenNewWorld = (bool)newValue, cache);
			AddCachedItem(path + "AvgSessionTime", avgSessionTime, newValue => AvgSessionTime = Utils.ToInt(newValue), cache);
			AddCachedItem(path + "SuperBossLevelUnlocked", superBossLevelUnlocked, newValue => SuperBossLevelUnlocked = (bool)newValue, cache);
			AddCachedItem(path + "GameLostReason", gameLostReason, newValue => GameLostReason = (string)newValue, cache);
			AddCachedItem(path + "HealthPoints", healthPoints, newValue => HealthPoints = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}