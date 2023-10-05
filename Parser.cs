using System.Text.RegularExpressions;
using System.Dynamic;
using System.Linq.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection.Metadata.Ecma335;

namespace INTERPRETE_C__to_HULK
{
    public class Parser
    {
        List<Token> TS;
        int position;

        public Dictionary<string,dynamic> Variables;

        public Parser(List<Token>Tokens_Sequency)
        {
            position = 0;
            TS = Tokens_Sequency;
            Variables = new Dictionary<string, dynamic>();

        }

        public Node Parse()
        {
            Node Tree =  Global_Layer();
            Expect(TokenType.D_COMMA, ";");
            return Tree;
        }
        
        public Node Global_Layer()
        {
            if( position < TS.Count && Convert.ToString(TS[position].Value) == "print" )
            {
                return Showing();
            }

            if( position < TS.Count && Convert.ToString(TS[position].Value) == "let" )
            {
                return Assigment();
            }

            if( position < TS.Count && Convert.ToString(TS[position].Value) == "if" )
            {
                return Conditional();
            }

            if( position < TS.Count && Convert.ToString(TS[position].Value) == "function" )
            {
                return Function();
            }

            return Layer_6(); 
        }

    	
        public Node Assigment()
        {
            position++;
            Node assigments = new Node{ Type = "assigment_list"};
            bool comma = false;

            do{
                if (comma)
                {
                    position++;
                }
                comma = true;

                Expect(TokenType.VARIABLE,"nombre_de_variable");
                Node name = new Node { Type = "name" , Value = TS[position-1].Value};
                Expect(TokenType.EQUAL,"=");
                Node value = Layer_6();

                Node var = new Node { Type = "assigment", Children = new List<Node>{name,value}}; 
                assigments.Children.Add(var);

            }while(TS[position].Type == TokenType.COMMA);

            Expect(TokenType.IN,"in");
            Node operations = Global_Layer();

            Node variable = new Node { Type = "Let", Children = new List<Node>{assigments,operations} }; 
            return variable;
        }

        public Node Showing()
        {
            position++;
            Expect(TokenType.L_PHARENTESYS, "(");
            
            Node expression = Global_Layer();
            Expect(TokenType.R_PHARENTESYS,")");
            return new Node {Type = "print", Children = new List<Node>{expression}};
        }

        public Node Conditional()
        {
            position++;
            Expect(TokenType.L_PHARENTESYS,"(");
            Node condition = Layer_6();
            Expect(TokenType.R_PHARENTESYS,")");
            Node operations_if = Global_Layer();
            Expect(TokenType.ELSE,"else");
            Node operations_else = Global_Layer();
            Node conditional_if_else = new Node { Type = "Conditional", Children = new List<Node>{condition,operations_if,operations_else} }; 
            return conditional_if_else;
        }

        public Node Function()
        {
            position++;
            Node parammeters = new Node{ Type = "parameters"};

            Expect( TokenType.VARIABLE,"nombre_de_funcion" );
            Node function_name = new Node { Type = "f_name" , Value = TS[position-1].Value};
            Expect( TokenType.L_PHARENTESYS,"(" );

            while (TS[position].Type == TokenType.VARIABLE)
            {
                Expect( TokenType.VARIABLE,"nombre_del_parametro" );
                Node parammeter_name = new Node { Type = "p_name" , Value = TS[position-1].Value};
                parammeters.Children.Add(parammeter_name);
                
                if(TS[position].Type == TokenType.COMMA)
                {
                    position++;
                }
            }
            
            Expect( TokenType.R_PHARENTESYS,")" );
            Expect( TokenType.DO, "=>");
            Node operation = Global_Layer();
            Node function = new Node { Type = "Function", Children = new List<Node>{ function_name, parammeters, operation}};
            return function; 
        }

        #region CAPAS

            public Node Layer_6()
            {
                Node node = Layer_5();
                if(position < TS.Count && Convert.ToString(TS[position].Value) == "@" )
                {
                    string? op = Convert.ToString(TS[position ++].Value);
                    Node right = Layer_5();
                    node = new Node { Type = op, Children = new List<Node>{node,right}};
                }
                return node;
            }

            public Node Layer_5()
            {
                Node node = Layer_4();
                while(position < TS.Count && (Convert.ToString(TS[position].Value) == "&" || Convert.ToString(TS[position].Value) == "|"))
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Layer_4();
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }
            public Node Layer_4()
            {
                Node node = Layer_3();
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "==" || Convert.ToString(TS[position].Value) == "!=" || Convert.ToString(TS[position].Value) == "<=" || Convert.ToString(TS[position].Value) == ">=" || Convert.ToString(TS[position].Value) == "<" || Convert.ToString(TS[position].Value) == ">"  ))
                {
                    string? op =  Convert.ToString(TS[position++].Value);
                    Node right = Layer_3();
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            public Node Layer_3()
            {
                Node node = Layer_2();
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "+" || Convert.ToString(TS[position].Value) == "-"))
                {
                    string? op =  Convert.ToString(TS[position++].Value);
                    Node right = Layer_2();
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;

            }

