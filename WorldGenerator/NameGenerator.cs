namespace WorldGenerator;

public static class NameGenerator
{
    private static string[] d_warfNames = { "Urist", "Uzol", "Tun", "Dodok", "Asmel", "Tosid", "Dumat" };
    private static string[] _dwarfSurnamePrefixes = { "Ustu", "Asmel", "Iton", "Dodok", "Akru", "Tun", "Molda" };
    private static string[] _dwarfSurnames = { "thoslan", "statir", "degel", "mosus", "risen", "eskal", "vabok" };
    private static string[] _fortressNamePrefix = { "Tatek", "Anzish", "Idath", "Enor", "Midrim", "Argod", "Musod" };
    private static string[] _fortressNameMain = { "Cenath", "Kuthdeng", "Tosid", "Neth", "Moldath", "Rashgur", "Kogan", "Shadust" };

    public static string GetDwarfName()
    {

        String dwarfName = d_warfNames[Random.Shared.Next(0, d_warfNames.Length)] + " " + _dwarfSurnamePrefixes[Random.Shared.Next(0, _dwarfSurnamePrefixes.Length)] + _dwarfSurnames[Random.Shared.Next(0, _dwarfSurnames.Length)];
        return dwarfName;
    }

    public static string GetFortressName() {

        string fortressName = _fortressNamePrefix[Random.Shared.Next(0, _fortressNamePrefix.Length)] + _fortressNameMain[Random.Shared.Next(0, _fortressNameMain.Length)];
        return fortressName;
    }
}