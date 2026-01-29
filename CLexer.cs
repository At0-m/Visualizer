using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{

    public class CLexer : Lexer, IDisposable
    {
        private Dictionary<string, TokenType> keyWords;
        private Dictionary<string, TokenType> firMention;
        private StreamReader sr;
        public readonly List<string> Includes = new();
        private readonly Stack<Token> _macroBuf = new();
        private class CLexerFromString : CLexer
        {
            private bool _isEofReached;

            public CLexerFromString(string src)
                : base("")
            {
                StreamReader?.Dispose();
                StreamReader = new StreamReader(new MemoryStream(
                    Encoding.UTF8.GetBytes(src + "\n")));
                CurrentChar = CurrentLine = CurrentColumn = 0;
                _isEofReached = false;
                GetNextChar();
            }

            public override TokenType Next(ref Token token)
            {
                if (_isEofReached)
                {
                    token.Type = TokenType.EOF;
                    return token.Type;
                }

                var type = base.Next(ref token);

                if (type == TokenType.EOF)
                    _isEofReached = true;

                return type;
            }
        }
        public CLexer(string s) : base(s)
        {
            sr = base.StreamReader;
            keyWords = new Dictionary<string, TokenType>();
            firMention = new Dictionary<string, TokenType>();
            iniz();
        }

        private void iniz()
        {
            keyWords["Alignas"] = TokenType.ALIGNAS;
            keyWords["Alignof"] = TokenType.ALIGNOF;
            keyWords["Atomic"] = TokenType.ATOMIC;
            keyWords["Bool"] = TokenType.BOOL;
            keyWords["Complex"] = TokenType.COMPLEX;
            keyWords["Generic"] = TokenType.GENERIC;
            keyWords["Imaginary"] = TokenType.IMAGINARY;
            keyWords["Noreturn"] = TokenType.NORETURN;
            keyWords["Static_assert"] = TokenType.STATIC_ASSERT;
            keyWords["Thread_local"] = TokenType.THREAD_LOCAL;
            keyWords["__func__"] = TokenType.__FUNC__;
            keyWords["__attribute__"] = TokenType.__ATTRIBUTE__;
            keyWords["__asm"] = TokenType.__ASM;

            keyWords["auto"] = TokenType.AUTO;
            keyWords["char"] = TokenType.CHAR;
            keyWords["const"] = TokenType.CONST;
            keyWords["double"] = TokenType.DOUBLE;
            keyWords["else"] = TokenType.ELSE;
            keyWords["enum"] = TokenType.ENUM;
            keyWords["extern"] = TokenType.EXTERN;
            keyWords["float"] = TokenType.FLOAT;
            keyWords["inline"] = TokenType.INLINE;
            keyWords["int"] = TokenType.INT;
            keyWords["long"] = TokenType.LONG;
            keyWords["register"] = TokenType.REGISTER;
            keyWords["restrict"] = TokenType.RESTRICT;
            keyWords["short"] = TokenType.SHORT;
            keyWords["sizeof"] = TokenType.SIZEOF;
            keyWords["signed"] = TokenType.SIGNED;
            keyWords["static"] = TokenType.STATIC;
            keyWords["struct"] = TokenType.STRUCT;
            keyWords["typedef"] = TokenType.TYPEDEF;
            keyWords["union"] = TokenType.UNION;
            keyWords["unsigned"] = TokenType.UNSIGNED;
            keyWords["void"] = TokenType.VOID;
            keyWords["volatile"] = TokenType.VOLATILE;

            firMention["Generic"] = TokenType.GENERIC;
            firMention["asm"] = TokenType.ASM;
            firMention["break"] = TokenType.BREAK;
            firMention["case"] = TokenType.CASE;
            firMention["continue"] = TokenType.CONTINUE;
            firMention["default"] = TokenType.DEFAULT;
            firMention["do"] = TokenType.DO;
            firMention["for"] = TokenType.FOR;
            firMention["goto"] = TokenType.GOTO;
            firMention["if"] = TokenType.IF;
            firMention["return"] = TokenType.RETURN;
            firMention["switch"] = TokenType.SWITCH;
            firMention["while"] = TokenType.WHILE;

        }

        private TokenType Keyword(string s)
        {
            if (keyWords.TryGetValue(s, out TokenType Type))
                return Type;
            else
            {
                if (firMention.TryGetValue(s, out Type))
                    return Type;
                return TokenType.IDENT;
            }

        }

        private void ReadName(Token token)
        {
            token.Name = "";
            token.Name += (char)CurrentChar;
            GetNextChar();
            while (char.IsLetterOrDigit((char)CurrentChar) || CurrentChar == '_')
            {
                token.Name += (char)CurrentChar;
                GetNextChar();
            }
            bool expanded = false;
            if (MacroTable.TryGet(token.Name, out var mac) && mac.Length <= MacroTable.INLINE_LIMIT && !mac.HasIdents)
            {
                PushMacroTokens(mac.Body);
                var first = _macroBuf.Pop();
                token.Type = first.Type;
                token.Name = first.Name;
                token.Info = first.Info;
                expanded = true;
            }
            if(!expanded) token.Type = Keyword(token.Name);
        }
        private void ReadNumberLit(Token token)
        {
            var sb = new StringBuilder();
            bool isFloat = false, forceFloat = false;

            while (char.IsDigit((char)CurrentChar))
            { sb.Append((char)CurrentChar); GetNextChar(); }

            if (CurrentChar == '.')
            {
                isFloat = true;
                sb.Append('.'); GetNextChar();
                while (char.IsDigit((char)CurrentChar))
                { sb.Append((char)CurrentChar); GetNextChar(); }
            }

            if (CurrentChar is 'e' or 'E')
            {
                isFloat = true;
                sb.Append((char)CurrentChar); GetNextChar();
                if (CurrentChar is '+' or '-')
                { sb.Append((char)CurrentChar); GetNextChar(); }
                while (char.IsDigit((char)CurrentChar))
                { sb.Append((char)CurrentChar); GetNextChar(); }
            }

            if (CurrentChar is 'f' or 'F')
            { forceFloat = true; isFloat = true; GetNextChar(); }

            string lexeme = sb.ToString();
            if (!isFloat)
            {
                token.Type = TokenType.INT_LIT;
                token.Info.Ival = int.Parse(lexeme);
            }
            else
            {
                token.Type = TokenType.FLOAT_LIT;
                double d = double.Parse(lexeme,
                                        System.Globalization.CultureInfo.InvariantCulture);
                token.Info.Dval = d;
                token.Info.Fval = (float)d;      
            }
        }

        private void ReadStringLit(Token token)
        {
            GetNextChar();
            token.Type = TokenType.STRING_LIT;
            token.Name = "";

            while (CurrentChar != '"')
            {
                token.Name += (char)CurrentChar;
                GetNextChar();
            }

            if (CurrentChar == '"') GetNextChar();

            do
            {
                while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();

                if (CurrentChar == '"')
                {
                    GetNextChar();
                    while (CurrentChar != '"')
                    {
                        token.Name += (char)CurrentChar;
                        GetNextChar();
                    }
                    GetNextChar();
                }
                else break;
            } while (true);

        }

        private void ReadCharLit(Token token)
        {
            GetNextChar();
            token.Type = TokenType.CHAR_LIT;

            if (CurrentChar == '\\')
            {
                GetNextChar();
                token.Info.Ival = CurrentChar switch
                {
                    '0' => 0,
                    'b' => 8,
                    't' => 9,
                    'n' => 10,
                    'v' => 11,
                    'f' => 12,
                    'r' => 13,
                    '\'' => 39,
                    '\\' => 92,
                    _ => throw new InvalidOperationException($"Invalid character constant at line {CurrentLine}")
                };
            }
            else
            {
                token.Info.Ival = CurrentChar;
                GetNextChar();
            }

            if (CurrentChar != '\'')
                Error(CurrentLine, "Character expected");
            GetNextChar();
        }

        private void Comment()
        {
            while (StreamReader.Peek() != null)
            {
                GetNextChar();
                if (CurrentChar == '*')
                {
                    GetNextChar();
                    if (CurrentChar == '/') break;
                }
                else if (CurrentChar == '/')
                {
                    GetNextChar();
                    if (CurrentChar == '*') Comment();
                }
            }
            GetNextChar();
        }

        private void getIncludeLine()
        {
            while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();

            string word = "";
            while (char.IsLetter((char)CurrentChar)) { word += (char)CurrentChar; GetNextChar(); }

            while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();
            char open = (char)CurrentChar;
            if (open != '<' && open != '"') return;
            char close = open == '<' ? '>' : '"';
            GetNextChar();

            string header = "";
            while (CurrentChar != close && CurrentChar != -1 && CurrentChar != '\n')
            {
                header += (char)CurrentChar;
                GetNextChar();
            }
            if (CurrentChar == close) GetNextChar();

            if (header != "") Includes.Add(header);
            while (CurrentChar != '\n' && CurrentChar != -1) GetNextChar();
        }

        private void HandleDirective()
        {
            while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();

            string word = "";
            while (char.IsLetter((char)CurrentChar)) { word += (char)CurrentChar; GetNextChar(); }

            if (word == "include")
            {
                getIncludeLine();
                return;
            }

            if (word == "define")
            {
                while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();

                string name = "";
                while (char.IsLetterOrDigit((char)CurrentChar) || CurrentChar == '_')
                { name += (char)CurrentChar; GetNextChar(); }

                bool funcLike = false;
                while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();
                if ((char)CurrentChar == '(')
                {
                    funcLike = true;
                    int depth = 0;
                    do
                    {
                        if (CurrentChar == '(') depth++;
                        else if (CurrentChar == ')') depth--;
                        GetNextChar();
                    } while (depth > 0 && CurrentChar != -1);
                }

                var bodyStr = new StringBuilder();
                while (CurrentChar != '\n' && CurrentChar != -1)
                { bodyStr.Append((char)CurrentChar); GetNextChar(); }

                var bodyTokens = new List<Token>();
                using (var lex = new CLexerFromString(bodyStr.ToString()))
                {
                    var t = new Token();
                    var temp = lex.Next(ref t);
                    while (temp != TokenType.EOF && temp != TokenType.NONE)
                    {
                        bodyTokens.Add(new Token { Type = t.Type, Name = t.Name, Info = t.Info });
                        temp = lex.Next(ref t);
                    }
                }

                MacroTable.Define(new Macro(name, bodyTokens, funcLike));
                return;
            }

            while (CurrentChar != '\n' && CurrentChar != -1) GetNextChar();
        }
        private void PushMacroTokens(IEnumerable<Token> toks)
        {
            foreach (var t in toks.Reverse())
                _macroBuf.Push(t);
        }

        public virtual TokenType Next(ref Token token)
        {
            if (_macroBuf.Count > 0)
            {
                token = _macroBuf.Pop();
                return token.Type;
            }
            while (char.IsWhiteSpace((char)CurrentChar)) GetNextChar();
            token.Line = CurrentLine;
            token.Column = CurrentColumn;

            if (char.IsLetter((char)CurrentChar) || CurrentChar == '_')
                ReadName(token);
            else if (char.IsDigit((char)CurrentChar))
                ReadNumberLit(token);
            else
            {
                switch ((char)CurrentChar)
                {
                    case '"':
                        ReadStringLit(token);
                        break;
                    case '\'':
                        ReadCharLit(token);
                        break;
                    case '#':
                        GetNextChar();
                        HandleDirective();
                        return Next(ref token);
                    case '&':
                        GetNextChar();
                        if (CurrentChar == '&')
                        {
                            GetNextChar();
                            token.Type = TokenType.AND;
                        }
                        else if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.AND_ASSIGN;
                        }
                        else token.Type = TokenType.LOGICAL_AND;
                        break;
                    case '!':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.NEQ;
                        }
                        else token.Type = TokenType.NOT;
                        break;
                    case '|':
                        GetNextChar();
                        if (CurrentChar == '|')
                        {
                            GetNextChar();
                            token.Type = TokenType.OR;
                        }
                        else if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.OR_ASSIGN;
                        }
                        else token.Type = TokenType.LOGICAL_OR;
                        break;
                    case '{':
                        GetNextChar();
                        token.Type = TokenType.LBRACE;
                        break;
                    case '}':
                        GetNextChar();
                        token.Type = TokenType.RBRACE;
                        break;
                    case '(':
                        GetNextChar();
                        token.Type = TokenType.LPAR;
                        break;
                    case ')':
                        GetNextChar();
                        token.Type = TokenType.RPAR;
                        break;
                    case '*':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.MUL_ASSIGN;
                        }
                        else token.Type = TokenType.MUL;
                        break;
                    case '+':
                        GetNextChar();
                        if (CurrentChar == '+')
                        {
                            GetNextChar();
                            token.Type = TokenType.INC;
                        }
                        else if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.ADD_ASSIGN;
                        }
                        else token.Type = TokenType.PLUS;
                        break;
                    case '-':
                        GetNextChar();
                        if (CurrentChar == '-')
                        {
                            GetNextChar();
                            token.Type = TokenType.DEC;
                        }
                        else if (CurrentChar == '>')
                        {
                            GetNextChar();
                            token.Type = TokenType.PTR_OP;
                        }
                        else if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.SUB_ASSIGN;
                        }
                        else token.Type = TokenType.MINUS;
                        break;
                    case '%':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.MOD_ASSIGN;
                        }
                        else
                        {
                            token.Type = TokenType.MOD;
                        }
                        break;
                    case ',':
                        GetNextChar();
                        token.Type = TokenType.COMMA;
                        break;
                    case '/':
                        GetNextChar();
                        if (CurrentChar == '/')
                        {
                            while (CurrentChar != '\n') GetNextChar();
                            return Next(ref token);
                        }
                        else if (CurrentChar == '*')
                        {
                            Comment();
                            return Next(ref token);
                        }
                        else if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.DIV_ASSIGN;
                        }
                        else token.Type = TokenType.DIV;
                        break;
                    case ';':
                        GetNextChar();
                        token.Type = TokenType.SEMICOLON;
                        break;
                    case '=':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.EQ;
                        }
                        else
                        { //TODO
                            token.Type = TokenType.ASSIGN;
                        }
                        break;
                    case '[':
                        GetNextChar();
                        token.Type = TokenType.LBRACK;
                        break;
                    case ']':
                        GetNextChar();
                        token.Type = TokenType.RBRACK;
                        break;
                    case '^':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.XOR_ASSIGN;
                        }
                        else token.Type = TokenType.XOR;
                        break;
                    case '~':
                        GetNextChar();
                        token.Type = TokenType.TILDA;
                        break;
                    case '?':
                        GetNextChar();
                        token.Type = TokenType.QUESTION;
                        break;
                    case ':':
                        GetNextChar();
                        token.Type = TokenType.COLON;
                        break;
                    case '<':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.LEQ;
                        }
                        else if (CurrentChar == '<')
                        {
                            GetNextChar();
                            if (CurrentChar == '=')
                            {
                                GetNextChar();
                                token.Type = TokenType.LSHIFT_ASSIGN;
                            }
                            else token.Type = TokenType.LSHIFT;
                        }
                        else token.Type = TokenType.LS;
                        break;
                    case '>':
                        GetNextChar();
                        if (CurrentChar == '=')
                        {
                            GetNextChar();
                            token.Type = TokenType.GEQ;
                        }
                        else if (CurrentChar == '>')
                        {
                            GetNextChar();
                            if (CurrentChar == '=')
                            {
                                GetNextChar();
                                token.Type = TokenType.RSHIFT_ASSIGN;
                            }
                            else token.Type = TokenType.RSHIFT;
                        }
                        else token.Type = TokenType.GT;
                        break;
                    case '.':
                        GetNextChar();
                        if (CurrentChar == '.')
                        {
                            GetNextChar();
                            if (CurrentChar == '.')
                            {
                                GetNextChar();
                                token.Type = TokenType.ELLIPSIS;
                            }
                            else token.Type = TokenType.NONE;
                        }
                        else token.Type = TokenType.EOF;
                        break;
                    case (char)TokenType.EOF:
                        token.Type = TokenType.EOF;
                        break;
                    default:
                        GetNextChar();
                        token.Type = TokenType.NONE;
                        break;

                }
            }

            if (token.Type == TokenType.IDENT && MacroTable.TryGet(token.Name, out var mac) &&
                mac.Length <= MacroTable.INLINE_LIMIT && !mac.HasIdents)
            {
                PushMacroTokens(mac.Body.Skip(1));
                var first = mac.Body[0];
                token.Type = first.Type;
                token.Name = first.Name;
                token.Info = first.Info;
            }
            return token.Type;
        }
        public void Dispose()
        {
            sr?.Dispose();
        }
    }

}
