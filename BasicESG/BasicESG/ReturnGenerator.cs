using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicESG
{
    public class ReturnGenerator
    {
        public double[] Mean { get; set; }
        public double[] Volatility { get; set; }
        public double[][] CorrelationMatrix { get; set; }

        private ShockGenerator _shockGenerator;

        public ReturnGenerator()
        {
           
        }

        public ReturnGenerator(double[] mean, double[] volatility, double[][] correlations)
        {
            CorrelationMatrix = correlations;
            Mean = mean;
            Volatility = volatility;
        }
        //returns scenario, timesteps, continuously compounded return
        public Dictionary<int, Dictionary<int, List<double>>> Generate(int timeSteps, int scenarios)
        {
            _shockGenerator = new ShockGenerator(CorrelationMatrix);
            var result = new Dictionary<int, Dictionary<int, List<double>>>();
            for (int i=1;i<=scenarios;i++)
            {
                var scenarioDict = new Dictionary<int, List<double>>();
                for(int j=1;j<=timeSteps;j++)
                {
                    var assetList = new List<double>();
                    var shocks = _shockGenerator.GenerateCorrelatedShocks();
                    for(int k=0;k<Mean.Count();k++)
                    {
                        assetList.Add(Math.Exp(Mean[k] - 0.5 * Volatility[k] * Volatility[k] + Volatility[k] * shocks[k])-1); //this is a very simple continuous model
                    }
                    scenarioDict.Add(j, assetList);
                }
                result.Add(i, scenarioDict);
            }
            return result;
        }

    }
}
