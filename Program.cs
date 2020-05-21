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
        public static void Solve(double tau) {
            double h = 0.05;
            int N2 = (int)Round(1 / tau), 
                N1 = (int)Round(1 / h);
            double x(double i) => i * h > 1 ? throw new Exception() : i * h;
            double t(double j) => j * tau > 1 ? throw new Exception() : j * tau;
            double[] f, c, a, b;
            Progonka prg;

            double[,] y = new double[N2 + 1, N1 + 1];
            for (int i = 0; i <= N1; i++) { y[0, i] = 1; }
            for (int j = 0; j <= N2; j++) { y[j, 0] = 1; }



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
                
                double tau = 0.05;

                int LineByTime(double tau, double time) => (int)Math.Round(time / tau);//0, 1 ... N_2 - 1


                int N;

            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
    }
}