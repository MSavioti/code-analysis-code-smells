using System;
using System.Collections.Generic;

namespace TP3
{
    public class CodeSmellsAnalyzer
    {
        public enum AnalysisType
        {
            MethodLength, ParameterCount, DataClass, MagicAttributes
        }

        public void ExecuteFullAnalysis(string filePath, bool changeDefaultValues = false)
        {
            Console.WriteLine("_______________________");
            Console.WriteLine("| INICIANDO OPERAÇÕES |");
            Console.WriteLine("_______________________");

            List<NodeAnalyzer> analyzers = InstantiateAllAnalyzers(filePath);

            if (changeDefaultValues)
                UpdateAnalyzersValues(analyzers);

            foreach (NodeAnalyzer analyzer in analyzers)
            {
                analyzer.Analyze();
            }
        }

        public void ExecuteSingleAnalysis(string filePath, AnalysisType analysisType, bool changeDefaultValues = false)
        {

        }


        private void UpdateAnalyzersValues(List<NodeAnalyzer> analyzers)
        {
            foreach (NodeAnalyzer analyzer in analyzers)
            {
                analyzer.UpdateAnalyzerValues();
            }
        }

        private List<NodeAnalyzer> InstantiateAllAnalyzers(string filePath)
        {
            return new List<NodeAnalyzer>
            {
                new MethodLengthAnalyzer(filePath),
                new ParameterCountAnalyzer(filePath),
                new DataClassAnalyzer(filePath),
                new MagicAttributesAnalyzer(filePath)
            };
        }
    }
}
