using System;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using NDesk.Options;
using KAOSFormalTools.Domain;

/*
 * WARNING : This code is highly experimental and... ugly, really ugly.
 * 
 * In addition OmniGraffle FileFormat is not documented which makes the whole
 * a lot more complex to engineer.
 * 
 * The purpose of the tool is generate all elements from the model. It will not
 * layout it, or try to make boxes the right size, etc. Use OmniGraffle for that
 * purpose!
 * 
 */

namespace KAOSFormalTools.OmnigraffleExport
{
    class MainClass
    {
        static Random random;

        public static void Main (string[] args)
        {
            bool show_help = false;
            
            var p = new OptionSet () {
                { "h|help",  "show this message and exit", 
                    v => show_help = true },
            };
            
            List<string> r;
            try {
                r = p.Parse (args);
                
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }
            
            if (show_help) {
                ShowHelp (p);
                return;
            }
            
            if (r.Count == 0) {
                PrintError ("Please provide a file");
                return;
            }
            
            if (r.Count > 1) {
                PrintError ("Please provide only one file");
                return;
            }
            
            if (!File.Exists (r[0])) {
                PrintError ("File `" + r[0] + "` does not exists");
                return;
            }

            random = new Random();
            
            var model =  BuildModel (r[0]);
            PrintHeader ();
            foreach (var g in model.RootGoals)
                DisplayGoal (g);

            PrintFooter();
        }

