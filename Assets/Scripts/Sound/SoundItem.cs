using UnityEngine;

[System.Serializable]
public class SoundItem
{

    public enum SoundName
    {
        NoSound,
        TypewriterSound,
        WalkingSound,
        ChipsCrunch,
        TrainWhistle,
        TrainMoving,
        TrainStation,
        Driving,
        OpenFrontDoor,
        WalkingOnWood,
        Boxes,
        Sigh,
        OpenWindow,
        WindGust,
        NotBirthdaySong,
        CandlesBlow,
        OpenShopDoor,
        LampToggle,
        BedtimeYawn,
        DeepSigh,
        DriveByCar,
        LockedDoor,
        KnockOnDoor,
        WakeupAlarm,
        SnoringSounds,
        SmashingGlass,
        BusApproach,
        BusDoorOpen,
        BusDriveAway,
        UncannyLecture,
        AudienceLaughter,
        SchoolBell,
        AirHorn,
    }

    public SoundName soundName;
    public AudioClip[] soundClips;
    public string soundDescription;
    [Range(0.1f, 1.5f)]
    public float soundPitchRandomVariationMin = 0.8f;
    [Range(0.1f, 1.5f)]
    public float soundPitchRandomVariationMax = 1.2f;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
}