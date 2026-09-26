using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using PassengerSchema;

namespace Passenger
{
    public class PipeLine
    {
        // Översätter all den data som matas in till ett format som ML kan använda för att träna modellen
        public static IEstimator<ITransformer> CreatePipeline(MLContext mlModel)
        {
            return mlModel
                .Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "SexEncoded",
                    inputColumnName: nameof(PassengerData.Sex)
                )
                .Append(
                    mlModel.Transforms.ReplaceMissingValues(
                        "AgeFilled",
                        nameof(PassengerData.Age),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlModel.Transforms.ReplaceMissingValues(
                        outputColumnName: "ParentChildFilled",
                        inputColumnName: nameof(PassengerData.Parch),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.DefaultValue
                    )
                )
                .Append(
                    mlModel.Transforms.ReplaceMissingValues(
                        outputColumnName: "SiblingSpouseFilled",
                        inputColumnName: nameof(PassengerData.SibSp),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.DefaultValue
                    )
                )
                .Append(
                    mlModel.Transforms.ReplaceMissingValues(
                        outputColumnName: "PclassFilled",
                        inputColumnName: nameof(PassengerData.Pclass),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlModel.Transforms.ReplaceMissingValues(
                        outputColumnName: "FareFilled",
                        inputColumnName: nameof(PassengerData.Fare),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlModel.Transforms.Concatenate(
                        "Features",
                        "AgeFilled",
                        "SexEncoded",
                        "ParentChildFilled",
                        "SiblingSpouseFilled",
                        "PclassFilled",
                        "FareFilled"
                    )
                )
                .Append(mlModel.Transforms.NormalizeMinMax("Features"))
                .Append(
                    mlModel.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: nameof(PassengerData.Survived),
                        featureColumnName: "Features"
                    )
                );
        }
    }
}
