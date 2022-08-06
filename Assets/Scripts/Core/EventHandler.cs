using System;
using System.Collections.Generic;


public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool idleLeft, bool idleRight, bool idleUp, bool idleDown);

public static class EventHandler
{
    // Inventory Update Event. Since we don't have so many parameters, using Action is fine
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdateEvent;

    // stuff like the UI for inventory will have to subscribe to this handler
    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> list)
    {
        if (InventoryUpdateEvent != null)
            InventoryUpdateEvent(inventoryLocation, list);
    }

    // Movement Event
    public static event MovementDelegate MovementEvent;

    // Movement Event Call For Publishers, aka people who want to trigger the event (which happens in this method),
    // and thus notify all subscribers
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isRunning, bool isIdle, bool idleLeft, bool idleRight, bool idleUp, bool idleDown)
    {
        if (MovementEvent != null)
            MovementEvent(inputX, inputY, isWalking, isRunning, isIdle, idleLeft, idleRight, idleUp, idleDown);
    }

    // Advance game minute
    public static event Action<int, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent( int gameDay, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameDay, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game hour
    public static event Action<int, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameDay, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent( gameDay,  gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game day
    public static event Action<int, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameDay, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameDay, gameHour, gameMinute, gameSecond);
        }
    }
}