using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TriggersTools.DiscordBots.Analyzer {
	internal sealed class Preconditioner {
		public const string Require = "Require";
		public const string Requires = "Requires";

		public static readonly Regex PreconditionRegex = new Regex("Require[^s].*");

		static readonly CSharpParseOptions ParseOptions = new CSharpParseOptions(
				languageVersion: LanguageVersion.Latest,
				documentationMode: DocumentationMode.None,
				kind: SourceCodeKind.Regular);

		//readonly MetadataReference[] _references;

		/*public Preconditioner(params MetadataReference[] references) {
			_references = references;
		}*/

		/*public IEnumerable<PreconditionerResult> Check(Stream file) {
			var tree = ParseFile(file);
			var compilation = CSharpCompilation.Create(nameof(Preconditioner));
			compilation = compilation.AddReferences(_references);
			compilation = compilation.AddSyntaxTrees(tree);
			var semanticModel = compilation.GetSemanticModel(tree);
			foreach (var item in tree.GetRoot().DescendantNodes()) {
				if (item.IsKind(SyntaxKind.Attribute)) {
					var attrNode = (AttributeSyntax) item;
					yield return CheckNode(attrNode, semanticModel);
				}
			}
		}*/

		public static string FixName(string attributeName) {
			if (attributeName.Length >= Requires.Length &&
				attributeName.StartsWith(Require) &&
				attributeName[Require.Length] != 's')
				return attributeName.Insert(Require.Length, "s");
			return attributeName;
		}

		/*private static bool IsRequirePrecondition(SemanticModel semanticModel, AttributeSyntax attributeNode) {
			var attributeType = semanticModel.GetTypeInfo(attributeNode);
			var namespaceSymbol = attributeType.Type.ContainingNamespace;
			string namespaceName = namespaceSymbol.ToDisplayString();
			if (namespaceName != "Discord.Commands")
				return false;
			string attributeName = attributeType.Type.Name;
			return (attributeName.Length >= Requires.Length &&
					attributeName.StartsWith("Require") &&
					attributeName[Require.Length] != 's');
		}*/

		public static PreconditionerResult CheckNode(AttributeSyntax attributeNode, SemanticModel semanticModel) {
			bool isRequire = IsRequirePrecondition(attributeNode, semanticModel);
			return new PreconditionerResult(isRequire, attributeNode.GetLocation());
		}

		public static bool IsRequirePrecondition(AttributeSyntax attributeNode, SemanticModel semanticModel) {
			var attributeTypeInfo = semanticModel.GetTypeInfo(attributeNode).Type;
			string namespaceName = attributeTypeInfo?.ContainingNamespace?.ToDisplayString();
			string attributeName = attributeTypeInfo?.Name;

			if (namespaceName == null || attributeName == null)
				return false;

			return (namespaceName == "Discord.Commands" && attributeName.Length >= Requires.Length &&
					attributeName.StartsWith("Require") && attributeName[Require.Length] != 's');
		}

		/*public static InvocationExpressionSyntax FindExpressionForConfigureAwait(SyntaxNode node) {
			foreach (var item in node.ChildNodes()) {
				if (item is InvocationExpressionSyntax invocationExpressionSyntax)
					return invocationExpressionSyntax;
				return FindExpressionForConfigureAwait(item);
			}
			return null;
		}

		public static PreconditionerResult CheckNode(AwaitExpressionSyntax awaitNode, SemanticModel semanticModel) {
			var possibleConfigureAwait = FindExpressionForConfigureAwait(awaitNode);
			if (possibleConfigureAwait != null && IsConfigureAwait(possibleConfigureAwait.Expression)) {
				if (HasFalseArgument(possibleConfigureAwait.ArgumentList)) {
					return new PreconditionerResult(false, awaitNode.GetLocation());
				}
				else {
					return new PreconditionerResult(true, awaitNode.GetLocation());
				}
			}
			else {
				var can = CanHaveConfigureAwait(awaitNode.Expression, semanticModel);
				return new PreconditionerResult(can, awaitNode.GetLocation());
			}
		}

		public static bool CanHaveConfigureAwait(ExpressionSyntax expression, SemanticModel semanticModel) {
			var typeInfo = semanticModel.GetTypeInfo(expression);
			var type = typeInfo.ConvertedType;
			if (type == null)
				return false;
			var members = type.GetMembers(ConfigureAwaitIdentifier);
			foreach (var item in members) {
				if (!(item is IMethodSymbol methodSymbol))
					break;
				var parameters = methodSymbol.Parameters;
				if (parameters.Length != 1)
					break;
				if (parameters[0].Type.SpecialType != SpecialType.System_Boolean)
					break;

				return true;
			}
			return false;
		}

		public static bool IsConfigureAwait(ExpressionSyntax expression) {
			if (!(expression is MemberAccessExpressionSyntax memberAccess))
				return false;
			if (!memberAccess.Name.Identifier.Text.Equals(ConfigureAwaitIdentifier, StringComparison.Ordinal))
				return false;
			return true;
		}

		public static bool HasFalseArgument(ArgumentListSyntax argumentList) {
			if (argumentList.Arguments.Count != 1)
				return false;
			if (!argumentList.Arguments[0].Expression.IsKind(SyntaxKind.FalseLiteralExpression))
				return false;
			return true;
		}*/

		SyntaxTree ParseFile(Stream file) {
			using (var reader = new StreamReader(file, Encoding.UTF8, true, 16 * 1024, true)) {
				return CSharpSyntaxTree.ParseText(reader.ReadToEnd(),
					options: ParseOptions);
			}
		}
	}
}
