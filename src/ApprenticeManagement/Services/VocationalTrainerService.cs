using ApprenticeManagement.Data;
using ApprenticeManagement.Models;

namespace ApprenticeManagement.Services;

/// <summary>CRUD service for <see cref="VocationalTrainer"/> entities.</summary>
public class VocationalTrainerService(DataStore store)
{
    private readonly DataStore _store = store;

    // ─── Create ──────────────────────────────────────────────────────────────

    public VocationalTrainer Create(string firstName, string lastName, string phone, string email)
    {
        var trainer = new VocationalTrainer
        {
            FirstName = firstName.Trim(),
            LastName  = lastName.Trim(),
            Phone     = phone.Trim(),
            Email     = email.Trim(),
        };

        _store.VocationalTrainers.Add(trainer);
        _store.Save();
        return trainer;
    }

    // ─── Read ─────────────────────────────────────────────────────────────────

    public IReadOnlyList<VocationalTrainer> GetAll() => _store.VocationalTrainers.AsReadOnly();

    public VocationalTrainer? GetById(Guid id)
        => _store.VocationalTrainers.FirstOrDefault(t => t.Id == id);

    // ─── Update ───────────────────────────────────────────────────────────────

    public bool Update(Guid id, string firstName, string lastName, string phone, string email)
    {
        var trainer = GetById(id);
        if (trainer is null) return false;

        trainer.FirstName = firstName.Trim();
        trainer.LastName  = lastName.Trim();
        trainer.Phone     = phone.Trim();
        trainer.Email     = email.Trim();

        _store.Save();
        return true;
    }

    // ─── Delete ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Deletes a trainer.  Apprentices linked to this trainer have their
    /// <c>VocationalTrainerId</c> cleared.
    /// </summary>
    public bool Delete(Guid id)
    {
        var trainer = GetById(id);
        if (trainer is null) return false;

        foreach (var apprentice in _store.Apprentices.Where(a => a.VocationalTrainerId == id))
            apprentice.VocationalTrainerId = null;

        _store.VocationalTrainers.Remove(trainer);
        _store.Save();
        return true;
    }
}
