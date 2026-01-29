using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Visualizer
{
    public class CParser : Parser
    {
        private bool inTypeDef;
        SequenceTypeNode allSeq;
        private bool isTypeSpecifiar(TokenType type, uint n)
        {
            if (type == TokenType.CHAR || type == TokenType.DOUBLE || type == TokenType.ENUM ||
                type == TokenType.FLOAT || type == TokenType.INT || type == TokenType.LONG ||
                type == TokenType.SHORT || type == TokenType.SIGNED || type == TokenType.STRUCT ||
                type == TokenType.UNION || type == TokenType.UNSIGNED || type == TokenType.VOID)
            { return true; }
            else if (type == TokenType.IDENT)
            {
                SOK obj = symbols.Find(GetLATokInfoSval(n));
                if (obj != symbols.NoObj && obj.Kind == ObjectKind.OK_TYPE) return true;
            }
            return false;
        }

        private bool isTypeQuilifer(TokenType type)
        {
            return type == TokenType.CONST || type == TokenType.VOLATILE || type == TokenType.RESTRICT ||
                   type == TokenType.ATOMIC || type == TokenType.__NULLABLE;
        }

        private bool isStorageClassSpecifier(TokenType type)
        {
            return type == TokenType.TYPEDEF || type == TokenType.EXTERN || type == TokenType.STATIC ||
                   type == TokenType.AUTO || type == TokenType.REGISTER;
        }

        protected override void initName()
        {
            name.Add(TokenType.NONE, "none");
            name.Add(TokenType.IDENT, "identifier");
            name.Add(TokenType.INT_LIT, "integer constant");
            name.Add(TokenType.CHAR_LIT, "character constant");
            name.Add(TokenType.FLOAT_LIT, "float constant");
            name.Add(TokenType.STRING_LIT, "string constant");
            name.Add(TokenType.SIZEOF, "sizeof");
            name.Add(TokenType.PLUS, "+");
            name.Add(TokenType.MINUS, "-");
            name.Add(TokenType.MUL, "*");
            name.Add(TokenType.DIV, "/");
            name.Add(TokenType.MOD, "%");
            name.Add(TokenType.SEMICOLON, ";");
            name.Add(TokenType.COLON, ":");
            name.Add(TokenType.COMMA, ",");
            name.Add(TokenType.PERIOD, ".");
            name.Add(TokenType.LPAR, "(");
            name.Add(TokenType.RPAR, ")");
            name.Add(TokenType.LBRACK, "[");
            name.Add(TokenType.RBRACK, "]");
            name.Add(TokenType.LBRACE, "{");
            name.Add(TokenType.RBRACE, "}");
            name.Add(TokenType.TILDA, "~");
            name.Add(TokenType.QUESTION, "?");
            name.Add(TokenType.LOGICAL_AND, "&");
            name.Add(TokenType.AND, "&&");
            name.Add(TokenType.LOGICAL_OR, "|");
            name.Add(TokenType.OR, "||");
            name.Add(TokenType.XOR, "^");
            name.Add(TokenType.PTR_OP, "->");
            name.Add(TokenType.INC, "++");
            name.Add(TokenType.DEC, "--");
            name.Add(TokenType.LSHIFT, "<<");
            name.Add(TokenType.RSHIFT, ">>");
            name.Add(TokenType.LS, "<");
            name.Add(TokenType.GT, ">");
            name.Add(TokenType.LEQ, "<=");
            name.Add(TokenType.GEQ, ">=");
            name.Add(TokenType.EQ, "==");
            name.Add(TokenType.NEQ, "!=");
            name.Add(TokenType.NOT, "!");
            name.Add(TokenType.ASSIGN, "=");
            name.Add(TokenType.MUL_ASSIGN, "*=");
            name.Add(TokenType.DIV_ASSIGN, "/=");
            name.Add(TokenType.MOD_ASSIGN, "%=");
            name.Add(TokenType.ADD_ASSIGN, "+=");
            name.Add(TokenType.SUB_ASSIGN, "-=");
            name.Add(TokenType.LSHIFT_ASSIGN, "<<");
            name.Add(TokenType.RSHIFT_ASSIGN, ">>");
            name.Add(TokenType.AND_ASSIGN, "&=");
            name.Add(TokenType.XOR_ASSIGN, "^=");
            name.Add(TokenType.OR_ASSIGN, "|=");
            name.Add(TokenType.TYPE_NAME, "type name");
            name.Add(TokenType.ELLIPSIS, "...");
            name.Add(TokenType.AUTO, "auto");
            name.Add(TokenType.CHAR, "char");
            name.Add(TokenType.CONST, "const");
            name.Add(TokenType.DOUBLE, "double");
            name.Add(TokenType.ELSE, "else");
            name.Add(TokenType.ENUM, "enum");
            name.Add(TokenType.EXTERN, "extern");
            name.Add(TokenType.FLOAT, "float");
            name.Add(TokenType.INT, "int");
            name.Add(TokenType.LONG, "long");
            name.Add(TokenType.REGISTER, "register");
            name.Add(TokenType.SHORT, "short");
            name.Add(TokenType.SIGNED, "signed");
            name.Add(TokenType.STATIC, "static");
            name.Add(TokenType.STRUCT, "struct");
            name.Add(TokenType.TYPEDEF, "typedef");
            name.Add(TokenType.UNION, "union");
            name.Add(TokenType.UNSIGNED, "unsigned");
            name.Add(TokenType.VOID, "void");
            name.Add(TokenType.VOLATILE, "volatile");
            name.Add(TokenType.ASM, "asm");
            name.Add(TokenType.BREAK, "break");
            name.Add(TokenType.CASE, "case");
            name.Add(TokenType.CONTINUE, "continue");
            name.Add(TokenType.DEFAULT, "default");
            name.Add(TokenType.DO, "do");
            name.Add(TokenType.FOR, "for");
            name.Add(TokenType.GOTO, "goto");
            name.Add(TokenType.IF, "if");
            name.Add(TokenType.RETURN, "return");
            name.Add(TokenType.SWITCH, "switch");
            name.Add(TokenType.WHILE, "while");
            name.Add(TokenType.EOF, "end of file");


        }

        private ASTNode GenericSelection()
        {
            check(TokenType.GENERIC);
            check(TokenType.LPAR);
            AssignmentExpression();
            check(TokenType.COMMA);
            GenericAssocList();
            check(TokenType.RPAR);
            return NullNode.Instance;
        }

        private ASTNode GenericAssocList()
        {
            SequenceTypeNode list = new SequenceTypeNode(GenericAssociation());
            while (sym == TokenType.COMMA)
            {
                GetToken();
                list.Add(GenericAssociation());
            }
            return list;
        }

        private ASTNode GenericAssociation()
        {
            ASTNode specOrDefault = NullNode.Instance;
            ASTNode assocExpr = NullNode.Instance;
            if (isTypeSpecifiar(sym, 1))
            {
                specOrDefault = TypeName();
                check(TokenType.COLON);
                assocExpr = AssignmentExpression();
            }
            else if (sym == TokenType.DEFAULT)
            {
                GetToken();
                specOrDefault = new IdentNode("defaul", tok.Line);
                check(TokenType.COLON);
                assocExpr = AssignmentExpression();
            }
            else ParsingError("expected type specifier or default");

            SequenceTypeNode association = new SequenceTypeNode();
            association.Add(specOrDefault);
            association.Add(assocExpr);
            return association;
        }

        private ASTNode TypeName()
        {
            ASTNode specQualList = SpecifierQualifierList();
            return specQualList;
        }

        private ASTNode PrimaryExpression()
        {
            ASTNode node = null;

            switch (sym)
            {
                case TokenType.IDENT:
                    GetToken();
                    if(!MacroTable.Contains(tok.Name))
                        RequireDeclared(tok.Name, tok.Line);
                    node = new IdentNode(tok.Name, tok.Line);
                    break;
                case TokenType.INT_LIT:
                    GetToken();
                    node = new IntConstNode(tok.Info.Ival);
                    break;
                case TokenType.CHAR_LIT:
                    GetToken();
                    node = new CharConstNode(tok.Info.Ival);
                    break;
                case TokenType.STRING_LIT:
                    GetToken();
                    node = new StringConstNode(tok.Name);
                    break;
                case TokenType.FLOAT_LIT:
                    GetToken();
                    node = new NormalConstNode(tok.Info.Fval);
                    break;
                case TokenType.LPAR:
                    GetToken();
                    if (sym == TokenType.MUL)
                    {
                        GetToken();
                        check(TokenType.IDENT);
                        node = new IdentNode(tok.Name, tok.Line);
                    }
                    else node = Expression();
                    break;
                case TokenType.GENERIC:
                    node = GenericSelection();
                    break;
            }
            return node;
        }

        private ASTNode ParsePostfixExperssion(ASTNode expr)
        {
            if (sym == TokenType.PERIOD)
            {
                ASTNode temp;

                GetToken();
                check(TokenType.IDENT);
                temp = new IdentNode(tok.Name, tok.Line);
                return new StructRefNode(expr, ParsePostfixExperssion(temp));
            }
            else if (sym == TokenType.PTR_OP)
            {
                ASTNode temp;
                GetToken();
                check(TokenType.IDENT);
                temp = new IdentNode(tok.Name, tok.Line);
                return new IndirectRefNode(expr, NullNode.Instance, ParsePostfixExperssion(temp));
            }

            return expr;
        }
        private ASTNode PostfixExpression(ASTNode typeName)
        {
            ASTNode index = NullNode.Instance;
            ASTNode expr = PrimaryExpression();

            while (true)
            {
                switch (sym)
                {
                    case TokenType.LBRACK:
                        GetToken();
                        index = Expression();
                        expr = new ArrayRefNode(expr, typeName, index);
                        check(TokenType.RBRACK);
                        break;
                    case TokenType.LPAR:
                        ASTNode args;
                        GetToken();

                        args = NullNode.Instance;
                        if (sym != TokenType.RPAR) args = ArgumentExpressionList();
                        check(TokenType.RPAR);
                        expr = new CallExprNode(expr, args);
                        break;
                    case TokenType.PERIOD:
                        {
                            ASTNode temp;

                            GetToken();
                            check(TokenType.IDENT);
                            temp = new IdentNode(tok.Name, tok.Line);
                            expr = new StructRefNode(expr, ParsePostfixExperssion(temp));
                            break;
                        }
                    case TokenType.PTR_OP:
                        {
                            ASTNode temp;

                            GetToken();
                            check(TokenType.IDENT);
                            temp = new IdentNode(tok.Name, tok.Line);
                            expr = new IndirectRefNode(expr, NullNode.Instance, ParsePostfixExperssion(temp));
                            break;
                        }
                    case TokenType.INC:
                        GetToken();
                        expr = new PostIncExprNode(expr);
                        break;
                    case TokenType.DEC:
                        GetToken();
                        expr = new PostDecExprNode(expr);
                        break;
                    default:
                        return expr;

                }
            }
        }

        private ASTNode ArgumentExpressionList()
        {
            SequenceTypeNode args = new SequenceTypeNode(AssignmentExpression());
            while (sym == TokenType.COMMA)
            {
                GetToken();
                args.Add(AssignmentExpression());
            }

            return args;
        }

        private bool isFirstOfPostfixExpression(TokenType type)
        {
            return (type >= TokenType.IDENT && type <= TokenType.STRING_LIT)
                    || type == TokenType.LPAR || type == TokenType.GENERIC;
        }

        private ASTNode UnaryExpression(ASTNode typeName)
        {
            ASTNode res = NullNode.Instance;
            ASTNode expr = NullNode.Instance;

            if (isFirstOfPostfixExpression(sym)) res = PostfixExpression(typeName);
            else if (sym == TokenType.INC)
            {
                GetToken();
                expr = UnaryExpression(typeName);
                res = new PreIncExprNode(expr);
            }
            else if (sym == TokenType.DEC)
            {
                GetToken();
                expr = UnaryExpression(typeName);
                res = new PreDecExprNode(expr);
            }
            else if (sym == TokenType.MINUS)
            {
                GetToken();                             
                expr = UnaryExpression(typeName);        

                if (expr is IntConstNode iconst)
                    res = new IntConstNode(-iconst.val);
                else if (expr is NormalConstNode fconst)
                    res = new NormalConstNode(-fconst.val);
                else
                    res = new NegateExprNode(expr);
            }
            else if (sym == TokenType.LOGICAL_AND || sym == TokenType.MUL || sym == TokenType.TILDA || sym == TokenType.NOT)
            {
                ASTNode type = ast.intTypeNode;
                ASTNodeType kind = UnaryOperator();

                if (kind == ASTNodeType.ASTNT_ADDR_EXPR || kind == ASTNodeType.ASTNT_INDIRECT_REF)
                {
                    type = new PointerTypeNode(ast.intTypeNode);
                    if (kind == ASTNodeType.ASTNT_ADDR_EXPR)
                        res = new AddrExprNode(type, CastExpression());
                    else
                        res = new IndirectRefNode(CastExpression(), type, NullNode.Instance);
                }
                else if (kind == ASTNodeType.ASTNT_BIT_NOT_EXPR)
                    res = new BitNotNodeExprNode(type, CastExpression());
                else if (kind == ASTNodeType.ASTNT_LOG_NOT_EXPR)
                    res = new LogNotExrNode(type, CastExpression());
            }
            else if (sym == TokenType.SIZEOF)
            {
                GetToken();
                if (sym == TokenType.LPAR)
                {
                    GetToken();
                    expr = TypeSpecifier();
                    if (sym == TokenType.MUL)
                    {
                        GetToken();
                        expr = new PointerTypeNode(expr);
                    }
                    check(TokenType.RPAR);
                }
                else expr = UnaryExpression(NullNode.Instance);
                res = new SizeOfExprNode(expr);
            }
            else if (sym == TokenType.ALIGNOF)
            {
                GetToken();
                check(TokenType.LPAR);
                expr = TypeName();
                check(TokenType.RPAR);
                res = new AlignOfExprNode(expr);
            }

            return res;
        }

        private ASTNodeType UnaryOperator()
        {
            switch (sym)
            {
                case TokenType.LOGICAL_AND:
                    GetToken();
                    return ASTNodeType.ASTNT_ADDR_EXPR;
                case TokenType.MUL:
                    GetToken();
                    return ASTNodeType.ASTNT_INDIRECT_REF;
                case TokenType.PLUS:
                    GetToken();
                    return ASTNodeType.ASTNT_NONE;
                case TokenType.MINUS:
                    GetToken();
                    return ASTNodeType.ASTNT_NONE;
                case TokenType.TILDA:
                    GetToken();
                    return ASTNodeType.ASTNT_BIT_NOT_EXPR;
                case TokenType.NOT:
                    GetToken();
                    return ASTNodeType.ASTNT_LOG_NOT_EXPR;
                default:
                    return ASTNodeType.ASTNT_NONE;
            }
        }
        private ASTNode CastExpression()
        {
            ASTNode typeName = NullNode.Instance;
            ASTNode expr = NullNode.Instance;
            bool isCast = false;
            bool isPtrType = false;

            if (sym == TokenType.LPAR && isTypeSpecifiar(GetLATok(2), 2))
            {
                GetToken();
                isCast = true;
                typeName = TypeSpecifier();

                if (sym == TokenType.MUL)
                {
                    GetToken();
                    isPtrType = true;
                }
                check(TokenType.RPAR);
            }
            expr = UnaryExpression(typeName);
            if (isCast)
            {
                if (isPtrType)
                    typeName = new PointerTypeNode(typeName);

                return new CastExprNode(typeName, expr);
            }
            else return expr;
        }

        private ASTNode MulExpression()
        {
            ASTNode res = CastExpression();
            while (true)
            {
                if (sym == TokenType.MUL)
                {
                    GetToken();
                    res = new MultExprNode(res, ast.intTypeNode, CastExpression());
                }
                else if (sym == TokenType.DIV)
                {
                    GetToken();
                    res = new TruncDivExprNode(res, ast.intTypeNode, CastExpression());
                }
                else if (sym == TokenType.MOD)
                {
                    GetToken();
                    res = new TruncModExprNode(res, ast.intTypeNode, CastExpression());
                }
                else break;
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode AddExpression()
        {
            ASTNode res = MulExpression();
            while (true)
            {
                if (sym == TokenType.PLUS)
                {
                    GetToken();
                    res = new PlusExprNode(res, ast.intTypeNode, MulExpression());
                }
                else if (sym == TokenType.MINUS)
                {
                    GetToken();
                    res = new MinusExprNode(res, ast.intTypeNode, MulExpression());
                }

                else break;
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode ShiftExpression()
        {
            ASTNode res = AddExpression();
            while (true)
            {
                if (sym == TokenType.LSHIFT)
                {
                    GetToken();
                    res = new LShiftExprNode(res, ast.intTypeNode, AddExpression());
                }
                else if (sym == TokenType.RSHIFT)
                {
                    GetToken();
                    res = new RShiftExprNode(res, ast.intTypeNode, AddExpression());
                }

                else break;
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode RelationalExpression()
        {
            ASTNode res = ShiftExpression();
            while (true)
            {
                if (sym == TokenType.LS)
                {
                    GetToken();
                    res = new LtExprNode(res, ast.intTypeNode, ShiftExpression());
                }
                else if (sym == TokenType.GT)
                {
                    GetToken();
                    res = new GtExprNode(res, ast.intTypeNode, ShiftExpression());
                }
                else if (sym == TokenType.LEQ)
                {
                    GetToken();
                    res = new LeExprNode(res, ast.intTypeNode, ShiftExpression());
                }
                else if (sym == TokenType.GEQ)
                {
                    GetToken();
                    res = new GeExprNode(res, ast.intTypeNode, ShiftExpression());
                }
                else break;
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode EqualExpression()
        {
            ASTNode res = RelationalExpression();
            while (true)
            {
                if (sym == TokenType.EQ)
                {
                    GetToken();
                    res = new EqExprNode(res, ast.intTypeNode, RelationalExpression());
                }
                else if (sym == TokenType.NEQ)
                {
                    GetToken();
                    res = new NeExprNode(res, ast.intTypeNode, RelationalExpression());
                }
                else break;
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode AndExpression()
        {
            ASTNode res = EqualExpression();
            while (sym == TokenType.AND)
            {
                GetToken();
                res = new BitAndExprNode(res, ast.intTypeNode, EqualExpression());
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode XorExpression()
        {
            ASTNode res = AndExpression();
            while (sym == TokenType.XOR)
            {
                GetToken();
                res = new BitXorExprNode(res, ast.intTypeNode, AndExpression());
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode OrExpression()
        {
            ASTNode res = XorExpression();
            while (sym == TokenType.OR)
            {
                GetToken();
                res = new BitOrExprNode(res, ast.intTypeNode, XorExpression());
            }
            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode LogAndExpression()
        {
            ASTNode res = OrExpression();
            while (sym == TokenType.LOGICAL_AND)
            {
                GetToken();
                res = new LogAndExprNode(res, ast.intTypeNode, OrExpression());
            }
            res.LineNumber = tok.Line;
            return res;
        }
        private ASTNode LogOrExpression()
        {
            ASTNode res = LogAndExpression();
            while (sym == TokenType.LOGICAL_OR)
            {
                GetToken();
                res = new LogOrExprNode(res, ast.intTypeNode, LogAndExpression());
            }
            res.LineNumber = tok.Line;
            return res;
        }
        private ASTNode CondExression()
        {
            ASTNode expr = NullNode.Instance;
            ASTNode res = LogOrExpression();
            if (sym == TokenType.QUESTION)
            {
                GetToken();
                expr = Expression();
                check(TokenType.COLON);
                res = new CondExprNode(res, expr, CondExression());
            }

            res.LineNumber = tok.Line;
            return res;
        }

        private ASTNode AssignmentExpression()
        {
            ASTNode res = CondExression();

            if (sym >= TokenType.ASSIGN && sym <= TokenType.OR_ASSIGN)
            {
                TokenType op = sym;

                GetToken();
                if (op >= TokenType.MUL_ASSIGN && op <= TokenType.OR_ASSIGN)
                {
                    ASTNode lhs = res, rhs = AssignmentExpression(), expr = NullNode.Instance;
                    switch (op)
                    {
                        case TokenType.MUL_ASSIGN:
                            expr = new MultExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.DIV_ASSIGN:
                            expr = new TruncDivExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.MOD_ASSIGN:
                            expr = new TruncModExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.ADD_ASSIGN:
                            expr = new PlusExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.SUB_ASSIGN:
                            expr = new MinusExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.LSHIFT_ASSIGN:
                            expr = new LShiftExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.RSHIFT_ASSIGN:
                            expr = new RShiftExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.AND_ASSIGN:
                            expr = new BitAndExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.XOR_ASSIGN:
                            expr = new BitXorExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                        case TokenType.OR_ASSIGN:
                            expr = new BitOrExprNode(lhs, ast.intTypeNode, rhs);
                            break;
                    }
                }
                else res = new AssignExprNode(res, ast.intTypeNode, AssignmentExpression());
            }
            res.LineNumber = tok.Line;

            return res;
        }
        private ASTNode Expression()
        {
            SequenceTypeNode seq = new SequenceTypeNode(AssignmentExpression());

            while (sym == TokenType.COMMA)
            {
                GetToken();
                seq.Add(AssignmentExpression());
            }

            return seq.size() == 1 ? seq.elements[0] : seq;
        }

        private ASTNode ConstantExpression() => CondExression();

        private ASTNode Declaration(ASTNode decl)
        {
            ASTNode initlist = NullNode.Instance;
            if (sym != TokenType.SEMICOLON)
                initlist = InitDeclList(decl);

            check(TokenType.SEMICOLON);
            if (initlist != NullNode.Instance) return initlist;
            else return decl;
        }

        private ASTNode DeclrSpecifiers()
        {
            ASTNode declSpec = NullNode.Instance;
            TokenType flags = TokenType.NONE;
            while (true)
            {
                if (isStorageClassSpecifier(sym)) StorageClassSpecifier(ref flags);
                else if (isTypeQuilifer(sym)) TypeQuilifer(ref flags);
                else if (isTypeSpecifiar(sym, 1))
                {
                    declSpec = TypeSpecifier();
                    declSpec.Flags = flags;
                }
                else if (sym == TokenType.MUL)
                {
                    GetToken();
                    declSpec = new PointerTypeNode(declSpec);
                    if (isTypeQuilifer(sym)) TypeQuilifer(ref flags);
                }
                else if (sym == TokenType.__ASM || sym == TokenType.__ATTRIBUTE__)
                    GccDeclaratorExtension();
                else if (sym == TokenType.INLINE || sym == TokenType.NORETURN) GetToken();
                else break;
            }
            return declSpec;
        }

        private SequenceTypeNode InitDeclList(ASTNode type)
        {
            SequenceTypeNode initList = new SequenceTypeNode(InitDeclarator(type));

            while (sym == TokenType.COMMA)
            {
                GetToken();
                initList.Add(InitDeclarator(type));
            }
            //allSeq.Add(initList);

            return initList;
        }

        private ASTNode InitDeclarator(ASTNode type)
        {
            ASTNode init = NullNode.Instance;

            ASTNode name = Declarator(ref type);

            // 2. Теперь type  корректный (array, pointer, function…)
            VarDeclNode declr = new VarDeclNode(type, name);

            if (sym == TokenType.ASSIGN)
            {
                GetToken();
                init = Initializer();
                declr.init = init;
            }
            return declr;
        }

        private void StorageClassSpecifier(ref TokenType flags)
        {
            if (sym == TokenType.TYPEDEF)
            {
                GetToken();
                inTypeDef = true;
            }
            else if (sym == TokenType.EXTERN)
            {
                GetToken();
                flags |= TokenType.INT_LIT;
            }
            else if (sym == TokenType.STATIC)
            {
                GetToken();
                flags |= TokenType.FLOAT_LIT;
            }
            else if (sym == TokenType.THREAD_LOCAL)
                GetToken();
            else if (sym == TokenType.AUTO)
            {
                GetToken();
                flags |= TokenType.MINUS;
            }
            else if (sym == TokenType.REGISTER)
            {
                GetToken();
                flags |= TokenType.DIV;
            }
        }

        private static ASTNode TypeToNode(AST ast, TKType type, string name)
        {
            return type.Kind switch
            {
                TypeKind.TK_NONE => NullNode.Instance,
                TypeKind.TK_CHAR => ast.charTypeNode,
                TypeKind.TK_SHORT => ast.shortTypeNode,
                TypeKind.TK_INT => ast.intTypeNode,
                TypeKind.TK_UNSIGNED => ast.unsignedTypeNode,
                TypeKind.TK_LONG => ast.longTypeNode,
                TypeKind.TK_FLOAT => ast.floatTypeNode,
                TypeKind.TK_DOUBLE => ast.doubleTypeNode,
                TypeKind.TK_VOID => ast.voidTypeNode,
                TypeKind.TK_ARRAY => new ArrayTypeNode(TypeToNode(ast, type.ElType ?? type.BaseType, name), 
                         type.Size > 0 ? new IntConstNode((int)type.Size) : NullNode.Instance),
                TypeKind.TK_STRUCT => new StructTypeNode(new IdentNode(name), NullNode.Instance),
                TypeKind.TK_UNION => NullNode.Instance,
                TypeKind.TK_BOOL => NullNode.Instance,
                TypeKind.TK_REAL => NullNode.Instance,
                TypeKind.TK_POINTER => new PointerTypeNode(TypeToNode(ast, type.BaseType, name)),
                TypeKind.TK_ENUM => NullNode.Instance,
                TypeKind.TK_FUNCTION => NullNode.Instance,
                _ => NullNode.Instance,
            };
        }
        private ASTNode TypeSpecifier()
        {
            if (sym == TokenType.VOID)
            {
                GetToken();
                return ast.voidTypeNode;
            }
            else if (sym == TokenType.CHAR)
            {
                GetToken();
                return ast.charTypeNode;
            }
            else if (sym == TokenType.SHORT)
            {
                GetToken();
                return ast.shortTypeNode;
            }
            else if (sym == TokenType.INT)
            {
                GetToken();
                return ast.intTypeNode;
            }
            else if (sym == TokenType.LONG)
            {
                GetToken();
                return ast.longTypeNode;
            }
            else if (sym == TokenType.FLOAT)
            {
                GetToken();
                return ast.floatTypeNode;
            }
            else if (sym == TokenType.DOUBLE) {
                GetToken();
                return ast.doubleTypeNode;
            }
            else if (sym == TokenType.SIGNED) GetToken();
            else if (sym == TokenType.UNSIGNED)
            {
                GetToken();
                return ast.unsignedTypeNode;
            }
            else if (sym == TokenType.STRUCT || sym == TokenType.UNION) return StructSpecifier();
            else if (sym == TokenType.ENUM) return EnumSpecifier();
            else if (sym == TokenType.IDENT)
            {
                SOK obj;

                GetToken();
                obj = symbols.Find(tok.Name);
                if (obj != symbols.NoObj)
                    return TypeToNode(ast, obj.Type, obj.Name);
                string error = $"unknown type '%s' {obj.Name}";
                ParsingError(error);
            }
            return NullNode.Instance;
        }

        private ASTNode StructSpecifier()
        {
            ASTNode typeName = NullNode.Instance;
            ASTNode typeBody = NullNode.Instance;
            SOK obj = null;
            ASTNodeType type;

            if (sym == TokenType.STRUCT)
            {
                GetToken();
                type = ASTNodeType.ASTNT_STRUCT_TYPE;
            }
            else
            {
                GetToken();
                type = ASTNodeType.ASTNT_UNION_TYPE;
            }

            if (sym == TokenType.IDENT)
            {
                TKType record;
                GetToken();

                typeName = new IdentNode(tok.Name, tok.Line);
                if (type == ASTNodeType.ASTNT_STRUCT_TYPE)
                    record = symbols.AllocType(TypeKind.TK_STRUCT);
                else
                    record = symbols.AllocType(TypeKind.TK_UNION);

                obj = symbols.Find(tok.Name);

                if (obj == symbols.NoObj)
                    obj = symbols.Insert(tok.Name, ObjectKind.OK_TYPE, record);
            }

            if (sym == TokenType.LBRACE)
            {
                GetToken();
                symbols.OpenScope();
                typeBody = StructDeclList();

                if (obj != null)
                {
                    if (type == ASTNodeType.ASTNT_UNION_TYPE)
                    {
                        SOK temp = symbols.GetCurrentScope().Locals;
                        int maxFieldTypeSize = 0;
                        while (temp != null)
                        {
                            maxFieldTypeSize = Math.Max((int)temp.Type.Size, maxFieldTypeSize);
                            temp = temp.Next;
                        }
                        symbols.GetCurrentScope().Size = maxFieldTypeSize;
                    }
                    obj.Type.Fields = symbols.GetCurrentScope().Locals;
                    obj.Type.Size = symbols.GetCurrentScope().Size;
                }
                symbols.CloseScope();
                check(TokenType.RBRACE);
            }
            if (type == ASTNodeType.ASTNT_STRUCT_TYPE) return new StructTypeNode(typeName, typeBody);
            else return new UnionTypeNode(typeName, typeBody);
        }

        private SequenceTypeNode StructDeclList()
        {
            SequenceTypeNode declList = new SequenceTypeNode(StructDecl());
            while (sym != TokenType.RBRACE)
                declList.Add(StructDecl());
            //allSeq.Add(declList);

            return declList;
        }

        private ASTNode StructDecl()
        {
            ASTNode typeSec = SpecifierQualifierList();
            SequenceTypeNode declaratorList = StructDeclaratorList(typeSec);

            check(TokenType.SEMICOLON);
            return declaratorList;
        }

        private ASTNode SpecifierQualifierList()
        {
            SequenceTypeNode list = new SequenceTypeNode(TypeSpecifier());

            while (isTypeSpecifiar(sym, 1))
                list.Add(TypeSpecifier());
            //allSeq.Add(list);

            return list;
        }

        private SequenceTypeNode StructDeclaratorList(ASTNode typeSec)
        {
            SequenceTypeNode decllist = new SequenceTypeNode(StructDeclarator(typeSec));

            while (sym == TokenType.COMMA)
            {
                GetToken();
                decllist.Add(StructDeclarator(typeSec));
            }
            return decllist;
        }

        private ASTNode StructDeclarator(ASTNode typeSec)
        {
            ASTNode name = Declarator(ref typeSec);
            return new FieldDecNode(typeSec, name);
        }

        private ASTNode EnumSpecifier()
        {
            ASTNode name = NullNode.Instance;
            ASTNode body = NullNode.Instance;

            GetToken();

            if (sym == TokenType.IDENT)
            {
                TKType enumType;

                GetToken();
                name = new IdentNode(tok.Name);
                enumType = symbols.AllocType(TypeKind.TK_ENUM);
                symbols.Insert(tok.Name, ObjectKind.OK_TYPE, enumType);
            }

            if (sym == TokenType.LBRACE)
            {
                GetToken();
                body = EnumeratorList();
                check(TokenType.RBRACE);
            }

            return new EnumeralTypeNode(name, body);
        }

        private SequenceTypeNode EnumeratorList()
        {
            SequenceTypeNode enums;
            SOK obj;
            int i = 0;

            check(TokenType.IDENT);
            enums = new SequenceTypeNode(new IdentNode(tok.Name));

            obj = symbols.Insert(tok.Name, ObjectKind.OK_CON, symbols.IntType);
            obj.IVal = i;

            if (sym == TokenType.ASSIGN)
            {
                GetToken();
                ConstantExpression();
            }

            while (sym == TokenType.COMMA)
            {
                GetToken();
                check(TokenType.IDENT);
                enums.Add(new IdentNode(tok.Name));

                i++;
                obj = symbols.Insert(tok.Name, ObjectKind.OK_CON, symbols.IntType);
                obj.IVal = i;

                if (sym == TokenType.ASSIGN)
                {
                    GetToken();
                    ConstantExpression();
                }
            }
            //allSeq.Add(enums);
            return enums;
        }
        private void TypeQuilifer(ref TokenType flags)
        {
            if (sym == TokenType.CONST)
            {
                GetToken();
                flags |= TokenType.LBRACK;
            }
            else if (sym == TokenType.VOLATILE)
            {
                GetToken();
                flags |= TokenType.NEQ;
            }
            else if (sym == TokenType.RESTRICT || sym == TokenType.ATOMIC || sym == TokenType.__NULLABLE)
                GetToken();
            else
                ParsingError("expected type qualifier");
        }


        private void FunctionSpecifier()
        {
            if (sym == TokenType.INLINE || sym == TokenType.NORETURN) GetToken();
            else ParsingError("expected function specifier");
        }

        private ASTNode AlignmentSpecifier()
        {
            check(TokenType.ALIGNAS);
            check(TokenType.LPAR);
            if (isTypeSpecifiar(sym, 1)) TypeName();
            else ConstantExpression();
            check(TokenType.RPAR);

            return NullNode.Instance;
        }

        private ASTNode Declarator(ref ASTNode typeSpec)
        {
            while (sym == TokenType.MUL)
            {
                TokenType flags;

                GetToken();

                flags = typeSpec.Flags;
                typeSpec = new PointerTypeNode(typeSpec);
                typeSpec.Flags = flags;
            }

            ASTNode decl = DirectDeclarator(ref typeSpec);

            while (sym == TokenType.__ASM || sym == TokenType.__ATTRIBUTE__)
                GccDeclaratorExtension();

            return decl;
        }

        private ASTNode DirectDeclarator(ref ASTNode typeSpec)
        {
            ASTNode declr = NullNode.Instance;
            ASTNode expr = NullNode.Instance;
            FunctionTypeNode funcType = null;
            bool isPtr = false;

            if (sym == TokenType.IDENT)
            {
                GetToken();
                declr = new IdentNode(tok.Name, tok.Line);
            }
            else
            {
                check(TokenType.LPAR);
                if (sym == TokenType.MUL)
                {
                    GetToken();

                    TokenType flags = TokenType.NONE;

                    if (isTypeQuilifer(sym)) TypeQuilifer(ref flags);

                    isPtr = true;
                    funcType = new FunctionTypeNode(typeSpec, NullNode.Instance);
                    typeSpec = new PointerTypeNode(funcType);
                }
                ASTNode temp = NullNode.Instance;
                declr = Declarator(ref temp);
                check(TokenType.RPAR);
            }

            while (true)
            {
                if (sym == TokenType.LBRACK)
                {
                    GetToken();

                    if (sym != TokenType.RBRACK)
                    {
                        expr = Expression();
                        typeSpec = new ArrayTypeNode(typeSpec, expr);
                    }
                    else typeSpec = new ArrayTypeNode(typeSpec, NullNode.Instance);
                    check(TokenType.RBRACK);
                    //GetToken();
                }
                else break;
            }
            if (sym == TokenType.LPAR && isPtr)
            {
                GetToken();
                if (sym == TokenType.RPAR)
                {
                    ASTNode funcPrms = ParameterTypeList();
                    funcType.prms = funcPrms;
                }
                check(TokenType.RPAR);
            }

            return declr;
        }

        private ASTNode GccDeclaratorExtension()
        {
            if (sym == TokenType.__ASM)
            {
                GetToken();
                check(TokenType.LPAR);
                check(TokenType.STRING_LIT);
                while (sym == TokenType.STRING_LIT) GetToken();
                check(TokenType.RPAR);
            }
            else if (sym == TokenType.__ATTRIBUTE__) GccAttributeSpecifier();
            else ParsingError("expected '__asm' or '__attribute__'");
            return NullNode.Instance;
        }

        private ASTNode GccAttributeSpecifier()
        {
            check(TokenType.__ATTRIBUTE__);
            check(TokenType.LPAR);
            check(TokenType.LPAR);
            GccAttributeList();
            check(TokenType.RPAR);
            check(TokenType.RPAR);

            return NullNode.Instance;
        }

        private ASTNode GccAttributeList()
        {
            if (sym == TokenType.IDENT)
            {
                SequenceTypeNode seq = new SequenceTypeNode(GccAttribute());
                while (sym == TokenType.COMMA)
                {
                    GetToken();
                    seq.Add(GccAttribute());
                }
                //allSeq.Add(seq);

                return seq;
            }
            return NullNode.Instance;
        }

        private ASTNode GccAttribute()
        {
            if (sym == TokenType.IDENT)
            {
                GetToken();
                if (sym == TokenType.LPAR)
                {
                    GetToken();
                    if (sym != TokenType.RPAR) ArgumentExpressionList();
                    check(TokenType.RPAR);
                }
            }
            return NullNode.Instance;
        }

        private ASTNode ParameterTypeList()
        {
            ASTNode paramList = ParameterList();
            if (sym == TokenType.COMMA)
            {
                GetToken();
                check(TokenType.ELLIPSIS);
            }
            return paramList;
        }

        private ASTNode ParameterList()
        {
            SequenceTypeNode prms = new SequenceTypeNode(ParameterDeclaration());

            while (sym == TokenType.COMMA)
            {
                if (GetLATok(2) == TokenType.ELLIPSIS) break;

                GetToken();
                prms.Add(ParameterDeclaration());
            }
            //allSeq.Add(prms);

            return prms;
        }

        private static bool IsFirstOfAbstractDecl(TokenType type)
        { return type == TokenType.MUL || type == TokenType.LPAR || type == TokenType.LBRACK; }

        private ASTNode ParameterDeclaration()
        {
            ASTNode typeSec = DeclrSpecifiers();
            ASTNode declr = NullNode.Instance;

            if (sym == TokenType.IDENT)
            {
                ASTNode dec = Declarator(ref typeSec);
                declr = new ParametrDecNode(typeSec, dec);
            }
            else if (IsFirstOfAbstractDecl(sym))
                declr = new ParametrDecNode(typeSec, AbstractDeclarator());
            declr.Declare(ast);
            return declr;
        }

        private ASTNode IqdentifierList()
        {
            check(TokenType.IDENT);
            SequenceTypeNode res = new SequenceTypeNode(new IdentNode(tok.Name, tok.Line));

            while (sym == TokenType.COMMA)
            {
                GetToken();
                check(TokenType.IDENT);
                res.Add(new IdentNode(tok.Name, tok.Line));
            }
            //allSeq.Add(res);

            return res;
        }
        private ASTNode AbstractDeclarator()
        {
            if (sym == TokenType.MUL)
            {
                GetToken();
                if (sym == TokenType.LPAR || sym == TokenType.LBRACK)
                    DirectABstractDeclr();
            }
            else if (sym == TokenType.LPAR || sym == TokenType.LBRACK)
                DirectABstractDeclr();
            else
                ParsingError("expected '*' or direct abstract declarator");
            return NullNode.Instance;
        }

        private ASTNode DirectABstractDeclr()
        {
            if (sym == TokenType.LPAR)
            {
                GetToken();
                if (IsFirstOfAbstractDecl(sym))
                    AbstractDeclarator();
                check(TokenType.RPAR);
            }
            else if (sym == TokenType.LBRACK)
            {
                GetToken();
                check(TokenType.RBRACK);
            }

            return NullNode.Instance;
        }

        private ASTNode Initializer()
        {
            ASTNode res = NullNode.Instance;

            if (sym == TokenType.LBRACE)
            {
                GetToken();

                res = InitializerList();

                check(TokenType.RBRACE);
            }
            else res = AssignmentExpression();

            return res;
        }

        private ASTNode InitializerList()
        {
            ASTNode init = Initializer();
            SequenceTypeNode res = new SequenceTypeNode(init);

            while (sym == TokenType.COMMA)
            {
                GetToken();

                init = Initializer();
                res.Add(init);
            }
            //allSeq.Add(res);

            return res;
        }

        private ASTNode Statement()
        {
            ASTNode stmt = NullNode.Instance;
            if (sym == TokenType.IDENT)
            {
                if (GetLATok(2) == TokenType.COLON)
                    stmt = LabeledStatement();
                else
                    stmt = ExpressionStmt();
            }
            else if ((sym == TokenType.DEC || sym == TokenType.INC) && GetLATok(2) == TokenType.IDENT) stmt = ExpressionStmt();
            else if (sym == TokenType.MUL || (sym == TokenType.LPAR && GetLATok(2) == TokenType.MUL))
                stmt = ExpressionStmt();
            else if (sym == TokenType.CASE || sym == TokenType.DEFAULT)
                stmt = LabeledStatement();
            else if (sym == TokenType.LBRACE)
                stmt = CompoundStatement();
            else if (sym == TokenType.IF || sym == TokenType.SWITCH)
                stmt = SelectionStatement();
            else if (sym == TokenType.WHILE || sym == TokenType.DO || sym == TokenType.FOR)
                stmt = IterationStatement();
            else if (sym == TokenType.GOTO || sym == TokenType.CONTINUE || sym == TokenType.BREAK
                 || sym == TokenType.RETURN)
                stmt = JumpStatement();
            else if (sym == TokenType.SIZEOF)
            {
                stmt = ExpressionStmt();
            }
            else if (sym == TokenType.ASM)
            {
                GetToken();
                check(TokenType.LPAR);
                check(TokenType.STRING_LIT);
                stmt = new AsmStmtNode(tok.Name);
                if (sym == TokenType.COLON)
                {
                    GetToken();
                    if (sym != TokenType.COLON)
                    {
                        check(TokenType.STRING_LIT);
                        check(TokenType.LPAR);
                        Expression();
                        check(TokenType.RPAR);
                        while (sym == TokenType.COMMA)
                        {
                            GetToken();
                            check(TokenType.STRING_LIT);
                            check(TokenType.LPAR);
                            Expression();
                            check(TokenType.RPAR);
                        }
                    }
                }
                if (sym == TokenType.COLON)
                {
                    GetToken();
                    if (sym != TokenType.COLON)
                    {
                        check(TokenType.STRING_LIT);
                        check(TokenType.LPAR);
                        Expression();
                        check(TokenType.RPAR);
                        while (sym == TokenType.COMMA)
                        {
                            GetToken();
                            check(TokenType.STRING_LIT);
                            check(TokenType.LPAR);
                            Expression();
                            check(TokenType.RPAR);
                        }
                    }
                }
                if (sym == TokenType.COLON)
                {
                    GetToken();
                    if (sym != TokenType.RPAR)
                    {
                        check(TokenType.STRING_LIT);
                        while (sym == TokenType.COMMA)
                        {
                            GetToken();
                            check(TokenType.STRING_LIT);
                        }
                    }
                }
                check(TokenType.RPAR);
                check(TokenType.SEMICOLON);
            }
            return stmt;
        }

        private ASTNode LabeledStatement()
        {
            ASTNode labstmt = NullNode.Instance;

            if (sym == TokenType.IDENT)
            {
                GetToken();
                ASTNode label = new IdentNode(tok.Name, tok.Line);
                check(TokenType.COLON);
                ASTNode stmt = Statement();
                labstmt = new LabelStmtNode(label, stmt);
            }
            else if (sym == TokenType.CASE)
            {
                GetToken();
                ASTNode expr = ConstantExpression();
                check(TokenType.COLON);
                ASTNode stmt = Statement();
                labstmt = new CaseLabelNode(expr, stmt);
            }
            else if (sym == TokenType.DEFAULT)
            {
                GetToken();
                check(TokenType.COLON);
                ASTNode stmt = Statement();
                labstmt = new CaseLabelNode(NullNode.Instance, stmt) { isDefault = true };
            }

            return labstmt;
        }

        private ASTNode CompoundStatement()
        {
            ASTNode compstmt = NullNode.Instance;
            check(TokenType.LBRACE);

            if (sym != TokenType.RBRACE)
                compstmt = StmtOrDecList();
            check(TokenType.RBRACE);

            return compstmt;
        }

        private ASTNode FunctionBody()
        {
            if (sym != TokenType.LBRACE)
                return NullNode.Instance;

            GetToken();

            var compound = new CompoundStmtNode(NullNode.Instance, NullNode.Instance);
            ASTNode decSeq = NullNode.Instance;
            ASTNode stmSeq = NullNode.Instance;

            while (sym != TokenType.RBRACE && sym != TokenType.EOF)
            {
                if (isStorageClassSpecifier(sym) || isTypeQuilifer(sym) || isTypeSpecifiar(sym, 1))
                {
                    ASTNode declSpec = DeclrSpecifiers();
                    ASTNode decl;

                    if (inTypeDef)
                    {
                        inTypeDef = false;
                        decl = new TypeDeclNode(Declarator(ref declSpec), declSpec);
                    }
                    else
                        decl = Declaration(declSpec);

                    decl.Declare(ast);

                    if (decSeq == NullNode.Instance)
                        decSeq = new SequenceTypeNode(decl);
                    else
                        ((SequenceTypeNode)decSeq).Add(decl);

                    continue;
                }

                ASTNode stmt = Statement();

                if (stmSeq == NullNode.Instance)
                    stmSeq = new SequenceTypeNode(stmt);
                else
                    ((SequenceTypeNode)stmSeq).Add(stmt);

                if (stmt.Type == ASTNodeType.ASTNT_RETURN_STMT)
                {
                    SkipToMatchingRBrace();
                    break;
                }
            }

            check(TokenType.RBRACE);

            compound.decls = decSeq;
            compound.stmt = stmSeq;
            return compound;
        }

        private ASTNode DeclarationList() => NullNode.Instance;

        private ASTNode StmtOrDecList()
        {
            ASTNode decl = NullNode.Instance;
            ASTNode stmt = NullNode.Instance;
            ASTNode stmts = NullNode.Instance;
            ASTNode decls = NullNode.Instance;

            while (isStorageClassSpecifier(sym) || isTypeQuilifer(sym) || isTypeSpecifiar(sym, 1))
            {
                ASTNode declSpec = DeclrSpecifiers();

                if (inTypeDef)
                {
                    inTypeDef = false;
                    decl = new TypeDeclNode(Declarator(ref declSpec), declSpec);
                    decl.Declare(ast);
                    check(TokenType.SEMICOLON);
                }
                else
                {
                    decl = Declaration(declSpec);
                    decl.Declare(ast);
                }

                if (decls == NullNode.Instance)
                    decls = new SequenceTypeNode(decl);
                else
                    ((SequenceTypeNode)decls).Add(decl);
            }

            while (true)
            {
                bool isStmtStart =
                    sym == TokenType.IDENT ||
                    ((sym == TokenType.DEC || sym == TokenType.INC) && GetLATok(2) == TokenType.IDENT) ||
                    (sym >= TokenType.ASM && sym <= TokenType.WHILE) ||
                    sym == TokenType.MUL ||
                    (sym == TokenType.LPAR && GetLATok(2) == TokenType.MUL);

                if (!isStmtStart) break;

                stmt = Statement();

                if (stmts == NullNode.Instance)
                    stmts = new SequenceTypeNode(stmt);
                else
                    ((SequenceTypeNode)stmts).Add(stmt);

                if (stmt.Type == ASTNodeType.ASTNT_RETURN_STMT)
                {
                    SkipToMatchingRBrace();
                    break;
                }
            }

            return new CompoundStmtNode(decls, stmts);
        }

        private ASTNode SelectionStatement()
        {
            ASTNode selstmt = NullNode.Instance;

            if (sym == TokenType.IF)
            {
                ASTNode expr = NullNode.Instance;
                ASTNode ifClause = NullNode.Instance;
                ASTNode elseClause = NullNode.Instance;

                GetToken();
                check(TokenType.LPAR);
                expr = Expression();
                check(TokenType.RPAR);

                ifClause = Statement();

                if (sym == TokenType.ELSE)
                {
                    GetToken();
                    elseClause = Statement();
                }
                selstmt = new IfStmtNode(expr, ifClause, elseClause);
            }
            else if (sym == TokenType.SWITCH)
            {
                ASTNode expr = NullNode.Instance;
                ASTNode stmt = NullNode.Instance;

                GetToken();
                check(TokenType.LPAR);
                expr = Expression();
                check(TokenType.RPAR);
                stmt = Statement();

                selstmt = new SwitchStmtNode(expr, stmt);
            }
            return selstmt;
        }

        private ASTNode IterationStatement()
        {
            ASTNode iterstmt = NullNode.Instance;
            ASTNode expr = NullNode.Instance;
            ASTNode stmt = NullNode.Instance;

            if (sym == TokenType.WHILE)
            {
                GetToken();
                check(TokenType.LPAR);

                expr = Expression();
                check(TokenType.RPAR);
                stmt = Statement();

                iterstmt = new WhileStmtNode(expr, stmt);
            }
            else if (sym == TokenType.DO)
            {
                GetToken();

                stmt = Statement();
                check(TokenType.WHILE);
                check(TokenType.LPAR);

                expr = Expression();
                check(TokenType.RPAR);
                check(TokenType.SEMICOLON);

                iterstmt = new DoStmtNode(expr, stmt);
            }
            else if (sym == TokenType.FOR)
            {
                ASTNode step = NullNode.Instance;
                ASTNode init = NullNode.Instance;

                GetToken();
                check(TokenType.LPAR);
                symbols.OpenScope();

                if (isStorageClassSpecifier(sym) || isTypeQuilifer(sym) || isTypeSpecifiar(sym, 1))
                {
                    ASTNode declSpec = DeclrSpecifiers();
                    init = Declaration(declSpec);
                    init.Declare(ast);
                }
                else if (sym != TokenType.SEMICOLON)
                {
                    init = Expression();
                    check(TokenType.SEMICOLON);
                }
                else if (sym == TokenType.SEMICOLON) GetToken();

                if (sym != TokenType.SEMICOLON)
                    expr = Expression();
                check(TokenType.SEMICOLON);

                if (sym != TokenType.RPAR)
                    step = Expression();
                check(TokenType.RPAR);

                stmt = Statement();

                var forNode = new ForStmtNode(init, expr, step, stmt);
                iterstmt = forNode;

                symbols.CloseScope();
            }

            return iterstmt;
        }

        private ASTNode JumpStatement()
        {
            ASTNode jumpstmt = NullNode.Instance;
            ASTNode expr = NullNode.Instance;

            if (sym == TokenType.GOTO)
            {
                GetToken();
                check(TokenType.IDENT);
                jumpstmt = new GotoStmtNode(new IdentNode(tok.Name, tok.Line));
                check(TokenType.SEMICOLON);
            }
            else if (sym == TokenType.CONTINUE)
            {
                GetToken();
                jumpstmt = new ContinueStmtNode();
                check(TokenType.SEMICOLON);
            }
            else if (sym == TokenType.BREAK)
            {
                GetToken();
                jumpstmt = new BreakStmtNode();
                check(TokenType.SEMICOLON);
            }
            else if (sym == TokenType.RETURN)
            {
                GetToken();
                if (sym != TokenType.SEMICOLON)
                    expr = Expression();
                check(TokenType.SEMICOLON);
                jumpstmt = new ReturnStmtNode(ast.intTypeNode, expr);
                jumpstmt.LineNumber = tok.Line;
            }

            return jumpstmt;
        }

        private ASTNode ExpressionStmt()
        {
            ASTNode expr = NullNode.Instance;
            //GetToken();
            if (sym != TokenType.SEMICOLON)
                expr = Expression();
            check(TokenType.SEMICOLON);

            return expr;
        }

        private ASTNode TranslationUnit()
        {
            ASTNode exprdecl = NullNode.Instance;
            ASTNode tunit = NullNode.Instance;

            symbols.OpenScope();

            while (true)
            {
                if (isStorageClassSpecifier(sym) || isTypeQuilifer(sym) || isTypeSpecifiar(sym, 1) ||
                    sym == TokenType.__ASM || sym == TokenType.__ATTRIBUTE__ || sym == TokenType.INLINE
                    || sym == TokenType.NORETURN)
                {
                    exprdecl = ExternalDeclaration();
                    if (tunit == NullNode.Instance)
                        tunit = new SequenceTypeNode(exprdecl);
                    else
                        ((SequenceTypeNode)tunit).Add(exprdecl);
                }
                else if (sym == TokenType.EOF || sym == TokenType.NONE) break;
                else
                {
                    ParsingError($"expected external declaration, found '$d' {sym}");
                }
            }

            ((SequenceTypeNode)tunit).scope = symbols.GetCurrentScope();
            symbols.CloseScope();

            return tunit;
        }

        private ASTNode ExternalDeclaration()
        {
            ASTNode declSpec = DeclrSpecifiers();

            if (GetLATok(2) == TokenType.LPAR || GetLATok(3) == TokenType.LPAR)
                return FunctionDef(declSpec);

            ASTNode decl;
            if (inTypeDef)
            {
                inTypeDef = false;
                decl = new TypeDeclNode(Declarator(ref declSpec), declSpec);
                decl.Declare(ast);
                check(TokenType.SEMICOLON);
            }
            else
            {
                decl = Declaration(declSpec);
                decl.Declare(ast);
            }
            return decl;
        }
        private ASTNode FunctionDef(ASTNode funcType)
        {
            ASTNode funcName, funcPrms = NullNode.Instance;

            if (sym == TokenType.MUL) { GetToken(); funcType = new PointerTypeNode(funcType); }

            ASTNode tmp = NullNode.Instance;
            funcName = Declarator(ref tmp);
            var funcSym = symbols.Insert(((IdentNode)funcName).val, ObjectKind.OK_FUNC, null);


            symbols.OpenScope();

            check(TokenType.LPAR);

            var funDecl = new FunctionDeclNode(funcType, funcName, NullNode.Instance, NullNode.Instance);

            if (sym != TokenType.RPAR)
            {
                funcPrms = (SequenceTypeNode)ParameterTypeList();
                funDecl.parametrs = funcPrms;

            }
            check(TokenType.RPAR);
            while (sym == TokenType.__ASM || sym == TokenType.__ATTRIBUTE__) GccDeclaratorExtension();

            if (sym != TokenType.SEMICOLON) funDecl.body = FunctionBody();
            else GetToken();

            SOK funcObj = symbols.Find(((IdentNode)funcName).val);
            if (funcObj != symbols.NoObj)
            {
                funcObj.Locals = symbols.GetCurrentScope().Locals;
                funcObj.Prmc = (uint)symbols.GetCurrentScope().NumberParametrs;
            }

            funDecl.scop = symbols.GetCurrentScope();
            symbols.CloseScope();
            ast.currFD = null;
            return funDecl;
        }
        public override void parse(string output)
        {
            MacroTable.Clear();
            ASTNode tree = NullNode.Instance;

            lexer.GetNextChar();
            initTokenBuffer();

            RegisterHeaderFunctions();

            tree = TranslationUnit();
            ast.root = tree;
        }
        public CParser(CLexer lexer) : base(lexer)
        {
            inTypeDef = false;
            allSeq = new SequenceTypeNode();

            initName();
        }
    }
}
