using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    public enum TokenType
    {
        NONE, IDENT, INT_LIT, CHAR_LIT, FLOAT_LIT, STRING_LIT, SIZEOF,

        PLUS, MINUS, MUL, DIV, MOD,

        PERIODS, SEMICOLON, COLON, COMMA, PERIOD, LPAR, RPAR, LBRACK,
        RBRACK, LBRACE, RBRACE, TILDA, QUESTION,

        AND, LOGICAL_AND, OR, LOGICAL_OR, XOR,

        PTR_OP, INC, DEC, LSHIFT, RSHIFT,

        LS, GT, LEQ, GEQ, EQ, NEQ, NOT,

        ASSIGN, MUL_ASSIGN, DIV_ASSIGN, MOD_ASSIGN, ADD_ASSIGN, SUB_ASSIGN,
        LSHIFT_ASSIGN, RSHIFT_ASSIGN, AND_ASSIGN, OR_ASSIGN, XOR_ASSIGN,

        TYPE_NAME, ELLIPSIS,

        AUTO, CHAR, CONST, DOUBLE, ELSE, ENUM, EXTERN, FLOAT, INLINE, INT,
        LONG, REGISTER, RESTRICT, SHORT, SIGNED, STATIC, STRUCT, TYPEDEF, UNION,
        UNSIGNED, VOID, VOLATILE,

        ASM, BREAK, CASE, CONTINUE, DEFAULT, DO, FOR, GOTO, IF, RETURN, SWITCH,
        WHILE,

        ALIGNAS, ALIGNOF, ATOMIC, BOOL, COMPLEX, GENERIC, IMAGINARY, NORETURN,
        STATIC_ASSERT, THREAD_LOCAL, __FUNC__, __ATTRIBUTE__, __ASM, __NULLABLE,
        EOF
    }
    public class Token
    {
        public TokenType Type;
        public TokenInfo Info;
        public string Name;
        public int Line;
        public int Column;

        public struct TokenInfo
        {
            public int Ival;
            public float Fval;
            public double Dval;
        }

        public Token()
        {
            Type = TokenType.NONE;
            Info.Ival = 0; Info.Fval = 0;
            Name = ""; Line = 0; Column = 0;
        }
    }

    public class Lexer
    {
        protected StreamReader StreamReader;
        protected int CurrentChar;
        protected int CurrentLine;
        protected int CurrentColumn;

        public Lexer(string filename)
        {
            CurrentLine = 1;
            CurrentColumn = CurrentChar = 0;
            if (string.IsNullOrEmpty(filename))
                StreamReader = new StreamReader(Console.OpenStandardInput());
            else
                StreamReader = new StreamReader(filename);

            if (StreamReader == null)
            {
                Console.WriteLine($"Fatal error: file '{filename}' does not exist!");
                Environment.Exit(1);
            }
        }

        protected static void Error(int curLine, string msg)
        {
            Console.WriteLine($"Error: {msg}; line {curLine}");
            Environment.Exit(1);
        }

        protected static bool IsHexDigit(char c)
        { return c >= 'A' && c <= 'F'; }

        protected static int Powr(int x, int n)
        {
            if (n == 0) return 1;
            else if (n == 1) return x;
            else
            {
                int res = 1;
                for (int i = 0; i < n; i++)
                    res *= x;
                return res;
            }
        }
        public void GetNextChar()
        {
            CurrentChar = StreamReader.Read();
            CurrentColumn++;
            if (CurrentChar == '\n')
            {
                CurrentColumn = 0;
                CurrentLine++;
            }
        }
        public virtual uint next(Token token) => 0;
        ~Lexer()
        {
            StreamReader.Close();
        }
    }
}
