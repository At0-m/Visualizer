using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reflection.Emit;

namespace Visualizer
{
    public class ProgramCFG
    {
        public static Dictionary<string, ControlFlowGraph> FunctionCFGs { get; }
            = new Dictionary<string, ControlFlowGraph>();

        public static void BuildAll(ASTNode root)
        {
            if (root is SequenceTypeNode seq)
            {
                foreach (var child in seq.elements)
                {
                    if (child is FunctionDeclNode funcDecl)
                    {
                        var builder = new CFGBuilder();
                        var cfg = builder.Build(funcDecl);

                        if (funcDecl.name is IdentNode ident)
                            FunctionCFGs[ident.val] = cfg;
                        else
                            FunctionCFGs["<anonymous>"] = cfg;
                    }
                }
            }
            else if (root is FunctionDeclNode singleFuncDecl)
            {
                var builder = new CFGBuilder();
                var cfg = builder.Build(singleFuncDecl);
                if (singleFuncDecl.name is IdentNode ident)
                    FunctionCFGs[ident.val] = cfg;
                else
                    FunctionCFGs["<anonymous>"] = cfg;
            }
        }
    }
    public class CFGNode
    {
        public int Id { get; set; }
        public List<ASTNode> Statements { get; }
        public List<CFGNode> Successors { get; }

        public CFGNode(int id)
        {
            Id = id;
            Statements = new List<ASTNode>();
            Successors = new List<CFGNode>();
        }

        public void AddStatement(ASTNode stmt)
        {
            Statements.Add(stmt);
        }

