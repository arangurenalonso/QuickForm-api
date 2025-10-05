
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Person.Domain;
public interface ICountryRepository
{
    Task<CountryDomain?> GetByIdAsync(MasterId masterId);
}
