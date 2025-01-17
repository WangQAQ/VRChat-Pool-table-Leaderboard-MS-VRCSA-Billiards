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
		/// ��ͼ - Key �Ƿ���������ݿ�
		/// 0 ������
		/// 1 ����
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
		/// ������ͼID
		/// </summary>
		/// <param name="name">����</param>
		/// <param name="worldGUID">����GUID</param>
		/// <param name="key">Key</param>
		/// <returns></returns>
		public async Task<(string Message, bool State, string TmpKey)> CreateWorldKey(string name, Guid worldGUID, string key)
		{
			//EditorPrefs.GetString($"BuildToolWorldGUID-{worldGUID.ToString()}", Guid.NewGuid().ToString());

			if (BuildToolLib.GetLoginState())
			{
				/* �ѵ�¼������JWT */
				var jwt = BuildToolLib.GetJWT();

				/* ��JWT */
				if (jwt == null)
					return ("Internal errors", false, null);

				/* ���������� */
				var requestBody = new PostMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key
				};

				/* ����HTTP���� */
				var client = new JwtHttpClient();

				var data = await client.SendPostRequestWithBodyAsync(KeyUrl, jwt, requestBody);
				return (data.Item1, data.Item2, null);
			}
			else
			{
				/* ʹ��TMPKey */
				var requestBody = new PostMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key
				};

				// ��������������л�Ϊ JSON �ַ���
				var jsonBody = JsonUtility.ToJson(requestBody);

				// ���� StringContent ����ָ�����ݺ���������
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var client = new HttpClient();
				var response = await client.PostAsync(KeyUrl, content);

				if (response.IsSuccessStatusCode)
				{
					// ��ȡTmpKey
					string tmpKey = await response.Content.ReadAsStringAsync();
					return ("Upload Done.", true, tmpKey);
				}
				else
				{
					// ��ȡ������Ϣ
					string responseBody = await response.Content.ReadAsStringAsync();
					return ($"Upload Error: {response.StatusCode} - {responseBody}", false, null);
				}
			}
		}
		#endregion

		#region Update
		/// <summary>
		/// ���µ�ͼID
		/// </summary>
		/// <param name="name">����</param>
		/// <param name="worldGUID">����GUID</param>
		/// <param name="key">Key</param>
		/// <returns></returns>
		public async Task<(string Message, bool State)> UpdateWorldKey(string name, Guid worldGUID, string key)
		{

			if (BuildToolLib.GetLoginState())
			{
				/* �ѵ�¼������JWT */
				var jwt = BuildToolLib.GetJWT();

				/* ��JWT */
				if (jwt == null)
					return ("Not Find JWT Please log in again and try again", false);

				var tmpKey = BuildToolLib.GetTmpKey(worldGUID.ToString());

				/* ���������� */
				var requestBody = new PutMap
				{
					Name = name,
					WorldGUID = worldGUID.ToString(),
					Key = key,
					TMPKey = tmpKey,
					VerificationMethod = true
				};

				/* �������� */
				var client = new JwtHttpClient();
				return await client.SendPutRequestWithBodyAsync($"{KeyUrl}/{worldGUID.ToString()}", jwt, requestBody);
			}
			else
			{
				/* δ��½ */
				/* ʹ��TMPKey */

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

				// ��������������л�Ϊ JSON �ַ���
				var jsonBody = JsonUtility.ToJson(requestBody);

				// ���� StringContent ����ָ�����ݺ���������
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				var client = new HttpClient();
				var response = await client.PutAsync($"{KeyUrl}/{worldGUID.ToString()}", content);

				if (response.IsSuccessStatusCode)
				{
					return ("Update Done", true);
				}
				else
				{
					// ��ȡ������Ϣ
					string responseBody = await response.Content.ReadAsStringAsync();
					return ($"Update Error: {response.StatusCode} - {responseBody}", false);
				}
			}
		}
		#endregion
	}
}

