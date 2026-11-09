using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
try
{
    var results = new List<object>();
    foreach (string path in args)
    {
        var root=CSharpSyntaxTree.ParseText(File.ReadAllText(path)).GetRoot();
        foreach (var node in root.DescendantNodes().Where(n => n is MethodDeclarationSyntax or ConstructorDeclarationSyntax))
        {
            var method=node as MethodDeclarationSyntax;
            var constructor=node as ConstructorDeclarationSyntax;
            var body=(SyntaxNode)method?.Body ?? (SyntaxNode)constructor?.Body
                ?? method?.ExpressionBody;
            if(body==null)continue;
            var branches=new List<object>();
            foreach(var branch in body.DescendantNodesAndSelf())
            {
                string kind = branch switch
                {
                    IfStatementSyntax => "if", ForStatementSyntax => "for",
                    ForEachStatementSyntax => "foreach", WhileStatementSyntax => "while",
                    DoStatementSyntax => "do", CatchClauseSyntax => "catch",
                    ConditionalExpressionSyntax => "?:", SwitchExpressionArmSyntax => "switch arm",
                    CaseSwitchLabelSyntax => "case", CasePatternSwitchLabelSyntax => "case pattern",
                    ConditionalAccessExpressionSyntax => "?.",
                    BinaryExpressionSyntax b when b.IsKind(SyntaxKind.LogicalAndExpression) => "&&",
                    BinaryExpressionSyntax b when b.IsKind(SyntaxKind.LogicalOrExpression) => "||",
                    BinaryExpressionSyntax b when b.IsKind(SyntaxKind.CoalesceExpression) => "??",
                    _ => null,
                };
                if(kind!=null)branches.Add(new { Kind=kind,Line=branch.GetLocation().GetLineSpan().StartLinePosition.Line+1 });
            }
            int depth=body.DescendantNodesAndSelf().Select(n=>n.AncestorsAndSelf().Count(a=>
                a is IfStatementSyntax or ForStatementSyntax or ForEachStatementSyntax
                    or WhileStatementSyntax or SwitchExpressionSyntax)).DefaultIfEmpty(0).Max();
            results.Add(new {
                File=path,Name=method?.Identifier.ValueText ?? constructor.Identifier.ValueText,
                Parameters=method?.ParameterList.Parameters.Count ?? constructor.ParameterList.Parameters.Count,
                Complexity=branches.Count+1,
                BodyLines=body.ToString().Split('\n').Count(x=>!string.IsNullOrWhiteSpace(x)),
                MethodLines=node.ToString().Split('\n').Count(x=>!string.IsNullOrWhiteSpace(x)),
                Depth=depth,Returns=body.DescendantNodes().OfType<ReturnStatementSyntax>().Count(),
                ConsoleCalls=body.DescendantNodes().OfType<InvocationExpressionSyntax>()
                    .Count(x=>x.Expression.ToString().StartsWith("Console.")),Branches=branches,
            });
        }
    }
    Console.WriteLine(JsonSerializer.Serialize(results,new JsonSerializerOptions{WriteIndented=true}));
    return 0;
}
catch(Exception error){Console.Error.WriteLine(error.Message);return 1;}
