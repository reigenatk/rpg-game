using System.Collections.Generic;

// this is a custom comparator class used to pass into SortedSet's constructor
public class NPCScheduleEventSort : IComparer<NPCScheduleEvent>
{

    // this class has to decide which schedules should be priotized over others.
    // Check if they are the same time, and then if they are compare priorities.
    public int Compare(NPCScheduleEvent npcScheduleEvent1, NPCScheduleEvent npcScheduleEvent2)
    {
        if (npcScheduleEvent1?.Time == npcScheduleEvent2?.Time)
        {
            if (npcScheduleEvent1?.priority < npcScheduleEvent2?.priority)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        else if (npcScheduleEvent1?.Time > npcScheduleEvent2?.Time)
        {
            return 1;
        }
        else if (npcScheduleEvent1?.Time < npcScheduleEvent2?.Time)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}