using WorldGenerator.AI;

namespace WorldGenerator.Traits;

public class AITrait : Trait<NullTraitData>
{
    public IScheduler? CurrentScheduler { get; private set; }
    private (IScheduler Scheduler, AssignSchedulerResult Result)? _pendingScheduler;

    public override void Tick()
    {
        if (CurrentScheduler == null)
            return;

        if (CurrentScheduler.State is SchedulerState.New)
        {
            CurrentScheduler.Start();
        }

        CurrentScheduler.Tick();

        if (CurrentScheduler.State is SchedulerState.Completed or SchedulerState.Failed or SchedulerState.Cancelled)
        {
            CurrentScheduler = _pendingScheduler?.Scheduler;
            if (_pendingScheduler != null)
            {
                // Inform any interested parties that assigning this scheduler succeeded
                // after it spent some time in the pending state
                _pendingScheduler.Value.Result.SignalSuccess();
                _pendingScheduler = null;
            }
        }

    }

    public IAssignSchedulerResult AssignScheduler(IScheduler newScheduler)
    {
        // There is no current scheduler - just assign the one we got asked for
        if (CurrentScheduler == null)
        {
            CurrentScheduler = newScheduler;
            CurrentScheduler.Owner = Owner;
            return AssignSchedulerResult.Success();
        }

        // There is a current scheduler and its priority is higher than the new one - don't assign it
        if (CurrentScheduler.Priority >= newScheduler.Priority)
        {
            return AssignSchedulerResult.Fail();
        }

        // The current scheduler's priority is lower than the new one but we've already got a
        // pending scheduler which has higher priority than the new scheduler
        if (_pendingScheduler != null && _pendingScheduler.Value.Scheduler.Priority >= newScheduler.Priority)
        {
            return AssignSchedulerResult.Fail();
        }

        // If we reached this far, we have higher priority than both the current and pending schedulers.
        // If a pending scheduler exists, we can just override it
        if (_pendingScheduler != null)
        {
            // Inform any interested parties that assigning this scheduler failed
            // after it spent some time in the pending state and scheduler with
            // more priority came along
            _pendingScheduler.Value.Result.SignalFailed();

            AssignSchedulerResult newResult = AssignSchedulerResult.Pending();
            _pendingScheduler = (newScheduler, newResult);
            newScheduler.Owner = Owner;
            return newResult;
        }

        // There wasn't a pending scheduler before, so we cancel the current scheduler to "let it know" there's
        // something pending that has more priority than it does and it's time to die
        CurrentScheduler.Cancel();
        if (CurrentScheduler.State is SchedulerState.Cancelled)
        {
            CurrentScheduler = newScheduler;
            CurrentScheduler.Owner = Owner;
            return AssignSchedulerResult.Success();
        }

        // It didn't cancel immediately - means it must do some cleanup action
        // Let's schedule our future scheduler to be set once the old one is done cancelling
        AssignSchedulerResult result = AssignSchedulerResult.Pending();
        _pendingScheduler = (newScheduler, result);
        newScheduler.Owner = Owner;
        return result;
    }
}

