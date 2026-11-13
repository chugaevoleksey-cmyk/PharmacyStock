using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;
var root = Path.GetFullPath(args[0]);
int Loc(string text) => text.Split('\n').Count(l => l.Trim().Length > 0 && !l.TrimStart().StartsWith("//"));
bool Decision(SyntaxNode n) => n is IfStatementSyntax or ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax or CatchClauseSyntax or CaseSwitchLabelSyntax or ConditionalExpressionSyntax || n is BinaryExpressionSyntax b && (b.IsKind(SyntaxKind.LogicalAndExpression) || b.IsKind(SyntaxKind.LogicalOrExpression) || b.IsKind(SyntaxKind.CoalesceExpression)) || n is SwitchExpressionArmSyntax a && a.Pattern is not DiscardPatternSyntax;
bool Control(SyntaxNode n) => n is IfStatementSyntax or ForStatementSyntax or ForEachStatementSyntax or WhileStatementSyntax or DoStatementSyntax or SwitchStatementSyntax or CatchClauseSyntax or UsingStatementSyntax;
var rows = new List<object>(); int total = 0;
foreach (var file in Directory.GetFiles(root, "*.cs", SearchOption.AllDirectories).Where(f => !f.Contains(Path.DirectorySeparatorChar+"obj"+Path.DirectorySeparatorChar) && !f.Contains(Path.DirectorySeparatorChar+"bin"+Path.DirectorySeparatorChar)))
{
    var text = File.ReadAllText(file); total += Loc(text);
    var syntax = CSharpSyntaxTree.ParseText(text).GetRoot();
    foreach(var m in syntax.DescendantNodes().OfType<MethodDeclarationSyntax>())
    {
        var body = (SyntaxNode?)m.Body ?? m.ExpressionBody!;
        var points = body.DescendantNodesAndSelf().Where(Decision).ToList();
        var nesting = body.DescendantNodesAndSelf().Where(Control).Select(n => 1 + n.Ancestors().TakeWhile(a => a != m).Count(a => Control(a) && !(a is IfStatementSyntax i && i.Else?.Statement is IfStatementSyntax next && next.Span.Contains(n.Span)))).DefaultIfEmpty(0).Max();
        rows.Add(new {file=Path.GetRelativePath(root,file).Replace('\\','/'), method=((TypeDeclarationSyntax)m.Parent!).Identifier.Text+"."+m.Identifier.Text, line=m.GetLocation().GetLineSpan().StartLinePosition.Line+1, loc=Loc(m.ToString()), cc=1+points.Count, depth=nesting, parameters=m.ParameterList.Parameters.Count, decisions=points.Select(n=>new {line=n.GetLocation().GetLineSpan().StartLinePosition.Line+1, kind=n.Kind().ToString()}).ToList()});
    }
}
Console.WriteLine(JsonSerializer.Serialize(new {moduleLoc=total,methods=rows},new JsonSerializerOptions{WriteIndented=true}));
