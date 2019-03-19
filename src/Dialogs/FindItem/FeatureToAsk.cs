using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MathNet.Numerics.Statistics;
using Microsoft.Graph;
using PimBot.State;

namespace PimBot.Dialogs.FindItem
{
    public enum FeatureType { Alphanumeric, Numeric, Logical };

    public class FeatureToAsk
    {
        public FeatureToAsk(string number, string description, int order, string unit)
        {
            Number = number;
            Description = description;
            Order = order;
            Unit = unit;
        }

        public string Number { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }

        public string Unit { get; set; }

        public double InformationGain { get; set; }

        public HashSet<string> ValuesList { get; set; }

        public FeatureType Type { get; set; }

        public void SetAndCheckType()
        {
            var isOnlyDigital = ValuesList.All(x => IsDigitsOnly(x));
            if (isOnlyDigital)
            {
                Type = FeatureType.Numeric;
            }
            else
            {
                Type = FeatureType.Alphanumeric;
            }
        }

        public List<string> GetPrintableValues()
        {
            switch (Type)
            {
                case FeatureType.Alphanumeric:
                    return ValuesList.ToList();

                case FeatureType.Numeric:
                    var median = GetMedianValue();
                    return new List<string>()
                    {
                        $"to {median} {(Type == FeatureType.Numeric ? Unit : string.Empty)}",
                        $"from {median} {(Type == FeatureType.Numeric ? Unit : string.Empty)}",
                    };

                default:
                    return new List<string>();
            }
        }

        public double GetMedianValue()
        {
            if (Type == FeatureType.Numeric)
            {
                var intList = ValuesList.Select(x => double.Parse(x, CultureInfo.InvariantCulture)).ToList();
              //  List<double> doubles = intList.Select<int, double>(i => i).ToList();
                var median = intList.Median();
                return median;
            }

            return -1;
        }

        private bool IsDigitsOnly(string str)
        {
            return double.TryParse(str, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out double unusedValue);
        }

        /// <summary>
        /// Calculate information gain based on entropy.
        /// </summary>
        /// <param name="items"></param>
        public void ComputeInformationGain(List<PimItem> items)
        {
            // Calculate enthropy of target
            double targetEntropy = 0;
            foreach (var item in items)
            {
                targetEntropy += (1 / (double)items.Count()) * Math.Log(1 / (double)items.Count(), 2);
            }

            targetEntropy *= -1;

            // Calculate entropy of element
            if (Type == FeatureType.Alphanumeric)
            {
                double fullEntropy = 0;

                foreach (var feature in ValuesList)
                {
                    // How many items there are with certain value
                    var count = 0;
                    foreach (var item in items)
                    {
                        // Count of 
                        count += item.PimFeatures.Where(i => i.Number == Number && i.Value == feature).Count();
                    }

                    // OJEB (nevim jeslti funguje)
                    double entrop = 0;
                    for (int i = 0; i < count; i++)
                    {
                        entrop += (1 / (double)count) * Math.Log(1 / (double)count, 2);
                    }

                    entrop *= -1;
                    fullEntropy += (count / (double)items.Count) * entrop;
                }

                InformationGain = targetEntropy - fullEntropy;
            }
            else if (Type == FeatureType.Numeric)
            {
                var median = GetMedianValue();

                // How many items there are with certain value
                var countLowerEqualThanAverage = 0;
                var countGreaterThanAverage = 0;

                foreach (var item in items)
                {
                    if (item.PimFeatures != null && item.PimFeatures.Count() > 0)
                    {
                        var x = item.PimFeatures
                            .Where(i => i.Number == Number && IsDigitsOnly(i.Value) && i.Value != string.Empty)
                            .ToList();

                        if (x.Count() > 0)
                        {
                            countLowerEqualThanAverage += Convert.ToDouble(x[0].Value) <= median ? 1 : 0;
                            countGreaterThanAverage += Convert.ToDouble(x[0].Value) > median ? 1 : 0;
                        }
                    }
                }

                double entrop1 = 0;
                double entrop2 = 0;

                for (int i = 0; i < countLowerEqualThanAverage; i++)
                {
                    entrop1 += (1 / (double)countLowerEqualThanAverage) * Math.Log(1 / (double)countLowerEqualThanAverage, 2);
                }

                entrop1 *= -1;

                for (int i = 0; i < countGreaterThanAverage; i++)
                {
                    entrop2 += (1 / (double)countGreaterThanAverage) * Math.Log(1 / (double)countGreaterThanAverage, 2);
                }

                entrop2 *= -1;

                var fullEntropy = ((countLowerEqualThanAverage / (double)items.Count) * entrop1) + ((countGreaterThanAverage / (double)items.Count) * entrop2);

                InformationGain = targetEntropy - fullEntropy;
            }
        }
    }
}
