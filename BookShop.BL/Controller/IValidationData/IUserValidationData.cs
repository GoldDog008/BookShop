namespace BookShop.BL.Controller.IValidationData
{
    public interface IUserValidationData
    {
        bool IsFirstNameValid(string firstName);
        bool IsLastNameValid(string lastName);
        bool IsRoleValid(int role);
        bool IsEmailValid(string email, bool isNewUser);
        bool IsPhoneValid(string phone);
        bool IsUserIdValid(int userId);
    }
}
