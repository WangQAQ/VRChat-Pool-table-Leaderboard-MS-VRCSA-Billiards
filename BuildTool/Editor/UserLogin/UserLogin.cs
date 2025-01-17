using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;


namespace WangQAQ.PoolBuild
{
	public class UserLogin
	{
		#region Url

		const string UrlCheckLogin = "https://wangqaq.com/AspAPI/AuthorizationV2/LoginAPIV2/CheckLogin";
		const string UrlLogin = "https://wangqaq.com/AspAPI/AuthorizationV2/LoginAPIV2/Login";

		#endregion

		#region DataObject

		[System.Serializable]
		public class LoginRequest
		{
			public string userEmail;
			public string password;
		}

		#endregion

		#region LoginFunc

		#region CheckLogin

		public async Task<bool> CheckLoginAsync()
		{
			var jwt = BuildToolLib.GetJWT();

			if(jwt == null) 
				return false;

			using (var client = new HttpClient())
			{
				// 添加 Authorization 头部，格式为 Bearer <token>
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

				try
				{
					// 发送 GET 请求
					HttpResponseMessage response = await client.GetAsync(UrlCheckLogin);

					// 检查响应状态码
					if (response.IsSuccessStatusCode)
					{
						BuildToolLib.SetLoginState(true);

						return true;
					}
					else
					{
						BuildToolLib.SetJWT("");
						BuildToolLib.SetLoginState(false);

						return false;
					}
				}
				catch (Exception ex)
				{
					BuildToolLib.SetJWT("");
					BuildToolLib.SetLoginState(false);

					Debug.Log("Exception: " + ex.Message);
					return false;
				}
			}
		}

		#endregion

		#region Login

		public async Task<bool> LoginAsync(string email,string password)
		{
			var requestBody = new LoginRequest
			{
				userEmail = email,
				password = password
			};

			using (var client = new HttpClient())
			{
				try
				{
					// 将对象序列化为 JSON 字符串
					string jsonBody = JsonUtility.ToJson(requestBody);

					// 创建 HttpContent 对象，指定内容类型为 JSON
					var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

					// 创建 HttpRequestMessage
					var requestMessage = new HttpRequestMessage(HttpMethod.Post, UrlLogin)
					{
						Content = content
					};

					// 发送请求
					var response = await client.SendAsync(requestMessage);

					// 检查响应状态
					if (response.IsSuccessStatusCode)
					{
						string responseContent = await response.Content.ReadAsStringAsync();
						BuildToolLib.SetJWT(responseContent);
						BuildToolLib.SetLoginState(true);

						Debug.Log("Login Successful.");
						return true;
					}
					else
					{
						string responseContent = await response.Content.ReadAsStringAsync();
						BuildToolLib.SetJWT("");
						BuildToolLib.SetLoginState(false);

						Debug.Log($"Login Error: {response.StatusCode} - {responseContent}");
						return false; 
					}
				}
				catch (Exception ex)
				{
					BuildToolLib.SetJWT("");
					BuildToolLib.SetLoginState(false);

					Debug.Log("Exception: " + ex.Message);
					return false;
				}
			}
		}

		#endregion

		#endregion
	}
}
