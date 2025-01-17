/*
 *  MIT License
 *  Copyright (c) 2024 WangQAQ
 *
 *  Elo 下载器 Json格式
 */
/*
 * 
 * API格式
 * {
 *     "scores": {
 *        "name" : "分数#胜率#总回合数",
 *         ...
 *     }
 *  }
 * 
 */
using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

public class EloDownload : IEloDownload
{
    // Elo下载URL
    [Header("URL")]
    [SerializeField] public VRCUrl url;
	public ILeaderboard[] _leaderboards;

	[HideInInspector] public int ReloadSecond = 60;

	public void Start()
    {
        VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);

        foreach (var leaderboard in _leaderboards)
        {
            if (leaderboard != null)
            {
                leaderboard._Init(this);
            }
        }
	}

    #region URL

    // 字符串下载成功回调
    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        if (VRCJson.TryDeserializeFromJson(result.Result, out var json))
        {
			/* 保存排序和非排序字典对象 */
			_eloDictionary = json.DataDictionary;

			_eloNameList = _eloDictionary.GetKeys();
			_eloDataList = _eloDictionary.GetValues();

			for (var i = 0; i < _leaderboards.Length ;i++ )
            {
                /* 延迟加载，防止 low 帧 */
                _leaderboards[i].SendCustomEventDelayedFrames(nameof(ILeaderboard.Refresh),i * 10);
			}
		}
        SendCustomEventDelayedSeconds("_AutoReload", ReloadSecond);
    }

    //字符串下载失败回调
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        SendCustomEventDelayedSeconds("_AutoReload", ReloadSecond);
    }

    //重新加载字符串函数
    public void _AutoReload()
    {
        //VRC下载API
        VRCStringDownloader.LoadUrl(url, (IUdonEventReceiver)this);
    }

    #endregion

    #region API

    // 读取玩家对应Elo分数
    override public string GetEloV2(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        if (_eloDictionary == null)
            return null;

        string data = _eloDictionary[name].ToString();

        if (!string.IsNullOrEmpty(data))
        {
			if (data != "KeyDoesNotExist")
			{
                /* 解包数据 */
                var SerializationData = data.Split('#',StringSplitOptions.RemoveEmptyEntries);

                if (!string.IsNullOrEmpty(SerializationData[0]))
                    return SerializationData[0];
			}
		}

        return "0";
    }

    #endregion
}
