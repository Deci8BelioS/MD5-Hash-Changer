using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;

namespace MD5_Hash_Changer.UI;

public class RoundedComboBox : ComboBox
{
    private int _radius = 6;
    private Color _bgColor = Color.FromArgb(45, 45, 48);
    private Color _hoverColor = Color.FromArgb(62, 62, 66);
    private Color _pressedColor = Color.FromArgb(28, 28, 28);
    private bool _hover, _pressed;

    private GraphicsPath? _cachedPath;
    private readonly StringFormat _textFormat;

    [Category("Custom Appearance")]
    public int Radius
    {
        get => _radius;
        set {
            _radius = value;
            _cachedPath = null;
            Invalidate();
        }
    }

    [Category("Custom Appearance")]
    public new Color BackColor
    {
        get => _bgColor;
        set {
            _bgColor = value;
            Invalidate();
        }
    }

    public RoundedComboBox()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        DropDownStyle = ComboBoxStyle.DropDownList;
        FlatStyle = FlatStyle.Flat;
        Cursor = Cursors.Hand;
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 28;
        Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        ForeColor = Color.FromArgb(240, 240, 240);
        _textFormat = new StringFormat
        {
            Alignment = StringAlignment.Near,
            LineAlignment = StringAlignment.Center
        };
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        _cachedPath = null;
    }

    protected override void OnMouseEnter(EventArgs e) {
        base.OnMouseEnter(e);
        _hover = true;
        Invalidate();
    }
    protected override void OnMouseLeave(EventArgs e) {
        base.OnMouseLeave(e);
        _hover = false;
        Invalidate();
    }
    protected override void OnMouseDown(MouseEventArgs e) {
        base.OnMouseDown(e);
        _pressed = true;
        Invalidate();
    }
    protected override void OnMouseUp(MouseEventArgs e) {
        base.OnMouseUp(e);
        _pressed = false;
        Invalidate();
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        var g = e.Graphics;
        Color itemBg = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? _hoverColor : _bgColor;
        using (var b = new SolidBrush(itemBg))
            g.FillRectangle(b, e.Bounds);
        if (e.Index >= 0)
        {
            using var textBrush = new SolidBrush(ForeColor);
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            string text = Items[e.Index]?.ToString() ?? string.Empty;
            Font font = e.Font ?? this.Font;
            g.DrawString(text, font, textBrush, e.Bounds, _textFormat);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        var parentColor = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        g.Clear(parentColor);
        var backColor = _pressed ? _pressedColor : _hover ? _hoverColor : _bgColor;
        if (_cachedPath == null)
        {
            var rect = new Rectangle(1, 1, Width - 3, Height - 3);
            _cachedPath = GetRoundedPath(rect, _radius);
        }
        using (var brush = new SolidBrush(backColor))
            g.FillPath(brush, _cachedPath);
        using (var pen = new Pen(Color.FromArgb(85, 85, 85), 1f))
            g.DrawPath(pen, _cachedPath);
        var textRect = new RectangleF(12, 0, Width - 32, Height);
        using (var textBrush = new SolidBrush(ForeColor))
        {
            g.DrawString(Text, Font, textBrush, textRect, _textFormat);
            var arrowRect = new Rectangle(Width - 20, Height / 2 - 3, 12, 6);
            Point[] arrowPoints = {
                new Point(arrowRect.X + 2, arrowRect.Y),
                new Point(arrowRect.Right - 2, arrowRect.Y),
                new Point(arrowRect.X + 5, arrowRect.Bottom)
            };
            g.FillPolygon(textBrush, arrowPoints);
        }
    }

    private static GraphicsPath GetRoundedPath(Rectangle r, int radius)
    {
        int d = radius * 2;
        var p = new GraphicsPath();
        try {
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
        } catch { p.AddRectangle(r); }
        return p;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _cachedPath?.Dispose();
            _textFormat.Dispose();
        }
        base.Dispose(disposing);
    }
}