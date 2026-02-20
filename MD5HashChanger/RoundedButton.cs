using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace MD5_Hash_Changer;

public class RoundedButton : Button
{
    private int _radio = 6;
    private Color _backgroundColor = Color.FromArgb(45, 45, 48);
    private Color _colorHover = Color.FromArgb(62, 62, 66);
    private Color _colorPressed = Color.FromArgb(28, 28, 28);
    private bool _hover;
    private bool _pressed;
    public int Radio
    {
        get => _radio;
        set { 
            _radio = value;
            Invalidate();
        }
    }

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
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        UseVisualStyleBackColor = false;
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Color.Transparent;
        FlatAppearance.MouseDownBackColor = Color.Transparent;
        ForeColor = Color.FromArgb(240, 240, 240);
        Font = new Font("Segoe UI", 9f);
        Cursor = Cursors.Hand;
    }

    protected override void OnMouseEnter(EventArgs e)
    { 
        base.OnMouseEnter(e); 
        _hover = true; 
        Invalidate(); 
    }

    protected override void OnMouseLeave(EventArgs e)
    { 
        base.OnMouseLeave(e); 
        _hover = false;
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e); 
        _pressed = true; 
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    { 
        base.OnMouseUp(e); 
        _pressed = false;
        Invalidate(); 
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        Color colorParent = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        e.Graphics.Clear(colorParent);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Color colorParent = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        Color background = _pressed ? _colorPressed : _hover ? _colorHover : _backgroundColor;
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        g.Clear(colorParent);
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = CrearPath(rect, _radio);
        using var brush = new SolidBrush(background);
        g.FillPath(brush, path);
        using var pen = new Pen(Color.FromArgb(85, 85, 85), 1f);
        g.DrawPath(pen, path);
        var fmt = new StringFormat
        {
            Alignment     = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
            FormatFlags   = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit
        };
        using var brTexto = new SolidBrush(ForeColor);
        var rectTexto = new RectangleF(4, 2, Width - 8, Height - 4);
        g.DrawString(Text, Font, brTexto, rectTexto, fmt);
    }

    private static GraphicsPath CrearPath(Rectangle r, int radio)
    {
        int d = radio * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}
