using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Xna.Framework.Input;

namespace Pokemon.Monogame.Services.Keyboard;

public sealed class KeyboardService : IKeyboardService
{
	private const string KeyboardMappingsFileName = "keyboard.json";

	private Dictionary<KeyboardMappings, Keys> _mappings;

	public KeyboardService() =>
		_mappings = new Dictionary<KeyboardMappings, Keys>();

	public void LoadMappings()
	{
		if (!File.Exists(KeyboardMappingsFileName))
		{
			_mappings = new Dictionary<KeyboardMappings, Keys>
			{
				[KeyboardMappings.Left] = Keys.Q,
				[KeyboardMappings.Right] = Keys.D,
				[KeyboardMappings.Up] = Keys.Z,
				[KeyboardMappings.Down] = Keys.S,
				[KeyboardMappings.Run] = Keys.LeftShift
			};

			SaveMappings();
			return;
		}

		_mappings = JsonSerializer.Deserialize<Dictionary<KeyboardMappings, Keys>>(File.ReadAllText(KeyboardMappingsFileName));
	}

	public void SaveMappings() =>
		File.WriteAllText(KeyboardMappingsFileName, JsonSerializer.Serialize(_mappings));

	public Keys GetKeyForMapping(KeyboardMappings mapping) =>
		_mappings[mapping];

	public KeyboardMappings GetMappingForKey(Keys key)
	{
		return _mappings.ContainsValue(key)
			? _mappings.First(x => x.Value == key).Key
			: KeyboardMappings.None;
	}

	public void SetKeyForMapping(KeyboardMappings mapping, Keys key) =>
		_mappings[mapping] = key;
}