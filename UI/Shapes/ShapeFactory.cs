using KaosEditor.Model;

namespace Shapes
{
	public class ShapeFactory
	{
		public static IShape Create(IModelElement element)
		{
			if (element is Goal) {
				return new GoalShape(element as Goal);
			
			} else if (element is Refinement) {
				return new RefinementShape(element as Refinement);
			
			} else if (element is Agent) {
				return new AgentShape(element as Agent);
			
			} else {
				return null;
			}
		}
	}
}
