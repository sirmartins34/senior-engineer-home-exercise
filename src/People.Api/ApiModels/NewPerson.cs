using System.ComponentModel;

namespace People.Api.ApiModels
{
    public class NewPerson
    {
        [Description("The person's name.")]
        public string Name { get; set; } = string.Empty;

        [Description("The person's date of birth.")]
        public DateOnly DateOfBirth { get; set; }
    }
}
