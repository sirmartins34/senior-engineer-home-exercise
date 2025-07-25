using People.Api.ApiModels;
using People.Data.Entities;

namespace People.Api.Mappings
{
    public static class PersonMapping
    {
        public static Person PersonApiToDBPerson(PersonApi person)
        {
            return new Person(person.Id, person.Name, person.DateOfBirth);
        }

        public static PersonApi DBPersonToPersonApi(Person person)
        {
            return new PersonApi(person.Id, person.Name, person.DateOfBirth);
        }

        public static IEnumerable<PersonApi> DBPersonToPersonApi(IEnumerable<Person> person)
        {
            return person.Select(p => DBPersonToPersonApi(p));
        }
    }
}
