using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace REIstacks.Application.Contracts.Requests;
public class FileUploadRequest
{
    [Required]
    public IFormFile File { get; set; }
}
