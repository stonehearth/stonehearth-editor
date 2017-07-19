using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Color = System.Drawing.Color;
using DrawingNode = Microsoft.Msagl.Drawing.Node;

namespace StonehearthEditor
{
    internal class EncounterNodeRenderer
    {
        public static readonly int kIconSize = 24;

        public static void SetupNodeRendering(DrawingNode node)
        {
            System.Diagnostics.Debug.Assert(node.Attr.Shape == Shape.Box, "Only box nodes are supported");
            node.DrawNodeDelegate = new DelegateToOverrideNodeRendering(DrawNode);
            node.NodeBoundaryDelegate = new DelegateToSetNodeBoundary(GetNodeBoundary);
        }

        // Based on https://github.com/Microsoft/automatic-graph-layout/blob/master/GraphLayout/tools/GraphViewerGDI/Draw.cs
        // MIT License
        private static bool DrawNode(DrawingNode node, object graphics)
        {
            Graphics g = (Graphics)graphics;

            DrawBox(g, node);
            DrawLabel(g, node);
            DrawIcon(g, node);

            return true;  // Returning false would enable the default rendering.
        }

        private static void DrawIcon(Graphics g, DrawingNode node)
        {
            Image image = node.UserData as Image;
            if (image == null)
            {
                return;
            }

            // Flip the image around its center.
            var m = g.Transform;
            var saveM = m.Clone();
            m.Multiply(new System.Drawing.Drawing2D.Matrix(1, 0, 0, -1, 0, 2 * (float)node.GeometryNode.Center.Y));

            g.Transform = m;

            var x = (float)(node.GeometryNode.Center.X - (node.GeometryNode.Width / 2) + (node.Attr.LabelMargin / 2));
            var y = (float)(node.GeometryNode.Center.Y - (kIconSize / 2));
            g.DrawImage(image, x, y, kIconSize, kIconSize);

            g.Transform = saveM;
        }

        private static void DrawLabel(Graphics g, DrawingNode node)
        {
            var label = node.Label;
            var brush = new SolidBrush(Draw.MsaglColorToDrawingColor(label.FontColor));
            var font = new Font(label.FontName, (float)label.FontSize, (System.Drawing.FontStyle)(int)label.FontStyle);
            var bbox = node.GeometryNode.BoundingBox;
            var rect = new RectangleF((float)bbox.Left, (float)bbox.Bottom - node.Attr.LabelMargin, (float)bbox.Width, (float)bbox.Height);
            if (node.UserData is Image)
            {
                rect.Offset(kIconSize, 0);
                var size = rect.Size;
                size.Width -= kIconSize;
                rect.Size = size;
            }

            DrawStringInRectCenter(g, brush, font, label.Text, rect);
        }

        private static void DrawStringInRectCenter(Graphics g, Brush brush, Font font, string text, RectangleF rect)
        {
            // Rotate the label around its center.
            var m = g.Transform;
            var saveM = m.Clone();
            m.Multiply(new Matrix(1, 0, 0, -1, 0, rect.Bottom + rect.Top));

            g.Transform = m;

            StringFormat stringFormat = StringFormat.GenericTypographic;
            stringFormat.Alignment = StringAlignment.Center;
            g.DrawString(text, font, brush, rect, stringFormat);

           g.Transform = saveM;
        }

        private static void FillTheGraphicsPath(DrawingNode drNode, float width, float height, ref float xRadius, ref float yRadius, GraphicsPath path)
        {
            NodeAttr nodeAttr = drNode.Attr;
            float w = width / 2;
            if (xRadius > w)
                xRadius = w;
            float h = height / 2;
            if (yRadius > h)
                yRadius = h;
            var x = (float)drNode.GeometryNode.Center.X;
            var y = (float)drNode.GeometryNode.Center.Y;
            float ox = w - xRadius;
            float oy = h - yRadius;
            float top = y + h;
            float bottom = y - h;
            float left = x - w;
            float right = x + w;

            const float PI = 180;
            if (ox > 0)
                path.AddLine(x - ox, bottom, x + ox, bottom);
            path.AddArc(x + ox - xRadius, y - oy - yRadius, 2 * xRadius, 2 * yRadius, 1.5f * PI, 0.5f * PI);

            if (oy > 0)
                path.AddLine(right, y - oy, right, y + oy);
            path.AddArc(x + ox - xRadius, y + oy - yRadius, 2 * xRadius, 2 * yRadius, 0, 0.5f * PI);
            if (ox > 0)
                path.AddLine(x + ox, top, x - ox, top);
            path.AddArc(x - ox - xRadius, y + oy - yRadius, 2 * xRadius, 2 * yRadius, 0.5f * PI, 0.5f * PI);
            if (oy > 0)
                path.AddLine(left, y + oy, left, y - oy);
            path.AddArc(x - ox - xRadius, y - oy - yRadius, 2 * xRadius, 2 * yRadius, PI, 0.5f * PI);
        }

        private static void DrawBox(Graphics g, DrawingNode drNode)
        {
            var pen = new Pen(Draw.MsaglColorToDrawingColor(drNode.Attr.Color), (float)drNode.Attr.LineWidth);
            NodeAttr nodeAttr = drNode.Attr;
            Color fc = Draw.MsaglColorToDrawingColor(nodeAttr.FillColor);
            var width = (float)drNode.Width;
            var height = (float)drNode.Height;
            var xRadius = (float)nodeAttr.XRadius;
            var yRadius = (float)nodeAttr.YRadius;
            var path = new GraphicsPath();
            FillTheGraphicsPath(drNode, width, height, ref xRadius, ref yRadius, path);
            Brush brush;
            if (nodeAttr.Styles.FirstOrDefault() == Style.Diagonals)
            {
                brush = new HatchBrush(HatchStyle.DiagonalCross, Color.FromArgb(160, 160, 165), fc);
            }
            else
            {
                brush = new SolidBrush(fc);
            }

            g.FillPath(brush, path);
            g.DrawPath(pen, path);
        }

        private static ICurve GetNodeBoundary(DrawingNode node)
        {
            double width;
            double height;
            var font = new Font(node.Label.FontName, (float)node.Label.FontSize, (System.Drawing.FontStyle)(int)node.Label.FontStyle);
            StringMeasure.MeasureWithFont(node.Label.Text, font, out width, out height);
            width += node.Attr.LabelMargin * 2;
            height += node.Attr.LabelMargin * 2;
            if (width <= 0)
            {
                // Temporary fix for Win7 where Measure fonts return negative length for the string " ".
                StringMeasure.MeasureWithFont("a", font, out width, out height);
            }

            if (node.UserData is Image)
            {
                width += kIconSize;
            }

            return NodeBoundaryCurves.GetNodeBoundaryCurve(node, width, height);
        }
    }
}
