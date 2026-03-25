using ApprenticeManagement.Models;
using ApprenticeManagement.Services;

namespace ApprenticeManagement.UI;

/// <summary>Menu for managing <see cref="Company"/> entities.</summary>
public class CompanyMenu(CompanyService companyService)
{
    private readonly CompanyService _companyService = companyService;

    public void Show()
    {
        while (true)
        {
            ConsoleHelper.PrintHeader("Company Management");
            Console.WriteLine("  [1] List all companies");
            Console.WriteLine("  [2] Add company");
            Console.WriteLine("  [3] Edit company");
            Console.WriteLine("  [4] Delete company");
            Console.WriteLine("  [0] Back");
            ConsoleHelper.PrintSeparator();

            int choice = ConsoleHelper.ReadInt("Choice", 0, 4);

            switch (choice)
            {
                case 1: ListCompanies();  break;
                case 2: AddCompany();     break;
                case 3: EditCompany();    break;
                case 4: DeleteCompany();  break;
                case 0: return;
            }
        }
    }

    private void ListCompanies()
    {
        ConsoleHelper.PrintHeader("All Companies");
        var companies = _companyService.GetAll();

        if (companies.Count == 0)
        {
            ConsoleHelper.PrintInfo("No companies found.");
            ConsoleHelper.Pause();
            return;
        }

        foreach (var c in companies)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  {c.Name}");
            Console.ResetColor();
            Console.WriteLine($"    ID:      {c.Id}");
            Console.WriteLine($"    Address: {c.Address}");
            Console.WriteLine($"    Phone:   {c.Phone}");
            Console.WriteLine($"    Email:   {c.Email}");
            ConsoleHelper.PrintSeparator();
        }

        ConsoleHelper.Pause();
    }

    private void AddCompany()
    {
        ConsoleHelper.PrintHeader("Add Company");

        string name    = ConsoleHelper.ReadString("Name");
        string address = ConsoleHelper.ReadString("Address", allowEmpty: true);
        string phone   = ConsoleHelper.ReadString("Phone",   allowEmpty: true);
        string email   = ConsoleHelper.ReadString("Email",   allowEmpty: true);

        var company = _companyService.Create(name, address, phone, email);
        ConsoleHelper.PrintSuccess($"Company '{company.Name}' created (ID: {company.Id}).");
        ConsoleHelper.Pause();
    }

    private void EditCompany()
    {
        ConsoleHelper.PrintHeader("Edit Company");
        var company = SelectCompany();
        if (company is null) return;

        string name    = ConsoleHelper.ReadStringOrDefault("Name",    company.Name);
        string address = ConsoleHelper.ReadStringOrDefault("Address", company.Address);
        string phone   = ConsoleHelper.ReadStringOrDefault("Phone",   company.Phone);
        string email   = ConsoleHelper.ReadStringOrDefault("Email",   company.Email);

        _companyService.Update(company.Id, name, address, phone, email);
        ConsoleHelper.PrintSuccess("Company updated.");
        ConsoleHelper.Pause();
    }

    private void DeleteCompany()
    {
        ConsoleHelper.PrintHeader("Delete Company");
        var company = SelectCompany();
        if (company is null) return;

        Console.Write($"  Are you sure you want to delete '{company.Name}'? (y/N): ");
        string confirm = Console.ReadLine() ?? string.Empty;

        if (confirm.Equals("y", StringComparison.OrdinalIgnoreCase))
        {
            _companyService.Delete(company.Id);
            ConsoleHelper.PrintSuccess("Company deleted.");
        }
        else
        {
            ConsoleHelper.PrintInfo("Deletion cancelled.");
        }

        ConsoleHelper.Pause();
    }

    /// <summary>Displays a numbered list of companies and returns the user's selection.</summary>
    public Company? SelectCompany()
    {
        var companies = _companyService.GetAll();

        if (companies.Count == 0)
        {
            ConsoleHelper.PrintInfo("No companies available.");
            ConsoleHelper.Pause();
            return null;
        }

        var names = companies.Select(c => c.Name).ToList();
        int idx   = ConsoleHelper.ChooseFromList("Select company", names);
        return companies[idx - 1];
    }
}
