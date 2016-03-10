using System.Linq;

namespace ConsoleMaster
{
    public class Approx
    {

        private static readonly CalcParser CalcParser = new CalcParser();

        public static double Solve(string eq)
        {
            Solver.RemoveWhites(ref eq);
            //assert valid
            var xChar = eq.Any(char.IsLetter);
            var split = eq.Split('=');
            var left = split[0];
            var right = split[1];



            return 0.0;
        }

        public static double CalcFor(string term, string xChar, double x)
        {
            var xStr = x.ToString("0." + new string('#', 339));
            term = term.Replace(xChar, xStr);
            return CalcParser.Evaluate(term);
        }
    }
}