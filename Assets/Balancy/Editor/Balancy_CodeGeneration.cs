#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using Balancy.Dictionaries;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Balancy.Editor
{
	public static class Balancy_CodeGeneration
	{
		private const string ADMINKA_GENERATOR = Constants.GeneralConstants.ADMINKA_GENERATOR;
		
#pragma warning disable 649
		[Serializable]
		private class GeneratedClass
		{
			public string className;
			public string classCode;
			public string relativePath;
		}

		[Serializable]
		private class GeneratedCode : ResponseData
		{
			public GeneratedClass[] list;
		}
#pragma warning restore 649

		private static Loader m_Loader;
		private static IEnumerator m_Coroutine;
		
		private static void SendRequestToServer(string gameId, string token, Constants.Environment env, Action<string, string> callback)
		{
			var helper = EditorCoroutineHelper.Create();
			var req = new EditorUtils.ServerRequest($"{ADMINKA_GENERATOR}/adminka/v1.8/generate?game_id={gameId}&env={(int) env}&version=1", false);
			req.SetHeader("Content-Type", "application/json")
				.SetHeader("Authorization", "Bearer " + token);
            
			var cor = UnityUtils.SendRequest(req, request =>
			{
#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
				if (request.isNetworkError || request.isHttpError)
#endif
				{
					callback?.Invoke(null, request.error);
					EditorUtility.DisplayDialog("Error", "Failed " + req.Url + " with error " + request.error, "Ok");
				}
				else
				{
					callback?.Invoke(request.downloadHandler.text, null);
				}
			});
			helper.LaunchCoroutine(cor);
		}
		
		public static void StartGeneration(string gameId, string token, Constants.Environment env, Action onComplete, string savePath)
		{
			SendRequestToServer(gameId, token, env, (data, error) =>
			{
				AssetDatabase.Refresh();
				if (!string.IsNullOrEmpty(error))
				{
					EditorUtility.DisplayDialog("Error", error, "Ok");
					onComplete?.Invoke();
				}
				else
				{
					var response = JsonConvert.DeserializeObject<GeneratedCode>(data);
					if (response?.Success ?? false)
					{
						ParseResponse(response, savePath);
						onComplete?.Invoke();
					}
					else
					{
						EditorUtility.DisplayDialog("Error", response?.Error?.Message, "Ok");
						onComplete();
					}
				}
			});
		}

		private static void ParseResponse(GeneratedCode code, string savePath)
		{
			if (Directory.Exists(savePath))
				Directory.Delete(savePath, true);

			Directory.CreateDirectory(savePath);

			SaveFiles(code, savePath);

			AssetDatabase.Refresh();
		}

		private static void SaveFiles(GeneratedCode code, string savePath)
		{
			foreach (var cl in code.list)
			{
				var folderPath = savePath + cl.relativePath;
				if (!string.IsNullOrEmpty(cl.relativePath))
				{
					if (!Directory.Exists(folderPath))
						Directory.CreateDirectory(folderPath);
				}

				var path = $"{folderPath}/{cl.className}.cs";
				using (StreamWriter sw = File.CreateText(path))
				{
					sw.Write(cl.classCode);
					sw.Close();
				}
			}
		}
	}
}
#endif