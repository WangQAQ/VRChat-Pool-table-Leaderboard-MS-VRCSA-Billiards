
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class IEloDownload : UdonSharpBehaviour
{
	#region PublicAPI

	virtual public string GetEloV2(string name)
	{
		/* Nop */
		return null;
	}

	#endregion

	#region Data

	// Elo排序列表

	/* 名称列表 */
	[HideInInspector] public DataList _eloNameList;
	/* 数据列表 */
	[HideInInspector] public DataList _eloDataList;

	// Elo字典(非顺序)
	[HideInInspector] public DataDictionary _eloDictionary;

	#endregion
}
