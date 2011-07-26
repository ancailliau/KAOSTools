/*
 * Most of this code has been adapted by John Hurliman to C# from the Cephes C 
 * library, which is authored and maintained by Stephen L. Moshier. The 
 * original Cephes license is reproduced below. Original portions of this code 
 * have been documented with the new author and are copyright of the noted 
 * author.
 * 
 * -----------------------------------------------------------------------
 * Some software in this archive may be from the book Methods and Programs
 * for Mathematical Functions (Prentice-Hall, 1989) or from the Cephes
 * Mathematical Library, a commercial product. In either event, it is
 * copyrighted by the author.  What you see here may be used freely
 * but it comes with no support or guarantee.
 * -----------------------------------------------------------------------
 */

using System;

namespace VisualRegression
{
    public class Statistics
    {
        public const double MaxNum = 1.7976931348623158E308;
        public const double MaxGam = 171.624376956302725;
        public const double MaxLog = 7.09782712893383996843E2;
        public const double MinLog = -7.08396418532264106224E2;
        public const double MaxLgm = 2.556348e305;
        public const double LnPI = 1.14472988584940017414;
        public const double S2PI = 2.50662827463100050242E0;
        public const double LS2PI = 0.91893853320467274178;
        public const double Big = 4.503599627370496E15;
        public const double BigInv = 2.22044604925031308085E-16;

        private static double[] A = {
             8.11614167470508450300E-4,
            -5.95061904284301438324E-4,
             7.93650340457716943945E-4,
            -2.77777777730099687205E-3,
             8.33333333333331927722E-2
        };
        private static double[] B = {
            -1.37825152569120859100E3,
            -3.88016315134637840924E4,
            -3.31612992738871184744E5,
            -1.16237097492762307383E6,
            -1.72173700820839662146E6,
            -8.53555664245765465627E5
        };
        private static double[] C = {
            -3.51815701436523470549E2,
            -1.70642106651881159223E4,
            -2.20528590553854454839E5,
            -1.13933444367982507207E6,
            -2.53252307177582951285E6,
            -2.01889141433532773231E6
        };

        // approximation for 0 <= |y - 0.5| <= 3/8
        private static double[] P0 = {
            -5.99633501014107895267E1,
             9.80010754185999661536E1,
            -5.66762857469070293439E1,
             1.39312609387279679503E1,
            -1.23916583867381258016E0,
        };
        private static double[] Q0 = {
             1.95448858338141759834E0,
             4.67627912898881538453E0,
             8.63602421390890590575E1,
            -2.25462687854119370527E2,
             2.00260212380060660359E2,
            -8.20372256168333339912E1,
             1.59056225126211695515E1,
            -1.18331621121330003142E0,
        };

        // Approximation for interval z = sqrt(-2 log y ) between 2 and 8
        // i.e., y between exp(-2) = .135 and exp(-32) = 1.27e-14
        private static double[] P1 = {
         4.05544892305962419923E0,
         3.15251094599893866154E1,
         5.71628192246421288162E1,
         4.40805073893200834700E1,
         1.46849561928858024014E1,
         2.18663306850790267539E0,
        -1.40256079171354495875E-1,
        -3.50424626827848203418E-2,
        -8.57456785154685413611E-4,
        };
        private static double[] Q1 = {
         1.57799883256466749731E1,
         4.53907635128879210584E1,
         4.13172038254672030440E1,
         1.50425385692907503408E1,
         2.50464946208309415979E0,
        -1.42182922854787788574E-1,
        -3.80806407691578277194E-2,
        -9.33259480895457427372E-4,
        };

        // Approximation for interval z = sqrt(-2 log y ) between 8 and 64
        // i.e., y between exp(-32) = 1.27e-14 and exp(-2048) = 3.67e-890
        private static double[] P2 = {
          3.23774891776946035970E0,
          6.91522889068984211695E0,
          3.93881025292474443415E0,
          1.33303460815807542389E0,
          2.01485389549179081538E-1,
          1.23716634817820021358E-2,
          3.01581553508235416007E-4,
          2.65806974686737550832E-6,
          6.23974539184983293730E-9,
        };
        private static double[] Q2 = {
          6.02427039364742014255E0,
          3.67983563856160859403E0,
          1.37702099489081330271E0,
          2.16236993594496635890E-1,
          1.34204006088543189037E-2,
          3.28014464682127739104E-4,
          2.89247864745380683936E-6,
          6.79019408009981274425E-9,
        };

