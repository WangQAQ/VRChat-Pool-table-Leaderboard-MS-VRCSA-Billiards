/*
 *  MIT License
 *  Copyright (c) 2024 WangQAQ
 *
 *  计分器排行榜自动绑定系统
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace WangQAQ.PoolBuild
{
	public class ScoreBinder : IPlugInitializer
	{
		public bool Init()
		{
			/* 初始化计分器 */
			var scoreManagerV4 = Component.FindObjectsOfType<ScoreManagerV4>();
			var eloApi = Component.FindFirstObjectByType<EloDownload>();

			if (eloApi == null || scoreManagerV4 == null)
				return false;

			foreach( var scoreManager in scoreManagerV4)
			{
				scoreManager.EloAPI = eloApi;

				// 显式地记录变更
				PrefabUtility.RecordPrefabInstancePropertyModifications(scoreManager);
			}

			return true;
		}
	}
}
#endif