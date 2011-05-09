using System;
using Model;
using System.Collections.Generic;
using Shapes;
using Arrows;

namespace Editor
{
	public class Graph
	{
		
		public List<IShape> Shapes {
			get;
			set;
		}
		
		public List<FilledArrow> Arrows {
			get;
			set;
		}
		
		public Graph ()
		{
			Shapes = new List<IShape>();
			Arrows = new List<FilledArrow>();
		}
		
		public Graph (GoalModel model)
			: this()
		{
			var mapping = new Dictionary<string, Shape>();
			foreach (var g in model.Goals) {
				var shape = new RectangleShape() { Label = g.Name };
				Shapes.Add(shape);
				mapping.Add(g.Name, shape);
			}
			
			foreach (var g in model.Goals) {
				if (g.Children.Count > 0) {
					var refinementCircle = new CircleShape();
					Shapes.Add(refinementCircle);
					Arrows.Add(new FilledArrow() {
						Start = refinementCircle,
						End = mapping[g.Name]
					});
					foreach (var g2 in g.Children) {
						Arrows.Add(new FilledArrow() {
							Start = mapping[g2.Name],
							End = refinementCircle
						});
					}
				}
			}
		}
	}
}

