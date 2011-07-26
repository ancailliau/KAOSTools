// 
// Obstacle.cs
//  
// Author:
//       Antoine Cailliau <antoine.cailliau@uclouvain.be>
// 
// Copyright (c) 2011 2011 Université Catholique de Louvain and Antoine Cailliau
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using Beaver.Likelihoods;
using System.Text.RegularExpressions;

namespace Beaver.Model
{
	public class Obstacle : KAOSElement
	{
		public string Name {
			get;
			set;
		}
		
		public string Definition {
			get;
			set;
		}
		
		public string Id {
			get;
			set;
		}
		
		public Likelihood Likelihood {
			get;
			set;
		}
		
		
		public double[] ComputedLikelihood {
			get;
			set;
		}
		
		public int NumSamples {
			get;
			set;
		}
		
		public Obstacle (string name, string definition, string likelihood)
			: this (name, definition, (Likelihood) null)
		{
			Likelihood = this.ParseLikelihood (likelihood);
		}
		
		public Obstacle (string name, string definition, Likelihood likelihood)
			: base ()
		{
			Id = Guid.NewGuid ().ToString ();
			Name = name;
			Definition = definition;
			Likelihood = likelihood;
			ComputedLikelihood = new double[MainClass.Controller.Configuration.NumBuckets];
		}
		
		public void SetLikelihood (string input)
		{
			Likelihood = this.ParseLikelihood (input);
		}
		
		private Likelihood ParseLikelihood (string input)
		{
			float probabilty;
			string patternUniform = @"[^\w]*\[[^\w]*([0-9\.]+)[^\w]*,[^\w]*([0-9\.]+)[^\w]*\][^\w]*";
			string patternTriangular = @"triangular[^\w]*\([^\w]*([0-9\.]+)[^\w]*,[^\w]*([0-9\.]+)[^\w]*,[^\w]*([0-9\.]+)[^\w]*\)[^\w]*";
			string patternPert = @"pert[^\w]*\([^\w]*([0-9\.]+)[^\w]*,[^\w]*([0-9\.]+)[^\w]*,[^\w]*([0-9\.]+)[^\w]*\)[^\w]*";
			var matchTriangular = Regex.Match (input, patternTriangular);
			var matchPert = Regex.Match (input, patternPert);
			var matchUniform = Regex.Match (input, patternUniform);
			
			if (float.TryParse (input, out probabilty)) {
				return new Probability(probabilty);
			} else if (matchTriangular.Success) {
				double min, mode, max;
				if (double.TryParse (matchTriangular.Groups [1].Value, out min)
					& double.TryParse (matchTriangular.Groups [2].Value, out mode)
					& double.TryParse (matchTriangular.Groups [3].Value, out max)) {
				return new Triangular (min, mode, max);
				}
			} else if (matchPert.Success) {
				double min, mode, max;
				if (double.TryParse (matchPert.Groups [1].Value, out min)
					& double.TryParse (matchPert.Groups [2].Value, out mode)
					& double.TryParse (matchPert.Groups [3].Value, out max)) {
				return new Pert (min, mode, max);
				}
			} else if (matchUniform.Success) {
				double min, max;
				if (double.TryParse (matchUniform.Groups [1].Value, out min)
					& double.TryParse (matchUniform.Groups [2].Value, out max)) {
				return new Uniform (min, max);
				}
			}
			Console.WriteLine ("Arg!");
			return null;
		}
		
		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof(Obstacle))
				return false;
			Beaver.Model.Obstacle other = (Beaver.Model.Obstacle)obj;
			return Id == other.Id;
		}


		public override int GetHashCode ()
		{
			unchecked {
				return (Id != null ? Id.GetHashCode () : 0);
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("[Obstacle: Name={0}]", Name);
		}


	}
}

