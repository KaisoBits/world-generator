﻿using WorldGenerator.AI;
using WorldGenerator.Traits;

namespace WorldGenerator;

public class JobOrderManager
{
    private readonly Queue<IIntent> _orders = [];

    private readonly HashSet<WorkerTrait> _freeWorkers = [];

    public void ScheduleMineJob(ITileView tile)
    {
        if (!tile.HasWall)
            return;

        IIntent mineIntent = new MineBlockIntent(tile);

        _orders.Enqueue(mineIntent);
    }

    public void Tick()
    {
        while (_orders.TryDequeue(out IIntent? intent))
        {
            if (intent is MineBlockIntent mineBlock)
            {
                WorkerTrait? worker = _freeWorkers.MinBy(w => (mineBlock.TargetTile.Position - w.Owner.Position).SimpleLen());
                if (worker == null)
                {
                    _orders.Enqueue(mineBlock);
                    break;
                }

                if (worker.AssignWorkOrder(mineBlock))
                    RevertReportForDuty(worker);
            }
        }
    }

    public void ReportForDuty(WorkerTrait entity)
    {
        _freeWorkers.Add(entity);
    }

    public void RevertReportForDuty(WorkerTrait entity)
    {
        _freeWorkers.Remove(entity);
    }
}
