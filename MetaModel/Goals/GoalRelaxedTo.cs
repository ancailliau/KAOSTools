using System;
namespace UCLouvain.KAOSTools.Core.Goals
{
	public class GoalRelaxedTo : KAOSCoreElement
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
	
		public GoalRelaxedTo(KAOSModel model) : base(model)
		{
		}
		
		public GoalRelaxedTo(KAOSModel model, string goalIdentifier, string obstacleIdentifier, Formula formula)
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
