using ApprenticeManagement.Models;
using ApprenticeManagement.Services;

namespace ApprenticeManagement.UI;

/// <summary>Menu for managing <see cref="VocationalTrainer"/> entities.</summary>
public class VocationalTrainerMenu(VocationalTrainerService trainerService)
{
    private readonly VocationalTrainerService _trainerService = trainerService;

    public void Show()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("Vocational Trainer Management");
            Console.WriteLine("  [1] List all trainers");
            Console.WriteLine("  [2] Add trainer");
            Console.WriteLine("  [3] Edit trainer");
            Console.WriteLine("  [4] Delete trainer");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 4);

            switch (choice)
            {
                case 1: ListTrainers();  break;
                case 2: AddTrainer();    break;
                case 3: EditTrainer();   break;
                case 4: DeleteTrainer(); break;
                case 0: return;
            }
        }
    }

    private void ListTrainers()
    {
        ConsoleHelper.PrintHeader("All Vocational Trainers");
        var trainers = _trainerService.GetAll();

        if (trainers.Count == 0)
        {
            ConsoleHelper.PrintInfo("No trainers found.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var t in trainers)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {t.FullName}");
            Console.ResetColor();
            Console.WriteLine($"    ID:    {t.Id}");
            Console.WriteLine($"    Phone: {t.Phone}");
            Console.WriteLine($"    Email: {t.Email}");
            ConsoleHelper.PrintSeparator();
        }

        ConsoleHelper.Pause();
    }

    private void AddTrainer()
    {
        ConsoleHelper.PrintHeader("Add Vocational Trainer");

        string firstName = ConsoleHelper.ReadString("First Name");
        string lastName  = ConsoleHelper.ReadString("Last Name");
        string phone     = ConsoleHelper.ReadString("Phone", allowEmpty: true);
        string email     = ConsoleHelper.ReadString("Email", allowEmpty: true);

        var trainer = _trainerService.Create(firstName, lastName, phone, email);
        ConsoleHelper.PrintSuccess($"Trainer '{trainer.FullName}' created (ID: {trainer.Id}).");
        ConsoleHelper.Pause();
    }

    private void EditTrainer()
    {
        ConsoleHelper.PrintHeader("Edit Vocational Trainer");
        var trainer = SelectTrainer();
        if (trainer is null) return;

        string firstName = ConsoleHelper.ReadStringOrDefault("First Name", trainer.FirstName);
        string lastName  = ConsoleHelper.ReadStringOrDefault("Last Name",  trainer.LastName);
        string phone     = ConsoleHelper.ReadStringOrDefault("Phone",      trainer.Phone);
        string email     = ConsoleHelper.ReadStringOrDefault("Email",      trainer.Email);

        _trainerService.Update(trainer.Id, firstName, lastName, phone, email);
        ConsoleHelper.PrintSuccess("Trainer updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteTrainer()
    {
        ConsoleHelper.PrintHeader("Delete Vocational Trainer");
        var trainer = SelectTrainer();
        if (trainer is null) return;

        Console.Write($"  Are you sure you want to delete '{trainer.FullName}'? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _trainerService.Delete(trainer.Id);
            ConsoleHelper.PrintSuccess("Trainer deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    /// <summary>Displays a numbered list of trainers and returns the user's selection.</summary>
    public VocationalTrainer? SelectTrainer()
    {
        var trainers = _trainerService.GetAll();

        if (trainers.Count == 0)
        {
            ConsoleHelper.PrintInfo("No trainers available.");
            ConsoleHelper.Pause();
            return null;
        }

        var names = trainers.Select(t => t.FullName).ToList();
        int idx   = ConsoleHelper.ChooseFromList("Select trainer", names);
        return trainers[idx - 1];
    }
}
