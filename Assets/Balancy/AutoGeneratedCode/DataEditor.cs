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

		public static class BalancyShop
		{
			public static List<Models.BalancyShop.GameSection> GameSections { get; private set; }
			public static List<Models.BalancyShop.MyItem> MyItems { get; private set; }
			public static List<Models.BalancyShop.UIStoreItem> UIStoreItems { get; private set; }
			public static List<Models.BalancyShop.BadgeInfo> BadgeInfos { get; private set; }
			public static List<Models.BalancyShop.MyOffer> MyOffers { get; private set; }
			public static List<Models.BalancyShop.MyStoreItem> MyStoreItems { get; private set; }

			public static void Init()
			{
				GameSections = DataManager.ParseList<Models.BalancyShop.GameSection>();
				MyItems = DataManager.ParseList<Models.BalancyShop.MyItem>();
				UIStoreItems = DataManager.ParseList<Models.BalancyShop.UIStoreItem>();
				BadgeInfos = DataManager.ParseList<Models.BalancyShop.BadgeInfo>();
				MyOffers = DataManager.ParseList<Models.BalancyShop.MyOffer>();
				MyStoreItems = DataManager.ParseList<Models.BalancyShop.MyStoreItem>();
			}
		}

		static partial void PrepareGeneratedData() {
			ParseDictionary<Models.BalancyShop.MyCustomSlot>();
			ParseDictionary<Models.BalancyShop.Badge>();
			BalancyShop.Init();
			SmartStorage.SetLoadSmartObjectMethod(LoadSmartObject);
		}
	}
#pragma warning restore 649
}