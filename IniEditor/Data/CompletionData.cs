using System;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;

namespace IniEditor.Data
{
    // 实现AvalonEdit ICompletionData接口以在“完成”下拉列表中提供条目。
    public class CompletionData : ICompletionData
    {
        public CompletionData(string text, int inputnum, string description)
        {
            this.Text = text;
            this.InputNum = inputnum;
            Description = description;
        }
        protected int InputNum;
        // 图像
        public System.Windows.Media.ImageSource Image => null;
        // 输出的文本
        public string Text { get; private set; }

        // 显示的控件
        public object Content => this.Text ;
        // 说明
        public object Description { get; private set; }
        // 优先级
        public double Priority => 1;

        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text.Substring(InputNum));
        }
    }
}
