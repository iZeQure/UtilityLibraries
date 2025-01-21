
var util = args[0];

switch (util.Trim().ToLower())
{
    case "totp":
        Console.WriteLine(TotpLibrary.Totp.GenerateFromSeed(args[1].Trim()));
        break;
    default:
		throw new ArgumentNullException(nameof(args), "Util not supported.");
}
