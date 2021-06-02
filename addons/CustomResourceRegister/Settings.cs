#if TOOLS
using Godot;
using Godot.Collections;

namespace CustomResourceRegister
{
	public static class Settings
	{
		public static string ScriptsFolder => GetSettings(nameof(ScriptsFolder)).ToString();
		public static string ClassPrefix => GetSettings(nameof(ClassPrefix)).ToString();

		public static void Init()
		{
			AddSetting(nameof(ScriptsFolder), Variant.Type.String, "res://");
			AddSetting(nameof(ClassPrefix), Variant.Type.String, "");
		}

		private static object GetSettings(string title)
		{
			return ProjectSettings.GetSetting($"{nameof(CustomResourceRegister)}/{title}");
		}

		private static void AddSetting(string title, Variant.Type type, object value)
		{
			title = SettingPath(title);
			if (ProjectSettings.HasSetting(title))
				return;
			ProjectSettings.SetSetting(title, value);
			var info = new Dictionary
			{
				["name"] = title,
				["type"] = type
			};
			ProjectSettings.AddPropertyInfo(info);
		}

		private static string SettingPath(string title) => $"{nameof(CustomResourceRegister)}/{title}";
	}
}
#endif