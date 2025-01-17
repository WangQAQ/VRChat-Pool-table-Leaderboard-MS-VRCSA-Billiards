using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase;
using WangQAQ.UdonPlug;

namespace WangQAQ.PoolBuild
{
	public class TagBinder : IPlugInitializer
	{
		public bool Init()
		{
			var _imageDownload = Component.FindObjectsOfType<ImageDownload>();

			if (_imageDownload == null)
				return false;

			foreach (var a in _imageDownload)
			{
				a.ImageUrlArray = new VRCUrl[100];

				for (int i = 0; i < 100; i++)
				{
					a.ImageUrlArray[i] = new VRCUrl($"https://oss.wangqaq.com/pool-tag-img/{i}");
				}

				PrefabUtility.RecordPrefabInstancePropertyModifications(a);
			}

			return true;
		}
	}

}
