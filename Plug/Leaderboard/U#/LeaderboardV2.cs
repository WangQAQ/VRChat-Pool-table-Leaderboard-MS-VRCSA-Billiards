﻿
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LeaderboardV2 : ILeaderboard
{
	#region EditTimeVariable

	[Header("UGUI Object")]
	[Tooltip("UGUI 分数榜")]
	[SerializeField] private TextMeshProUGUI TmpUGUI;

	[Space(10)]
	[Header("Text Setting")]
	[Tooltip("ELO Text Len")]
	[SerializeField] private int stringLen = 10;

	[Tooltip("下载成功时字符串")]
	[TextArea]
	[SerializeField] private string loadingString = "- <size=50>Loading_Failed</size> -<br>- <size=120>QAQ</size> -<br>- <size=50>Check_Your_Internet</size> -";

	#endregion

	#region PrivateVariable

	private IEloDownload _eloDownload;

	#endregion

	#region PublicApi

	public override void _Init(IEloDownload eloDownload)
	{
		_eloDownload = eloDownload;
	}

	public override void Refresh()
	{
		loadString();
	}

	#endregion

	#region PrivateFunc

	/* 序列化排行榜字符串 */
	private void loadString()
	{
		if (_eloDownload._eloNameList == null	|| 
			_eloDownload._eloNameList.Count == 0||
			_eloDownload._eloDataList == null   ||
			_eloDownload._eloDataList.Count == 0)
		{
			TmpUGUI.text = loadingString;
			return;
		}

		// 格式化字符串
		string leaderBoardString = "";

		DataList names = _eloDownload._eloNameList;
		DataList scores = _eloDownload._eloDataList;

		for (int i = 0; i < names.Count; i++)
		{
			var nameTmp = names[i].ToString().Replace(" ", " ");
			// 转码，去除小数点，格式化，替换空格 \u0020 到 \u00A0 ,裁剪长度
			leaderBoardString +=
				(i + 1).ToString() + "."
				+ (nameTmp.Length > stringLen ? nameTmp.Substring(0, stringLen) : nameTmp)
				+ " "
				+ $"<color=#FFD700>{scores[i].ToString()}</color>"
				+ "\n";
		}

		// Loading String
		TmpUGUI.text = leaderBoardString;
	}

	#endregion

}