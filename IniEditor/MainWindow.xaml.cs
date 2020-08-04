using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

using IniEditor.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace IniEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ReLoadTheme();

            for (int i = 0; i < 5; i++)
            {
                Editor.Text += $"[Section{i + 1}]";
                Editor.Text += Environment.NewLine;
                Editor.Text += "Key = Value; Summary";
                Editor.Text += Environment.NewLine;
                Editor.Text += Environment.NewLine;
            }
            Editor.Document.Changed += Document_Changed;
            Editor.MouseMove += (o, e) => Method();
            Editor.KeyUp += (o, e) => Method();
            Editor.TextArea.TextEntered += TextArea_TextEntered;

            foldingManager = FoldingManager.Install(Editor.TextArea);
            foldingStrategy = new FoldingStrategy();
            Update();
        }
        private FoldingManager foldingManager;
        private FoldingStrategy foldingStrategy;
        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            Editor.ShowCompletionWindow(Editor.GetCursorWord());

            Task.Run(Update);
        }
        private void Update()
        {
            Editor.Dispatcher.Invoke(() => foldingStrategy.UpdateFoldings(foldingManager, Editor.Document));
            treeViewRoot.Dispatcher.Invoke(() =>
                treeViewRoot.ItemsSource = foldingManager.AllFoldings.Select(i =>
                {
                    var tvi = new TreeViewItem { Header = i.Title, Tag = i };
                    tvi.MouseDoubleClick += Tvi_MouseDoubleClick;
                    return tvi;
                }));
        }

        private async void Tvi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!((sender as TreeViewItem)?.Tag is FoldingSection fs)) return;

            var lineNum = Editor.Document.GetLineByOffset(Editor.CaretOffset = fs.StartOffset).LineNumber;
            var selectStart = Task.Run(() => fs.StartOffset);
            var selectEnd = Task.Run(() => fs.Title.Length + 2);

            EditorScroll.ScrollToAvalonEdit(Editor.TextArea, lineNum);
            Editor.Select(await selectStart, await selectEnd);
            FocusManager.SetFocusedElement(this, Editor);
        }

        private void Document_Changed(object sender, ICSharpCode.AvalonEdit.Document.DocumentChangeEventArgs e)
        {
            Method();
        }

        void ReLoadTheme()
        {
            using XmlTextReader reader = new XmlTextReader("IniRule.xml");
            Editor.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }

        void Method()
        {
            var line = Editor.TextArea.Document.GetLineByOffset(Editor.CaretOffset);
            tbLine.Text = Editor.TextArea.Document.GetText(line.Offset, line.Length);
        }
    }
}
