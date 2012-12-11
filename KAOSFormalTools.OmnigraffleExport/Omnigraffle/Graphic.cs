using System;
using System.Collections.Generic;

namespace KAOSFormalTools.OmnigraffleExport.Omnigraffle
{
    public class ApplicationVersion 
    {
        public string Name    { get; set; }
        public string Version { get; set; }
    }

    public class BackgroundGraphic
    {
        public Bounds Bounds     { get; set; }
        public BackgroundGraphicClass Class      { get; set; }
        public int    ID         { get; set; }
        public ShadowInfo   Shadow   { get; set; }
        public bool   DrawStroke { get; set; }

        public BackgroundGraphic ()
        {
            ID = 0;
            Bounds = new Bounds (0, 0, 0, 0);
            Class = BackgroundGraphicClass.SolidGraphic;
            Shadow = new ShadowInfo () { Draws = false };
            DrawStroke = false;
        }
    }

    public enum BackgroundGraphicClass
    {
        SolidGraphic
    }

    public class Layer 
    {
        public string Name  { get; set; }
        public bool   Lock  { get; set; }
        public bool   Print { get; set; }
        public bool   View  { get; set; }

        public Layer ()
        {
            Name = "Unnamed Layer";
            Lock = false;
            Print = true;
            View = true;
        }
    }

    public class LayoutInfo
    {
        public bool                    Animate                  { get; set; }
        public bool                    AutoLayout               { get; set; }
        public HierarchicalOrientation HierarchicalOrientation  { get; set; }
        public double                  CircoMinDist             { get; set; }
        public double                  CircoSeparation          { get; set; }
        public double                  DotRankSep               { get; set; }
        public LayoutEngine            LayoutEngine             { get; set; }
        public double                  NeatoSeparation          { get; set; }

        public bool                    TwopiOverlap             { get; set; }
        public double                  TwopiSeparation          { get; set; }
        public double                  TwopiRankSep             { get; set; }


        public LayoutInfo ()
        {
            Animate = false;
            AutoLayout = false;
            HierarchicalOrientation = HierarchicalOrientation.TopBottom;
            CircoMinDist = 0;
            CircoSeparation = 0;
            DotRankSep = 0;
            LayoutEngine = LayoutEngine.Dot;
            NeatoSeparation = 0;
            TwopiSeparation = 1;
            TwopiRankSep = 100;
            TwopiOverlap = true;
        }
    }

    public class PrintInfo
    {
        public double BottomMargin            { get; set; }
        public double TopMargin               { get; set; }
        public double LeftMargin              { get; set; }
        public double RightMargin             { get; set; }

        public string HorizonalPagination     { get; set; }
        public string PaperSize               { get; set; }

        public bool   PrintReverseOrientation { get; set; }
    }

    public class WindowInfo
    {
        public int         CurrentSheet     { get; set; }
        public string      Frame            { get; set; }
        public bool        ListView         { get; set; }
        public int         OutlineWidth     { get; set; }
        public bool        RightSidebar     { get; set; }
        public bool        ShowRuler        { get; set; }
        public bool        Sidebar          { get; set; }
        public int         SidebarWidth     { get; set; }
        public string      VisibleRegion    { get; set; }
        public double      Zoom             { get; set; }
    }

    public class Sheet
    {
        public int                ActiveLayerIndex     = 0;
        public bool               AutoAdjust           = true;
        public BackgroundGraphic  BackgroundGraphic;
        public int                BaseZoom             = 0;
        public string             CanvasOrigin         = "{0, 0}";
        public int                ColumnAlign          = 1;
        public double             ColumnSpacing        = 36;
        public string             DisplayScale         = "1 0/72 in = 1.0000 in";
        public int HPages = 1;
        public int VPages = 1;
        public bool KeepToScale = false;
        public List<Layer> Layers;
        
        public LayoutInfo LayoutInfo;
        
        public Orientation Orientation = Orientation.Landscape;
        
