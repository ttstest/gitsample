using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace gitsample
{
    public partial class EntryCustomerMaster : Form
    {
        public EntryCustomerMaster()
        {
            InitializeComponent();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (((Control)sender).Name != notesText.Name)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    SelectNextControl(this.ActiveControl, !e.Shift, true, true, true);
                }
            }
        }

        private bool ValidateControl(Control sender)
        {
            bool ret = true;

            // 必須項目検証
            switch(sender)
            {
                case UserControls.ExTextBox v:
                    if (v.IsRequired)
                    {
                        if (string.IsNullOrWhiteSpace(v.Text))
                        {
                            ret = false;
                        }
                    }
                    break;
                default:
                    break;
            }

            if (!ret)
            {
                MessageBox.Show("必須項目を入力してください");
            }

            if (ret)
            {
                switch (sender)
                {
                    case UserControls.ExTextBox v:
                        if (Encoding.GetEncoding("shift-jis").GetByteCount(v.Text) > v.MaxByteLength)
                        {
                            ret = false;
                        }
                        break;
                    default:
                        break;
                }
            }

            if (!ret)
            {
                MessageBox.Show("入力可能な桁数を超えています");
            }

            // パターンマッチ検証
            if (ret)
            {
                switch (sender)
                {
                    case UserControls.ExTextBox v:
                            if (!Regex.IsMatch(v.Text, v.InputPattern))
                            {
                                ret = false;
                            }
                        break;
                    default:
                        break;
                }
            }

            //// データベース検証


            return true;
        }
    }
}
