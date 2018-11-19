using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TriggersTools.DiscordBots.Analyzer {
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreconditionerCodeFixProvider)), Shared]
	public sealed class PreconditionerCodeFixProvider : CodeFixProvider {
		public override ImmutableArray<string> FixableDiagnosticIds {
			get => ImmutableArray.Create(PreconditionerAnalyzer.DiagnosticId);
		}

		public override FixAllProvider GetFixAllProvider() {
			return WellKnownFixAllProviders.BatchFixer;
		}

		public override async Task RegisterCodeFixesAsync(CodeFixContext context) {
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var diagnostic = context.Diagnostics.First();
			if (root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is AttributeSyntax node) {
				context.RegisterCodeFix(
					CodeAction.Create("Correct to `Requires`", c => FixAsync(context.Document, node, c), "Correct to `Requires`"),
					diagnostic);
			}
		}

		static async Task<Document> FixAsync(Document document, AttributeSyntax node, CancellationToken cancellationToken) {
			var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
			var attributesNameNode = node.Name;
			string attributeName = attributesNameNode.ToString();
			string newAttributeName = Preconditioner.FixName(attributeName);
			var newAttributeNameNode = SyntaxFactory.ParseName(newAttributeName);

			return document.WithSyntaxRoot(root.ReplaceNode(attributesNameNode, newAttributeNameNode));
		}
	}
}