        public bool PrintOnePage = false;
        public int RowAlign = 1;
        public double RowSpacing = 36;
        public string Title = "Canvas 1";
        public int UniqueId = 1;
        public bool Expanded = true;
        public double Zoom = 1;

        public List<Graphic> GraphicsList;

        public Sheet (int uniqueId, string title)
        {
            this.UniqueId = uniqueId;
            this.Title = title;

            BackgroundGraphic = new BackgroundGraphic ();

            LayoutInfo = new LayoutInfo ();

            GraphicsList = new List<Graphic> ();
            Layers = new List<Layer> ();
        }
    }

    public class ZoomValue {
        public string Name       { get; set; }
        public double Zoom       { get; set; }
    }

    public enum LayoutEngine {
        Dot, Neato, Circo, Twopi
    }
    
    public enum HierarchicalOrientation {
        TopBottom, BottomTop, LeftRight, RightLeft
    }
    
    public enum Orientation {
        Portrait, Landscape, PageSetup
    }

    public class ShadowInfo {
        public bool Draws { get; set; }
        public Point ShadowVector { get; set; }

        public ShadowInfo ()
        {
            Draws = true;
            ShadowVector = new Point (0, 4);
        }
    }

    public abstract class Graphic {
        public int       ID        { get; set; }
        public string Class;

        public Graphic (int id)
        {
            this.ID = id;
        }
    }

    public class Group : Graphic {
        public List<Graphic> Graphics { get; set; }
        public Group (int id) : base (id)
        {
            Graphics = new List<Graphic>();
            Class = "Group";
        }
    }

    public class ShapedGraphic : Graphic {
        public Bounds    Bounds    { get; set; }
        public FitText   FitText   { get; set; }
        public Flow      Flow      { get; set; }
        public FontInfo  FontInfo  { get; set; }
        public Shape     Shape     { get; set; }
        public ShapeData ShapeData { get; set; }
        public StyleInfo Style     { get; set; }
        public TextInfo    Text      { get; set; }

        public bool VFlip { get; set; }
        public bool HFlip { get; set; }

        public bool AllowConnections { get; set; }


        public ShapedGraphic (int id, Shape shape, double x1, double y1, double x2, double y2)
            : base (id)
        {
            Class = "ShapedGraphic";

            Bounds = new Bounds (x1, y1, x2, y2);
            this.Shape = shape;
            this.ShapeData = new ShapeData ();

            FontInfo = new FontInfo ();

            Style = new StyleInfo ();

            VFlip = false;
            HFlip = false;
            AllowConnections = true;
        }
    }

    public class TextInfo {
        public string Text { get; set; }
        public int SideMargin { get; set; }
        public int TopBottomMargin { get; set; }
        public TextAlignement Alignement { get; set; } 

        public TextInfo (string text)
        {
            Text = text;
            Alignement = TextAlignement.Left;
            SideMargin = 0; TopBottomMargin = 0;
        }
    }

    public class LineGraphic : Graphic {
        public LineEndInfo Head { get; set; }
        public LineEndInfo Tail { get; set; }

        public List<Point> Points { get; set; }

        public StyleInfo Style { get; set; }

        public LineGraphic (int id)
            : base (id)
        {
            Class = "LineGraphic";

            Points = new List<Point> ();
            Style = new StyleInfo ();
        }
    }

    public class LineEndInfo {
        public int ID { get; set; }

        public LineEndInfo (int id)
        {
            this.ID = id;
        }
    }

    public enum TextAlignement {
        Left, Right, Center, Justified
    }

    public enum FitText {
        Vertical, Clip
    }
    
    public enum Flow {
        Resize, Clip
    }

    public enum Shape {
        Bezier, Rectangle, Circle
    }

    public class ShapeData {
        public List<Point> UnitPoints { get; set; }
        public ShapeData ()
        {
            UnitPoints = new List<Point>();
        }
    }


