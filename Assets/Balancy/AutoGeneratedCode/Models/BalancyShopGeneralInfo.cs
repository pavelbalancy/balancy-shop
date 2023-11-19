using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class BalancyShopGeneralInfo : BaseData
	{

		[JsonProperty]
		private bool shopOpened;


		[JsonIgnore]
		public bool ShopOpened
		{
			get => shopOpened;
			set {
				if (UpdateValue(ref shopOpened, value))
					_cache?.UpdateStorageValue(_path + "ShopOpened", shopOpened);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static BalancyShopGeneralInfo Instantiate()
		{
			return Instantiate<BalancyShopGeneralInfo>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "ShopOpened", shopOpened, newValue => ShopOpened = (bool)newValue, cache);
		}
	}
#pragma warning restore 649
}