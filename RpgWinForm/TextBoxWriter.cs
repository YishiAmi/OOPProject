using System.IO;
using System.Text;
using System.Windows.Forms;

namespace RpgWinForm
{
    /// <summary>
    /// A TextWriter that redirects Console.Write / Console.WriteLine
    /// calls into a WinForms TextBox.  This lets the library keep using
    /// Console.WriteLine while the UI displays the messages.
    /// </summary>
    public class TextBoxWriter : TextWriter
    {
        private readonly TextBox _output;

        public TextBoxWriter(TextBox output)
        {
            _output = output;
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            AppendSafely(value.ToString());
        }

        public override void Write(string? value)
        {
            if (value != null) AppendSafely(value);
        }

        public override void WriteLine(string? value)
        {
            AppendSafely((value ?? string.Empty) + Environment.NewLine);
        }

        private void AppendSafely(string text)
        {
            if (_output.IsDisposed) return;

            if (_output.InvokeRequired)
            {
                _output.BeginInvoke(new Action(() => _output.AppendText(text)));
            }
            else
            {
                _output.AppendText(text);
            }
        }
    }
}
