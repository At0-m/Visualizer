using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Visualizer;
using ScintillaNET;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace Visualizer
{
    public partial class MainForm : Form
    {
        private Scintilla codeEditor;
        private Button visualizeAstButton;
        private Button visualizeCfgButton;
        private Button openFileButton;
        private Button parseButton;
        private PictureBox resultPictureBox;
        private Panel imagePanel;          
        private double _zoom = 1.0;       
        private const double ZoomStep = .10;
        private const double MinZoom = .10;
        private const double MaxZoom = 5.0;
        private const int CodeZoomStep = 1;  
        private const int CodeZoomMin = -10; 
        private const int CodeZoomMax = 20; 
        private static readonly Color ColPrimary = ColorTranslator.FromHtml("#1B2F4A"); // Navy
        private static readonly Color ColSecondary = ColorTranslator.FromHtml("#E1DED7"); // Beige
        private static readonly Font UiFont = new Font("Segoe UI", 10F, FontStyle.Regular);
        private readonly SplitContainer split = new SplitContainer();
        private double _splitRatio = 0.35;     
        private Label infoLabel;
        private Image baseImage;

        private string astDotPath = "ast.dot";
        private string cfgDotPath = "cfg.dot";

        private string astPngPath = "ast.png";
        private string cfgPngPath = "cfg.png";

        private string lastOpenedFile = "";

        private CParser? lastParser;      
        private bool isParsedOk = false;   
        private ASTNode? parsedRoot = null; 
        public MainForm()
        {
            DoubleBuffered = true;
            InitializeComponent();

            split.SplitterMoved += Split_SplitterMoved;

            Shown += (_, __) =>
            {
                InitSplitContainerSafe();
                ApplySplitRatio();
            };

            Resize += (_, __) => ApplySplitRatio();
        }
        private void InitSplitContainerSafe()
        {
            split.Panel1MinSize = 320;
            split.Panel2MinSize = 350;

            ApplySplitRatio();
        }
        private void Split_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (split.Width > 0)
                _splitRatio = (double)split.SplitterDistance / split.Width;
        }

        private void ApplySplitRatio()
        {
            if (split.Width == 0) return;  

            if (split.Width < split.Panel1MinSize + split.Panel2MinSize)
                split.Panel2MinSize = Math.Max(0, split.Width - split.Panel1MinSize);

            int desired = (int)(split.Width * _splitRatio);

            desired = Math.Max(split.Panel1MinSize, desired);
            desired = Math.Min(desired, split.Width - split.Panel2MinSize);

            split.SplitterDistance = desired;
        }

        private void InitializeComponent()
        {
            var header = new Panel
            {
                BackColor = ColPrimary,
                Dock = DockStyle.Top,
                Height = 60
            };

            var title = new Label
            {
                Text = "C Visualizer",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            };
            header.Controls.Add(title);
            openFileButton = MakeFlatButton("Open");
            parseButton = MakeFlatButton("Parse");
            visualizeAstButton = MakeFlatButton("AST");
            visualizeCfgButton = MakeFlatButton("CFG");
            var btnFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                WrapContents = false,
                Padding = new Padding(0, 10, 20, 0)
            };

            btnFlow.Controls.Add(parseButton);
            btnFlow.Controls.Add(openFileButton);
            btnFlow.Controls.SetChildIndex(parseButton, 0);
            btnFlow.Controls.SetChildIndex(openFileButton, 0);
            btnFlow.Controls.AddRange(new Control[] { visualizeAstButton, visualizeCfgButton });
            header.Controls.Add(btnFlow);

            codeEditor = new Scintilla
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 11f),
                LexerName = "cpp",
                Margin = new Padding(0),
                TabIndex = 0
            };
            codeEditor.StyleClearAll();
            codeEditor.Styles[Style.Default].Font = "Consolas";
            codeEditor.Styles[Style.Default].SizeF = 11f;
            codeEditor.Styles[Style.Default].BackColor = ColorTranslator.FromHtml("#ECE9E3"); // beige
            codeEditor.Styles[Style.Default].ForeColor = ColorTranslator.FromHtml("#313943"); // navy

            codeEditor.StyleClearAll();                

            codeEditor.Styles[Style.Cpp.Word].ForeColor = ColorTranslator.FromHtml("#005CBE"); // keywords      – bright blue
            codeEditor.Styles[Style.Cpp.Word2].ForeColor = ColorTranslator.FromHtml("#0E7C7B"); // types        – turquoise
            codeEditor.Styles[Style.Cpp.Number].ForeColor = ColorTranslator.FromHtml("#AF00DB"); // numbers     – purple
            codeEditor.Styles[Style.Cpp.String].ForeColor = ColorTranslator.FromHtml("#B84E00"); // strings     – terracotta
            codeEditor.Styles[Style.Cpp.Character].ForeColor = codeEditor.Styles[Style.Cpp.String].ForeColor;
            codeEditor.Styles[Style.Cpp.Comment].ForeColor = ColorTranslator.FromHtml("#6A9955"); // comments
            codeEditor.Styles[Style.Cpp.CommentLine].ForeColor = ColorTranslator.FromHtml("#6A9955");
            codeEditor.Styles[Style.Cpp.Preprocessor].ForeColor = ColorTranslator.FromHtml("#8B742B"); // includes

            codeEditor.Margins[0].Width = 32;
            codeEditor.Styles[Style.LineNumber].BackColor = ColorTranslator.FromHtml("#ECE9E3");
            codeEditor.Styles[Style.LineNumber].ForeColor = ColorTranslator.FromHtml("#A49C8F");

            codeEditor.SetSelectionBackColor(true, ColorTranslator.FromHtml("#D8D5CE"));
            codeEditor.CaretForeColor = ColorTranslator.FromHtml("#313943");
            codeEditor.CaretLineVisible = true;
            codeEditor.CaretLineBackColor = ColorTranslator.FromHtml("#DFDBD4");

            codeEditor.SetKeywords(0,
                "auto break case char const continue default do double else enum extern "
              + "float for goto if int long register return short signed sizeof static "
              + "struct switch typedef union unsigned void volatile while");

            codeEditor.Text = @"
