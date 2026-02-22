using System.Diagnostics;
using MD5_Hash_Changer.UI;
using MD5_Hash_Changer.Lib;
using MD5_Hash_Changer.Lang;
using System.Linq;

namespace MD5_Hash_Changer;

public partial class MainForm : Form
{
    public int currentRowIndex = 0;
    public int totalFiles = 0;
    public bool running = false;
    private ComboBox cmbLanguage = null!;

    public MainForm()
    {
        InitializeComponent();
        this.MinimumSize = new Size(1035, 600);
        SetupUI();
    }

    private void SetupUI()
    {
        string sysLang = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Locale.Set(sysLang == "en" ? AppLanguage.EN : AppLanguage.ES);
        BuildLanguageSelector();
        ApplyLocale();
        
        this.Load += (s, e) => {
            ThemeManager.ApplyDarkTitleBar(this.Handle);
            ThemeManager.ApplyDarkMode(this, contextMenudgvMD5);
            ThemeManager.RefreshButtons(panelAction);
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
        cmbLanguage.SelectedIndex = (Locale.Current == AppLanguage.EN) ? 1 : 0;
        
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

    private void UpdateProgress(int current)
    {
        labelItem.Text = current.ToString();
        progressBarStatus.Value = Math.Min(current, progressBarStatus.Maximum);
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
            labelTotalItem.Text = totalFiles.ToString();
            progressBarStatus.Maximum = totalFiles;
        }
    }

    private void btnAddFiles_Click(object sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Multiselect = true, Title = Locale.DlgSelectFiles };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            AddFiles(dlg.FileNames.OfType<string>());
        }
    }

    private void btnAddFolder_Click(object sender, EventArgs e)
    {
        var fbd = new FolderPicker
        {
            Title = Locale.DlgFolderTitle,
            OkButtonLabel = Locale.DlgFolderOkBtn,
            FileNameLabel = Locale.DlgFolderNameLabel
        };
        if (fbd.ShowDialog(Handle) == true && fbd.ResultPath != null) 
        {
            AddFiles(new string[] { fbd.ResultPath! });
        }
    }

    private void btnRemoveAll_Click(object sender, EventArgs e)
    {
        dataGridFileMD5.Rows.Clear();
        totalFiles = 0;
        labelItem.Text = "0";
        labelTotalItem.Text = "0";
        progressBarStatus.Value = 0;
    }

    private void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow row in dataGridFileMD5.SelectedRows)
            dataGridFileMD5.Rows.RemoveAt(row.Index);
        dataGridFileMD5.ClearSelection();
        totalFiles = dataGridFileMD5.RowCount;
        labelTotalItem.Text = totalFiles.ToString();
        progressBarStatus.Maximum = totalFiles > 0 ? totalFiles : 1;
    }

    private void btnStartMD5_Click(object sender, EventArgs e)
    {
        if (running) { running = false; return; }
        if (dataGridFileMD5.RowCount == 0) return;
        running = true;
        totalFiles = dataGridFileMD5.RowCount;
        progressBarStatus.Maximum = totalFiles;
        btnStartMD5.Text = Locale.BtnStop;
        var names = dataGridFileMD5.Rows.Cast<DataGridViewRow>().Select(r => r.Cells[0].Value?.ToString() ?? string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToArray();
        new Thread(() => ProcessFiles(names)) { IsBackground = true }.Start();
    }

    private void ProcessFiles(string[] names)
    {
        for (int i = 0; i < names.Length; i++)
        {
            if (!running) break;
            bool alreadyDone = false;
            this.Invoke(() => 
            {
                var currentStatus = dataGridFileMD5.Rows[i].Cells[3].Value?.ToString();
                if (currentStatus == "OK") alreadyDone = true;
            });
            if (alreadyDone) 
            {
                this.Invoke(() => UpdateProgress(i + 1));
                continue;
            }
            this.Invoke(() => dataGridFileMD5.Rows[i].Cells[3].Value = Locale.StatusProcessing);
            try
            {
                MD5Processor.ApplyHashChange(names[i], out string oldMd5, out string newMd5);
                this.Invoke(() => {
                    dataGridFileMD5.Rows[i].Cells[1].Value = oldMd5;
                    dataGridFileMD5.Rows[i].Cells[2].Value = newMd5;
                    dataGridFileMD5.Rows[i].Cells[3].Value = "OK";
                    UpdateProgress(i + 1);
                });
            }
            catch (Exception ex)
            {
                this.Invoke(() => dataGridFileMD5.Rows[i].Cells[3].Value = "Error: " + ex.Message);
            }
        }
        this.Invoke(() => { running = false; btnStartMD5.Text = Locale.BtnStart; });
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data!.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data?.GetData(DataFormats.FileDrop) is string[] items)
        {
            AddFiles(items.Where(i => i != null).Select(i => i!));
        }
    }

    private void dataGridFileMD5_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Delete) btnRemoveSelected_Click(sender, e);
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
        string data = string.Join("\r\n", dataGridFileMD5.SelectedRows.Cast<DataGridViewRow>()
            .Select(r => $"{r.Cells[0].Value}\t{r.Cells[1].Value}\t{r.Cells[2].Value}"));
        if (!string.IsNullOrEmpty(data)) Clipboard.SetText(data);
    }

    private void contextMenuExportToCSV_Click(object sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog { Filter = Locale.DlgFilterCSV, Title = Locale.DlgExportTitle };
        if (dlg.ShowDialog() != DialogResult.OK) return;
        string data = string.Join("\r\n", dataGridFileMD5.Rows.Cast<DataGridViewRow>()
            .Select(r => $"{r.Cells[0].Value},{r.Cells[1].Value},{r.Cells[2].Value}"));
        File.WriteAllText(dlg.FileName, data);
    }

    private void contextMenuOpenFile_Click(object sender, EventArgs e)
    {
        try {
            Process.Start(new ProcessStartInfo {
                FileName = dataGridFileMD5.Rows[currentRowIndex].Cells[0].Value?.ToString(),
                UseShellExecute = true
            });
        } catch { }
    }

    private void deleteSelectedRowsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        btnRemoveSelected_Click(sender, e);
    }
}