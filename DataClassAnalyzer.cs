using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TP3
{
    public class DataClassAnalyzer : NodeAnalyzer
    {
        public int GetterSetterMaxLineCount;
        public bool SetterHasReturnStament;
        private const int DefaultGetterLineCount = 5;
        private const bool DefaultSetterHasReturnStament = false;
        private readonly List<ClassDeclarationSyntax> _classesFound;
        private readonly List<ClassDeclarationSyntax> _dataClasses;

        public DataClassAnalyzer(string filePath) : base(filePath)
        {
            _classesFound = new List<ClassDeclarationSyntax>();
            _dataClasses = new List<ClassDeclarationSyntax>();
            GetterSetterMaxLineCount = DefaultGetterLineCount;
            SetterHasReturnStament = DefaultSetterHasReturnStament;
        }

        public override void Analyze()
        {
            _classesFound.Clear();
            _dataClasses.Clear();
            base.Analyze();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);
            _classesFound.Add(node);
        }

        protected override void OnVisitsFinished()
        {
            foreach (var classNode in _classesFound)
            {
                if (IsDataClass(classNode))
                    _dataClasses.Add(classNode);
            }

            base.OnVisitsFinished();
        }

        protected override void ShowProcessStartMessage()
        {
            base.ShowProcessStartMessage();
            Console.WriteLine($"Verificando se o arquivo em \"{CurrentFilePath}\" possui Data Classes...");
        }

        protected override void ShowResults()
        {
            if (_dataClasses.Count > 0)
            {
                Console.WriteLine("Uma ou mais Data Classes foram encontradas no arquivo:\n");

                foreach (var classDeclarationSyntax in _dataClasses)
                {
                    Console.WriteLine(classDeclarationSyntax.Identifier);
                }

                Console.Write("\nA(s) classe(s) listada(s) não possui(em) métodos além de construtores, getters e setters. ");
                Console.WriteLine("Considere sua remodelagem adicionando comportamentos próprios à esta.");
            }
            else
            {
                Console.WriteLine($"O arquivo em \"{CurrentFilePath}\" não possui Data Classes.");
            }

            base.ShowResults();
        }

        private bool IsDataClass(ClassDeclarationSyntax classNode)
        {
            var classVariableMembers = GetClassFieldVariables(classNode);
            var classMethods = classNode.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();

            if (ContainsBehaviourMethod(classMethods, classVariableMembers))
                return false;

            return true;
        }

        private bool ContainsBehaviourMethod(List<MethodDeclarationSyntax> methodNodes, List<VariableDeclarationSyntax> classVariableMembers)
        {
            if (HasLongMethod(methodNodes))
                return true;

            if (HasShortBehaviourMethod(methodNodes, classVariableMembers))
                return true;

            return false;
        }

        private bool HasShortBehaviourMethod(List<MethodDeclarationSyntax> methodNodes, List<VariableDeclarationSyntax> classVariableMembers)
        {
            foreach (var method in methodNodes)
            {
                if ((!IsGetter(method, classVariableMembers)) && (!IsSetter(method, classVariableMembers)))
                    return true;
            }

            return false;
        }

        private bool IsGetter(MethodDeclarationSyntax methodDeclarationNode, List<VariableDeclarationSyntax> classVariableMembers)
        {
            if (methodDeclarationNode.ReturnType.GetFirstToken().RawKind.Equals(SyntaxKind.VoidKeyword.GetHashCode()))
                return false;

            if (methodDeclarationNode.ParameterList.Parameters.Count > 0)
                return false;

            if (GetIntersectedNodesInBody<ExpressionStatementSyntax>(methodDeclarationNode).Any())
                return false;

            if (!HasSingleReturnStatement(methodDeclarationNode, out ReturnStatementSyntax returnNode))
                return false;

            if (!IsFieldMember(returnNode.Expression, classVariableMembers))
                return false;

            return true;
        }

        private bool IsSetter(MethodDeclarationSyntax methodDeclarationNode, List<VariableDeclarationSyntax> classVariableMembers)
        {
            if (methodDeclarationNode.ParameterList.Parameters.Count != 1)
                return false;

            if (!SetterHasReturnStament)
            {
                if (GetIntersectedNodesInBody<ReturnStatementSyntax>(methodDeclarationNode).Any())
                    return false;

                if (!methodDeclarationNode.ReturnType.GetFirstToken().RawKind.Equals(SyntaxKind.VoidKeyword.GetHashCode()))
                    return false;
            }

            AssignmentExpressionSyntax assignmentExpressionSyntax = null;
            int assignmentExpressionCount = 0;

            foreach (var assignmentExpression in GetIntersectedNodesInBody<AssignmentExpressionSyntax>(methodDeclarationNode))
            {
                if (!assignmentExpression.IsKind(SyntaxKind.SimpleAssignmentExpression))
                    return false;

                assignmentExpressionSyntax = assignmentExpression;
                assignmentExpressionCount++;

                if (assignmentExpressionCount > 1)
                    return false;
            }

            if (!IsFieldMember(assignmentExpressionSyntax?.Left, classVariableMembers))
                return false;

            return true;
        }

        private bool HasLongMethod(IEnumerable<MethodDeclarationSyntax> methodNodes)
        {
            foreach (var methodDeclarationSyntax in methodNodes)
            {
                if (!IsShortMethod(methodDeclarationSyntax))
                    return true;
            }

            return false;
        }

        private bool IsShortMethod(MethodDeclarationSyntax methodDeclarationNode)
        {
            return methodDeclarationNode.Body.GetText().Lines.Count <= GetterSetterMaxLineCount;
        }

        private bool IsFieldMember(ExpressionSyntax expressionSyntax, List<VariableDeclarationSyntax> classVariableMembers)
        {
            foreach (var member in classVariableMembers)
            {
                foreach (var variable in member.Variables)
                {
                    if (expressionSyntax.ToString().Equals(variable.Identifier.ToString()))
                        return true;
                }
            }

            return false;
        }

        private List<VariableDeclarationSyntax> GetClassFieldVariables(ClassDeclarationSyntax classNode)
        {
            var classVariableMembers = new List<VariableDeclarationSyntax>();

            foreach (var classFieldMember in classNode.DescendantNodes().OfType<FieldDeclarationSyntax>())
            {
                classVariableMembers.Add(classFieldMember.Declaration);
            }

            return classVariableMembers;
        }

        private bool HasSingleReturnStatement(MethodDeclarationSyntax methodDeclarationNode, out ReturnStatementSyntax returnNode)
        { 
            var returnNodes = GetIntersectedNodesInBody<ReturnStatementSyntax>(methodDeclarationNode).ToList();
            bool hasSingleReturnStatement = returnNodes.Count == 1;
            returnNode = hasSingleReturnStatement ? returnNodes[0] : null;
            return hasSingleReturnStatement;
        }
    }
}
