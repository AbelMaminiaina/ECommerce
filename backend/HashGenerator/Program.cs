class Program
{
    static void Main(string[] args)
    {
        // Hash pour "User123!" et "Admin123!"
        Console.WriteLine("\n🔐 Génération des hash BCrypt...\n");

        var userPassword = "User123!";
        var userHash = BCrypt.Net.BCrypt.HashPassword(userPassword, 11);
        Console.WriteLine($"🔑 Mot de passe: {userPassword}");
        Console.WriteLine($"🔐 Hash: {userHash}\n");

        var adminPassword = "Admin123!";
        var adminHash = BCrypt.Net.BCrypt.HashPassword(adminPassword, 11);
        Console.WriteLine($"🔑 Mot de passe: {adminPassword}");
        Console.WriteLine($"🔐 Hash: {adminHash}\n");
    }
}
