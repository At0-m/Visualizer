using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visualizer;

namespace Visualizer
{
    public class Visitor
    {
        public virtual void Visit(NullNode node) { }
        public virtual void Visit(IdentNode node) { }
        public virtual void Visit(IntConstNode node) { }
        public virtual void Visit(NormalConstNode node) { }
        public virtual void Visit(StringConstNode node) { }
        public virtual void Visit(CharConstNode node) { }

        public virtual void Visit(SizeOfExprNode node) => node.expr.Accept(this);

        public virtual void Visit(AlignOfExprNode node) => node.expr.Accept(this);

        public virtual void Visit(TypeDeclNode node)
        {
            node.name.Accept(this);
            node.body.Accept(this);
        }

        public virtual void Visit(FunctionDeclNode node)
        {
            node.name.Accept(this);
            node.type.Accept(this);
            node.parametrs.Accept(this);
            node.body.Accept(this);
        }

        public virtual void Visit(VarDeclNode node)
        {
            node.name.Accept(this);
            node.type.Accept(this);
            node.init.Accept(this);
        }
        public virtual void Visit(ParametrDecNode node)
        {
            node.name.Accept(this);
            node.type.Accept(this);
        }
        public virtual void Visit(FieldDecNode node)
        {
            node.name.Accept(this);
            node.type.Accept(this);
        }
        public virtual void Visit(AsmStmtNode node) { }
        public virtual void Visit(BreakStmtNode node) { }

        public virtual void Visit(CaseLabelNode node)
        {
            node.expr.Accept(this);
            node.stmt.Accept(this);
        }
        public virtual void Visit(CompoundStmtNode node)
        {
            node.decls.Accept(this);
            node.stmt.Accept(this);
        }

        public virtual void Visit(ContinueStmtNode node) { }

        public virtual void Visit(DoStmtNode node)
        {
            node.condition.Accept(this);
            node.body.Accept(this);
        }

        public virtual void Visit(ForStmtNode node)
        {
            node.init.Accept(this);
            node.condition.Accept(this);
            node.body.Accept(this);
            node.step.Accept(this);
        }

        public virtual void Visit(GotoStmtNode node) { }

        public virtual void Visit(IfStmtNode node)
        {
            node.condition.Accept(this);
            node.ifClause.Accept(this);
            node.elseClause.Accept(this);
        }

        public virtual void Visit(LabelStmtNode node)
        {
            node.label.Accept(this);
            node.stmt.Accept(this);
        }

        public virtual void Visit(ReturnStmtNode node) => node.expr.Accept(this);
        public virtual void Visit(SwitchStmtNode node)
        {
            node.expr.Accept(this);
            node.stmt.Accept(this);
        }
        public virtual void Visit(WhileStmtNode node)
        {
            node.condition.Accept(this);
            node.body.Accept(this);
        }

        public virtual void Visit(CastExprNode node) => node.expr.Accept(this);
        public virtual void Visit(BitNotNodeExprNode node) => node.expr.Accept(this);
        public virtual void Visit(LogNotExrNode node) { }

        public virtual void Visit(PreDecExprNode node) => node.expr.Accept(this);
        public virtual void Visit(PreIncExprNode node) => node.expr.Accept(this);
        public virtual void Visit(PostDecExprNode node) => node.expr.Accept(this);
        public virtual void Visit(PostIncExprNode node) => node.expr.Accept(this);

        public virtual void Visit(AddrExprNode node) => node.expr.Accept(this);
        public virtual void Visit(IndirectRefNode node) { }
        public virtual void Visit(NopExprNode node) { }

        public virtual void Visit(LShiftExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(RShiftExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(BitOrExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(BitXorExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(BitAndExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(LogAndExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(LogOrExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(PlusExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(MinusExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(MultExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(TruncDivExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(TruncModExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(ArrayRefNode node) { }
        public virtual void Visit(StructRefNode node) { }

        public virtual void Visit(LtExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(LeExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(GtExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(GeExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(EqExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }
        public virtual void Visit(NeExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(AssignExprNode node)
        {
            node.lhs.Accept(this);
            node.rhs.Accept(this);
        }

        public virtual void Visit(CondExprNode node) { }

        public virtual void Visit(CallExprNode node)
        {
            node.expr.Accept(this);
            node.args.Accept(this);
        }
        public virtual void Visit(VoidTypeNode node) { }
        public virtual void Visit(IntTypeNode node) { }
        public virtual void Visit(NormalTypeNode node) { }
        public virtual void Visit(EnumeralTypeNode node)
        {
            node.name.Accept(this);
            node.body.Accept(this);
        }
        public virtual void Visit(PointerTypeNode node) => node.baseType.Accept(this);
        public virtual void Visit(FunctionTypeNode node)
        {
            node.type.Accept(this);
            node.prms.Accept(this);
        }

        public virtual void Visit(ArrayTypeNode node) { }

        public virtual void Visit(StructTypeNode node)
        {
            node.name.Accept(this);
            node.body.Accept(this);
        }

        public virtual void Visit(UnionTypeNode node) { }

        public virtual void Visit(SequenceTypeNode node)
        {
            var temp = node.elements;
            foreach (var elem in temp) elem.Accept(this);
        }

    }

}
