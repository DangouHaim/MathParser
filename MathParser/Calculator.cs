using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MathParser
{
    class Calculator
    {
        string data = "";

        List<string> Value;
        List<string> Operator;

        public Calculator()
        {
            data = "";
            Value = new List<string>();
            Operator = new List<string>();
        }

        public Calculator(string expression)
        {
            data = expression.Replace(" ", "").Replace('.', ',').ToLower();
            Value = new List<string>();
            Operator = new List<string>();
        }

        public double Calculate()
        {
            double res = 0;

            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentException();
            }

            //res = double.Parse(SimpleEx(Value.ToArray(), Operator.ToArray(), data));
            res = double.Parse(ComplexEx(data));
            return Math.Round(res, 5);
        }

        private string ComplexEx(string complexEx)
        {
            string origin = complexEx;

            while(origin.IndexOf('(') > -1)
            {
                string[] simpleExes = GetSimpleExes(origin);
                foreach (string s in simpleExes)
                {
                    if (s.StartsWith("(") || s.StartsWith("sin(") || s.StartsWith("cos(") || s.StartsWith("sqrt("))
                    {
                        string o = s.Remove(s.IndexOf('('), 1).Remove(s.Length - 2);
                        Value = GetCalculationPatterns(o, true).Item1.ToList();
                        Operator = GetCalculationPatterns(o, true).Item2.ToList();

                        if(s.StartsWith("cos("))
                        {
                            origin = origin.Replace(s, Math.Cos(double.Parse(SimpleEx(Value.ToArray(), Operator.ToArray(), o))).ToString());
                        }
                        else
                        {
                            if (s.StartsWith("cos("))
                            {
                                origin = origin.Replace(s, Math.Sin(double.Parse(SimpleEx(Value.ToArray(), Operator.ToArray(), o))).ToString());
                            }
                            else
                            {
                                if (s.StartsWith("sqrt("))
                                {
                                    origin = origin.Replace(s, Math.Sqrt(double.Parse(SimpleEx(Value.ToArray(), Operator.ToArray(), o))).ToString());
                                }
                                else
                                {
                                    origin = origin.Replace(s, SimpleEx(Value.ToArray(), Operator.ToArray(), o));
                                }
                            }
                        }
                        origin = origin.Replace("+-", "-").Replace("-+", "-");
                    }
                }
            }
            Value = GetCalculationPatterns(origin, true).Item1.ToList();
            Operator = GetCalculationPatterns(origin, true).Item2.ToList();
            origin = SimpleEx(Value.ToArray(), Operator.ToArray(), origin);
            return origin;
        }

        private Tuple<string[], string[]> GetCalculationPatterns(string input, bool absolute = false)
        {
            List<string> val = new List<string>();
            List<string> op = new List<string>();
            if(!absolute)
            {
                foreach (var i in new Regex(@"\-?\d+,*\d*").Matches(input))
                {
                    val.Add(i.ToString());
                }
                foreach (var i in new Regex(@"[* / ^]").Matches(input))
                {
                    op.Add(i.ToString());
                }
            }
            else
            {
                foreach (var i in new Regex(@"\-?\d+,*\d*").Matches(input))
                {
                    val.Add(i.ToString());
                }
                foreach (var i in new Regex(@"[* / ^ \- +]").Matches(input))
                {
                    op.Add(i.ToString());
                }
            }
            return new Tuple<string[], string[]>(val.ToArray(), op.ToArray());
        }

        private string SimpleEx(string[] v, string[] op, string or)
        {
            string origin = or;

            for (int i = op.Length - 1; i >= 0; i--)
            {
                if (op[i] == "^")
                {
                    origin = origin.Replace(v[i] + op[i] + v[i + 1], (Math.Pow(double.Parse(v[i]), double.Parse(v[i + 1]))).ToString());
                    v = GetCalculationPatterns(origin, true).Item1;
                    op = GetCalculationPatterns(origin, true).Item2;
                    i = op.Length;
                }
            }

            for (int i = op.Length - 1; i >= 0; i--)
            {

                if (op[i] == "*")
                {
                    origin = origin.Replace(v[i] + op[i] + v[i + 1], (double.Parse(v[i]) * double.Parse(v[i + 1])).ToString());
                    v = GetCalculationPatterns(origin, true).Item1;
                    op = GetCalculationPatterns(origin, true).Item2;
                    i = op.Length;
                }
                else
                {
                    if (op[i] == "/")
                    {
                        if(double.Parse(v[i + 1]) == 0)
                        {
                            throw new DivideByZeroException();
                        }
                        origin = origin.Replace(v[i] + op[i] + v[i + 1], (double.Parse(v[i]) / double.Parse(v[i + 1])).ToString());
                        v = GetCalculationPatterns(origin, true).Item1;
                        op = GetCalculationPatterns(origin, true).Item2;
                        i = op.Length;
                    }
                }
            }
            double res = 0;
            foreach(var i in v)
            {
                res += double.Parse(i);
            }
            origin = res.ToString();
            return origin;
        }

        private string[] GetSimpleExes(string complexEx)
        {
            List<string> res = new List<string>();

            string exstrong = @"(\-?\d,?\d?[+ / * ^]?(\-?\d,?\d?)?[+ / * ^]?)+";

            foreach (var v in new Regex(@"(sin)?(cos)?(sqrt)?\(" + exstrong + @"\)").Matches(complexEx))
            {
                res.Add(v.ToString());
            }
            return res.ToArray();
        } // (|) ... (|) 
    }
}
