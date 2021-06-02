#if TOOLS
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
		private Control _control;

		public override void _EnterTree()
		{
			Settings.Init();
			RegisterCustomClasses();
			_control = CreateBottomMenuControl();
			AddControlToBottomPanel(_control, "CRR");
		}

		public override void _ExitTree()
		{
			UnregisterCustomClasses();
			RemoveControlFromBottomPanel(_control);
			_control = null;
		}

		private void RegisterCustomClasses()
		{
			_scripts.Clear();

			var file = new File();

			foreach (var type in GetCustomResourceTypes())
			{
				var path = ClassPath(type);
				if (!file.FileExists(path))
					continue;
				var script = GD.Load<Script>(path);
				if (script == null)
					continue;
				AddCustomType($"{Settings.ClassPrefix}{type.Name}", nameof(Resource), script, null);
				GD.Print($"Register custom resource: {type.Name} -> {path}");
				_scripts.Add(type.Name);
			}

			foreach (var type in GetCustomNodes())
			{
				var path = ClassPath(type);
				if (!file.FileExists(path))
					continue;
				var script = GD.Load<Script>(path);
				if (script == null)
					continue;
				AddCustomType($"{Settings.ClassPrefix}{type.Name}", nameof(Node), script, null);
				GD.Print($"Register custom node: {type.Name} -> {path}");
				_scripts.Add(type.Name);
			}
		}

		private static string ClassPath(Type type)
		{
			return $"{Settings.ScriptsFolder}/{type.Namespace?.Replace(".", "/") ?? ""}/{type.Name}.cs";
		}

		private static IEnumerable<Type> GetCustomResourceTypes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(Resource)));
		}

		private static IEnumerable<Type> GetCustomNodes()
		{
			var assembly = Assembly.GetAssembly(typeof(Plugin));
			return assembly.GetTypes().Where(t =>
				!t.IsAbstract && t.IsSubclassOf(typeof(Node)));
		}

		private void UnregisterCustomClasses()
		{
			foreach (var script in _scripts)
			{
				RemoveCustomType(script);
				GD.Print($"Unregister custom resource: {script}");
			}

			_scripts.Clear();
		}

		private Control CreateBottomMenuControl()
		{
			var container = new GridContainer()
			{
				RectMinSize = new Vector2(100, 100),
			};
			var button = new Button {Text = "Refresh"};
			button.Connect("pressed", this, nameof(OnRefreshPressed));
			container.AddChild(button);
			return container;
		}

		private void OnRefreshPressed()
		{
			UnregisterCustomClasses();
			RegisterCustomClasses();
		}
	}
}
#endif