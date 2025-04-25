using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    static class TypeResolver
    {
        public static TKType Resolve(ASTNode node, AST ast)
        {
            Symbols stb = ast.stb;

            if (ReferenceEquals(node, ast.charTypeNode)) return stb.CharType;
            if (ReferenceEquals(node, ast.shortTypeNode)) return stb.ShortType;
            if (ReferenceEquals(node, ast.intTypeNode)) return stb.IntType;
            if (ReferenceEquals(node, ast.unsignedTypeNode)) return stb.UnsignedType;
            if (ReferenceEquals(node, ast.longTypeNode)) return stb.LongType;
            if (ReferenceEquals(node, ast.floatTypeNode)) return stb.FloatType;
            if (ReferenceEquals(node, ast.voidTypeNode)) return stb.VoidType;

            if (node.Type == ASTNodeType.ASTNT_POINTER_TYPE)
            {
                var ptr = stb.AllocType(TypeKind.TK_POINTER);
                ptr.BaseType = Resolve(((PointerTypeNode)node).baseType, ast);
                ptr.Size = IntPtr.Size;
                return ptr;
            }

            if (node.Type == ASTNodeType.ASTNT_ARRAY_TYPE)
            {
                var arr = stb.AllocType(TypeKind.TK_ARRAY);
                arr.ElType = Resolve(((ArrayTypeNode)node).type, ast);
                arr.Size = arr.ElType.Size;
                return arr;
            }

            if (node.Type == ASTNodeType.ASTNT_STRUCT_TYPE)
                return stb.AllocType(TypeKind.TK_STRUCT);

            if (node.Type == ASTNodeType.ASTNT_UNION_TYPE)
                return stb.AllocType(TypeKind.TK_UNION);

            if (node.Type == ASTNodeType.ASTNT_ENUMERAL_TYPE)
                return stb.AllocType(TypeKind.TK_ENUM);

            return stb.NoType;
        }
    }
    public class Tools
    {
        public static ASTNode ASSIGN_NODE(ASTNode node1, ASTNode node2)
        {
            node1 = node2;
            if (node1 != NullNode.Instance)
                node1.IncRefCount();
            return node1;
        }

        public static ASTNode DELETE_REF(ASTNode node1)
        {
            if (node1 != NullNode.Instance)
            {
                node1.DecRefCount();
                if (node1.RefCount == 0)
                    node1 = null;
            }
            return node1;
        }

        public static void printTab(int n)
        {
            int i, x;
            for (i = 0; i < n * 4; i++)
                Console.Write(" ");
        }
    }
    internal static class DotHelpers
    {
        public static string Escape(string s) =>
            s
            .Replace("\\", "\\\\")   
            .Replace("\"", "\\\"")   
            .Replace("\r", "")        
            .Replace("\n", "\\l");    
    }
    public class Global
    {
        public static int BodyFunc = 0;
        public static int ind = 2;
        public static List<int> BodyIndex = [];
        public static bool isFunc = false;
        public static bool isExpr = true;
        public static bool ifBody = false;
        public static bool isExprFor = true;
        public static bool ContStr = true;
        public static bool IsTogoth = false;
        public static bool IsCase = false;
        public static bool IsCall = false;
        public static bool IsCallOp = false;
        public static string str = "";
        public static string dopstr = "";
        public static string LastStr = "";

    }
}

