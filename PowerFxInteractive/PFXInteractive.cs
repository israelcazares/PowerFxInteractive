using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.PowerFx;
using System.Linq;
using Microsoft.PowerFx.Types;


namespace PowerFxInteractive
{
    public class PFXInteractive
    {
        private static RecalcEngine myPowerFXEngine;
        public string myExpression;

        private readonly string[] myPFXFunction = new string[] { "Set", "Collect", "ClearCollect" };

        public HashSet<(string formula, string result)> Result { get; set; } = new HashSet<(string formula, string result)>();


        public PFXInteractive(RecalcEngine engine, string expression)
        {
            myPowerFXEngine = engine;
            myExpression = expression;
        }

        public string formatExpression(string s)
        {
            var output = s.Replace(" ", "&nbsp;");
            output = string.Join("<br>", output.Split("\n"));
            return "<br>" + output + "<br><br>";
        }


        public void EvalPowerFx(string expr)
        {

            var cleanExpression = string.Join("", expr.Trim().Split("\n")).TrimEnd(';');

            try
            {
                Match match = myPFXFunction.Select(PFXFunction => Regex.Match(cleanExpression, @$"^\s*{PFXFunction}\(\s*(?<ident>\w+)\s*,\s*(?<expr>.*)\)\s*$")).FirstOrDefault(x => x.Success);

                // variable assignment: Set( <ident>, <expr> )
                if (match != null)
                {
                    var r = myPowerFXEngine.Eval(match.Groups["expr"].Value);
                    myPowerFXEngine.UpdateVariable(match.Groups["ident"].Value, r);


                    Result.Add((formatExpression(expr), PrintResult(r)));
                }

                // formula definition: <ident> = <formula>
                else if ((match = Regex.Match(cleanExpression, @"^\s*(?<ident>\w+)\s*=(?<formula>.*)$")).Success)
                {

                    myPowerFXEngine.SetFormula(match.Groups["ident"].Value, match.Groups["formula"].Value, (string name, FormulaValue newValue) =>
                    {
                        OnUpdate(expr, name, newValue);
                    });

                }
                // eval and print everything else, unless empty lines and single line comment (which do nothing)
                else if (!Regex.IsMatch(cleanExpression, @"^\s*//") && Regex.IsMatch(cleanExpression, @"[^\s]"))
                {
                    var r = myPowerFXEngine.Eval(cleanExpression);

                    if (r is ErrorValue errorValue)
                    {
                        Result.Add((formatExpression(expr), @$"{{""Error"": ""{errorValue.Errors[0].Message}""}}"));

                    }
                    else
                    {
                        Result.Add((formatExpression(expr), PrintResult(r)));
                    }
                }
            }
            catch (Exception e)
            {
                Result.Add((formatExpression(expr), e.Message));
            }

            //end of function
        }


        public PFXInteractive ParseFx()
        {

            string[] expressions = null;

            expressions = myExpression.Contains(";") ? myExpression.Split($";\n") : myExpression.Split("\n");

            foreach (var expression in expressions)
            {
                EvalPowerFx(expression);
            }
            return this;

        }

        private void OnUpdate(string expr, string name, FormulaValue newValue)
        {
            //Console.Write($"{name}: ");

            if (newValue is ErrorValue errorValue)
            {

                Result.Add((expr, $@"{{""Error"": ""{errorValue.Errors[0].Message}""}}"));
            }
            else
            {
                Result.Add((formatExpression(expr), PrintResult(newValue)));
                
            }
        }

        static string PrintResult(object value)
        {
            string resultString = "";

            if (value is BlankValue)
                resultString = "Blank()";
            else if (value is RecordValue record)
            {
                var separator = "";
                resultString = "{";
                foreach (var field in record.Fields)
                {
                    resultString += separator + $"{field.Name}:";
                    resultString += PrintResult(field.Value);
                    separator = ", ";
                }
                resultString += "}";
            }
            else if (value is TableValue table)
            {
                int valueSeen = 0, recordsSeen = 0;
                string separator = "";

                // check if the table can be represented in simpler [ ] notation,
                //   where each element is a record with a field named Value.
                foreach (var row in table.Rows)
                {
                    recordsSeen++;
                    if (row.Value is RecordValue scanRecord)
                    {
                        foreach (var field in scanRecord.Fields)
                            if (field.Name == "Value")
                            {
                                valueSeen++;
                                resultString += separator + PrintResult(field.Value);
                                separator = ", ";
                            }
                            else
                                valueSeen = 0;
                    }
                    else
                        valueSeen = 0;
                }

                if (valueSeen == recordsSeen)
                    return ("[" + resultString + "]");
                else
                {
                    // no, table is more complex that a single column of Value fields,
                    //   requires full treatment
                    resultString = "Table(";
                    separator = "";
                    foreach (var row in table.Rows)
                    {
                        resultString += separator + PrintResult(row.Value);
                        separator = ", ";
                    }
                    resultString += ")";
                }
            }
            else if (value is ErrorValue errorValue)
                resultString = "<Error: " + errorValue.Errors[0].Message + ">";
            else if (value is StringValue str)
                resultString = "\"" + str.ToObject().ToString().Replace("\"", "\"\"") + "\"";
            else if (value is FormulaValue fv)
                resultString = fv.ToObject().ToString();
            else
                throw new Exception("unexpected type in PrintResult");

            return resultString;
        }
    }
}