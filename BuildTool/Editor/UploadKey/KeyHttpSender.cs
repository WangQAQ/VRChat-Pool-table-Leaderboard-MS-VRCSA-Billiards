using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WangQAQ.PoolBuild
{
	public class KeyHttpSender
	{
		#region Config

		const string KeyUrl = "https://wangqaq.com/AspAPI/table/UploadMapKey";

		#endregion

		#region DataObject

		[System.Serializable]
		public class PostMap
		{
			public string Name;
			public string WorldGUID;
			public string Key;
		}

		[System.Serializable]
		public class PutMap
		{
			public string Name;
			public string WorldGUID;
			public string Key;
			public string TMPKey;
			public bool VerificationMethod;
		}

		#endregion

		#region Get
		/// <summary>
		/// 地图 - Key 是否存在于数据库
		/// 0 不存在
		/// 1 存在
		/// </summary>
		/// <param name="worldGUID"></param>
		/// <returns></returns>
		public async Task<bool> IsMapExist(Guid worldGUID)
		{
			var httpClient = new HttpClient();

			var response = await httpClient.GetAsync($"{KeyUrl}/{worldGUID.ToString()}");

			if (!response.IsSuccessStatusCode)
				return false;

			return true;
		}

		#endregion

		#region Create
		/// <summary>
		/// 创建地图ID
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="worldGUID">世界GUID</param>
		/// <param name="key">Key</param>
		/// <returns></returns>
		public async Task<(string Message, bool State, string TmpKey)> CreateWorldKey(string name, Guid worldGUID, string key)
		{
			//EditorPrefs.GetString($"BuildToolWorldGUID-{worldGUID.ToString()}", Guid.NewGuid().ToString());

			if (BuildToolLib.GetLoginState())
			{
				/* 已登录，查找JWT */
				var jwt = BuildToolLib.GetJWT();

				/* 无JWT */
				if (jwt == null)
					return ("Internal errors", false, null);

				/* 构建请求体 */
				var requestBody = new PostMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key
				};

				/* 发送HTTP请求 */
				var client = new JwtHttpClient();

				var data = await client.SendPostRequestWithBodyAsync(KeyUrl, jwt, requestBody);
				return (data.Item1, data.Item2, null);
			}
			else
			{
				/* 使用TMPKey */
				var requestBody = new PostMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key
				};

				// 将请求体对象序列化为 JSON 字符串
				var jsonBody = JsonUtility.ToJson(requestBody);

				// 创建 StringContent 对象，指定内容和内容类型
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var client = new HttpClient();
				var response = await client.PostAsync(KeyUrl, content);

				if (response.IsSuccessStatusCode)
				{
					// 读取TmpKey
					string tmpKey = await response.Content.ReadAsStringAsync();
					return ("Upload Done.", true, tmpKey);
				}
				else
				{
					// 读取错误信息
					string responseBody = await response.Content.ReadAsStringAsync();
					return ($"Upload Error: {response.StatusCode} - {responseBody}", false, null);
				}
			}
		}
		#endregion

		#region Update
		/// <summary>
		/// 更新地图ID
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="worldGUID">世界GUID</param>
		/// <param name="key">Key</param>
		/// <returns></returns>
		public async Task<(string Message, bool State)> UpdateWorldKey(string name, Guid worldGUID, string key)
		{

			if (BuildToolLib.GetLoginState())
			{
				/* 已登录，查找JWT */
				var jwt = BuildToolLib.GetJWT();

				/* 无JWT */
				if (jwt == null)
					return ("Not Find JWT Please log in again and try again", false);

				var tmpKey = BuildToolLib.GetTmpKey(worldGUID.ToString());

				/* 构建请求体 */
				var requestBody = new PutMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key,
					TMPKey = tmpKey,
					VerificationMethod = true
				};

				/* 发送请求 */
				var client = new JwtHttpClient();
				return await client.SendPutRequestWithBodyAsync($"{KeyUrl}/{worldGUID.ToString()}", jwt, requestBody);
			}
			else
			{
				/* 未登陆 */
				/* 使用TMPKey */

				var tmpKey = BuildToolLib.GetTmpKey(worldGUID.ToString());

				if (tmpKey == null)
					return ("No Tmp Key Error", false);

				var requestBody = new PutMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key,
					TMPKey = tmpKey,
					VerificationMethod = false
				};

				// 将请求体对象序列化为 JSON 字符串
				var jsonBody = JsonUtility.ToJson(requestBody);

				// 创建 StringContent 对象，指定内容和内容类型
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var client = new HttpClient();
				var response = await client.PutAsync($"{KeyUrl}/{worldGUID.ToString()}", content);

				if (response.IsSuccessStatusCode)
				{
					return ("Update Done", true);
				}
				else
				{
					// 读取错误信息
					string responseBody = await response.Content.ReadAsStringAsync();
					return ($"Update Error: {response.StatusCode} - {responseBody}", false);
				}
			}
		}
		#endregion
	}
}

