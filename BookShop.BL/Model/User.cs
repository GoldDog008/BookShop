using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShop.BL.Model;

[Serializable]
public partial class User
{
    #region Свойства
    public int Id { get; set; }

    /// <summary>
    /// Имя.
    /// </summary>
    public string FirstName { get; set; } = null!;
    /// <summary>
    /// Фамилия.
    /// </summary>
    public string LastName { get; set; } = null!;
    /// <summary>
    /// Номер телефона.
    /// </summary>
    public string? Phone { get; set; }
    /// <summary>
    /// Электронная почта.
    /// </summary>
    /// 
    public string Email { get; set; }
    /// <summary>
    /// Id место жительства.
    /// </summary>
    public int? ResidenceId { get; }
    /// <summary>
    /// Id роли пользователя.
    /// </summary>
    public int RoleId { get; set; }

    public virtual Residence? Residence { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<SalesHistory> SalesHistories { get; } = new List<SalesHistory>();
    #endregion

    /// <summary>
    /// Создать нового пользователя.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="roleId"></param>
    /// <param name="phone"></param>
    /// <param name="residenceId"></param>
    public User(string firstName, string lastName, int roleId, string email, string? phone = null, int? residenceId = null) 
    {
        FirstName = firstName;
        LastName = lastName;
        RoleId = roleId;
        Email = email;
        Phone = phone;
        ResidenceId = residenceId;
    }

    public override string ToString()
    {
        return $"{FirstName.Trim()} {LastName.Trim()}";
    }
}
