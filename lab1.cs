#define PART_2_ON
using System;
using static System.Math;
using Rychusoft.NumericalLibraries.Integral;
using System.Linq;

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

    class Solver
    {
        public static (double[], double[], double[]) Solve(int N) {
            double h = 1.0 / N;
            double X(double i) => i * h;
            double[] f, c, a, b, y1, y2, y3;
            Progonka prg;

            //###################################################################################################################

            f = new double[N + 1];
            f[0] = 1 + 5.0 / 4 * h;
            f[N] = h / 2 * (Sin(1) - 2 * Cos(1));
            for (int i = 1; i <= N - 1; i++) { f[i] = Sin(X(i)) - 2 * Cos(X(i)); }

            c = new double[N + 1];
            c[0] = 2.0 / h + 1 + h / 4;
            c[N] = -1.0 / h - Tan(1) - h / 2 * (1 - Tan(1));
            for (int i = 1; i <= N - 1; i++) { c[i] = -2 * (2 - X(i)) / h / h - X(i); }

            a = new double[N];
            a[N - 1] = -1.0 / h;
            for (int i = 1; i <= N - 1; i++) { a[i - 1] = -(2 - X(i)) / h / h - 1.0 / 2 / h; }

            b = new double[N];
            b[0] = 2.0 / h;
            for (int i = 1; i <= N - 1; i++) { b[i] = -(2 - X(i)) / h / h + 1.0 / 2 / h; }

            prg = new Progonka(a, c, b, f);
            prg.PerformProgonka();
            y1 = (double[])prg.y.Clone();
            //foreach (var i in y1) { Console.WriteLine(i); }

            //###################################################################################################################

            string k_str = "2 - x";
            string rev_k_str = "1.0/(2 - x)";
            string q_str = "x";
            string f_str = "2 * cos(x) - sin(x)";
            double hi0 = 1, hi1 = Tan(1), g0 = 1, g1 = 0, half = 1.0 / 2, fi_0, fi_N, d_0, d_N;

            double rev_k_int(double a, double b) => Log(Abs(a - 2) / Abs(b - 2));
            double q_str_int(double a, double b) => (b * b - a * a) / 2;
            double f_str_int(double a, double b) => (2 * Sin(b) + Cos(b) - 2 * Sin(a) - Cos(a));


            Func<double, double> a_i = i => h / rev_k_int(X(i - 1), X(i));
            Func<double, double> d_i = i => 1.0 / h * q_str_int(X(i - half), X(i + half));
            Func<double, double> phi_i = i => 1.0 / h * f_str_int(X(i - half), X(i + half));

            f = new double[N + 1];

            fi_0 = 2.0 / h * f_str_int( 0, half * h);
            fi_N = 2.0 / h * f_str_int( 1 - half * h, 1);

            f[0] = -(g0 + h / 2 * fi_0);
            f[N] = -(g1 + h / 2 * fi_N);
            for (int i = 1; i <= N - 1; i++) { f[i] = -phi_i(i); }

            c = new double[N + 1];
            d_0 = 2.0 / h * q_str_int(0, half * h);
            d_N = 2.0 / h * q_str_int(1 - half * h, 1);


            c[0] = -a_i(1) / h - h / 2 * d_0 - hi0;
            c[N] = -a_i(N) / h - hi1 - h / 2 * d_N;
            for (int i = 1; i <= N - 1; i++) { c[i] = -a_i(i + 1) / h / h - a_i(i) / h / h - d_i(i); }

            a = new double[N];
            a[N - 1] = -(a_i(N) / h);
            for (int i = 1; i <= N - 1; i++) { a[i - 1] = -(a_i(i) / h / h); }

            b = new double[N];
            b[0] = -(a_i(1) / h);
            for (int i = 1; i <= N - 1; i++) { b[i] = -(a_i(i + 1) / h / h); }

            prg = new Progonka(a, c, b, f);
            prg.PerformProgonka();
            y2 = (double[])prg.y.Clone();

            //###################################################################################################################

            Func<Func<double, double>, double, double, double> intMid = (func, a, b) => func((a + b) / 2) * (b - a);
            double k_func(double x) => 2 - x;
            double q_func(double x) => x;
            double f_func(double x) => 2 * Cos(x) - Sin(x);

            a_i = i => 1.0 / h * (intMid(k_func, X(i - 1), X(i)) -
            intMid(x => q_func(x) * (X(i) - x) * (x - X(i - 1)), X(i - 1), X(i)));

            d_i = i => 1.0 / h / h * (intMid(x => q_func(x) * (x - X(i - 1)), X(i - 1), X(i)) +
            intMid(x => q_func(x) * (X(i + 1) - x), X(i), X(i + 1)));

            phi_i = i => 1.0 / h / h * (intMid(x => f_func(x) * (x - X(i - 1)), X(i - 1), X(i)) +
            intMid(x => f_func(x) * (X(i + 1) - x), X(i), X(i + 1)));

            f = new double[N + 1];
            fi_0 = 2.0 / h / h * intMid(x => f_func(x) * (h - x), 0, h);
            fi_N = 2.0 / h / h * intMid(x => f_func(x) * (x - 1 + h), 1 - h, 1);
            f[0] = -(g0 + h / 2 * fi_0);
            f[N] = -(g1 + h / 2 * fi_N);
            for (int i = 1; i <= N - 1; i++) { f[i] = -phi_i(i); }

            c = new double[N + 1];
            d_0 = 2.0 / h / h * intMid(x => q_func(x) * (h - x), 0, h);
            d_N = 2.0 / h / h * intMid(x => q_func(x) * (x - 1 + h), 1 - h, 1);
            c[0] = -a_i(1) / h - h / 2 * d_0 - hi0;
            c[N] = -a_i(N) / h - hi1 - h / 2 * d_N;
            for (int i = 1; i <= N - 1; i++) { c[i] = -a_i(i + 1) / h / h - a_i(i) / h / h - d_i(i); }

            a = new double[N];
            a[N - 1] = -(a_i(N) / h);
            for (int i = 1; i <= N - 1; i++) { a[i - 1] = -(a_i(i) / h / h); }

            b = new double[N];
            b[0] = -(a_i(1) / h);
            for (int i = 1; i <= N - 1; i++) { b[i] = -(a_i(i + 1) / h / h); }

            prg = new Progonka(a, c, b, f);
            prg.PerformProgonka();
            y3 = (double[])prg.y.Clone();

            return (y1, y2, y3);
        }
    }
    class MyClass
    {
        static double GetNorma(double[] y1, double[] y2)
        {
            double[] norma = new double[y1.Length];
            for (int i = 0; i < y1.Length; i++)
            {
                norma[i] = Math.Abs(y1[i] - y2[i]);
            }
            return norma.Max();
        }

        static double[] GetEleven(double[] y)
        {
            double[] res = new double[11];
            int n = (y.Length - 1) / 10; //N=30 => n=3
            for (int i = 0; i <= 10; i++)
            {
                res[i] = y[n * i];
            }
            return res;
        }

        static void Show(double[] y, string str = "") {
            Console.WriteLine(str);
            foreach (var item in y)
            {
                Console.WriteLine("{0:f8}", item);
            }
        }

        static void Show(double[] y1, double[] y2, string str = "")
        {
            Console.WriteLine(str);
            double[] norma = new double[y1.Length];
            for (int i = 0; i < y1.Length; i++)
            {
                norma[i] = Math.Abs(y1[i] - y2[i]);
            }
            Show(norma, str);
        }

        static void Main(string[] args)
        {
            try
            {
                int N;

                N = 100;
                var ys1 = Solver.Solve(N);
                var y1_accurate = GetEleven(ys1.Item1);
                var y2_accurate = GetEleven(ys1.Item2);
                var y3_accurate = GetEleven(ys1.Item3);

                N = 10;
                var ys2 = Solver.Solve(N);
                var y1 = GetEleven(ys2.Item1);
                var y2 = GetEleven(ys2.Item2);
                var y3 = GetEleven(ys2.Item3);

                Show(y1, "y1"); Show(y2, "y2"); Show(y3, "y3");

                Show(y1, y2_accurate); Show(y2, y2_accurate); Show(y3, y2_accurate);

                Console.WriteLine("norms");
                Console.WriteLine();
                Console.WriteLine(GetNorma(y1, y2_accurate));
                Console.WriteLine(GetNorma(y2, y2_accurate));
                Console.WriteLine(GetNorma(y3, y2_accurate));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}
