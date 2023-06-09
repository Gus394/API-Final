using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Univali.Api.DbContexts;
using Univali.Api.Entities;
using Univali.Api.Models;

namespace Univali.Api.Controllers;


[Route("api/customers/{customerId}/addresses")]
public class AddressesController : MainController
{
    private readonly Data _data;
    private readonly IMapper _mapper;
    private readonly CustomerContext _context;

    public AddressesController(Data data, IMapper mapper, CustomerContext context)
    {
        _data = data ?? throw new ArgumentNullException(nameof(data));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpGet]
    public ActionResult<IEnumerable<AddressDto>> GetAddresses(int customerId)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        var addressesToReturn = new List<AddressDto>();

        foreach (var address in customerFromDatabase.Addresses)
        {
            addressesToReturn.Add(new AddressDto
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City
            });
        }
        return Ok(addressesToReturn);
    }

    [HttpGet("{addressId}", Name = "GetAddress")]
    public ActionResult<AddressDto> GetAddress(int customerId, int addressId)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == customerId);

        if (customerFromDatabase == null) return NotFound();


        var addressFromDatabase = customerFromDatabase.Addresses
            .FirstOrDefault(address => address.Id == addressId);

        if (addressFromDatabase == null) return NotFound();

        var addressToReturn = new AddressDto
        {
            Id = addressFromDatabase.Id,
            Street = addressFromDatabase.Street,
            City = addressFromDatabase.City
        };

        return Ok(addressToReturn);
    }

    [HttpPost]
    public ActionResult<AddressDto> CreateAddress(
       int customerId,
       AddressForCreationDto addressForCreationDto)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(c => c.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        var maxAddressId = _data.Customers
            .SelectMany(c => c.Addresses).Max(a => a.Id);

        var addressEntity = new Address()
        {
            Id = ++maxAddressId,
            Street = addressForCreationDto.Street,
            City = addressForCreationDto.City
        };

        customerFromDatabase.Addresses.Add(addressEntity);

        var addressToReturn = new AddressDto()
        {
            Id = addressEntity.Id,
            City = addressEntity.City,
            Street = addressEntity.Street
        };

        return CreatedAtRoute("GetAddress",
            new
            {
                customerId = customerFromDatabase.Id,
                addressId = addressToReturn.Id
            },
            addressToReturn
        );
    }

    [HttpPut("{addressId}")]
    public ActionResult UpdateAddress(int customerId, int addressId,
       AddressForUpdateDto addressForUpdateDto)
    {
        if (addressForUpdateDto.Id != addressId) return BadRequest();

        // Método Any retorna true ou false.
        // Se customer existe retorna true, se não existe retorna false.
        var customerExists = _context.Customers
            .Any(c => c.Id == customerId);

        if (!customerExists) return NotFound();

        var addressFromDatabase = _context.Addresses
            .FirstOrDefault(a => a.CustomerId == customerId && a.Id == addressId);

        if (addressFromDatabase == null) return NotFound();

        _mapper.Map(addressForUpdateDto, addressFromDatabase);
        _context.SaveChanges();

        return NoContent();
    }

    [HttpDelete("{addressId}")]
    public ActionResult DeleteAddress(int customerId, int addressId)
    {
        var customerFromDatabase = _data.Customers
            .FirstOrDefault(customer => customer.Id == customerId);

        if (customerFromDatabase == null) return NotFound();

        var addressFromDatabase = customerFromDatabase.Addresses
            .FirstOrDefault(address => address.Id == addressId);

        if (addressFromDatabase == null) return NotFound();

        customerFromDatabase.Addresses.Remove(addressFromDatabase);

        return NoContent();
    }
}
