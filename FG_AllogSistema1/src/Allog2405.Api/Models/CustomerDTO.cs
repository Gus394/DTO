using Allog2405.Api.Entities;
namespace Allog2405.Api.Models;

public class CustomerDTO
{
    // id, maybe
    public string FirstName {private get; set;} = string.Empty;
    public string LastName {private get; set;} = string.Empty;
    public string FullName
    {
        get
        {
            return FirstName + " " + LastName;
        }
    }
    public string CPF {get; set;} = string.Empty;

    public CustomerDTO(Customer customer) {
        FirstName = customer.firstName;
        LastName = customer.lastName;
        CPF = customer.cpf;
    }
}