using ApprenticeManagement.Data;
using ApprenticeManagement.Models;

namespace ApprenticeManagement.Services;

/// <summary>CRUD service for <see cref="Company"/> entities.</summary>
public class CompanyService(DataStore store)
{
    private readonly DataStore _store = store;

    // ─── Create ──────────────────────────────────────────────────────────────

    /// <summary>Adds a new company and persists the change.</summary>
    public Company Create(string name, string address, string phone, string email)
    {
        var company = new Company
        {
            Name    = name.Trim(),
            Address = address.Trim(),
            Phone   = phone.Trim(),
            Email   = email.Trim(),
        };

        _store.Companies.Add(company);
        _store.Save();
        return company;
    }

    // ─── Read ─────────────────────────────────────────────────────────────────

    public IReadOnlyList<Company> GetAll() => _store.Companies.AsReadOnly();

    public Company? GetById(Guid id)
        => _store.Companies.FirstOrDefault(c => c.Id == id);

    // ─── Update ───────────────────────────────────────────────────────────────

    /// <summary>Updates the mutable fields of an existing company.</summary>
    public bool Update(Guid id, string name, string address, string phone, string email)
    {
        var company = GetById(id);
        if (company is null) return false;

        company.Name    = name.Trim();
        company.Address = address.Trim();
        company.Phone   = phone.Trim();
        company.Email   = email.Trim();

        _store.Save();
        return true;
    }

    // ─── Delete ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Deletes a company.  Any apprentice that referenced this company will
    /// have their <c>CompanyId</c> cleared.
    /// </summary>
    public bool Delete(Guid id)
    {
        var company = GetById(id);
        if (company is null) return false;

        // Un-link apprentices.
        foreach (var apprentice in _store.Apprentices.Where(a => a.CompanyId == id))
            apprentice.CompanyId = null;

        _store.Companies.Remove(company);
        _store.Save();
        return true;
    }
}
