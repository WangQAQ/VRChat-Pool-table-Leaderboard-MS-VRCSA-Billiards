using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using WangQAQ.ED;
namespace WangQAQ.PoolBuild
{
	public class EloBinder : IPlugInitializer
	{
		public bool Init()
		{
			var _cc20 = Component.FindFirstObjectByType<CC20>();
			var stringGUID = BuildToolLib.GetWorldGUID();
			var key = BuildToolLib.GetKey(stringGUID);
			
			if(key == null )
				return false;

			var _rankingObject = Component.FindObjectsOfType<RankingSystem>();

			foreach(var a in _rankingObject)
			{
				a._cc20 = _cc20;
				a.WorldGUID = stringGUID;
				a.Key = key;

				// 显式地记录变更
				PrefabUtility.RecordPrefabInstancePropertyModifications(a);
			}

			return true;
		}
	}
}

