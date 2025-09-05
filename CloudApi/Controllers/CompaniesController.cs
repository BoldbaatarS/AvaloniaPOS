using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using CloudApi.DTOs;
using CloudApi;
using CloudApi.Utils;


namespace CloudApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly CloudDbContext _db;
    private readonly IMapper _mapper;


    public CompaniesController(CloudDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<CompanyDto>>>> GetAll()
    {
        var companies = await _db.Companies
            .ProjectTo<CompanyDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return Ok(ApiResponse<IEnumerable<CompanyDto>>.Success(HttpContext, companies));
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> GetById(Guid id)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null)
        {
            return Ok(ApiResponse<CompanyDto>.Fail(HttpContext, StatusCodes.Status404NotFound, "Компани олдсонгүй"));
        }

        var dto = _mapper.Map<CompanyDto>(company);
        return Ok(ApiResponse<CompanyDto>.Success(HttpContext, dto));
    }



    [HttpPost]
    public async Task<ActionResult<ApiResponse<CompanyDto>>> Create(CompanyCreateDto dto)
    {
        // Нэр + Регистрийн хослол давхардаж байгаа эсэхийг шалгах
        bool exists = await _db.Companies.AnyAsync(c =>
            c.Name.ToLower() == dto.Name.ToLower() &&
            c.Register == dto.Register);

        if (exists)
        {
            return Ok(ApiResponse<CompanyDto>.Fail(HttpContext, StatusCodes.Status409Conflict, "Ижил нэр ба регистрийн дугаартай компани аль хэдийн бүртгэгдсэн байна."));
        }

        // Утасны дугаар шалгах
        var newPhones = StringHelpers.SplitPhones(dto.Phone);
        if (newPhones.Any())
        {
            var existingPhones = _db.Companies
                .Where(c => c.Phone != null)
                .AsEnumerable()
                .SelectMany(c => StringHelpers.SplitPhones(c.Phone))
                .ToList();

            if (newPhones.Intersect(existingPhones).Any())
            {
                return Ok(ApiResponse<CompanyDto>.Fail(HttpContext, StatusCodes.Status409Conflict, "Оруулсан утасны дугаар өмнө нь бүртгэгдсэн байна."));
            }
        }

        var company = _mapper.Map<Company>(dto);
        _db.Companies.Add(company);
        await _db.SaveChangesAsync();
        var result = _mapper.Map<CompanyDto>(company);
       // return Ok(ApiResponse<CompanyDto>.Success(result));
        return Ok(ApiResponse<CompanyDto>.Success(HttpContext, result));
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, CompanyCreateDto dto)
    {
        var company = await _db.Companies.FindAsync(id);
        if (company == null) return NotFound();

        // Нэр + Регистрийн хослол давхардаж байгаа эсэхийг шалгах
        bool exists = await _db.Companies.AnyAsync(c =>
            c.Id != id &&
            c.Name.ToLower() == dto.Name.ToLower() &&
            c.Register == dto.Register);

        if (exists)
        {
            return Conflict(new { message = "Ижил нэр ба регистрийн дугаартай компани аль хэдийн бүртгэгдсэн байна." });
        }

        // Утасны дугаар шалгах
        var newPhones = StringHelpers.SplitPhones(dto.Phone);
        if (newPhones.Any())
        {
            var existingPhones = _db.Companies
                .Where(c => c.Id != id && c.Phone != null)
                .AsEnumerable()
                .SelectMany(c => StringHelpers.SplitPhones(c.Phone))
                .ToList();

            if (newPhones.Intersect(existingPhones).Any())
            {
                return Conflict(new { message = "Оруулсан утасны дугаар өмнө нь бүртгэгдсэн байна." });
            }
        }

        _mapper.Map(dto, company);
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<CompanyDto>(company));
    }

    

    [HttpDelete("{id}")]
    public IActionResult Delete(Guid id)
    {
        var company = _db.Companies.Find(id);
        if (company == null) return NotFound();

        _db.Companies.Remove(company);
        _db.SaveChanges();
        return NoContent();
    }
}
