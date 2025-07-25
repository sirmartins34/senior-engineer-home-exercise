using System.ComponentModel;

namespace People.Api.ApiModels
{
    public record PersonApi(
        [Description("The person's id.")] int Id,
        [Description("The person's name.")] string Name,
        [Description("The person's date of birth.")] DateOnly DateOfBirth);
}
