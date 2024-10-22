using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiKeyworks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControllerEmprestimos : Controller
    {
        private readonly ILogger<ControllerEmprestimos> _logger;
        private readonly string jsonpathemp;

        public ControllerEmprestimos(ILogger<ControllerEmprestimos> logger)
        {
            _logger = logger;
            jsonpathemp = Path.Combine("C:", "Users", "arthu", "source", "repos", "ApiKeyworks", "ApiKeyworks", "Data", "emprestimos.json");
        }

        [HttpPost(Name = "PostEmprestimos")]
        public async Task<IActionResult> Post([FromBody] Emprestimo emprestimo)
        {
            if (emprestimo == null)
            {
                return BadRequest("Emprestimo data is null.");
            }

            try
            {
                List<Emprestimo> emprestimos = new List<Emprestimo>();

                if (System.IO.File.Exists(jsonpathemp))
                {
                    string existingJson = await System.IO.File.ReadAllTextAsync(jsonpathemp);
                    if (!string.IsNullOrEmpty(existingJson))
                    {
                        emprestimos = JsonSerializer.Deserialize<List<Emprestimo>>(existingJson) ?? new List<Emprestimo>();
                    }
                }

                emprestimo.EmprestimoID = Emprestimo.GerarIDEmp();
                emprestimos.Add(emprestimo);

                string jsonString = JsonSerializer.Serialize(emprestimos, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(jsonpathemp, jsonString);

                return CreatedAtAction(nameof(Post), new { id = emprestimo.EmprestimoID }, emprestimo);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing emprestimos JSON.");
                return BadRequest("Invalid JSON format.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Emprestimo.");
                return StatusCode(500, "Internal server error.");
            }
        }

    }
}
