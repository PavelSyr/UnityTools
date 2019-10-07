using UnityEngine;
using UnityEditor.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Events;
using System;

namespace com.ich.Tools.UI
{
	public static class UIButtonOnClickBinding
	{
		public static void Bind(Transform root, MonoBehaviour context, int parentLevel = 0, string suffix = "_OnClick")
		{
			var ctx = context.GetType();
			Debug.Log($"Bind Buttons to *{suffix} Methods of {ctx} type");
			var methods = ctx.GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			var scopeMethods = from m in methods
							   where m.Name.Contains(suffix)
							   select m;
								
			var btns = root.GetComponentsInChildren<Button>();
			var scopeBtns = parentLevel <= 0 ? btns :
							from b in btns
							where b.transform.GetParentLevel(root) <= parentLevel
							select b;

			foreach (var item in scopeBtns)
			{
				while (item.onClick.GetPersistentEventCount() > 0)
				{
					UnityEventTools.RemovePersistentListener(item.onClick, item.onClick.GetPersistentEventCount() - 1);
				}

				var clickMethods = from m in scopeMethods
								   where m.Name.ToLower().Contains(item.name.ToLower())
								   select m;

				foreach(var cm in clickMethods)
				{
					Debug.Log($"<Color=green>bind : {item.name} to {cm.Name} </Color>");
					try
					{
						var callback = Delegate.CreateDelegate(typeof(UnityAction), context, cm.Name) as UnityAction;
						UnityEventTools.AddPersistentListener(item.onClick, callback);
					}
					catch (Exception ex)
					{
						Debug.LogError($"Bind {item.name} to {cm.Name} is Falied with : " + ex);
					}
				}

				if (item.onClick.GetPersistentEventCount() == 0)
				{
					Debug.Log($"<Color=red>Method is not found for button : {item.name}. Please add {ctx}.{item.name}{suffix}() method to bint it to {item.name} button</Color>");
				}
			}
		}

		private static int GetParentLevel(this Transform transform, Transform origin = null)
		{
			if (transform.parent == null || transform.parent == origin)
				return 0;

			return GetParentLevel(transform.parent, origin) + 1;
		}
	}
}