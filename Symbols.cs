using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    public enum ObjectKind
    {
        OK_CON, OK_VAR, OK_VAR_STATIC, OK_TYPE,
        OK_FUNC, OK_PAR, OK_LAB, OK_PTR
    }

    public enum TypeKind
    {
        TK_NONE, TK_CHAR, TK_SHORT, TK_INT, TK_UNSIGNED, TK_LONG,
        TK_FLOAT, TK_DOUBLE, TK_VOID, TK_ARRAY, TK_STRUCT, TK_UNION,
        TK_BOOL, TK_REAL, TK_POINTER, TK_ENUM, TK_FUNCTION
    }

    public class TKType
    {
        public TypeKind Kind { get; }
        public TKType ElType { get; set; }
        public TKType BaseType { get; set; }
        public TKType FuncType { get; set; }
        public uint NumOfFields { get; }
        public uint Len { get; }
        public SOK Fields { get; set; }
        public long Size { get; set; }
        public bool IsSigned { get; set; }

        public TKType(TypeKind kind)
        {
            Kind = kind;
            ElType = null;
            BaseType = null;
            FuncType = null;
            NumOfFields = Len = 0;
            Size = 0;
            Fields = null;
            IsSigned = false;
        }

    }

    public class SOK
    {
        public ObjectKind Kind { get; set; }
        public string Name { get; set; }
        public TKType Type { get; set; }

        public int IVal { get; set; }
        public int Scope_Level { get; set; }


        public uint Prmc { get; set; }
        public SOK Locals { get; set; } 

        public bool IsConst { get; set; }

        public SOK Next { get; set; }

        public SOK(string name, ObjectKind kind, TKType type)
        {
            Name = name;
            Kind = kind;
            Type = type;
            IVal = Scope_Level = 0;
            Prmc = 0;
            Locals = null;
            IsConst = false;
            Next = null;
        }
    }

    public class Scope
    {
        public Scope Out { get; set; }
        public SOK Locals { get; set; }
        public int NumberVar { get; set; }
        public int NumberParametrs { get; set; }
        public int Size { get; set; }

        public Scope()
        {
            Out = null; Locals = null;
            NumberVar = NumberParametrs = Size = 0;
        }
    }

    public class Symbols
    {
        private Scope CurrentScope;
        private Scope GlobalScope;
        private short IsGlobal;
        private List<Scope> ScopeP;
        private List<SOK> ObjectP;
        private List<TKType> TypeP;

        public TKType CharType { get; }
        public TKType ShortType { get; }
        public TKType IntType { get; }
        public TKType UnsignedType { get; }
        public TKType LongType { get; }
        public TKType FloatType { get; }
        public TKType DoubleType { get; }
        public TKType VoidType { get; }
        public TKType NoType { get; }
        public TKType NullType { get; }

        public SOK NoObj { get; }

        public Symbols()
        {
            ScopeP = new List<Scope>();
            ObjectP = new List<SOK>();
            TypeP = new List<TKType>();
            IsGlobal = -1;
            CurrentScope = AllocScope();
            GlobalScope = CurrentScope;

            CharType = CreateType(TypeKind.TK_CHAR, 1, true);
            ShortType = CreateType(TypeKind.TK_SHORT, 2, true);
            IntType = CreateType(TypeKind.TK_INT, 4, true);
            UnsignedType = CreateType(TypeKind.TK_UNSIGNED, 4, false);
            LongType = CreateType(TypeKind.TK_LONG, 4, true);
            FloatType = CreateType(TypeKind.TK_FLOAT, 4, false);
            DoubleType = CreateType(TypeKind.TK_DOUBLE, 8, false);
            VoidType = CreateType(TypeKind.TK_VOID, 4, false);
            NoType = AllocType(TypeKind.TK_NONE);
            Insert("NOTYPE", ObjectKind.OK_TYPE, NoType);
            NullType = AllocType(TypeKind.TK_POINTER);
            NoObj = AllocaObject("noObj", ObjectKind.OK_VAR, NoType);
            Insert("__builtin_va_list", ObjectKind.OK_TYPE, NoType);


        }

        public TKType AllocType(TypeKind kind)
        {
            var type = new TKType(kind);
            TypeP.Add(type);
            return type;
        }

        public SOK AllocaObject(string name, ObjectKind kind, TKType type)
        {
            var obj = new SOK(name, kind, type);
            ObjectP.Add(obj);
            return obj;
        }

        public Scope AllocScope()
        {
            var scope = new Scope();
            ScopeP.Add(scope);
            return scope;
        }
        public TKType CreateType(TypeKind kind, uint size, bool isSigned)
        {
            var type = AllocType(kind);
            type.Size = size;
            type.IsSigned = isSigned;
            return type;
        }

        public SOK Insert(string name, ObjectKind kind, TKType type)
        {
            var obj = AllocaObject(name, kind, type);
            obj.Scope_Level = IsGlobal;

            if (kind == ObjectKind.OK_VAR) CurrentScope.NumberVar++;
            else if (kind == ObjectKind.OK_PAR) CurrentScope.NumberParametrs++;

            SOK last = null;
            for (var p = CurrentScope.Locals; p != null; p = p.Next)
            {
                if (p.Name == name) return NoObj;
                last = p;
            }
            if (last == null) CurrentScope.Locals = obj;
            else last.Next = obj;
            return obj;
        }

        public SOK Find(string name)
        {
            for (var s = CurrentScope; s != null; s = s.Out)
            {
                for (var p = s.Locals; p != null; p = p.Next)
                    if (p.Name == name)
                        return p;

            }
            return NoObj;
        }
        public void OpenScope()
        {
            var newScope = AllocScope();
            newScope.Out = CurrentScope;
            CurrentScope = newScope;
            CurrentScope.Size = 0;
            IsGlobal++;
        }
        public void CloseScope()
        {
            CurrentScope = CurrentScope.Out;
            IsGlobal--;
        }

        public void SetCurrentScope(Scope scope) => CurrentScope = scope;
        public Scope GetCurrentScope() => CurrentScope;

        public bool IsIntegralType(TKType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type == CharType || type == IntType || type == UnsignedType ||
                   type == ShortType || type == LongType;
        }

        public bool IsNormalType(TKType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type == FloatType || type == DoubleType;
        }

        public bool IsArithmeticType(TKType type) => IsIntegralType(type) || IsNormalType(type);

        public bool IsPointerType(TKType type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            return type.Kind == TypeKind.TK_POINTER;
        }

        public bool IsScalarType(TKType type) => IsPointerType(type) || IsArithmeticType(type);

        public bool IsFunctionPointerType(TKType type)
        {
            return type.Kind == TypeKind.TK_POINTER && type.BaseType.Kind == TypeKind.TK_FUNCTION;
        }

        public bool EqualFunctionPointersTypes(TKType t1, TKType t2)
        {
            return IsFunctionPointerType(t1) && IsFunctionPointerType(t2) &&
                (t1.BaseType.FuncType == t2.BaseType.FuncType);
        }

        public bool EqualBasePointerTypes(TKType t1, TKType t2)
        {
            return t1.Kind == TypeKind.TK_POINTER && t2.Kind == TypeKind.TK_POINTER &&
                (t1.BaseType == t2.BaseType);
        }

        public bool Compatible(TKType t1, TKType t2)
        {
            if (t1 == null || t2 == null) throw new ArgumentNullException(t1 == null ? nameof(t1) : nameof(t2));

            return t1 == t2 || (IsIntegralType(t1) && IsIntegralType(t2)) ||
                   (IsIntegralType(t1) && t2.Kind == TypeKind.TK_POINTER) ||
                   (IsIntegralType(t2) && t1.Kind == TypeKind.TK_POINTER) ||
                   (t1.Kind == TypeKind.TK_POINTER && t2.BaseType == NoType) ||
                   (t2.Kind == TypeKind.TK_POINTER && t1.BaseType == NoType) ||
                   (t1.Kind == TypeKind.TK_POINTER && t2.Kind == TypeKind.TK_POINTER &&
                    Compatible(t1.BaseType, t2.BaseType));
        }

        public bool Assignable(TKType t1, TKType t2)
        {
            if (t1 == null || t2 == null) throw new ArgumentNullException(t1 == null ? nameof(t1) : nameof(t2));

            return (IsArithmeticType(t1) && IsArithmeticType(t2)) ||
                   (t1.Kind == TypeKind.TK_POINTER && t2.BaseType == NoType) ||
                   (t2.Kind == TypeKind.TK_POINTER && t1.BaseType == NoType) ||
                   (t1.Kind == TypeKind.TK_POINTER && t2.Kind == TypeKind.TK_POINTER &&
                   (t1.BaseType == t2.BaseType));
        }

        public bool Convertible(TKType t1, TKType t2) => true;
    }
}
