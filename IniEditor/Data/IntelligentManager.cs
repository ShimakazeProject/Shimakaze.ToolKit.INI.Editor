using System.Collections.Generic;
using System.Linq;
using System.Text;

using ICSharpCode.AvalonEdit.CodeCompletion;

namespace IniEditor.Data
{
    public static class IntellisenseManager
    {
        // 关键字字典
        private static Dictionary<string, string> dictionary = new Dictionary<string, string>();

        static IntellisenseManager()
        {
            dictionary.Add("Key", "键1");
            dictionary.Add("key", "键2");
            dictionary.Add("key1", "键2");
            dictionary.Add("key2", "键2");
            dictionary.Add("key3", "键2");
        }

        public static void ShowCompletionWindow(this ICSharpCode.AvalonEdit.TextEditor editor, string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return;

            var completionWindow = editor.GetCompletionWindow();

            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            data.Clear();
            foreach (var word in dictionary.Where(word => word.Key.StartsWith(s)))
                data.Add(new CompletionData(word.Key, s.Length, word.Value));

            if (data.Count == 0) return;
            completionWindow.CompletionList.ListBox.SelectedIndex = 0;
            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
        }
        private static CompletionWindow GetCompletionWindow(this ICSharpCode.AvalonEdit.TextEditor editor)
        {
            // Open code completion after the user has pressed dot:
            var completionWindow = new CompletionWindow(editor.TextArea);

            SourceChord.FluentWPF.AcrylicWindow.SetEnabled(completionWindow, true);
            SourceChord.FluentWPF.AcrylicWindow.SetTintColor(completionWindow, System.Windows.Media.Colors.Black);
            SourceChord.FluentWPF.AcrylicWindow.SetFallbackColor(completionWindow, System.Windows.Media.Colors.Black);
            SourceChord.FluentWPF.AcrylicWindow.SetAcrylicWindowStyle(completionWindow, SourceChord.FluentWPF.AcrylicWindowStyle.None);
            SourceChord.FluentWPF.PointerTracker.SetEnabled(completionWindow, true);


            completionWindow.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);

            (completionWindow.Content as System.Windows.Controls.Control).Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);

            completionWindow.CompletionList.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Transparent);
            completionWindow.CompletionList.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            return completionWindow;
        }
    }

    public static class AvalonEditExtension
    {
        public static string GetCursorWord(this ICSharpCode.AvalonEdit.TextEditor editor)
        {
            var locate = editor.TextArea.Document.GetLocation(editor.CaretOffset);
            var line = editor.TextArea.Document.GetLineByNumber(locate.Line);
            var s = editor.TextArea.Document.GetText(line.Offset, line.Length);
            if (string.IsNullOrWhiteSpace(s)) return s;
            var separator = new char[] { ' ', '.' };
            int spacenum = (s.Substring(0, locate.Column - 1).Where(ch => separator.Contains(ch))).Count();
            var words = s.Split(separator);


            return words[spacenum];
        }
    }
}
