using System;
using Balancy.Data;
using System.Collections.Generic;

namespace Balancy
{
#pragma warning disable 649

	public partial class DataEditor
	{

		private static void LoadSmartObject(string userId, string name, string key, Action<ParentBaseData> callback)
		{
			switch (name)
			{
				case "BalancyShopData":
				{
					SmartStorage.LoadSmartObject<Data.BalancyShopData>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "SmartObjects.UnnyProfile":
				{
					SmartStorage.LoadSmartObject<Data.SmartObjects.UnnyProfile>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				default:
					UnnyLogger.Critical("No SmartObject found by name " + name);
					break;
			}
		}

		static partial void MoveAllData(string userId)
		{
			MigrateSmartObject(userId, "BalancyShopData");
			MigrateSmartObject(userId, "UnnyProfile");
		}

		static partial void TransferAllSmartObjectsFromLocalToCloud(string userId)
		{
			TransferSmartObjectFromLocalToCloud<Data.BalancyShopData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.SmartObjects.UnnyProfile>(userId);
		}

		static partial void ResetAllSmartObjects(string userId)
		{
			ResetSmartObject<Data.BalancyShopData>(userId);
			ResetSmartObject<Data.SmartObjects.UnnyProfile>(userId);
		}

		static partial void PreloadAllSmartObjects(string userId, bool skipServerLoading)
		{
			SmartStorage.LoadSmartObject<Data.BalancyShopData>(userId, null, skipServerLoading);
		}

		public static List<Models.BattlePass_GameEvent> BattlePass_GameEvents { get; private set; }
		public static List<Models.WheelOfFortune> WheelOfFortunes { get; private set; }
		public static List<Models.BattlePass> BattlePasses { get; private set; }
		public static class BalancyShop
		{
			public static List<Models.BalancyShop.BadgeInfo> BadgeInfos { get; private set; }
			public static List<Models.BalancyShop.GameSection> GameSections { get; private set; }
			public static List<Models.BalancyShop.MyOffer> MyOffers { get; private set; }
			public static List<Models.BalancyShop.MyStoreItem> MyStoreItems { get; private set; }
			public static List<Models.BalancyShop.MyItem> MyItems { get; private set; }
			public static List<Models.BalancyShop.UIStoreItem> UIStoreItems { get; private set; }

			public static void Init()
			{
				BadgeInfos = DataManager.ParseList<Models.BalancyShop.BadgeInfo>();
				GameSections = DataManager.ParseList<Models.BalancyShop.GameSection>();
				MyOffers = DataManager.ParseList<Models.BalancyShop.MyOffer>();
				MyStoreItems = DataManager.ParseList<Models.BalancyShop.MyStoreItem>();
				MyItems = DataManager.ParseList<Models.BalancyShop.MyItem>();
				UIStoreItems = DataManager.ParseList<Models.BalancyShop.UIStoreItem>();
			}
		}
		public static class PvE
		{
			public static List<Models.PvE.Mission> Missions { get; private set; }
			public static List<Models.PvE.DifficultyLevel> DifficultyLevels { get; private set; }
			public static List<Models.PvE.Chapter> Chapters { get; private set; }

			public static void Init()
			{
				Missions = DataManager.ParseList<Models.PvE.Mission>();
				DifficultyLevels = DataManager.ParseList<Models.PvE.DifficultyLevel>();
				Chapters = DataManager.ParseList<Models.PvE.Chapter>();
			}
		}
		public static class Ads
		{
			public static List<Models.Ads.Config> Configs { get; private set; }

			public static void Init()
			{
				Configs = DataManager.ParseList<Models.Ads.Config>();
			}
		}
		public static class MergeGame
		{
			public static List<Models.MergeGame.GameItem> GameItems { get; private set; }
			public static List<Models.MergeGame.Spawner> Spawners { get; private set; }

			public static void Init()
			{
				GameItems = DataManager.ParseList<Models.MergeGame.GameItem>();
				Spawners = DataManager.ParseList<Models.MergeGame.Spawner>();
			}
		}

		static partial void PrepareGeneratedData() {
			BattlePass_GameEvents = DataManager.ParseList<Models.BattlePass_GameEvent>();
			ParseDictionary<Models.BalancyShop.MyCustomSlot>();
			ParseDictionary<Models.BalancyShop.Badge>();
			ParseDictionary<Models.DropItem>();
			WheelOfFortunes = DataManager.ParseList<Models.WheelOfFortune>();
			BattlePasses = DataManager.ParseList<Models.BattlePass>();
			ParseDictionary<Models.BattlePassPoint>();
			ParseDictionary<Models.Ads.Setting.ForcedAds>();
			ParseDictionary<Models.Ads.Setting.DailyBonus>();
			ParseDictionary<Models.Ads.Setting.DoubleRewards>();
			ParseDictionary<Models.MergeGame.ItemEffectCut>();
			ParseDictionary<Models.MergeGame.ItemEffectAutoDrop>();
			ParseDictionary<Models.MergeGame.ItemEffectDowngradable>();
			ParseDictionary<Models.MergeGame.ItemEffectBubble>();
			BalancyShop.Init();
			PvE.Init();
			Ads.Init();
			MergeGame.Init();
			SmartStorage.SetLoadSmartObjectMethod(LoadSmartObject);
		}
	}
#pragma warning restore 649
}