            public Node Layer_2()
            {
                Node node = Layer_1();
                string? a = Convert.ToString(TS[position].Value);
                while (position < TS.Count && (Convert.ToString(TS[position].Value) == "*" || Convert.ToString(TS[position].Value) == "/" || Convert.ToString(TS[position].Value) == "%"))
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Layer_1();
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }

            public Node Layer_1()
            {
                Node node = Factor();
                while(position < TS.Count && Convert.ToString(TS[position].Value) == "^" )
                {
                    string? op = Convert.ToString(TS[position++].Value);
                    Node right = Factor();
                    node = new Node { Type = op, Children = new List<Node> { node, right } };
                }
                return node;
            }


            public Node Factor ()
            {
                Token current_token= TS[position];
                if (position >= TS.Count)
                    throw new Exception("Unexpected end of input");
                if(current_token.Type == TokenType.L_PHARENTESYS)
                {
                    position++;
                    Node node = Layer_5();
                    if(position>=TS.Count && TS[position].Type != TokenType.R_PHARENTESYS )
                    {
                        Input_Error(" ')' Expected!");
                    }
                    position++;
                    return node;
                }

                else if( TS[position].Type == TokenType.NUMBER )
                {
                    dynamic value = Convert.ToDouble(TS[position++].Value);
                    return new Node { Type = "number", Value = value };
                }

                else if( TS[position].Type == TokenType.TRUE)
                {
                    dynamic value = Convert.ToDouble(TS[position++].Value);
                    return new Node { Type = "true", Value = value };
                }

                else if( TS[position].Type == TokenType.FALSE )
                {
                    dynamic value = Convert.ToDouble(TS[position++].Value);
                    return new Node { Type = "false", Value = value };
                }

                else if( TS[position].Type == TokenType.STRING )
                {
                    dynamic? value = Convert.ToString(TS[position++].Value);
                    return new Node { Type = "string", Value = value};
                }

                else if( TS[position].Type == TokenType.VARIABLE)
                {
                    if(TS[position+1].Type == TokenType.L_PHARENTESYS)
                    {
                        dynamic? f_name = Convert.ToString(TS[position++].Value);
                        position++;
                        Node name =  new Node { Type = "d_function_name", Value = f_name};
                        Node param = new Node{ Type = "parameters"};
                        if(TS[position].Type != TokenType.R_PHARENTESYS)
                        {
                            do
                            {
                                Node parammeter_name = new Node { Type = "p_name" , Value = Layer_6()};
                                param.Children.Add(parammeter_name);

                                if(TS[position].Type == TokenType.COMMA)
                                {
                                    position++;
                                }
                            }while (TS[position-1].Type == TokenType.COMMA);
                        }

                        Expect(TokenType.R_PHARENTESYS, ")");
                        return new Node { Type = "declared_function", Children = new List<Node> {name,param}};

                    }

                    dynamic? value = Convert.ToString(TS[position++].Value);
                    return new Node { Type = "variable", Value = value};
                }

                else if(TS[position].Type == TokenType.COS)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "cos", Children = new List<Node>{valor}};
                }

                else if(TS[position].Type == TokenType.SEN)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "sen", Children = new List<Node>{valor}};
                }

                else if(TS[position].Type == TokenType.LOG)
                {
                    position++;
                    Expect(TokenType.L_PHARENTESYS,"(");
                    Node valor = Layer_6();
                    Expect(TokenType.R_PHARENTESYS, ")");
                    return new Node {Type = "log", Children = new List<Node>{valor}};
                }
                 
                else if( position < TS.Count && Convert.ToString(TS[position].Value) == "let" )
                {
                    return Assigment();
                }

                else if(TS[position] == null)
                {
                    return new Node{};
                }

                else
                {
                    Input_Error("its not a number ,')' or a string" );
                    return new Node{Type="error",Value=0};
                }
            }
        #endregion

        #region Auxiliar

            private void Input_Error(string error)
            {
                throw new Exception("SYNTAX ERROR: " + error);
            }

            public void Expect(TokenType tokenType, object value)
            {
                if(TS[position].Type == tokenType)
                {
                    position++;
                }
                else
                {
                    Input_Error("[" +position + "] " + (string)value + " Expected!, " + TS[position].Value + "was received");
                }
            }




        #endregion

    }
}