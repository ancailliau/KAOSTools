// 
// Triangular.cs
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
using VisualRegression;

namespace Beaver.Likelihoods
{
	public class Pert : Likelihood
	{
		private double min = 0;
		private double mode = 0.5;
		private double max = 1;
		
		private double mean;
		private double alpha1;
		private double alpha2;
		
		private Random r;
		
		public Pert (double min, double mode, double max)
		{
			this.min = min;
			this.mode = mode;
			this.max = max;
			
			r = new Random ();
			
			mean = (min + 4 * mode + max) / 6;
			alpha1 = 6 * ( mean - min ) / (max - min);
			alpha2 = 6 * ( max - mean ) / (max - min);
		}
		
		public double GetSample ()
		{
			var randomValue = r.NextDouble ();
			return Statistics.IINCBETA(alpha1, alpha2, randomValue * Statistics.INCBETA (alpha1, alpha2, 1)) * ( max - min ) + min;
		}
		
		public override string ToString ()
		{
			return string.Format ("triangular({0},{1},{2})", min, mode, max);
		}

	}
}

