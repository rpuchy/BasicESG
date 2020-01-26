using DotNetMatrix;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicESG
{
    /// <summary>
    /// This method generates correlated standard normal shocks mean = 0, vol = 1
    /// </summary>

    public class ShockGenerator
    {

        private GeneralMatrix _correlationMatrix;
        private GeneralMatrix _L;
        private List<SimpleRNG> _randomNumberGenerators = new List<SimpleRNG>();

        public ShockGenerator(double[][] correlationMatrix)
        {
            _correlationMatrix = new GeneralMatrix(correlationMatrix);
            var cholDecomp = new CholeskyDecomposition(_correlationMatrix);
            _L = cholDecomp.GetL();
            var random = new Random();
            foreach(var row in _correlationMatrix.Array)
            {
                _randomNumberGenerators.Add(new SimpleRNG( random));//this is not optimal but probably ok for descriptive purposes
            }
        }

        public double[] GenerateCorrelatedShocks()
        {
            return  _L.Multiply(new GeneralMatrix(new double[][] { _randomNumberGenerators.Select(x => x.GetNormal()).ToArray() }).Transpose()).Array.Select(x=> x.First()).ToArray();
        }

        public void Reset()
        {
            _randomNumberGenerators.Clear();
            var random = new Random();
            foreach (var row in _correlationMatrix.Array)
            {
                _randomNumberGenerators.Add(new SimpleRNG(random));//this is not optimal but probably ok for descriptive purposes
            }
        }

    }
}
