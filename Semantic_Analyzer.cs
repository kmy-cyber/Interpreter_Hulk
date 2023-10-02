
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
            Evaluate(node);
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
                    return Evaluate(node.Children[0]) + Evaluate(node.Children[1]);
                case "-":
                    return Evaluate(node.Children[0]) - Evaluate(node.Children[1]);
                case "*":
                    return Evaluate(node.Children[0]) * Evaluate(node.Children[1]);
                case "/":
                    return Evaluate(node.Children[0]) / Evaluate(node.Children[1]);
                case "^":
                    return Math.Pow(Evaluate(node.Children[0]),Evaluate(node.Children[1]));
                case "%":
                    return Evaluate(node.Children[0]) % Evaluate(node.Children[1]);
                
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
                    return Evaluate(node.Children[0]) + Evaluate(node.Children[1]);

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
                        throw new Exception("La funcion"+ node.Children[0]+"ya tiene una definicion en el contexto actual");
                    }
                    functions_declared.Add(function);
                    
                    return functions_declared;
                case "declared_function":
                    string name = node.Children[0].Value;
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
                        throw new Exception("La funcion"+ name +" no existe declarada en el contexto actual");
                    }
                break;
                case "assigment_list":
                    Save_Var(node);   
                    break;
                case "Let":
                    Evaluate(node.Children[0]);
                    dynamic result = Evaluate(node.Children[1]);
                    Scopes.Remove(Scopes[Scopes.Count-1]);
                    return result;
                    
                default:
                    throw new Exception("Operador desconocido: " + node.Type);
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
                    throw new Exception ("La variable"+ name +" ya tiene una definicion (ya existe en el contexto actual)");
                }
                if(Function_Exist(name))
                {
                    throw new Exception("La variable"+ name +" que intenta declarar ya tiene una definicion como funcion");
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
                        throw new Exception ("La cantidad de parametros insertados en la funcion "+ name +" no coincide con los que debe recibir la funcion");
                    }
                }
            }
            if(!is_found)
            {
                throw new Exception ("La funcion "+ name +" no ha sido declarada");
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
    }
}