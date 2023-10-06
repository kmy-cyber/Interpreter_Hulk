
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Xml;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using INTERPRETE_C_to_HULK;

namespace INTERPRETE_C__to_HULK
{
    public class Semantic_Analyzer
    {
        Node AST;

        Dictionary<string,dynamic> variables_globales;
        
        public List<Function_B> functions_declared = new List<Function_B>();
        
        public List<Dictionary<string,dynamic>> Scopes;
        
        public Semantic_Analyzer()
        {
            variables_globales = new Dictionary<string,dynamic>
            {
                {"PI",Math.PI},
                {"TAU",Math.Pow(Math.PI,2)},
                {"true",true},
                {"false",false},
            };
             
        }

        public void Read_Parser(Node n)
        {
            AST = n;
            Scopes = new List<Dictionary<string,dynamic>>{variables_globales};
        }

        public void Choice(Node node)
        {
            switch (node.Type)
            {
                case "declared_function":
                    Console.WriteLine(Evaluate(node));
                    break;  
                default:
                    Evaluate(node);
                    break;                  
            }
        }

        public dynamic? Evaluate(Node node)
        {
            switch (node.Type)
            {
                case "number":
                    return node.Value;
                case "string":
                    return node.Value; 
                case "true":
                    return true;
                case "false":
                    return false;
                case "variable":
                    // return node.Value;
                    return Scopes[Scopes.Count - 1][node.Value];
                case "d_function_name":
                    return node.Value;
                case "+":
                    dynamic ?left_s = Evaluate(node.Children[0]);
                    dynamic ?right_s = Evaluate(node.Children[1]);
                    Type_Expected(right_s,left_s,"number","+");
                    return left_s + right_s;
                    //return Evaluate(node.Children[0]) - Evaluate(node.Children[1]);
                case "-":
                    dynamic ?left_r = Evaluate(node.Children[0]);
                    dynamic ?right_r = Evaluate(node.Children[1]);
                    Type_Expected(right_r,left_r,"number","-");
                    return left_r - right_r;
                    //return Evaluate(node.Children[0]) - Evaluate(node.Children[1]);
                case "*":
                    dynamic ?left_m = Evaluate(node.Children[0]);
                    dynamic ?right_m = Evaluate(node.Children[1]);
                    Type_Expected(right_m,left_m,"number","*");
                    return left_m * right_m;
                    //return Evaluate(node.Children[0]) * Evaluate(node.Children[1]);
                case "/":
                    dynamic ?left_d = Evaluate(node.Children[0]);
                    dynamic ?right_d = Evaluate(node.Children[1]);
                    Type_Expected(right_d,left_d,"number","/");
                    return left_d / right_d;
                    //return Evaluate(node.Children[0]) / Evaluate(node.Children[1]);
                case "^":
                    dynamic ?left_p = Evaluate(node.Children[0]);
                    dynamic ?right_p = Evaluate(node.Children[1]);
                    Type_Expected(right_p,left_p,"number","^");
                    return Math.Pow(left_p,right_p);
                    //return Math.Pow(Evaluate(node.Children[0]),Evaluate(node.Children[1]));
                case "%":
                    dynamic ?left_u = Evaluate(node.Children[0]);
                    dynamic ?right_u = Evaluate(node.Children[1]);
                    Type_Expected(right_u,left_u,"number","%");
                    return left_u % right_u;
                    //return Evaluate(node.Children[0]) % Evaluate(node.Children[1]);
                
                //? Boolean operations
                case ">":
                    return Evaluate(node.Children[0]) > Evaluate(node.Children[1]);
                case "<":
                    return Evaluate(node.Children[0]) < Evaluate(node.Children[1]);
                case ">=":
                    return Evaluate(node.Children[0]) >= Evaluate(node.Children[1]);
                case "<=":
                    return Evaluate(node.Children[0]) <= Evaluate(node.Children[1]);
                case "==":
                    return Evaluate(node.Children[0]) == Evaluate(node.Children[1]);
                case "!=":
                    return Evaluate(node.Children[0]) != Evaluate(node.Children[1]);
                case "@":
                    dynamic ?left_st = Evaluate(node.Children[0]);
                    dynamic ?right_st = Evaluate(node.Children[1]);
                    Type_Expected(right_st,left_st,"string","+");
                    return left_st + right_st;
                    //return Evaluate(node.Children[0]) + Evaluate(node.Children[1]);

                //? Expressions
                case "f_name":
                    return node.Value;
                case "p_name":
                    return node.Value;
                case "name":
                    return node.Value;
                case "print":
                    dynamic? value_print = Evaluate(node.Children[0]);
                    Console.WriteLine(value_print);
                    return value_print;
                case "cos":
                    dynamic? value_cos = Evaluate(node.Children[0]);
                    return Math.Cos(value_cos);
                case "sen":
                    dynamic? value_sen = Evaluate(node.Children[0]);
                    return Math.Sin(value_sen);  
                case "log":
                    dynamic? value_log = Evaluate(node.Children[0]);
                    return Math.Log10(value_log);
                case "Conditional":
                    if(Evaluate(node.Children[0]))
                    {
                        return Evaluate(node.Children[1]);
                    }
                    return Evaluate(node.Children[2]);
                case "Function":
                    Dictionary<string,dynamic> Var = Get_Var_Param(node.Children[1]);
                    Function_B function = new Function_B(node.Children[0].Value,node.Children[2],Var);
                    if(Function_Exist(node.Children[0].Value))
                    {
                        throw new Exception("The function "+ "\' " + node.Children[0].Value + " \'" + "already exist in the current context");
                    }
                    functions_declared.Add(function);
                    return functions_declared;
                case "declared_function":
                    string ?name = node.Children[0].Value;
                    Node param_f = node.Children[1];
                    if(Function_Exist(name))
                    {
                        Dictionary<string,dynamic> Scope_actual = new Dictionary<string,dynamic>();
                        Scopes.Add(Scope_actual);
                        int f_position = Call_Function(functions_declared,name,param_f);
                        dynamic? value = Evaluate(functions_declared[f_position].Operation_Node);
                        Scopes.Remove(Scopes[Scopes.Count-1]);
                        return value;
                    }
                    else
                    {
                        Input_Error ("The function "+ name +" does not exist in the current context");
                    }
                    break;
                case "assigment_list":
                    Save_Var(node);   
                    break;
                case "Let":
                    Evaluate(node.Children[0]);
                    dynamic ?result = Evaluate(node.Children[1]);
                    Scopes.Remove(Scopes[Scopes.Count-1]);
                    return result;
                    
                default:
                    throw new Exception("SEMANTIC ERROR: Unknown operator: " + node.Type);
            }
            return 0;
        }

