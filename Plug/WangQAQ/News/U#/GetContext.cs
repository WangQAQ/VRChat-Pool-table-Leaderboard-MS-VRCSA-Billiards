using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using WangQAQ.UdonPlug;

public class GetContext : UdonSharpBehaviour
{
	[HideInInspector] public string Title;
	[HideInInspector] public string Description;

	[HideInInspector] public int UrlID;

	private GetMainContext _ContextDownloader;

	[SerializeField] public Text _title;
	[SerializeField] public Text _context;

	public void _Init()
	{
		_ContextDownloader = transform.Find("../../../../../../../ContextDownloader").GetComponent<GetMainContext>();
		_title = transform.Find("frame/Title/Text").GetComponent<Text>();
		_context = transform.Find("frame/Context/Text").GetComponent<Text>();

		_title.text = Title;
		_context.text = Description;
	}

	public void OnChick()
	{
		if (_ContextDownloader != null)
		{
			_ContextDownloader._UrlID = (uint)UrlID;
			_ContextDownloader.GetContext();
		}
	}
}
