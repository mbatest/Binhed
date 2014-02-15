using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
namespace BinHed
{
    public delegate bool PreRemoveTab(int indx);
    public class TabControlEx : TabControl
    {
        public TabControlEx()
            : base()
        {
            PreRemoveTabPage = null;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }
        public event PreRemoveTab PreRemoveTabPage;
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Rectangle r = e.Bounds;
            r = GetTabRect(e.Index);
            r.Offset(2, 2);
            r.Width = 5;
            r.Height = 5;
            Brush b = new SolidBrush(Color.Black);
            Pen p = new Pen(b);
            if (this.SelectedIndex == e.Index)
            {
                e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
                e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);
            }
            string Title = this.TabPages[e.Index].Text;
            Font f = this.Font;
            e.Graphics.DrawString(Title, f, b, new PointF(r.X + 5, r.Y));
        }
        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle r = GetTabRect(i);
                r.Offset(2, 2);
                r.Width = 5;
                r.Height = 5;
                if (r.Contains(p))
                {
                    CloseTab(i);
                }
            }
        }
        private void CloseTab(int i)
        {
            if (PreRemoveTabPage != null)
            {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
            }
            base.OnMouseDown(e);
        }
    }
}