#include <stdio.h>
int main() {
    int x = 0;
    if (x < 10) {
        x++;
    }
    return x;
}";
            split.Dock = DockStyle.Fill;
            split.SplitterWidth = 3;
            split.BackColor = ColPrimary;
            split.FixedPanel = FixedPanel.Panel1;

            split.Panel1.Controls.Add(codeEditor);
            split.Panel2.Controls.Add(imagePanel);


            imagePanel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White };
            resultPictureBox = new PictureBox { SizeMode = PictureBoxSizeMode.StretchImage };
            imagePanel.Controls.Add(resultPictureBox);
            imagePanel.MouseWheel += ImagePanel_MouseWheel;
            split.Panel1.Controls.Add(codeEditor);
            split.Panel2.Controls.Add(imagePanel);

            infoLabel = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = UiFont,
                Padding = new Padding(10, 0, 0, 0),
                ForeColor = ColPrimary,
                BackColor = ColSecondary,
                Text = "Enter the C‑code on the left, then click the appropriate button"
            };

            this.KeyPreview = true;            
            this.KeyDown += (s, e) =>
            {
                if (e.Control && e.KeyCode == Keys.O)
                    OpenFileButton_Click(s, EventArgs.Empty);
            };
            BackColor = ColSecondary;
            Font = UiFont;
            Text = "C Parser Visualizer (AST & CFG)";
            ClientSize = new Size(1080, 720);
            MinimumSize = new Size(880, 600);

            Controls.AddRange(new Control[] { split, header, infoLabel });



            codeEditor.MouseWheel += CodeEditor_MouseWheel;
            openFileButton.Click += OpenFileButton_Click;
            visualizeAstButton.Click += VisualizeAstButton_Click;
            visualizeCfgButton.Click += VisualizeCfgButton_Click;
            parseButton.Click += ParseButton_Click;
        }
        private Button MakeFlatButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = ColPrimary,
                BackColor = ColSecondary,
                FlatStyle = FlatStyle.Flat,
                Height = 35,
                Width = 140,
                Margin = new Padding(10, 0, 0, 0),
                TabStop = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#D1CEC7"); 
            return btn;
        }
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            int distance = Math.Max(split.Panel1MinSize, ClientSize.Width * 35 / 100);
            distance = Math.Min(distance, split.Width - split.Panel2MinSize);

            split.SplitterDistance = distance;
        }
        private void ImagePanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (resultPictureBox.Image == null) return;

            if (e.Delta > 0 && _zoom < MaxZoom) _zoom += ZoomStep;
            else if (e.Delta < 0 && _zoom > MinZoom) _zoom -= ZoomStep;

            ApplyZoom();
        }

        private void ApplyZoom()
        {
            if (baseImage == null) return;

            resultPictureBox.Width = (int)(baseImage.Width * _zoom);
            resultPictureBox.Height = (int)(baseImage.Height * _zoom);

            imagePanel.AutoScrollMinSize = resultPictureBox.Size;
            imagePanel.Invalidate();                
        }
        private void CodeEditor_MouseWheel(object? sender, MouseEventArgs e)
        {
            if ((ModifierKeys & Keys.Control) != Keys.Control) return;

            if (e.Delta > 0 && codeEditor.Zoom < CodeZoomMax)
                codeEditor.Zoom += CodeZoomStep;
            else if (e.Delta < 0 && codeEditor.Zoom > CodeZoomMin)
                codeEditor.Zoom -= CodeZoomStep;

            if (e is HandledMouseEventArgs hme)
                hme.Handled = true;
        }
        private void OpenFileButton_Click(object sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog
            {
                Title = "Open a C-file or a text file",
                Filter = "C files (*.c)|*.c|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                RestoreDirectory = true
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                codeEditor.Text = File.ReadAllText(dlg.FileName, Encoding.UTF8);
                lastOpenedFile = Path.GetFileName(dlg.FileName);

                infoLabel.Text = $"File {lastOpenedFile} loaded";
            }
            catch (Exception ex)
            {
                infoLabel.Text = "Couldn't open the file: " + ex.Message;
            }
        }
        private void VisualizeAstButton_Click(object sender, EventArgs e)
        {
            if (!isParsedOk || parsedRoot == null)
            {
                infoLabel.Text = "It is impossible to build an AST because the parsing failed or failed";
                return;
            }

            try
            {
                resultPictureBox.Image?.Dispose();
                resultPictureBox.Image = null;

                GenerateAstDot(parsedRoot, astDotPath);

                DotVisualizer.RenderDotToPng(astDotPath, astPngPath);

                ShowImage(astPngPath);

                _zoom = 1.0;
                ApplyZoom();
                infoLabel.Text = "The AST has been successfully generated";
            }
            catch (Exception ex)
            {
                infoLabel.Text = "Error AST:\n" + ex;
            }
        }

        private void VisualizeCfgButton_Click(object sender, EventArgs e)
        {
            if (!isParsedOk || parsedRoot == null)
            {
                infoLabel.Text = "It is impossible to build a CFG because the parsing is unsuccessful";
                return;
            }
            try
            {
                resultPictureBox.Image?.Dispose();
                resultPictureBox.Image = null;

                GenerateCfgDot(parsedRoot, cfgDotPath);
                DotVisualizer.RenderDotToPng(cfgDotPath, cfgPngPath);

                ShowImage(cfgPngPath);

                _zoom = 1.0;
                ApplyZoom();
                infoLabel.Text = "The CFG has been successfully generated";
            }
            catch (Exception ex)
            {
                infoLabel.Text = "Error CFG:\n" + ex;
            }
        }

        private void ParseButton_Click(object sender, EventArgs e)
        {
            var code = codeEditor.Text;
            File.WriteAllText("temp_input.c", code, Encoding.UTF8);

            lastParser?.Dispose();
            lastParser = null;

            using var lexer = new CLexer("temp_input.c");
            var parser = new CParser(lexer);   
            parser.parse("outFileIrrelevant");

            if (parser.HasErrors)
            {
                isParsedOk = false;
                parsedRoot = null;

                infoLabel.Text = "Parsing error:\n" + parser.ErrorLog.ToString();
            }
            else
            {
                isParsedOk = true;
                parsedRoot = parser.ast.root;
                infoLabel.Text = "Parsing completed successfully";
            }

            lastParser = parser;  
        }
        private void GenerateAstDot(ASTNode root, string dotFilePath)
        {

            FileWriter.Open(dotFilePath);
            FileWriter.Write("digraph AST {");

            var visitor = new CVisitor();
            visitor.Visit((SequenceTypeNode)root);

            FileWriter.Write("}");
            FileWriter.Close();
        }

        private void GenerateCfgDot(ASTNode root, string dotFilePath)
        {
            ProgramCFG.BuildAll(root);

            var sb = new StringBuilder();
            sb.AppendLine("digraph all_functions {");

            foreach (var (funcName, cfg) in ProgramCFG.FunctionCFGs)
                sb.AppendLine(cfg.ToDot(funcName + "_", true));

            sb.AppendLine("}");

            FileWriter.Open(dotFilePath);
            FileWriter.Write(sb.ToString());
            FileWriter.Close();
        }
        private void ShowImage(string path)
        {
            baseImage?.Dispose();
            resultPictureBox.Image?.Dispose();

            using var bmpTemp = new Bitmap(path);
            baseImage = new Bitmap(bmpTemp);    
            resultPictureBox.Image = baseImage;

            _zoom = 1.0;
            ApplyZoom();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            baseImage?.Dispose();
        }
    }
}