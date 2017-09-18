using System;
namespace UCLouvain.KAOSTools.Core.Goals
{
	public class GoalProvided : KAOSCoreElement
	{
		public string GoalIdentifier
		{
			get;
			set;
		}
		
		public string ObstacleIdentifier
		{
			get;
			set;
		}
		
		public Formula Formula
		{
			get;
			set;
		}
	
		public GoalProvided(KAOSModel model) : base(model)
		{
		}
		
		public GoalProvided(KAOSModel model, string goalIdentifier, string obstacleIdentifier, Formula formula)
			: base(model)
		{
			GoalIdentifier = goalIdentifier;
			ObstacleIdentifier = obstacleIdentifier;
			Formula = formula;
		}

		public override KAOSCoreElement Copy()
		{
			throw new NotImplementedException();
		}
	}
}
