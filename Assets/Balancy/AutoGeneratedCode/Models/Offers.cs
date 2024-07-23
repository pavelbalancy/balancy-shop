using Newtonsoft.Json;
using System;

namespace Balancy.Data
{
#pragma warning disable 649

	public class Offers : BaseData
	{



		protected override void InitParams() {
			base.InitParams();

		}

		public static Offers Instantiate()
		{
			return Instantiate<Offers>();
		}

		protected override void AddAllParamsToCache(string path, IInternalStorageCache cache)
		{
			base.AddAllParamsToCache(path, cache);
		}
	}
#pragma warning restore 649
}