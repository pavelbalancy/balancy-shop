using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Profile : ParentBaseData
	{

		[JsonProperty]
		private Data.Characters characters;


		[JsonIgnore]
		public Data.Characters Characters => characters;

		protected override void InitParams() {
			base.InitParams();

			ValidateData(ref characters);
		}

		public static Profile Instantiate()
		{
			return Instantiate<Profile>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
			AddCachedItem(path + "Characters", Characters, null, cache);
		}
	}
#pragma warning restore 649
}