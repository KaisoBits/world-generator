using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorldGenerator;

public class ConsoleInterface
{
    public void StartDisplayingEvents()
    {
        var listBottom = EventBus.EventList.TakeLast(20);
        foreach (var item in listBottom)
            Console.WriteLine(item);

        Subscription sub = EventBus.Subscribe<GameEvent>(ge => Console.WriteLine(ge));
    }
}
