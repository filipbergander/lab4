using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML.Data;

namespace PassengerSchema
{
    // Data med typer över varje passagerare
    public class PassengerData
    {
        [LoadColumn(1)]
        public bool Survived { get; set; }

        [LoadColumn(2)]
        public float Pclass { get; set; }

        [LoadColumn(4)]
        public string Sex { get; set; } = "";

        [LoadColumn(5)]
        public float Age { get; set; }

        [LoadColumn(6)]
        public float SibSp { get; set; }

        [LoadColumn(7)]
        public float Parch { get; set; }

        [LoadColumn(9)]
        public float Fare { get; set; }
    }

    // Det som modellen ska förutsäga, chans för överlevnad hos en passagerare
    public class PassengerPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Survived { get; set; }

        public float Probability { get; set; }

        public float Score { get; set; }
    }
}
