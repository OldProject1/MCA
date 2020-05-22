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
        public static double[,] Solve(double tau) {
            double h = 0.05;
            int N2 = (int)Round(1 / tau), 
                N1 = (int)Round(1 / h);
            double x(double i) => i * h > 1 ? throw new Exception("out of grid") : i * h;
            double t(double j) => j * tau > 1 ? throw new Exception("out of grid") : j * tau;
            double mu(double t) => (t - 1) * Exp(-t);
            double phi(double x, double t) => -(x + t * t) * Exp(-x * t);
            double[] f, c, a, b, res;
            double[,] y = new double[N2 + 1, N1 + 1];
            Progonka prg;

            for (int i = 0; i <= N1; i++) { y[0, i] = 1; }
            for (int j = 0; j <= N2; j++) { y[j, 0] = 1; }

            res = new double[N1 + 1];
            f = new double[N1 + 1];
            c = new double[N1 + 1];
            a = new double[N1];
            b = new double[N1];
            for (int j = 0; j <= N2 - 1; j++)
            {
                Array.Clear(f, 0, f.Length);
                Array.Clear(c, 0, c.Length);
                Array.Clear(a, 0, a.Length);
                Array.Clear(b, 0, b.Length);

                c[0] = 1;
                c[N1] = 1.0 / h + 1 + h / 2 / tau;
                for (int i = 1; i <= N1 - 1; i++) { c[i] = 1.0 / tau + 2.0 / h / h; }

                a[N1 - 1] = 1.0 / h;
                for (int i = 1; i <= N1 - 1; i++) { a[i - 1] = 1.0 / h / h; }

                b[0] = 0;
                for (int i = 1; i <= N1 - 1; i++) { b[i] = 1.0 / h / h; }

                f[0] = 1;
                f[N1] = -mu(t(j + 1)) + h / 2 / tau * y[j, N1] + h / 2 * phi(1, t(j + 1));
                for (int i = 1; i <= N1 - 1; i++) { f[i] = phi(x(i), t(j)) + y[j, i] / tau; }

                prg = new Progonka(a, c, b, f);
                prg.PerformProgonka();
                for (int i = 0; i <= N1; i++)
                {
                    y[j + 1, i] = prg.y[i];
                }
            }
            return y;

        }
    }
    class MyClass
    {
        //max of diff
        static double GetNorma(double[] y1, double[] y2)
        {
            double[] norma = new double[y1.Length];
            for (int i = 0; i < y1.Length; i++)
            {
                norma[i] = Math.Abs(y1[i] - y2[i]);
            }
            return norma.Max();
        }

        static void ShowLikeColumn(double[] y, string str = "") {
            Console.WriteLine(str);
            foreach (var item in y)
            {
                Console.WriteLine("{0:f6}", item);
            }
            Console.WriteLine();
        }

        static void ShowMatr(double[,] mas)
        {
            int rows = mas.GetUpperBound(0) + 1;
            int columns = mas.Length / rows;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++) { 
                    Console.Write("{0:f3}", mas[i, j]);
                    Console.Write(' ');
                }
                Console.WriteLine();
            }

        }

        static double[] getLine(double[,] y, int lineNum)
        {
            int rows = y.GetUpperBound(0) + 1;
            int columns = y.Length / rows;
            double[] res = new double[columns];
            for (int i = 0; i < columns;i++)
            {
                res[i] = y[lineNum, i];
            }
            return res;
        }

        static void Main(string[] args)
        {
            try
            {
                double time, tau;
                double[,] y;
                int LineByTime(double tau, double time) => (int)Math.Round(time / tau);//0, 1 ... N_2 - 1

                tau = 0.05;
                y = Solver.Solve(tau);
                time = 0.1;
                ShowLikeColumn(getLine(y, LineByTime(tau, time)));
                time = 0.5;
                ShowLikeColumn(getLine(y, LineByTime(tau, time)));

                tau = 0.01;
                y = Solver.Solve(tau);
                time = 0.1;
                ShowLikeColumn(getLine(y, LineByTime(tau, time)));
                time = 0.5;
                ShowLikeColumn(getLine(y, LineByTime(tau, time)));
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}