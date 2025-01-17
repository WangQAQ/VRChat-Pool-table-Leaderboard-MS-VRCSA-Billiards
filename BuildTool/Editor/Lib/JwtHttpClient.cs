using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace WangQAQ.PoolBuild
{
	internal class JwtHttpClient
	{
		private readonly HttpClient _httpClient;

		public JwtHttpClient()
		{
			_httpClient = new HttpClient();
		}

		/// <summary>
		/// 发送带 JWT 和 Body 的Post请求 
		/// </summary>
		/// <param name="url">网址</param>
		/// <param name="jwtToken">JWT</param>
		/// <param name="requestBody">请求体（匿名类对象）</param>
		/// <returns></returns>
		public async Task<(string, bool)> SendPostRequestWithBodyAsync(string url,string jwtToken, object requestBody)
		{
			try
			{
				// 将对象序列化为 JSON 字符串
				string jsonBody = JsonUtility.ToJson(requestBody);

				// 创建 HttpContent 对象，指定内容类型为 JSON
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				// 创建 HttpRequestMessage
				var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
				{
					Content = content
				};

				// 添加 Authorization 头
				requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

				// 发送请求
				var response = await _httpClient.SendAsync(requestMessage);

				// 检查响应状态
				if (response.IsSuccessStatusCode)
				{
					Debug.Log("Upload Successful.");
					return ("Upload Successful.", true);
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					Debug.Log($"Upload Error: {response.StatusCode} - {responseContent}");
					return ($"Upload Error: {response.StatusCode} - {responseContent}", false);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
				return ("Exception: " + ex.Message, false);
			}
		}

		/// <summary>
		/// 发送带 JWT 和 Body 的Put请求 
		/// </summary>
		/// <param name="url">网址</param>
		/// <param name="jwtToken">JWT</param>
		/// <param name="requestBody">请求体（匿名类对象）</param>
		/// <returns></returns>
		public async Task<(string, bool)> SendPutRequestWithBodyAsync(string url, string jwtToken, object requestBody)
		{
			try
			{
				// 将对象序列化为 JSON 字符串
				string jsonBody = JsonUtility.ToJson(requestBody);

				// 创建 HttpContent 对象，指定内容类型为 JSON
				var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

				// 创建 HttpRequestMessage
				var requestMessage = new HttpRequestMessage(HttpMethod.Put, url)
				{
					Content = content
				};

				// 添加 Authorization 头
				requestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

				// 发送请求
				var response = await _httpClient.SendAsync(requestMessage);

				// 检查响应状态
				if (response.IsSuccessStatusCode)
				{
					Debug.Log("Upload Successful.");
					return ("Upload Successful.", true);
				}
				else
				{
					string responseContent = await response.Content.ReadAsStringAsync();
					Debug.Log($"Upload Error: {response.StatusCode} - {responseContent}");
					return ($"Upload Error: {response.StatusCode} - {responseContent}", false);
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Exception: " + ex.Message);
				return ("Exception: " + ex.Message, false);
			}
		}
	}
}
