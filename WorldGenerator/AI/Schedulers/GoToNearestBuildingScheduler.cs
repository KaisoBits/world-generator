﻿namespace WorldGenerator.AI;

public class GoToNearestBuildingScheduler : Scheduler
{
    public override IEnumerable<ISchedulerTask> GetTasks()
    {
        yield return new FindEntityPosition(this, Layer.Buildings, "targetPosition", false);
        yield return new ApproachPosition(this, (Position)Recall("targetPosition")!);
    }
}
