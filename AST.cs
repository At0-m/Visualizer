using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    public class AST
    {
        public ASTNode root { get; set; }
        public Symbols stb { get; private set; }
        public SOK currFD { get; set; }

        public int errros { get; }
        public int warnings { get; private set; }

        public SOK currRecordObj;

        public ASTNode voidTypeNode { get; }
        public ASTNode intTypeNode { get; }
        public ASTNode longTypeNode { get; }
        public ASTNode unsignedTypeNode { get; }
        public ASTNode shortTypeNode { get; }
        public ASTNode charTypeNode { get; }
        public ASTNode floatTypeNode { get; }
        public ASTNode doubleTypeNode { get; }

        List<StructTypeNode> stuctTypes;
        List<UnionTypeNode> unionTypes;

        public AST(Symbols stb)
        {
            this.stb = stb;
            voidTypeNode = Tools.ASSIGN_NODE(voidTypeNode, new VoidTypeNode());
            charTypeNode = Tools.ASSIGN_NODE(charTypeNode, new IntTypeNode(1, true));
            shortTypeNode = Tools.ASSIGN_NODE(shortTypeNode, new IntTypeNode(2, true));
            intTypeNode = Tools.ASSIGN_NODE(intTypeNode, new IntTypeNode(4, true));
            longTypeNode = Tools.ASSIGN_NODE(longTypeNode, new IntTypeNode(8, true));
            unsignedTypeNode = Tools.ASSIGN_NODE(unsignedTypeNode, new IntTypeNode(4, false));
            floatTypeNode = Tools.ASSIGN_NODE(floatTypeNode, new NormalTypeNode(4, false));
            doubleTypeNode = Tools.ASSIGN_NODE(doubleTypeNode, new NormalTypeNode(8, true));
            currFD = null;
            currRecordObj = null;
            errros = warnings = 0;

            root = null;
        }


        public void Error(int line, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Console.WriteLine($"Error: {message}; line {line}");
            Environment.Exit(1);
        }

        public void Warning(int line, string format, params object[] args)
        {
            string message = string.Format(format, args);
            Console.WriteLine($"Warning: {message}; line {line}");
            warnings++;
        }

        public void Declare()
        {
            if (root.Type == ASTNodeType.ASTNT_LIST)
                stb.SetCurrentScope(((SequenceTypeNode)root).scope);
            root.Declare(this);

            if (root.Type == ASTNodeType.ASTNT_LIST)
                stb.SetCurrentScope(null);
        }

        public void Visit(Visitor visitor)
        {
            if (root.Type == ASTNodeType.ASTNT_LIST)
                stb.SetCurrentScope(((SequenceTypeNode)root).scope);

            visitor.Visit((SequenceTypeNode)root);

            if (root.Type == ASTNodeType.ASTNT_LIST)
                stb.SetCurrentScope(null);
        }

        public TKType GetStbType(ASTNode node)
        {
            switch (node.Type)
            {
                case ASTNodeType.ASTNT_VOID_TYPE:
                    return stb.VoidType;
                case ASTNodeType.ASTNT_INTEGRAL_TYPE:
                    IntTypeNode intTypeNode = (IntTypeNode)node;
                    if (intTypeNode.isSigned)
                    {
                        return intTypeNode.alignment switch
                        {
                            1 => stb.CharType,
                            2 => stb.ShortType,
                            4 => stb.IntType,
                            8 => stb.LongType,
                            _ => stb.NoType
                        };
                    }
                    else
                    {
                        return intTypeNode.alignment switch
                        {
                            4 => stb.UnsignedType,
                            _ => stb.NoType
                        };
                    }
                case ASTNodeType.ASTNT_NORMAL_TYPE:
                    NormalTypeNode normalTypeNode = (NormalTypeNode)node;
                    return normalTypeNode.isDouble ? stb.DoubleType : stb.FloatType;
                case ASTNodeType.ASTNT_COMPLEX_TYPE: return stb.NoType;
                case ASTNodeType.ASTNT_ENUMERAL_TYPE: return stb.NoType;
                case ASTNodeType.ASTNT_BOOLEAN_TYPE: return stb.NoType;
                case ASTNodeType.ASTNT_REFERENCE_TYPE: return stb.NoType;

                case ASTNodeType.ASTNT_POINTER_TYPE:
                    {
                        TKType type = stb.AllocType(TypeKind.TK_POINTER);
                        PointerTypeNode pointerTypeNode = (PointerTypeNode)node;
                        type.BaseType = GetStbType(pointerTypeNode.baseType);
                        type.Size = 4;
                        return type;
                    }

                case ASTNodeType.ASTNT_FUNCTION_TYPE:
                    {
                        TKType type = stb.AllocType(TypeKind.TK_FUNCTION);
                        FunctionDeclNode FunctionTypeNode = (FunctionDeclNode)node;
                        type.FuncType = GetStbType(FunctionTypeNode.type);
                        type.Size = 4;
                        return type;
                    }
                case ASTNodeType.ASTNT_ARRAY_TYPE:
                    {
                        TKType type = stb.AllocType(TypeKind.TK_ARRAY);
                        ArrayTypeNode ArrayTypeNode = (ArrayTypeNode)node;
                        type.ElType = GetStbType(ArrayTypeNode.type);
                        if (ArrayTypeNode.expr.Type == ASTNodeType.ASTNT_INTEGER_CONST)
                            type.Size = type.ElType.Size * ((IntConstNode)ArrayTypeNode.expr).val;

                        return type;
                    }
                case ASTNodeType.ASTNT_STRUCT_TYPE:
                    {
                        TKType type = null;

                        StructTypeNode StructTypeNode = (StructTypeNode)node;

                        if (StructTypeNode.name.Type == ASTNodeType.ASTNT_IDENT_NODE)
                        {
                            string recName = ((IdentNode)StructTypeNode.name).val;
                            int recLine = ((IdentNode)StructTypeNode.name).LineNumber;
                            SOK obj = stb.Find(recName);

                            if (obj == stb.NoObj)
                                Error(recLine, "struct type '%s' not declared", recName);
                            else type = obj.Type;
                        }

                        return type;
                    }
                case ASTNodeType.ASTNT_UNION_TYPE:
                    {
                        TKType type = null;

                        UnionTypeNode unionTypeNode = (UnionTypeNode)node;

                        if (unionTypeNode.name.Type == ASTNodeType.ASTNT_IDENT_NODE)
                        {
                            string unionName = ((IdentNode)unionTypeNode.name).val;
                            int unionLine = ((IdentNode)unionTypeNode.name).LineNumber;
                            SOK obj = stb.Find(unionName);

                            if (obj == stb.NoObj)
                                Error(unionLine, "unione type '%s' not declared", unionName);
                            else type = obj.Type;
                        }

                        return type;
                    }
                case ASTNodeType.ASTNT_NONE_TYPE: return stb.NoType;
                default: return stb.NoType;
            }
        }
    }
    static class ASTHelpers
    {
        public static IEnumerable<ASTNode> Children(this ASTNode node)
        {
            if (node == null || node is NullNode) yield break;

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            var members = node.GetType().GetMembers(flags).Where(m => m is PropertyInfo or FieldInfo);

            foreach (var m in members)
            {
                object value = m switch
                {
                    PropertyInfo pi => pi.GetValue(node),
                    FieldInfo fi => fi.GetValue(node),
                    _ => null
                };

                switch (value)
                {
                    case null:
                        break;
                    case ASTNode child when child != NullNode.Instance:
                        yield return child;
                        break;
                    case IEnumerable list:
                        foreach (var el in list)
                            if (el is ASTNode n && n != NullNode.Instance)
                                yield return n;
                        break;
                }
            }
        }
    }
}
