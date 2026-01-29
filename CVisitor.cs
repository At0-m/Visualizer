using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Visualizer
{
    public class CVisitor : Visitor
    {
        private int level;
        private int inList;
        private bool inEnum;

        public CVisitor()
        {
            level = inList = 0;
            inEnum = false;
        }
        public CVisitor(int level)
        {
            this.level = level;
            inEnum = false; inList = 0;
        }

        public override void Visit(IdentNode node)
        {

            if (Global.isFunc)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
            }
            else if (Global.IsCall)
            {
                Global.dopstr += $"{node.val}";
            }
            else if (Global.IsTogoth)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Global.IsTogoth = false;
                Global.LastStr = Global.str + $"{node.val}";
                Global.str = "";
            }
            else if (Global.ContStr)
            {
                string a = "a" + Global.ind.ToString();
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Writing.WriteStr($"{a_body} -> {a}");
                Global.LastStr = Global.str + $"{node.val}";
                Global.str = "";
            }
            else if (Global.ifBody == false)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                if (Global.isExpr)
                {
                    Writing.WriteStr($"{a} -> {a_next}");
                }
                Global.isExpr = true;
            }
            else
            {
                Writing.WriteStr(Global.LastStr);
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");

                Global.LastStr = ($"{a} -> {a_next}");
            }
        }
        public override void Visit(IntConstNode node)
        {

            if (Global.isFunc)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
            }
            else if (Global.IsCall)
            {
                Global.dopstr += $"{node.val}";
            }
            else if (Global.IsTogoth)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Global.str = "";
                Global.IsTogoth = false;

            }
            else if (Global.ContStr)
            {
                if (Global.LastStr != "")
                {
                    string a_l = "a" + (Global.ind - 1).ToString();
                    Writing.WriteStr($"{a_l}[label=" + '"' + Global.LastStr + "=" + $"{node.val}" + '"' + "];");
                    if (Global.IsCase == false) Global.LastStr = "";
                }
                else
                {
                    string a = "a" + Global.ind.ToString();
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Global.ind++;
                    Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                    Writing.WriteStr($"{a_body} -> {a}");
                    Global.str = "";
                }
            }

            else if (Global.ifBody == false)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                if (Global.isExpr)
                {
                    Writing.WriteStr($"{a} -> {a_next}");
                }
                Global.isExpr = true;
            }
            else
            {
                Writing.WriteStr(Global.LastStr);
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");

                Global.LastStr = ($"{a} -> {a_next}");
            }
        }
        public override void Visit(NormalConstNode node)
        {
            if (Global.isFunc)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
            }
            else if (Global.IsCall)
            {
                Global.dopstr += $"{node.val}";
            }
            else if (Global.IsTogoth)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Global.str = "";
                Global.IsTogoth = false;

            }
            else if (Global.ContStr)
            {
                if (Global.LastStr != "")
                {
                    string a_l = "a" + (Global.ind - 1).ToString();
                    Writing.WriteStr($"{a_l}[label=" + '"' + Global.LastStr + "=" + $"{node.val}" + '"' + "];");
                    if (Global.IsCase == false) Global.LastStr = "";
                }
                else
                {
                    string a = "a" + Global.ind.ToString();
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Global.ind++;
                    Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                    Writing.WriteStr($"{a_body} -> {a}");
                    Global.str = "";
                }
            }

            else if (Global.ifBody == false)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                if (Global.isExpr)
                {
                    Writing.WriteStr($"{a} -> {a_next}");
                }
                Global.isExpr = true;
            }
            else
            {
                Writing.WriteStr(Global.LastStr);
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");

                Global.LastStr = ($"{a} -> {a_next}");
            }
        }
        public override void Visit(StringConstNode node)
        {

            if (Global.isFunc)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
            }
            else if (Global.IsCall)
            {
                Global.dopstr += "'" + $"{node.val}" + "'";
            }
            else if (Global.IsTogoth)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Global.str = "";
                Global.IsTogoth = false;
            }
            else if (Global.ContStr)
            {

                string a = "a" + Global.ind.ToString();
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Writing.WriteStr($"{a_body} -> {a}");
                Global.str = "";
            }
            else if (Global.ifBody == false)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                if (Global.isExpr)
                {
                    Writing.WriteStr($"{a} -> {a_next}");
                }
                Global.isExpr = true;
            }
            else
            {
                Writing.WriteStr(Global.LastStr);
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                Global.LastStr = ($"{a} -> {a_next}");
            }
        }
        public override void Visit(CharConstNode node)
        {
            if (node.val == '\n')
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{"'\\n'"}" + '"' + "];");
                Writing.WriteStr($"{a} -> {a_next}");

            }
            else if (Global.IsCall)
            {
                Global.dopstr += "'" + $"{node.val}" + "'";
            }
            else if (Global.IsTogoth)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                Global.str = "";
                Global.IsTogoth = false;
            }
            else if (Global.ContStr)
            {
                if (Global.LastStr != "")
                {
                    string a_l = "a" + (Global.ind - 1).ToString();
                    Writing.WriteStr($"{a_l}[label=" + '"' + Global.LastStr + "=" + $"{node.val}" + '"' + "];");
                    Global.LastStr = "";
                }
                else
                {
                    string a = "a" + Global.ind.ToString();
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Global.ind++;
                    Writing.WriteStr($"{a}[label=" + '"' + Global.str + $"{node.val}" + '"' + "];");
                    Writing.WriteStr($"{a_body} -> {a}");
                    Global.str = "";
                }
            }
            else
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                string a_next = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a}[label=" + '"' + $"{node.val}" + '"' + "];");
                Writing.WriteStr($"{a} -> {a_next}");
            }
        }
        public override void Visit(NegateExprNode node)
        {
            node.expr.Accept(this);
        }


        public override void Visit(SizeOfExprNode node)
        {
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "sizeof " + '"' + "];");
            if (node.expr != NullNode.Instance) node.expr.Accept(this);
            string a_next = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a_next}[label=" + '"' + Global.str + '"' + "];");
            Global.str = "";
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
        }

        public override void Visit(AlignOfExprNode node)
        {
            if (node.expr != NullNode.Instance) node.expr.Accept(this);
        }

        public override void Visit(TypeDeclNode node)
        {
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.ind++;
            Writing.WriteStr($"{a}[label=" + '"' + "typedef " + $"" + '"' + "];");
            Writing.WriteStr($"{a_body} -> {a}");
            Global.str += "typedef ";
            node.body.Accept(this);
            Console.Write(" ");
            node.name.Accept(this);

            //Writing.WriteStr($"{a}[label=" + '"' + $"{node.body}" +" "+ $"{node.name}"+'"' + "];");  //с этим надо разобраться   
        }

        public override void Visit(FunctionDeclNode node)
        {
            string a_func = "a" + Global.ind.ToString();
            Global.ind++;
            Writing.WriteStr($"{a_func}[label=" + '"' + "Function" + '"' + "];");
            Global.isFunc = true;
            node.type.Accept(this);
            Global.str = "";
            string a_type = "a" + (Global.ind - 1).ToString();
            Writing.WriteStr($"{a_func} -> {a_type}" + "[label = " + '"' + "type" + '"' + "]; ");
            node.name.Accept(this);
            Global.isFunc = false;
            string a_name = "a" + (Global.ind - 1).ToString();
            Writing.WriteStr($"{a_func} -> {a_name}" + "[label = " + '"' + "name" + '"' + "]; ");
            string a_par = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a_par}[label=" + '"' + "Parameters" + '"' + "];");
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.ContStr = true;
            node.parametrs.Accept(this);
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            Writing.WriteStr($"{a_func} -> {a_par}" + "[label = " + '"' + "parameters" + '"' + "]; ");
            string a_body = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a_body}[label=" + '"' + "Body" + '"' + "];");
            Writing.WriteStr($"{a_func} -> {a_body}" + "[label = " + '"' + "body" + '"' + "]; ");
            Global.BodyFunc = Global.ind;
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            node.body.Accept(this);
        }

        public override void Visit(VarDeclNode node)
        {
            Global.str = "";
            Global.dopstr = "";
            node.type.Accept(this);
            Global.IsCall = true;
            node.name.Accept(this);

            if (node.type.Type == ASTNodeType.ASTNT_ARRAY_TYPE)
            {

                ((ArrayTypeNode)node.type).expr.Accept(this);

            }

            if (node.init != NullNode.Instance)
            {
                Global.dopstr += "=";
                node.init.Accept(this);

            }
            Global.IsCall = false;
            Global.str += Global.dopstr;
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.ind++;
            Writing.WriteStr($"{a}[label=" + '"' + Global.str + '"' + "];");
            if (Global.isExprFor) Writing.WriteStr($"{a_body} -> {a}");
            Global.str = "";
            Global.dopstr = "";
        }
        public override void Visit(ParametrDecNode node)
        {
            if (node.type != NullNode.Instance)
                node.type.Accept(this);

            if (node.name != NullNode.Instance)
                node.name.Accept(this);
        }
        public override void Visit(FieldDecNode node)
        {
            if (node.type != NullNode.Instance)
                node.type.Accept(this);

            if (node.name != NullNode.Instance)
                node.name.Accept(this);
        }
        public override void Visit(AsmStmtNode node)
        {
            //Tools.printTab(level + 1);
            //Console.WriteLine("ASTNT_ASM_STMT");
            //Tools.printTab(level + 1);
            //Console.WriteLine("Data: ");
            //Tools.printTab(level + 2);
            //Console.WriteLine(node.data);
        }

        public override void Visit(BreakStmtNode node)
        {
            string a = "a" + Global.ind.ToString();
            Global.ind++;
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "break" + '"' + "];");
            if (Global.IsCase)
            {
                Global.IsCase = false;
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            }
        }

        public override void Visit(CaseLabelNode node)
        {
            if (Global.IsCase)
            {
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            }
            level++;
            Global.IsCase = true;
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.ContStr = true;
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "case" + '"' + "];");


            if (node.expr != NullNode.Instance)
            {
                string a_ex = "a" + Global.ind.ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_ex}[label=" + '"' + "Expression" + '"' + "];");
                node.expr.Accept(this);

            }
            if (node.stmt != NullNode.Instance)
            {
                node.stmt.Accept(this);
            }
            level--;
        }
        public override void Visit(CompoundStmtNode node)
        {
            if (node.decls != NullNode.Instance) node.decls.Accept(this);

            if (node.stmt != NullNode.Instance) node.stmt.Accept(this);
        }
        public override void Visit(ContinueStmtNode node)
        {
            string a = "a" + Global.ind.ToString();
            Global.ind++;
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "continue" + '"' + "];");
        }

        public override void Visit(DoStmtNode node)
        {
            Global.ContStr = false;

            string a = "a" + Global.ind.ToString();
            string a_mainb = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_mainb} -> {a}");
            Global.ind++;
            Writing.WriteStr($"{a}[label=" + '"' + "do " + '"' + "];");
            string a_DoBody = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a} -> {a_DoBody}" + "[label = " + '"' + "do body" + '"' + "]; ");
            Writing.WriteStr($"{a_DoBody}[label=" + '"' + "Body" + '"' + "];");
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.ContStr = true;
            node.body.Accept(this);
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            string a_w = "a" + Global.ind.ToString();
            Global.ind++;
            Writing.WriteStr($"{a_w}[label=" + '"' + "while " + '"' + "];");
            string a_cond = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a_w} -> {a_cond}" + "[label = " + '"' + "condition" + '"' + "]; ");
            Global.isExpr = false;
            Global.ContStr = false;
            node.condition.Accept(this);
            Global.ContStr = true;
            Global.isExpr = true;
            Writing.WriteStr($"{a_w} -> {a_DoBody}" + "[label = " + '"' + "true" + '"' + "]; ");
            Global.ContStr = true;
        }

        public override void Visit(ForStmtNode node)
        {
            Global.ContStr = false;
            string a = "a" + Global.ind.ToString();
            string a_mainb = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_mainb} -> {a}");
            Global.ind++;
            Writing.WriteStr($"{a}[label=" + '"' + "for " + '"' + "];");
            if (node.init != NullNode.Instance)
            {
                Global.ContStr = false;
                Global.ifBody = false;
                Global.isExpr = false;
                Global.isExprFor = false;
                Global.BodyIndex.Add(Global.ind - 1);
                node.init.Accept(this);
                string a_init = "a" + (Global.ind - 1).ToString();
                Writing.WriteStr($"{a} -> {a_init}" + "[label = " + '"' + "initialization" + '"' + "]; ");
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
                Global.isExprFor = true;
                Global.ifBody = true;
                Global.isExpr = true;
            }

            if (node.condition != NullNode.Instance)
            {
                Global.ContStr = false;
                Global.isExpr = false;
                Global.ifBody = false;
                string a_cond = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_cond}" + "[label = " + '"' + "condition" + '"' + "]; ");
                node.condition.Accept(this);
                Global.ifBody = true;
                Global.isExpr = true;
            }
            if (node.step != NullNode.Instance)
            {
                Global.ContStr = false;
                Global.isExprFor = false;
                string a_step = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_step}" + "[label = " + '"' + "step" + '"' + "]; ");

                node.step.Accept(this);
                Global.isExprFor = true;
            }
            string a_body = "a" + Global.ind.ToString();
            Writing.WriteStr($"{a} -> {a_body}" + "[label = " + '"' + "body" + '"' + "]; ");
            Writing.WriteStr($"{a_body}[label=" + '"' + "Body" + '"' + "];");
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.ContStr = true;
            node.body.Accept(this);
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            Global.ContStr = true;
        }

        public override void Visit(GotoStmtNode node)
        {
            node.label.Accept(this);

        }

        public override void Visit(IfStmtNode node)
        {
            Global.ContStr = false;
            string a = "a" + Global.ind.ToString();
            string a_mainb = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_mainb} -> {a}");
            Global.ind++;
            Writing.WriteStr($"{a}[label=" + '"' + "if " + '"' + "];");
            if (node.condition != NullNode.Instance)
            {
                string a_cond = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_cond}" + "[label = " + '"' + "condition " + '"' + "]; ");
                node.condition.Accept(this);
            }
            if (node.ifClause != NullNode.Instance)
            {
                //if (node.ifClause.Type != ASTNodeType.ASTNT_COMPOUND_STMT)
                //Tools.printTab(level + 1);
                string a_body = "a" + (Global.ind).ToString();
                Writing.WriteStr($"{a} -> {a_body}" + "[label = " + '"' + "if body" + '"' + "]; ");
                Global.ifBody = true;
                Writing.WriteStr($"{a_body}[label=" + '"' + "If Body" + '"' + "];");
                Global.BodyIndex.Add(Global.ind);
                Global.ind++;
                Global.ContStr = true;
                node.ifClause.Accept(this);
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
                Global.ifBody = true;
                Global.LastStr = "";
            }

            if (node.elseClause != NullNode.Instance)
            {

                //if (node.elseClause.Type == ASTNodeType.ASTNT_IF_STMT)
                //else Console.WriteLine();

                //if (node.elseClause.Type != ASTNodeType.ASTNT_COMPOUND_STMT &&
                // node.elseClause.Type != ASTNodeType.ASTNT_IF_STMT)
                //Tools.printTab(level + 1);
                string a_elBody = "a" + (Global.ind).ToString();
                Writing.WriteStr($"{a} -> {a_elBody}" + "[label = " + '"' + "else body" + '"' + "]; ");
                Writing.WriteStr($"{a_elBody}[label=" + '"' + "Else Body" + '"' + "];");
                Global.BodyIndex.Add(Global.ind);
                Global.ind++;
                Global.ContStr = true;
                node.elseClause.Accept(this);
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            }
            Global.ContStr = true;
        }

        public override void Visit(LabelStmtNode node)
        {
            node.label.Accept(this);
            node.stmt.Accept(this);
        }

        public override void Visit(ReturnStmtNode node)
        {
            Global.str = "";
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_body} -> {a}");
            Global.str += "retrun ";
            Global.IsTogoth = true;
            node.expr.Accept(this);
            Global.IsTogoth = false;
        }

        public override void Visit(SwitchStmtNode node)
        {


            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "Switch " + '"' + "];");
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.ContStr = true;

            if (node.expr != NullNode.Instance)
            {
                string a_ex = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_ex}[label = " + '"' + "Swith Expression" + '"' + "]; ");
                Global.IsTogoth = true;
                node.expr.Accept(this);
                Global.IsTogoth = false;
            }
            if (node.stmt != NullNode.Instance)
            {
                node.stmt.Accept(this);
            }
            if (Global.IsCase)
            {
                Global.IsCase = false;
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            }
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);

        }

        public override void Visit(WhileStmtNode node)
        {
            Global.ContStr = false;
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.ind++;
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "while" + '"' + "];");
            if (node.condition != NullNode.Instance)
            {
                Global.isExpr = false;
                string a_cond = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_cond}" + "[label = " + '"' + "condition" + '"' + "]; ");
                node.condition.Accept(this);
                Global.isExpr = true;

            }
            if (node.body != NullNode.Instance)
            {
                string a_bodywh = "a" + Global.ind.ToString();
                Writing.WriteStr($"{a} -> {a_bodywh}" + "[label = " + '"' + "body" + '"' + "]; ");
                Writing.WriteStr($"{a_bodywh}[label=" + '"' + "Body" + '"' + "];");
                Global.BodyIndex.Add(Global.ind);
                Global.ind++;
                Global.ContStr = true;
                node.body.Accept(this);
                Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
            }
            Global.ContStr = true;

        }
        public override void Visit(CastExprNode node)
        {
            node.type.Accept(this);
            node.expr.Accept(this);
        }

        public override void Visit(BitNotNodeExprNode node)
        {
            if (node.expr != NullNode.Instance)
            {
                node.expr.Accept(this);
            }
        }
        public override void Visit(LogNotExrNode node)
        {
            node.expr.Accept(this);
            if (node.expr != NullNode.Instance)
            {
                node.expr.Accept(this);
            }
        }
        public override void Visit(PreDecExprNode node)
        {
            string a = "a" + (Global.ind).ToString();
            string a_next = "a" + (Global.ind + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a}[label=" + '"' + "--" + '"' + "];");
            if (Global.isExprFor)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }

            Global.isFunc = true;
            node.expr.Accept(this);
            Global.isFunc = false;
        }
        public override void Visit(PreIncExprNode node)
        {
            string a = "a" + (Global.ind).ToString();
            string a_next = "a" + (Global.ind + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            if (Global.isExprFor)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }

            Writing.WriteStr($"{a}[label=" + '"' + "++" + '"' + "];");
            Global.isFunc = true;
            node.expr.Accept(this);
            Global.isFunc = false;
        }
        public override void Visit(PostDecExprNode node)
        {

            if (node.expr != NullNode.Instance)
            {
                Global.ifBody = false;
                string a = "a" + (Global.ind).ToString();
                string a_next = "a" + (Global.ind + 1).ToString();
                node.expr.Accept(this);
                Global.ind++;
                if (Global.isExprFor) Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a_next}[label=" + '"' + "--" + '"' + "];");
            }

        }
        public override void Visit(PostIncExprNode node)
        {
            if (node.expr != NullNode.Instance)
            {
                Global.ifBody = false;
                string a = "a" + (Global.ind).ToString();
                string a_next = "a" + (Global.ind + 1).ToString();
                node.expr.Accept(this);
                Global.ind++;
                if (Global.isExprFor) Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a_next}[label=" + '"' + "++" + '"' + "];");
            }
        }
        public override void Visit(AddrExprNode node)
        {
            Global.IsTogoth = true;
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            Global.ind++;
            if (node.expr != NullNode.Instance)
            {
                Global.IsTogoth = true;
                Global.str = "&";
                node.expr.Accept(this);
                Writing.WriteStr($"{a_body} -> {a}");
                Writing.WriteStr($"{a}[label=" + '"' + Global.str + '"' + "];");
                Global.str = "";
                Global.IsTogoth = false;
            }
        }
        public override void Visit(IndirectRefNode node)
        {
            //if (node.field != NullNode.Instance) Console.Write("*");

            if (node.expr != NullNode.Instance) node.expr.Accept(this);

            if (node.field != NullNode.Instance)
            {
                //Console.Write("->");
                node.field.Accept(this);
            }
        }

        public override void Visit(NopExprNode node)
        {
        }
        public override void Visit(LShiftExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "<<" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(RShiftExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + ">>" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }

        }
        public override void Visit(BitOrExprNode node)
        {
            Global.ifBody = false;
            Global.isExpr = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "||" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(BitXorExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "^" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(BitAndExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "&&" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(LogAndExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "&" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(LogOrExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "|" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(PlusExprNode node)
        {
            Global.ContStr = false;
            Global.isExpr = false;
            Global.ifBody = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            if (Global.IsCall) Global.dopstr += "+";
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            if (Global.IsCall == false)
            {
                string a = "a" + (RemInd1).ToString();
                string a_next = "a" + (RemInd2).ToString();
                string a_last = "a" + (RemInd1 + 1).ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a} -> {a_last}");
                Writing.WriteStr($"{a}[label=" + '"' + "+" + '"' + "];");
                if (Global.IsCallOp)
                {
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Writing.WriteStr($"{a_body} -> {a}");
                }
            }
        }
        public override void Visit(MinusExprNode node)
        {
            Global.ContStr = false;
            Global.isExpr = false;
            Global.ifBody = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            if (Global.IsCall) Global.dopstr += "-";
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            if (Global.IsCall == false)
            {
                string a = "a" + (RemInd1).ToString();
                string a_next = "a" + (RemInd2).ToString();
                string a_last = "a" + (RemInd1 + 1).ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a} -> {a_last}");
                Writing.WriteStr($"{a}[label=" + '"' + "-" + '"' + "];");
                if (Global.IsCallOp)
                {
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Writing.WriteStr($"{a_body} -> {a}");
                }
            }
        }
        public override void Visit(MultExprNode node)
        {

            Global.ContStr = false;
            Global.isExpr = false;
            Global.ifBody = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            if (Global.IsCall) Global.dopstr += "*";
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            if (Global.IsCall == false)
            {
                string a = "a" + (RemInd1).ToString();
                string a_next = "a" + (RemInd2).ToString();
                string a_last = "a" + (RemInd1 + 1).ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a} -> {a_last}");
                Writing.WriteStr($"{a}[label=" + '"' + "*" + '"' + "];");
                if (Global.IsCallOp)
                {
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Writing.WriteStr($"{a_body} -> {a}");
                }
            }
        }
        public override void Visit(TruncDivExprNode node)
        {
            Global.ContStr = false;
            Global.isExpr = false;
            Global.ifBody = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            if (Global.IsCall) Global.dopstr += "/";
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            if (Global.IsCall == false)
            {
                string a = "a" + (RemInd1).ToString();
                string a_next = "a" + (RemInd2).ToString();
                string a_last = "a" + (RemInd1 + 1).ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a} -> {a_last}");
                Writing.WriteStr($"{a}[label=" + '"' + "/" + '"' + "];");
                if (Global.IsCallOp)
                {
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Writing.WriteStr($"{a_body} -> {a}");
                }
            }
        }
        public override void Visit(TruncModExprNode node)
        {
            Global.ContStr = false;
            Global.isExpr = false;
            Global.ifBody = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            if (Global.IsCall) Global.dopstr += "%";
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            if (Global.IsCall == false)
            {
                string a = "a" + (RemInd1).ToString();
                string a_next = "a" + (RemInd2).ToString();
                string a_last = "a" + (RemInd1 + 1).ToString();
                Global.ind++;
                Writing.WriteStr($"{a} -> {a_next}");
                Writing.WriteStr($"{a} -> {a_last}");
                Writing.WriteStr($"{a}[label=" + '"' + "%" + '"' + "];");
                if (Global.IsCallOp)
                {
                    string a_body = "a" + Global.BodyIndex[^1].ToString();
                    Writing.WriteStr($"{a_body} -> {a}");
                }
            }
        }
        public override void Visit(ArrayRefNode node)
        {

            string a = "a" + Global.ind.ToString();
            Global.ind++;
            string a_body = "a" + Global.BodyIndex[^1].ToString();
            if (node.expr != NullNode.Instance)
            {
                Global.IsCall = true;
                node.expr.Accept(this);
            }
            Global.dopstr += "[";
            if (node.index != NullNode.Instance)
            {
                Global.IsCall = true;
                node.index.Accept(this);
            }
            Global.IsCall = false;
            Global.dopstr += "]";
            Writing.WriteStr($"{a}[label=" + '"' + Global.dopstr + '"' + "];");
            Global.dopstr = "";
        }
        public override void Visit(StructRefNode node)
        {
            if (node.name != NullNode.Instance) node.name.Accept(this);

            //Console.Write(".");

            if (node.member != NullNode.Instance) node.member.Accept(this);

        }

        public override void Visit(LtExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "<" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(LeExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "<=" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(GtExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + ">" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }

        public override void Visit(GeExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.isExpr = false;
            Global.ContStr = false;
            Global.ifBody = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + ">=" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(EqExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.ContStr = false;
            Global.ifBody = false;
            Global.isExpr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "==" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }
        }
        public override void Visit(NeExprNode node)
        {
            Global.isExpr = false;
            Global.ifBody = false;
            Global.ContStr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.ifBody = false;
            Global.ContStr = false;
            Global.isExpr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "!=" + '"' + "];");
            if (Global.IsCallOp)
            {
                string a_body = "a" + Global.BodyIndex[^1].ToString();
                Writing.WriteStr($"{a_body} -> {a}");
            }

        }
        public override void Visit(AssignExprNode node)
        {

            Global.isExpr = false;
            int RemInd1 = Global.ind;
            Global.ind++;
            Global.ifBody = false;
            Global.ContStr = false;
            node.lhs.Accept(this);
            int RemInd2 = Global.ind;
            Global.ifBody = false;
            Global.isExpr = false;
            Global.ContStr = false;
            node.rhs.Accept(this);
            Global.ContStr = true;
            string a = "a" + (RemInd1).ToString();
            string a_body = "a" + (Global.BodyIndex[^1]).ToString();
            string a_next = "a" + (RemInd2).ToString();
            string a_last = "a" + (RemInd1 + 1).ToString();
            Global.ind++;
            if (Global.isExprFor) Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a} -> {a_next}");
            Writing.WriteStr($"{a} -> {a_last}");
            Writing.WriteStr($"{a}[label=" + '"' + "=" + '"' + "];");

        }
        public override void Visit(CondExprNode node)
        {
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + (Global.BodyIndex[^1]).ToString();
            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + "TernOp" + '"' + "];");
            node.condition.Accept(this);
            node.ifClause.Accept(this);
            node.elseClause.Accept(this);
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);
        }

        public override void Visit(CallExprNode node)
        {
            string a = "a" + Global.ind.ToString();
            string a_body = "a" + Global.BodyIndex[^1].ToString();

            Global.BodyIndex.Add(Global.ind);
            Global.ind++;
            Global.IsCall = true;
            node.expr.Accept(this);
            Global.IsCall = false;
            Writing.WriteStr($"{a_body} -> {a}");
            Writing.WriteStr($"{a}[label=" + '"' + Global.dopstr + '"' + "];");
            Global.dopstr = "";

            Global.ContStr = true;
            Global.IsCallOp = true;
            node.args.Accept(this);
            Global.IsCallOp = false;
            Global.ContStr = false;
            Global.BodyIndex.RemoveAt(Global.BodyIndex.Count - 1);

        }
        public override void Visit(VoidTypeNode node)
        {
            string a = "a" + Global.ind.ToString();
            Global.ind++;
            Global.str += "void ";
            if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "void" + '"' + "];");
        }

        public override void Visit(IntTypeNode node)
        {
            bool isSigned = false;
            if (!node.isSigned)
            {
                isSigned = true;
            }
            string a = "a" + Global.ind.ToString();
            Global.ind++;
            switch (node.alignment)
            {
                case 1:
                    if (isSigned)
                    {
                        Global.str += "unsigned char ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "unsigned char" + '"' + "];");
                    }
                    else
                    {
                        Global.str += "char ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "char" + '"' + "];");
                    }
                    break;
                case 2:
                    if (isSigned)
                    {
                        Global.str += "unsigned short ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "unsigned short " + '"' + "];");
                    }
                    else
                    {
                        Global.str += "short ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "short " + '"' + "];");
                    }
                    break;
                case 4:
                    if (isSigned)
                    {
                        Global.str += "unsigned int ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "unsigned int" + '"' + "];");
                    }
                    else
                    {
                        Global.str += "int ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "int" + '"' + "];");
                    }
                    break;
                case 8:
                    if (isSigned)
                    {
                        Global.str += "unsigned long ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "unsigned long" + '"' + "];");
                    }
                    else
                    {
                        Global.str += "long ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "long" + '"' + "];");
                    }
                    break;
                default:
                    if (isSigned)
                    {
                        Global.str += "unsigned int ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "unsigned int " + '"' + "];");
                    }
                    else
                    {
                        Global.str += "int ";
                        if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "int " + '"' + "];");
                    }
                    break;
            }
        }

        public override void Visit(NormalTypeNode node)
        {
            if (node.isDouble)
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Global.str += "double ";
                if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "double " + '"' + "];");
            }
            else
            {
                string a = "a" + Global.ind.ToString();
                Global.ind++;
                Global.str += "float ";
                if (Global.isFunc) Writing.WriteStr($"{a}[label=" + '"' + "float " + '"' + "];");
            }
        }

        public override void Visit(EnumeralTypeNode node)
        {
            //Console.Write("enum ");
            node.name.Accept(this);
            //Tools.printTab(level);
            //Console.WriteLine("\n{");
            //level++;
            //inList++;
            node.body.Accept(this);
            //inList--;
            //level--;
            //Tools.printTab(level);
            //Console.Write("}");
        }

        public override void Visit(PointerTypeNode node)
        {
            node.baseType.Accept(this);
        }

        public override void Visit(FunctionTypeNode node)
        {
            node.prms.Accept(this);
        }

        public override void Visit(ArrayTypeNode node) => node.type.Accept(this);

        public override void Visit(StructTypeNode node)
        {
            //Console.Write("struct ");
            if (node.name != NullNode.Instance) node.name.Accept(this);

            if (node.body != NullNode.Instance)
            {
                //Console.WriteLine();
                //Tools.printTab(level);
                //Console.WriteLine("{");

                //level++;
                node.body.Accept(this);
                // level--;

                //Tools.printTab(level);
                //Console.Write("}");
            }
        }
        public override void Visit(NullNode node) { }

        public static bool isNonSemi(ASTNodeType n)
        {
            return n == ASTNodeType.ASTNT_IF_STMT || n == ASTNodeType.ASTNT_WHILE_STMT || n == ASTNodeType.ASTNT_DO_STMT ||
                   n == ASTNodeType.ASTNT_FUNCTION_DECL || n == ASTNodeType.ASTNT_COMPOUND_STMT || n == ASTNodeType.ASTNT_INTEGRAL_TYPE;
        }

        public override void Visit(SequenceTypeNode node)
        {
            List<ASTNode> el = node.elements;

            foreach (var e in el)
            {
                if (e == NullNode.Instance) continue;

                e.Accept(this);

            }
        }
    }


    public static class FileWriter
    {
        private static StreamWriter writer;

        public static void Open(string path)
        {
            Close();  

            var fs = new FileStream(path,
                                    FileMode.Create,   
                                    FileAccess.Write,
                                    FileShare.Read);

            writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)) { AutoFlush = true};
        }
        
        public static void Write(string line)
        {
            if (writer == null)
                throw new InvalidOperationException("FileWriter.Open() has not been called yet");

            writer.WriteLine(line);
        }

        public static void Close()
        {
            writer?.Dispose();
            writer = null;
        }
    }

    public static class Writing
    {
        public static void WriteStr(string msg) => FileWriter.Write(msg);
    }

}