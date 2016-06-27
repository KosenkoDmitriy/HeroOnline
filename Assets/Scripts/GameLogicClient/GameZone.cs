using System;

public class GameZone
{
    public Guid ServerId { get; set; }
    public string ZoneName { get; set; }
    public string TCPAddress { get; set; }
    public string UDPAddress { get; set; }
    public int ZoneMaxCCU { get; set; }
    public int ZoneCurCCU { get; set; }

    public void InitData(string zoneInfo)
    {
        string[] allFields = zoneInfo.Split('\n');
        if (allFields.Length == 5)
        {
            ZoneName = allFields[0];
            TCPAddress = allFields[1];
            UDPAddress = allFields[2];
            ZoneMaxCCU = Int32.Parse(allFields[3]);
            ZoneCurCCU = Int32.Parse(allFields[4]);
        }
    }
}
