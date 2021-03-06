using UnityEngine;
using UnityEngine.UI;
using System;

// This file was auto generated by ElementSystem
public partial class ImitateShotPage : Page
{
	private RawImage _rawImage_moniter;
	protected RawImage RawImage_moniter
	{
		get
		{
			if(_rawImage_moniter == null)
			{
				var t = ElementUtils.FindByPath(transform, "$rawImage_moniter");
				_rawImage_moniter = t.GetComponent<RawImage>();
			}
			return _rawImage_moniter;
		}
	}

	private RawImage _rawImage_fixed;
	protected RawImage RawImage_fixed
	{
		get
		{
			if(_rawImage_fixed == null)
			{
				var t = ElementUtils.FindByPath(transform, "$rawImage_fixed");
				_rawImage_fixed = t.GetComponent<RawImage>();
			}
			return _rawImage_fixed;
		}
	}

}
