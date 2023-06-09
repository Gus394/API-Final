using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Univali.Api.DbContexts;
using Univali.Api.Entities;
using Univali.Api.Models;
using Univali.Api.Repositories;

namespace Univali.Api.Controllers;


[Route("api/customers")]
public class CustomersController : MainController
{
    private readonly Data _data;
    private readonly IMapper _mapper;
    private readonly CustomerContext _context;

    private readonly ICustomerRepository _customerRepository;

    public CustomersController(Data data, IMapper mapper, CustomerContext context, ICustomerRepository customerRepository)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
    {
        var customersFromDatabase = await _customerRepository.GetCustomersAsync();
        var customersToReturn = _mapper
            .Map<IEnumerable<CustomerDto>>(customersFromDatabase);

        return Ok(customersToReturn);
    } 

    [HttpGet("{customerId}", Name = "GetCustomerById")]
    public ActionResult<CustomerDto> GetCustomerById(int customerId)
    {
        var customerFromDatabase = _customerRepository.GetCustomerById(customerId);

        if (customerFromDatabase == null) return NotFound();

        var customerToReturn = _mapper.Map<CustomerDto>(customerFromDatabase);

        return Ok(customerToReturn);
    }

    [HttpGet("cpf/{cpf}")]
    public ActionResult<CustomerDto> GetCustomerByCpf(string cpf)
    {
        var customerFromDatabase = _context.Customers // _data
            .FirstOrDefault(c => c.Cpf == cpf);

        if (customerFromDatabase == null)
        {
            return NotFound();
        }

        var customerToReturn = _mapper.Map<CustomerDto>(customerFromDatabase);

        /*CustomerDto customerToReturn = new CustomerDto
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };*/

        return Ok(customerToReturn);
    }

