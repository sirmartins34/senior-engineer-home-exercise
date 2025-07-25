using People.Data.Exceptions;
using PeopleData = People.Data.Entities;

namespace Person.Tests.People.Data
{
    [TestClass]
    sealed public class PersonDataTest
    {
        private PeopleData.Person validPerson = new PeopleData.Person(1, "John Doe", DateOnly.FromDateTime(new DateTime(2000, 3, 20)));


        [DataTestMethod]
        [DataRow("Isaac Newton")]
        [DataRow("Stephen Hawking")]
        [DataRow("Alan Turing")]
        [DataRow("Ada Lovelace")]
        public void TestSetName(string name)
        {
            validPerson.SetName(name);
            Assert.AreEqual(name, validPerson.Name);
        }



        [DataTestMethod]
        [DataRow("Isaac     Newton", "Isaac Newton")]
        [DataRow("   Stephen Hawking   ", "Stephen Hawking")]
        [DataRow("   Alan     Turing      ", "Alan Turing")]
        [DataRow("Ada Lovelace", "Ada Lovelace")]
        public void TestSetName(string name, string expectedName)
        {
            validPerson.SetName(name);
            Assert.AreEqual(expectedName, validPerson.Name);
        }

        [DataTestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow("     ")]
        [ExpectedException(typeof(InvalidNameException), "Name can't be null, empty or just space characters")]
        public void TestSetNameException(string name)
        {
            validPerson.SetName(name);
        }


        [DataTestMethod]
        [DataRow(2000, 3, 20)]
        [DataRow(1997, 9, 7)]
        public void TestSetDateOfBirth(int year, int month, int day)
        {
            var dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            validPerson.SetDateOfBirth(dateOfBirth);
            Assert.AreEqual(dateOfBirth, validPerson.DateOfBirth);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person is too old to be alive")]
        public void TestSetDateOfBirthTooOldException()
        {
            var dateOfBirth = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            validPerson.SetDateOfBirth(dateOfBirth);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person hasn't born yet...")]
        public void TestSetDateOfBirthHaventBornException()
        {
            var dateOfBirth = DateOnly.FromDateTime(new DateTime(1899, 12, 31));
            validPerson.SetDateOfBirth(dateOfBirth);
        }
    }
}
