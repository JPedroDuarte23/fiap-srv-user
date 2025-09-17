using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FiapSrvUser.Domain.Entities;
using FiapSrvUser.Domain.Enums;

namespace FiapSrvUser.Application.DTOs;

[ExcludeFromCodeCoverage]
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime BornDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserRole Role { get; set; }
    public string UserName { get; set; }
    public string PublisherName {  get; set; }
    public string Document { get; set; }
    public string Afiliation { get; set; }

    public UserDto(Player player)
    {
        Id = player.Id;
        Name = player.Name;
        Email = player.Email;
        BornDate = player.BornDate;
        CreatedAt = player.CreatedAt;
        Role = player.Role;
        UserName = player.Username;
        Document = player.Cpf;
    }
    public UserDto(Publisher publisher)
    {
        Id = publisher.Id;
        Name = publisher.Name;
        BornDate = publisher.BornDate;
        CreatedAt = publisher.CreatedAt;
        Role = publisher.Role;
        UserName = publisher.Username;
        PublisherName = publisher.PublisherName;
        Document = publisher.Cnpj;
        Afiliation = publisher.CompanyName;
    }

}
