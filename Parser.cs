using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    public enum DeclarationKind
    {
        DK_VARIABLE, DK_PARAMETR, DK_FIELD,
        DK_TYPE_DECL, DK_IDENT
    }
    public class Parser
    {
        private const int N = 4;

        protected CLexer lexer;
        public Symbols symbols { get; protected set; }
        public AST ast { get; protected set; }

        protected TokenType sym;
        protected int parsErrors;

        protected Dictionary<TokenType, string> name;
        protected static Dictionary<string, string[]> Headers = new()
        {
            ["stdio.h"] = new[]{ "printf","scanf","puts","gets",
                                  "putchar","getchar","fopen","fclose",
                                  "fread","fwrite" },
            ["stdlib.h"] = new[]{ "malloc","calloc","realloc","free",
                                  "exit","atoi","atof","rand","srand" },
            ["string.h"] = new[]{ "strlen","strcmp","strncmp","strcpy",
                                  "strncpy","strcat","memcpy","memset" },
            ["math.h"] = new[]{ "sin","cos","tan","sqrt","pow",
                                  "fabs","log","exp" },
            ["time.h"] = new[]{ "time","clock","difftime","asctime",
                                  "ctime","strftime","localtime" },
            ["setjmp.h"] = new[] { "setjmp", "longjmp" }
        };
        public bool HasErrors { get; private set; }
        public StringBuilder ErrorLog { get; } = new StringBuilder();

        protected Token tok, la;
        protected Token[] buffer = new Token[N];
        protected uint tokIndex, laIndex, newLaIndex;

        protected void GetToken()
        {
            tokIndex = laIndex;
            laIndex = (1 + laIndex) % N;
            newLaIndex = (2 + laIndex) % N;

            tok = buffer[tokIndex];

            sym = buffer[laIndex].Type;

            lexer.Next(ref buffer[newLaIndex]);
        }

        protected void RequireDeclared(string name, int line)
        {
            if (MacroTable.Contains(name)) return;
            if (symbols.Find(name) == symbols.NoObj)
                ParsingError($"identifier '{name}' is not declared (line {line})");
        }
        protected TokenType GetLATok(uint k)
        {
            Debug.Assert(k < N, "k is greater than number of possible lookahead tokens");

            return buffer[(tokIndex + k) % N].Type;
        }

        protected string GetLATokInfoSval(uint k)
        {
            Debug.Assert(k < N, "k is greater than number of possible lookahead tokens");

            return buffer[(tokIndex + k) % N].Name;
        }

        protected virtual void ParsingError(string msg)
        {
            ErrorLog.AppendLine($"[Error] (Line {tok.Line}): {msg}");
            HasErrors = true;
        }
        protected void SkipToMatchingRBrace()
        {
            int depth = 0;
            while (sym != TokenType.EOF)
            {
                if (sym == TokenType.LBRACE)
                {
                    depth++;
                }
                else if (sym == TokenType.RBRACE)
                {
                    if (depth == 0)
                        return;
                    depth--;
                }
                GetToken();
            }
        }
        protected void check(TokenType expected)
        {
            if (sym == expected) GetToken();
            else ParsingError("expected '%s' " + name[expected]);
        }

        protected void initTokenBuffer()
        {
            sym = lexer.Next(ref buffer[0]);
            lexer.Next(ref buffer[1]);
            lexer.Next(ref buffer[2]);
            laIndex = 0;
        }

        protected virtual void initName() { }

        protected void RegisterHeaderFunctions()
        {
            if (lexer is not CLexer clex) return;

            foreach (var header in clex.Includes)
            {
                if (!Headers.TryGetValue(header, out var funcs)) continue;

                foreach (var f in funcs)
                {
                    if (symbols.Find(f) != symbols.NoObj) continue;

                    var ftype = symbols.AllocType(TypeKind.TK_FUNCTION);
                    ftype.FuncType = symbols.IntType;

                    symbols.Insert(f, ObjectKind.OK_FUNC, ftype);
                }
                if (header == "setjmp.h")
                {
                    var jbType = symbols.AllocType(TypeKind.TK_ARRAY);
                    jbType.ElType = symbols.IntType;    
                    jbType.Size = 1;

                    symbols.Insert("jmp_buf", ObjectKind.OK_TYPE, jbType);
                }
            }
        }
        public virtual void parse(string output) { }

        public int GetCurLine() => tok.Line;

        public Parser(CLexer lexer)
        {
            name = new Dictionary<TokenType, string>();
            this.lexer = lexer;
            parsErrors = 0;
            ast = new AST(symbols);
            for (int i = 0; i < N; i++) buffer[i] = new Token();
            symbols = new Symbols();
            ast = new AST(symbols);
        }
        public void Dispose()
        {
            lexer?.Dispose();   
        }
    }
}
