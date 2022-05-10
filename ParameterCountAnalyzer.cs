using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP3
{
    public class ParameterCountAnalyzer : NodeAnalyzer
    {
        public int MaxParametersCount;
        private const int DefaultMaxParameters = 3;
        private readonly List<MethodDeclarationSyntax> _methodsTooManyParameters;

        public ParameterCountAnalyzer(string filePath) : base(filePath)
        {
            MaxParametersCount = DefaultMaxParameters;
            _methodsTooManyParameters = new List<MethodDeclarationSyntax>();
        }

        public override void Analyze()
        {
            _methodsTooManyParameters.Clear();
            base.Analyze();
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            CheckParameterListLength(node);
        }

        protected override void ShowProcessStartMessage()
        {
            base.ShowProcessStartMessage();
            Console.WriteLine("Procurando por métodos com sua lista de parâmetros longa...");
        }

        protected override void ShowResults()
        {
            if (_methodsTooManyParameters.Count > 0)
            {
                Console.WriteLine($"O(s) seguinte(s) método(s) excederam a quantidade máxima de parâmetros (Limite atual: {MaxParametersCount})\n");

                foreach (MethodDeclarationSyntax node in _methodsTooManyParameters)
                {
                    Console.Write($" - O método {node.Identifier} na linha {GetNodeLineNumber(node.Span)} ");
                    Console.WriteLine($"tem {node.ParameterList.Parameters.Count} parâmetros em sua assinatura. ");
                }

                Console.WriteLine("\nConsidere encapsular alguns dos parâmetros contidos nos métodos listados.");
            }
            else
            {
                Console.WriteLine($"Nenhum métodos excedeu a quantidade máxima de parâmetros estipulada (Limite atual: {MaxParametersCount})");
            }

            base.ShowResults();
        }

        private void CheckParameterListLength(MethodDeclarationSyntax methodDeclarationNode)
        {
            if (methodDeclarationNode.ParameterList.Parameters.Count > MaxParametersCount)
                _methodsTooManyParameters.Add(methodDeclarationNode);
        }
    }
}
