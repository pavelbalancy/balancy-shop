#if UNITY_EDITOR && !BALANCY_SERVER
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Balancy
{
    public class EditorUtils
    {
        public static List<Texture> GetAllTexturesFromMaterial(Renderer renderer)
        {
            List<Texture> allTexture = new List<Texture>();
            Shader shader = renderer.sharedMaterial.shader;
            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                {
                    Texture texture = renderer.sharedMaterial.GetTexture(ShaderUtil.GetPropertyName(shader, i));
                    allTexture.Add(texture);
                }
            }

            return allTexture;
        }

        public class ServerRequest : UnityUtils.IUnityRequestInfo
        {
            public string Url { get; }
            public Dictionary<string, string> Headers { get; }
            public Dictionary<string, object> Body { get; }
            public bool IsMultipart { get; set; }
            public List<UnityUtils.ImageInfo> Images { get; private set;}

            public string Method;

            public string GetMethod()
            {
                return Method;
            }
            
            public ServerRequest(string fullUrl, bool isPost)
            {
                Url = fullUrl;
                Headers = new Dictionary<string, string>();
                Body = isPost ? new Dictionary<string, object>() : null;
                Method = isPost ? "POST" : "GET";
            }

            public ServerRequest(string url, string method = "POST")
            {
                Url = Constants.GeneralConstants.ADMINKA_API_URL + url;
                Headers = new Dictionary<string, string>();
                Body = method == "POST" ? new Dictionary<string, object>() : null;
                Method = method;
            }

            public ServerRequest SetHeader(string key, string value)
            {
                if (Headers.ContainsKey(key))
                    Headers[key] = value;
                else
                    Headers.Add(key, value);
                return this;
            }
            
            public ServerRequest AddBody(string key, object value)
            {
                Body.Add(key, value);
                return this;
            }

            public ServerRequest AddTexture(Texture2D img, string name, string prefabName)
            {
                if (Images == null)
                    Images = new List<UnityUtils.ImageInfo>();
                Images.Add(new UnityUtils.ImageInfo(img, name, prefabName));

                return this;
            }
            
            public ServerRequest SetMultipart()
            {
                IsMultipart = true;

                return this;
            }
        }
    }
}
#endif