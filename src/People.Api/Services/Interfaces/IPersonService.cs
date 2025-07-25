using People.Api.ApiModels;
using People.Data.Entities;

namespace People.Api.Services.Interfaces
{
    public interface IPersonService
    {
        Task<Person> AddPersonAsync(NewPerson newPerson);
        Task<Person> UpdatePersonAsync(PersonApi person);
        Task DeletePersonAsync(int id);
        Task<IEnumerable<Person>> GetAllAsync();
    }
}
