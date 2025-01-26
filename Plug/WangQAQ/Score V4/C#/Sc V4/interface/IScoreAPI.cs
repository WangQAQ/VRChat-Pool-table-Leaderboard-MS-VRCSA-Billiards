
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class IScoreAPI : UdonSharpBehaviour
{
	#region API

	/* 初始化 */
	public virtual void _Init(BilliardsModule billiardsModule) { /* Nop */  }

	/* Tick函数 */
	public virtual void _Tick() { /* Nop */ }

	//API lobbyPlayerList
	public virtual void _LobbyOpen(string[] lobbyPlayerList) { /* Nop */  }

	//API nowPlayerList
	public virtual void _PlayerChanged(string[] nowPlayerList, int gameState) { /* Nop */  }

	//API startPlayerList
	public virtual void _GameStarted(string[] startPlayerList) { /* Nop */  }

	//API winningTeamLocal
	public virtual void _GameEnd(uint winningTeamLocal) { /* Nop */  }

	/* 刷新计分器 */
	public virtual void _GameReset() { /* Nop */ }

	/* 设置练习模式（注意仅设置值，不会同步，为了对接BM） */
	public virtual void _SetPracticeMode(bool value) { /* Nop */ }


	#endregion
}