    public class Bounds {
        public Point TopLeft     { get; set; }
        public Point BottomRight { get; set; }

        public Bounds (double x1, double y1, double x2, double y2)
        {
            TopLeft = new Point (x1, y1);
            BottomRight = new Point (x2, y2);
        }
    }

    public class Point {
        public double X   { get; set; }
        public double Y   { get; set; }

        public Point (double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public class FontInfo {
        public Color Color   { get; set; }
        public string Font   { get; set; }
        public double Kerning { get; set; }
        public double Size   { get; set; }

        public FontInfo ()
        {
            Color = new Color (0, 0, 0);
            Font = "Helvetica";
            Size = 14;
        }
    }

    public class StyleInfo {
        public FillInfo Fill { get; set; }
        public ShadowInfo Shadow { get; set; }
        public StrokeInfo Stroke { get; set; }

        public StyleInfo ()
        {
            Fill = new FillInfo ();
            Shadow = new ShadowInfo ();
            Stroke = new StrokeInfo ();
        }
    }

    public class StrokeInfo {
        public double CornerRadius { get; set; }
        public Color Color { get; set; }
        public Arrow HeadArrow { get; set; }
        public Arrow TailArrow { get; set; }
        public bool Legacy { get; set; }
        public double Width { get; set; }

        public StrokeInfo ()
        {
            Color = new Color ();
            HeadArrow = Arrow.None;
            TailArrow = Arrow.None;
            Legacy = true;
            Width = 1;
            CornerRadius = 0;
        }
    }

    public enum Arrow
    {
        None, FilledArrow, SharpBackCross, Arrow
    }

    public class FillInfo {
        public Color Color { get; set; }
    }

    public class Color {
        public double r { get; set; }
        public double g { get; set; }
        public double b { get; set; }

        public Color () : this (0, 0, 0)
        {}

        public Color (double r, double g, double b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
    }

    public class Document
    {
        public DateTime           CreationDate         = DateTime.Now;
        public DateTime           ModificationDate     = DateTime.Now;

        public string             Creator              = "KAOSFormalTools.OmnigraffleExport";
        public int                GraphDocumentVersion = 8;

        public bool GuidesLocked = false;
        public bool GuidesVisible = true;

        public int  ImageCounter = 1;

        public bool LinksVisible = false;
        public bool MagnetsVisible = false;
        public bool NotesVisible = false;
        public bool OriginVisible = false;
        public bool PageBreaks = false;

        public bool ReadOnly = false;

        public bool SmartAlignmentGuidesActive = true;
        public bool SmartDistanceGuidesActive = true;
        public bool UseEntirePage;

        public WindowInfo WindowInfo;
        public PrintInfo  PrintInfo;

        public ApplicationVersion ApplicationVersion;

        public List<Sheet> Canvas;

        public Document ()
        {
            ApplicationVersion = new ApplicationVersion () {
                Name = "com.omnigroup.OmniGrafflePro",
                Version = "139.16.0.171715"
            };

            Canvas = new List<Sheet> ();

            PrintInfo = new PrintInfo () {
                BottomMargin = 41, TopMargin = 18, LeftMargin = 18, RightMargin = 18,
                PrintReverseOrientation = false, PaperSize = "{595, 842}", 
                HorizonalPagination = "BAtzdHJlYW10eXBlZIHoA4QBQISEhAhOU051bWJlcgCEhAdOU1ZhbHVlAISECE5TT2JqZWN0AIWEASqEhAFxlwCG"
            };

            WindowInfo = new WindowInfo () {
                CurrentSheet = 0,
                Frame = "{{373, 4}, {693, 874}}",
                ListView = true,
                OutlineWidth = 142,
                RightSidebar = false,
                ShowRuler = false,
                Sidebar = true,
                SidebarWidth = 120,
                VisibleRegion = "{{0, 0}, {558, 720}}",
                Zoom = 1,
            };
        }
    }
}