        /// <summary>
        /// The smallest number which when added to 1 gives something different than 1
        /// </summary>
        /// <returns>The machine epsilon, for floating point calculations</returns>
        public static double MachineEpsilon()
        {
            double eps = 1.0D;
            while (1.0D + (eps /= 2.0D) > 1.0D);
            return 2.0D * eps;

            //!!! These extra values are left here in case of future performance 
            // bottlenecks, they can be used as static values for the epsilon

            // Double.Epsilon seems optimistically small compared to manually 
            // calculating the epsilon
            //return Double.Epsilon;

            // The above routine calculates the following value on my machine
            //return 0.00000000000000022204460492503131D;

            // We could also use this safer value to take in to account less 
            // accurate processors
            //return 0.0000000000001D;
        }

        /// <summary>
        /// Evaluates a polynomial of degree N
        /// </summary>
        /// <param name="x">Main variant</param>
        /// <param name="coef">Array of coefficients in reverse order</param>
        /// <param name="N">
        /// Degree of the polynomial, also one less than the number of coefficients. Must be >= 1
        /// </param>
        /// <returns>Solution of the polynomial</returns>
        public static double PolyEval(double x, double[] coef, int N)
        {
            if (N < 1) { return 0.0D; }

            double ans = coef[0];
            int i = 1;

            do
            {
                ans = ans * x + coef[i];
                i++;
            } while (--N > 0);

            return ans;
        }

        /// <summary>
        /// Evaluates a polynomial of degree N, where coef[N] = 1
        /// </summary>
        /// <param name="x">Main variant</param>
        /// <param name="coef">Array of coefficients in reverse order</param>
        /// <param name="N">Degree of the polynomial, also the number of coefficients. Must be >= 2</param>
        /// <returns>Solution of the polynomial</returns>
        public static double PolyEval1(double x, double[] coef, int N)
        {
            if (N < 2) { return 0.0D; }

            double ans = x + coef[0];
            int i = 1;
            N--;

            do
            {
                ans = ans * x + coef[i];
                i++;
            } while (--N > 0);

          return ans;
        }

        /// <summary>
        /// Gamma function of x
        /// </summary>
        /// <param name="x">Value at which to evaluate the function</param>
        /// <returns>gamma(x)</returns>
        public static double Gamma(double x)
        {
            double[] P = {
                1.60119522476751861407E-4,
                1.19135147006586384913E-3,
                1.04213797561761569935E-2,
                4.76367800457137231464E-2,
                2.07448227648435975150E-1,
                4.94214826801497100753E-1,
                9.99999999999999996796E-1
            };
            double[] Q = {
                -2.31581873324120129819E-5,
                5.39605580493303397842E-4,
                -4.45641913851797240494E-3,
                1.18139785222060435552E-2,
                3.58236398605498653373E-2,
                -2.34591795718243348568E-1,
                7.14304917030273074085E-2,
                1.00000000000000000320E0
            };

            double p, q, z;

            q = Math.Abs(x);
            z = 1.0;

            while (x >= 3.0)
            {
                x -= 1.0;
                z *= x;
            }

            while (x < 0.0)
            {
                if (x > -1E-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }

                z /= x;
                x += 1.0;
            }

            while (x < 2.0)
            {
                if (x < 1e-9)
                {
                    return (z / ((1.0 + 0.5772156649015329 * x) * x));
                }

                z /= x;
                x += 1.0;
            }

            if (x == 2.0)
            {
                return (z);
            }

            x -= 2.0;
            p = PolyEval(x, P, 6);
            q = PolyEval(x, Q, 7);

            return (z * p / q);
        }

        //!!! Leaving this here in case of future performance bottlenecks, 
        // this can be used as a faster alternative.
        //
        ///// <summary>
        ///// Simple gamma function, will give a close approximation
        ///// </summary>
        ///// <param name="x">Value at which to evaluate the function</param>
        ///// <returns>Result of the gamma function given x</returns>
        //public static double Gamma(double x)
        //{
        //    double s = (2.50662827510730 +
        //                190.9551718930764D /
        //                (x + 1D) - 216.8366818437280D /
        //                (x + 2D) + 60.19441764023333D /
        //                (x + 3D) - 3.08751323928546D /
        //                (x + 4D) + 0.00302963870525D /
        //                (x + 5D) - 0.00001352385959072596D /
        //                (x + 6D)) / x;

        //    if (s < 0) { return -Math.Exp((x + 0.5D) * Math.Log(x + 5.5D) - x - 5.5 + Math.Log(-s)); }
        //    else { return Math.Exp((x + 0.5D) * Math.Log(x + 5.5D) - x - 5.5 + Math.Log(s)); }
        //}

