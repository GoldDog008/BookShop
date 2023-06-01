namespace BookShop.BL.Controller.IValidationData
{
    public interface IBookValidationData
    {
        bool IsNameValid(string name);
        bool IsPriceValid(decimal price);
        bool IsCountValid(int count);
        bool IsAuthorIdValid(int? authorId);
    }
}
