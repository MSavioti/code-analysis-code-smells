using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace TP3
{
    public class MethodLengthAnalyzer : NodeAnalyzer
    {
        public int MaxLinesInMethodBody;
        public bool CountBlankLines;
        public bool CountBracketOnlyLines;
        private const int DefaultMaxLines = 10;
        private const bool CountBlankLinesDefault = false;
        private const bool CountBracketOnlyLinesDefault = false;
        private const int BracketsCount = 1;
        private readonly List<LongMethodInfo> _longMethodInfos;

        private readonly struct LongMethodInfo
        {
            public readonly string MethodName;
            public readonly string FilePath;
            public readonly int ValidLines;

            public LongMethodInfo(string methodName, string filePath, int validLines)
            {
                MethodName = methodName;
                FilePath = filePath;
                ValidLines = validLines;
            }
        }

        public MethodLengthAnalyzer(string filePath) : base(filePath)
        {
            MaxLinesInMethodBody = DefaultMaxLines;
            CountBlankLines = CountBlankLinesDefault;
            CountBracketOnlyLines = CountBracketOnlyLinesDefault;
            _longMethodInfos = new List<LongMethodInfo>();
        }

        public override void Analyze()
        {
            _longMethodInfos.Clear();
            base.Analyze();
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            ComputeMethodSize(node);
        }

        protected override void ShowProcessStartMessage()
        {
            base.ShowProcessStartMessage();
            Console.WriteLine("Procurando por métodos longos...");
        }

        protected override void ShowResults()
        {
            if (_longMethodInfos.Count > 0)
            {
                Console.WriteLine($"O(s) método(s) a seguir excedera(m) a quantidade máxima de linhas (Valor atual: {MaxLinesInMethodBody}).\n");

                foreach (var longMethodInfo in _longMethodInfos)
                {
                    Console.WriteLine($" - O método \"{longMethodInfo.MethodName}\" tem {longMethodInfo.ValidLines} linhas válidas em seu corpo.");
                }

                Console.WriteLine("\nConsidere dividir o conteúdo do(s) método(s) listado(s) em dois ou mais métodos.");
            }
            else
            {
                Console.WriteLine($"Nenhum método excedeu a quantidade máxima de linhas estipulada(Valor atual: {MaxLinesInMethodBody}).");
            }

            base.ShowResults();
        }

        private void ComputeMethodSize(MethodDeclarationSyntax methodDeclarationNode)
        {
            if (methodDeclarationNode.Body.GetText().Lines.Count <= MaxLinesInMethodBody)
                return;

            int validLines = GetValidLines(methodDeclarationNode.Body.GetText().Lines);

            if (validLines < MaxLinesInMethodBody)
                return;

            _longMethodInfos.Add(new LongMethodInfo(methodDeclarationNode.Identifier.ToString(), CurrentFilePath, validLines));
        }

        private int GetValidLines(TextLineCollection lines)
        {
            int validLinesCount = 0;

            foreach (TextLine line in lines)
            {
                if (IsValidLine(line))
                    validLinesCount++;
            }

            return validLinesCount;
        }

        private bool IsValidLine(TextLine line)
        {
            if ((!CountBlankLines) && (IsLineEmpty(line)))
                return false;

            if ((!CountBracketOnlyLines) && (IsBracketOnly(line)))
                return false;

            return true;
        }

        private bool IsLineEmpty(TextLine line)
        {
            return line.Span.Length == 0;
        }

        private bool IsBracketOnly(TextLine line)
        {
            string cachedLine = line.ToString();

            if (!(cachedLine.Contains("{")) && !(cachedLine.Contains("}")))
                return false;

            if (cachedLine.Trim().Length > BracketsCount)
                return false;

            return true;
        }
    }
}
