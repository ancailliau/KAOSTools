using System;
using System.IO;
using CE.iPhone.PList;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace KAOSTools.OmnigraffleExport
{
    public class OmniGraffleGenerator
    {
        public static void Export (Omnigraffle.Document doc, TextWriter stream)
        {
            var root = ExportDocument (doc);
            using (MemoryStream memStream = new MemoryStream ()) {
                root.Save (memStream, PListFormat.Xml);
                stream.Write (Encoding.UTF8.GetString (memStream.ToArray()));
            }
        }

        public static void Export (Omnigraffle.Document doc, string filename)
        {
            var root = ExportDocument (doc);
            root.Save (filename);
        }

        private static IPListElement ExportBackgroundGraphicClass (Omnigraffle.BackgroundGraphicClass @class)
        {
            if (@class == Omnigraffle.BackgroundGraphicClass.SolidGraphic)
                return new PListString ("SolidGraphic");

            return null;
        }

        private static PListRoot ExportDocument (Omnigraffle.Document doc)
        {
            var root = new PListRoot ();
            root.Format = PListFormat.Xml;
            
            var dict = new PListDict ();

            var applicationVersion = new PListArray ();
            applicationVersion.Add (new PListString (doc.ApplicationVersion.Name));
            applicationVersion.Add (new PListString (doc.ApplicationVersion.Version));
            dict.Add ("ApplicationVersion", applicationVersion);

            var sheets = new PListArray ();
            foreach (var sheet in doc.Canvas) {
                var sheet_dict = new PListDict ();

                sheet_dict.Add ("ActiveLayerIndex", new PListInteger (sheet.ActiveLayerIndex));
                sheet_dict.Add ("AutoAdjust", new PListBool (sheet.AutoAdjust));
                
                var backgroundGraphic = new PListDict ();
                backgroundGraphic.Add ("Bounds", ExportBounds (sheet.BackgroundGraphic.Bounds));
                backgroundGraphic.Add ("Class", ExportBackgroundGraphicClass (sheet.BackgroundGraphic.Class));
                backgroundGraphic.Add ("ID", new PListInteger (sheet.BackgroundGraphic.ID));
                
                var style = new PListDict ();
                var shadow = ExportShadow (sheet.BackgroundGraphic.Shadow);
                
                var stroke = new PListDict ();
                stroke.Add ("Draws", new PListString (sheet.BackgroundGraphic.DrawStroke ? "YES" : "NO"));
                
                style.Add ("shadow", shadow);
                style.Add ("stroke", stroke);
                backgroundGraphic.Add ("Style", style);

                sheet_dict.Add ("BackgroundGraphic", backgroundGraphic);

                sheet_dict.Add ("BaseZoom", new PListInteger (sheet.BaseZoom));
                sheet_dict.Add ("CanvasOrigin", new PListString (sheet.CanvasOrigin));
                sheet_dict.Add ("ColumnAlign", new PListInteger (sheet.ColumnAlign));
                
                sheet_dict.Add ("ColumnSpacing", new PListReal (sheet.ColumnSpacing));

                sheet_dict.Add ("DisplayScale", new PListString (sheet.DisplayScale));
                sheet_dict.Add ("HPages", new PListInteger (sheet.HPages));

                var layers = new PListArray ();
                if (sheet.Layers.Count > 1) {
                    foreach (var layer in sheet.Layers) {
                        layers.Add (ExportLayer (layer));
                    }
                } else {
                    layers.Add (ExportLayer (new Omnigraffle.Layer ()));
                }
                sheet_dict.Add ("Layers", layers);
                
                var layoutInfo = new PListDict ();
                layoutInfo.Add ("Animate", new PListString (sheet.LayoutInfo.Animate ? "YES" : "NO"));

                if (sheet.LayoutInfo.AutoLayout) 
                    layoutInfo.Add ("AutoLayout", new PListInteger (1));

                if (sheet.LayoutInfo.HierarchicalOrientation == Omnigraffle.HierarchicalOrientation.LeftRight) 
                    layoutInfo.Add ("HierarchicalOrientation", new PListInteger (0));
                else if (sheet.LayoutInfo.HierarchicalOrientation == Omnigraffle.HierarchicalOrientation.TopBottom) 
                    layoutInfo.Add ("HierarchicalOrientation", new PListInteger (1));
                else if (sheet.LayoutInfo.HierarchicalOrientation == Omnigraffle.HierarchicalOrientation.RightLeft) 
                    layoutInfo.Add ("HierarchicalOrientation", new PListInteger (2));
                else if (sheet.LayoutInfo.HierarchicalOrientation == Omnigraffle.HierarchicalOrientation.BottomTop) 
                    layoutInfo.Add ("HierarchicalOrientation", new PListInteger (3));
                
                if (sheet.LayoutInfo.LayoutEngine == Omnigraffle.LayoutEngine.Circo) 
                    layoutInfo.Add ("layoutEngine", new PListString ("circo"));
                else if (sheet.LayoutInfo.LayoutEngine == Omnigraffle.LayoutEngine.Dot) 
                    layoutInfo.Add ("layoutEngine", new PListString ("dot"));
                else if (sheet.LayoutInfo.LayoutEngine == Omnigraffle.LayoutEngine.Neato) 
                    layoutInfo.Add ("layoutEngine", new PListString ("neato"));
                else if (sheet.LayoutInfo.LayoutEngine == Omnigraffle.LayoutEngine.Twopi) 
                    layoutInfo.Add ("layoutEngine", new PListString ("twopi"));
                
                layoutInfo.Add ("circoMinDist", new PListReal (sheet.LayoutInfo.CircoMinDist));
                layoutInfo.Add ("circoSeparation", new PListReal (sheet.LayoutInfo.CircoSeparation));
                layoutInfo.Add ("dotRankSep", new PListReal (sheet.LayoutInfo.DotRankSep));
                layoutInfo.Add ("neatoSeparation", new PListReal (sheet.LayoutInfo.NeatoSeparation));

                layoutInfo.Add ("twopiOverlap", new PListBool (sheet.LayoutInfo.TwopiOverlap));
                layoutInfo.Add ("twopiSeparation", new PListReal (sheet.LayoutInfo.TwopiSeparation));
                layoutInfo.Add ("twopiRankSep", new PListReal (sheet.LayoutInfo.TwopiRankSep));
                
                sheet_dict.Add ("LayoutInfo", layoutInfo);

                if (sheet.Orientation == Omnigraffle.Orientation.Portrait)
                    sheet_dict.Add ("Orientation", new PListInteger(0));
                else if (sheet.Orientation == Omnigraffle.Orientation.Landscape)
                    sheet_dict.Add ("Orientation", new PListInteger(1));
                else if (sheet.Orientation == Omnigraffle.Orientation.PageSetup)
                    sheet_dict.Add ("Orientation", new PListInteger(2));

                sheet_dict.Add ("PrintOnePage", new PListBool (sheet.PrintOnePage));
                sheet_dict.Add ("RowAlign", new PListInteger (sheet.RowAlign));
                sheet_dict.Add ("RowSpacing", new PListReal (sheet.RowSpacing));
                
                sheet_dict.Add ("SheetTitle", new PListString (sheet.Title));

                sheet_dict.Add ("UniqueID", new PListInteger (sheet.UniqueId));
                sheet_dict.Add ("VPages", new PListInteger (sheet.VPages));

                var graphics_array = new PListArray ();

                foreach (var graphic in sheet.GraphicsList) {
                    graphics_array.Add (ExportGraphic (graphic));
                }

                sheet_dict.Add ("GraphicsList", graphics_array);

                sheet_dict.Add ("GridInfo", new PListDict ());

                sheets.Add (sheet_dict);
            }
            dict.Add ("Sheets", sheets);


            dict.Add ("CreationDate", new PListString (doc.CreationDate.ToString ("yyyy-MM-dd hh:mm:ss +0000")));
            dict.Add ("ModificationDate", new PListString (doc.ModificationDate.ToString ("yyyy-MM-dd hh:mm:ss +0000")));

            dict.Add ("Creator", new PListString (doc.Creator));

            dict.Add ("GraphDocumentVersion", new PListInteger (doc.GraphDocumentVersion));

            dict.Add ("GuidesLocked", new PListString (doc.GuidesLocked ? "YES" : "NO"));
            dict.Add ("GuidesVisible", new PListString (doc.GuidesVisible ? "YES" : "NO"));

            dict.Add ("ImageCounter", new PListInteger (doc.ImageCounter));

            dict.Add ("KeepToScale", new PListBool ());

            dict.Add ("LinksVisible", new PListString (doc.LinksVisible ? "YES" : "NO"));
            dict.Add ("MagnetsVisible", new PListString (doc.MagnetsVisible ? "YES" : "NO"));
            dict.Add ("NotesVisible", new PListString (doc.NotesVisible ? "YES" : "NO"));
            dict.Add ("OriginVisible", new PListString (doc.OriginVisible ? "YES" : "NO"));
            dict.Add ("PageBreaks", new PListString (doc.PageBreaks ? "YES" : "NO"));

            dict.Add ("MasterSheets", new PListArray ());
            dict.Add ("Modifier", new PListString ());

            var printInfo = new PListDict ();

            var bottomMargin = new PListArray ();
            bottomMargin.Add (new PListString ("float"));
            bottomMargin.Add (new PListString (doc.PrintInfo.BottomMargin.ToString ()));
            printInfo.Add ("NSBottomMargin", bottomMargin);

            var leftMargin = new PListArray ();
            leftMargin.Add (new PListString ("float"));
            leftMargin.Add (new PListString (doc.PrintInfo.LeftMargin.ToString ()));
            printInfo.Add ("NSLeftMargin", leftMargin);

            var rightMargin = new PListArray ();
            rightMargin.Add (new PListString ("float"));
            rightMargin.Add (new PListString (doc.PrintInfo.RightMargin.ToString ()));
            printInfo.Add ("NSRightMargin",rightMargin);

            var topMargin = new PListArray ();
            topMargin.Add (new PListString ("float"));
            topMargin.Add (new PListString (doc.PrintInfo.TopMargin.ToString ()));
            printInfo.Add ("NSTopMargin", topMargin);

            var horizonalPagination = new PListArray ();
            horizonalPagination.Add (new PListString ("coded"));
            horizonalPagination.Add (new PListString (doc.PrintInfo.HorizonalPagination));
            printInfo.Add ("NSHorizonalPagination", horizonalPagination);

            var paperSize = new PListArray ();
            paperSize.Add (new PListString ("size"));
            paperSize.Add (new PListString (doc.PrintInfo.PaperSize));
            printInfo.Add ("NSPaperSize", paperSize);

            var printReverseOrientation = new PListArray ();
            printReverseOrientation.Add (new PListString ("int"));
            printReverseOrientation.Add (new PListString (doc.PrintInfo.PrintReverseOrientation ? "1" : "0"));
            printInfo.Add ("NSPrintReverseOrientation", printReverseOrientation);

            dict.Add ("PrintInfo", printInfo);

            dict.Add ("ReadOnly", new PListString (doc.ReadOnly ? "YES" : "NO"));
            dict.Add ("SmartAlignmentGuidesActive", new PListString (doc.SmartAlignmentGuidesActive ? "YES" : "NO"));
            dict.Add ("SmartDistanceGuidesActive", new PListString (doc.SmartDistanceGuidesActive ? "YES" : "NO"));


            dict.Add ("UseEntirePage", new PListBool (doc.UseEntirePage));

            var windowInfo = new PListDict ();
            windowInfo.Add ("CurrentSheet", new PListInteger (doc.WindowInfo.CurrentSheet));

            var expanded_canvases = new PListArray ();
            foreach (var sheet in doc.Canvas.Where (s => s.Expanded)) {
                var canvas_dict = new PListDict ();
                canvas_dict.Add ("name", new PListString(sheet.Title));
                expanded_canvases.Add (canvas_dict);
            }
            windowInfo.Add ("ExpandedCanvases", expanded_canvases);

            windowInfo.Add ("Frame", new PListString (doc.WindowInfo.Frame));
            windowInfo.Add ("ListView", new PListBool (doc.WindowInfo.ListView));
            windowInfo.Add ("RightSidebar", new PListBool (doc.WindowInfo.RightSidebar));
            windowInfo.Add ("ShowRuler", new PListBool (doc.WindowInfo.ShowRuler));
            windowInfo.Add ("Sidebar", new PListBool (doc.WindowInfo.Sidebar));

            windowInfo.Add ("SidebarWidth", new PListInteger (doc.WindowInfo.SidebarWidth));
            windowInfo.Add ("OutlineWidth", new PListInteger (doc.WindowInfo.OutlineWidth));

            windowInfo.Add ("VisibleRegion", new PListString ("{{0, 0}, {558, 720}}"));

            windowInfo.Add ("Zoom", new PListReal (doc.WindowInfo.Zoom));

            var zoom_values = new PListArray ();
            foreach (var sheet in doc.Canvas) {
                var zoom_array = new PListArray ();
                zoom_array.Add (new PListString (sheet.Title));
                zoom_array.Add (new PListReal (sheet.Zoom));
                zoom_array.Add (new PListReal (1));
            }
            windowInfo.Add ("ZoomValues", zoom_values);

            dict.Add ("WindowInfo", windowInfo);

            root.Root = dict;

            return root;
        }

        private static IPListElement ExportLayer (KAOSTools.OmnigraffleExport.Omnigraffle.Layer layer)
        {
            var layer_dict = new PListDict ();
            layer_dict.Add ("Name", new PListString (layer.Name));
            layer_dict.Add ("Lock", new PListString (layer.Lock ? "YES" : "NO"));
            layer_dict.Add ("View", new PListString (layer.View ? "YES" : "NO"));
            layer_dict.Add ("Print", new PListString (layer.Print ? "YES" : "NO"));

            return layer_dict;
        }

        private static  IPListElement ExportGraphic (Omnigraffle.Graphic graphic) {
            if (graphic is Omnigraffle.ShapedGraphic) 
                return ExportShapedGraphic ((Omnigraffle.ShapedGraphic) graphic);
            else if (graphic is Omnigraffle.LineGraphic) 
                return ExportLineGraphic ((Omnigraffle.LineGraphic) graphic);
            else if (graphic is Omnigraffle.Group) 
                return ExportGroup ((Omnigraffle.Group) graphic);
            else
                throw new NotImplementedException ();
        }

        private static IPListElement ExportBounds (Omnigraffle.Bounds bounds)
        {
            return new PListString (string.Format ("{{{{{0}, {1}}}, {{{2}, {3}}}}}", bounds.TopLeft.X, bounds.TopLeft.Y, bounds.BottomRight.X, bounds.BottomRight.Y));
        }

        private static  IPListElement ExportGroup (Omnigraffle.Group @group) {
            var dict = new PListDict ();
            
            dict.Add ("Class", new PListString (@group.Class));
            dict.Add ("ID", new PListInteger (@group.ID));

            var graphics_array = new PListArray ();
            foreach (var graphic in @group.Graphics) {
                graphics_array.Add (ExportGraphic (graphic));
            }

            dict.Add ("Graphics", graphics_array);

            return dict;
        }

        private static  IPListElement ExportShapedGraphic (Omnigraffle.ShapedGraphic graphic) {
            var dict = new PListDict ();

            dict.Add ("Bounds", ExportBounds (graphic.Bounds));
            dict.Add ("Class", new PListString (graphic.Class));
            dict.Add ("ID", new PListInteger (graphic.ID));

            var shape = new PListString ();
            if (graphic.Shape == Omnigraffle.Shape.Bezier)
                shape.Value = "Bezier";
            else if (graphic.Shape == Omnigraffle.Shape.Rectangle)
                shape.Value = "Rectangle";
            else if (graphic.Shape == Omnigraffle.Shape.Circle)
                shape.Value = "Circle";
            else
                throw new NotImplementedException ();

            dict.Add ("Shape", shape);

                var fit_text = new PListString ();
                if (graphic.FitText == Omnigraffle.FitText.Vertical)
                    fit_text.Value = "Vertical";
                if (graphic.FitText == Omnigraffle.FitText.Clip)
                    fit_text.Value = "Clip";
                dict.Add ("FitText", fit_text);

                var flow = new PListString ();
                if (graphic.Flow == Omnigraffle.Flow.Resize)
                    flow.Value = "Resize";
                if (graphic.Flow == Omnigraffle.Flow.Clip)
                    flow.Value = "Clip";
                dict.Add ("Flow", flow);

            if (graphic.FontInfo != default (Omnigraffle.FontInfo)) {
                dict.Add ("Font", ExportFont (graphic.FontInfo));
            }

            if (graphic.ShapeData != default (Omnigraffle.ShapeData)) 
                dict.Add ("ShapeData", ExportShapeData (graphic.ShapeData));

            if (graphic.Style != default (Omnigraffle.StyleInfo)) 
                dict.Add ("Style", ExportStyle (graphic.Style));
            
            if (graphic.VFlip) 
                dict.Add ("VFlip", new PListBool (graphic.VFlip));

            if (graphic.HFlip) 
                dict.Add ("HFlip", new PListBool (graphic.HFlip));

            if (!graphic.AllowConnections) 
                dict.Add ("AllowConnections", new PListString (graphic.AllowConnections ? "YES" : "NO"));

            if (graphic.Text != default(Omnigraffle.TextInfo))
                dict.Add ("Text", ExportText (graphic));

            if (graphic.Line != null) {
                var d2 = new PListDict();
                d2.Add ("ID", new PListInteger(graphic.Line.ID));
                d2.Add ("Position", new PListReal(graphic.Line.Position));
                d2.Add ("RotationType", new PListInteger(graphic.Line.RotationType == Omnigraffle.RotationType.Default ? 0 : 0));
                dict.Add ("Line", d2);
            }

            return dict;
        }

        private static  IPListElement ExportLineGraphic (Omnigraffle.LineGraphic graphic) {
            var dict = new PListDict ();
            
            dict.Add ("Class", new PListString (graphic.Class));
            dict.Add ("ID", new PListInteger (graphic.ID));
            
            dict.Add ("Head", ExportLineEndInfo (graphic.Head));
            dict.Add ("Tail", ExportLineEndInfo (graphic.Tail));

            dict.Add ("Points", ExportPoints (graphic.Points));

            dict.Add ("Style", ExportStyle (graphic.Style));
            
            return dict;
        }

        private static IPListElement ExportPoints (List<Omnigraffle.Point> points)
        {
            var array = new PListArray ();
            foreach (var point in points) {
                array.Add (ExportPoint (point));
            }
            return array;
        }

        private static IPListElement ExportLineEndInfo (Omnigraffle.LineEndInfo @end)
        {
            var dict = new PListDict ();
            dict.Add ("ID", new PListInteger (@end.ID));
            return dict;
        }

        static PListDict ExportFont (Omnigraffle.FontInfo font)
        {
            var dict = new PListDict ();
            dict.Add ("Color", ExportColor (font.Color));
            dict.Add ("Font", new PListString (font.Font));
            dict.Add ("NSKern", new PListReal (font.Kerning));
            dict.Add ("Size", new PListReal (font.Size));
            return dict;
        }

        static PListDict ExportStyle (Omnigraffle.StyleInfo style)
        {
            var dict = new PListDict ();

            if (style.Fill != default (Omnigraffle.FillInfo)) {
                var fill = new PListDict ();

                if (style.Fill.Color != default (Omnigraffle.Color)) 
                    fill.Add ("Color", ExportColor (style.Fill.Color));

                dict.Add ("fill", fill);
            }

            if (style.Shadow != default (Omnigraffle.ShadowInfo))
                dict.Add ("shadow", ExportShadow (style.Shadow));

            if (style.Stroke != default (Omnigraffle.StrokeInfo)) 
                dict.Add ("stroke", ExportStroke (style.Stroke));

            return dict;
        }

        static IPListElement ExportArrow (Omnigraffle.Arrow tailArrow)
        {
            if (tailArrow == KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.None)
                return new PListString ("0");

            if (tailArrow == KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.FilledArrow)
                return new PListString ("FilledArrow");

            if (tailArrow == KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.SharpBackCross)
                return new PListString ("SharpBackCross");
            
            if (tailArrow == KAOSTools.OmnigraffleExport.Omnigraffle.Arrow.Arrow)
                return new PListString ("Arrow");

            throw new NotImplementedException ();
        }

        static IPListElement ExportStroke (Omnigraffle.StrokeInfo stroke)
        {
            var dict = new PListDict ();

            if (stroke.Draws) {
            dict.Add ("HeadArrow", ExportArrow (stroke.HeadArrow));
            dict.Add ("TailArrow", ExportArrow (stroke.TailArrow));
            
            dict.Add ("Legacy", new PListBool (stroke.Legacy));

            dict.Add ("Width", new PListReal (stroke.Width));

            dict.Add ("CornerRadius", new PListReal (stroke.CornerRadius));
            } else {
                dict.Add ("Draws", new PListString (stroke.Draws ? "YES" : "NO"));
            }

            return dict;
        }

        static IPListElement ExportShapeData (Omnigraffle.ShapeData data)
        {
            var dict = new PListDict ();

            if (data.UnitPoints != null && data.UnitPoints.Count > 0) {
                var unitPoints_array = new PListArray ();
                foreach (var point in data.UnitPoints) {
                    unitPoints_array.Add (ExportPoint (point));
                }
                dict.Add ("UnitPoints", unitPoints_array);
            }

            return dict;
        }

        static IPListElement ExportPoint (Omnigraffle.Point point)
        {
            return new PListString (string.Format ("{{{0}, {1}}}", point.X, point.Y));
        }

        private static IPListElement ExportColor (Omnigraffle.Color color)
        {
            var dict = new PListDict ();
            dict.Add ("r", new PListString (color.r.ToString ()));
            dict.Add ("g", new PListString (color.g.ToString ()));
            dict.Add ("b", new PListString (color.b.ToString ()));
            return dict;
        }

        private static  PListDict ExportShadow (Omnigraffle.ShadowInfo shadowInfo)
        {
            var shadow = new PListDict ();
            shadow.Add ("Draws", new PListString (shadowInfo.Draws ? "YES" : "NO"));

            if (shadowInfo.ShadowVector != null)
                shadow.Add ("ShadowVector", new PListString (string.Format ("{{{0}, {1}}}", shadowInfo.ShadowVector.X,  shadowInfo.ShadowVector.Y)));

            return shadow;
        }

        private static IPListElement ExportText (Omnigraffle.ShapedGraphic graphic)
        {
            var dict = new PListDict ();

            var alignement = @"\ql";
            if (graphic.Text.Alignement == Omnigraffle.TextAlignement.Center)
                alignement = @"\qc";
            else if (graphic.Text.Alignement == Omnigraffle.TextAlignement.Right)
                alignement = @"\qr";
            else if (graphic.Text.Alignement == Omnigraffle.TextAlignement.Justified)
                alignement = @"\qj";

            dict.Add ("Text", new PListString (string.Format (@"{{\rtf1\ansi\ansicpg1252\cocoartf1138\cocoasubrtf470
{{\fonttbl\f0\fswiss\fcharset0 {1};}}
{{\colortbl;\red255\green255\blue255;}}
\pard\tx560\tx1120\tx1680\tx2240\tx2800\tx3360\tx3920\tx4480\tx5040\tx5600\tx6160\tx6720\pardirnatural{3}

\f0\fs{2} \cf0 {0}}}", GetRtfUnicodeEscapedString (graphic.Text.Text), graphic.FontInfo.Font, graphic.FontInfo.Size * 2, alignement)));

            // if (graphic.Text.SideMargin > 0)
                dict.Add ("Pad", new PListInteger (graphic.Text.SideMargin));

            // if (graphic.Text.TopBottomMargin > 0)
                dict.Add ("VerticalPad", new PListInteger (graphic.Text.TopBottomMargin));

            return dict;
        }

        private static string GetRtfUnicodeEscapedString(string s)
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
    }
}

