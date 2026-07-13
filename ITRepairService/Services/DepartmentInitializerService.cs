using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ITRepairService.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ITRepairService.Services;

public class DepartmentInitializerService : IDepartmentInitializerService
{
    private const string DepartmentSectionStoreRelativePath = "App_Data/department-section-master.json";
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DepartmentInitializerService(AppDbContext dbContext, IWebHostEnvironment webHostEnvironment)
    {
        _dbContext = dbContext;
        _webHostEnvironment = webHostEnvironment;
    }

    /// <summary>
    /// Ensures that a department exists in the master data.
    /// If the department does not exist, it will be added automatically.
    /// </summary>
    /// <param name="departmentName">The department name from AD</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the department was newly added, false if it already existed</returns>
    public async Task<bool> EnsureDepartmentExistsAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        var normalizedDepartment = NormalizeMasterValue(departmentName);

        if (string.IsNullOrWhiteSpace(normalizedDepartment))
        {
            return false;
        }

        var masterData = await LoadDepartmentSectionStoreAsync(cancellationToken);
        var existsInMaster = masterData.Departments
            .Any(name => string.Equals(name, normalizedDepartment, StringComparison.OrdinalIgnoreCase));

        if (existsInMaster)
        {
            // Department already exists in master data
            return false;
        }

        // Check if department exists in user data
        var usedDepartmentNames = await _dbContext.Users
            .AsNoTracking()
            .Select(user => user.Department)
            .ToListAsync(cancellationToken);

        var existsInUsage = usedDepartmentNames
            .Select(NormalizeMasterValue)
            .Any(name => string.Equals(name, normalizedDepartment, StringComparison.OrdinalIgnoreCase));

        if (existsInUsage)
        {
            // Department exists in usage but not in master, add it to master
            masterData.Departments.Add(normalizedDepartment);
            masterData.InactiveDepartments.RemoveAll(name => string.Equals(name, normalizedDepartment, StringComparison.OrdinalIgnoreCase));
            await SaveDepartmentSectionStoreAsync(masterData, cancellationToken);
            return true;
        }

        // Department is completely new, add it to master
        masterData.Departments.Add(normalizedDepartment);
        masterData.InactiveDepartments.RemoveAll(name => string.Equals(name, normalizedDepartment, StringComparison.OrdinalIgnoreCase));
        await SaveDepartmentSectionStoreAsync(masterData, cancellationToken);
        return true;
    }

    private async Task<DepartmentSectionStoreModel> LoadDepartmentSectionStoreAsync(CancellationToken cancellationToken)
    {
        var storePath = GetDepartmentSectionStorePath();
        if (!System.IO.File.Exists(storePath))
        {
            return new DepartmentSectionStoreModel();
        }

        await using var stream = System.IO.File.OpenRead(storePath);
        var data = await JsonSerializer.DeserializeAsync<DepartmentSectionStoreModel>(stream, cancellationToken: cancellationToken);
        if (data is null)
        {
            return new DepartmentSectionStoreModel();
        }

        data.Departments = data.Departments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.Sections = data.Sections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.SectionDepartmentMappings = data.SectionDepartmentMappings
            .Select(item => new SectionDepartmentMappingModel
            {
                Section = NormalizeMasterValue(item.Section),
                Department = NormalizeMasterValue(item.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Section)
            .ToList();

        data.Sections = data.Sections
            .Concat(data.SectionDepartmentMappings.Select(item => item.Section))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveSections = data.InactiveSections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveDepartments = data.InactiveDepartments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        return data;
    }

    private async Task SaveDepartmentSectionStoreAsync(DepartmentSectionStoreModel data, CancellationToken cancellationToken)
    {
        data.Departments = data.Departments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.Sections = data.Sections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.SectionDepartmentMappings = data.SectionDepartmentMappings
            .Select(item => new SectionDepartmentMappingModel
            {
                Section = NormalizeMasterValue(item.Section),
                Department = NormalizeMasterValue(item.Department)
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Section))
            .GroupBy(item => item.Section, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Section)
            .ToList();

        data.Sections = data.Sections
            .Concat(data.SectionDepartmentMappings.Select(item => item.Section))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveSections = data.InactiveSections
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        data.InactiveDepartments = data.InactiveDepartments
            .Select(NormalizeMasterValue)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name)
            .ToList();

        var storePath = GetDepartmentSectionStorePath();
        var directory = Path.GetDirectoryName(storePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        await using var stream = System.IO.File.Create(storePath);
        await JsonSerializer.SerializeAsync(stream, data, new JsonSerializerOptions
        {
            WriteIndented = true
        }, cancellationToken);
    }

    private string GetDepartmentSectionStorePath()
    {
        return Path.Combine(_webHostEnvironment.ContentRootPath, DepartmentSectionStoreRelativePath);
    }

    private static string NormalizeMasterValue(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
    }

    private sealed class DepartmentSectionStoreModel
    {
        public List<string> Departments { get; set; } = new();
        public List<string> Sections { get; set; } = new();
        public List<string> InactiveDepartments { get; set; } = new();
        public List<string> InactiveSections { get; set; } = new();
        public List<SectionDepartmentMappingModel> SectionDepartmentMappings { get; set; } = new();
    }

    private sealed class SectionDepartmentMappingModel
    {
        public string Section { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}