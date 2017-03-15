using System;
using ExcelExport;
using System.IO;
using OfficeOpenXml;
using KAOSTools.MetaModel;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ExcelExport
{
    class MainClass : KAOSTools.Utils.KAOSToolCLI
    {
        static Color blue0 = Color.FromArgb (79,129,189);
        static Color blue1 = Color.FromArgb (219,229,241);
        static Color blue2 = Color.FromArgb (184,204,228);
        static Color blue3 = Color.FromArgb (149,179,215);
        static Color blue4 = Color.FromArgb (55,96,145);
        static Color blue5 = Color.FromArgb (37,64,97);


        static void SetSpacer (ExcelRow row)
        {
            row.Height = 10;
        }

        static void SetInput (ExcelRange range)
        {
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor (blue1);
        }

        static void SetFormatPercentage (ExcelRange range)
        {
            range.Style.Numberformat.Format ="0.00%";
        }

        static void SetTitleFormat (ExcelRange range)
        {
            range.Style.Font.Size = 13;
            range.Style.Font.Name = "Calibri";
            range.Style.Font.Bold = true;
            range.Style.Font.Color.SetColor (blue0);

            range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
            range.Style.Border.Bottom.Color.SetColor (blue2);

        }

        static Dictionary<string, int> obstacleCache;

        static Dictionary<string, int> goalCache;

        static int startIndexGoals;

        static Dictionary<string, int> constraintCache;

        static int startIndexConstraint;

        static int startIndexObstacle;

        static Dictionary<string, int> cmCache;

        static Dictionary<string, int> leafObstacleCache;

        static Dictionary<string, int> rootGoalCache;

        static ExcelWorksheet inputWorksheet;

        static ExcelWorksheet combinationWorksheet;

        static IEnumerable<string> resolutions;

        static Dictionary<string, int> costCache;

        static Dictionary<string, int> variableCostCMCache;

        static Dictionary<string, int> variableCostCache;


        static int currentCol;

        public static void Main (string [] args)
        {
            Init (args);

            Console.WriteLine ("Constraints : ");
            foreach (var e in model.Elements.OfType<Constraint> ()) {
                Console.WriteLine (e.Identifier + " --> " + string.Join (",", e.Conflict) + " - " + string.Join (",", e.Or));
            }

            var filename = @"/Users/acailliau/Desktop/tmp.xlsx";

            var fileinfo = new FileInfo (filename);
            if (fileinfo.Exists) {
                fileinfo.Delete ();
                fileinfo = new FileInfo (filename);
            }

            var pck = new ExcelPackage (fileinfo);

            int cl = 1;
            int endOfInput = 0;

            inputWorksheet = pck.Workbook.Worksheets.Add ("Inputs");

            leafObstacleCache = new Dictionary<string, int> ();
            rootGoalCache = new Dictionary<string, int> ();

            inputWorksheet.Cells [cl, 1].Value = "Leaf obstacle";
            inputWorksheet.Cells [cl, 2].Value = "EPS";
            SetTitleFormat (inputWorksheet.Cells [cl, 1, cl, 2]);

            cl++;
            SetSpacer (inputWorksheet.Row (cl));
            cl++;

            foreach (var o in model.LeafObstacles ()) {
                inputWorksheet.Cells [cl, 1].Value = o.FriendlyName;
                leafObstacleCache.Add (o.Identifier, cl);

                inputWorksheet.Cells [cl, 2].Value = o.EPS;
                SetInput (inputWorksheet.Cells [cl, 2]);
                SetFormatPercentage (inputWorksheet.Cells [cl, 2]);

                cl++;
            }

            cl++;
            inputWorksheet.Cells [cl, 1].Value = "Root goals";
            inputWorksheet.Cells [cl, 2].Value = "RDS";
            SetTitleFormat (inputWorksheet.Cells [cl, 1, cl, 2]);

            cl++;
            SetSpacer (inputWorksheet.Row (cl));

            cl++;
            foreach (var g in model.RootGoals ()) {
                inputWorksheet.Cells [cl, 1].Value = g.FriendlyName;
                inputWorksheet.Cells [cl, 2].Value = g.RDS;
                SetInput (inputWorksheet.Cells [cl, 2]);
                SetFormatPercentage (inputWorksheet.Cells [cl, 2]);
                
                rootGoalCache.Add (g.Identifier, cl);
                cl++;
            }


            cl++;
            inputWorksheet.Cells [cl, 1].Value = "Costs";

            int col = 2;
            variableCostCache = new Dictionary<string, int> ();
            variableCostCMCache = new Dictionary<string, int> ();
            foreach (var variable in model.Elements.OfType<CostVariable> ().Distinct ()) {
                inputWorksheet.Cells [cl, col].Value = (string.IsNullOrWhiteSpace (variable.Name) ? variable.Identifier : variable.Name);
                inputWorksheet.Cells [cl, col].Style.Font.Bold = true;
                variableCostCache.Add (variable.Identifier, col);
                col++;
            }
            SetTitleFormat (inputWorksheet.Cells [cl, 1, cl, col-1]);

            cl++;
            SetSpacer (inputWorksheet.Row (cl));
            cl++;

            foreach (var cmGoal in model.Resolutions().Select (x => x.ResolvingGoal ())) {
                inputWorksheet.Cells [cl, 1].Value = cmGoal.FriendlyName;
                variableCostCMCache.Add (cmGoal.Identifier, cl);
                foreach (var variable in model.Elements.OfType<CostVariable> ()) {
                    if (cmGoal.Costs.ContainsKey (variable)) {
                        inputWorksheet.Cells [cl, variableCostCache[variable.Identifier]].Value = cmGoal.Costs[variable];
                    } else {
                        inputWorksheet.Cells [cl, variableCostCache[variable.Identifier]].Value = 0;
                    }
                    SetInput (inputWorksheet.Cells [cl, variableCostCache[variable.Identifier]]);
                }
                cl++;
            }

            endOfInput = cl;

            inputWorksheet.Cells.AutoFitColumns ();

            combinationWorksheet = pck.Workbook.Worksheets.Add ("Combinations");

            resolutions = model.Resolutions ().Select (r => r.ResolvingGoalIdentifier).Distinct ();
            var nresolution = resolutions.Count ();

            Console.WriteLine (nresolution + " resolution(s) were identified.");

            // List the countermeasures (title for the combinations)
            cl = 1;
            cmCache = new Dictionary<string, int> ();
            foreach (var r in resolutions) {
                combinationWorksheet.Cells [cl, 1].Value = model.Goal(x => x.Identifier == r).FriendlyName;
                cmCache.Add (r, cl);
                cl++;
            }

            // Title for the constraints
            constraintCache = new Dictionary<string, int> ();
            startIndexConstraint = cl + 1;
            cl = startIndexConstraint;
            foreach (var o in model.Elements.OfType<Constraint> ()) {
                combinationWorksheet.Cells [cl, 1].Value = o.FriendlyName;
                constraintCache.Add (o.Identifier, cl);
                cl++;
            }

            // Title for the obstacles
            obstacleCache = new Dictionary<string, int> ();
            startIndexObstacle = cl + 1;

            var currentLineObstacle = startIndexObstacle;
            foreach (var o in model.Obstacles ()) {
                combinationWorksheet.Cells [currentLineObstacle, 1].Value = o.FriendlyName;
                obstacleCache.Add (o.Identifier, currentLineObstacle);
                currentLineObstacle++;
            }

            // Title for the goals
            goalCache = new Dictionary<string, int> ();
            startIndexGoals = currentLineObstacle + 1;
            currentLineObstacle = startIndexGoals;

            foreach (var g in model.Goals ()) {
                combinationWorksheet.Cells [currentLineObstacle, 1].Value = g.FriendlyName;
                goalCache.Add (g.Identifier, currentLineObstacle);
                currentLineObstacle++;
            }

            // Title for cost
            currentLineObstacle++;
            combinationWorksheet.Cells [currentLineObstacle, 1].Value = "Cost";
            costCache = new Dictionary<string, int> ();
            foreach (var variable in model.Elements.OfType<CostVariable> ().Distinct ()) {
                combinationWorksheet.Cells [currentLineObstacle, 1].Value = (string.IsNullOrWhiteSpace (variable.Name) ? variable.Identifier : variable.Name);
                costCache.Add (variable.Identifier, currentLineObstacle);
                currentLineObstacle++;
            }

            int costSumLine = currentLineObstacle;
            combinationWorksheet.Cells [currentLineObstacle, 1].Value = "Sum of costs"; currentLineObstacle++;
            int costSupermumLine = currentLineObstacle;
            combinationWorksheet.Cells [currentLineObstacle, 1].Value = "Supremum of costs"; currentLineObstacle++;

            combinationWorksheet.Column (1).AutoFit ();

            Console.WriteLine ("Generating " + Math.Pow (2, nresolution) + " combinations");
            currentCol = 2;
            PrintCombination (nresolution, printCombinationColumn);
            Console.WriteLine (" (done)");


            var resultWorksheet = pck.Workbook.Worksheets.Add ("Analysis Results");
            pck.Workbook.Worksheets.MoveToStart (resultWorksheet.Index);

            endOfInput = 1;
            resultWorksheet.Cells [endOfInput, 1].Value = "Analysis Results";
            SetTitleFormat (resultWorksheet.Cells [endOfInput, 1, endOfInput, 2]);
            endOfInput++;

            SetSpacer (resultWorksheet.Row (endOfInput));
            endOfInput++;

            resultWorksheet.Cells [endOfInput, 1].Value = "Minimal cost for satisfying RDS"; endOfInput++;
            resultWorksheet.Cells [endOfInput, 1].Value = "- Minimum of sum";
            var aformula = "MIN(IF(1";
            foreach (var RDSGoal in model.RootGoals ()) {
                var rootGoalEPSLine = goalCache [RDSGoal.Identifier];
                aformula += "* (" + combinationWorksheet.Cells [rootGoalEPSLine, 2, rootGoalEPSLine, currentCol - 1].FullAddress + ">=" + inputWorksheet.Cells [rootGoalCache [RDSGoal.Identifier], 2].FullAddress + ")";
            }
            foreach (var c in constraintCache) {
                aformula += "* (" + combinationWorksheet.Cells [c.Value, 2, c.Value, currentCol - 1].FullAddress + ")";
            }
            aformula += "," + combinationWorksheet.Cells [costSumLine, 2, costSumLine, currentCol - 1].FullAddress + "))";
            var minCost = resultWorksheet.Cells [endOfInput, 2];
            minCost.CreateArrayFormula (aformula);
            endOfInput++;

            resultWorksheet.Cells [endOfInput, 1].Value = "- Minimum of supremum";
            aformula = "MIN(IF(1";
            foreach (var RDSGoal in model.RootGoals ()) {
                var rootGoalEPSLine = goalCache [RDSGoal.Identifier];
                aformula += "* (" + combinationWorksheet.Cells [rootGoalEPSLine, 2, rootGoalEPSLine, currentCol - 1].FullAddress + ">=" + inputWorksheet.Cells [rootGoalCache [RDSGoal.Identifier], 2].FullAddress + ")";
            }
            foreach (var c in constraintCache) {
                aformula += "* (" + combinationWorksheet.Cells [c.Value, 2, c.Value, currentCol - 1].FullAddress + ")";
            }
            aformula += "," + combinationWorksheet.Cells [costSupermumLine, 2, costSupermumLine, currentCol - 1].FullAddress + "))";
            minCost = resultWorksheet.Cells [endOfInput, 2];
            minCost.CreateArrayFormula (aformula);
            endOfInput++;

            endOfInput++;

            resultWorksheet.Cells [endOfInput, 1].Value = "Combination (offset) for minimum of supremum";
            aformula = "MATCH(" + minCost.Address + ", IF(1";

            foreach (var RDSGoal in model.RootGoals ()) {
                var rootGoalEPSLine = goalCache [RDSGoal.Identifier];
                aformula += "* (" + combinationWorksheet.Cells [rootGoalEPSLine, 2, rootGoalEPSLine, currentCol - 1].FullAddress + ">=" + inputWorksheet.Cells [rootGoalCache [RDSGoal.Identifier], 2].FullAddress + ")";
            }
            foreach (var c in constraintCache) {
                aformula += "* (" + combinationWorksheet.Cells [c.Value, 2, c.Value, currentCol - 1].FullAddress + ")";
            }
            aformula += "," + combinationWorksheet.Cells [costSupermumLine, 2, costSupermumLine, currentCol - 1].FullAddress + "),0)";
            var offset = resultWorksheet.Cells [endOfInput, 2];
            offset.CreateArrayFormula (aformula);

            endOfInput++;
            endOfInput++;
            resultWorksheet.Cells [endOfInput, 1].Value = "Selected counter-measures";
            endOfInput++;

            foreach (var r in cmCache) {
                resultWorksheet.Cells [endOfInput, 1].Value = " - " + model.Goal(x => x.Identifier == r.Key).FriendlyName;
                resultWorksheet.Cells [endOfInput, 2].Formula = "OFFSET(" + combinationWorksheet.Cells [r.Value, 2].FullAddress + ",0," + offset.Address + "-1)";
                endOfInput++;
            }

            resultWorksheet.Cells.AutoFitColumns ();


            pck.Save ();
            pck.Dispose ();

        }


        static void printCombinationColumn (bool[] array)
        {
            int currentLineObstacle;
            Console.Write (".");

            // Print the combination
            for (int j = 1; j <= array.Length; j++) {
                combinationWorksheet.Cells [j, currentCol].Value = (array[j-1] ? 1 : 0);
            }

            // Print the constraints
            foreach (var o in model.Elements.OfType<Constraint> ()) {
                combinationWorksheet.Cells [constraintCache[o.Identifier], currentCol].Formula = "AND(NOT(AND(" + string.Join (",", o.Conflict.Select (x => combinationWorksheet.Cells [cmCache[x], currentCol].Address)) + ")),OR("+string.Join (",", o.Or.Select (x => combinationWorksheet.Cells [cmCache[x], currentCol].Address))+"))";
                combinationWorksheet.Cells [constraintCache[o.Identifier], currentCol].Calculate ();
                if (!combinationWorksheet.Cells [constraintCache[o.Identifier], currentCol].GetValue<bool> ()) {
                    combinationWorksheet.Cells [1, currentCol, startIndexObstacle, currentCol].Value = "";
                    return;
                }
            }

            // Print the obstacle likelihoods
            currentLineObstacle = startIndexObstacle;
            foreach (var o in model.Obstacles ()) {
                if (o.Refinements ().Count () > 0) {
                    string formula = "1-1";
                    foreach (var r in o.Refinements ()) {
                        formula += "*(1-1";
                        foreach (var so in r.SubobstacleIdentifiers) {
                            formula += "*" + combinationWorksheet.Cells [obstacleCache [so], currentCol].Address;
                        }
                        formula += ")";
                    }
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = formula;

                } else {
                    if (!leafObstacleCache.ContainsKey (o.Identifier)) {
                        Console.WriteLine (o.Identifier + " was not found ");
                        continue;
                    }

                    string formula = inputWorksheet.Cells [leafObstacleCache[o.Identifier], 2].FullAddress;
                    foreach (var r in o.Resolutions ()) {
                        var sg = r.ResolvingGoalIdentifier;
                        formula += "* ( 1 - " + combinationWorksheet.Cells [cmCache [sg], currentCol].Address + " ) ";
                    }
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = formula;
                }
                SetFormatPercentage (combinationWorksheet.Cells [currentLineObstacle, currentCol]);
                currentLineObstacle++;
            }

            // Print goal likelihoods
            currentLineObstacle = startIndexGoals;
            foreach (var g in model.Goals ()) {
                if (g.Refinements ().Count () > 0) {
                    string formula = "1";
                    var r = g.Refinements ().Single ();
                    foreach (var sg in r.SubGoalIdentifiers) {
                        formula += "*" + combinationWorksheet.Cells [goalCache [sg], currentCol].Address;
                    }
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = formula;

                } else if (g.Obstructions ().Count () > 0) {
                    var r = g.Obstructions ().Single ();
                    string formula = "1-1";
                    var sg = r.ObstacleIdentifier;
                    formula += "* "+ combinationWorksheet.Cells [obstacleCache[sg], currentCol].Address;
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = formula;

                } else {
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Style.Fill.BackgroundColor.SetColor (Color.Yellow);
                    combinationWorksheet.Cells [currentLineObstacle, currentCol].Value = 1;

                }
                SetFormatPercentage (combinationWorksheet.Cells [currentLineObstacle, currentCol]);
                currentLineObstacle++;
            }

            currentLineObstacle++;
            combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = "SUM("+combinationWorksheet.Cells[1, currentCol, array.Length, currentCol].Address+")";


            int startCost = currentLineObstacle;
            foreach (var variable in model.Elements.OfType<CostVariable> ().Distinct ()) {
                string f = "0";
                foreach (var r in resolutions) {
                    f += "+ " + combinationWorksheet.Cells [cmCache[r], currentCol].Address + " * " + inputWorksheet.Cells [variableCostCMCache[r], variableCostCache[variable.Identifier]].FullAddress;
                }
                combinationWorksheet.Cells [costCache[variable.Identifier], currentCol].Formula = f;
                currentLineObstacle++;
            }

            if (currentLineObstacle > startCost) {
                combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = "SUM(" + combinationWorksheet.Cells [startCost, currentCol, currentLineObstacle - 1, currentCol] + ")";
                currentLineObstacle++;
                combinationWorksheet.Cells [currentLineObstacle, currentCol].Formula = "MAX(" + combinationWorksheet.Cells [startCost, currentCol, currentLineObstacle - 1, currentCol] + ")";
                currentLineObstacle++;
            }

            combinationWorksheet.Column (currentCol).AutoFit ();
            currentCol++;
        }

        static void PrintCombination (int size, Action<bool[]> del)
        {
            var data = new bool[size];
            PC (data, size, del);
        }

        static void PC (bool[] data, int index, Action<bool[]> del)
        {
            if (index == 0) {
                del (data);
                return;
            }

            data [index - 1] = false;
            PC (data, index - 1, del);

            data [index - 1] = true;
            PC (data, index - 1, del);
        }

    }
}
