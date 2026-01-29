using System.Collections.Generic;
using System.Linq;

namespace Visualizer
{
    public class UBAnalyzer
    {
        private readonly ControlFlowGraph cfg;

        private readonly Dictionary<CFGNode, HashSet<string>> IN = new();
        private readonly Dictionary<CFGNode, HashSet<string>> OUT = new();

        private readonly Dictionary<CFGNode, Dictionary<string, ConstState>> CONST_IN = new();
        private readonly Dictionary<CFGNode, Dictionary<string, ConstState>> CONST_OUT = new();
        private readonly Dictionary<string, int> arraySizes = new();
        private enum ConstState { Undef, NonConst, Const0, ConstNon0 }
        public record UbWarn(string Msg, int Line);

        public UBAnalyzer(ControlFlowGraph cfg) => this.cfg = cfg;
        private void Warn(CFGNode n, string msg, ASTNode src)
        {
            int ln = src?.LineNumber ?? 0;
            if (!n.Warnings.Any(w => w.Line == ln && w.Msg == msg))
                n.Warnings.Add(new UbWarn(msg, ln));
        }
        public void Run()
        {
            InitSets();
            bool changed;
            do
            {
                changed = false;
                foreach (var n in cfg.Nodes)
                {
                    var preds = cfg.Nodes.Where(p => p.Successors.Contains(n)).ToList();
                    var newIn = preds.Count == 0 ? new HashSet<string>() : preds.Select(p => OUT[p]).Aggregate((a, b) =>
                        { var inter = new HashSet<string>(a); inter.IntersectWith(b); return inter; });

                    changed |= !IN[n].SetEquals(newIn);
                    IN[n] = newIn;

                    var newConstIn = JoinConst(preds.Select(p => CONST_OUT[p]).ToList());
                    if (!DictEqual(CONST_IN[n], newConstIn))
                    {
                        CONST_IN[n] = newConstIn;
                        changed = true;
                    }

                    var outSet = new HashSet<string>(IN[n]);
                    var outConst = new Dictionary<string, ConstState>(CONST_IN[n]);

                    foreach (var stmt in n.Statements)
                    {
                        Transfer(stmt, outSet, outConst, n);
                    }

                    changed |= !OUT[n].SetEquals(outSet) || !DictEqual(CONST_OUT[n], outConst);
                    OUT[n] = outSet;
                    CONST_OUT[n] = outConst;
                }
            } while (changed);
        }

        private void InitSets()
        {
            foreach (var n in cfg.Nodes)
            {
                IN[n] = new();
                OUT[n] = new();
                CONST_IN[n] = new();
                CONST_OUT[n] = new();
            }
        }

        private static Dictionary<string, ConstState> JoinConst(List<Dictionary<string, ConstState>> maps)
        {
            if (maps.Count == 0) return new();

            var res = new Dictionary<string, ConstState>(maps.First());
            foreach (var m in maps.Skip(1))
            {
                foreach (var kv in res.Keys.ToList())
                {
                    if (!m.TryGetValue(kv, out var s) || s != res[kv])
                        res[kv] = ConstState.NonConst;
                }
            }
            return res;
        }

        private static bool DictEqual<TKey, TValue>(Dictionary<TKey, TValue> a,  Dictionary<TKey, TValue> b)
        {
            if (a.Count != b.Count) return false;
            foreach (var kv in a)
                if (!b.TryGetValue(kv.Key, out var v) || !Equals(v, kv.Value)) return false;
            return true;
        }

