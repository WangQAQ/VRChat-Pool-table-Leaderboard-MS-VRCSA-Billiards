using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WangQAQ.PoolBuild
{
	public class MagnificationBinder : IPlugInitializer
	{
		public bool Init()
		{
			var _magnificationDownload = Component.FindFirstObjectByType<MagnificationDownload>();
			var _magnificationBoards = Component.FindObjectsOfType<Magnification>();

			if (_magnificationDownload == null ||
				_magnificationBoards == null)
				return false;

			foreach (var a in _magnificationBoards)
			{
				a._download = _magnificationDownload;

				PrefabUtility.RecordPrefabInstancePropertyModifications(a);
			}

			return true;
		}
	}

}
