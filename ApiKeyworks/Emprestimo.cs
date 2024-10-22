using System.Text.Json;

namespace ApiKeyworks
{
    public class Emprestimo
    {
        public int EmprestimoID { get; set; }
        public int LivroID { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        string jsonpathemp = "C:\\Users\\arthu\\source\\repos\\ApiKeyworks\\ApiKeyworks\\Data\\emprestimos.json";

        public Emprestimo(int idLivro, DateTime inicio, DateTime fim)
        {
            EmprestimoID = GerarIDEmp();
            LivroID = idLivro;
            DataInicio = inicio;
            DataFim = fim;
        }

        public Emprestimo() { }

        public static int GerarIDEmp()
        {
            string json;
            string jsonpathemp = "C:\\Users\\arthu\\source\\repos\\ApiKeyworks\\ApiKeyworks\\Data\\emprestimos.json";

            try
            {
                json = File.ReadAllText(jsonpathemp);
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
                List<Emprestimo> emprestimos = JsonSerializer.Deserialize<List<Emprestimo>>(json);
                if (emprestimos == null || emprestimos.Count == 0)
                {
                    return 0;
                }

                return emprestimos.Max(l => l.EmprestimoID) + 1;
            }
            catch (JsonException e)
            {
                Console.WriteLine($"JSON error: {e.Message}");
                return 0;
            }
        }
    }
}
