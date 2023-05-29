using System.Text.RegularExpressions;
using Allog2405.Api.Models;
using Allog2405.Api.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Allog2405.Api.Controllers;

[ApiController]
[Route("api/Customers")]
public class CustomersController : ControllerBase {

    //Retorna o status de validação do cpf.
    //Valores de retorno:
    //0 = Sucesso
    //1 = CPF nulo
    //2 = CPF inválido
    //3 = CPF já existente
    private int ValidarCpf(string cpf) {
        string cpfRegexExp = @"^[0-9]{11}$";

        CustomerData _data = CustomerData.Get();

        Regex cpfRegex = new Regex(cpfRegexExp);

        if(cpf == null)
            return 1;
        if(!cpfRegex.Match(cpf).Success)
            return 2;
        foreach(Customer c in _data.listaCustomers)
            if (cpf == c.cpf)
                return 3;
        
        return 0;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CustomerDTO>> GetCustomers() {
        List<Customer> listaCustomers = CustomerData.Get().listaCustomers;
        List<CustomerDTO> listaCustomersResult = new List<CustomerDTO>();
        foreach(Customer c in listaCustomers) {
            listaCustomersResult.Add(new CustomerDTO(c));
        }
        return Ok(listaCustomersResult);
    }

    [HttpGet("{id:int:min(1)}", Name = "GetCustomerPorId")]
    public ActionResult<CustomerDTO> GetCustomerPorId(int id) {
        Customer? Customer = CustomerData.Get().listaCustomers.FirstOrDefault(c => c.id == id, null);
        if (Customer == null) return NotFound();

        CustomerDTO CustomerResult = new CustomerDTO(Customer);
        return Ok(CustomerResult);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDTO> GetCustomerPorCpf(string cpf) {
        Customer? Customer = CustomerData.Get().listaCustomers.FirstOrDefault(c => c.cpf == cpf, null);
        if(Customer == null) return NotFound();

        CustomerDTO CustomerResult = new CustomerDTO(Customer);
        return Ok(CustomerResult);
    }

    [HttpPost]
    public ActionResult CreateCustomer(CustomerDTO CustomerBody) {
        CustomerData _data = CustomerData.Get();

        int cpfValidacao = ValidarCpf(CustomerBody.CPF);
        switch(cpfValidacao) {
            case 1: return BadRequest("BADREQUEST: CPF é nulo.");
            case 2: return BadRequest("BADREQUEST: CPF é inválido.");
            case 3: return Conflict("CONFLITO: CPF já utilizado.");
        }
        CustomerBody.FirstName ??= String.Empty; // ?
        CustomerBody.LastName ??= String.Empty;

        Customer newCustomer = new Customer {
            id = _data.listaCustomers.Max(c => c.id) + 1,
            firstName = CustomerBody.FirstName,
            lastName = CustomerBody.LastName,
            cpf = CustomerBody.CPF
        };

        _data.listaCustomers.Add(newCustomer);

        return CreatedAtRoute(
            "GetCustomerPorId",
            new {id = newCustomer.id},
            newCustomer
        );
    }

    [HttpPut("{id}")]
    public ActionResult EditCustomer(int id, CustomerDTO CustomerBody) {
        CustomerData _data = CustomerData.Get();

        Customer? Customer = _data.listaCustomers.FirstOrDefault(n => n.id == id, null);
        if(Customer == null) return NotFound();

        if(CustomerBody.FirstName != null) Customer.firstName = CustomerBody.FirstName;
        if(CustomerBody.LastName != null) Customer.lastName = CustomerBody.LastName;
        if(CustomerBody.CPF != null) {
            int cpfValidacao = ValidarCpf(CustomerBody.CPF);
            switch(cpfValidacao) {
                case 2: return BadRequest("BADREQUEST: CPF é inválido.");
                case 3: return Conflict("CONFLITO: CPF já utilizado.");
            }
            Customer.cpf = CustomerBody.CPF;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult<Customer> DeleteCustomerPorId(int id) {
        CustomerData _data = CustomerData.Get();

        Customer? Customer = _data.listaCustomers.FirstOrDefault(n => n.id == id, null);
        if (Customer == null) return NotFound();

        _data.listaCustomers.Remove(Customer);
        return NoContent();
    }


}