using System;
using System.Collections.Generic;

namespace ChemicalEquations
{
    //300H2O+SiHg3(((O2)3Kj4)5)600
    class Program
    {

        public enum Type
        {
            NUM,
            PLUS,
            EL,
            LP,
            RP,
            EOF
        }
        public class Lex
        {
            public string value;
            public Type type;

        }

        static void Main(string[] args)
        {
            string inputFormula = Console.ReadLine();

            List<Lex> inputlexes = splitFormula(inputFormula);
            inputlexes.Add(new Lex{ value = "", type = Type.EOF});
            Parser p = new Parser();
            var inputD = p.ParseFormula(inputlexes);
            List<Dictionary<string, int>> formulas = new List<Dictionary<string, int>>();
            List<string> rawformulas = new List<string>();
            
            int n = int.Parse(Console.ReadLine());
            for (int i = 0; i < n; ++i)
            {
                string formula = Console.ReadLine();
                rawformulas.Add(formula);
                List<Lex> l = splitFormula(formula);
                l.Add(new Lex { value = "", type = Type.EOF });
                formulas.Add(p.ParseFormula(l));
            }

            for (int i = 0; i < formulas.Count; ++i)
            {
                if (Compare(inputD, formulas[i]))
                {
                    Console.WriteLine(inputFormula + "==" + rawformulas[i]);
                } else
                {
                    Console.WriteLine(inputFormula + "!=" + rawformulas[i]);

                }
            }
            



            /*
            foreach (var a in inputlexes)
            {
                Console.WriteLine(a.type + "   " + a.value);
            }
            Console.ReadLine();
            var f = p.ParseFormula(inputlexes);
            foreach(var el in f)
            {
                Console.WriteLine(el.Key + "   " + el.Value);
            }
            */
        }

        /*
        <формула> ::= [<число>] <последовательность> {"+" [<число>] <последовательность>}
        <последовательность> ::= <элемент> [<число>] {<элемент> [<число>]}
        <элемент> ::= <химический элемент> | "(" <последовательность> ")"


        <химический элемент> ::= <прописная буква> [<строчная буква>]
        <прописная буква> ::= "A".."Z"
        <строчная буква> ::= "a".."z"
        <число> ::= "1".."9" {"0".."9"}

        5S((ABC2)3B)4
         */
        public static bool Compare(Dictionary<string, int> a, Dictionary<string, int> b)
        {
            foreach (var el in a)
            {
                if (b.ContainsKey(el.Key))
                {
                    if (b[el.Key] != a[el.Key])
                    {
                        return false;
                    }
                } else
                {
                    return false;
                }
            }
            foreach (var el in b)
            {
                if (!a.ContainsKey(el.Key))
                {
                    return false;
                }
            }

            return true;
        }

        class Parser
        {
            Lex current;
            int i = 0;
            public bool GetNext(List<Lex> lexes)
            {
                if (i < lexes.Count)
                {
                    current = lexes[i];
                    ++i;
                    return true;
                }
                else
                {
                    return false;
                }

            }
            public Dictionary<string, int> ParseFormula(List<Lex> lexes)
            {
                i = 0;
                int gMult = 1;
                Dictionary<string, int> r = new Dictionary<string, int>();
                GetNext(lexes);
                if (current.type == Type.NUM)
                {
                    gMult = int.Parse(current.value);
                    GetNext(lexes);
                }
                var p = ParseSeq(lexes);
                p = MultDictionary(p, gMult);
                r = AddDictionary(r, p);

                while (current.type == Type.PLUS)
                {
                    gMult = 1;

                    GetNext(lexes);
                    if (current.type == Type.NUM)
                    {
                        gMult = int.Parse(current.value);
                        GetNext(lexes);
                    }
                    p = ParseSeq(lexes);
                    p = MultDictionary(p, gMult);
                    r = AddDictionary(r, p);
                }

                return r;

            }

            public Dictionary<string, int> ParseSeq(List<Lex> lexes)
            {
                Dictionary<string, int> r = new Dictionary<string, int>();
                while (current.type == Type.EL || current.type == Type.LP)
                {
                    var p = parseEl(lexes);
                    if (current.type == Type.NUM)
                    {
                        p = MultDictionary(p, int.Parse(current.value));
                        r = AddDictionary(r, p);
                        GetNext(lexes);
                    } else
                    {
                        r = AddDictionary(r, p);
                    }
                    /*
                    else if (current.type == Type.EL)
                    {
                        r = AddDictionary(r, p);
                        GetNext(lexes);
                    } else
                    {
                        r = AddDictionary(r, p);
                    }
                    */

                }
                return r;
            }

            public Dictionary<string, int> parseEl(List<Lex> lexes)
            {
                Dictionary<string, int> a = new Dictionary<string, int>();
                if (current.type == Type.EL)
                {
                    a = new Dictionary<string, int>() { { current.value, 1 } };
                    GetNext(lexes);
                    return a;
                }
                else if (current.type == Type.LP)
                {
                    GetNext(lexes);
                    a = ParseSeq(lexes);
                    if (current.type == Type.RP)
                    {
                        GetNext(lexes);
                    }
                    return a;
                }

                return a;
            }

            public Dictionary<string, int> AddDictionary(Dictionary<string, int> a, Dictionary<string, int> b)
            {
                foreach (var el in b)
                {
                    if (a.ContainsKey(el.Key))
                    {
                        a[el.Key] += el.Value;
                    }
                    else
                    {
                        a.Add(el.Key, el.Value);
                    }
                }
                return a;
            }

            public Dictionary<string, int> MultDictionary(Dictionary<string, int> d, int num)
            {
                foreach (var a in d)
                {
                    d[a.Key] *= num;
                }
                return d;
            }
        }
        


        public static List<Lex> splitFormula(string input)
        {
            List<Lex> output = new List<Lex>();
            for (int i = 0; i < input.Length; ++i)
            {
                if (input[i] == '+')
                {
                    output.Add(new Lex {value = input[i].ToString(), type = Type.PLUS });
                }
                else if (input[i] == '(')
                {
                    output.Add(new Lex { value = input[i].ToString(), type = Type.LP });

                }
                else if (input[i] == ')')
                {
                    output.Add(new Lex { value = input[i].ToString(), type = Type.RP });
                }
                else if (input[i] >= 'A' && input[i] <= 'Z')
                {
                    if (i + 1 < input.Length)
                    {
                        if (input[i+1] >= 'a' && input[i+1] <= 'z')
                        {
                            output.Add(new Lex { value = input[i].ToString() + input[i + 1].ToString(), type = Type.EL });
                            ++i;
                            continue;
                        }
                    }
                    output.Add(new Lex { value = input[i].ToString(), type = Type.EL });


                }
                else if (input[i] >= '0' && input[i] <= '9')
                {
                    output.Add(new Lex { value = input[i].ToString(), type = Type.NUM });
                    for (int j = i + 1; j < input.Length; ++j)
                    {
                        if (input[j] >= '0' && input[j] <= '9')
                        {
                            output[output.Count - 1].value += input[j];
                        } 
                        else
                        {
                            i = j - 1;
                            break;
                        }
                        ++i;
                    }
                }
            }
            return output;
        }

    }

}
