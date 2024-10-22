using System.Linq.Expressions;
using System.Text.Json;

namespace ApiKeyworks
{
    public class Biblioteca
    {
        //public List<Emprestimo> Emprestimos {  get; set; }
        //public List<Livro> Livros { get; set; }

        public void GerarEmprestimo(int IDLivro, DateTime Inicio, DateTime Fim)
        {
            
            var emprestimo = new Emprestimo(IDLivro, Inicio, Fim);
        }


        

    }
}
