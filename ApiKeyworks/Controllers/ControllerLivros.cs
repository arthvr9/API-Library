using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ApiKeyworks.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class ControllerLivros : ControllerBase
    {
        private readonly ILogger<ControllerLivros> _logger;
        private readonly string jsonpathlivros;
        
        public ControllerLivros(ILogger<ControllerLivros> logger)
        {
            _logger = logger;
            jsonpathlivros = Path.Combine("C:", "Users", "arthu", "source", "repos", "ApiKeyworks", "ApiKeyworks", "Data", "livros.json");
        }


        [HttpPost(Name = "PostLivro")]
        public async Task<IActionResult> Post([FromBody] Livro livro)
        {
            if (livro == null)
            {
                return BadRequest("Livro == null");
            }

            try
            {
                List<Livro> livros = new List<Livro>();

                if (System.IO.File.Exists(jsonpathlivros))
                {
                    string existingJson = await System.IO.File.ReadAllTextAsync(jsonpathlivros);
                    if (!string.IsNullOrEmpty(existingJson))
                    {
                        livros = JsonSerializer.Deserialize<List<Livro>>(existingJson) ?? new List<Livro>();
                    }
                }

                livro.Id = Livro.GerarIDLivro();
                livros.Add(livro);

                string jsonString = JsonSerializer.Serialize(livros, new JsonSerializerOptions { WriteIndented = true });
                await System.IO.File.WriteAllTextAsync(jsonpathlivros, jsonString);

                return CreatedAtAction(nameof(Post), livro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving Livro.");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet(Name = "GetLivros")]

        public ActionResult<IEnumerable<string>> Get()
        {
            List<Livro> livros = new List<Livro>();

            if (System.IO.File.Exists(jsonpathlivros))
            {
                string existingJson = System.IO.File.ReadAllText(jsonpathlivros);
                if(!string.IsNullOrEmpty(existingJson))
                {
                    livros = JsonSerializer.Deserialize<List<Livro>>(existingJson) ?? new List<Livro>();
                }
                return Ok(livros);
            }
            return BadRequest();
        }
    }
}
