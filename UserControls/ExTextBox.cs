using System.ComponentModel;
using System.Windows.Forms;

namespace UserControls
{
    public partial class ExTextBox : TextBox
    {
        public ExTextBox()
        {
            InitializeComponent();
            this.MaxByteLength = 65535;
        }

        [Category("ユーザ―プロパティ")]
        [DefaultValue(65535)]
        [Description("エディット コントロールに入力できる最大文字バイト数を指定します。")]
        public virtual int MaxByteLength { get; set; }

        [Category("ユーザ―プロパティ")]
        [DefaultValue(false)]
        [Description("入力必須かどうかを指定します。")]
        public virtual bool IsRequired { get; set; }

        [Category("ユーザ―プロパティ")]
        [DefaultValue("")]
        [Description("入力できるパターンを指定します。")]
        public virtual string InputPattern { get; set; }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            const int WM_CHAR = 0x0102;
            const int WM_PASTE = 0x0302;

            switch (m.Msg)
            {
                case WM_CHAR:
                    KeyPressEventArgs eKeyPress = new KeyPressEventArgs((char)(m.WParam.ToInt32()));
                    this.OnChar(eKeyPress);

                    if (eKeyPress.Handled)
                    {
                        return;
                    }

                    break;
                case WM_PASTE:
                    if (this.MaxLength * 2 > this.MaxByteLength)
                    {
                        this.OnPaste(new System.EventArgs());
                        return;
                    }

                    break;
            }

            base.WndProc(ref m);
        }

        protected virtual void OnChar(System.Windows.Forms.KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }

            System.Text.Encoding sjisEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
            int textByteCount = sjisEncoding.GetByteCount(this.Text);
            int inputByteCount = sjisEncoding.GetByteCount(e.KeyChar.ToString());
            int selectedTextByteCount = sjisEncoding.GetByteCount(this.SelectedText);

            if ((textByteCount + inputByteCount - selectedTextByteCount) > this.MaxByteLength)
            {
                e.Handled = true;
            }
        }

        protected virtual void OnPaste(System.EventArgs e)
        {
            object clipboardText = Clipboard.GetDataObject().GetData(System.Windows.Forms.DataFormats.Text);

            if (clipboardText == null)
            {
                return;
            }

            System.Text.Encoding sjisEncoding = System.Text.Encoding.GetEncoding("Shift_JIS");
            string inputText = clipboardText.ToString();
            int textByteCount = sjisEncoding.GetByteCount(this.Text);
            int inputByteCount = sjisEncoding.GetByteCount(inputText);
            int selectedTextByteCount = sjisEncoding.GetByteCount(this.SelectedText);
            int remainByteCount = this.MaxByteLength - (textByteCount - selectedTextByteCount);

            if (remainByteCount <= 0)
            {
                return;
            }

            if (remainByteCount >= inputByteCount)
            {
                this.SelectedText = inputText;
            }
            else
            {
                this.SelectedText = inputText.Substring(0, remainByteCount);
            }
        }
    }
}
