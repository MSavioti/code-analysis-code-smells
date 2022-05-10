using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP3
{
    public class MagicAttributesAnalyzer : NodeAnalyzer
    {
        public List<double> MagicNumbersWhitelist;
        public List<string> MagicStringsWhitelist;
        public List<char> MagicCharsWhitelist;
        private readonly List<MagicAttributeInfo> _magicAttributeInfos;

        private readonly struct MagicAttributeInfo
        {
            public readonly int LineNumber;
            public readonly string MagicValue;

            public MagicAttributeInfo(int lineNumber, string magicValue)
            {
                LineNumber = lineNumber;
                MagicValue = magicValue;
            }
        }

        public MagicAttributesAnalyzer(string filePath) : base(filePath)
        {
            MagicNumbersWhitelist = new List<double>() { -1, 0, 1 };
            MagicStringsWhitelist = new List<string>() {""};
            MagicCharsWhitelist = new List<char>() {'\0', '\r', '\n'};
            _magicAttributeInfos = new List<MagicAttributeInfo>();
        }

        public override void Analyze()
        {
            _magicAttributeInfos.Clear();
            base.Analyze();
        }

        public override void VisitLiteralExpression(LiteralExpressionSyntax node)
        {
            base.VisitLiteralExpression(node);
            ProcessLiteralExpression(node);
        }

        protected override void ShowProcessStartMessage()
        {
            base.ShowProcessStartMessage();
            Console.WriteLine("Procurando por atributos mágicos...");
            PrintWhitelists();
        }

        protected override void ShowResults()
        {
            if (_magicAttributeInfos.Count > 0)
            {
                Console.WriteLine("Os seguintes atributos mágicos foram encontrados:\n");

                foreach (MagicAttributeInfo magicAttributeInfo in _magicAttributeInfos)
                {
                    Console.WriteLine($" - {magicAttributeInfo.MagicValue} encontrado na linha {magicAttributeInfo.LineNumber}");
                }

                Console.WriteLine("\nConsidere atribuir os valores mostrados a constantes com nomes descritivos.");
            }
            else
            {
                Console.WriteLine("Não foram encontrados atributos mágicos no arquivo.");
            }

            base.ShowResults();
        }

        private void ProcessLiteralExpression(LiteralExpressionSyntax literalExpression)
        {
            if (!CanBeMagicAttribute(literalExpression))
                return;

            if (IsInWhitelist(literalExpression))
                return;

            _magicAttributeInfos.Add(new MagicAttributeInfo(GetNodeLineNumber(literalExpression.Span), literalExpression.ToString()));
        }

        private bool IsInWhitelist(LiteralExpressionSyntax literalExpression)
        {
            if (literalExpression.Token.IsKind(SyntaxKind.NumericLiteralToken))
            {
                foreach (double magic in MagicNumbersWhitelist)
                {
                    if (literalExpression.ToString().Equals(magic.ToString(new NumberFormatInfo())))
                        return true;
                }
            }
            else if (literalExpression.Token.IsKind(SyntaxKind.CharacterLiteralToken))
            {
                foreach (char magic in MagicCharsWhitelist)
                {
                    if (literalExpression.ToString().Equals(magic.ToString()))
                        return true;
                }
            }
            else if (literalExpression.Token.IsKind(SyntaxKind.StringLiteralToken))
            {
                foreach (string magic in MagicStringsWhitelist)
                {
                    if (literalExpression.ToString().Equals(magic))
                        return true;
                }
            }

            return false;
        }

        private bool CanBeMagicAttribute(LiteralExpressionSyntax literalExpression)
        {
            return literalExpression.Token.IsKind(SyntaxKind.NumericLiteralToken) ||
                   literalExpression.Token.IsKind(SyntaxKind.CharacterLiteralToken) ||
                   literalExpression.Token.IsKind(SyntaxKind.StringLiteralToken);
        }

        private void PrintWhitelists()
        {
            if ((MagicNumbersWhitelist.Count == 0) && (MagicStringsWhitelist.Count == 0) && (MagicCharsWhitelist.Count == 0))
                return;

            Console.Write("Os seguintes atributos literais não serão considerados ");
            Console.WriteLine("como atributos mágicos e não aparecerão nos resultados:");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - -");

            foreach (var d in MagicNumbersWhitelist)
            {
                Console.WriteLine(d);
            }

            foreach (string s in MagicStringsWhitelist)
            {
                if (s.Equals(""))
                    Console.WriteLine("\"\"");
                else
                    Console.WriteLine(s);
            }

            Console.WriteLine("Tipo char começando com \\");

            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("A impressão de atributos literais reservados foi terminada.\n");
        }
    }
}