        private void Scan(ASTNode expr,HashSet<string> defs,Dictionary<string, ConstState> consts, CFGNode node)
        {
            if (expr == null || expr is NullNode) return;

            switch (expr.Type)
            {
                case ASTNodeType.ASTNT_VAR_DECL:
                    {
                        var v = (VarDeclNode)expr;

                        string varName = null;
                        int? arrayLen = null;
                        bool isArray = false;
                        if (v.type is ArrayTypeNode arrDecl && v.name is IdentNode baseId && arrDecl.expr is IntConstNode len)
                        {
                            varName = baseId.val;
                            arrayLen = len.val;
                            isArray = true;
                        }
                        if (v.name is IdentNode simpleId)
                        {
                            varName = simpleId.val;
                        }

                        if (varName != null && (v.init != NullNode.Instance || isArray))
                        {
                            defs.Add(varName);

                            if (v.init != NullNode.Instance) 
                                consts[varName] = ExprConst(v.init, consts, node);
                        }

                        if (isArray && arrayLen.HasValue)
                            arraySizes[varName] = arrayLen.Value;
                        
                        Scan(v.init, defs, consts, node);
                        return;
                    }
                case ASTNodeType.ASTNT_PARM_DECL:
                    {
                        var p = (ParametrDecNode)expr;
                        if (p.name is IdentNode pid)
                            defs.Add(pid.val);
                        return;
                    }
                case ASTNodeType.ASTNT_FUNCTION_DECL:
                    {
                        var f = (FunctionDeclNode)expr;
                        Scan(f.parametrs, defs, consts, node);
                        Scan(f.body, defs, consts, node);
                        return;
                    }
                case ASTNodeType.ASTNT_IDENT_NODE:
                    var id = (IdentNode)expr;
                    if (!defs.Contains(id.val))
                        Warn(node, $"UB: Using an uninitialized variable '{id.val}'", id);
                    return; 
                case ASTNodeType.ASTNT_TRUNC_DIV_EXPR:
                case ASTNodeType.ASTNT_TRUNC_MOD_EXPR:
                    dynamic d = expr;
                    Scan(d.lhs, defs, consts, node);
                    Scan(d.rhs, defs, consts, node);
                    var st = ExprConst(d.rhs, consts, node);
                    if (st == ConstState.Const0)
                        Warn(node, "UB: Dividing by 0", expr);
                    else if (st is ConstState.Undef or ConstState.NonConst)
                        Warn(node, "UB: Possible division by 0", expr);
                    return;

                case ASTNodeType.ASTNT_ARRAY_REF:
                    {
                        var ar = (ArrayRefNode)expr;
                        Scan(ar.expr, defs, consts, node);
                        Scan(ar.index, defs, consts, node);

                        if (ar.expr is IdentNode baseId && arraySizes.TryGetValue(baseId.val, out int len))
                        {
                            int? idxConst = ar.index is IntConstNode c ? c.val : ExprConst(ar.index, consts, node) == ConstState.Const0 ? 0 : null;

                            if (idxConst.HasValue)
                            {
                                if (idxConst < 0 || idxConst >= len)
                                    Warn(node, $"UB: index {idxConst} out of bounds [{len}]", ar);
                            }
                            else
                            {
                                Warn(node, $"UB: possible out-of-bounds access to '{baseId.val}[{len}]'", ar);
                            }
                        }
                        return;
                    }

                case ASTNodeType.ASTNT_ASSIGN_EXPR:
                    var asg = (AssignExprNode)expr;
                    Scan(asg.rhs, defs, consts, node);

                    if (asg.lhs is IdentNode lid)
                    {
                        defs.Add(lid.val);
                        consts[lid.val] = ExprConst(asg.rhs, consts, node);
                    }
                    else
                    {
                        Scan(asg.lhs, defs, consts, node);
                    }
                    return;
            }

            foreach (var child in expr.Children())
                Scan(child, defs, consts, node);
        }
        private void Transfer(ASTNode stmt, HashSet<string> defs,  Dictionary<string, ConstState> consts, CFGNode node)
        {
            Scan(stmt, defs, consts, node);
        }

        private ConstState ExprConst(ASTNode expr, Dictionary<string, ConstState> consts,CFGNode node)
        {
            return expr switch
            {
                IntConstNode ic when ic.val == 0 => ConstState.Const0,
                IntConstNode => ConstState.ConstNon0,
                IdentNode id when consts.TryGetValue(id.val, out var st) => st,
                _ => ConstState.NonConst
            };
        }

    }
}