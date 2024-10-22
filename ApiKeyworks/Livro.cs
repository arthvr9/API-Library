using System.Text.Json;

namespace ApiKeyworks
{
    public class Livro
    {
        public int Id { get; set; }
        public string ISBN { get; set; }
        public string Nome { get; set; }
        public string Autor { get; set; }

        public Livro(string iSBN, string nome, string autor)
        {
            ISBN = iSBN;
            Nome = nome;
            Autor = autor;
            Id = GerarIDLivro();
        }
        public Livro() { }

        public static int GerarIDLivro()
        {
            string json;
            string jsonpathlivros = "C:\\Users\\arthu\\source\\repos\\ApiKeyworks\\ApiKeyworks\\Data\\livros.json";

            try
            {
                json = File.ReadAllText(jsonpathlivros);
            }
            catch (FileNotFoundException)
            {
                return 0;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"I/O error: {ex.Message}");
                return 0;
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                return 0;
            }

            try
            {
                List<Livro> livros = JsonSerializer.Deserialize<List<Livro>>(json);
                if (livros == null || livros.Count == 0)
                {
                    return 0;
                }

                return livros.Max(l => l.Id) + 1;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON error: {e.Message}");
                return 0;
            }
        }




    }
}
