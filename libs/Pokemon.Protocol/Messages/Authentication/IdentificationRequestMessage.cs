using Pokemon.Core.Network.Metadata;
using Pokemon.Core.Serialization;

namespace Pokemon.Protocol.Messages.Authentication;

public sealed class IdentificationRequestMessage : PokemonMessage
{
	public new const ushort Identifier = 1;

	public override ushort MessageId =>
		Identifier;

	public string Username { get; set; }

	public string Password { get; set; }

	public IdentificationRequestMessage()
	{
	}

	public IdentificationRequestMessage(string username, string password)
	{
		Username = username;
		Password = password;
	}

	public override void Serialize(PokemonWriter writer)
	{
		writer.WriteString(Username);
		writer.WriteString(Password);
	}

	public override void Deserialize(PokemonReader reader)
	{
		Username = reader.ReadString();
		Password = reader.ReadString();
	}
}