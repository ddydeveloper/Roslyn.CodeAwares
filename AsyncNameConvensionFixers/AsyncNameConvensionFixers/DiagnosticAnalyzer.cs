using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AsyncNameConvensionFixers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AsyncNameConvensionFixersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "AsyncConvention";
        private const string Title = "Wrong async name convention";
        private const string Category = "Usage";

        private static readonly DiagnosticDescriptor AddPostfixRule = new DiagnosticDescriptor(DiagnosticId, Title,
            "\"Async\" postfix is absent", Category, DiagnosticSeverity.Info, true, "Add \"Async\" postfix");

        private static readonly DiagnosticDescriptor ReplacePostfixPositionRule = new DiagnosticDescriptor(DiagnosticId, Title,
            "Wrong position of the \"Async\" postfix", Category, DiagnosticSeverity.Info, true, "Move \"Async\" to the end of the method name");

        private static readonly DiagnosticDescriptor ReplacePostfixRule = new DiagnosticDescriptor(DiagnosticId, Title,
            "Wrong \"Async\" postfix", Category, DiagnosticSeverity.Info, true, "Replace to \"Async\"");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(AddPostfixRule, ReplacePostfixRule, ReplacePostfixPositionRule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeNode, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            if (!methodDeclaration.Modifiers.Any(SyntaxKind.AsyncKeyword))
            {
                return;
            }

            if (methodDeclaration.Identifier.ValueText.EndsWith("Async"))
            {
                return;
            }

            if (methodDeclaration.Identifier.ValueText.EndsWith("Asycn"))
            {
                context.ReportDiagnostic(Diagnostic.Create(ReplacePostfixRule, context.Node.GetLocation()));
                return;
            }

            if (methodDeclaration.Identifier.ValueText.Contains("Async"))
            {
                context.ReportDiagnostic(Diagnostic.Create(ReplacePostfixPositionRule, context.Node.GetLocation()));
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(AddPostfixRule, context.Node.GetLocation()));
        }
    }
}
