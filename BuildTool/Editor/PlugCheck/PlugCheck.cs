using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WangQAQ.PoolBuild
{
	public static class PlugCheck
	{
		public static (bool isDone, string Message) Check()
		{
			/* �����Ƿ����ҽ���һ�� TableHook */
			var tableHookDownloads = Component.FindObjectsOfType<TableHook>();
			if (tableHookDownloads == null || tableHookDownloads.Count() != 1)
				return (false, "Too much or too little TableHook");

			/* �����Ƿ����ҽ���1�� Elo Download */
			var eloDownloads = Component.FindObjectsOfType<EloDownload>();
			if (eloDownloads == null || eloDownloads.Count() != 1)
				return (false, "Too much or too little EloDownload");

			return (true, "Done");
		}
	}
}
