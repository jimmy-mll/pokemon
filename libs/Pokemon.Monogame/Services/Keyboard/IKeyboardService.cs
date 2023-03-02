using Microsoft.Xna.Framework.Input;

namespace Pokemon.Monogame.Services.Keyboard;

public interface IKeyboardService
{
	void LoadMappings();

	void SaveMappings();

	Keys GetKeyForMapping(KeyboardMappings mapping);

	KeyboardMappings GetMappingForKey(Keys key);

	void SetKeyForMapping(KeyboardMappings mapping, Keys key);
}