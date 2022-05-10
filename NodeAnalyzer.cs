using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TP3
{
    public class NodeAnalyzer : CSharpSyntaxVisitor
    {
        private SyntaxTree _tree;
        private CSharpCompilation _compilation;
        public SemanticModel SemanticModel { get; private set; }
        public string CurrentFilePath { get; private set; }

        public NodeAnalyzer(string filePath)
        {
            CurrentFilePath = filePath;
            _tree = ParseProgramFile(filePath);
        }

        public virtual void Analyze()
        {
            VisitAllNodes(_tree.GetRoot());
        }

        public virtual void UpdateAnalyzerValues()
        {

        }

        protected void VisitAllNodes(SyntaxNode root)
        {
            ShowProcessStartMessage();
            VisitChildrenNodes(root);
            OnVisitsFinished();
        }

        protected virtual void OnVisitsFinished()
        {
            ShowResults();
        }

        protected virtual void ShowProcessStartMessage()
        {
            Console.WriteLine();
        }

        protected virtual void ShowResults()
        {
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - -");
        }

        protected IEnumerable<T> GetIntersectedNodesInBody<T>(MethodDeclarationSyntax methodDeclarationNode)
        {
            return methodDeclarationNode.DescendantNodes().Where(
                node => node.Span.IntersectsWith(methodDeclarationNode.Body.Span)).OfType<T>();
        }

        protected int GetNodeLineNumber(TextSpan span)
        {
            return _tree.GetLineSpan(span).StartLinePosition.Line + 1;
        }

        private void VisitChildrenNodes(SyntaxNode node)
        {
            foreach (var childNode in node.ChildNodes())
            {
                Visit(childNode);

                if (childNode.ChildNodes().Any())
                {
                    VisitChildrenNodes(childNode);
                }
            }
        }

        private static SyntaxTree ParseProgramFile(string filePath)
        {
            StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8);
            return CSharpSyntaxTree.ParseText(streamReader.ReadToEnd());
        }

        private void CreateCompilationUnit(string filePath)
        {
            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            _compilation = CSharpCompilation.Create("TP3", new[] { _tree }, new[] { mscorlib });
            SemanticModel = _compilation.GetSemanticModel(_tree);
        }
    }
}