        /// <summary>
        /// Logarithm of the gamma function of x
        /// </summary>
        /// <param name="x">Value at which to evaluate the function</param>
        /// <returns>Result of the logarithm of gamma(x)</returns>
        public static double LNGamma(double x)
        {
            double p, q, u, w, z;
            int i;
            int sgngam = 1;

            if (x < -34.0D)
	        {
	            q = -x;
	            w = LNGamma(q);
	            p = Math.Floor(q);

	            if (p == q) { return (sgngam * MaxNum); }

	            i = (int)p;

	            if ((i & 1) == 0) { sgngam = -1; }
                else { sgngam = 1; }

	            z = q - p;

	            if (z > 0.5)
		        {
		            p += 1.0D;
		            z = p - q;
		        }

	            z = q * Math.Sin(Math.PI * z);
	            
                if (z == 0.0D) { return (sgngam * MaxNum); }

	            z = LnPI - Math.Log(z) - w;
	            return z;
	        }

            if(x < 13.0D)
	        {
	            z = 1.0;
	            p = 0.0;
	            u = x;

	            while (u >= 3.0D)
		        {
		            p -= 1.0;
		            u = x + p;
		            z *= u;
		        }

	            while (u < 2.0D)
		        {
                    if (u == 0.0D) { return (sgngam * MaxNum); }
		            z /= u;
		            p += 1.0D;
		            u = x + p;
		        }

                if (z < 0.0D)
                {
                    sgngam = -1;
                    z = -z;
                }
                else
                {
                    sgngam = 1;
                }

                if (u == 2.0D) { return Math.Log(z); }

	            p -= 2.0D;
	            x = x + p;
	            p = x * PolyEval(x, B, 5) / PolyEval1(x, C, 6);

	            return (Math.Log(z) + p);
	        }

            if(x > MaxLgm) { return (sgngam * MaxNum); }

            q = (x - 0.5D) * Math.Log(x) - x + LS2PI;

            if (x > 1.0e8D) { return q; }

            p = 1.0D / (x*x);

            if (x >= 1000.0)
            {
                q += ((7.9365079365079365079365e-4D * p
                    - 2.7777777777777777777778e-3D) * p
                    + 0.0833333333333333333333D) / x;
            }
            else
            {
                q += PolyEval(p, A, 4) / x;
            }

            return q;
        }

        /// <summary>
        /// Inverse of the Normal distribution function
        /// </summary>
        /// <param name="y0">Value at which to evaluate the function</param>
        /// <returns>
        /// The argument x for which the area under the Gaussian probability 
        /// density function (integrated from minus infinity to x) is equal to y0
        /// </returns>
        public static double NDTRI(double y0)
        {
            double x, y, z, y2, x0, x1;
            int code;

            if (y0 <= 0.0) { return -MaxNum; }
            if (y0 >= 1.0) { return MaxNum; }

            code = 1;
            y = y0;

            if (y > (1.0D - 0.13533528323661269189D)) // 0.135... = Math.Exp(-2)
            {
                y = 1.0D - y;
                code = 0;
            }

            if (y > 0.13533528323661269189D)
            {
                y = y - 0.5D;
                y2 = y * y;
                x = y + y * (y2 * PolyEval(y2, P0, 4) / PolyEval1(y2, Q0, 8));
                x = x * S2PI;

                return (x);
            }

            x = Math.Sqrt(-2.0 * Math.Log(y));
            x0 = x - Math.Log(x) / x;

            z = 1.0 / x;

            if (x < 8.0) { x1 = z * PolyEval(z, P1, 8) / PolyEval1(z, Q1, 8); }
            else { x1 = z * PolyEval(z, P2, 8) / PolyEval1(z, Q2, 8); }

            x = x0 - x1;

            if (code != 0) { x = -x; }

            return x;
        }

