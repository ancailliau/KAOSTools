using System;
using KAOSTools.Core;
namespace KAOSTools.Parsing.Builders.Declarations
{
	public class ObstacleDeclareBuilder : DeclareBuilder
    {
		public ObstacleDeclareBuilder()
        {
        }

        public override void BuildDeclare(ParsedDeclare parsedElement, KAOSModel model)
        {
			Obstacle g = model.obstacleRepository.GetObstacle(parsedElement.Identifier);
			if (g == null)
			{
				g = new Obstacle(model, parsedElement.Identifier);
				model.obstacleRepository.Add(g);
			}
		}

		public override KAOSCoreElement GetBuiltElement(Parsing.ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.obstacleRepository.GetObstacle(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedObstacle;
		}
    }
}
