// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.QuestMachine
{

    public class Plan
    {

        public List<PlanStep> steps; // Steps taken.
        public WorldModel worldModel; // Resulting world model.

        public Plan(Plan plan, PlanStep step, WorldModel worldModel)
        {
            this.steps = new List<PlanStep>();
            if (plan != null) this.steps.AddRange(plan.steps);
            if (step != null) steps.Add(step);
            this.worldModel = worldModel;
        }

        public override string ToString()
        {
            var s = string.Empty;
            for (int i = 0; i < steps.Count; i++)
            {
                s += steps[i] + ", ";
            }
            s += ">";
            return s;
        }
    }

}
