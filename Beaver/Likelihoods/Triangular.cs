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

namespace Beaver.Likelihoods
{
	public class Triangular : Likelihood
	{
		private double min = 0;
		private double mode = 0.5;
		private double max = 1;
		
		private Random r;
		
		private int nbStratification = 10;
		private bool[] stratification;
		private int count = 0;
		
		public Triangular (double min, double mode, double max)
		{
			this.min = min;
			this.mode = mode;
			this.max = max;
			
			r = new Random ();
			stratification = new bool[nbStratification];
			for (int i = 0; i < nbStratification; i++) {
				stratification[i] = false;
			}
		}
		
		public double GetSample ()
		{
			if (count == nbStratification) {
				for (int i = 0; i < nbStratification; i++) {
					stratification[i] = false;
				}
				count = 0;
			}
			count ++;
			
			var randomStratification = r.Next (nbStratification);
			while (stratification[randomStratification] == true) {
				randomStratification = (randomStratification + 1) % nbStratification;
			}
			stratification[randomStratification] = true;
			
			var minvalue = ( 1f / nbStratification ) * randomStratification;
			var maxvalue = ( 1f / nbStratification ) * ( randomStratification + 1);
			
			var randomValue = r.NextDouble () * (maxvalue - minvalue) + minvalue;
			
			if (randomValue < (mode - min)/(max-min)) {
				return min + Math.Sqrt (randomValue * (max - min) * (mode - min));
			} else {
				return max - Math.Sqrt ((1 - randomValue) * (max - min) * (max - mode));
			}
		}
		
		public override string ToString ()
		{
			return string.Format ("triangular({0},{1},{2})", min, mode, max);
		}

	}
}

