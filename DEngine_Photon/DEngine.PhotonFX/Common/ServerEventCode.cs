namespace DEngine.PhotonFX.Common
{
    public enum ServerEventCode : byte
    {
        Default = 0,
        RoomEvent,
        ZoneEvent,
        WorldEvent,
        MasterEvent,
        SlaveEvent,
    }
}