        static void PrintHeader ()
        {
            Console.WriteLine (@"
<?xml version=""1.0"" encoding=""UTF-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
    <plist version=""1.0"">
        <dict>
        <key>ActiveLayerIndex</key>
        <integer>0</integer>
        <key>ApplicationVersion</key>
        <array>
        <string>com.omnigroup.OmniGrafflePro</string>
        <string>139.16.0.171715</string>
        </array>
        <key>AutoAdjust</key>
        <true/>
        <key>BackgroundGraphic</key>
        <dict>
        <key>Bounds</key>
        <string>{{0, 0}, {559, 783}}</string>
        <key>Class</key>
        <string>SolidGraphic</string>
        <key>ID</key>
        <integer>2</integer>
        <key>Style</key>
        <dict>
        <key>shadow</key>
        <dict>
        <key>Draws</key>
        <string>NO</string>
        </dict>
        <key>stroke</key>
        <dict>
        <key>Draws</key>
        <string>NO</string>
        </dict>
        </dict>
        </dict>
        <key>BaseZoom</key>
        <integer>0</integer>
        <key>CanvasOrigin</key>
        <string>{0, 0}</string>
        <key>ColumnAlign</key>
        <integer>1</integer>
        <key>ColumnSpacing</key>
        <real>36</real>
        <key>CreationDate</key>
        <string>2012-10-10 08:00:43 +0000</string>
        <key>Creator</key>
        <string></string>
        <key>DisplayScale</key>
        <string>1 0/72 in = 1.0000 in</string>
        <key>GraphDocumentVersion</key>
        <integer>8</integer>
        <key>GraphicsList</key>
        <array>");
        }

        static void PrintFooter () 
        {
            Console.WriteLine (@"
    </array>
    <key>GridInfo</key>
    <dict/>
    <key>GuidesLocked</key>
    <string>NO</string>
    <key>GuidesVisible</key>
    <string>YES</string>
    <key>HPages</key>
    <integer>1</integer>
    <key>ImageCounter</key>
    <integer>1</integer>
    <key>KeepToScale</key>
    <false/>
    <key>Layers</key>
    <array>
        <dict>
            <key>Lock</key>
            <string>NO</string>
            <key>Name</key>
            <string>Layer 1</string>
            <key>Print</key>
            <string>YES</string>
            <key>View</key>
            <string>YES</string>
        </dict>
    </array>
    <key>LayoutInfo</key>
    <dict>
        <key>Animate</key>
        <string>NO</string>
        <key>AutoLayout</key>
        <integer>2</integer>
        <key>HierarchicalOrientation</key>
        <integer>3</integer>
        <key>circoMinDist</key>
        <real>18</real>
        <key>circoSeparation</key>
        <real>0.0</real>
        <key>dotRankSep</key>
        <real>0.20000000298023224</real>
        <key>layoutEngine</key>
        <string>dot</string>
        <key>neatoSeparation</key>
        <real>0.0</real>
        <key>twopiSeparation</key>
        <real>0.0</real>
    </dict>
    <key>LinksVisible</key>
    <string>NO</string>
    <key>MagnetsVisible</key>
    <string>NO</string>
    <key>MasterSheets</key>
    <array/>
    <key>ModificationDate</key>
    <string>2012-10-10 09:08:33 +0000</string>
    <key>Modifier</key>
    <string></string>
    <key>NotesVisible</key>
    <string>NO</string>
    <key>Orientation</key>
    <integer>2</integer>
    <key>OriginVisible</key>
    <string>NO</string>
    <key>PageBreaks</key>
    <string>YES</string>
    <key>PrintInfo</key>
    <dict>
        <key>NSBottomMargin</key>
        <array>
            <string>float</string>
            <string>41</string>
        </array>
        <key>NSHorizonalPagination</key>
        <array>
            <string>coded</string>
            <string>BAtzdHJlYW10eXBlZIHoA4QBQISEhAhOU051bWJlcgCEhAdOU1ZhbHVlAISECE5TT2JqZWN0AIWEASqEhAFxlwCG</string>
        </array>
        <key>NSLeftMargin</key>
        <array>
            <string>float</string>
            <string>18</string>
        </array>
        <key>NSPaperSize</key>
        <array>
            <string>size</string>
            <string>{595, 842}</string>
        </array>
        <key>NSPrintReverseOrientation</key>
        <array>
            <string>int</string>
            <string>0</string>
        </array>
        <key>NSRightMargin</key>
        <array>
            <string>float</string>
            <string>18</string>
        </array>
        <key>NSTopMargin</key>
        <array>
            <string>float</string>
            <string>18</string>
        </array>
    </dict>
    <key>PrintOnePage</key>
    <false/>
    <key>ReadOnly</key>
    <string>NO</string>
    <key>RowAlign</key>
    <integer>1</integer>
    <key>RowSpacing</key>
    <real>36</real>
    <key>SheetTitle</key>
    <string>Canvas 1</string>
    <key>SmartAlignmentGuidesActive</key>
    <string>YES</string>
    <key>SmartDistanceGuidesActive</key>
    <string>YES</string>
    <key>UniqueID</key>
    <integer>1</integer>
    <key>UseEntirePage</key>
    <false/>
    <key>VPages</key>
    <integer>1</integer>
    <key>WindowInfo</key>
    <dict>
        <key>CurrentSheet</key>
        <integer>0</integer>
        <key>ExpandedCanvases</key>
        <array>
            <dict>
                <key>name</key>
                <string>Canvas 1</string>
            </dict>
        </array>
        <key>Frame</key>
        <string>{{373, 4}, {693, 874}}</string>
        <key>ListView</key>
        <true/>
        <key>OutlineWidth</key>
        <integer>142</integer>
        <key>RightSidebar</key>
        <false/>
        <key>ShowRuler</key>
        <true/>
        <key>Sidebar</key>
        <true/>
        <key>SidebarWidth</key>
        <integer>120</integer>
        <key>VisibleRegion</key>
        <string>{{0, 0}, {558, 720}}</string>
        <key>Zoom</key>
        <real>1</real>
        <key>ZoomValues</key>
        <array>
            <array>
                <string>Canvas 1</string>
                <real>1</real>
                <real>1</real>
            </array>
        </array>
    </dict>
</dict>
</plist>
");
        }

        static int DisplayGoal (Goal g)
        {
            var id = random.Next();

            bool assignedToSoftwareAgents = (from a in g.AssignedAgents select a.Software == true).Count () > 0;

            string str = @"
        <dict>
            <key>Bounds</key>
            <string>{{126.4205322265625, 122.99999809265137}, {88.501709000000005, 20.5}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FitText</key>
            <string>Vertical</string>
            <key>Flow</key>
            <string>Resize</string>
            <key>FontInfo</key>
            <dict>
                <key>Color</key>
                <dict>
                    <key>w</key>
                    <string>0</string>
                </dict>
                <key>Font</key>
                <string>ArialMT</string>
                <key>NSKern</key>
                <real>0.0</real>
                <key>Size</key>
                <real>8</real>
            </dict>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Shape</key>
            <string>Bezier</string>
            <key>ShapeData</key>
            <dict>
                <key>UnitPoints</key>
                <array>
                    <string>{-0.46320438000000003, -0.49998664999999998}</string>
                    <string>{-0.46320438000000003, -0.5}</string>
                    <string>{0.49996470999999998, -0.5}</string>
                    <string>{0.5, -0.5}</string>
                    <string>{0.50002289, -0.5}</string>
                    <string>{0.45904254999999999, 0.5}</string>
                    <string>{0.45904254999999999, 0.5}</string>
                    <string>{0.45905972, 0.49998664999999998}</string>
                    <string>{-0.5, 0.5}</string>
                    <string>{-0.5, 0.5}</string>
                    <string>{-0.5, 0.5}</string>
                    <string>{-0.46320438000000003, -0.5}</string>
                </array>
            </dict>
            <key>Style</key>
            <dict>
                <key>fill</key>
                <dict>
                    <key>Color</key>";

            if (!assignedToSoftwareAgents) {
                str += @"
                    <dict>
                        <key>b</key>
                        <string>1</string>
                        <key>g</key>
                        <string>0.896814</string>
                        <key>r</key>
                        <string>0.810871</string>
                    </dict>";
            } else {
                str += @"
                    <dict>
                        <key>b</key>
                        <string>0.672223</string>
                        <key>g</key>
                        <string>0.979841</string>
                        <key>r</key>
                        <string>1</string>
                    </dict>";
            }
            str += @"
                </dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>";

            if (g.AssignedAgents.Count > 0) {
                str += @"
                <key>stroke</key>
                <dict>
                <key>Width</key>
                <real>2</real>
                </dict>";
            }

            str += @"
            </dict>
            <key>Text</key>
            <dict>
                <key>Text</key>
                <string>{\rtf1\ansi\ansicpg1252\cocoartf1138\cocoasubrtf470
{\fonttbl\f0\fswiss\fcharset0 ArialMT;}
{\colortbl;\red255\green255\blue255;}
\pard\tx560\tx1120\tx1680\tx2240\tx2800\tx3360\tx3920\tx4480\tx5040\tx5600\tx6160\tx6720\pardirnatural\qc

\f0\fs16 \cf0 \expnd0\expndtw0\kerning0
" + g.Name + @"}</string>
            </dict>
        </dict>";
            
            Console.WriteLine (str);

            foreach (var refinement in g.Refinements)
                DisplayRefinement (refinement, id);

            foreach (var agent in g.AssignedAgents)
                DisplayAgent (agent, id);

            foreach (var obstacle in g.Obstruction) {
                int o = DisplayObstacle (obstacle);
                DisplayArrow (o, id);
            }

            return id;
        }

        static void DisplayAgent (Agent a, int parent)
        {
            var id = random.Next();
            var str = @"
        <dict>
            <key>Bounds</key>
            <string>{{114, 87.885440000000017}, {33.499878000000002, 11.5}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FitText</key>
            <string>Vertical</string>
            <key>Flow</key>
            <string>Resize</string>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Shape</key>
            <string>Bezier</string>
            <key>ShapeData</key>
            <dict>
                <key>UnitPoints</key>
                <array>
                    <string>{-0.36199283999999998, -0.49997330000000001}</string>
                    <string>{-0.36199283999999998, -0.5}</string>
                    <string>{0.35809898000000001, -0.5}</string>
                    <string>{0.35825062000000002, -0.5}</string>
                    <string>{0.35825062000000002, -0.5}</string>
                    <string>{0.50000571999999999, 0}</string>
                    <string>{0.5, 0}</string>
                    <string>{0.50000571999999999, 0}</string>
                    <string>{0.35800075999999997, 0.5}</string>
                    <string>{0.35803223000000001, 0.5}</string>
                    <string>{0.35812187000000001, 0.49997330000000001}</string>
                    <string>{-0.35821056000000001, 0.5}</string>
                    <string>{-0.35821056000000001, 0.5}</string>
                    <string>{-0.35821056000000001, 0.5}</string>
                    <string>{-0.5, 0}</string>
                    <string>{-0.5, 0}</string>
                    <string>{-0.5, 0}</string>
                    <string>{-0.36199283999999998, -0.5}</string>
                </array>
            </dict>
            <key>Style</key>
            <dict>
                <key>fill</key>
                <dict>
                    <key>Color</key>
                    <dict>
                        <key>b</key>
                        <string>1</string>
                        <key>g</key>
                        <string>0.670259</string>
                        <key>r</key>
                        <string>0.824276</string>
                    </dict>
                </dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>
            </dict>
            <key>Text</key>
            <dict>
                <key>Pad</key>
                <integer>10</integer>
                <key>Text</key>
                <string>{\rtf1\ansi\ansicpg1252\cocoartf1138\cocoasubrtf470
{\fonttbl\f0\fswiss\fcharset0 ArialMT;}
{\colortbl;\red255\green255\blue255;}
\pard\tx560\tx1120\tx1680\tx2240\tx2800\tx3360\tx3920\tx4480\tx5040\tx5600\tx6160\tx6720\pardirnatural\qc

\f0\fs16 \cf0 " + a.Name + @"}</string>
            </dict>
            <key>Wrap</key>
            <string>NO</string>
        </dict>

";
            Console.WriteLine (str);

            var circle_id = random.Next();
            str = @"
        <dict>
            <key>Bounds</key>
            <string>{{276.16535494999999, 357.16535494999999}, {5.6692901000000004, 5.6692901000000004}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FontInfo</key>
            <dict>
                <key>Color</key>
                <dict>
                    <key>b</key>
                    <string>0.835294</string>
                    <key>g</key>
                    <string>0.556863</string>
                    <key>r</key>
                    <string>0.333333</string>
                </dict>
                <key>Font</key>
                <string>TimesNewRomanPSMT</string>
                <key>Size</key>
                <real>8</real>
            </dict>
            <key>ID</key>
            <integer>" + circle_id + @"</integer>
            <key>Shape</key>
            <string>Circle</string>
            <key>Style</key>
            <dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>
            </dict>
            <key>Text</key>
            <dict>
                <key>Pad</key>
                <integer>10</integer>
            </dict>
            <key>VFlip</key>
            <string>YES</string>
        </dict>";
            Console.WriteLine (str);

            DisplayArrow (circle_id, parent);
            DisplayLine (id, circle_id);
        }
        
        static void DisplayRefinement (GoalRefinement r, int parent)
        {
            var id = random.Next();
            var str = @"
        <dict>
            <key>Bounds</key>
            <string>{{276.16535494999999, 357.16535494999999}, {5.6692901000000004, 5.6692901000000004}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FontInfo</key>
            <dict>
                <key>Color</key>
                <dict>
                    <key>b</key>
                    <string>0.835294</string>
                    <key>g</key>
                    <string>0.556863</string>
                    <key>r</key>
                    <string>0.333333</string>
                </dict>
                <key>Font</key>
                <string>TimesNewRomanPSMT</string>
                <key>Size</key>
                <real>8</real>
            </dict>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Shape</key>
            <string>Circle</string>
            <key>Style</key>
            <dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>
            </dict>
            <key>Text</key>
            <dict>
                <key>Pad</key>
                <integer>10</integer>
            </dict>
            <key>VFlip</key>
            <string>YES</string>
        </dict>";
            Console.WriteLine (str);
            DisplayArrow (id, parent);
            
            foreach (var goal in r.Children) {
                var g = DisplayGoal (goal);
                
                DisplayLine (g, id);
            }
        }
        
        static void DisplayRefinement (ObstacleRefinement r, int parent)
        {
            var id = random.Next();
            var str = @"
        <dict>
            <key>Bounds</key>
            <string>{{276.16535494999999, 357.16535494999999}, {5.6692901000000004, 5.6692901000000004}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FontInfo</key>
            <dict>
                <key>Color</key>
                <dict>
                    <key>b</key>
                    <string>0.835294</string>
                    <key>g</key>
                    <string>0.556863</string>
                    <key>r</key>
                    <string>0.333333</string>
                </dict>
                <key>Font</key>
                <string>TimesNewRomanPSMT</string>
                <key>Size</key>
                <real>8</real>
            </dict>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Shape</key>
            <string>Circle</string>
            <key>Style</key>
            <dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>
            </dict>
            <key>Text</key>
            <dict>
                <key>Pad</key>
                <integer>10</integer>
            </dict>
            <key>VFlip</key>
            <string>YES</string>
        </dict>";
            Console.WriteLine (str);
            DisplayArrow (id, parent);
            
            foreach (var obstacle in r.Children) {
                var g = DisplayObstacle (obstacle);
                
                DisplayLine (g, id);
            }
        }

        static void DisplayArrow (int @from, int to)
        {
            var id = random.Next();
            var str = @"
        <dict>
            <key>Class</key>
            <string>LineGraphic</string>
            <key>FontInfo</key>
            <dict>
            <key>Font</key>
            <string>ArialMT</string>
            <key>Size</key>
            <real>6</real>
            </dict>
            <key>Head</key>
            <dict>
            <key>ID</key>
            <integer>" + to + @"</integer>
            </dict>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Points</key>
            <array>
            <string>{277.47208419167532, 357.03513732100595}</string>
            <string>{227.36256086024207, 259.79950772800538}</string>
            </array>
            <key>Style</key>
            <dict>
            <key>stroke</key>
            <dict>
            <key>HeadArrow</key>
            <string>FilledArrow</string>
            <key>HeadScale</key>
            <real>0.5</real>
            <key>Legacy</key>
            <true/>
            <key>TailArrow</key>
            <string>0</string>
            </dict>
            </dict>
            <key>Tail</key>
            <dict>
            <key>ID</key>
            <integer>" + from + @"</integer>
            </dict>
        </dict>";
            Console.WriteLine (str);
        }

        static void DisplayLine (int @from, int to)
        {
            var id = random.Next();
            var str = @"
        <dict>
            <key>Class</key>
            <string>LineGraphic</string>
            <key>FontInfo</key>
            <dict>
            <key>Font</key>
            <string>ArialMT</string>
            <key>Size</key>
            <real>6</real>
            </dict>
            <key>Head</key>
            <dict>
            <key>ID</key>
            <integer>" + to + @"</integer>
            </dict>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Points</key>
            <array>
            <string>{277.47208419167532, 357.03513732100595}</string>
            <string>{227.36256086024207, 259.79950772800538}</string>
            </array>
            <key>Style</key>
            <dict>
            <key>stroke</key>
            <dict>
            <key>HeadArrow</key>
            <string>0</string>
            <key>HeadScale</key>
            <real>0.5</real>
            <key>Legacy</key>
            <true/>
            <key>TailArrow</key>
            <string>0</string>
            </dict>
            </dict>
            <key>Tail</key>
            <dict>
            <key>ID</key>
            <integer>" + from + @"</integer>
            </dict>
        </dict>";
            Console.WriteLine (str);
        }

        static int DisplayObstacle (Obstacle o)
        {
            var id = random.Next();
                        
            string str = @"
<dict>
            <key>Bounds</key>
            <string>{{133, 128}, {62.501465000000003, 20.500243999999999}}</string>
            <key>Class</key>
            <string>ShapedGraphic</string>
            <key>FitText</key>
            <string>Vertical</string>
            <key>Flow</key>
            <string>Resize</string>
            <key>FontInfo</key>
            <dict>
                <key>Color</key>
                <dict>
                    <key>w</key>
                    <string>0</string>
                </dict>
                <key>Font</key>
                <string>ArialMT</string>
                <key>NSKern</key>
                <real>0.0</real>
                <key>Size</key>
                <real>8</real>
            </dict>
            <key>HFlip</key>
            <string>YES</string>
            <key>ID</key>
            <integer>" + id + @"</integer>
            <key>Shape</key>
            <string>Bezier</string>
            <key>ShapeData</key>
            <dict>
                <key>UnitPoints</key>
                <array>
                    <string>{-0.45797824999999998, -0.49999714000000001}</string>
                    <string>{-0.45797824999999998, -0.5}</string>
                    <string>{0.49995278999999998, -0.5}</string>
                    <string>{0.5, -0.5}</string>
                    <string>{0.49999976000000002, -0.5}</string>
                    <string>{0.45321059000000002, 0.5}</string>
                    <string>{0.45322131999999998, 0.5}</string>
                    <string>{0.45325375000000001, 0.49998282999999999}</string>
                    <string>{-0.50000071999999995, 0.5}</string>
                    <string>{-0.5, 0.5}</string>
                    <string>{-0.50000095, 0.5}</string>
                    <string>{-0.45797824999999998, -0.5}</string>
                </array>
            </dict>
            <key>Style</key>
            <dict>
                <key>fill</key>
                <dict>
                    <key>Color</key>
                    <dict>
                        <key>b</key>
                        <string>0.611992</string>
                        <key>g</key>
                        <string>0.590278</string>
                        <key>r</key>
                        <string>1</string>
                    </dict>
                </dict>
                <key>shadow</key>
                <dict>
                    <key>Draws</key>
                    <string>NO</string>
                </dict>";

            if (o.Refinements.Count() == 0) {
                str += @"
                <key>stroke</key>
                <dict>
                    <key>Width</key>
                    <real>2</real>
                </dict>";
            }

            str += @"</dict>
            <key>Text</key>
            <dict>
                <key>Text</key>
                <string>{\rtf1\ansi\ansicpg1252\cocoartf1138\cocoasubrtf470
{\fonttbl\f0\fswiss\fcharset0 ArialMT;}
{\colortbl;\red255\green255\blue255;}
\pard\tx560\tx1120\tx1680\tx2240\tx2800\tx3360\tx3920\tx4480\tx5040\tx5600\tx6160\tx6720\pardirnatural\qc

\f0\fs16 \cf0 \expnd0\expndtw0\kerning0
" + o.Name + @"}</string>
            </dict>
        </dict>";
            
            Console.WriteLine (str);
            
            foreach (var refinement in o.Refinements)
                DisplayRefinement (refinement, id);
                        
            return id;
        }


        static GoalModel BuildModel (string filename)
        {
            var parser = new KAOSFormalTools.Parsing.Parser ();
            return parser.Parse (File.ReadAllText (filename));
        }
        
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: KAOSFormalTools.OmnigraffleExport model");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("KAOSFormalTools.OmnigraffleExport: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `KAOSFormalTools.OmnigraffleExport --help' for more information.");
            Console.ResetColor ();
        }
    }
}
