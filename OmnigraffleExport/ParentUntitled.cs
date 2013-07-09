using System;
using System.Collections.Generic;
using KAOSTools.OmnigraffleExport.Omnigraffle;
using KAOSTools.MetaModel;
using System.Text;

namespace KAOSTools.OmnigraffleExport
{
    public class ParentUntitled
    {
        protected IDictionary<string, IList<Graphic>> shapes;
        protected Sheet sheet;

        protected static int _i = 1;
        protected static int NextId {
            get {
                return _i++;
            }
        }

        public ParentUntitled (Sheet sheet, IDictionary<string, IList<Graphic>> shapes)
        {
            this.sheet = sheet;
            this.shapes = shapes;
        }

        protected void Add (string key, Graphic graphic)
        {
            if (!this.shapes.ContainsKey (key)) {
                this.shapes.Add (key, new List<Graphic> ());
            }
            this.shapes[key].Add (graphic);

            sheet.GraphicsList.Add (graphic);
        }

        protected static string GetRtfUnicodeEscapedString(string s)
        {
            if (s == null)
                return null;

            var sb = new StringBuilder();
            foreach (var c in s)
            {
                if(c == '\\' || c == '{' || c == '}')
                    sb.Append(@"\" + c);
                else if (c <= 0x7f)
                    sb.Append(c);
                else
                    sb.Append("\\u" + Convert.ToUInt32(c) + "?");
            }
            return sb.ToString();
        }

        protected ShapedGraphic GetParallelogram ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Bezier, 
                                             50, 50, 175, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));

            return graphic;
        }

        protected ShapedGraphic GetInvertParallelogram ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Bezier,
                                             50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, -0.5));

            return graphic;
        }

        protected ShapedGraphic GetHomeShape ()
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.5, 0.5));
            graphic.ShapeData.UnitPoints.Add(new KAOSTools.OmnigraffleExport.Omnigraffle.Point(-0.45, -0.5));

            return graphic;
        }

        protected ShapedGraphic GetHexagon ()
        {
            var graphic = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Bezier, 50, 50, 150, 70);

            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, -0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, 0.5));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.5, 0));
            graphic.ShapeData.UnitPoints.Add (new KAOSTools.OmnigraffleExport.Omnigraffle.Point (-0.45, -0.5));

            return graphic;
        }

        protected ShapedGraphic GetRectangle ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Rectangle, 
                                             50, 50, 175, 70);
            return graphic;
        }

        protected ShapedGraphic GetCloud ()
        {
            var graphic = new ShapedGraphic (NextId, 
                                             Omnigraffle.Shape.Cloud, 
                                             50, 50, 175, 70);
            return graphic;
        }

        protected ShapedGraphic GetCircle ()
        {
            var circle = new ShapedGraphic (NextId, Omnigraffle.Shape.Circle, 50, 50, 10, 10);
            circle.Style.Shadow.Draws = false;

            circle.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Clip;
            circle.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Clip;

            return circle;
        }

        protected LineGraphic GetLine (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = new Omnigraffle.LineGraphic (NextId);
            line.Head = new Omnigraffle.LineEndInfo (to.ID);
            line.Tail = new Omnigraffle.LineEndInfo (@from.ID);

            if (to is ShapedGraphic) {
                line.Points.Add ((to as ShapedGraphic).Bounds.TopLeft);
            } else if (to is Omnigraffle.Group) {
                line.Points.Add (new Point(25,25));
            }

            if (@from is ShapedGraphic) {
                line.Points.Add ((@from as ShapedGraphic).Bounds.BottomRight);
            } else if (@from is Omnigraffle.Group) {
                line.Points.Add (new Point(30,30));
            }

            line.Style.Shadow.Draws = false;
            
            if (dashed)
                line.Style.Stroke.Pattern = StrokePattern.Dashed;

            return line;
        }

        protected LineGraphic GetArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow;

            return line;
        }

        protected LineGraphic GetFilledArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.HeadArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow;
            return line;
        }

        protected LineGraphic GetSharpBackCrossArrow (Graphic @from, Graphic to, bool dashed = false)
        {
            var line = GetLine (@from, to, dashed);
            line.Style.Stroke.TailArrow = KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross;
            return line;
        }

        protected void AddText (LineGraphic graphic, string text, float position = 0.5f)
        {
            var labelBox = new Omnigraffle.ShapedGraphic (NextId, Omnigraffle.Shape.Rectangle, 50, 50, 100, 100);
            labelBox.Text = new Omnigraffle.TextInfo (text) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 0, TopBottomMargin = 0
            };
            labelBox.FontInfo.Size = 10;
            labelBox.Style.Shadow.Draws = false;
            labelBox.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Yes;
            labelBox.Wrap = false;
            labelBox.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;
            labelBox.Style.Fill.Color = new KAOSTools.OmnigraffleExport.Omnigraffle.Color (1, 1, 1);
            labelBox.Style.Stroke.Draws = false;
            labelBox.Line = new LineInfo (graphic.ID) { Position = position };

            sheet.GraphicsList.Add (labelBox);
        }

        protected enum TextAlignement {
            Left, Center, Right
        }

        protected void AddText (ShapedGraphic graphic, string text, 
                                bool bold = false, TextAlignement align = TextAlignement.Center)
        {
            var str = (bold ? @"\b " : "");
            str += (align == TextAlignement.Left ? @"\ql " : "");
            str += (align == TextAlignement.Right ? @"\qr " : "");
            str += (align == TextAlignement.Center ? @"\qc " : "");
            str += GetRtfUnicodeEscapedString (text);

            graphic.Text = new Omnigraffle.TextInfo (str) {
                Alignement = KAOSTools.OmnigraffleExport.Omnigraffle.TextAlignement.Center,
                SideMargin = 10,
                TopBottomMargin = 3
            };
            graphic.Style.Shadow.Draws = false;
            graphic.FitText = KAOSTools.OmnigraffleExport.Omnigraffle.FitText.Vertical;
            graphic.Flow = KAOSTools.OmnigraffleExport.Omnigraffle.Flow.Resize;

        }

        protected void SetFillColor (ShapedGraphic graphic, double red, double green, double blue)
        {
            graphic.Style.Fill.Color = new Color (red, green, blue);
        }

        protected void SetStrokeWidth (ShapedGraphic graphic, int lineWidth)
        {
            graphic.Style.Stroke.Width = lineWidth;
        }

        protected void AddParallelogram (string key,
                                       string text,
                                       int lineWidth,
                                       double red, double green, double blue)
        {
            var graphic = GetParallelogram ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddInvertParallelogram (string key,
                                             string text,
                                             int lineWidth,
                                             double red, double green, double blue)
        {
            var graphic = GetInvertParallelogram ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddHomeShape (string key,
                                   string text,
                                   int lineWidth,
                                   double red, double green, double blue)
        {
            var graphic = GetHomeShape ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddHexagon (string key,
                                 string text,
                                 int lineWidth,
                                 double red, double green, double blue)
        {
            var graphic = GetHexagon ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddRectangle (string key,
                                     string text,
                                     int lineWidth,
                                     double red, double green, double blue)
        {
            var graphic = GetRectangle ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }

        protected void AddCloud (string key,
                               string text,
                               int lineWidth,
                               double red, double green, double blue)
        {
            var graphic = GetCloud ();
            AddText (graphic, text);
            SetFillColor (graphic, red, green, blue);
            SetStrokeWidth (graphic, lineWidth);

            Add (key, graphic);
        }
    }
}