        private Dictionary<string,dynamic> Get_Var_Param(Node parameters)
        {
            Dictionary<string, dynamic> Param = new Dictionary<string, dynamic>();

            for(int i=0; i<parameters.Children.Count; i++)
            {
                string name = parameters.Children[i].Value;
                Param.Add(name, null);
            } 
            return Param;
        }


        private void Save_Var(Node Children_assigment_list)
        {
            Dictionary<string,dynamic> Var_let_in = new Dictionary<string,dynamic>();
            foreach(string key in Scopes[Scopes.Count - 1].Keys)
            {
                Var_let_in.Add(key, Scopes[Scopes.Count - 1][key]);
            }
            foreach (Node Child in Children_assigment_list.Children)
            {
                string name = Child.Children[0].Value;
                dynamic value = Evaluate(Child.Children[1]);
                if(Var_let_in.ContainsKey(name))
                {
                    Input_Error ("The variable "+ name +" already has a definition in the current context");
                }
                if(Function_Exist(name))
                {
                    Input_Error ("The variable "+ name +" already has a definition as a function in the current context");
                }
                
                Var_let_in.Add(name, value);
            }
            Scopes.Add(Var_let_in);
        }

        private int Call_Function(List<Function_B> f,string name, Node param)
        {
            bool is_found = false;
            for(int i=0; i<f.Count; i++)
            {
                if(f[i].Name_function == name)
                {
                    is_found = true;
                    if(f[i].variable_param.Count == param.Children.Count )
                    {
                        
                        foreach(string key in Scopes[Scopes.Count - 2].Keys)
                        {
                            Scopes[Scopes.Count - 1].Add(key, Scopes[Scopes.Count - 2][key]);
                        }

                        int count = 0;
                        foreach(string key in f[i].variable_param.Keys)
                        {
                            f[i].variable_param[key] = param.Children[count].Value;
                            if(Scopes[Scopes.Count - 1].ContainsKey(key))
                            {
                                Scopes[Scopes.Count - 1][key] = Evaluate(param.Children[count].Value);
                                count++;
                            }
                            else
                            {
                                Scopes[Scopes.Count - 1].Add(key, Evaluate(param.Children[count].Value));
                                count++;
                            }
                        }

                        return i; 
                    }
                    else
                    {
                        Input_Error ("Function "+ name + " receives " +f[i].variable_param.Count+" argument(s), but "+ param.Children.Count +" were given.");
                    }
                }
            }
            if(!is_found)
            {
                Input_Error ("The function "+ name +" has not been declared");
            }

            return -1;
        }

        private bool Function_Exist(string name)
        {
            foreach( Function_B b in functions_declared)
            {
                if(b.Name_function == name)
                {
                    return true;
                }
            }
            return false;
        }

        private void Input_Error(string error )
        {
            throw new Exception("SEMANTIC ERROR: " + error);
        }

        private void Type_Expected(dynamic value1, dynamic value2 , string type, string op)
        {
            if(value1 is double && value2 is double && type == "number")
            {
                return;
            }
            else if(value1 is string && value2 is string && type == "string")
            {
                return;
            }
            else
            {
                Input_Error("Operator \'"+ op+"\' cannot be used between \'" + value1 +"\' and \'"+ value2 +"\'");
            }
        }
        
    }
}