        public void AddSuccessor(CFGNode succ)
        {
            if (!Successors.Contains(succ))
                Successors.Add(succ);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"Node {Id}:\n");
            foreach (var s in Statements)
            {
                sb.Append("   " + s.Type + "\n");
            }
            return sb.ToString();
        }
    }

    public class ControlFlowGraph
    {
        private List<CFGNode> nodes;
        public IReadOnlyList<CFGNode> Nodes => nodes;
        private Dictionary<(int, int), string> edgeLabels = new();
        public ControlFlowGraph()
        {
            nodes = new List<CFGNode>();
        }

        public CFGNode CreateNode()
        {
            var node = new CFGNode(nodes.Count);
            nodes.Add(node);
            return node;
        }

        public void AddEdge(CFGNode from, CFGNode to, string label = null)
        {
            if (from == null || to == null) return;
            from.AddSuccessor(to);
            if (label != null)
                edgeLabels[(from.Id, to.Id)] = label;
        }

        public bool GetEdgeName(CFGNode from, CFGNode to, out string label)
        {
            label = null;
            if (from == null || to == null || from.Statements.Count == 0)
                return false;

            if (from.Successors.Count == 2)
            {
                bool ed = (to == from.Successors[0]);
                label = ed ? "True" : "False";
                return true;
            }
            return edgeLabels.TryGetValue((from.Id, to.Id), out label);
        }
        public void PrintCFG()
        {
            foreach (var n in nodes)
            {
                Console.WriteLine(n.ToString());
                var succIds = n.Successors.Select(s => s.Id);
                Console.WriteLine($"   Successors: {string.Join(", ", succIds)}\n");
            }
        }


        public static string BuildLabel(ASTNode node)
        {
            if (node == null)
                return "<null>";

            switch (node.Type)
            {
                //idents and consts 
                case ASTNodeType.ASTNT_IDENT_NODE:
                    {
                        var n = (IdentNode)node;
                        return n.val;
                    }

                case ASTNodeType.ASTNT_INTEGER_CONST:
                    {
                        var n = (IntConstNode)node;
                        return n.val.ToString();
                    }

                case ASTNodeType.ASTNT_STRING_CONST:
                    {
                        var n = (StringConstNode)node;
                        return n.val;
                    }

                case ASTNodeType.ASTNT_CHAR_CONST:
                    {
                        var n = (CharConstNode)node;
                        char c = (char)n.val;
                        return (c == '\n') ? "'\\n'" : $"'{c}'";
                    }

                //decls
                case ASTNodeType.ASTNT_VAR_DECL:
                    {
                        var n = (VarDeclNode)node;
                        string typeStr = BuildLabel(n.type);
                        string nameStr = BuildLabel(n.name);
                        string initStr = (n.init == NullNode.Instance) ? "" : " = " + BuildLabel(n.init);
                        return $"{typeStr} {nameStr}{initStr}";
                    }

                case ASTNodeType.ASTNT_FUNCTION_DECL:
                    {
                        var n = (FunctionDeclNode)node;

                        string typeStr = BuildLabel(n.type);
                        string nameStr = BuildLabel(n.name);
                        string paramsStr = BuildLabel(n.parametrs);
                        return $"{typeStr} {nameStr}({paramsStr})";
                    }

                case ASTNodeType.ASTNT_TYPE_DECL:
                    {
                        var n = (TypeDeclNode)node;
                        string bodyStr = BuildLabel(n.body);
                        string nameStr = BuildLabel(n.name);
                        return $"typedef {bodyStr} {nameStr}";
                    }

                case ASTNodeType.ASTNT_PARM_DECL:
                    {
                        var n = (ParametrDecNode)node;
                        string typeStr = BuildLabel(n.type);
                        string nameStr = BuildLabel(n.name);
                        return $"{typeStr} {nameStr}";
                    }

                case ASTNodeType.ASTNT_FIELD_DECL:
                    {
                        var n = (FieldDecNode)node;
                        string typeStr = BuildLabel(n.type);
                        string nameStr = BuildLabel(n.name);
                        return $"{typeStr} {nameStr}";
                    }

                //unary
                case ASTNodeType.ASTNT_NEGATE_EXPR:
                    {
                        return "-" + BuildLabel(node);
                    }

                case ASTNodeType.ASTNT_BIT_NOT_EXPR:
                    {
                        var n = (BitNotNodeExprNode)node;
                        return "~" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_LOG_NOT_EXPR:
                    {
                        var n = (LogNotExrNode)node;
                        return "!" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_PREINCREMENT_EXPR:
                    {
                        var n = (PreIncExprNode)node;
                        return "++" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_POSTINCREMENT_EXPR:
                    {
                        var n = (PostIncExprNode)node;
                        return BuildLabel(n.expr) + "++";
                    }

                case ASTNodeType.ASTNT_PREDECREMENT_EXPR:
                    {
                        var n = (PreDecExprNode)node;
                        return "--" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_POSTDECREMENT_EXPR:
                    {
                        var n = (PostDecExprNode)node;
                        return BuildLabel(n.expr) + "--";
                    }

                case ASTNodeType.ASTNT_ADDR_EXPR:
                    {
                        var n = (AddrExprNode)node;
                        return "&" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_INDIRECT_REF:
                    {
                        var n = (IndirectRefNode)node;
                        if (n.field == NullNode.Instance)
                            return "*" + BuildLabel(n.expr);
                        else
                            return BuildLabel(n.expr) + "->" + BuildLabel(n.field);
                    }

                case ASTNodeType.ASTNT_CAST_EXPR:
                    {
                        var n = (CastExprNode)node;
                        return "(" + BuildLabel(n.type) + ")" + BuildLabel(n.expr);
                    }

                case ASTNodeType.ASTNT_SIZEOF_EXPR:
                    {
                        var n = (SizeOfExprNode)node;
                        return "sizeof(" + BuildLabel(n.expr) + ")";
                    }

                case ASTNodeType.ASTNT_ALIGNOF_EXPR:
                    {
                        var n = (AlignOfExprNode)node;
                        return "_Alignof(" + BuildLabel(n.expr) + ")";
                    }

                case ASTNodeType.ASTNT_NOP_EXPR:
                    {
                        return "<nop>";
                    }

                //binary
                case ASTNodeType.ASTNT_PLUS_EXPR:
                    {
                        var n = (PlusExprNode)node;
                        return BuildLabel(n.lhs) + " + " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_MINUS_EXPR:
                    {
                        var n = (MinusExprNode)node;
                        return BuildLabel(n.lhs) + " - " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_MULT_EXPR:
                    {
                        var n = (MultExprNode)node;
                        return BuildLabel(n.lhs) + " * " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_TRUNC_DIV_EXPR:
                    {
                        var n = (TruncDivExprNode)node;
                        return BuildLabel(n.lhs) + " / " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_TRUNC_MOD_EXPR:
                    {
                        var n = (TruncModExprNode)node;
                        return BuildLabel(n.lhs) + " % " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_LSHIFT_EXPR:
                    {
                        var n = (LShiftExprNode)node;
                        return BuildLabel(n.lhs) + " << " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_RSHIFT_EXPR:
                    {
                        var n = (RShiftExprNode)node;
                        return BuildLabel(n.lhs) + " >> " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_BIT_IOR_EXPR:
                    {
                        var n = (BitOrExprNode)node;
                        return BuildLabel(n.lhs) + " | " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_BIT_XOR_EXPR:
                    {
                        var n = (BitXorExprNode)node;
                        return BuildLabel(n.lhs) + " ^ " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_BIT_AND_EXPR:
                    {
                        var n = (BitAndExprNode)node;
                        return BuildLabel(n.lhs) + " & " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_LOG_AND_EXPR:
                    {
                        var n = (LogAndExprNode)node;
                        return BuildLabel(n.lhs) + " && " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_LOG_OR_EXPR:
                    {
                        var n = (LogOrExprNode)node;
                        return BuildLabel(n.lhs) + " || " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_LT_EXPR:
                    {
                        var n = (LtExprNode)node;
                        return BuildLabel(n.lhs) + " < " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_LE_EXPR:
                    {
                        var n = (LeExprNode)node;
                        return BuildLabel(n.lhs) + " <= " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_GT_EXPR:
                    {
                        var n = (GtExprNode)node;
                        return BuildLabel(n.lhs) + " > " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_GE_EXPR:
                    {
                        var n = (GeExprNode)node;
                        return BuildLabel(n.lhs) + " >= " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_EQ_EXPR:
                    {
                        var n = (EqExprNode)node;
                        return BuildLabel(n.lhs) + " == " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_NE_EXPR:
                    {
                        var n = (NeExprNode)node;
                        return BuildLabel(n.lhs) + " != " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_ASSIGN_EXPR:
                    {
                        var n = (AssignExprNode)node;
                        return BuildLabel(n.lhs) + " = " + BuildLabel(n.rhs);
                    }

                case ASTNodeType.ASTNT_COND_EXPR:
                    {
                        var n = (CondExprNode)node;
                        return BuildLabel(n.condition) + " ? "
                             + BuildLabel(n.ifClause) + " : "
                             + BuildLabel(n.elseClause);
                    }

                case ASTNodeType.ASTNT_ARRAY_REF:
                    {
                        var n = (ArrayRefNode)node;
                        return BuildLabel(n.expr) + "[" + BuildLabel(n.index) + "]";
                    }

                case ASTNodeType.ASTNT_STRUCT_REF:
                    {
                        var n = (StructRefNode)node;
                        return BuildLabel(n.name) + "." + BuildLabel(n.member);
                    }

                case ASTNodeType.ASTNT_CALL_EXPR:
                    {
                        var n = (CallExprNode)node;
                        return BuildLabel(n.expr) + "(" + BuildLabel(n.args) + ")";
                    }

                //operatos
                case ASTNodeType.ASTNT_BREAK_STMT:
                    return "break";

                case ASTNodeType.ASTNT_CONTINUE_STMT:
                    return "continue";

                case ASTNodeType.ASTNT_GOTO_STMT:
                    {
                        var n = (GotoStmtNode)node;
                        return "goto " + BuildLabel(n.label);
                    }

                case ASTNodeType.ASTNT_LABEL_STMT:
                    {
                        var n = (LabelStmtNode)node;
                        return BuildLabel(n.label) + ": …";
                    }

                case ASTNodeType.ASTNT_IF_STMT:
                    return BuildLabel(((IfStmtNode)node).condition);

                case ASTNodeType.ASTNT_WHILE_STMT:
                    return BuildLabel(((WhileStmtNode)node).condition);

                case ASTNodeType.ASTNT_DO_STMT:
                    return BuildLabel(((DoStmtNode)node).condition);


                case ASTNodeType.ASTNT_SWITCH_STMT:
                    return BuildLabel(((SwitchStmtNode)node).expr);

                case ASTNodeType.ASTNT_CASE_LABEL:
                    return BuildLabel(((CaseLabelNode)node).expr);

                case ASTNodeType.ASTNT_RETURN_STMT:
                    {
                        var n = (ReturnStmtNode)node;
                        if (n.expr != NullNode.Instance)
                            return "return " + BuildLabel(n.expr);
                        else
                            return "return";
                    }

                case ASTNodeType.ASTNT_ASM_STMT:
                    return "asm-stmt";


                //types
                case ASTNodeType.ASTNT_VOID_TYPE:
                    return "void";

                case ASTNodeType.ASTNT_INTEGRAL_TYPE:
                    {
                        var t = (IntTypeNode)node;
                        var sb = new StringBuilder();
                        if (!t.isSigned) sb.Append("unsigned ");
                        switch (t.alignment)
                        {
                            case 1: sb.Append("char"); break;
                            case 2: sb.Append("short"); break;
                            case 4: sb.Append("int"); break;
                            case 8: sb.Append("long"); break;
                            default: sb.Append("int"); break;
                        }
                        return sb.ToString().Trim();
                    }

                case ASTNodeType.ASTNT_BOOLEAN_TYPE:
                    return "bool";

                case ASTNodeType.ASTNT_NORMAL_TYPE:
                    {
                        var t = (NormalTypeNode)node;
                        return t.isDouble ? "double" : "float";
                    }

                case ASTNodeType.ASTNT_POINTER_TYPE:
                    {
                        var n = (PointerTypeNode)node;
                        return BuildLabel(n.baseType) + "*";
                    }

                case ASTNodeType.ASTNT_REFERENCE_TYPE:
                    return "&(x)";

                case ASTNodeType.ASTNT_FUNCTION_TYPE:
                    return "func‑type";

                case ASTNodeType.ASTNT_ARRAY_TYPE:
                    {
                        var n = (ArrayTypeNode)node;
                        return BuildLabel(n.type) + "[]";
                    }


                case ASTNodeType.ASTNT_LIST:
                    {
                        var seq = (SequenceTypeNode)node;
                        var sb = new StringBuilder();
                        for (int i = 0; i < seq.elements.Count; i++)
                        {
                            if (seq.elements[i] == NullNode.Instance) continue;
                            if (i > 0) sb.Append(", ");
                            sb.Append(BuildLabel(seq.elements[i]));
                        }
                        return sb.ToString();
                    }

                case ASTNodeType.ASTNT_NONE:
                    {
                        return "";
                    }

                default:
                    return node.Type.ToString();
            }
        }

        public string ToDot(string prefix = "", bool isSubgraph = false)
        {
            string clusterName = prefix.TrimEnd('_');

            var sb = new StringBuilder();

            if (isSubgraph)
            {
                sb.AppendLine($"subgraph cluster_{clusterName} {{");
                sb.AppendLine($"   label=\"{clusterName}\";");
            }
            else
            {
                sb.AppendLine("digraph cfg {");
            }

            foreach (var node in nodes)
            {
                string label;
                if (node.Statements.Count == 1)
                {
                    var stmt = node.Statements[0];
                    label = BuildLabel(stmt);
                }
                else
                {
                    label = $"Node {node.Id}";
                }

                label = DotHelpers.Escape(label);
                string shape = "box";
                bool isCondition = false;
                bool isStartOrEnd = false;

                foreach (var stmt in node.Statements)
                {
                    switch (stmt.Type)
                    {
                        case ASTNodeType.ASTNT_IF_STMT:
                        case ASTNodeType.ASTNT_SWITCH_STMT:
                        case ASTNodeType.ASTNT_LT_EXPR:
                        case ASTNodeType.ASTNT_LE_EXPR:
                        case ASTNodeType.ASTNT_GT_EXPR:
                        case ASTNodeType.ASTNT_GE_EXPR:
                        case ASTNodeType.ASTNT_EQ_EXPR:
                        case ASTNodeType.ASTNT_NE_EXPR:
                        case ASTNodeType.ASTNT_BIT_IOR_EXPR:
                        case ASTNodeType.ASTNT_BIT_XOR_EXPR:
                        case ASTNodeType.ASTNT_BIT_AND_EXPR:
                        case ASTNodeType.ASTNT_LOG_AND_EXPR:
                        case ASTNodeType.ASTNT_LOG_OR_EXPR:
                            isCondition = true;
                            break;

                        case ASTNodeType.ASTNT_FUNCTION_DECL:
                        case ASTNodeType.ASTNT_RETURN_STMT:
                            isStartOrEnd = true;
                            break;
                    }
                }

                if (isCondition)
                    shape = "diamond";
                if (isStartOrEnd)
                    shape = "oval";

                string nodeId = prefix + node.Id;

                sb.AppendLine($"   {nodeId} [label=\"{label}\", shape={shape}];");

                if (isCondition && node.Successors.Count == 2)
                {
                    sb.AppendLine($"   {nodeId} -> {prefix}{node.Successors[0].Id} [color=\"green\", label=\"True\"];");
                    sb.AppendLine($"   {nodeId} -> {prefix}{node.Successors[1].Id} [color=\"red\", label=\"False\"];");
                }
                else
                {
                    foreach (var succ in node.Successors)
                    {
                        if (succ == null) continue;
                        string edgeA = "";
                        if (GetEdgeName(node, succ, out var lbl))
                            edgeA = $" [label=\"{DotHelpers.Escape(lbl)}\"]";
                        sb.AppendLine($"   {nodeId} -> {prefix}{succ.Id}{edgeA};");
                    }
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    public class CFGBuilder
    {
        private ControlFlowGraph cfg;
        private CFGNode currentNode;

        private Stack<CFGNode> loopConditionStack = new Stack<CFGNode>();
        private Stack<CFGNode> loopExitStack = new Stack<CFGNode>();

        private Stack<CFGNode> switchExitStack = new Stack<CFGNode>();
        private Stack<CFGNode> continueTargetStack = new();

        private Dictionary<CFGNode, string> currentSwitchCases = null;
        private readonly List<CFGNode> pendingJoins = new List<CFGNode>();

        private readonly Stack<SwitchCtx> switchStack = new();
        private CFGNode switchCFGNode;
        private bool FlagReturn = false;

        private sealed class SwitchCtx
        {
            public bool HasDefault = false;
        }

        public ControlFlowGraph Build(ASTNode root)
        {
            cfg = new ControlFlowGraph();
            currentNode = new CFGNode(0);

            Visit(root);

            return cfg;
        }

        private void FlushPending(CFGNode target)
        {
            foreach (var pred in pendingJoins)
                cfg.AddEdge(pred, target);
            pendingJoins.Clear();
        }
        private void Visit(ASTNode node)
        {
            if (node == null || node is NullNode) return;

            switch (node.Type)
            {
                case ASTNodeType.ASTNT_LIST:
                    VisitSequence((SequenceTypeNode)node);
                    break;

                case ASTNodeType.ASTNT_FUNCTION_DECL:
                    VisitFunctionDecl((FunctionDeclNode)node);
                    break;

                case ASTNodeType.ASTNT_COMPOUND_STMT:
                    VisitCompoundStmt((CompoundStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_IF_STMT:
                    VisitIfStmt((IfStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_WHILE_STMT:
                    VisitWhileStmt((WhileStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_DO_STMT:
                    VisitDoWhileStmt((DoStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_FOR_STMT:
                    VisitForStmt((ForStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_SWITCH_STMT:
                    VisitSwitchStmt((SwitchStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_ASM_STMT:
                    VisitAsmStmt(node);
                    break;

                case ASTNodeType.ASTNT_BREAK_STMT:
                    VisitBreakStmt(node);
                    break;
                case ASTNodeType.ASTNT_CONTINUE_STMT:
                    VisitContinueStmt(node);
                    break;

                case ASTNodeType.ASTNT_RETURN_STMT:
                    {
                        VisitReturnStmt(node);
                        break;
                    }

                case ASTNodeType.ASTNT_CASE_LABEL:
                    VisitCaseLabel((CaseLabelNode)node);
                    break;

                case ASTNodeType.ASTNT_LABEL_STMT:
                    VisitLabelStmt((LabelStmtNode)node);
                    break;

                case ASTNodeType.ASTNT_STRUCT_TYPE:
                case ASTNodeType.ASTNT_ENUMERAL_TYPE:
                    EmitSimpleNode(node);
                    break;

                default:
                    EmitSimpleNode(node);
                    break;
            }
        }

        private void VisitSequence(SequenceTypeNode seq)
        {
            foreach (var child in seq.elements)
            {
                Visit(child);
            }
        }

        private void VisitFunctionDecl(FunctionDeclNode funcDecl)
        {
            EmitSimpleNode(funcDecl);
            Visit(funcDecl.body);
        }

        private void VisitCompoundStmt(CompoundStmtNode compound)
        {
            Visit(compound.decls);
            Visit(compound.stmt);
        }

        private void VisitIfStmt(IfStmtNode ifNode)
        {
            var condNode = EmitSimpleNode(ifNode);   

            currentNode = null;
            int thenStart = cfg.Nodes.Count;
            Visit(ifNode.ifClause);                  

            var thenEntry = cfg.Nodes[thenStart];   
            cfg.AddEdge(condNode, thenEntry, "True");

            var thenExit = currentNode;            

            CFGNode elseExit = null;
            if (ifNode.elseClause is not NullNode)
            {
                currentNode = null;
                int elseStart = cfg.Nodes.Count;
                Visit(ifNode.elseClause);

                var elseEntry = cfg.Nodes[elseStart];
                cfg.AddEdge(condNode, elseEntry, "False");

                elseExit = currentNode;            
            }
            else
            {
                pendingJoins.Add(condNode);
            }

            if (FlagReturn && thenExit != null)
                pendingJoins.Add(thenExit);

            if (FlagReturn && elseExit != null)
                pendingJoins.Add(elseExit);

            currentNode = null;
        }

        private void VisitWhileStmt(WhileStmtNode whileNode)
        {
            var condNode = EmitSimpleNode(whileNode.condition);

            loopConditionStack.Push(condNode);
            loopExitStack.Push(null);

            currentNode = null;
            int bodyStartIdx = cfg.Nodes.Count;
            Visit(whileNode.body);
            var bodyEntry = cfg.Nodes[bodyStartIdx];

            cfg.AddEdge(condNode, bodyEntry);

            if (currentNode != null)
                cfg.AddEdge(currentNode, condNode);

            pendingJoins.Add(condNode);

            loopConditionStack.Pop();
            loopExitStack.Pop();

            currentNode = null;
            FlagReturn = false;
        }

        private void VisitDoWhileStmt(DoStmtNode doNode)
        {
            var exitNode = cfg.CreateNode();
            loopExitStack.Push(exitNode);

            var condNode = cfg.CreateNode();
            condNode.AddStatement(doNode.condition);
            loopConditionStack.Push(condNode);

            var pred = currentNode;
            if (pred != null) pendingJoins.Add(pred);
            currentNode = null;

            int bodyStartIdx = cfg.Nodes.Count;
            Visit(doNode.body);
            var bodyEntry = cfg.Nodes[bodyStartIdx];

            cfg.AddEdge(currentNode, condNode);
            cfg.AddEdge(condNode, bodyEntry);
            cfg.AddEdge(condNode, exitNode);

            loopConditionStack.Pop();
            loopExitStack.Pop();
            currentNode = exitNode;
            FlagReturn = false;
        }

        private void VisitForStmt(ForStmtNode forNode)
        {
            var forInit = EmitSimpleNode(forNode.init);

            var condNode = EmitSimpleNode(forNode.condition);
            cfg.AddEdge(forInit, condNode);

            loopConditionStack.Push(condNode);

            currentNode = condNode;
            Visit(forNode.body);

            var stepNode = EmitSimpleNode(forNode.step);

            cfg.AddEdge(stepNode, condNode);

            var exitNode = currentNode;

            loopExitStack.Push(exitNode);

            currentNode = condNode;

            loopConditionStack.Pop();
            loopExitStack.Pop();
            FlagReturn = false;
        }

        private void VisitSwitchStmt(SwitchStmtNode sw)
        {
            var condNode = EmitSimpleNode(sw);

            switchStack.Push(new SwitchCtx());

            var oldCases = currentSwitchCases;
            currentSwitchCases = new Dictionary<CFGNode, string>();

            currentNode = null;
            Visit(sw.stmt);

            foreach (var kv in currentSwitchCases)
                cfg.AddEdge(condNode, kv.Key, kv.Value);

            if (!switchStack.Peek().HasDefault)
                pendingJoins.Add(condNode);

            if (currentNode != null && !FlagReturn)
                pendingJoins.Add(currentNode);

            currentSwitchCases = oldCases;
            switchStack.Pop();
            currentNode = null;
            FlagReturn = false;
        }

        private void VisitCaseLabel(CaseLabelNode cl)
        {
            if (switchStack.Count > 0 && cl.isDefault)
                switchStack.Peek().HasDefault = true;

            string lbl = cl.isDefault ? "default" : ControlFlowGraph.BuildLabel(cl);

            int before = cfg.Nodes.Count;

            var fall = currentNode;

            currentNode = null;
            Visit(cl.stmt);

            var entry = cfg.Nodes[before];
            currentSwitchCases[entry] = lbl;

            if (fall != null)
                cfg.AddEdge(fall, entry);
        }

        private void VisitLabelStmt(LabelStmtNode labelNode)
        {
            var labelCFGNode = EmitSimpleNode(labelNode);
            Visit(labelNode.stmt);
        }

        private void VisitAsmStmt(ASTNode asmNode)
        {
            EmitSimpleNode(asmNode);
        }

        private void VisitBreakStmt(ASTNode brkAst)
        {
            var brkNode = EmitSimpleNode(brkAst);

            if (switchStack.Count > 0)
            {
                pendingJoins.Add(brkNode);
            }
            else if (loopExitStack.Count > 0)
            {
                pendingJoins.Add(brkNode);
            }

            currentNode = null;
        }

        private void VisitContinueStmt(ASTNode _)
        {
            if (currentNode == null) return;

            if (continueTargetStack.Count > 0)
                cfg.AddEdge(currentNode, continueTargetStack.Peek());
            else if (loopConditionStack.Count > 0)
                cfg.AddEdge(currentNode, loopConditionStack.Peek());

            currentNode = null;
        }
        private void VisitReturnStmt(ASTNode retAst)
        {
            var retNode = EmitSimpleNode(retAst);

            if (loopConditionStack.Count > 0)
                cfg.AddEdge(retNode, loopConditionStack.Peek());

            FlagReturn = true;

            currentNode = null;         
        }
        private CFGNode EmitSimpleNode(ASTNode stmt)
        {
            if (stmt == null || stmt is NullNode)
                return currentNode;

            var node = cfg.CreateNode();
            node.AddStatement(stmt);

            if (currentNode != null)
            {
                cfg.AddEdge(currentNode, node);
            }
            else
            {
                if (switchStack.Count == 0)
                    FlushPending(node);
            }

            currentNode = node;
            return node;
        }
    }
}