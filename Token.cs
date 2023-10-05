namespace INTERPRETE_C__to_HULK
{
    public enum TokenType
    {
        //?TIPO DE DATOS
        NUMBER,
        STRING,
        BOOLEAN,

        //?OPERADORES
        OPERATOR,
       // PLUS,
       // MINUS,
       // MULTIPLY,
       // DIVIDE,
        EQUAL,
        DO,

        //PUNTUADORES
        L_PHARENTESYS,
        R_PHARENTESYS,
        PRINT,
        COMMA,
        D_COMMA,
        COMMILLAS,

        //?Own Words
        LET,
        IN,
        IF,
        ELSE,
        TRUE,
        FALSE,
        FUNCTION,

        //? Reserved Word
        COS,
        SEN,
        LOG,

        //?
        VARIABLE,
        EOF
    }

    public class Token {
        public TokenType Type { get; }
        public object Value { get; }

        public Token(TokenType type, object value) {
            Type = type;
            Value = value;
        }

        public override string ToString() {
            return $"Token({Type}, {Value})";
        }
    }

}