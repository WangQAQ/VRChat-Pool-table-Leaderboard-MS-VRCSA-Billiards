using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRC.Core;

namespace WangQAQ.PoolBuild
{
	public static class BuildToolLib
	{
		#region Tool

		// 快速获取 WorldGUID
		public static string GetWorldGUID()
		{
			// 初始化对象
			var pipelineOBJ = Component.FindFirstObjectByType<PipelineManager>();

			// 获取世界GUID
			if (pipelineOBJ != null)
			{
				if (pipelineOBJ.GetType() == typeof(PipelineManager))
				{
					var tmp = pipelineOBJ.blueprintId.Split("_", StringSplitOptions.RemoveEmptyEntries);

					if (tmp.Length == 2)
						return tmp[1];
					else
						return Guid.Empty.ToString();
				}
				else
				{
					return Guid.Empty.ToString();
				}
			}
			else
			{
				return Guid.Empty.ToString();
			}
		}

		#endregion

		#region EditorPrefs

		public static string GetKey(string guid)
		{
			var key = EditorPrefs.GetString($"BuildToolWorldGUID-{guid}" , null);

			if (!string.IsNullOrEmpty(key))
				return key;

			return string.Empty;
		}

		public static void SetKey(string guid,string key)
		{
			EditorPrefs.SetString($"BuildToolWorldGUID-{guid}", key);
		}

		public static string GetJWT()
		{
			return EditorPrefs.GetString("BuildToolWorldLoginJwt", null);
		}

		public static void SetJWT(string jwt)
		{
			EditorPrefs.SetString("BuildToolWorldLoginJwt", jwt);
		}

		public static string GetTmpKey(string guid)
		{
			return EditorPrefs.GetString($"BuildToolWorldTmpKey-{guid}", null);
		}

		public static void SetTmpKey(string guid,string tmpKey)
		{
			EditorPrefs.SetString($"BuildToolWorldTmpKey-{guid}", tmpKey);
		}

		public static bool GetLoginState()
		{
			return EditorPrefs.GetBool("BuildToolWorldIsLogin", false); ;
		}

		public static void SetLoginState(bool state)
		{
			EditorPrefs.SetBool("BuildToolWorldIsLogin", state);
		}

		#endregion
	}
}