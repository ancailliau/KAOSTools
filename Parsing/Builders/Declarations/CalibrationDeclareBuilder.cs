using System;
using KAOSTools.Core;
using KAOSTools.Parsing.Parsers;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class CalibrationDeclareBuilder : DeclareBuilder
    {
		public CalibrationDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Calibration g = model.modelMetadataRepository.GetCalibration(parsedElement.Identifier);
			if (g == null)
			{
				g = new Calibration(model, parsedElement.Identifier);
				model.modelMetadataRepository.Add(g);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.modelMetadataRepository.GetCalibration(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedCalibration;
		}
    }
}
