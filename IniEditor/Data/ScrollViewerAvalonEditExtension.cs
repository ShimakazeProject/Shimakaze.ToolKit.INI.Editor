using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;

namespace IniEditor.Data
{
    public static class ScrollViewerAvalonEditExtension
    {
        public static void ScrollToAvalonEdit(this ScrollViewer scrollViewer, TextArea textArea, int line)
        {
            scrollViewer.ScrollToAvalonEdit(textArea, line, -1);
        }
        public static void ScrollToAvalonEdit(this ScrollViewer scrollViewer, TextArea textArea, int line, int column)
        {
            scrollViewer.ScrollToAvalonEdit(textArea, line, column, VisualYPosition.LineMiddle, (scrollViewer != null) ? (scrollViewer.ViewportHeight / 2.0) : 0.0, 0.3);
        }
        public static void ScrollToAvalonEdit(this ScrollViewer scrollViewer, TextArea textArea, int line, int column, VisualYPosition yPositionMode, double referencedVerticalViewPortOffset, double minimumScrollFraction)
        {

            TextView textView = textArea.TextView;
            TextDocument document = textView.Document;
            if (scrollViewer == null || document == null)
            {
                return;
            }

            if (line < 1)
            {
                line = 1;
            }

            if (line > document.LineCount)
            {
                line = document.LineCount;
            }

            if (!((System.Windows.Controls.Primitives.IScrollInfo)textView).CanHorizontallyScroll)
            {
                VisualLine orConstructVisualLine = textView.GetOrConstructVisualLine(document.GetLineByNumber(line));
                for (double num = referencedVerticalViewPortOffset; num > 0.0; num -= orConstructVisualLine.Height)
                {
                    DocumentLine previousLine = orConstructVisualLine.FirstDocumentLine.PreviousLine;
                    if (previousLine == null)
                    {
                        break;
                    }

                    orConstructVisualLine = textView.GetOrConstructVisualLine(previousLine);
                }
            }

            Point visualPosition = textArea.TextView.GetVisualPosition(new TextViewPosition(line, Math.Max(1, column)), yPositionMode);
            double num2 = visualPosition.Y - referencedVerticalViewPortOffset;
            if (Math.Abs(num2 - scrollViewer.VerticalOffset) > minimumScrollFraction * scrollViewer.ViewportHeight)
            {
                scrollViewer.ScrollToVerticalOffset(Math.Max(0.0, num2));
            }

            if (column <= 0)
            {
                return;
            }

            if (visualPosition.X > scrollViewer.ViewportWidth - 60.0)
            {
                double num3 = Math.Max(0.0, visualPosition.X - scrollViewer.ViewportWidth / 2.0);
                if (Math.Abs(num3 - scrollViewer.HorizontalOffset) > minimumScrollFraction * scrollViewer.ViewportWidth)
                {
                    scrollViewer.ScrollToHorizontalOffset(num3);
                }
            }
            else
            {
                scrollViewer.ScrollToHorizontalOffset(0.0);
            }
        }

    }
}
