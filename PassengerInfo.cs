using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;

namespace PassengerInfo
{
    public class PassengerData
    {
        public bool Survived { get; set; }
        public float Pclass { get; set; }
        public string Sex { get; set; } = "";
        public float Age { get; set; }
        public float SibSp { get; set; }
        public float Parch { get; set; }
        public float Fare { get; set; }
    }

    public class PassengerPrediction
    {
        public bool Survived { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
