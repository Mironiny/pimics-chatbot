using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graph;
using MathNet.Numerics.Statistics;

namespace PimBot.Dialogs.FindItem
{
    public enum FeatureType { Alphanumeric, Numeric, Logical };

    public class FeatureToAsk
    {
        public FeatureToAsk(string number, string description, int order)
        {
            Number = number;
            Description = description;
            Order = order;
        }

        public string Number { get; set; }

        public string Description { get; set; }

        public int Order { get; set; }


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
                    var intList = ValuesList.Select(int.Parse).ToList();
                    List<double> doubles = intList.Select<int, double>(i => i).ToList();
                    var median = doubles.Median();
                    return new List<string>()
                    {
                        $"to {median}",
                        $"from {median}",
                    };

                default:
                    return new List<string>();
            }
        }

        public double GetAvarageValue()
        {
            if (Type == FeatureType.Numeric)
            {
                var intList = ValuesList.Select(int.Parse).ToList();
                List<double> doubles = intList.Select<int, double>(i => i).ToList();
                var median = doubles.Median();
                return median;
            }

            return -1;
        }

        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }
}
