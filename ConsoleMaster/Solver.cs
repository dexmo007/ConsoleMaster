using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace ConsoleMaster
{
    public static class Solver
    {

        private static readonly CalcParser Cp = new CalcParser();

        private static char Validate(string eq)
        {
            var firstIndex = eq.IndexOf("=");
            if (firstIndex == -1 || firstIndex == 0 || firstIndex == eq.Length - 1 ||
                firstIndex != eq.LastIndexOf("="))
            {
                throw new InvalidEquation(eq);
            }
            var variable = '\0';
            // check chars in eq string and determine the variable char
            foreach (var c in eq)
            {
                if (!char.IsLetterOrDigit(c) && c != '.' && c != '=' && !CalcParser.OperatorChars.Contains(c))
                {
                    throw new InvalidEquation(eq);
                }
                if (char.IsLetter(c))
                {
                    if (variable == '\0')
                    {
                        variable = c;
                    }
                    else if (c != variable)
                    {
                        throw new InvalidEquation(eq);
                    }
                }
            }
            return variable;
        }

        public static void RemoveWhites(ref string str)
        {
            str = new string(str.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray());
        }

        private static double Solve(string eq)
        {
            RemoveWhites(ref eq);
            var variable = Validate(eq);
            var split = eq.Split('=');
            var left = split[0];
            var right = split[1];
            return 0.0;
        }



        private static Term Evaluate(string term, char variable)
        {
            var resTerm = new Term();
            if (!term.Contains(variable))
            {
                resTerm.Summand = Cp.Evaluate(term);
                resTerm.Scale = 0.0;
            }
            else
            {
                var exTerm = new ExTerm(term);
                return Evaluate(exTerm, variable);
            }
            return resTerm;
        }

        private static Term Evaluate(ExTerm xt, char variable)
        {
            var rest = xt.Rest;
            var xIndex = rest.IndexOf(variable);
            var end = xIndex + 1;
            while (end < rest.Length)
            {
                if (rest[end] == '+' || rest[end] == '-')
                {
                    break;
                }
                end++;
            }
            var start = xIndex - 1;
            while (start > 0)
            {
                if (rest[start] == '+' || rest[start] == '-')
                {
                    break;
                }
                start--;
            }
            var exterm = new ExTerm();
            var subTerm = rest.Substring(start + 1, end);
            exterm.Term = Evaluate(subTerm, variable);
            exterm.Rest = rest.Substring(end);
            //todo
            return new Term();
        }
    }

    internal class ExTerm
    {
        public Term Term { get; set; }
        public string Rest { get; set; }

        public ExTerm()
        {
        }

        public ExTerm(string rest)
        {
            Term = new Term();
            Rest = rest;
        }
    }

    internal class Term
    {
        public double Scale { get; set; }
        public double Summand { get; set; }

        public Term()
        {
            Scale = 0;
            Summand = 0;
        }

        public void Add(Term other)
        {
            Scale += other.Scale;
            Summand += other.Summand;
        }

        public double CalcFor(double x)
        {
            return Scale * x + Summand;
        }
    }

    public class InvalidEquation : Exception
    {
        public InvalidEquation(string eq) : base(eq)
        {

        }
    }
}