        /// <summary>
        /// Calculate the incomplete beta integral
        /// </summary>
        /// <param name="aa">First number of degrees of freedom, must be positive</param>
        /// <param name="bb">Second number of degrees of freedom, must be positive</param>
        /// <param name="xx">Value at which to evaluate the function, in the range of 0 to 1</param>
        /// <returns>Result of the incomplete beta integral</returns>
        public static double INCBETA(double aa, double bb, double xx)
        {
            double a, b, t, x, xc, w, y;
            int flag;
            double MACHEP = MachineEpsilon();

            if (aa <= 0.0 || bb <= 0.0) { return 0.0; }

            if ((xx <= 0.0) || ( xx >= 1.0))
	        {
                if (xx == 0.0) { return 0.0; }
                if (xx == 1.0) { return 1.0; }

	            return 0.0;
	        }

            flag = 0;

            if ((bb * xx) <= 1.0 && xx <= 0.95)
	        {
	            t = PSeries(aa, bb, xx);
		        goto done;
	        }

            w = 1.0 - xx;

            // Reverse a and b if x is greater than the mean.
            if (xx > (aa/(aa+bb)))
	        {
	            flag = 1;
	            a = bb;
	            b = aa;
	            xc = xx;
	            x = w;
	        }
            else
	        {
	            a = aa;
	            b = bb;
	            xc = w;
	            x = xx;
	        }

            if (flag == 1 && (b * x) <= 1.0 && x <= 0.95)
	        {
	            t = PSeries(a, b, x);
	            goto done;
	        }

            // Choose expansion for better convergence.
            y = x * (a + b - 2.0) - ( a - 1.0);
            if (y < 0.0) { w = INCBETACF(a, b, x); }
            else { w = INCBETAD(a, b, x) / xc; }

            y = a * Math.Log(x);
            t = b * Math.Log(xc);

            if ((a + b) < MaxGam && Math.Abs(y) < MaxLog && Math.Abs(t) < MaxLog)
	        {
	            t = Math.Pow(xc, b);
                t *= Math.Pow(x, a);
	            t /= a;
	            t *= w;
	            t *= Gamma(a + b) / (Gamma(a) * Gamma(b));
	            goto done;
	        }

            // Resort to logarithms
            y += t + LNGamma(a + b) - LNGamma(a) - LNGamma(b);
            y += Math.Log(w/a);
            if (y < MinLog) { t = 0.0; }
            else { t = Math.Exp(y); }

done:
            if (flag == 1)
	        {
                if (t <= MACHEP) { t = 1.0 - MACHEP; }
                else { t = 1.0 - t; }
	        }

            return t;
        }

        /// <summary>
        /// Continued fraction expansion #1 for incomplete beta integral
        /// </summary>
        /// <param name="a">First number of degrees of freedom, must be positive</param>
        /// <param name="b">Second number of degrees of freedom, must be positive</param>
        /// <param name="x">Value at which to evaluate the function, range from 0 to 1</param>
        /// <returns>Result of the incomplete beta integral</returns>
        protected static double INCBETACF(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, thresh;
            int n;
            double MACHEP = MachineEpsilon();

            k1 = a;
            k2 = a + b;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = b - 1.0;
            k7 = k4;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {

                xk = -(x * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (x * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0)
                    r = pk / qk;
                if (r != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh)
                    goto cdone;

                k1 += 1.0;
                k2 += 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 -= 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)) > Big)
                {
                    pkm2 *= BigInv;
                    pkm1 *= BigInv;
                    qkm2 *= BigInv;
                    qkm1 *= BigInv;
                }
                if ((Math.Abs(qk) < BigInv) || (Math.Abs(pk) < BigInv))
                {
                    pkm2 *= Big;
                    pkm1 *= Big;
                    qkm2 *= Big;
                    qkm1 *= Big;
                }
            }
            while (++n < 300);

