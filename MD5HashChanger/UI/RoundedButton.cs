using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.ComponentModel;

namespace MD5_Hash_Changer.UI;

public class RoundedButton : Button
{
    private int _radio = 6;
    private Color _backgroundColor = Color.FromArgb(45, 45, 48);
    private Color _colorHover = Color.FromArgb(62, 62, 66);
    private Color _colorPressed = Color.FromArgb(28, 28, 28);
    private bool _hover;
    private bool _pressed;
    private GraphicsPath? _cachedPath;
    private readonly StringFormat _textFormat;

    [Category("Custom Appearance")]
    public int Radio
    {
        get => _radio;
        set {
            _radio = value;
            _cachedPath = null;
            Invalidate();
        }
    }

    [Category("Custom Appearance")]
    public new Color BackColor
    {
        get => _backgroundColor;
        set {
            _backgroundColor = value;
            Invalidate();
        }
    }

    public RoundedButton()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
        UseVisualStyleBackColor = false;
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Color.Transparent;
        FlatAppearance.MouseDownBackColor = Color.Transparent;
        ForeColor = Color.FromArgb(240, 240, 240);
        Font = new Font("Segoe UI", 9f);
        Cursor = Cursors.Hand;
        _textFormat = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit
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

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        Color colorParent = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        g.Clear(colorParent);
        Color background = _pressed ? _colorPressed : _hover ? _colorHover : _backgroundColor;
        if (_cachedPath == null)
        {
            var rect = new Rectangle(1, 1, Width - 3, Height - 3);
            _cachedPath = CrearPath(rect, _radio);
        }

        using (var brush = new SolidBrush(background))
        {
            g.FillPath(brush, _cachedPath);
        }

        using (var pen = new Pen(Color.FromArgb(85, 85, 85), 1f))
        {
            g.DrawPath(pen, _cachedPath);
        }

        using (var brTexto = new SolidBrush(Enabled ? ForeColor : Color.Gray))
        {
            var rectTexto = new RectangleF(4, 2, Width - 8, Height - 4);
            g.DrawString(Text, Font, brTexto, rectTexto, _textFormat);
        }
    }

    private static GraphicsPath CrearPath(Rectangle r, int radio)
    {
        int d = radio * 2;
        var p = new GraphicsPath();
        try
        {
            p.AddArc(r.X, r.Y, d, d, 180, 90);
            p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            p.CloseFigure();
        }
        catch
        {
            p.AddRectangle(r);
        }
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