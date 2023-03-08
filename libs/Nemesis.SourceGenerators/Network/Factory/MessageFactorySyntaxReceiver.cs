using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Nemesis.SourceGenerators.Network.Factory;

internal sealed class MessageFactorySyntaxReceiver : ISyntaxContextReceiver
{
	private const string BaseMessageClassName = "PokemonMessage";

	public List<INamedTypeSymbol> CandidateClasses { get; } = new();

	public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
	{
		if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
			return;

		if (context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
			return;

		var baseSymbol = classSymbol.BaseType;

		while (baseSymbol != null && !baseSymbol.Name.Equals(BaseMessageClassName, StringComparison.Ordinal))
			baseSymbol = baseSymbol.BaseType;

		if (baseSymbol is null || !baseSymbol.Name.Equals(BaseMessageClassName, StringComparison.Ordinal))
			return;

		CandidateClasses.Add(classSymbol);
	}
}