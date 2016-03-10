using System;
using System.Linq;
using System.Speech.Synthesis;

namespace ConsoleMaster
{
    internal class Calculator
    {
        private static CalcParser _calcParser;

        public static void Main(string[] args)
        {

            Console.WriteLine("Welcome to the lonely man entertainment system!");
            Console.WriteLine("-----------------------------------------------\n");

            _calcParser = new CalcParser();
            var eyyCount = 0;
            while (true)
            {
                Console.Write(">>>");
                try
                {
                    var cmd = Console.ReadLine();
                    if (cmd == null || cmd.Equals(""))
                    {
                        continue;
                    }
                    if (cmd.Equals("exit"))
                    {
                        return;
                    }
                    if (cmd.Equals("show"))
                    {
                        PrintVariables();
                        continue;
                    }
                    if (cmd.Equals("clear all") || cmd.Equals("clear"))
                    {
                        _calcParser.Variables.Clear();
                        Console.WriteLine("variables cleared.");
                        continue;
                    }
                    if (cmd.StartsWith("clear"))
                    {
                        var name = cmd.Substring(5).Trim();
                        Console.WriteLine(_calcParser.Variables.Remove(name)
                            ? "variable " + name + " cleared."
                            : "no such variable.");
                        continue;
                    }
                    if (cmd.Equals("eyy ich komm rein in die diskothek") || cmd.Equals("eyy"))
                    {
                        switch (eyyCount)
                        {
                            case 0:
                                Console.WriteLine("wäa?");
                                eyyCount++;
                                continue;
                            case 1:
                                Console.WriteLine("wer?");
                                eyyCount++;
                                continue;
                            case 2:
                                Console.WriteLine("SSIO ALLEINE AUF FICKTOURNEE!!");
                                eyyCount = 0;
                                continue;
                            default:
                                Console.WriteLine("wäa?");
                                continue;
                        }
                    }
                    if (cmd.Equals("solver"))
                    {
                        Solver.StartSolver();
                        continue;                            
                    }
                    // case: assignment
                    if (cmd.Contains('='))
                    {
                        if (cmd.Count(c => c == '=') != 1)
                        {
                            throw new Exception("only one '=' allowed!");
                        }
                        var eq = cmd.Split('=');
                        if (eq.Length != 2)
                        {
                            throw new Exception("assignment invalid!");
                        }
                        var left = eq[0];
                        var right = eq[1];
                        string name;
                        string value;
                        // determine which side is name and which is value/calc
                        if (_calcParser.ValidateVarName(left))
                        {
                            name = left;
                            value = right;
                        }
                        else if (_calcParser.ValidateVarName(right))
                        {
                            name = right;
                            value = left;
                        }
                        else
                        {
                            throw new Exception("no side qualifies for variable name!");
                        }
                        value = value.Replace(" ", "").Replace("-(","-1*(");
                        var res = _calcParser.Evaluate(value);
                        res = Math.Round(res, 10);
                        _calcParser.Variables[name] = res;
                        Console.WriteLine(name + " = " + res);
                    }
                    // case: pure calculation
                    else
                    {
                        cmd = cmd.Replace(" ", "").Replace("-(","-1*(");
                        var res = Math.Round(_calcParser.Evaluate(cmd), 10);
                        Console.WriteLine(res);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("WÄA??");
                    Console.WriteLine(e.Message);
                }
            }


        }

        private static void PrintVariables()
        {
            if (_calcParser.Variables.Count == 0)
            {
                Console.WriteLine("no variables declared.");
                return;
            }
            foreach (var variable in _calcParser.Variables)
            {
                Console.WriteLine(variable.Key + " = " + variable.Value);
            }
        }
    }

}
