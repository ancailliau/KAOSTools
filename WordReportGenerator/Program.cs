using System;
using KAOSTools.Utils;
using Novacode;
using KAOSTools.MetaModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Drawing;
using ObstaclePriotirizer;

namespace WordReportGenerator
{
    class MainClass: KAOSToolCLI
    {
        public static void Main(string[] args)
        {
            Init(args);

            Console.WriteLine("Hello World!");

            Example01Main(model);
        }

        static void Example01Main(KAOSModel model)
        {
            var filename = "Example01.docx";

            var cp = new CPSCalculator(model);
            cp.ComputeCPS();

            using (DocX document = DocX.Create (filename, DocumentTypes.Document)) {

                document.AddHeaders();
                Header header_default = document.Headers.odd;
                Paragraph p1 = header_default.InsertParagraph();
                p1.Alignment = Alignment.right;
                p1.Append("Version: " + model.Version);

                var t1 = document.InsertParagraph();
                t1.Append(model.Title);
                t1.FontSize(20);
                t1.Bold();
                t1.Alignment = Alignment.center;

                t1 = document.InsertParagraph();
                t1.Alignment = Alignment.center;
                t1.Append(model.Author);

                ExportGoals(model, document);
                ExportObstacles(model, document);
                ExportCriticalObstacles(model, document, 20);

                document.Save();
            }

            using (FileStream zipToOpen = new FileStream(filename, FileMode.Open)) {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update)) {
                    var word_rels = archive.GetEntry("word/_rels/document.xml.rels");
                    string s;
                    using (StreamReader sr = new StreamReader(word_rels.Open())) {
                        s = sr.ReadToEnd();
                    }
					s = s.Replace("file://", "");
                    word_rels.Delete();

                    word_rels = archive.CreateEntry("word/_rels/document.xml.rels");
                    using (StreamWriter sw = new StreamWriter(word_rels.Open())) {
                        sw.Write (s);
                    }
                }
            }

        }

        static void ExportCriticalObstacles(KAOSModel model, DocX document, int v)
        {
            var ec = new EffectCalculator(model);

            var t1 = document.InsertParagraph();
            t1.Append("Critical combinations");
            t1.StyleName = "Heading1";


            foreach (var root in model.RootGoals()) {

                t1 = document.InsertParagraph();
                t1.Append("Critical combinations regarding " + root.FriendlyName);
                t1.StyleName = "Heading2";

                var e = ec.GetEffects(root);

                t1 = document.InsertParagraph();
                t1.Append("This section presents the combinations of obstacles that " +
                          "cause the goal " + root.FriendlyName + " to be violated. ");
                
                t1.Append("This section only presents the " + v 
                          + " smallest combination maximizing the violation. The tool found " + e.Count() + " combinations " +
                          "of obstacles causing the goal " + root.FriendlyName + " to be violated.");
                
				int i = 1;
                foreach (var ee in e.OrderBy(x => x.Key.Count).ThenBy(x => x.Value).Take (v)) {
                    t1 = document.InsertParagraph();
                    t1.Append("Critical combination " + (i++));
                    t1.StyleName = "Heading3";

                    t1 = document.InsertParagraph();
                    t1.Append(string.Format ("The following combination causes the goal {0} " +
                                             "to have a computed probability of satisfaction of {1:0.####}%.", root.FriendlyName, ee.Value * 100));
                    
                    var l = document.AddList(listType: ListItemType.Bulleted);
                    foreach (var obstacle in ee.Key.OrderBy (x => x.EPS)) {
                        document.AddListItem(l, string.Format ("{0} (Estimated probability: {1:0.####}%)", obstacle.FriendlyName, obstacle.EPS * 100));
                    }
                    document.InsertList(l);
                }
            }
        }

        static void ExportGoals(KAOSModel model, DocX document)
        {
            Paragraph t1;
            List l;

            t1 = document.InsertParagraph();
            t1.Append("Goals");
            t1.StyleName = "Heading1";

            Console.WriteLine("Generating goals");

            foreach (var g in model.Goals().OrderBy (x => x.FriendlyName)) {
                Console.WriteLine("Generating " + g.FriendlyName);
                var p = document.InsertParagraph();
                p.StyleName = "Heading2";
                p.Append(g.FriendlyName);

                p = document.InsertParagraph();
                if (string.IsNullOrEmpty(g.Definition)) {
                    p.Append("Definition missing");
                    p.Color(Color.Red);
                } else {
                    p.Append(g.Definition);
                }

                p = document.InsertParagraph();
                p.Append("Computed probability:").Bold();
                var p2 = p.Append(string.Format(" {0:0.####}%", g.CPS * 100));

                if (g.Refinements().Count() > 0) {
                    p = document.InsertParagraph();
                    p.StyleName = "Heading3";
                    p.Append("Refined by");

                    foreach (var gg in g.Refinements()) {
                        l = document.AddList(listType: ListItemType.Bulleted);
                        foreach (var ggg in gg.SubGoals()) {
                            document.AddListItem(l, ggg.FriendlyName);
                        }
                        foreach (var ggg in gg.DomainProperties()) {
                            document.AddListItem(l, ggg.FriendlyName);
                        }
                        document.InsertList(l);
                    }
                }

                if (g.Obstructions().Count() > 0) {
                    p = document.InsertParagraph();
                    p.StyleName = "Heading3";
                    p.Append("Obstructed by");

                    l = document.AddList(listType: ListItemType.Bulleted);
                    foreach (var gg in g.Obstructions()) {
                        document.AddListItem(l, gg.Obstacle().FriendlyName);
                        document.InsertList(l);
                    }
                }
            }
        }

        static void ExportObstacles(KAOSModel model, DocX document)
        {
            List l;
            Paragraph t1;
            Console.WriteLine("Generating obstacles");

            t1 = document.InsertParagraph();
            t1.Append("Obstacles");
            t1.StyleName = "Heading1";

            foreach (var g in model.Obstacles().OrderBy(x => x.FriendlyName)) {
                Console.WriteLine("Generating " + g.FriendlyName);
                var p = document.InsertParagraph();
                p.StyleName = "Heading2";
                p.Append(g.FriendlyName);

                //p = document.InsertParagraph();
                //p.StyleName = "Heading3";
                //p.Append("Definition");

                p = document.InsertParagraph();
                if (string.IsNullOrWhiteSpace(g.Definition)) {
                    p.Append("Definition missing");
                    p.Color(Color.Red);
                } else {
                    p.Append(g.Definition);
                }

                if (g.Refinements().Count() > 0) {
                    p = document.InsertParagraph();
                    p.Append("Computed probability:").Bold();
                    var p2 = p.Append(string.Format(" {0:0.####}%", g.CPS * 100));

                    int i = 1;
                    foreach (var gg in g.Refinements()) {
                        p = document.InsertParagraph();
                        p.StyleName = "Heading3";
                        if (g.Refinements().Count() == 1) {
                            p.Append("Refinement");
                        } else {
                            p.Append("Alternative Refinement " + (i++));
                        }

                        l = document.AddList(listType: ListItemType.Bulleted);
                        foreach (var ggg in gg.SubObstacles()) {
                            document.AddListItem(l, ggg.FriendlyName);
                        }
                        foreach (var ggg in gg.DomainProperties()) {
                            document.AddListItem(l, ggg.FriendlyName);
                        }
                        document.InsertList(l);
                    }
                } else {
                    p = document.InsertParagraph();
                    p.Append("Estimated probability:").Bold();
                    var p2 = p.Append(string.Format(" {0:0.####}%", g.EPS * 100));
                    if (g.EPS == 0) {
                        p2.Color(Color.Red);
                        p2.Highlight(Highlight.yellow);
                    }
                }

                if (g.Resolutions().Count() > 0) {
                    p = document.InsertParagraph();
                    p.StyleName = "Heading3";
                    p.Append("Resolved by");

                    l = document.AddList(listType: ListItemType.Bulleted);
                    foreach (var gg in g.Resolutions()) {
                        document.AddListItem(l, gg.ResolvingGoal().FriendlyName);
                        document.InsertList(l);
                    }
                }
            }
        }
   }
}
