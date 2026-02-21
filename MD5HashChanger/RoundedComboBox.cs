using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace MD5_Hash_Changer;
public class RoundedComboBox : ComboBox
{
    private int _radius = 6;
    private Color _bgColor = Color.FromArgb(45, 45, 48);
    private Color _hoverColor = Color.FromArgb(62, 62, 66);
    private Color _pressedColor = Color.FromArgb(28, 28, 28);
    private bool _hover, _pressed;

    public int Radius
    {
        get => _radius;
        set {
            _radius = value;
            Invalidate();
        }
    }

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
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        DropDownStyle = ComboBoxStyle.DropDownList;
        FlatStyle = FlatStyle.Flat;
        Cursor = Cursors.Hand;
        DrawMode = DrawMode.OwnerDrawFixed;
        ItemHeight = 28;
        Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        ForeColor = Color.FromArgb(240, 240, 240);
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
        var parentColor = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        e.Graphics.Clear(parentColor);
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        e.DrawBackground();
        e.DrawFocusRectangle();
        if (e.Index >= 0)
        {
            var fmt = new StringFormat {
                Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center
            };
            e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, fmt);
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var parentColor = Parent?.BackColor ?? Color.FromArgb(30, 30, 30);
        var backColor = _pressed ? _pressedColor : _hover ? _hoverColor : _bgColor;
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        e.Graphics.Clear(parentColor);
        var rect = new Rectangle(1, 1, Width - 3, Height - 3);
        using var path = GetRoundedPath(rect, _radius);
        using var brush = new SolidBrush(backColor);
        using var pen = new Pen(Color.FromArgb(85, 85, 85), 1f);
        e.Graphics.FillPath(brush, path);
        e.Graphics.DrawPath(pen, path);
        var textRect = new RectangleF(12, 0, Width - 32, Height);
        var fmt = new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };
        using var textBrush = new SolidBrush(ForeColor);
        e.Graphics.DrawString(Text, Font, textBrush, textRect, fmt);
        var arrowRect = new Rectangle(Width - 20, Height / 2 - 3, 12, 6);
        using var arrowPath = new GraphicsPath();
        arrowPath.AddPolygon(new Point[] {
            new Point(arrowRect.X + 2, arrowRect.Y),
            new Point(arrowRect.Right - 2, arrowRect.Y),
            new Point(arrowRect.X + 5, arrowRect.Bottom)
        });
        e.Graphics.FillPath(textBrush, arrowPath);
    }

    private static GraphicsPath GetRoundedPath(Rectangle r, int radius)
    {
        int d = radius * 2;
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }
}