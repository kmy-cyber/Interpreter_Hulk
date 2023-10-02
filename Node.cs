using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace INTERPRETE_C__to_HULK
{
    public class Node
    {

        public string? Type { get; set; }
        public dynamic? Value { get; set; }
        public List<Node> Children { get; set; }

        public Node()
        {
            Children = new List<Node>();
        }
        
    }
}