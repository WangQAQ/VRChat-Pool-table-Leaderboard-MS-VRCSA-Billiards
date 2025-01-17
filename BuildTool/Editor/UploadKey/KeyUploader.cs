using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.Core;
using VRC.SDK3.Editor;
using VRC.SDKBase.Editor.Api;

namespace WangQAQ.PoolBuild
{
	public class KeyUploader
	{
		private static KeyHttpSender keyHttpSender = new KeyHttpSender();

		#region Upload & Update

		public async Task<(bool State, string)> UploadOrUpdateKeyAsync()
		{
			if (VRCSdkControlPanel.TryGetBuilder<IVRCSdkWorldBuilderApi>(out var builder))
			{
				var key = GenerateRandomKey(32);
				VRCWorld vrcWorldOBJ;

				/* 获取世界对象 */
				try
				{
					var pipelineOBJ = Component.FindObjectsOfType<PipelineManager>().Single();

					vrcWorldOBJ = await VRCApi.GetWorld(pipelineOBJ.blueprintId);

					if (vrcWorldOBJ.Name == null || vrcWorldOBJ.ID == null)
						return (false, "Get World Object Error");
				}
				catch
				{
					return (false, "No PipelineManager Find Error");
				}

				/* 获取世界名称 */
				var GUID = Guid.Parse(vrcWorldOBJ.ID.Split('_')[1]);
				var Name = vrcWorldOBJ.Name;

				/* 上传对象 */
				if (!await isMapExist(GUID))
				{
					/* 不存在地图 创建地图 */
					var data = await keyHttpSender.CreateWorldKey(Name, GUID, key);

					if (data.State)
					{
						/* 成功 */
						if (data.TmpKey != null)
						{
							/* 写入TmpKey */
							BuildToolLib.SetTmpKey(GUID.ToString(), data.TmpKey);
						}

						/* 写入主密钥 */
						BuildToolLib.SetKey(GUID.ToString(), key);

						return (true, "Upload Key Done");
					}
					else
					{
						/* 失败 */
						return (false, data.Message);
					}
				}
				else
				{
					/* 存在地图 更新地图 */
					var data = await keyHttpSender.UpdateWorldKey(Name, GUID, key);

					if (data.State)
					{
						/* 写入主密钥 */
						BuildToolLib.SetKey(GUID.ToString(), key);

						/* 成功 */
						return (true, "Update Key Done");
					}
					else
					{
						/* 失败 */
						return (false, data.Message);
					}
				}
			}
			else
			{
				return (false, "Unknown SDK Error");
			}
		}

		#endregion

		#region Func

		/// <summary>
		/// 查找地图是否存在
		/// </summary>
		/// <param name="mapGUID"></param>
		private async Task<bool> isMapExist(Guid mapGUID)
		{
			return await keyHttpSender.IsMapExist(mapGUID);
		}

		public static string GenerateRandomKey(int length)
		{
			// 每个字符可以表示为 4 位（二进制）或者 8 位（ASCII），我们用 Base64 编码
			byte[] randomBytes = new byte[length];
			using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomBytes);
			}

			// 使用 Base64 编码，使结果更加可读
			return Convert.ToBase64String(randomBytes).Substring(0, length);
		}

		#endregion
	}
}
