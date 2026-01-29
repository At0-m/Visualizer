using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace Visualizer
{


    public enum ASTNodeType
    {
        ASTNT_NONE,
        ASTNT_IDENT_NODE,
        ASTNT_LIST,
        ASTNT_SIZEOF_EXPR,
        ASTNT_ALIGNOF_EXPR,


        ASTNT_FUNCTION_DECL,
        ASTNT_TYPE_DECL,
        ASTNT_VAR_DECL,
        ASTNT_PARM_DECL,
        ASTNT_FIELD_DECL,


        ASTNT_ASM_STMT,
        ASTNT_BREAK_STMT,
        ASTNT_CASE_LABEL,
        ASTNT_COMPOUND_STMT,
        ASTNT_CONTINUE_STMT,
        ASTNT_DECL_STMT,
        ASTNT_DO_STMT,
        ASTNT_EXPR_STMT,
        ASTNT_FILE_STMT,
        ASTNT_FOR_STMT,
        ASTNT_GOTO_STMT,
        ASTNT_IF_STMT,
        ASTNT_LABEL_STMT,
        ASTNT_RETURN_STMT,
        ASTNT_SCOPE_STMT,
        ASTNT_SWITCH_STMT,
        ASTNT_WHILE_STMT,


        ASTNT_INTEGER_CONST,
        ASTNT_REAL_CONST,
        ASTNT_COMPLEX_CONST,
        ASTNT_STRING_CONST,
        ASTNT_CHAR_CONST,
        ASTNT_CAST_EXPR,
        ASTNT_NEGATE_EXPR,
        ASTNT_BIT_NOT_EXPR,
        ASTNT_LOG_NOT_EXPR,
        ASTNT_PREDECREMENT_EXPR,
        ASTNT_PREINCREMENT_EXPR,
        ASTNT_POSTDECREMENT_EXPR,
        ASTNT_POSTINCREMENT_EXPR,
        ASTNT_ADDR_EXPR,
        ASTNT_INDIRECT_REF,
        ASTNT_FIX_TRUNC_EXPR,
        ASTNT_FLOAT_EXPR,
        ASTNT_COMPLEX_EXPR,
        ASTNT_NON_LVALUE_EXPR,
        ASTNT_NOP_EXPR,
        ASTNT_LSHIFT_EXPR,
        ASTNT_RSHIFT_EXPR,
        ASTNT_BIT_IOR_EXPR,
        ASTNT_BIT_XOR_EXPR,
        ASTNT_BIT_AND_EXPR,
        ASTNT_LOG_AND_EXPR,
        ASTNT_LOG_OR_EXPR,
        ASTNT_PLUS_EXPR,
        ASTNT_MINUS_EXPR,
        ASTNT_MULT_EXPR,
        ASTNT_TRUNC_DIV_EXPR,
        ASTNT_TRUNC_MOD_EXPR,
        ASTNT_RDIV_EXPR,
        ASTNT_ARRAY_REF,
        ASTNT_STRUCT_REF,
        ASTNT_LT_EXPR,
        ASTNT_LE_EXPR,
        ASTNT_GT_EXPR,
        ASTNT_GE_EXPR,
        ASTNT_EQ_EXPR,
        ASTNT_NE_EXPR,
        ASTNT_ASSIGN_EXPR,
        ASTNT_INIT_EXPR,
        ASTNT_COMPOUND_EXPR,
        ASTNT_COND_EXPR,
        ASTNT_CALL_EXPR,


        ASTNT_GETC_EXPR,
        ASTNT_PUTC_EXPR,
        ASTNT_GETI_EXPR,
        ASTNT_PUTI_EXPR,


        ASTNT_VOID_TYPE,
        ASTNT_INTEGRAL_TYPE,
        ASTNT_NORMAL_TYPE,
        ASTNT_COMPLEX_TYPE,
        ASTNT_ENUMERAL_TYPE,
        ASTNT_BOOLEAN_TYPE,
        ASTNT_POINTER_TYPE,
        ASTNT_REFERENCE_TYPE,
        ASTNT_FUNCTION_TYPE,
        ASTNT_ARRAY_TYPE,
        ASTNT_STRUCT_TYPE,
        ASTNT_NONE_TYPE,
        ASTNT_UNION_TYPE
    }

    public class ASTNode
    {
        public ASTNodeType Type { get; set; }
        public int LineNumber { get; set; }
        public TokenType Flags { get; set; }
        public uint RefCount { get; set; }

        public ASTNode()
        {
            Flags = TokenType.NONE;
            RefCount = 0;
            LineNumber = 0;
            Type = ASTNodeType.ASTNT_NONE;
        }

        public ASTNode(ASTNodeType type) : this()
        {
            Type = type;
        }

        public virtual void Declare(AST ast) { }
        public virtual TKType CheckType(AST ast) => null;
        public virtual void Accept(Visitor visitor) { }

        public bool IsType()
        {
            return Type == ASTNodeType.ASTNT_VOID_TYPE || Type == ASTNodeType.ASTNT_INTEGRAL_TYPE ||
                   Type == ASTNodeType.ASTNT_NORMAL_TYPE || Type == ASTNodeType.ASTNT_COMPLEX_TYPE ||
                   Type == ASTNodeType.ASTNT_ENUMERAL_TYPE || Type == ASTNodeType.ASTNT_BOOLEAN_TYPE ||
                   Type == ASTNodeType.ASTNT_POINTER_TYPE || Type == ASTNodeType.ASTNT_REFERENCE_TYPE ||
                   Type == ASTNodeType.ASTNT_FUNCTION_TYPE || Type == ASTNodeType.ASTNT_ARRAY_TYPE ||
                   Type == ASTNodeType.ASTNT_STRUCT_TYPE || Type == ASTNodeType.ASTNT_NONE_TYPE ||
                   Type == ASTNodeType.ASTNT_UNION_TYPE;
        }

        public void IncRefCount() => RefCount++;
        public void DecRefCount() => RefCount--;

    }

    public class NullNode : ASTNode
    {
        private static NullNode instance = null;

        public NullNode()
        {
            Type = ASTNodeType.ASTNT_NONE;
        }

        public static NullNode Instance => instance ??= new NullNode();

        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }

    public class IdentNode : ASTNode
    {
        public string val { get; }
        public IdentNode(string val)
        {
            this.val = val;
            Type = ASTNodeType.ASTNT_IDENT_NODE;
        }

        public IdentNode(string val, int line) : this(val)
        {
            LineNumber = line;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class IntConstNode : ASTNode
    {
        public int val { get; }

        public IntConstNode(int val)
        {
            this.val = val;
            Type = ASTNodeType.ASTNT_INTEGER_CONST;
        }
        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class NormalConstNode : ASTNode
    {
        public float val { get; }

        public NormalConstNode(float val)
        {
            this.val = val;
            Type = ASTNodeType.ASTNT_REAL_CONST;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class StringConstNode : ASTNode
    {
        public string val { get; }

        public StringConstNode(string val)
        {
            this.val = val;
            Type = ASTNodeType.ASTNT_STRING_CONST;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class CharConstNode : ASTNode
    {
        public int val { get; }

        public CharConstNode(int val)
        {
            this.val = val;
            Type = ASTNodeType.ASTNT_CHAR_CONST;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class SizeOfExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public SizeOfExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_SIZEOF_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class AlignOfExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public AlignOfExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_ALIGNOF_EXPR;

            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }



        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class TypeDeclNode : ASTNode
    {
        public ASTNode name { get; private set; }
        public ASTNode body { get; private set; }

        public TypeDeclNode(ASTNode name, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_TYPE_DECL;

            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.body = Tools.ASSIGN_NODE(this.body, body);

        }

        public void setName(ASTNode name) => this.name = Tools.ASSIGN_NODE(this.name, name);
        public void setBody(ASTNode body) => this.body = Tools.ASSIGN_NODE(this.body, body);


        public override void Accept(Visitor visitor) => visitor.Visit(this);


        public override void Declare(AST ast)
        {
            if (name == NullNode.Instance) throw new InvalidOperationException("Invalid type declaration");

            Symbols symbols = ast.stb;
            string typeName = ((IdentNode)name).val;
            int typeNameLine = ((IdentNode)name).LineNumber;
            SOK obj = null;

            if (ast.stb.Find(typeName) == ast.stb.NoObj)
                obj = symbols.Insert(typeName, ObjectKind.OK_VAR, symbols.NoType);
            else
                obj = symbols.Insert(typeName, ObjectKind.OK_TYPE, symbols.NoType);

            if (obj == symbols.NoObj)
                ast.Error(typeNameLine, "type name '%s' already exists", typeName);
        }
    }

    public class FunctionDeclNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode name { get; private set; }
        public ASTNode parametrs { get; set; }
        public ASTNode body { get; set; }
        public Scope scop { get; set; }


        public FunctionDeclNode(ASTNode type, ASTNode name, ASTNode parametrs, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_FUNCTION_DECL;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.parametrs = Tools.ASSIGN_NODE(this.parametrs, parametrs);
            this.body = Tools.ASSIGN_NODE(this.body, body);
            this.scop = null;
        }

        public void SetType(ASTNode type) => this.type = Tools.ASSIGN_NODE(this.type, type);
        public void SetName(ASTNode name) => this.name = Tools.ASSIGN_NODE(this.name, name);
        public void SetParametrs(ASTNode parametrs) => this.parametrs = Tools.ASSIGN_NODE(this.parametrs, parametrs);
        public void SetBody(ASTNode type) => this.body = Tools.ASSIGN_NODE(this.body, body);

        public bool IsFwdDec() => body == NullNode.Instance;

        public override void Accept(Visitor visitor) => visitor.Visit(this);



    }

    public class VarDeclNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode name { get; private set; }
        public ASTNode init { get; set; }

        public VarDeclNode(ASTNode type, ASTNode name)
        {
            Type = ASTNodeType.ASTNT_VAR_DECL;
            this.init = NullNode.Instance;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.name = Tools.ASSIGN_NODE(this.name, name); ;

        }

        public VarDeclNode(ASTNode type, ASTNode name, ASTNode init)
        {
            Type = ASTNodeType.ASTNT_VAR_DECL;
            this.init = Tools.ASSIGN_NODE(this.init, init);
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.name = Tools.ASSIGN_NODE(this.name, name);
        }

        public void SetType(ASTNode type) => this.type = Tools.ASSIGN_NODE(this.type, type);
        public void SetName(ASTNode name) => this.name = Tools.ASSIGN_NODE(this.name, name);
        public void SetInit(ASTNode init) => this.init = Tools.ASSIGN_NODE(this.init, init);


        public override void Accept(Visitor visitor) => visitor.Visit(this);

        public override void Declare(AST ast)
        {
            if (name == NullNode.Instance)
                throw new InvalidOperationException("anonymous variable");

            string id = ((IdentNode)name).val;
            int line = ((IdentNode)name).LineNumber;

            TKType tk = TypeResolver.Resolve(type, ast);

            ObjectKind kind = (Flags & TokenType.STATIC) != 0
                                ? ObjectKind.OK_VAR_STATIC
                                : ObjectKind.OK_VAR;

            if (ast.stb.Find(id) == ast.stb.NoObj)
                ast.stb.Insert(id, kind, tk);
        }

    }

    public class ParametrDecNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode name { get; private set; }

        public ParametrDecNode(ASTNode type, ASTNode name)
        {
            Type = ASTNodeType.ASTNT_PARM_DECL;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.name = Tools.ASSIGN_NODE(this.name, name);
        }


        public override void Accept(Visitor visitor) => visitor.Visit(this);

        public override void Declare(AST ast)
        {
            if (name == NullNode.Instance) return;


            string id = ((IdentNode)name).val;
            int line = ((IdentNode)name).LineNumber;

            TKType tk = TypeResolver.Resolve(type, ast);

            SOK obj = ast.stb.Insert(id, ObjectKind.OK_PAR, tk);
            if (obj == ast.stb.NoObj)
                ast.Error(line, "duplicate parameter '%s'", id);
        }

    }

    public class FieldDecNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode name { get; private set; }

        public FieldDecNode(ASTNode type, ASTNode name)
        {
            Type = ASTNodeType.ASTNT_FIELD_DECL;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.name = Tools.ASSIGN_NODE(this.name, name);
        }

        public void SetType(ASTNode type) => this.type = Tools.ASSIGN_NODE(this.type, type);
        public void SetName(ASTNode name) => this.name = Tools.ASSIGN_NODE(this.name, name);

        public override void Accept(Visitor visitor) => visitor.Visit(this);
        public override void Declare(AST ast)
        {
            string id = ((IdentNode)name).val;
            TKType tk = ast.stb.NoType;
            ast.stb.Insert(id, ObjectKind.OK_VAR, tk);
        }

    }

    public class AsmStmtNode : ASTNode
    {
        public string data { get; }

        public AsmStmtNode(string data)
        {
            Type = ASTNodeType.ASTNT_ASM_STMT;
            this.data = data;
        }
        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class BreakStmtNode : ASTNode
    {
        public BreakStmtNode() => Type = ASTNodeType.ASTNT_BREAK_STMT;

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class CaseLabelNode : ASTNode
    {
        public ASTNode expr { get; private set; }
        public ASTNode stmt { get; private set; }
        public bool isDefault { get; set; }
        public CaseLabelNode(ASTNode expr, ASTNode stmt)
        {
            Type = ASTNodeType.ASTNT_CASE_LABEL;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
            this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);
            isDefault = false;
        }

        public void setExpr(ASTNode expr)
        {
            this.expr = Tools.DELETE_REF(this.expr);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }
        public void setStmt(ASTNode stmt)
        {
            this.stmt = Tools.DELETE_REF(this.stmt);
            this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }


    public class CompoundStmtNode : ASTNode
    {
        public ASTNode decls { get; set; }
        public ASTNode stmt { get; set; }
        public ASTNode scope { get; set; }

        public CompoundStmtNode(ASTNode decls, ASTNode stmt)
        {
            Type = ASTNodeType.ASTNT_COMPOUND_STMT;
            this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);
            this.decls = Tools.ASSIGN_NODE(this.decls, decls);
            scope = null;
        }

        public void setDecls(ASTNode decls) => this.decls = Tools.ASSIGN_NODE(this.decls, decls);
        public void setStmt(ASTNode stmt) => this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);

        public override void Declare(AST ast) => decls.Declare(ast);

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class ContinueStmtNode : ASTNode
    {
        public ContinueStmtNode() => Type = ASTNodeType.ASTNT_CONTINUE_STMT;

        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }

    public class DoStmtNode : ASTNode
    {
        public ASTNode condition { get; private set; }
        public ASTNode body { get; private set; }

        public DoStmtNode(ASTNode condition, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_DO_STMT;
            this.condition = Tools.ASSIGN_NODE(this.condition, condition);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class ForStmtNode : ASTNode
    {
        public ASTNode init { get; private set; }
        public ASTNode condition { get; private set; }

        public ASTNode step { get; private set; }
        public ASTNode body { get; private set; }


        public ForStmtNode(ASTNode init, ASTNode condition, ASTNode step, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_FOR_STMT;
            this.init = Tools.ASSIGN_NODE(this.init, init);
            this.condition = Tools.ASSIGN_NODE(this.condition, condition);
            this.step = Tools.ASSIGN_NODE(this.step, step);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class GotoStmtNode : ASTNode
    {
        public ASTNode label { get; private set; }

        public GotoStmtNode(ASTNode label)
        {
            Type = ASTNodeType.ASTNT_GOTO_STMT;
            this.label = Tools.ASSIGN_NODE(this.label, label);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class IfStmtNode : ASTNode
    {
        public ASTNode condition { get; private set; }
        public ASTNode ifClause { get; private set; }
        public ASTNode elseClause { get; private set; }

        public IfStmtNode(ASTNode condition, ASTNode ifClause, ASTNode elseClause)
        {
            Type = ASTNodeType.ASTNT_IF_STMT;

            this.condition = Tools.ASSIGN_NODE(this.condition, condition);
            this.ifClause = Tools.ASSIGN_NODE(this.ifClause, ifClause);
            this.elseClause = Tools.ASSIGN_NODE(this.elseClause, elseClause);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);



    }

    public class LabelStmtNode : ASTNode
    {
        public ASTNode label { get; private set; }
        public ASTNode stmt { get; private set; }

        public LabelStmtNode(ASTNode label, ASTNode stmt)
        {
            Type = ASTNodeType.ASTNT_LABEL_STMT;
            this.label = Tools.ASSIGN_NODE(this.label, label);
            this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }

    public class ReturnStmtNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }

        public ReturnStmtNode(ASTNode type, ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_RETURN_STMT;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class SwitchStmtNode : ASTNode
    {
        public ASTNode expr { get; private set; }
        public ASTNode stmt { get; private set; }

        public SwitchStmtNode(ASTNode expr, ASTNode stmt)
        {
            Type = ASTNodeType.ASTNT_SWITCH_STMT;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
            this.stmt = Tools.ASSIGN_NODE(this.stmt, stmt);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }


    public class WhileStmtNode : ASTNode
    {
        public ASTNode condition { get; private set; }
        public ASTNode body { get; private set; }

        public WhileStmtNode(ASTNode condition, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_WHILE_STMT;
            this.condition = Tools.ASSIGN_NODE(this.condition, condition);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class CastExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }

        public CastExprNode(ASTNode type, ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_CAST_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class BitNotNodeExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }

        public BitNotNodeExprNode(ASTNode type, ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_BIT_NOT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class LogNotExrNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }

        public LogNotExrNode(ASTNode type, ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_LOG_NOT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class PreDecExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public PreDecExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_PREDECREMENT_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class PreIncExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public PreIncExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_PREINCREMENT_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class PostDecExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public PostDecExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_POSTDECREMENT_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class PostIncExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public PostIncExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_POSTINCREMENT_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class AddrExprNode : ASTNode
    {
        public ASTNode type { get; private set; }

        public ASTNode expr { get; private set; }

        public AddrExprNode(ASTNode expr, ASTNode type)
        {
            Type = ASTNodeType.ASTNT_ADDR_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class IndirectRefNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }
        public ASTNode field { get; private set; }


        public IndirectRefNode(ASTNode expr, ASTNode type, ASTNode field)
        {
            Type = ASTNodeType.ASTNT_INDIRECT_REF;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
            this.field = Tools.ASSIGN_NODE(this.field, field); ;
        }

        public void SetExpr(ASTNode expr)
        {
            this.expr = Tools.DELETE_REF(this.expr);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }
        public void SetType(ASTNode type)
        {
            this.type = Tools.DELETE_REF(this.type);
            this.type = Tools.ASSIGN_NODE(this.type, type);
        }
        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class NopExprNode : ASTNode
    {
        public NopExprNode() => Type = ASTNodeType.ASTNT_NOP_EXPR;
        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }

    public class LShiftExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public LShiftExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_LSHIFT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }


    public class RShiftExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public RShiftExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_RSHIFT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class BitOrExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public BitOrExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_BIT_IOR_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class BitXorExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public BitXorExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_BIT_XOR_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class BitAndExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public BitAndExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_BIT_AND_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class LogAndExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public LogAndExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_LOG_AND_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class LogOrExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public LogOrExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_LOG_OR_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class PlusExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public PlusExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_PLUS_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class MinusExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public MinusExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_MINUS_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);
    }
    public class NegateExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }

        public NegateExprNode(ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_NEGATE_EXPR;   
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        //public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class MultExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public MultExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_MULT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class TruncDivExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public TruncDivExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_TRUNC_DIV_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class TruncModExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }


        public TruncModExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_TRUNC_MOD_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class ArrayRefNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }
        public ASTNode index { get; private set; }


        public ArrayRefNode(ASTNode expr, ASTNode type, ASTNode index)
        {
            Type = ASTNodeType.ASTNT_ARRAY_REF;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
            this.index = Tools.ASSIGN_NODE(this.index, index); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class StructRefNode : ASTNode
    {
        public ASTNode name { get; set; }
        public ASTNode member { get; set; }


        public StructRefNode(ASTNode name, ASTNode member)
        {
            Type = ASTNodeType.ASTNT_STRUCT_REF;
            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.member = Tools.ASSIGN_NODE(this.member, member);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class LtExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public LtExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_LT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class LeExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public LeExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_LE_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

        
    }
    public class GtExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public GtExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_GT_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class GeExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public GeExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_GE_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class EqExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public EqExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_EQ_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }
    public class NeExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public NeExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_NE_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class AssignExprNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode lhs { get; private set; }
        public ASTNode rhs { get; private set; }

        public AssignExprNode(ASTNode lhs, ASTNode type, ASTNode rhs)
        {
            Type = ASTNodeType.ASTNT_ASSIGN_EXPR;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.lhs = Tools.ASSIGN_NODE(this.lhs, lhs);
            this.rhs = Tools.ASSIGN_NODE(this.rhs, rhs); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);


    }

    public class CondExprNode : ASTNode
    {
        public ASTNode condition { get; private set; }
        public ASTNode ifClause { get; private set; }
        public ASTNode elseClause { get; private set; }

        public CondExprNode(ASTNode condition, ASTNode ifClause, ASTNode elseClause)
        {
            Type = ASTNodeType.ASTNT_COND_EXPR;
            this.condition = Tools.ASSIGN_NODE(this.condition, condition);
            this.ifClause = Tools.ASSIGN_NODE(this.ifClause, ifClause);
            this.elseClause = Tools.ASSIGN_NODE(this.elseClause, elseClause); ;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }
    public class CallExprNode : ASTNode
    {
        public ASTNode expr { get; private set; }
        public ASTNode args { get; private set; }

        public CallExprNode(ASTNode expr, ASTNode args)
        {
            Type = ASTNodeType.ASTNT_CALL_EXPR;
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
            this.args = Tools.ASSIGN_NODE(this.args, args);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class VoidTypeNode : ASTNode
    {
        public VoidTypeNode() => Type = ASTNodeType.ASTNT_VOID_TYPE;

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class IntTypeNode : ASTNode
    {
        public uint alignment { get; }
        public bool isSigned { get; }

        public IntTypeNode(uint alignment, bool isSigned)
        {
            Type = ASTNodeType.ASTNT_INTEGRAL_TYPE;
            this.alignment = alignment;
            this.isSigned = isSigned;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class NormalTypeNode : ASTNode
    {
        public uint alignment { get; }
        public bool isDouble { get; }

        public NormalTypeNode(uint alignment, bool isDouble)
        {
            Type = ASTNodeType.ASTNT_NORMAL_TYPE;
            this.alignment = alignment;
            this.isDouble = isDouble;
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class EnumeralTypeNode : ASTNode
    {
        public ASTNode name { get; private set; }
        public ASTNode body { get; private set; }

        public EnumeralTypeNode(ASTNode name, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_ENUMERAL_TYPE;
            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class PointerTypeNode : ASTNode
    {
        public ASTNode baseType { get; set; }

        public PointerTypeNode(ASTNode baseType)
        {
            Type = ASTNodeType.ASTNT_POINTER_TYPE;
            this.baseType = Tools.ASSIGN_NODE(this.baseType, baseType);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class FunctionTypeNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode prms { get; set; }


        public FunctionTypeNode(ASTNode type, ASTNode prms)
        {
            Type = ASTNodeType.ASTNT_FUNCTION_TYPE;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.prms = Tools.ASSIGN_NODE(this.prms, prms);
        }

        public void setType(ASTNode type) => this.type = Tools.ASSIGN_NODE(this.type, type);
        public void setPrms(ASTNode prms) => this.prms = Tools.ASSIGN_NODE(this.prms, prms);


        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class ArrayTypeNode : ASTNode
    {
        public ASTNode type { get; private set; }
        public ASTNode expr { get; private set; }


        public ArrayTypeNode(ASTNode type, ASTNode expr)
        {
            Type = ASTNodeType.ASTNT_ARRAY_TYPE;
            this.type = Tools.ASSIGN_NODE(this.type, type);
            this.expr = Tools.ASSIGN_NODE(this.expr, expr);
        }

        public void setType(ASTNode type) => this.type = Tools.ASSIGN_NODE(this.type, type);

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class StructTypeNode : ASTNode
    {
        public ASTNode name { get; private set; }
        public ASTNode body { get; private set; }


        public StructTypeNode(ASTNode name, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_STRUCT_TYPE;
            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class UnionTypeNode : ASTNode
    {
        public ASTNode name { get; private set; }
        public ASTNode body { get; private set; }


        public UnionTypeNode(ASTNode name, ASTNode body)
        {
            Type = ASTNodeType.ASTNT_UNION_TYPE;
            this.name = Tools.ASSIGN_NODE(this.name, name);
            this.body = Tools.ASSIGN_NODE(this.body, body);
        }

        public override void Accept(Visitor visitor) => visitor.Visit(this);

    }

    public class SequenceTypeNode : ASTNode
    {
        public Scope scope { get; set; }
        public List<ASTNode> elements { get; private set; }


        public SequenceTypeNode()
        {
            elements = new List<ASTNode>();
            Type = ASTNodeType.ASTNT_LIST;
            scope = null;
        }

        public SequenceTypeNode(ASTNode node)
        {
            elements = new List<ASTNode>();
            Type = ASTNodeType.ASTNT_LIST;
            AddEl(node);
        }

        public void Add(ASTNode node)
        {
            if (node.Type == ASTNodeType.ASTNT_LIST)
            {
                var temp = ((SequenceTypeNode)node).elements;

                foreach (var e in temp) AddEl(e);

                node = null;
            }
            else AddEl(node);
        }
        public void AddEl(ASTNode el)
        {
            if (el != NullNode.Instance)
            {
                elements.Add(el);
                el.IncRefCount();
            }
        }

        public int size() => elements.Count();
        public override void Accept(Visitor visitor) => visitor.Visit(this);


        public override void Declare(AST ast)
        {
            foreach (var e in elements)
                e.Declare(ast);
        }
    }

}