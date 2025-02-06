/*
 *  MIT License
 *  Copyright (c) 2024 WangQAQ
 *
 *  Build主体
 */

using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine;
using WangQAQ.PoolBuild;
using UnityEditor.SceneManagement;
using System;
using System.Diagnostics;

namespace WangQAQ.Plug
{
	public class BuildTool : EditorWindow
	{
		#region PrivateVariables

		private static IPlugInitializer[] plugInitializer =
		{
			new ScoreBinder(),
			new EloBinder(),
			new EloDownloadBinder(),
			new ColorBinder(),
			new MagnificationBinder(),
			new TagBinder(),
			new NewsBinder(),
			new TableHookBinder()
		};

		#region Message

		/* 检查回弹字符串 */
		private string checkString = string.Empty;

		/* 绑定回弹字符串 */
		private string bindString = string.Empty;

		/* 登录回弹字符串 */
		private string loginString = string.Empty;

		/* 上传密钥回弹字符串 */
		private string uploadString = string.Empty;

		#endregion

		/* 是否登录 */
		private bool isLogin = false;

		/* 中间量 */
		private string emailInput = "Enter Email (输入注册时的邮箱)";
		private string passwordInput = "Enter Password 输入密码";

		/* 分段按钮开关 */
		private bool isCheck = false;
		private bool isUploadKey = false;

		/* 静态对象 */
		private static UserLogin _userLogin = new UserLogin();
		private static KeyUploader _keyUploader = new KeyUploader();
		#endregion

		#region API

		public void OnEnable()
		{
			// 执行初始化操作
			RefreshLoginState();
		}

		#endregion

		#region Menu

		[MenuItem("MS-VRCSA/BuildTool")]
		public static void ShowWindow()
		{
			//向此委托添加函数，以便将其执行延迟到检视面板更新完成之后
			//每个函数在添加后仅执行一次
			EditorApplication.delayCall += () =>
			{
				//获取窗口
				var window = GetWindow<BuildTool>("Pool-BuildTool");
				//设置窗口位置及大小
				window.position = new Rect(200, 200, 400, 600);
				//限制最小尺寸
				window.minSize = new Vector2(400, 600);
				//限制最大尺寸
				window.maxSize = new Vector2(400, 600);
				//打开窗口
				window.Show();
			};
		}

		#endregion

		#region GUI

		public async void OnGUI()
		{
			/* 标题 */
			GUILayout.Label("BuildSystem - 构建系统", new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold });

			/* 检查插件数量 */
			GUILayout.Label("1.CheckPlug (检查插件)", new GUIStyle(GUI.skin.label) { fontSize = 15 });

			if (GUILayout.Button("CheckPlug (检查插件)"))
			{
				var data = PlugCheck.Check();

				checkString = data.Message;

				if(data.isDone)
					isCheck = true;
			}

			/*显示检查信息*/
			if (!string.IsNullOrEmpty(checkString))
			{
				GUILayout.Label(checkString);
			}

			/* 调整布局 */
			GUILayout.Space(10);
			
			/* 是否检查插件 */
			GUI.enabled = isCheck && !isUploadKey;

			/* 上传密钥 */
			GUILayout.Label("2.Upload Key (上传密钥)", new GUIStyle(GUI.skin.label) { fontSize = 15 });

			if (GUILayout.Button("(Re)UploadKey ((重新)上传地图密钥)"))
			{
				if (BuildToolLib.GetWorldGUID() != Guid.Empty.ToString())
				{
					var data = await _keyUploader.UploadOrUpdateKeyAsync();
					uploadString = data.Item2;

					if(data.State)
						isUploadKey = true;
				}
				else
				{
					uploadString = "Please upload the map once (请上传一遍地图)";
				}
			}

			/* 显示上传回弹 */
			if (!string.IsNullOrEmpty(uploadString))
			{
				GUILayout.Label(uploadString);
			}

			/* 调整布局 */
			GUILayout.Space(10);

			/* 是否检查插件和上传密钥 */
			GUI.enabled = isCheck && isUploadKey;

			/* 尝试初始化脚本测试 */
			GUILayout.Label("3.InitializePlug (初始化脚本)", new GUIStyle(GUI.skin.label) { fontSize = 15 });

			if (GUILayout.Button("InitializePlug (初始化脚本)"))
			{
				/* 调用绑定器 */
				foreach (var a in plugInitializer)
				{
					if (!a.Init())
					{
						bindString = $"Error : {nameof(a)}";
					}
				}
				bindString = "Done.";

				// 标记场景为脏，这样可以确保下次保存时变更被保存
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}

			/*显示绑定信息*/
			if (!string.IsNullOrEmpty(bindString))
			{
				GUILayout.Label(bindString);
			}

			/* 调整布局 */
			GUILayout.FlexibleSpace();

			/* 复原 GUI 标志位 */
			GUI.enabled = true;

			/* 登录 或 退出 */
			if (!isLogin)
			{
				GUILayout.Label("为了防止密钥丢失，推荐您使用登录账号并将绑定到账户", new GUIStyle(GUI.skin.label) { fontSize = 15, normal = { textColor = Color.red } });
				GUILayout.Label("In order to prevent the loss of the key, it is recommended that you log in to your account and link the map to your account", new GUIStyle(GUI.skin.label) { fontSize = 13, normal = { textColor = Color.red }, wordWrap = true });

				/* 输入账号 */
				GUILayout.Label("4.Login (登录，可选项) (Optional)", new GUIStyle(GUI.skin.label) { fontSize = 15 });

				/* 调整布局 */
				GUILayout.Space(5);

				emailInput = EditorGUILayout.TextField("Email", emailInput);

				/* 输入密码 */
				passwordInput = EditorGUILayout.TextField("Password", passwordInput);

				/* 调整布局 */
				GUILayout.Space(5);

				/* 登录 */
				if (GUILayout.Button("Login (登录)"))
				{
					if(!await _userLogin.LoginAsync(emailInput, passwordInput))
					{
						loginString = "The account or password incorrect (账号或密码错误)";
					}
					RefreshLoginState();
				}

				/* 显示登录回弹 */
				if (!string.IsNullOrEmpty(loginString))
				{
					GUILayout.Label(loginString);
				}
			}
			else
			{
				/* 清除登录回弹信息 */
				loginString = "";

				/* 退出 */
				if (GUILayout.Button("Logout (退出)"))
				{
					BuildToolLib.SetJWT("");
					RefreshLoginState();
				}
			}

			/* 调整布局 */
			GUILayout.Space(5);

			/* 打开个人主页 */
			if (GUILayout.Button("My Account (Open Web) (个人账号)"))
			{
				/* 测试链接 */
				string url = "https://www.wangqaq.com/PoolBar/Account";
				Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
			}

			/* 调整布局 */
			GUILayout.Space(20);
		}

		#endregion

		#region PublicAPI

		public async void RefreshLoginState()
		{
			await _userLogin.CheckLoginAsync();
			isLogin = BuildToolLib.GetLoginState();

			Repaint();
		}

		#endregion
	}

	[InitializeOnLoad]
	public class StartupWindowOpener
	{
		#region OnStartup

		static StartupWindowOpener()
		{
			// 检查 EditorPrefs 中是否已经设置过标志
			if (!EditorPrefs.GetBool("BuildToolIsInitialized", false))
			{
				// 当 Unity 编辑器初始化时调用
				EditorApplication.delayCall += () =>
				{
					BuildTool.ShowWindow();
				};

				// 设置 EditorPrefs 中的标志
				EditorPrefs.SetBool("BuildToolIsInitialized", true);
			}
		}

		#endregion
	}
}


