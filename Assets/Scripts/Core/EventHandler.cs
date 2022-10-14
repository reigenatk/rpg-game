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
    public static event Action<string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(string dayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(dayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game hour
    public static event Action<string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(string dayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(dayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Advance game day
    public static event Action<string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(string dayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(dayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    // Scene Load Events - in the order they happen

    // Before Scene Unload Fade Out Event
    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if (BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    // Before Scene Unload Event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if (BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    // After Scene Loaded Event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    // After Scene Load Fade In Event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if (AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
}