using System;
using Model;

namespace Shapes
{
	public class ShapeFactory
	{
		public static IShape Create(IModelElement element)
		{
			if (element is Goal) {
				return new GoalShape(element as Goal);
				
			} else {
				return null;
			}
		}
	}
}
