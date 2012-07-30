using System;
using NUnit.Framework;
using Beaver.Domain;

namespace Beaver.Tests
{
    [TestFixture()]
    public class TestLAS
    {
        [Test()]
        public void BuildGoalModel ()
        {
            var model = new GoalModel ();

            model.Add (new Goal ("Achieve [AmbulanceOnScene When IncidentReported]", "G (incidentReported -> F ambulanceOnScene)"));

            model.Add (new Goal ("Achieve [AmbulanceAllocated When IncidentReported]", "G (incidentReported -> F ambulanceAllocated)"));
            model.Add (new Goal ("Achieve [AmbulanceOnScene When AmbulanceAllocated]", "G (ambulanceAllocated -> F ambulanceOnScene)"));
            model.Add (new GoalRefinement ("Achieve [AmbulanceOnScene When IncidentReported]",
                                           "Achieve [AmbulanceAllocated When IncidentReported]",
                                           "Achieve [AmbulanceOnScene When AmbulanceAllocated]"));

            model.Add (new Goal ("Achieve [AmbulanceMobilized When AmbulanceAllocated]", "G (ambulanceAllocated -> F ambulanceMobilized)"));
            model.Add (new Goal ("Achieve [AmbulanceOnScene When AmbulanceMobilized]", "G (ambulanceMobilized -> F ambulanceOnScene)"));
            model.Add (new GoalRefinement ("Achieve [AmbulanceOnScene When IncidentReported]",
                                           "Achieve [AmbulanceAllocated When IncidentReported]",
                                           "Achieve [AmbulanceOnScene When AmbulanceAllocated]"));

            model.Add (new Goal ("Achieve [AmbulanceMobilized When AmbulanceAllocatedOnRoad]", "G (ambulanceAllocated & onRoad -> F ambulanceMobilized)"));
            model.Add (new Goal ("Achieve [AmbulanceMobilized When AmbulanceAllocatedAtStation]", "G (ambulanceAllocated & !onRoad -> F ambulanceMobilized)"));
            model.Add (new GoalRefinement ("Achieve [AmbulanceOnScene When IncidentReported]",
                                           "Achieve [AmbulanceAllocated When IncidentReported]",
                                           "Achieve [AmbulanceOnScene When AmbulanceAllocated]"));

            model.Add (new Goal ("Achieve [AmbulanceMobilizedByFax When AmbulanceAllocatedAtStation]", "G (ambulanceAllocated & !onRoad -> F ambulanceMobilizedByFax)"));
            model.Add (new Goal ("Achieve [AmbulanceMobilizedByPhone When AmbulanceAllocatedAtStation]", "G (ambulanceAllocated & !onRoad -> F ambulanceMobilizedByPhone)"));
            model.Add (new DomainProperty ("Ambulance Mobilized By Fax or Phone", "G ((ambulanceMobilizedByFax | ambulanceMobilizedByPhone) -> ambulanceMobilized)"));
            model.Add (new GoalRefinement ("Achieve [AmbulanceOnScene When IncidentReported]",
                                           "Achieve [AmbulanceAllocated When IncidentReported]",
                                           "Achieve [AmbulanceOnScene When AmbulanceAllocated]"));

            Console.WriteLine (string.Join (", ", model.GetAlphabet ()));
        }
    }
}