        cdone:
            return (ans);
        }

        /// <summary>
        /// Continued fraction expansion #2 for incomplete beta integral
        /// </summary>
        /// <param name="a">First number of degrees of freedom, must be positive</param>
        /// <param name="b">Second number of degrees of freedom, must be positive</param>
        /// <param name="x">Value at which to evaluate the function, range from 0 to 1</param>
        /// <returns>Result of the incomplete beta integral</returns>
        protected static double INCBETAD(double a, double b, double x)
        {
            double xk, pk, pkm1, pkm2, qk, qkm1, qkm2;
            double k1, k2, k3, k4, k5, k6, k7, k8;
            double r, t, ans, z, thresh;
            int n;
            double MACHEP = MachineEpsilon();

            k1 = a;
            k2 = b - 1.0;
            k3 = a;
            k4 = a + 1.0;
            k5 = 1.0;
            k6 = a + b;
            k7 = a + 1.0; ;
            k8 = a + 2.0;

            pkm2 = 0.0;
            qkm2 = 1.0;
            pkm1 = 1.0;
            qkm1 = 1.0;
            z = x / (1.0 - x);
            ans = 1.0;
            r = 1.0;
            n = 0;
            thresh = 3.0 * MACHEP;
            do
            {

                xk = -(z * k1 * k2) / (k3 * k4);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                xk = (z * k5 * k6) / (k7 * k8);
                pk = pkm1 + pkm2 * xk;
                qk = qkm1 + qkm2 * xk;
                pkm2 = pkm1;
                pkm1 = pk;
                qkm2 = qkm1;
                qkm1 = qk;

                if (qk != 0)
                    r = pk / qk;
                if (r != 0)
                {
                    t = Math.Abs((ans - r) / r);
                    ans = r;
                }
                else
                    t = 1.0;

                if (t < thresh)
                    goto cdone;

                k1 += 1.0;
                k2 -= 1.0;
                k3 += 2.0;
                k4 += 2.0;
                k5 += 1.0;
                k6 += 1.0;
                k7 += 2.0;
                k8 += 2.0;

                if ((Math.Abs(qk) + Math.Abs(pk)) > Big)
                {
                    pkm2 *= BigInv;
                    pkm1 *= BigInv;
                    qkm2 *= BigInv;
                    qkm1 *= BigInv;
                }
                if ((Math.Abs(qk) < BigInv) || (Math.Abs(pk) < BigInv))
                {
                    pkm2 *= Big;
                    pkm1 *= Big;
                    qkm2 *= Big;
                    qkm1 *= Big;
                }
            }
            while (++n < 300);
        cdone:
            return (ans);
        }

        /// <summary>
        /// Power series for the incomplete beta integral
        /// </summary>
        /// <param name="a">Unknown</param>
        /// <param name="b">Unknown</param>
        /// <param name="x">Value at which to evaluate the function</param>
        /// <returns>Unknown</returns>
        protected static double PSeries(double a, double b, double x)
        {
            double s, t, u, v, n, t1, z, ai;
            double MACHEP = MachineEpsilon();

            ai = 1.0 / a;
            u = (1.0 - b) * x;
            v = u / (a + 1.0);
            t1 = v;
            t = u;
            n = 2.0;
            s = 0.0;
            z = MACHEP * ai;

            while (Math.Abs(v) > z)
            {
                u = (n - b) * x / n;
                t *= u;
                v = t / (a + n);
                s += v;
                n += 1.0;
            }

            s += t1;
            s += ai;

            u = a * Math.Log(x);

            if ((a + b) < MaxGam && Math.Abs(u) < MaxLog)
            {
                t = Gamma(a + b) / (Gamma(a) * Gamma(b));
                s = s * t * Math.Pow(x, a);
            }
            else
            {
                t = LNGamma(a + b) - LNGamma(a) - LNGamma(b) + u + Math.Log(s);
                if (t < MinLog)
                    s = 0.0;
                else
                    s = Math.Exp(t);
            }

            return (s);
        }

        /// <summary>
        /// Inverse of the incomplete beta
        /// </summary>
        /// <param name="aa">Unknown</param>
        /// <param name="bb">Unknown</param>
        /// <param name="yy0">Value between 0 and 1</param>
        /// <returns>The result of the inverse incomplete beta</returns>
        public static double IINCBETA(double aa, double bb, double yy0)
        {
            double a, b, y0, d, y, x, x0, x1, lgm, yp, di, dithresh, yl, yh, xt;
            int i, rflg, dir, nflg;
            double MACHEP = MachineEpsilon();

            i = 0;

            if (yy0 <= 0) { return 0.0D; }
            if (yy0 >= 1.0) { return 1.0D; }

            x0 = 0.0D;
            yl = 0.0D;
            x1 = 1.0D;
            yh = 1.0D;
            nflg = 0;

            if (aa <= 1.0D || bb <= 1.0D)
            {
                dithresh = 1.0e-6D;
                rflg = 0;
                a = aa;
                b = bb;
                y0 = yy0;
                x = a / (a + b);
                y = INCBETA(a, b, x);
                goto ihalve;
            }
            else
            {
                dithresh = 1.0e-4D;
            }

            // approximation to inverse function
            yp = -NDTRI(yy0);

            if (yy0 > 0.5D)
            {
                rflg = 1;
                a = bb;
                b = aa;
                y0 = 1.0D - yy0;
                yp = -yp;
            }
            else
            {
                rflg = 0;
                a = aa;
                b = bb;
                y0 = yy0;
            }

            lgm = (yp * yp - 3.0D) / 6.0D;
            x = 2.0D / (1.0D / (2.0D * a - 1.0D) + 1.0D / (2.0D * b - 1.0D));
            d = yp * Math.Sqrt(x + lgm) / x
                - (1.0D / (2.0D * b - 1.0D) - 1.0D / (2.0D * a - 1.0D))
                * (lgm + 5.0D / 6.0D - 2.0D / (3.0D * x));
            d = 2.0D * d;

            if (d < MinLog)
            {
                x = 0.0D;
                goto done;
            }

            x = a / (a + b * Math.Exp(d));
            y = INCBETA(a, b, x);
            yp = (y - y0) / y0;

            if (Math.Abs(yp) < 0.2) { goto newt; }

            // Resort to interval halving if not close enough.
ihalve:
            dir = 0;
            di = 0.5D;
            for (i = 0; i < 100; i++)
            {
                if (i != 0)
                {
                    x = x0 + di * (x1 - x0);

                    if (x == 1.0D) { x = 1.0 - MACHEP; }
                    if (x == 0.0D)
                    {
                        di = 0.5D;
                        x = x0 + di * (x1 - x0);
                        if (x == 0.0)
                        {
                            x = 0.0D;
                            goto done;
                        }
                    }

                    y = INCBETA(a, b, x);
                    yp = (x1 - x0) / (x1 + x0);
                    if (Math.Abs(yp) < dithresh) { goto newt; }
                    yp = (y - y0) / y0;
                    if (Math.Abs(yp) < dithresh) { goto newt; }
                }

                if (y < y0)
                {
                    x0 = x;
                    yl = y;

                    if (dir < 0)
                    {
                        dir = 0;
                        di = 0.5D;
                    }
                    else if (dir > 3)
                    {
                        di = 1.0D - (1.0D - di) * (1.0D - di);
                    }
                    else if (dir > 1)
                    {
                        di = 0.5D * di + 0.5D;
                    }
                    else
                    {
                        di = (y0 - y) / (yh - yl);
                    }

                    dir += 1;

                    if (x0 > 0.75D)
                    {
                        if (rflg == 1)
                        {
                            rflg = 0;
                            a = aa;
                            b = bb;
                            y0 = yy0;
                        }
                        else
                        {
                            rflg = 1;
                            a = bb;
                            b = aa;
                            y0 = 1.0D - yy0;
                        }

                        x = 1.0D - x;
                        y = INCBETA(a, b, x);
                        x0 = 0.0D;
                        yl = 0.0D;
                        x1 = 1.0D;
                        yh = 1.0D;

                        goto ihalve;
                    }
                }
                else
                {
                    x1 = x;

                    if (rflg == 1 && x1 < MACHEP)
                    {
                        x = 0.0D;
                        goto done;
                    }

                    yh = y;

                    if (dir > 0)
                    {
                        dir = 0;
                        di = 0.5D;
                    }
                    else if (dir < -3)
                    {
                        di = di * di;
                    }
                    else if (dir < -1)
                    {
                        di = 0.5D * di;
                    }
                    else
                    {
                        di = (y - y0) / (yh - yl);
                    }

                    dir -= 1;
                }
            }

            if (x0 >= 1.0D)
            {
                x = 1.0D - MACHEP;
                goto done;
            }

            if (x <= 0.0D)
            {
                x = 0.0D;
                goto done;
            }

newt:
            if (nflg != 0) { goto done; }

            nflg = 1;
            lgm = LNGamma(a + b) - LNGamma(a) - LNGamma(b);

            for (i = 0; i < 8; i++)
            {
                // Compute the function at this point
                if (i != 0) { y = INCBETA(a, b, x); }

                if (y < yl)
                {
                    x = x0;
                    y = yl;
                }
                else if (y > yh)
                {
                    x = x1;
                    y = yh;
                }
                else if (y < y0)
                {
                    x0 = x;
                    yl = y;
                }
                else
                {
                    x1 = x;
                    yh = y;
                }

                if (x == 1.0D || x == 0.0D) { break; }

                // Compute the derivative of the function at this point
                d = (a - 1.0D) * Math.Log(x) + (b - 1.0D) * Math.Log(1.0D - x) + lgm;

                if (d < MinLog) { goto done; }
                if (d > MaxLog) { break; }

                d = Math.Exp(d);

                // Compute the step to the next approximation of x.
                d = (y - y0) / d;
                xt = x - d;

                if (xt <= x0)
                {
                    y = (x - x0) / (x1 - x0);
                    xt = x0 + 0.5D * y * (x - x0);

                    if (xt <= 0.0D) { break; }
                }

                if (xt >= x1)
                {
                    y = (x1 - x) / (x1 - x0);
                    xt = x1 - 0.5D * y * (x1 - x);
                    if (xt >= 1.0D) { break; }
                }

                x = xt;

                if (Math.Abs(d / x) < 128.0 * MACHEP) { goto done; }
            }

            // Did not converge
            dithresh = 256.0D * MACHEP;
            goto ihalve;

done:
            if (rflg != 0)
            {
                if (x <= MACHEP) { x = 1.0D - MACHEP; }
                else { x = 1.0D - x; }
            }

            return x;
        }

        /// <summary>
        /// Calculate the probability density function of the Student T distribution
        /// </summary>
        /// <remarks>
        /// Author: John Hurliman
        /// </remarks>
        /// <param name="t">T value to calculate the PDF of</param>
        /// <param name="k">Degrees of freedom</param>
        /// <returns>
        /// PDF of t in a Student T distribution with k degrees of freedom
        /// </returns>
        public static double TPDF(double t, int k)
        {
            double _k = (double)k;
            return Gamma((_k + 1) / 2) / (Math.Sqrt(_k * Math.PI) * Gamma(_k / 2) * Math.Pow(1 + ((t * t) / _k), (_k + 1) / 2));
        }

        /// <summary>
        /// Calculate the cumulative density function of the Student T distribution
        /// </summary>
        /// <param name="t">T value to calculate the CDF of</param>
        /// <param name="k">Degrees of freedom</param>
        /// <returns>CDF of t in a Student T distribution with k degrees of freedom</returns>
        public static double TCDF(double t, int k)
        {
            double Epsilon = MachineEpsilon();

            if (k <= 0) { return -1D; }
            if (t == 0) { return 0.5D; }
            if (t < -2) { return 0.5D * INCBETA(0.5D * k, 0.5D, k / (k + t*t)); }

            double x = (t < 0) ? -t : t;
            double z = 1.0 + x * x / k;
            double p;

            if ((k & 1) != 0)
            {
                double xsqk = x / Math.Sqrt(k);
                p = Math.Atan(xsqk);

                if (k > 1)
                {
                    double f = 1;
                    double tz = 1;
                    int j = 3;

                    while ((j <= (k - 2)) && ((tz / f) > Epsilon))
                    {
                        tz *= (j - 1) / (z * j);
                        f += tz;
                        j += 2;
                    }

                    p += f * xsqk / z;
                }

                p *= 2.0D / Math.PI;
            }
            else
            {
                double f = 1;
                double tz = 1;
                int j = 2;

                while ((j <= (k - 2)) && ((tz / f) > Epsilon))
                {
                    tz *= (j - 1) / (z * j);
                    f += tz;
                    j += 2;
                }

                p = f * x / Math.Sqrt(z * k);
            }

            return 0.5D + 0.5D * ((t < 0) ? -p : p);
        }

        /// <summary>
        /// Calculate the inverse cumulative density function of the Student T distribution
        /// </summary>
        /// <param name="p">p = cdf(t), A percentage value greater than zero and less than one</param>
        /// <param name="k">Degree of freedom</param>
        /// <returns>The t value in a Student distribution with k degrees of freedom given p, equal to cdf(t)</returns>
        public static double ITCDF(double p, int k)
        {
            double MAXNUM = 1.7976931348623158E308;
            double t, rk, z;
            int rflg;

            if (k <= 0 || p <= 0.0D || p >= 1.0D) { return 0.0D; }

            rk = k;

            if (p > 0.25D && p < 0.75D)
            {
                if (p == 0.5D) { return 0.0D; }

                z = 1.0D - 2.0D * p;
                z = IINCBETA(0.5D, 0.5D * rk, Math.Abs(z));
                t = Math.Sqrt(rk * z / (1.0D - z));

                if (p < 0.5D) { t = -t; }

                return t;
            }

            rflg = -1;
            if (p >= 0.5D)
            {
                p = 1.0D - p;
                rflg = 1;
            }
            z = IINCBETA(0.5D * rk, 0.5D, 2.0D * p);

            if (MAXNUM * z < rk) { return (rflg * MAXNUM); }

            t = Math.Sqrt(rk / z - rk);
            return (rflg * t);
        }
    }

    /// <summary>
    /// Given two arrays of doubles, will run a univariate ordinary 
    /// least squares regression on the values.
    /// </summary>
    /// <remarks>
    /// Author: John Hurliman
    /// </remarks>
    public class OlsRegression
    {
        private int _n;

        /// <summary>Numer of samples</summary>
        public int n
        {
            get { return _n; }
        }

        private double _b1;

        /// <summary>b1 coefficient, the Y-intercept of the equation</summary>
        public double b1
        {
            get { return _b1; }
        }

        private double _b2;

        /// <summary>b2 coefficient, the slope of the equation</summary>
        public double b2
        {
            get { return _b2; }
        }

        private double _var;

        /// <summary>Variance</summary>
        public double Var
        {
            get { return _var; }
        }

        private double _seb2;

        /// <summary>Standard error of b2 coefficient</summary>
        public double SEb2
        {
            get { return _seb2; }
        }

        private double _r2;

        /// <summary>Coefficient of determination</summary>
        public double R2
        {
            get { return _r2; }
        }

        private double _ssXX;

        /// <summary>(Xi - Xbar)^2</summary>
        public double ssXX
        {
            get { return _ssXX; }
        }

        private double _muX;

        /// <summary>Mean value of the predictors</summary>
        public double muX
        {
            get { return _muX; }
        }

        private double[] _residuals;

        /// <summary>
        /// Retrieves a specific value in the residuals array (Yi - b1 - b2Xi)
        /// </summary>
        /// <param name="i">Array position to return</param>
        /// <returns>A single member of the residuals double[] array</returns>
        public double Residual(int i)
        {
            return _residuals[i];
        }

        /// <summary>
        /// Constructor, retrieves the machine epsilon and calls Clear() to reset the class variables
        /// </summary>
        /// <seealso cref="M:VisualRegression.OlsRegression.Clear"/>
        public OlsRegression()
        {
            Clear();
        }

        /// <summary>
        /// Resets all the class variables to zero
        /// </summary>
        public void Clear()
        {
            _n = 0;
            _b1 = _b2 = _var = _seb2 = _r2 = _ssXX = _muX = 0.0D;
            _residuals = new double[0];
        }

        /// <summary>
        /// Finds the statistical mean of the given array
        /// </summary>
        /// <param name="set">Array of values to calculate the mean</param>
        /// <returns>sum(X) / n</returns>
        public static double Mean(double[] set)
        {
            double sum = 0.0D;

            for (int i = 0; i < set.Length; i++)
            {
                sum += set[i];
            }

            return (sum / set.Length);
        }

        /// <summary>
        /// Calculate the OLS regression of the given arrays
        /// </summary>
        /// <remarks>
        /// Portions of this function have been adapted from JFreeChart (http://www.jfree.org/jfreechart/) 
        /// which is released under the GNU Lesser General Public License.
        /// </remarks>
        /// <param name="responses">Array of the regression responses, or Y values</param>
        /// <param name="predictors">Array of the regression predictors, or X values</param>
        /// <returns>
        /// true if successful, false if too few inputs or mismatched arrays
        /// </returns>
        public bool Calculate(double[] responses, double[] predictors)
        {
            double sumX = 0.0D, sumY = 0.0D, sumYY = 0.0D, sumXY = 0.0D, 
                   sumXX = 0.0D, ssYY = 0.0D, ssXY = 0.0D;

            // Test for valid inputs
            if (responses.Length < 2 || responses.Length != predictors.Length) { return false; }

            // Get the mean values of the x and y samples
            _muX = Mean(predictors);
            double muY = Mean(responses);

            // Initialize the class-wide variables
            _n = responses.Length;
            _b1 = _b2 = _seb2 = _r2 = 0.0D;
            _ssXX = 0.0D;
            _residuals = new double[_n];

            double __n = (double)_n;

            for (int i = 0; i < n; i++)
            {
                sumX += predictors[i];
                sumY += responses[i];
                sumXX += predictors[i] * predictors[i];
                sumYY += responses[i] * responses[i];
                sumXY += predictors[i] * responses[i];

                _ssXX += (predictors[i] - _muX) * (predictors[i] - _muX);
                ssYY += (responses[i] - muY) * (responses[i] - muY);
                ssXY += (predictors[i] - _muX) * (responses[i] - muY);
            }

            // b1 and b2 formulas
            _b2 = (sumXY - (sumX * sumY) / __n) / (sumXX - (sumX * sumX) / __n);
            _b1 = (sumY - _b2 * sumX) / __n;

            // Variance formula
            _var = (ssYY - _b2 * ssXY) / (__n - 2);

            // Standard error of b2 formula
            _seb2 = Math.Sqrt(_var / _ssXX);

            // Coefficient of determination formula
            _r2 = (_b2 * (sumXY - sumX * sumY / __n)) / (sumYY - (sumY * sumY) / __n);

            // Calculate the residuals, and sum them to check our math
            for (int i = 0; i < _n; i++)
            {
                _residuals[i] = responses[i] - _b1 - _b2 * predictors[i];
            }

            return true;
        }
    }
}
