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
				case "GameModesData":
				{
					SmartStorage.LoadSmartObject<Data.GameModesData>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "AdConfigData":
				{
					SmartStorage.LoadSmartObject<Data.AdConfigData>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "Profile":
				{
					SmartStorage.LoadSmartObject<Data.Profile>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "LootBoxesData":
				{
					SmartStorage.LoadSmartObject<Data.LootBoxesData>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "GameplayData":
				{
					SmartStorage.LoadSmartObject<Data.GameplayData>(userId, key, responseData =>
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
				case "DefaultProfile":
				{
					SmartStorage.LoadSmartObject<Data.DefaultProfile>(userId, key, responseData =>
					{
						callback?.Invoke(responseData.Data);
					});
					break;
				}
				case "BalancyShopData":
				{
					SmartStorage.LoadSmartObject<Data.BalancyShopData>(userId, key, responseData =>
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
			MigrateSmartObject(userId, "GameModesData");
			MigrateSmartObject(userId, "AdConfigData");
			MigrateSmartObject(userId, "Profile");
			MigrateSmartObject(userId, "LootBoxesData");
			MigrateSmartObject(userId, "GameplayData");
			MigrateSmartObject(userId, "UnnyProfile");
			MigrateSmartObject(userId, "DefaultProfile");
			MigrateSmartObject(userId, "BalancyShopData");
		}

		static partial void TransferAllSmartObjectsFromLocalToCloud(string userId)
		{
			TransferSmartObjectFromLocalToCloud<Data.GameModesData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.AdConfigData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.Profile>(userId);
			TransferSmartObjectFromLocalToCloud<Data.LootBoxesData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.GameplayData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.SmartObjects.UnnyProfile>(userId);
			TransferSmartObjectFromLocalToCloud<Data.DefaultProfile>(userId);
			TransferSmartObjectFromLocalToCloud<Data.BalancyShopData>(userId);
		}

		static partial void ResetAllSmartObjects(string userId)
		{
			ResetSmartObject<Data.GameModesData>(userId);
			ResetSmartObject<Data.AdConfigData>(userId);
			ResetSmartObject<Data.Profile>(userId);
			ResetSmartObject<Data.LootBoxesData>(userId);
			ResetSmartObject<Data.GameplayData>(userId);
			ResetSmartObject<Data.SmartObjects.UnnyProfile>(userId);
			ResetSmartObject<Data.DefaultProfile>(userId);
			ResetSmartObject<Data.BalancyShopData>(userId);
		}

		static partial void PreloadAllSmartObjects(string userId, bool skipServerLoading)
		{
			SmartStorage.LoadSmartObject<Data.GameModesData>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.AdConfigData>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.Profile>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.LootBoxesData>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.GameplayData>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.DefaultProfile>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.BalancyShopData>(userId, null, skipServerLoading);
		}

		public static List<Models.WheelOfFortune> WheelOfFortunes { get; private set; }
		public static class PvE
		{
			public static List<Models.PvE.Quest> Quests { get; private set; }
			public static List<Models.PvE.Mission> Missions { get; private set; }
			public static List<Models.PvE.DifficultyLevel> DifficultyLevels { get; private set; }
			public static List<Models.PvE.Chapter> Chapters { get; private set; }

			public static void Init()
			{
				Quests = DataManager.ParseList<Models.PvE.Quest>();
				Missions = DataManager.ParseList<Models.PvE.Mission>();
				DifficultyLevels = DataManager.ParseList<Models.PvE.DifficultyLevel>();
				Chapters = DataManager.ParseList<Models.PvE.Chapter>();
			}
		}
		public static class BattlePass
		{
			public static List<Models.BattlePass.BattlePass> BattlePasses { get; private set; }
			public static List<Models.BattlePass.BattlePassGameEvent> BattlePassGameEvents { get; private set; }

			public static void Init()
			{
				BattlePasses = DataManager.ParseList<Models.BattlePass.BattlePass>();
				BattlePassGameEvents = DataManager.ParseList<Models.BattlePass.BattlePassGameEvent>();
			}
		}
		public static class PlayableCharacterConfig
		{
			public static List<Models.PlayableCharacterConfig.LevelProgression> LevelProgressions { get; private set; }
			public static List<Models.PlayableCharacterConfig.TierProgression> TierProgressions { get; private set; }
			public static List<Models.PlayableCharacterConfig.Ability> Abilities { get; private set; }
			public static List<Models.PlayableCharacterConfig.Character> Characters { get; private set; }

			public static void Init()
			{
				LevelProgressions = DataManager.ParseList<Models.PlayableCharacterConfig.LevelProgression>();
				TierProgressions = DataManager.ParseList<Models.PlayableCharacterConfig.TierProgression>();
				Abilities = DataManager.ParseList<Models.PlayableCharacterConfig.Ability>();
				Characters = DataManager.ParseList<Models.PlayableCharacterConfig.Character>();
			}
		}
		public static class BalancyShop
		{
			public static List<Models.BalancyShop.BadgeInfo> BadgeInfos { get; private set; }
			public static List<Models.BalancyShop.MyOffer> MyOffers { get; private set; }
			public static List<Models.BalancyShop.MyStoreItem> MyStoreItems { get; private set; }
			public static List<Models.BalancyShop.MyItem> MyItems { get; private set; }
			public static List<Models.BalancyShop.GameSection> GameSections { get; private set; }
			public static List<Models.BalancyShop.UIStoreItem> UIStoreItems { get; private set; }

			public static void Init()
			{
				BadgeInfos = DataManager.ParseList<Models.BalancyShop.BadgeInfo>();
				MyOffers = DataManager.ParseList<Models.BalancyShop.MyOffer>();
				MyStoreItems = DataManager.ParseList<Models.BalancyShop.MyStoreItem>();
				MyItems = DataManager.ParseList<Models.BalancyShop.MyItem>();
				GameSections = DataManager.ParseList<Models.BalancyShop.GameSection>();
				UIStoreItems = DataManager.ParseList<Models.BalancyShop.UIStoreItem>();
			}
		}
		public static class Equipment
		{
			public static List<Models.Equipment.Weapons> Weapons { get; private set; }
			public static List<Models.Equipment.Clothes> Clothes { get; private set; }
			public static List<Models.Equipment.Consumables> Consumables { get; private set; }

			public static void Init()
			{
				Weapons = DataManager.ParseList<Models.Equipment.Weapons>();
				Clothes = DataManager.ParseList<Models.Equipment.Clothes>();
				Consumables = DataManager.ParseList<Models.Equipment.Consumables>();
			}
		}
		public static class LootBox
		{
			public static List<Models.LootBox.LootBox> LootBoxes { get; private set; }

			public static void Init()
			{
				LootBoxes = DataManager.ParseList<Models.LootBox.LootBox>();
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
		public static class Collections
		{
			public static List<Models.Collections.CollectionItem> CollectionItems { get; private set; }
			public static List<Models.Collections.CollectionEvent> CollectionEvents { get; private set; }
			public static List<Models.Collections.CollectionInfo> CollectionInfos { get; private set; }

			public static void Init()
			{
				CollectionItems = DataManager.ParseList<Models.Collections.CollectionItem>();
				CollectionEvents = DataManager.ParseList<Models.Collections.CollectionEvent>();
				CollectionInfos = DataManager.ParseList<Models.Collections.CollectionInfo>();
			}
		}
		public static class NPCConfig
		{
			public static List<Models.NPCConfig.RegularNPC> RegularNPCS { get; private set; }
			public static List<Models.NPCConfig.BossNPC> BossNPCS { get; private set; }

			public static void Init()
			{
				RegularNPCS = DataManager.ParseList<Models.NPCConfig.RegularNPC>();
				BossNPCS = DataManager.ParseList<Models.NPCConfig.BossNPC>();
			}
		}

		static partial void PrepareGeneratedData() {
			ParseDictionary<Models.BalancyShop.Badge>();
			ParseDictionary<Models.CharacterConfig.TierLevel>();
			ParseDictionary<Models.LootBox.DropItem>();
			ParseDictionary<Models.Ads.Setting.DoubleRewards>();
			ParseDictionary<Models.Ads.Setting.DailyBonus>();
			ParseDictionary<Models.ContentHolder>();
			ParseDictionary<Models.BalancyShop.StoreSlotWithUI>();
			ParseDictionary<Models.ActivatedOffer>();
			ParseDictionary<Models.Ads.Setting.ForcedAds>();
			WheelOfFortunes = DataManager.ParseList<Models.WheelOfFortune>();
			ParseDictionary<Models.BattlePassPoint>();
			PvE.Init();
			BattlePass.Init();
			PlayableCharacterConfig.Init();
			BalancyShop.Init();
			Equipment.Init();
			LootBox.Init();
			Ads.Init();
			Collections.Init();
			NPCConfig.Init();
			SmartStorage.SetLoadSmartObjectMethod(LoadSmartObject);
		}
	}
#pragma warning restore 649
}