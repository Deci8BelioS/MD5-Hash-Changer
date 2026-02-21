/*
 * Source code: https://github.com/ewwink/MD5-Hash-Changer
 */

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
namespace MD5_Hash_Changer;
public partial class MainForm : Form
{
    public int currentRowIndex = 0;
    public int totalFiles = 0;
    public bool running = false;
    private static readonly Color C_BG = Color.FromArgb(30,  30,  30);
    private static readonly Color C_CTRL = Color.FromArgb(45,  45,  48);
    private static readonly Color C_TEXT = Color.FromArgb(240, 240, 240);
    private static readonly Color C_GRID_BG = Color.FromArgb(28,  28,  28);
    private static readonly Color C_GRID_SEL = Color.FromArgb(0,   120, 215);
    private static readonly Color C_BORDER = Color.FromArgb(70,  70,  70);
    private ComboBox cmbLanguage = null!;
    [DllImport("dwmapi.dll")]
    private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, uint cbAttribute);
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
    private const int DWMWA_USE_IMMERSIVE_DARK_MODE_PRE20H1 = 19;
    private void ApplyDarkTitleBar()
    {
        int val = 1;
        if (DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref val, sizeof(uint)) != 0)
            DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE_PRE20H1, ref val, sizeof(uint));
    }
    public MainForm()
    {
        InitializeComponent();
        this.MinimumSize = new Size(1035, 600);
        string sysLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Locale.Set(sysLang == "en" ? AppLanguage.EN : AppLanguage.ES);
        BuildLanguageSelector();
        ApplyLocale();
        Load += (s, e) =>
        {
            ApplyDarkTitleBar();
            ApplyDarkMode(this);
            RefreshButtons(panelAction);
        };
    }
    private void BuildLanguageSelector()
    {
        cmbLanguage = new RoundedComboBox 
        { 
            Width = 52, 
            Height = 24, 
            Anchor = AnchorStyles.Right | AnchorStyles.Top 
        };
        cmbLanguage.Items.AddRange(new[] { "Es", "En" });
        cmbLanguage.SelectedIndex = 0;
        panelAction.SizeChanged += (s, e) => cmbLanguage.Location = new Point(panelAction.Width - cmbLanguage.Width - 8, (panelAction.Height - cmbLanguage.Height) / 2);
        cmbLanguage.Location = new Point(panelAction.Width - cmbLanguage.Width - 8, (panelAction.Height - cmbLanguage.Height) / 2);
        cmbLanguage.SelectedIndexChanged += (s, e) =>
        {
            var lang = cmbLanguage.SelectedItem?.ToString() == "En" ? AppLanguage.EN : AppLanguage.ES;
            Locale.Set(lang);
            ApplyLocale();
        };
        panelAction.Controls.Add(cmbLanguage);
    }
    private void ApplyLocale()
    {
        Text = Locale.WindowTitle;
        btnAddFiles.Text = Locale.BtnAddFiles;
        btnAddFolder.Text = Locale.BtnAddFolder;
        btnRemoveSelected.Text = Locale.BtnRemoveSelected;
        btnRemoveAll.Text = Locale.BtnRemoveAll;
        btnStartMD5.Text = running ? Locale.BtnStop : Locale.BtnStart;
        dataGridFileMD5.Columns[0].HeaderText = Locale.ColFileName;
        dataGridFileMD5.Columns[1].HeaderText = Locale.ColOldMD5;
        dataGridFileMD5.Columns[2].HeaderText = Locale.ColNewMD5;
        dataGridFileMD5.Columns[3].HeaderText = Locale.ColStatus;
        contextMenuCopyRows.Text = Locale.CtxCopyRows;
        contextMenuDeleteRows.Text = Locale.CtxDeleteRows;
        contextMenuOpenFile.Text = Locale.CtxOpenFile;
        contextMenuExportToCSV.Text = Locale.CtxExportCSV;
        label1.Text = Locale.LabelItem;
        label2.Text = Locale.LabelTotal;
        foreach (DataGridViewRow row in dataGridFileMD5.Rows)
        {
            var cell = row.Cells[3].Value?.ToString();
            if (cell == "en espera" || cell == "waiting") row.Cells[3].Value = Locale.StatusWaiting;
        }
    }
    private void ApplyDarkMode(Control parent)
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
        contextMenudgvMD5.BackColor = C_CTRL;
        contextMenudgvMD5.ForeColor = C_TEXT;
        contextMenudgvMD5.Renderer = new DarkMenuRenderer();
    }
    private static void ApplyDarkDataGrid(DataGridView dgv)
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
    private static void RefreshButtons(Panel panel)
    {
        foreach (Control child in panel.Controls)
            if (child is RoundedButton rb) { rb.Invalidate(); rb.Update(); }
    }
    private sealed class DarkMenuRenderer : ToolStripProfessionalRenderer
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
    private void btnRemoveAll_Click(object sender, EventArgs e)
    {
        dataGridFileMD5.Rows.Clear();
        totalFiles = 0;
        labelItem.Text = "0";
        labelTotalItem.Text = "0";
        progressBarStatus.Value = 0;
        progressBarStatus.Maximum = 1;
    }
    private void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dataGridFileMD5.SelectedRows)
            dataGridFileMD5.Rows.RemoveAt(row.Index);
        dataGridFileMD5.ClearSelection();
        totalFiles = dataGridFileMD5.RowCount;
        labelItem.Text = "0";
        labelTotalItem.Text = totalFiles.ToString();
        progressBarStatus.Maximum = totalFiles > 0 ? totalFiles : 1;
        progressBarStatus.Value = 0;
    }
    private void AddFiles(IEnumerable<string> routes)
    {
        int added = 0;
        foreach (string route in routes)
        {
            if (File.Exists(route))
            {
                dataGridFileMD5.Rows.Add(route, "", "", Locale.StatusWaiting);
                added++;
            }
            else if (Directory.Exists(route))
            {
                try
                {
                    foreach (string file in Directory.GetFiles(route, "*", SearchOption.AllDirectories))
                    {
                        dataGridFileMD5.Rows.Add(file, "", "", Locale.StatusWaiting);
                        added++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Locale.ErrDirAccess(route, ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }
        if (added > 0)
        {
            totalFiles = dataGridFileMD5.RowCount;
            labelItem.Text = "0";
            labelTotalItem.Text = totalFiles.ToString();
            progressBarStatus.Value = 0;
            progressBarStatus.Maximum = totalFiles;
        }
    }
    private void btnAddFiles_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Multiselect = true, Title = Locale.DlgSelectFiles };
        if (dlg.ShowDialog() == DialogResult.OK) AddFiles(dlg.FileNames);
    }
    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var fbd = new FolderPicker
        {
            Title = Locale.DlgFolderTitle,
            OkButtonLabel = Locale.DlgFolderOkBtn,
            FileNameLabel = Locale.DlgFolderNameLabel
        };
        if (fbd.ShowDialog(Handle) == true) AddFiles(new[] { fbd.ResultPath });
    }
    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
    }
    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        var items = (string[])e.Data!.GetData(DataFormats.FileDrop)!;
        AddFiles(items);
    }
    private void btnStartMD5_Click(object sender, EventArgs e)
    {
        if (running)
        {
            running = false;
            return;
        }
        if (dataGridFileMD5.RowCount == 0) return;
        running = true;
        totalFiles = dataGridFileMD5.RowCount;
        labelItem.Text = "0";
        labelTotalItem.Text = totalFiles.ToString();
        progressBarStatus.Value = 0;
        progressBarStatus.Maximum = totalFiles;
        btnStartMD5.Text = Locale.BtnStop;
        var names = new string[totalFiles];
        for (int i = 0; i < totalFiles; i++)
            names[i] = dataGridFileMD5.Rows[i].Cells[0].Value?.ToString() ?? "";
        new Thread(() => ChangeMD5(names)) { IsBackground = true }.Start();
    }
    private void ChangeMD5(string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (!running) break;
            this.Invoke(() => dataGridFileMD5.Rows[i].Cells[3].Value = Locale.StatusProcessing);
            try
            {
                long size = new FileInfo(names[i]).Length;
                if (size == 0)
                {
                    this.Invoke(() =>
                    {
                        labelItem.Text = (i + 1).ToString();
                        progressBarStatus.Value = Math.Min(i + 1, progressBarStatus.Maximum);
                        dataGridFileMD5.Rows[i].Cells[3].Value = Locale.StatusEmpty;
                    });
                }
                else
                {
                    string md5Old = GetMD5(names[i]);
                    using (var fs = new FileStream(names[i], FileMode.Append))
                        fs.Write(new byte[] { 0 }, 0, 1);
                    string md5New = GetMD5(names[i]);
                    this.Invoke(() =>
                    {
                        labelItem.Text = (i + 1).ToString();
                        progressBarStatus.Value = Math.Min(i + 1, progressBarStatus.Maximum);
                        dataGridFileMD5.Rows[i].Cells[1].Value = md5Old;
                        dataGridFileMD5.Rows[i].Cells[2].Value = md5New;
                        dataGridFileMD5.Rows[i].Cells[3].Value = "OK";
                    });
                }
            }
            catch (Exception ex)
            {
                this.Invoke(() =>
                {
                    labelItem.Text = (i + 1).ToString();
                    progressBarStatus.Value = Math.Min(i + 1, progressBarStatus.Maximum);
                    dataGridFileMD5.Rows[i].Cells[3].Value = "Error: " + ex.Message;
                });
            }
        }
        this.Invoke(() =>
        {
            btnStartMD5.Text = Locale.BtnStart;
            running = false;
        });
    }
    private static string GetMD5(string file)
    {
        long size = new FileInfo(file).Length;
        int  bufSize = size > 1_048_576L ? 1_048_576 : 4_096;
        using var ha = MD5.Create();
        using var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, bufSize);
        byte[] buf = new byte[bufSize];
        byte[] bufOld;
        int reads = fs.Read(buf, 0, buf.Length);
        do
        {
            bufOld = buf;
            int readOld = reads;
            buf = new byte[bufSize];
            reads = fs.Read(buf, 0, buf.Length);
            if (reads == 0) ha.TransformFinalBlock(bufOld, 0, readOld);
            else ha.TransformBlock(bufOld, 0, readOld, bufOld, 0);
        } while (reads != 0);
        return BitConverter.ToString(ha.Hash!).Replace("-", "").ToLower();
    }
    private void dataGridFileMD5_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode != Keys.Delete) return;
        foreach (DataGridViewRow row in dataGridFileMD5.SelectedRows)
            dataGridFileMD5.Rows.RemoveAt(row.Index);
        totalFiles = dataGridFileMD5.RowCount;
        labelItem.Text = "0";
        labelTotalItem.Text = totalFiles.ToString();
        progressBarStatus.Maximum = totalFiles > 0 ? totalFiles : 1;
        progressBarStatus.Value = 0;
    }
    private void dgvMD5_MouseClick(object sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right) return;
        currentRowIndex = dataGridFileMD5.HitTest(e.X, e.Y).RowIndex;
        bool hasRow = currentRowIndex > -1 && dataGridFileMD5.Rows[currentRowIndex].Selected;
        contextMenuCopyRows.Enabled = hasRow;
        contextMenuDeleteRows.Enabled = hasRow;
        contextMenuOpenFile.Enabled = hasRow;
        contextMenudgvMD5.Show(dataGridFileMD5, new Point(e.X, e.Y));
    }
    private void contextMenuCopyRow_Click(object sender, EventArgs e)
    {
        string data = "";
        for (int i = 0; i < dataGridFileMD5.RowCount; i++)
        {
            var row = dataGridFileMD5.Rows[i];
            if (row.Selected)
                data += $"{row.Cells[0].Value}\t{row.Cells[1].Value}\t{row.Cells[2].Value}\r\n";
        }
        if (!string.IsNullOrEmpty(data)) Clipboard.SetText(data);
    }
    private void contextMenuExportToCSV_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog
        {
            Filter = Locale.DlgFilterCSV,
            Title = Locale.DlgExportTitle
        };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        string data = "";
        for (int i = 0; i < dataGridFileMD5.RowCount; i++)
        {
            var row = dataGridFileMD5.Rows[i];
            data += $"{row.Cells[0].Value},{row.Cells[1].Value},{row.Cells[2].Value}\r\n";
        }
        File.WriteAllText(dlg.FileName, data);
    }
    private void contextMenuOpenFile_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = dataGridFileMD5.Rows[currentRowIndex].Cells[0].Value?.ToString(),
                UseShellExecute = true
            });
        }
        catch { }
    }
    private void deleteSelectedRowsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dataGridFileMD5.SelectedRows)
            dataGridFileMD5.Rows.RemoveAt(row.Index);
        totalFiles = dataGridFileMD5.RowCount;
        labelItem.Text = "0";
        labelTotalItem.Text = totalFiles.ToString();
        progressBarStatus.Maximum = totalFiles > 0 ? totalFiles : 1;
        progressBarStatus.Value = 0;
    }
}
