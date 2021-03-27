using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;

namespace CustomResourceRegister
{
	[Tool]
	public class Plugin : EditorPlugin
	{
		private readonly List<string> _scripts = new List<string>();

		public override void _EnterTree()
		{
			Settings.Init();
			RegisterCustomResources();
		}

		public override void _ExitTree()
		{
			UnregisterCustomResources();
		}

		private void RegisterCustomResources()
		{
			_scripts.Clear();

			foreach (var type in GetCustomResourceTypes())
			{
				var path = $"{Settings.ScriptsFolder}/{type.Namespace?.Replace(".", "/") ?? ""}/{type.Name}.cs";
				var script = GD.Load<Script>(path);
				AddCustomType($"{Settings.ClassPrefix}{type.Name}", nameof(Resource), script, null);
				GD.Print($"Register custom resource: {type.Name} -> {path}");
				_scripts.Add(type.Name);
			}
		}

		private static IEnumerable<Type> GetCustomResourceTypes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Resource)));
		}

		private void UnregisterCustomResources()
		{
			foreach (var script in _scripts)
			{
				RemoveCustomType(script);
				GD.Print($"Unregister custom resource: {script}");
			}

			_scripts.Clear();
		}
	}
}