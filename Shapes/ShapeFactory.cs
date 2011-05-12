using System;
using Model;

namespace Shapes
{
	public class ShapeFactory
	{
		public static IShape Create(string id, IModelElement element)
		{
			if (element is Goal) {
				return new GoalShape(id, element as Goal);
			
			} else if (element is Refinement) {
				return new RefinementShape(id, element as Refinement);
				
			} else {
				return null;
			}
		}
	}
}
