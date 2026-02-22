using System.Runtime.InteropServices;

namespace MD5_Hash_Changer.UI;

public static class ThemeManager
{
    public static readonly Color C_BG = Color.FromArgb(30, 30, 30);
    public static readonly Color C_CTRL = Color.FromArgb(45, 45, 48);
    public static readonly Color C_TEXT = Color.FromArgb(240, 240, 240);
    public static readonly Color C_GRID_BG = Color.FromArgb(28, 28, 28);
    public static readonly Color C_GRID_SEL = Color.FromArgb(0, 120, 215);
    public static readonly Color C_BORDER = Color.FromArgb(70, 70, 70);

    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, uint cbAttribute);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_PRE20H1 = 19;

    public static void ApplyDarkTitleBar(IntPtr handle)
    {
        int val = 1;
        if (DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref val, sizeof(uint)) != 0)
            DwmSetWindowAttribute(handle, DWMWA_USE_IMMERSIVE_DARK_MODE_PRE20H1, ref val, sizeof(uint));
    }

    public static void ApplyDarkMode(Control parent, ContextMenuStrip? contextMenu = null)
    {
        parent.BackColor = C_BG;
        parent.ForeColor = C_TEXT;
        foreach (Control ctrl in parent.Controls)
        {
            switch (ctrl)
            {
                case DataGridView dgv:
                    dgv.Font = new Font("Segoe UI", 9f);
                    dgv.RowTemplate.Height = 26;
                    ApplyDarkDataGrid(dgv);
                    break;
                case ComboBox cmb:
                    cmb.BackColor = C_CTRL;
                    cmb.ForeColor = C_TEXT;
                    break;
                case Button:
                    break;
                case Panel panel:
                    panel.BackColor = C_BG;
                    ApplyDarkMode(panel);
                    break;
                case Label lbl:
                    lbl.BackColor = C_BG;
                    lbl.ForeColor = C_TEXT;
                    break;
                case ProgressBar pb:
                    pb.BackColor = C_CTRL;
                    break;
                default:
                    ctrl.BackColor = C_BG;
                    ctrl.ForeColor = C_TEXT;
                    if (ctrl.HasChildren) ApplyDarkMode(ctrl);
                    break;
            }
        }
        if (contextMenu != null)
        {
            contextMenu.BackColor = C_CTRL;
            contextMenu.ForeColor = C_TEXT;
            contextMenu.Renderer = new DarkMenuRenderer();
        }
    }

    public static void ApplyDarkDataGrid(DataGridView dgv)
    {
        dgv.BackgroundColor = C_GRID_BG;
        dgv.GridColor = Color.FromArgb(55, 55, 55);
        dgv.BorderStyle = BorderStyle.None;
        dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgv.RowHeadersVisible = false;
        dgv.DefaultCellStyle.BackColor = C_GRID_BG;
        dgv.DefaultCellStyle.ForeColor = C_TEXT;
        dgv.DefaultCellStyle.SelectionBackColor = C_GRID_SEL;
        dgv.DefaultCellStyle.SelectionForeColor = C_TEXT;
        dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);
        dgv.RowsDefaultCellStyle.BackColor = C_GRID_BG;
        dgv.RowsDefaultCellStyle.ForeColor = C_TEXT;
        dgv.RowsDefaultCellStyle.SelectionBackColor = C_GRID_SEL;
        dgv.RowsDefaultCellStyle.SelectionForeColor = C_TEXT;
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(36, 36, 36);
        dgv.AlternatingRowsDefaultCellStyle.ForeColor = C_TEXT;
        dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = C_GRID_SEL;
        dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = C_TEXT;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 37, 38);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(180, 180, 180);
        dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 37, 38);
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Regular);
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
        dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
        dgv.ColumnHeadersHeight = 30;
        dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgv.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
        dgv.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;
    }

    public static void RefreshButtons(Panel panel)
    {
        foreach (Control child in panel.Controls)
            if (child is RoundedButton rb) { rb.Invalidate(); rb.Update(); }
    }

    public sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
    {
        public DarkMenuRenderer() : base(new DarkColorTable()) { }
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            var color = e.Item.Selected ? Color.FromArgb(0, 120, 215) : Color.FromArgb(45, 45, 48);
            using var brush = new SolidBrush(color);
            e.Graphics.FillRectangle(brush, e.Item.ContentRectangle);
        }
    }

    private sealed class DarkColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => Color.FromArgb(0, 120, 215);
        public override Color MenuItemBorder => Color.FromArgb(70, 70, 70);
        public override Color MenuBorder => Color.FromArgb(70, 70, 70);
        public override Color ToolStripDropDownBackground => Color.FromArgb(45, 45, 48);
        public override Color ImageMarginGradientBegin => Color.FromArgb(45, 45, 48);
        public override Color ImageMarginGradientMiddle => Color.FromArgb(45, 45, 48);
        public override Color ImageMarginGradientEnd => Color.FromArgb(45, 45, 48);
    }
}