    [HttpPost]
    public ActionResult<CustomerDto> CreateCustomer(
        CustomerForCreationDto customerForCreationDto)
    {
        var customerEntity = _mapper.Map<Customer>(customerForCreationDto);

        _context.Customers.Add(customerEntity);
        _context.SaveChanges();

        var customerToReturn = _mapper.Map<CustomerDto>(customerEntity);

        return CreatedAtRoute
        (
            "GetCustomerById",
            new { id = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("{id}")]
    public ActionResult UpdateCustomer(int id,
        CustomerForUpdateDto customerForUpdateDto)
    {
        if (id != customerForUpdateDto.Id) return BadRequest();

        var customerFromDatabase = _context.Customers // _data
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        _mapper.Map(customerForUpdateDto, customerFromDatabase);
        _context.SaveChanges(); //

        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteCustomer(int id)
    {
        var customerFromDatabase = _context.Customers // _data
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        _context.Customers.Remove(customerFromDatabase); //
        _context.SaveChanges();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public ActionResult PartiallyUpdateCustomer(
        [FromBody] JsonPatchDocument<CustomerForPatchDto> patchDocument,
        [FromRoute] int id)
    {
        var customerFromDatabase = _context.Customers // _data
            .FirstOrDefault(customer => customer.Id == id);

        if (customerFromDatabase == null) return NotFound();

        /*var customerToPatch = new CustomerForPatchDto
        {
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf
        };*/

        var customerToPatch = _mapper.Map<CustomerForPatchDto>(customerFromDatabase); // ?

        patchDocument.ApplyTo(customerToPatch, ModelState);

        if(!TryValidateModel(customerToPatch))
        {
            return ValidationProblem(ModelState);
        }

        customerFromDatabase.Name = customerToPatch.Name;
        customerFromDatabase.Cpf = customerToPatch.Cpf;

        _context.SaveChanges();

        return NoContent();
    }

    [HttpGet("with-addresses")]
    public ActionResult<IEnumerable<CustomerWithAddressesDto>> GetCustomersWithAddresses()
    {
        // Include faz parte do pacote Microsoft.EntityFrameworkCore, precisa importar
        // using Microsoft.EntityFrameworkCore;
        var customersFromDatabase = _context.Customers.Include(c => c.Addresses).ToList();

        // Mapper faz o mapeamento do customer e do address
        // Configure o profile
        // CreateMap<Entities.Customer, Models.CustomerWithAddressesDto>();
        // CreateMap<Entities.Address, Models.AddressDto>();
        var customersToReturn = _mapper.Map<IEnumerable<CustomerWithAddressesDto>>(customersFromDatabase);

        return Ok(customersToReturn);
    }

    [HttpGet("with-addresses/{customerId}", Name = "GetCustomerWithAddressesById")]
    public ActionResult<CustomerWithAddressesDto> GetCustomerWithAddressesById(int customerId)
    {
        var customerFromDatabase = _context // _data
            .Customers.FirstOrDefault(c => c.Id == customerId);

        //customerFromDatabase.Addresses.ToList(); //= _context.Customers.Include(c => c.Addresses).ToList(); //
        
        if (customerFromDatabase == null) return NotFound();

        _context.Customers.Include(c => c.Addresses).ToList();
        var customerToReturn = _mapper.Map<CustomerWithAddressesDto>(customerFromDatabase); // IEnumerable, customerFromDatabase

        /*var addressesDto = customerFromDatabase
            .Addresses.Select(address =>
            new AddressDto
            {
                Id = address.Id,
                City = address.City,
                Street = address.Street
            }
        ).ToList();

        var customerToReturn = new CustomerWithAddressesDto
        {
            Id = customerFromDatabase.Id,
            Name = customerFromDatabase.Name,
            Cpf = customerFromDatabase.Cpf,
            Addresses = addressesDto
        };*/

        return Ok(customerToReturn);
    }

    [HttpPost("with-addresses")]
    public ActionResult<CustomerWithAddressesDto> CreateCustomerWithAddresses(
       CustomerWithAddressesForCreationDto customerWithAddressesForCreationDto)
    {
        /*var maxAddressId = _context.Customers // _data
            .SelectMany(c => c.Addresses).Max(c => c.Id);

        List<Address> AddressesEntity = customerWithAddressesForCreationDto.Addresses
            .Select(address =>
                new Address
                {
                    Id = ++maxAddressId,
                    Street = address.Street,
                    City = address.City
                }).ToList();

        /*var customerEntity = new Customer
        {
            Id = _data.Customers.Max(c => c.Id) + 1, 
            Name = customerWithAddressesForCreationDto.Name,
            Cpf = customerWithAddressesForCreationDto.Cpf,
            Addresses = AddressesEntity 
        };*/

        var customerEntity = _mapper.Map<Customer>(customerWithAddressesForCreationDto);

        _context.Customers.Add(customerEntity); // _data
        _context.SaveChanges();

        var customerToReturn = _mapper.Map<CustomerDto>(customerEntity);

        /*List<AddressDto> addressesDto = customerEntity.Addresses
            .Select(address =>
                new AddressDto
                {
                    Id = address.Id,
                    Street = address.Street,
                    City = address.City
                }).ToList();

        var customerToReturn = new CustomerWithAddressesDto
        {
            Id = customerEntity.Id,
            Name = customerEntity.Name,
            Cpf = customerEntity.Cpf,
            Addresses = addressesDto
        };*/

        return CreatedAtRoute
        (
            "GetCustomerWithAddressesById",
            new { customerId = customerToReturn.Id },
            customerToReturn
        );
    }

    [HttpPut("with-addresses/{customerId}")]
    public ActionResult UpdateCustomerWithAddresses(int customerId,
       CustomerWithAddressesForUpdateDto customerWithAddressesForUpdateDto)
    {
        if (customerId != customerWithAddressesForUpdateDto.Id) return BadRequest();

        var customerFromDatabase = _context.Customers // _data
            .FirstOrDefault(c => c.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        _mapper.Map(customerWithAddressesForUpdateDto, customerFromDatabase);
        _context.SaveChanges();

        /*customerFromDatabase.Name = customerWithAddressesForUpdateDto.Name;
        customerFromDatabase.Cpf = customerWithAddressesForUpdateDto.Cpf;

        var maxAddressId = _data.Customers
            .SelectMany(c => c.Addresses)
            .Max(c => c.Id);

        customerFromDatabase.Addresses = customerWithAddressesForUpdateDto
                                        .Addresses.Select(
                                            address =>
                                            new Address()
                                            {
                                                Id = ++maxAddressId,
                                                City = address.City,
                                                Street = address.Street
                                            }
                                        ).ToList();*/
        
        return NoContent();
    }
}