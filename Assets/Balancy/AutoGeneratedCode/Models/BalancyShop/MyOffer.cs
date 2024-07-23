using Newtonsoft.Json;
using System;

namespace Balancy.Models.BalancyShop
{
#pragma warning disable 649

	public class MyOffer : SmartObjects.GameOffer
	{

		[JsonProperty]
		private string unnyIdUIStoreSlotData;
		[JsonProperty]
		private string unnyIdUIPopupData;


		[JsonIgnore]
		public Models.BalancyShop.UIStoreItem UIStoreSlotData => DataEditor.GetModelByUnnyId<Models.BalancyShop.UIStoreItem>(unnyIdUIStoreSlotData);

		[JsonProperty("badge")]
		public readonly Models.LiveOps.Store.SlotType Badge;

		[JsonIgnore]
		public Models.BalancyShop.UIStoreItem UIPopupData => DataEditor.GetModelByUnnyId<Models.BalancyShop.UIStoreItem>(unnyIdUIPopupData);

	}
#pragma warning restore 649
}