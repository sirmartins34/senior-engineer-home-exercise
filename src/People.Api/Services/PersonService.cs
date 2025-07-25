using Microsoft.EntityFrameworkCore;
using People.Api.ApiModels;
using People.Api.Exceptions;
using People.Api.Services.Interfaces;
using People.Data.Context;
using People.Data.Entities;

namespace People.Api.Services
{
    public class PersonService : IPersonService
    {
        private readonly Context _context;

        public PersonService(Context context)
        {
            _context = context;
        }


        public Task<Person> AddPersonAsync(NewPerson newPerson)
        {
            if (newPerson == null)
                throw new ArgumentNullException(nameof(newPerson));

            // Get next Id
            var id = _context.MyEntities.Any() ? _context.MyEntities.Max(x => x.Id) + 1 : 1;
            var person = new Person(id, newPerson.Name, newPerson.DateOfBirth);

            // save user
            _context.MyEntities.Add(person);
            _context.SaveChangesAsync();

            return Task.FromResult<Person>(person);
        }
        public Task<Person> UpdatePersonAsync(PersonApi updatePerson)
        {
            Person? person;


            if (updatePerson == null)
                throw new ArgumentNullException(nameof(updatePerson));

            // validate
            if ((person = _context.MyEntities.SingleOrDefault(p => p.Id == updatePerson.Id)) == null)
            {
                throw new InvalidPersonIdException("Person's id not found");
            }

            person.SetName(updatePerson.Name);
            person.SetDateOfBirth(updatePerson.DateOfBirth);

            // save user
            _context.MyEntities.Update(person);
            _context.SaveChangesAsync();

            return Task.FromResult(person);
        }
        public Task DeletePersonAsync(int id)
        {
            Person? person;

            // validate
            if ((person = _context.MyEntities.SingleOrDefault(p => p.Id == id)) == null)
            {
                throw new InvalidPersonIdException("Person's id not found");
            }

            // save user
            _context.MyEntities.Remove(person);
            _context.SaveChangesAsync();

            return Task.CompletedTask;
        }
        public async Task<IEnumerable<Person>> GetAllAsync() => await _context.MyEntities.ToListAsync();
    }
}
