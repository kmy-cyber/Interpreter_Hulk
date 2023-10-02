using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace INTERPRETE_C__to_HULK
{
    public class Lexer
    {
        string Text; //texto de entrada
        int position;
        //Token currentToken;
        char currentChar;
        public List<Token> Tokens_sequency { get; }

        public Lexer(string text)
        {
            Text = text;
            position = 0; //es 0 para empezar a analizar desde el primer elemento de la entrada
            currentChar = Text[position]; 
            //currentToken = Get_token();
            Tokens_sequency = Get_Sequency(Text,position);
        }



    #region  TOKENS
        public Token Get_token()
        {
            while(currentChar != '\0')
            {
                if(char.IsWhiteSpace(currentChar))
                {
                    Skip_Space();
                    continue;
                }

                if(char.IsDigit(currentChar) )
                {
                    return new Token(TokenType.NUMBER,Int_Analyzer());
                }

                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    string word = String_Analyzer();
                    return Own_Words(word);
                    
                }

                if(currentChar == '+')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '+');
                }

                if(currentChar == '-')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'-');
                }

                if(currentChar == '*')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'*');
                }

                if(currentChar == '/')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'/');
                }

                if(currentChar == '^')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'^');
                }

                if(currentChar == '%')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'%');
                } 

                if(currentChar == '=')
                {
                    Move_on();
                    if(currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "==");
                    }
                    else if(currentChar == '>')
                    {
                        Move_on();
                        return new Token(TokenType.DO, "=>");
                    }
                    else
                    {
                        return new Token(TokenType.EQUAL, "=");
                    }
                }

                if(currentChar == '!')
                {
                    Move_on();
                    if(currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "!=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, "!");
                    }
                }

                if(currentChar == '<')
                {
                    Move_on();
                    if(currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, "<=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, "<");
                    }
                }

                if(currentChar == '>')
                {
                    Move_on();
                    if(currentChar == '=')
                    {
                        Move_on();
                        return new Token(TokenType.OPERATOR, ">=");
                    }
                    else
                    {
                        return new Token(TokenType.OPERATOR, ">");
                    }
                }

                if(currentChar == '&')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '&');
                }

                if(currentChar == '|')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR, '|');
                }

                if(currentChar == '(')
                {
                    Move_on();
                    return new Token(TokenType.L_PHARENTESYS,'(');
                }

                if(currentChar == ')')
                {
                    Move_on();
                    return new Token(TokenType.R_PHARENTESYS,')');
                }

                if(currentChar == ';')
                {
                    Move_on();
                    return new Token(TokenType.D_COMMA,';');
                }

                if(currentChar == ',')
                {
                    Move_on();
                    return new Token(TokenType.COMMA,',');
                }

                if(currentChar == '\"')
                {
                    Move_on();
                    (bool,string,int) s = Read(position,Text);
                    if(!s.Item1)
                    {
                        Input_Error(" no ha cerrado las comillas, string invalido");
                    }
                    position = s.Item3;
                    Move_on();
                    return new Token(TokenType.STRING,s.Item2);
                }

                if(currentChar == '@')
                {
                    Move_on();
                    return new Token(TokenType.OPERATOR,'@');
                }

                Input_Error("caracter" + currentChar + "no reconocido"); 
            }
            return new Token(TokenType.EOF,null);
        }
    #endregion
       
        public Token Own_Words(string word)
        {

            switch (word)
            {
                case "print":
                    return new Token(TokenType.PRINT,"print");
                case "let":
                    return new Token(TokenType.LET,"let");
                case "in":
                    return new Token(TokenType.IN,"in");
                case "if":
                    return new Token(TokenType.IF,"if");
                case "else":
                    return new Token(TokenType.ELSE,"else");
                case "function":
                    return new Token(TokenType.FUNCTION,"function");
                case "PI":
                    return new Token(TokenType.NUMBER,Math.PI);
                case "TAU":
                    return new Token(TokenType.NUMBER,Math.Pow(Math.PI,2));
                case "true":
                    return new Token(TokenType.TRUE,"true");
                case "false":
                    return new Token(TokenType.TRUE,"false");
                case "cos":
                    return new Token(TokenType.COS,"cos");
                case "sen":
                    return new Token(TokenType.SEN,"sen");
                default:
                    return new Token(TokenType.VARIABLE, word);
            }
        }

        #region Auxiliares

        public void Input_Error(string error )
        {
            throw new Exception(error);
        }

        public void Move_on()//avanza al siguiente
        {
            position++;
            if(position < Text.Length)
            {
                currentChar = Text[position];
            }
            else
            {
                currentChar = '\0';
            }
        }

        public void Skip_Space()//salta espacios en blanco
        {
            while(currentChar != '\0' && char.IsWhiteSpace(currentChar))
            {
                Move_on();
            } 
        }

        public int Int_Analyzer()
        {
            string number = "";
            while(currentChar != '\0' && char.IsDigit(currentChar))
            {
                number+= currentChar;
                Move_on();
            }
            return int.Parse(number);
        }

        public string String_Analyzer()
        {
            string value = "";
            while (position < Text.Length && (char.IsLetterOrDigit(currentChar)|| currentChar == '_'))
            {
                value += currentChar;
                Move_on();
            }
            return value;
        }
        static (bool,string,int) Read(int position, string text)
        {
            string s = "";
            while( position < text.Length && text[position] != '\"')
            {
                s+=text[position];
                position++;
            }
            if( position <= text.Length && text[position] == '\"' )
            {

                return (true,s,position);
            }

            return (false,"",0);
        }
        
        public List<Token> Get_Sequency(string text,int position)
        {
            List<Token> TS = new List<Token>();
            while(text[position] != '\0')
            {
                Token currentToken = Get_token();
                TS.Add(currentToken);
                if(currentToken.Type == TokenType.EOF)
                {
                    break;
                }
            }
            return TS;
        }
        #endregion


    }
}