using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ErrorOr;

public static class ModelStateDictionaryExtensions
{
    public static ModelStateDictionary AddErrors(
        this ModelStateDictionary modelStateDictionary,
        List<Error> errors)
    {
        foreach (var error in errors)
        {
            modelStateDictionary.AddModelError(error.Code, error.Description);
        }

        return modelStateDictionary;
    }
}
