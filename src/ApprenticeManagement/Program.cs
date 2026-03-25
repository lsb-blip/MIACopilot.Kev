using ApprenticeManagement.Data;
using ApprenticeManagement.Services;
using ApprenticeManagement.UI;

// ── Bootstrap ────────────────────────────────────────────────────────────────

// Resolve data.json next to the running executable so it is always predictable.
string dataPath = Path.Combine(AppContext.BaseDirectory, "data.json");
var store       = new DataStore(dataPath);

var companyService   = new CompanyService(store);
var trainerService   = new VocationalTrainerService(store);
var apprenticeService = new ApprenticeService(store);

var companyMenu   = new CompanyMenu(companyService);
var trainerMenu   = new VocationalTrainerMenu(trainerService);
var apprenticeMenu = new ApprenticeMenu(apprenticeService, companyService, trainerService);

// ── Main loop ────────────────────────────────────────────────────────────────

while (true)
{
    ConsoleHelper.PrintHeader("Apprentice Management System");
    Console.WriteLine("  [1] Manage Companies");
    Console.WriteLine("  [2] Manage Vocational Trainers");
    Console.WriteLine("  [3] Manage Apprentices");
    Console.WriteLine("  [0] Exit");
    ConsoleHelper.PrintSeparator();

    int choice = ConsoleHelper.ReadInt("Choice", 0, 3);

    switch (choice)
    {
        case 1: companyMenu.Show();    break;
        case 2: trainerMenu.Show();    break;
        case 3: apprenticeMenu.Show(); break;
        case 0:
            ConsoleHelper.PrintSuccess("Goodbye!");
            return;
    }
}
