using System;
using System.Collections.Generic;
using System.Threading;
using ReGoap.Core;
using ReGoap.Planner;
using ReGoap.Utilities;
using UnityEngine;

namespace ReGoap.Unity
{ // every thread runs on one of these classes
    public class ReGoapPlannerThread<T, W>
    {
        private readonly ReGoapPlanner<T, W> planner;
        public static Queue<ReGoapPlanWork<T, W>> WorksQueue;
        private bool isRunning = true;
        private readonly Action<ReGoapPlannerThread<T, W>, ReGoapPlanWork<T, W>, IReGoapGoal<T, W>> onDonePlan;

        public ReGoapPlannerThread(ReGoapPlannerSettings plannerSettings, Action<ReGoapPlannerThread<T, W>, ReGoapPlanWork<T, W>, IReGoapGoal<T, W>> onDonePlan)
        {
            planner = new ReGoapPlanner<T, W>(plannerSettings);
            this.onDonePlan = onDonePlan;
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void MainLoop()
        {
            while (isRunning)
            {
                CheckWorkers();
                Thread.Sleep(0);
            }
        }

        public void CheckWorkers()
        {
            if (WorksQueue.Count > 0)
            {
                ReGoapPlanWork<T, W> checkWork;
                lock (WorksQueue)
                {
                    checkWork = WorksQueue.Dequeue();
                }
                var work = checkWork;
                planner.Plan(work.Agent, work.BlacklistGoal, work.Actions,
                    (newGoal) => onDonePlan(this, work, newGoal));
            }
        }
    }
}