
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRC.Udon;

public class LeaderboardV2 : ILeaderboard
{
	#region EditTimeVariable

	[Header("UI Object")]
	[Tooltip("UI 分数榜")]
	[SerializeField] private Text ScoreText;
	[SerializeField] private Text NameText;

	/* 玩家个人等级排行 */
	[SerializeField] private Text PlayerRateText;

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
		if (_eloDownload._eloNameList == null ||
			_eloDownload._eloNameList.Count == 0 ||
			_eloDownload._eloDataList == null ||
			_eloDownload._eloDataList.Count == 0)
		{
			NameText.text = loadingString;
			ScoreText.text = loadingString;
			return;
		}

		// 格式化字符串
		string leaderBoardNameString = "";
		string leaderBoardScoreString = "";
		string leaderBoardPlayerRate = "# NotFind";

		DataList names = _eloDownload._eloNameList;
		DataList scores = _eloDownload._eloDataList;

		for (int i = 0; i < names.Count; i++)
		{
			var nameTmp = names[i].ToString().Replace(" ", " ");
			// 转码，去除小数点，格式化，替换空格 \u0020 到 \u00A0 ,裁剪长度
			leaderBoardNameString +=
				(i + 1).ToString()
				+ "."
				+ (nameTmp.Length > stringLen ? nameTmp.Substring(0, stringLen) : nameTmp)
				+ "\n";

			leaderBoardScoreString += scores[i].String.Split("#")[0] + "\n";

			/* 尝试查找本地玩家 */
			if (nameTmp == Networking.LocalPlayer.displayName)
			{
				leaderBoardPlayerRate = $"# {(i + 1).ToString()}";
			}
		}

		// Loading String
		NameText.text = leaderBoardNameString;
		ScoreText.text = leaderBoardScoreString;
		PlayerRateText.text = leaderBoardPlayerRate;
	}

	#endregion

}
