using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Counter : BaseData
	{

		[JsonProperty]
		private int epicChestGuaranteedDrop;


		[JsonIgnore]
		public int EpicChestGuaranteedDrop
		{
			get => epicChestGuaranteedDrop;
			set {
				if (UpdateValue(ref epicChestGuaranteedDrop, value))
					_cache?.UpdateStorageValue(_path + "EpicChestGuaranteedDrop", epicChestGuaranteedDrop);
			}
		}

		protected override void InitParams() {
			base.InitParams();

		}

		public static Counter Instantiate()
		{
			return Instantiate<Counter>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "EpicChestGuaranteedDrop", epicChestGuaranteedDrop, newValue => EpicChestGuaranteedDrop = Utils.ToInt(newValue), cache);
		}
	}
#pragma warning restore 649
}