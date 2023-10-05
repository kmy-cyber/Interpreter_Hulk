using System.Collections.Generic;
using INTERPRETE_C_to_HULK;

namespace INTERPRETE_C__to_HULK
{
    public class Program
    {

        public static void Main() 
        {

            Semantic_Analyzer sa = new Semantic_Analyzer();
            string s = "let a = \"hello world\" in print(a + 5);";
             Lexer T =  new Lexer(s);
                    List<Token> TS = T.Tokens_sequency;
                    Parser P = new Parser(TS);
                    //Parser_1 Q = new Parser_1(TS);
                    //Console.WriteLine(Q.Expression());
                    Node N = P.Parse();
                    sa.Read_Parser(N);
                    sa.Choice (N);   

            //while(true)
            //{
            //    Console.Write("> ");
            //    string s = Console.ReadLine();
            //    if(s == "")
            //    {
            //        break;
            //    }
            //    try
            //    {
            //       //string s =Console.ReadLine();
            //        Lexer T =  new Lexer(s);
            //        List<Token> TS = T.Tokens_sequency;
            //        Parser P = new Parser(TS);
            //        //Parser_1 Q = new Parser_1(TS);
            //        //Console.WriteLine(Q.Expression());
            //        Node N = P.Parse();
            //        sa.Read_Parser(N);
            //        
            //        Console.ForegroundColor = ConsoleColor.DarkBlue;
            //        sa.Choice (N);   
            //    }
            //    catch (System.Exception ex)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        //Console.WriteLine($"Error: {ex.Message}");
            //        Console.WriteLine(ex.Message);
            //    }
//
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
        }
    }

}