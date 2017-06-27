using System;
using UCLouvain.KAOSTools.Core;
using UCLouvain.KAOSTools.Parsing.Parsers;
namespace UCLouvain.KAOSTools.Parsing.Builders.Declarations
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
			} else if (!parsedElement.Override) {
				throw new BuilderException("Cannot declare twice the same element. Use override instead.", parsedElement);
			}
		}

		public override KAOSCoreElement GetBuiltElement(ParsedDeclare parsedElement, KAOSModel model)
		{
			return model.obstacleRepository.GetObstacle(parsedElement.Identifier);
		}

		public override bool IsBuildable(ParsedDeclare element)
		{
            return element is ParsedObstacle;
		}
    }
}
