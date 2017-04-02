using System;
using System.Collections.Generic;
using System.Linq;

namespace NoiseMapper.Tools
{
    class Acoustics
    {
        internal static double Log10Sum(List<double> levelCollection)
        {
            List<double> antiLogCollection = new List<double>(levelCollection.Count);

            foreach (var level in levelCollection)
            {
                antiLogCollection.Add(AntiLog10(level));
            }

            double logSum = 10 * (double)Math.Log10(antiLogCollection.Sum());
            return logSum;
        }

        internal static double AntiLog10(double level)
        {
            double antiLog = (double)Math.Pow(10, 0.1 * level);
            return antiLog;
        }

    }
}
