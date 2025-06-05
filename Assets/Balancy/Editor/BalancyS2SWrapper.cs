#if UNITY_EDITOR && !BALANCY_SERVER
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Balancy.Editor
{
    public class BalancyS2SWrapper
    {
        private const string BASE_URL = "https://s2s.balancy.dev";
        
        private string _gameId;
        private string _privateKey;

        public BalancyS2SWrapper(string gameId, string privateKey)
        {
            _gameId = gameId;
            _privateKey = privateKey;
        }

        /// <summary>
        /// Creates a request with properly formed signature for Balancy s2s API
        /// </summary>
        /// <param name="endpoint">API endpoint, e.g. "/v1/games/{game_id}/branches/{branch_name}/templates/{template_name}/entities"</param>
        /// <param name="method">HTTP method (GET, POST, DELETE, etc.)</param>
        /// <param name="branchName">Branch name (if required)</param>
        /// <returns>ServerRequest object ready to use</returns>
        public EditorUtils.ServerRequest CreateRequest(string endpoint, string method = "GET", string branchName = null)
        {
            // Form the full URL, replacing placeholders
            string fullUrl = BASE_URL + endpoint
                .Replace("{game_id}", _gameId)
                .Replace("{branch_name}", branchName ?? "main");

            bool isPost = method == "POST" || method == "PUT" || method == "PATCH";
            var request = new EditorUtils.ServerRequest(fullUrl, isPost)
            {
                Method = method
            };

            // Add Content-Type for POST/PUT requests
            if (isPost)
            {
                if (method == "PUT")
                {
                    // request.SetHeader("Content-Type", "multipart/form-data");
                } else
                    request.SetHeader("Content-Type", "application/json");
            }

            return request;
        }

        /// <summary>
        /// Signs the request by adding required headers for Balancy authentication
        /// </summary>
        /// <param name="request">Request to sign</param>
        /// <returns>The same request with added headers</returns>
        public EditorUtils.ServerRequest SignRequest(EditorUtils.ServerRequest request)
        {
            // Generate timestamp (Unix timestamp in milliseconds)
            long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            
            // Generate nonce (32 characters)
            string nonce = GenerateNonce(32);
            
            // Form JSON data for signature
            string jsonData = GetJsonDataForSignature(request);
            
            // Calculate signature
            string signature = CalculateSignature(timestamp, nonce, _gameId, jsonData);
            
            // Add headers
            request.SetHeader("Balancy-Signature", signature);
            request.SetHeader("Balancy-Timestamp", timestamp.ToString());
            request.SetHeader("Balancy-Nonce", nonce);
            
            return request;
        }

        /// <summary>
        /// Sends a signed request to the server
        /// </summary>
        /// <param name="request">Prepared and signed request</param>
        /// <param name="callback">Callback to be called after request completion</param>
        public void SendRequest(EditorUtils.ServerRequest request, Action<UnityWebRequest> callback)
        {
            var helper = EditorCoroutineHelper.Create();
            var signedRequest = SignRequest(request);
            
            helper.LaunchCoroutine(UnityUtils.SendRequest(signedRequest, callback));
        }

        #region Helper Methods

        /// <summary>
        /// Generates a random string of specified length
        /// </summary>
        private string GenerateNonce(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new System.Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Returns data for signature depending on request type
        /// </summary>
        private string GetJsonDataForSignature(EditorUtils.ServerRequest request)
        {
            if (request.Method == "POST")
            {
                // For POST/PUT/PATCH/DELETE, use request body
                return JsonConvert.SerializeObject(request.Body ?? new Dictionary<string, object>());
            }
            else
            {
                // For GET requests, use an empty object {}
                return "{}";
            }
        }

        /// <summary>
        /// Calculates HMAC-SHA256 signature according to Balancy specification
        /// </summary>
        private string CalculateSignature(long timestamp, string nonce, string gameId, string jsonData)
        {
            // Form the string to sign
            string dataToSign = $"{timestamp}\n{nonce}\n{gameId}\n{jsonData}";
            
            // Calculate HMAC-SHA256
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_privateKey)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                
                // Convert bytes to string in hex format
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Checks request status by its ID
        /// </summary>
        public IEnumerator CheckRequestStatus(string requestId, Action<string, Dictionary<string, object>> callback)
        {
            var statusRequest = CreateRequest($"/v1/games/{_gameId}/requests/{requestId}", "GET");
            bool completed = false;
            Dictionary<string, object> result = null;
            string status = null;
            
            SendRequest(statusRequest, request => 
            {
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var response = JsonConvert.DeserializeObject<Dictionary<string, object>>(request.downloadHandler.text);
                    status = response["status"].ToString();
                    result = response;
                }
                else
                {
                    status = "failed";
                    result = new Dictionary<string, object> { { "error", request.error } };
                }
                completed = true;
            });
            
            // Wait for request completion
            while (!completed)
                yield return null;
                
            callback?.Invoke(status, result);
        }

        #endregion

        #region API Methods

        /// <summary>
        /// Adds or updates template entities
        /// </summary>
        public EditorUtils.ServerRequest AddOrUpdateTemplateEntities(string branchName, string templateName, string primaryKey, List<Dictionary<string, object>> entities, Dictionary<string, string> primaryKeys = null)
        {
            var request = CreateRequest($"/v1/games/{_gameId}/branches/{branchName}/templates/{templateName}/entities", "POST");
            
            var settings = new Dictionary<string, object>
            {
                { "primary_key", primaryKey }
            };
            
            if (primaryKeys != null && primaryKeys.Count > 0)
            {
                settings.Add("primary_keys", primaryKeys);
            }
            
            var body = new Dictionary<string, object>
            {
                { "settings", settings },
                { "entities", entities }
            };
            
            foreach (var entry in body)
            {
                request.AddBody(entry.Key, entry.Value);
            }
            
            return request;
        }

        /// <summary>
        /// Deletes template entities
        /// </summary>
        public EditorUtils.ServerRequest DeleteTemplateEntities(string branchName, string templateName, string primaryKey, List<string> values)
        {
            var request = CreateRequest($"/v1/games/{_gameId}/branches/{branchName}/templates/{templateName}/entities", "DELETE");
            
            request.AddBody("settings", new Dictionary<string, object> { { "primary_key", primaryKey } });
            request.AddBody("values", values);
            
            return request;
        }

        /// <summary>
        /// Gets template entity by primary key
        /// </summary>
        public EditorUtils.ServerRequest GetTemplateEntityByPrimaryKey(string branchName, string templateName, string primaryKey, string primaryValue, Dictionary<string, string> primaryKeys = null)
        {
            string endpoint = $"/v1/games/{_gameId}/branches/{branchName}/templates/{templateName}/entities/{primaryValue}?primary_key={primaryKey}";
            
            // Add additional primary_keys if they exist
            if (primaryKeys != null && primaryKeys.Count > 0)
            {
                foreach (var pair in primaryKeys)
                {
                    endpoint += $"&primary_keys[{pair.Key}]={pair.Value}";
                }
            }
            
            return CreateRequest(endpoint, "GET");
        }

        #endregion
    }
}
#endif