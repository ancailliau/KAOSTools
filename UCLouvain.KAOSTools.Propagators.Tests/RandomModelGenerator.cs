using System;
using KAOSTools.Core;
using MathNet.Numerics.Distributions;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using UCLouvain.KAOSTools.Core.SatisfactionRates;

namespace UCLouvain.KAOSTools.Propagators.Tests
{
    public class RandomModelGenerator
    {
        readonly RandomModelOptions _options;
        readonly KAOSModel _model;
        readonly Random _random;
        readonly Faker _faker;

        Goal root;
        
        public RandomModelGenerator () : this (new RandomModelOptions ())
        {}

        public RandomModelGenerator (RandomModelOptions options)
        {
            this._options = options;
            _model = new KAOSModel ();
            _random = new Random ();
            _faker = new Faker ();
        }

        public KAOSModel Generate ()
        {
            root = GenerateGoal (); 
            
            var goalToRefine = new Stack<Tuple<Goal,int>> (new [] { new Tuple<Goal,int> (root, 0) });
            var obstacleToRefine = new Stack<Tuple<Obstacle,int>> ();
            var goalToObstruct = new Stack<Goal> ();
            
            while (goalToRefine.Count > 0) {
                var current = goalToRefine.Pop ();
                var r = GenerateGoalRefinement (current.Item1);
                if (current.Item2 < _options.GoalMaxHeight) {
                    foreach (var sg in r.SubGoals ())
                        goalToRefine.Push (new Tuple<Goal,int> (sg, current.Item2 + 1));
                } else {
                    foreach (var sg in r.SubGoals ()) {
                        if (_random.NextDouble () < _options.ObstructionThreshold) {
                            goalToObstruct.Push (sg);
                        }
                    }
                }
            }
            
            while (goalToObstruct.Count > 0) {
                var current = goalToObstruct.Pop ();
                var r = GenerateObstruction (current);
                obstacleToRefine.Push (new Tuple<Obstacle, int> (r.Obstacle (), 0));
            }
            
            while (obstacleToRefine.Count > 0) {
                var current = obstacleToRefine.Pop ();
                var r = GenerateObstacleRefinement (current.Item1);
                if (current.Item2 < _options.ObstacleMaxHeight) {
                    foreach (var sg in r.SubObstacles ())
                        obstacleToRefine.Push (new Tuple<Obstacle,int> (sg, current.Item2 + 1));
                } else {
                    foreach (var sg in r.SubObstacles ())
                        _model.satisfactionRateRepository.AddObstacleSatisfactionRate (sg.Identifier,
                            new DoubleSatisfactionRate (_random.NextDouble ()));
                }
            }

            return _model;
        }
        
        Goal GenerateGoal ()
        {
            Goal goal = new Goal (_model, "goal-" + Guid.NewGuid ().ToString ());
            _model.Add (goal);
            return goal;
        }
        
        Obstacle GenerateObstacle ()
        {
            Obstacle obstacle = new Obstacle (_model, "obstacle-" + Guid.NewGuid ().ToString ());
            _model.Add (obstacle);
            return obstacle;
        }

        GoalRefinement GenerateGoalRefinement (Goal parent)
        {
            var mode = (_options.MinGoalBranchingFactor + _options.MaxGoalBranchingFactor)/2;
            var nbchild = Triangular.Sample (_random, _options.MinGoalBranchingFactor, 
                                          _options.MaxGoalBranchingFactor, 
                                          mode);

            var refinement = new GoalRefinement (_model) {
                ParentGoalIdentifier = parent.Identifier
            };
            do {
                refinement.RefinementPattern = _faker.PickRandom<RefinementPattern> ();
            } while (refinement.RefinementPattern == RefinementPattern.None
                     || refinement.RefinementPattern == RefinementPattern.Case);

            for (int i = 0; i < nbchild; i++) {
                var c = GenerateGoal ();
                refinement.Add (c);
            }
            _model.Add (refinement);
            return refinement;
        }

        Obstruction GenerateObstruction (Goal parent)
        {
            var obstruction = new Obstruction (_model);
            obstruction.ObstructedGoalIdentifier = parent.Identifier;

            var obstacle = GenerateObstacle ();
            obstruction.ObstacleIdentifier = obstacle.Identifier;
            
            _model.Add (obstruction);
            return obstruction;
        }

        ObstacleRefinement GenerateObstacleRefinement (Obstacle parent)
        {
            var mode = (_options.MinObstacleANDBranchingFactor + _options.MaxObstacleANDBranchingFactor) / 2;
            var nbchild = Triangular.Sample (_random, _options.MinObstacleANDBranchingFactor, 
                                          _options.MaxObstacleANDBranchingFactor, 
                                          mode);

            var refinement = new ObstacleRefinement (_model) {
                ParentObstacleIdentifier = parent.Identifier
            };
            for (int i = 0; i < nbchild; i++) {
                var c = GenerateObstacle ();
                refinement.Add (c);
            }
            _model.Add (refinement);
            return refinement;
        }
        
        int GetHeight ()
        {
            return GetHeight (root);
        }
        
        int GetHeight (Goal g)
        {
            if (g.Refinements ().Count () > 0)
                return 1 + g.Refinements ().Select (x => x.SubGoals ().Select (y => GetHeight (y)).Max ()).Max ();
            else
                return 1;
        }
    }
    
    public class RandomModelOptions
    {
        public int GoalMaxHeight = 2;
        public int MinGoalBranchingFactor = 1;
        public int MaxGoalBranchingFactor = 2;

        public double ObstructionThreshold = .3;
        
        public int ObstacleMaxHeight = 2;
        public int MinObstacleANDBranchingFactor = 1;
        public int MaxObstacleANDBranchingFactor = 2;
        
        public int MinObstacleORBranchingFactor = 1;
        public int MaxObstacleORBranchingFactor = 4;
    }
}
