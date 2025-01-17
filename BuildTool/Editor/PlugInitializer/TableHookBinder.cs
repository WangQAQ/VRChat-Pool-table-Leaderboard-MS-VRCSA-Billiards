using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using WangQAQ.PoolBuild;

namespace WangQAQ.PoolBuild
{
	public class TableHookBinder : IPlugInitializer
	{
		public bool Init()
		{
			var _billiardsModule = Component.FindObjectsOfType<BilliardsModule>();
			var _tableHook = Component.FindFirstObjectByType<TableHook>();

			if (_tableHook == null ||
				_tableHook == null)
				return false;

			foreach (var a in _billiardsModule)
			{
				a.tableHook = _tableHook;

				PrefabUtility.RecordPrefabInstancePropertyModifications(a);
			}

			return true;
		}
	}
}
