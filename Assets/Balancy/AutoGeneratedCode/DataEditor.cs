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
				case "LootBoxesData":
				{
					SmartStorage.LoadSmartObject<Data.LootBoxesData>(userId, key, responseData =>
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
			MigrateSmartObject(userId, "LootBoxesData");
		}

		static partial void TransferAllSmartObjectsFromLocalToCloud(string userId)
		{
			TransferSmartObjectFromLocalToCloud<Data.BalancyShopData>(userId);
			TransferSmartObjectFromLocalToCloud<Data.SmartObjects.UnnyProfile>(userId);
			TransferSmartObjectFromLocalToCloud<Data.LootBoxesData>(userId);
		}

		static partial void ResetAllSmartObjects(string userId)
		{
			ResetSmartObject<Data.BalancyShopData>(userId);
			ResetSmartObject<Data.SmartObjects.UnnyProfile>(userId);
			ResetSmartObject<Data.LootBoxesData>(userId);
		}

		static partial void PreloadAllSmartObjects(string userId, bool skipServerLoading)
		{
			SmartStorage.LoadSmartObject<Data.BalancyShopData>(userId, null, skipServerLoading);
			SmartStorage.LoadSmartObject<Data.LootBoxesData>(userId, null, skipServerLoading);
		}

		public static List<Models.BattlePass> BattlePasses { get; private set; }
		public static List<Models.MyGameEvent> MyGameEvents { get; private set; }
		public static class BalancyShop
		{
			public static List<Models.BalancyShop.GameSection> GameSections { get; private set; }
			public static List<Models.BalancyShop.BadgeInfo> BadgeInfos { get; private set; }
			public static List<Models.BalancyShop.UIStoreItem> UIStoreItems { get; private set; }
			public static List<Models.BalancyShop.MyOffer> MyOffers { get; private set; }
			public static List<Models.BalancyShop.MyStoreItem> MyStoreItems { get; private set; }
			public static List<Models.BalancyShop.MyItem> MyItems { get; private set; }

			public static void Init()
			{
				GameSections = DataManager.ParseList<Models.BalancyShop.GameSection>();
				BadgeInfos = DataManager.ParseList<Models.BalancyShop.BadgeInfo>();
				UIStoreItems = DataManager.ParseList<Models.BalancyShop.UIStoreItem>();
				MyOffers = DataManager.ParseList<Models.BalancyShop.MyOffer>();
				MyStoreItems = DataManager.ParseList<Models.BalancyShop.MyStoreItem>();
				MyItems = DataManager.ParseList<Models.BalancyShop.MyItem>();
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

		static partial void PrepareGeneratedData() {
			BattlePasses = DataManager.ParseList<Models.BattlePass>();
			ParseDictionary<Models.BattlePassPoint>();
			ParseDictionary<Models.BalancyShop.Badge>();
			ParseDictionary<Models.ContentHolder>();
			ParseDictionary<Models.BalancyShop.MyCustomSlot>();
			ParseDictionary<Models.LootBox.DropItem>();
			MyGameEvents = DataManager.ParseList<Models.MyGameEvent>();
			BalancyShop.Init();
			LootBox.Init();
			SmartStorage.SetLoadSmartObjectMethod(LoadSmartObject);
		}
	}
#pragma warning restore 649
}