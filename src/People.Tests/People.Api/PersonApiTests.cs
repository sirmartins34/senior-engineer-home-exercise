using Microsoft.EntityFrameworkCore;
using People.Api.ApiModels;
using People.Api.Exceptions;
using People.Api.Services;
using People.Data.Exceptions;
using DataContext = People.Data.Context;

namespace Person.Tests.People.Api
{
    [TestClass]
    sealed public class PeopleApiTest
    {
        private PersonService GetNewService()
        {
            return new PersonService(
                new DataContext.Context(new DbContextOptionsBuilder<DataContext.Context>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options
                )
            );
        }


        private readonly string[] _setupNames = { "Isaac Newton", "Stephen Hawking", "Alan Turing", "Ada Lovelace" };

        private async void SetupAFewPeople(PersonService service)
        {
            foreach (var name in _setupNames)
            {
                var person = new NewPerson() { Name = name, DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 5, 4)) };
                // Act
                var result = await service.AddPersonAsync(person);
            }
        }



        #region Tests - AddPersonAsync
        [DataTestMethod]
        [DataRow(new string[] { "Isaac Newton", "Stephen Hawking", "Alan Turing", "Ada Lovelace" })]
        [DataRow(new string[] { "Isaac Newton" })]
        [DataRow(new string[] { "Isaac Newton", "Stephen Hawking" })]
        public async Task TestAddPersonAsync(string[] names)
        {
            var service = GetNewService();

            int processed = 0;
            foreach (var name in names)
            {
                var person = new NewPerson() { Name = name, DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 5, 4)) };
                // Act
                var result = await service.AddPersonAsync(person);
                processed++;

                // Assert
                Assert.AreEqual(processed, result.Id);
                Assert.AreEqual(name, result.Name);


                // Act
                var tempList = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(processed, tempList.Count());
            }

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.AreEqual(names.Length, list.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'newPerson')")]
        public async Task TestAddPersonAsyncNullParamSent()
        {
            var service = GetNewService();

            NewPerson? person = null;

            try {
                // Act
                var result = await service.AddPersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(0, list.Count());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        [ExpectedException(typeof(InvalidNameException), "Name can't be null, empty or just space characters")]
        public async Task TestAddPersonAsyncInvalidName(string name)
        {
            var service = GetNewService();

            var person = new NewPerson() { Name = name, DateOfBirth = DateOnly.FromDateTime(new DateTime(1990, 5, 4)) };

            try
            {
                // Act
                var result = await service.AddPersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(0, list.Count());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(1899, 12, 31)]
        [DataRow(1543, 3, 20)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person is too old to be alive")]
        public async Task TestAddPersonAsyncInvalidDateOfBirthTooOld(int year, int month, int day)
        {
            var service = GetNewService();

            var person = new NewPerson() { Name = "John Doe", DateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day)) };

            try
            {
                // Act
                var result = await service.AddPersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(0, list.Count());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(2220, 12, 31)]
        [DataRow(2532, 3, 20)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person hasn't born yet...")]
        public async Task TestAddPersonAsyncInvalidDateOfBirthNotBornYet(int year, int month, int day)
        {
            var service = GetNewService();

            var person = new NewPerson() { Name = "John Doe", DateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day)) };

            try
            {
                // Act
                var result = await service.AddPersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(0, list.Count());

                throw;
            }
        }
        #endregion Tests - AddPersonAsync


        #region Tests - UpdatePersonAsync
        [DataTestMethod]
        [DataRow(3, "Michio Kaku", 1947, 1, 24)]
        public async Task TestUpdatePersonAsync(int id, string name, int year, int month, int day)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();


            DateOnly dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            PersonApi person = new PersonApi(id, name, dateOfBirth);

            // Act
            var result = await service.UpdatePersonAsync(person);

            // Assert
            Assert.AreEqual(id, result.Id);
            Assert.AreEqual(name, result.Name);
            Assert.AreEqual(dateOfBirth, result.DateOfBirth);

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.AreEqual(initialListCount, list.Count());

            var personInList = list.SingleOrDefault(p => p.Id == id);
            // Assert
            Assert.IsNotNull(personInList);
            Assert.AreEqual(id, personInList.Id);
            Assert.AreEqual(name, personInList.Name);
            Assert.AreEqual(dateOfBirth, personInList.DateOfBirth);

            CollectionAssert.AreEqual(initialList.Where(p => p.Id != id).ToList(), list.Where(p => p.Id != id).ToList());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Value cannot be null. (Parameter 'updatePerson')")]
        public async Task TestUpdatePersonAsyncNullParamSent()
        {
            var service = GetNewService();

            PersonApi? person = null;

            try
            {
                // Act
                var result = await service.UpdatePersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(0, list.Count());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(50, "Michio Kaku", 1947, 1, 24)]
        [ExpectedException(typeof(InvalidPersonIdException), "Person's id not found")]
        public async Task TestUpdatePersonAsyncInvalidId(int id, string name, int year, int month, int day)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();


            DateOnly dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            PersonApi person = new PersonApi(id, name, dateOfBirth);

            try {
                // Act
                var result = await service.UpdatePersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(initialListCount, list.Count());

                CollectionAssert.AreEqual(initialList.ToList(), list.ToList());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(3, "", 1947, 1, 24)]
        [DataRow(3, "   ", 1947, 1, 24)]
        [DataRow(3, null, 1947, 1, 24)]
        [ExpectedException(typeof(InvalidNameException), "Name can't be null, empty or just space characters")]
        public async Task TestUpdatePersonAsyncInvalidName(int id, string name, int year, int month, int day)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();


            DateOnly dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            PersonApi person = new PersonApi(id, name, dateOfBirth);

            try
            {
                // Act
                var result = await service.UpdatePersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(initialListCount, list.Count());

                CollectionAssert.AreEqual(initialList.ToList(), list.ToList());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(3, "Michio Kaku", 1865, 7, 12)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person is too old to be alive")]
        public async Task TestUpdatePersonAsyncInvalidDateOfBirthTooOld(int id, string name, int year, int month, int day)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();


            DateOnly dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            PersonApi person = new PersonApi(id, name, dateOfBirth);

            try
            {
                // Act
                var result = await service.UpdatePersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(initialListCount, list.Count());

                CollectionAssert.AreEqual(initialList.ToList(), list.ToList());

                throw;
            }
        }

        [DataTestMethod]
        [DataRow(3, "Michio Kaku", 2155, 9, 3)]
        [ExpectedException(typeof(ArgumentOutOfRangeException), "This person hasn't born yet...")]
        public async Task TestUpdatePersonAsyncInvalidDateOfBirthNotBornYet(int id, string name, int year, int month, int day)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();


            DateOnly dateOfBirth = DateOnly.FromDateTime(new DateTime(year, month, day));
            PersonApi person = new PersonApi(id, name, dateOfBirth);

            try
            {
                // Act
                var result = await service.UpdatePersonAsync(person);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(initialListCount, list.Count());

                CollectionAssert.AreEqual(initialList.ToList(), list.ToList());

                throw;
            }
        }
        #endregion Tests - UpdatePersonAsync


        #region Tests - DeletePersonAsync
        [DataTestMethod]
        [DataRow(new int[] { 3 })]
        [DataRow(new int[] { 3, 1 })]
        public async Task TestDeletePersonAsync(int[] ids)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();

            // Act
            foreach (var id in ids)
            {
                await service.DeletePersonAsync(id);
            }

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.AreEqual(initialListCount - ids.Length, list.Count());

            foreach (var id in ids)
            {
                var personInList = list.SingleOrDefault(p => p.Id == id);
                // Assert
                Assert.IsNull(personInList);
            }

            CollectionAssert.AreEqual(initialList.Where(p => !ids.Contains(p.Id)).ToList(), list.ToList());
        }

        [DataTestMethod]
        [DataRow(50)]
        [ExpectedException(typeof(InvalidPersonIdException), "Person's id not found")]
        public async Task TestDeletePersonAsyncInvalidId(int id)
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            var initialList = await service.GetAllAsync();
            var initialListCount = initialList.Count();

            try
            {
                // Act
                await service.DeletePersonAsync(id);
            }
            catch
            {
                // Test that nothing was added

                // Act
                var list = await service.GetAllAsync();

                // Assert
                Assert.AreEqual(initialListCount, list.Count());

                CollectionAssert.AreEqual(initialList.ToList(), list.ToList());

                throw;
            }
        }
        #endregion Tests - DeletePersonAsync


        #region Tests - GetAllAsync
        [TestMethod]
        public async Task TestGetAllAsyncEmptyList()
        {
            var service = GetNewService();

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.AreEqual(0, list.Count());
        }

        [TestMethod]
        public async Task TestGetAllAsync()
        {
            var service = GetNewService();

            SetupAFewPeople(service);

            // Act
            var list = await service.GetAllAsync();

            // Assert
            Assert.AreEqual(_setupNames.Length, list.Count());
        }
        #endregion Tests - GetAllAsync
    }
}
