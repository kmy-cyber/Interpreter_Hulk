using System.Globalization;
using System.IO;
using INTERPRETE_C__to_HULK;

namespace INTERPRETE_C_to_HULK
{
    public class Function_B
    {
        public Dictionary<string , dynamic> variable_param;
        public string Name_function;
        public Node Operation_Node;

        public Function_B(string name, Node node,Dictionary<string , dynamic> param )
        {
            this.Name_function = name;
            this.Operation_Node = node;
            this.variable_param = param;
        }

        //public void Call_Function()
        //{
        //    Semantic_Analyzer sa = new Semantic_Analyzer(Operation_Node);
        //    dynamic answer = sa.Evaluate(Operation_Node);
        //}
    }
}