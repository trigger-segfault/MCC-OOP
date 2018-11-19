using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TriggersTools.DiscordBots.Analyzer {
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed class PreconditionerAnalyzer : DiagnosticAnalyzer {
		public const string DiagnosticId = "RequiresPreconditioner";

		static readonly string Title = "DNRP001";
		static readonly string MessageFormat = "'Require' precondition is obsolete, use 'Requires' instead";
		const string Category = "Code";

		static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context) {
			context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.Attribute);
		}
		
		static void Analyze(SyntaxNodeAnalysisContext context) {
			var attrNode = (AttributeSyntax) context.Node;
			var check = Preconditioner.CheckNode(attrNode, context.SemanticModel);
			if (check.NeedsRequires) {
				var diagnostic = Diagnostic.Create(Rule, check.Location);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
