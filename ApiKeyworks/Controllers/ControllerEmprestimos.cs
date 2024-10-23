using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ApiKeyworks.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControllerEmprestimos : ControllerBase
    {
        private readonly ILogger<ControllerEmprestimos> _logger;
        private readonly string _jsonPathEmp;
        private readonly string _jsonPathLivros;

        public ControllerEmprestimos(ILogger<ControllerEmprestimos> logger)
        {
            _logger = logger;
            _jsonPathEmp = Path.Combine("C:", "Users", "arthu", "source", "repos", "ApiKeyworks", "ApiKeyworks", "Data", "emprestimos.json");
            _jsonPathLivros = Path.Combine("C:", "Users", "arthu", "source", "repos", "ApiKeyworks", "ApiKeyworks", "Data", "livros.json");
        }

        private (bool IsValid, string ErrorMessage) ValidarLivro(int livroID)
        {
            if (!System.IO.File.Exists(_jsonPathLivros))
            {
                return (false, "Livros file not found.");
            }

            string json;
            try
            {
                json = System.IO.File.ReadAllText(_jsonPathLivros);
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "I/O error while reading the file: {Path}", _jsonPathLivros);
                return (false, "Internal server error while reading file.");
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return (false, "No existing books found.");
            }

            List<Livro> livros;
            try
            {
                livros = JsonSerializer.Deserialize<List<Livro>>(json) ?? new List<Livro>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error for file: {Path}", _jsonPathLivros);
                return (false, "Invalid JSON format.");
            }

            bool exists = livros.Any(l => l.Id == livroID);
            return exists ? (true, string.Empty) : (false, "This book doesn't exist.");
        }

        private (bool IsValid, string ErrorMessage) ValidarEmprestimo(Emprestimo emprestimo)
        {
            var livroValidation = ValidarLivro(emprestimo.LivroID);
            if (!livroValidation.IsValid)
            {
                return (false, livroValidation.ErrorMessage);
            }

            if (!System.IO.File.Exists(_jsonPathEmp))
            {
                return (true, string.Empty);
            }

            string json;
            try
            {
                json = System.IO.File.ReadAllText(_jsonPathEmp);
                if (string.IsNullOrWhiteSpace(json))
                {
                    return (false, "Emprestimos file is empty.");
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "I/O error while reading the file: {Path}", _jsonPathEmp);
                return (false, "Internal server error while reading file.");
            }

            List<Emprestimo> emprestimos;
            try
            {
                emprestimos = JsonSerializer.Deserialize<List<Emprestimo>>(json) ?? new List<Emprestimo>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error for file: {Path}", _jsonPathEmp);
                return (false, "Invalid JSON format.");
            }

            bool exists = emprestimos.Any(e => e.LivroID == emprestimo.LivroID);
            return exists ? (false, "This loan already exists.") : (true, string.Empty);
        }

        private async Task<List<T>> ReadJsonAsync<T>(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return new List<T>();
            }

            string json = await System.IO.File.ReadAllTextAsync(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                _logger.LogWarning("The file {Path} is empty.", path);
                return new List<T>();
            }

            return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
        }

        private async Task WriteJsonAsync<T>(string path, List<T> data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(path, jsonString);
        }

        [HttpPost(Name = "PostEmprestimos")]
        public async Task<IActionResult> Post([FromBody] Emprestimo emprestimo)
        {
            if (emprestimo == null)
            {
                return BadRequest("Emprestimo data is null.");
            }

            var validation = ValidarEmprestimo(emprestimo);
            if (!validation.IsValid)
            {
                return BadRequest(validation.ErrorMessage);
            }

            try
            {
                List<Emprestimo> emprestimos = await ReadJsonAsync<Emprestimo>(_jsonPathEmp);
                emprestimo.EmprestimoID = Emprestimo.GerarIDEmp();
                emprestimos.Add(emprestimo);
                await WriteJsonAsync(_jsonPathEmp, emprestimos);
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

        [HttpGet(Name = "GetEmprestimos")]
        public async Task<ActionResult<IEnumerable<Emprestimo>>> Get()
        {
            if (!System.IO.File.Exists(_jsonPathEmp))
            {
                return NotFound("Emprestimos file not found.");
            }

            string existingJson;
            try
            {
                existingJson = await System.IO.File.ReadAllTextAsync(_jsonPathEmp);
                if (string.IsNullOrEmpty(existingJson))
                {
                    return Ok(new List<Emprestimo>());
                }
            }
            catch (IOException ex)
            {
                _logger.LogError(ex, "I/O error while reading the file: {Path}", _jsonPathEmp);
                return StatusCode(500, "Internal server error while reading file.");
            }

            List<Emprestimo> emprestimos;
            try
            {
                emprestimos = JsonSerializer.Deserialize<List<Emprestimo>>(existingJson) ?? new List<Emprestimo>();
                return Ok(emprestimos);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON deserialization error.");
                return BadRequest("Invalid JSON format.");
            }
        }
    }
}
