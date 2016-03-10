using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleMaster
{
    internal class CalcParser
    {

        public readonly Dictionary<string, double> Map = new Dictionary<string, double>();
        public Dictionary<string, double> Variables = new Dictionary<string, double>();
        public const string OperatorChars = "+-*/^";

        public CalcParser()
        {
            InitConstMap();
        }

        public double Evaluate(string str)
        {
            if (str.Contains('(') || str.Contains(')'))
            {
                ValidateParen(str);
                return EvaluateParenStatement(str);
            }
            return EvaluateStatement(str);
        }

        public double EvaluateParenStatement(string statement)
        {
            var isOperationAhead = false;
            var lastOpenIndex = -1;
            for (int i = 0; i < statement.Length; i++)
            {
                var c = statement[i];
                switch (c)
                {
                    case '(':
                        lastOpenIndex = i;
                        isOperationAhead = i > 0 && !OperatorChars.Contains(statement[i - 1]);
                        break;
                    case ')':
                        var parenContent = statement.Substring(lastOpenIndex + 1, i - lastOpenIndex - 1);
                        var res = Evaluate(parenContent);
                        if (isOperationAhead)
                        {
                            var sb = new StringBuilder();
                            for (int j = lastOpenIndex - 1; j >= 0 && !OperatorChars.Contains(statement[j]); j--)
                            {
                                sb.Append(statement[j]);
                            }
                            var opName = sb.ToString().Reverse().ToArray();
                            var sOpName = new string(opName);
                            var totalRes = EvaluateOperation(sOpName, res);
                            statement = statement.Replace(sOpName + "(" + parenContent + ")", totalRes.ToString("0." + new string('#', 339)));
                            return Evaluate(statement);
                        }
                        statement = statement.Replace("(" + parenContent + ")",
                            res.ToString("0." + new string('#', 339)));
                        return Evaluate(statement);
                }
            }
            return EvaluateStatement(statement);
        }


        public double EvaluateOperation(string name, double val)
        {
            if (name.StartsWith("log"))
            {
                if (name.Length > 3)
                {
                    var sLogBase = name.Substring(3, name.Length - 3);
                    try
                    {
                        var logBase = int.Parse(sLogBase);
                        var logRes = Math.Log(val, logBase);
                        return Math.Round(logRes, 10);
                    }
                    catch (Exception)
                    {
                        throw new Exception("invalid log base: " + sLogBase);
                    }
                }
                var log = Math.Log(val, 2);
                return Math.Round(log, 10);
            }
            double res;
            switch (name)
            {
                // logarithms
                case "ln":
                    res = Math.Log(val);
                    break;
                case "lg":
                    res = Math.Log10(val);
                    break;
                // square root
                case "sqrt":
                    res = Math.Sqrt(val);
                    break;
                // absolute
                case "abs":
                    res = Math.Abs(val);
                    break;
                // trig operation
                case "sin":
                    res = Math.Sin(val);
                    break;
                case "cos":
                    res = Math.Cos(val);
                    break;
                case "tan":
                    res = Math.Tan(val);
                    break;
                case "atan":
                    res = Math.Atan(val);
                    break;
                case "asin":
                    res = Math.Asin(val);
                    break;
                case "acos":
                    res = Math.Acos(val);
                    break;
                // no such operation
                default:
                    throw new Exception("invalid operation name (" + name + ")!");
            }
            return Math.Round(res, 10);
        }

        public double EvaluateStatement(string statement)
        {
            var operators = Operators(statement);
            // powers first
            for (int i = operators.Count - 1; i >= 0; i--)
            {
                var opIndex = operators[i];
                var c = statement[opIndex];
                if (c == '^')
                {
                    var startIndex = i - 1 >= 0 ? operators[i - 1] + 1 : 0;
                    var endIndex = i + 1 < operators.Count ? operators[i + 1] - 1 : statement.Length - 1;
                    var operant1 = statement.Substring(startIndex, opIndex - startIndex);
                    var operant2 = statement.Substring(opIndex + 1, endIndex - opIndex);
                    var res = Calculate(operant1, c, operant2);
                    statement = statement.Replace(operant1 + c + operant2, res.ToString("0." + new string('#', 339)));
                    return EvaluateStatement(statement);
                }
            }
            // then multiplication and division
            for (int i = 0; i < operators.Count; i++)
            {
                var opIndex = operators[i];
                var c = statement[opIndex];
                if (c == '*' || c == '/')
                {
                    var startIndex = i - 1 >= 0 ? operators[i - 1] + 1 : 0;
                    var endIndex = i + 1 < operators.Count ? operators[i + 1] - 1 : statement.Length - 1;
                    var operant1 = statement.Substring(startIndex, opIndex - startIndex);
                    var operant2 = statement.Substring(opIndex + 1, endIndex - opIndex);
                    var res = Calculate(operant1, c, operant2);
                    statement = statement.Replace(operant1 + c + operant2, res.ToString("0." + new string('#', 339)));
                    return EvaluateStatement(statement);
                }
            }
            // eventually addition and substraction
            for (int i = 0; i < operators.Count; i++)
            {
                var opIndex = operators[i];
                var c = statement[opIndex];
                if (c == '+' || c == '-')
                {
                    var startIndex = i - 1 >= 0 ? operators[i - 1] + 1 : 0;
                    var endIndex = i + 1 < operators.Count ? operators[i + 1] - 1 : statement.Length - 1;
                    var operant1 = statement.Substring(startIndex, opIndex - startIndex);
                    var operant2 = statement.Substring(opIndex + 1, endIndex - opIndex);
                    var res = Calculate(operant1, c, operant2);
                    statement = statement.Replace(operant1 + c + operant2, res.ToString("0." + new string('#', 339)));
                    return EvaluateStatement(statement);
                }
            }
            return GetValue(statement);
        }

        public double Calculate(string s1, char op, string s2)
        {
            return Calculate(GetValue(s1), op, GetValue(s2));
        }

        public List<int> Operators(string line)
        {
            var list = new List<int>();
            var prevChar = '*';
            for (int i = 0; i < line.Length; i++)
            {
                var c = line[i];
                if (OperatorChars.Contains(c) && !OperatorChars.Contains(prevChar) && prevChar != '(')
                {
                    list.Add(i);
                }
                prevChar = c;
            }
            return list;
        }

        public double GetValue(string exp)
        {
            // check if such constant exist
            if (Map.ContainsKey(exp))
            {
                return Map[exp];
            }
            // check if such variable exists
            if (Variables.ContainsKey(exp))
            {
                return Variables[exp];
            }
            // try parsing the string to double value
            return double.Parse(exp);
        }

        public double Calculate(double d1, char op, double d2)
        {
            var res = d1;
            switch (op)
            {
                case '+':
                    res += d2;
                    break;
                case '-':
                    res -= d2;
                    break;
                case '*':
                    res *= d2;
                    break;
                case '/':
                    res /= d2;
                    break;
                case '^':
                    res = Math.Pow(d1, d2);
                    break;
                default:
                    throw new Exception("invalid operator!");
            }
            return res;
        }

        public void ValidateParen(string line)
        {
            var stack = new Stack<char>();
            foreach (var c in line)
            {
                if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    if (stack.Count == 0)
                    {
                        throw new Exception("invalid parentheses!");
                    }
                    stack.Pop();
                }
            }
            if (stack.Count != 0)
            {
                throw new Exception("invalid parentheses!");
            }

        }

        public bool ValidateVarName(string nameCandidate)
        {
            if (string.IsNullOrEmpty(nameCandidate))
            {
                throw new Exception("no side of assignment must be empty!");
            }

            if (nameCandidate.IndexOfAny(OperatorChars.ToCharArray()) == -1 && !Map.ContainsKey(nameCandidate) &&
                   !char.IsDigit(nameCandidate[0]) && !nameCandidate.Trim().Contains(" "))
            {
                try
                {
                    EvaluateOperation(nameCandidate, 1.0);
                    return false;
                }
                catch (Exception)
                {
                    return true;
                }

            }
            return false;
        }

        public void InitConstMap()
        {
            using (var streamReader = GetResourceStreamReader("const.tx"))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }
                    var split = line.Split('=');
                    var name = split[0];
                    var val = double.Parse(split[1]);
                    Map.Add(name, val);
                }
            }
        }

        public static StreamReader GetResourceStreamReader(string fileName)
        {
            var ass = typeof(CalcParser).Assembly;
            var stream = ass.GetManifestResourceStream("ConsoleMaster.resources." + fileName);
            if (stream == null)
            {
                throw new Exception();
            }
            return new StreamReader(stream);
        }
    }
}
