using UnityEngine;
using System.Collections.Generic;

public class SingleObject<T> : MonoBehaviour where T: SingleObject<T>
{
	static T instance;
	public static T Instance {
		get 
		{
			if(instance==null)
			{
				
				instance = MonoUtil.CreateBehaviour<T>(typeof(T).Name,null);
				GameObject.DontDestroyOnLoad(instance.gameObject);
			}
			return instance;
		}
	}

	protected virtual void Destroy ()
	{
		if(gameObject!=null)GameObject.Destroy(gameObject);
		instance = null;
	}
}