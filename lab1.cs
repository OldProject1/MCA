using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System;
using static System.Math;
using Rychusoft.NumericalLibraries.Integral;

namespace SandBox
{
    class Progonka
    {
        private double[] a, b, c, f;
        public double[] y { get; private set; }
        private double[] alpha;
        private double[] betta;
        private int SIZE;
        public Progonka(double[] a, double[] c, double[] b, double[] f)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.f = f;
            SIZE = f.Length;
            y = new double[SIZE];
            alpha = new double[SIZE - 1];
            betta = new double[SIZE];
        }
        public void PerformProgonka()
        {
            DefineAlphaAndBetta();
            DefineSolution();
        }
        private void DefineAlphaAndBetta()
        {
            alpha[0] = b[0] / c[0];
            betta[0] = f[0] / c[0];
            for (int i = 1; i < SIZE - 1; i++)
            {
                alpha[i] = b[i] / (c[i] - a[i - 1] * alpha[i - 1]);
                betta[i] = (f[i] + a[i - 1] * betta[i - 1]) / (c[i] - a[i - 1] * alpha[i - 1]);
            }
            betta[SIZE - 1] = (f[SIZE - 1] + a[SIZE - 2] * betta[SIZE - 2]) / (c[SIZE - 1] - a[SIZE - 2] * alpha[SIZE - 2]);
        }
        private void DefineSolution()
        {
            y[SIZE - 1] = betta[SIZE - 1];
            for (int i = SIZE - 2; i >= 0; i--)
            {
                y[i] = alpha[i] * y[i + 1] + betta[i];
            }
        }
    }
    class MyClass
    {
        static void Main(string[] args)
        {
            try
            {
                int N = 10;// N + 1 = 11 = num of nodes 
                double h = 1.0 / N;
                double[] x = new double[N + 1];
                for (int i = 0; i <= N; i++)
                {
                    x[i] = i * h;
                }
                double[] a = new double[N - 2];
                double[] c = new double[N - 1];
                double[] b = new double[N - 2];
                double[] f = new double[N - 1];
                double A_0 = 2.0 / h + 1 + h / 4;
                double A_1 = -1.0 / h - Tan(1) + h / 2 * (1 - Tan(1));

                f[0] = Sin(x[1]) - 2 * Cos(x[1]) - ((2 - x[1]) / h / h + 1.0 / 2 / h) / A_0 * (1 + 5.0 / 4 * h);
                f[N - 2] = Sin(x[N - 1]) - 2 * Cos(x[N - 1]) + ((2 - x[N - 1]) / h / h - 1.0 / 2 / h) * h / 2 / A_1 * (Sin(1) - 2 * Cos(1));
                for (int i = 2; i <= N - 2; i++)
                {
                    f[i - 1] = Sin(x[i]) - 2 * Cos(x[i]);
                }

                c[0] = (-2 * (2 - x[1]) / h / h - x[1]) + (2.0 / h / A_0) * ((2 - x[1]) / h / h + 1.0 / 2 / h);
                c[N - 2] = (-2 * (2 - x[N - 1]) / h / h - x[N - 1]) - 1.0 / h / A_1 * ((2 - x[N - 1]) / h / h - 1.0 / 2 / h);
                for (int i = 2; i <= N - 2; i++)
                {
                    c[i - 1] = -2 * (2 - x[i]) / h / h - x[i];
                }

                for (int i = 2; i <= N - 1; i++)
                {
                    a[i - 2] = -(2 - x[i]) / h / h - 1.0 / 2 / h;
                }

                for (int i = 1; i <= N - 2; i++)
                {
                    b[i - 1] = -(2 - x[i]) / h / h + 1.0 / 2 / h;
                }


                Progonka prg = new Progonka(a, c, b, f);
                prg.PerformProgonka();
                double[] y = new double[prg.y.Length + 2];
                for (int i = 0; i < prg.y.Length; i++)
                {
                    y[i + 1] = prg.y[i];
                }
                y[N] = -1.0 / h / A_1 * y[N - 1] - h / 2 / A_1 * (Sin(1) - 2 * Cos(1));
                y[0] = 2.0 / h / A_0 * y[1] + 1.0 / A_0 * (1 + 5.0 / 4 * h);

                foreach (double res in y) { Console.WriteLine(res); }

                //###################################################################################################################
                double X(double i) => i * h;

                //Func<Func<double, double>, double, double, double> integrMiddle = (func, a, b) => func((a + b) / 2) * (b - a);

                //double rev_k_func(double x) => 1.0 / (2 - x);
                //double k_func(double x) => 2 - x;
                //double q_func(double x) => x;
                //double f_func(double x) => 2 * Cos(x) - Sin(x);

                string k_str = "2 - x";
                string rev_k_str = "1.0/(2 - x)";
                string q_str = "x";
                string f_str = "2 * cos(x) - sin(x)";
                double hi0 = 1, hi1 = Tan(1), g0 = 1, g1 = 0, half = 1.0 / 2;

                //string k_str = "cos(x)^2 + 1";
                //string rev_k_str = "1.0/(cos(x)^2 + 1)";
                //string q_str = "1";
                //string f_str = "sin(x)^2";
                //double hi0 = 1, hi1 = 1, g0 = 0, g1 = 1, half = 1.0 / 2;

                double a_i(double i) => h / new Integral(rev_k_str, X(i - 1), X(i)).ComputeIntegral();
                double d_i(double i) => 1.0 / h * new Integral(q_str, X(i - half), X(i + half)).ComputeIntegral();
                double phi_i(double i) => 1.0 / h * new Integral(f_str, X(i - half), X(i + half)).ComputeIntegral();

                f = new double[N + 1];
                var fi_0 = 2.0 / h * new Integral(f_str, 0, half).ComputeIntegral();
                var fi_N = 2.0 / h * new Integral(f_str, 1 - half, 1).ComputeIntegral();
                f[0] = -(g0 + h / 2 * fi_0);
                f[N] = -(g1 + h / 2 * fi_N);
                for (int i = 1; i <= N - 1; i++) { f[i] = -phi_i(i); }

                c = new double[N + 1];
                var d_0 = 2.0 / h * new Integral(q_str, 0, half).ComputeIntegral();
                var d_N = 2.0 / h * new Integral(q_str, 1 - half, 1).ComputeIntegral();
                c[0] = -a_i(1) / h - h / 2 * d_0 - hi0;
                c[N] = -a_i(N) / h - hi1 - h / 2 * d_N;
                for (int i = 1; i <= N - 1; i++) { c[i] = -a_i(i + 1) / h / h - a_i(i) / h / h - d_i(i); }

                a = new double[N];
                a[N - 1] = -(a_i(N) / h);
                for (int i = 1; i <= N - 1; i++) { a[i - 1] = -(a_i(i) / h / h); }

                b = new double[N];
                b[0] = -(a_i(1) / h);
                for (int i = 1; i <= N - 1; i++) { b[i] = -(a_i(i + 1) / h / h); }

                Progonka prg2 = new Progonka(a, c, b, f);
                prg2.PerformProgonka();
                double[] y2 = new double[prg2.y.Length];
                Console.WriteLine();
                for (int i = 0; i < prg2.y.Length; i++)
                {
                    y2[i] = prg2.y[i];
                    Console.WriteLine(prg2.y[i]);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
