namespace WorldGenerator;

public class NameGenerator
{
    static string[] DwarfNames = { "Urist", "Uzol", "Tun", "Dodok", "Asmel", "Tosid", "Dumat" };
    static string[] DwarfSurnamePrefixes = { "Ustu", "Asmel", "Iton", "Dodok", "Akru", "Tun", "Molda" };
    static string[] DwarfSurnames = { "thoslan", "statir", "degel", "mosus", "risen", "eskal", "vabok" };

    public static String GetDwarfName()
    {

        String DwarfName = DwarfNames[Random.Shared.Next(0, 7)] + " " + DwarfSurnamePrefixes[Random.Shared.Next(0, 7)] + DwarfSurnames[Random.Shared.Next(0, 7)];
        return DwarfName;
    }
}