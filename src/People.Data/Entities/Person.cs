using People.Data.Exceptions;


namespace People.Data.Entities
{
    public class Person
    {
        public int Id { get; internal set; }
        public string Name { get; internal set; } = string.Empty;
        public DateOnly DateOfBirth { get; internal set; }

        public Person(int id, string name, DateOnly dateOfBirth) {
            Id = id;
            SetName(name);
            SetDateOfBirth(dateOfBirth);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidNameException("Name can't be null, empty or just space characters");
            }
            // Split by whitespace, remove empty entries, then join with single spaces
            Name = string.Join(" ", name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        public void SetDateOfBirth(DateOnly dateOfBirth)
        {
            if (dateOfBirth < new DateOnly(1900, 1, 1))
            {
                throw new ArgumentOutOfRangeException("This person is too old to be alive");
            }
            if (dateOfBirth > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new ArgumentOutOfRangeException("This person hasn't born yet...");
            }
            DateOfBirth = dateOfBirth;
        }

    }
}
