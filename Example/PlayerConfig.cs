using Godot;

namespace Example
{
	public class PlayerConfig : Resource
	{
		[Export]
		public int Health;

		[Export]
		public int Speed;
	}
}